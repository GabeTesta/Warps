using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using Warps.Curves;
using System.Drawing;

namespace Warps.Panels
{
	[System.Diagnostics.DebuggerDisplay("{Label} {Corners}", Name = "{Label}", Type = "{GetType()}")]
	public class Panel : IRebuild
	{
		//public Panel()
		//{
		//	Locked = true;
		//	m_seams = new MouldCurve[2];
		//}
		//public Panel(MouldCurve[] seams, List<SeamSegment>[] ends)
		//{
		//	Locked = true;
		//	m_seams = seams.Clone() as MouldCurve[];
		//	if( ends != null )
		//		m_sends = ends.Clone() as List<SeamSegment>[];
		//}

		//public Panel(MouldCurve s1, MouldCurve s2, EndSeam e1, EndSeam e2, PanelCorner[] panCorners)
		//{
		//	Locked = true;
		//	m_seams = new MouldCurve[] { s1, s2 };
		//	m_sends = new EndSeam[] { e1, e2 };
		//	Corners = panCorners;
		//}
		public Panel(PanelGroup group, PanelCorner[,] panCorners)
		{
			Locked = true;
			m_group = group;
			Corners = panCorners;
			m_label = m_group.PanLabel();

			m_seams = new IMouldCurve[2];
			//first create 2 seams from the corner curvepoints
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				if (Corners[nEdg, 1].Seams[0] != null && Corners[nEdg, 1].Seams[0] == Corners[nEdg, 2].Seams[0] )//edge curve, use segment
					m_seams[nEdg] = new SegmentCurve(Corners[nEdg, 1].Seams[0], Corners[nEdg, 1].sPos[0], Corners[nEdg, 2].sPos[0]);
					//m_seams[nEdg] = new SegmentCurve(m_group.PanLabel(nEdg), Corners[nEdg, 1].Seams[0], new Vect2(Corners[nEdg, 1].sPos[0], Corners[nEdg, 2].sPos[0]));
				else//internal seam, span girth
					m_seams[nEdg] = new MouldCurve(m_group.PanLabel(nEdg), sail, new IFitPoint[] { Corners[nEdg, 1].GetSeamPoint(1), Corners[nEdg, 2].GetSeamPoint(1) });
			}
				//s2 = new MouldCurve(m_group.PanLabel(1), sail, new IFitPoint[] { Corners[1, 1].GetSeamPoint(1), Corners[1, 2].GetSeamPoint(1) });


			int[] tmesh = new int[] { 4,8 };
			//FlatWidth(tmesh);
			TMeshRBF2(tmesh);
			//TMesh(tmesh);
			Flatten();
		}

		PanelGroup m_group;
		Sail sail { get { return m_group.Sail; } }
		ISurface Mould
		{
			get { return sail.Mould; }
		}

		IMouldCurve[] m_seams;//primary seams, should be 2 
		public IMouldCurve[] Seams
		{
			get { return m_seams; }
		}
		public PanelCorner[,] Corners;//construction corner points, should be 2x4
		
		#region TMESH

		Vect3[,] m_mesh = null;
		/// <summary>
		/// gets the TMESH array size
		/// </summary>
		/// <param name="nDir">0 for cross-seam, 1 for along-seam</param>
		/// <returns>the number of panels in the specified mesh direction</returns>
		int TMESH(int nDir) { return m_mesh.GetLength(nDir); }

		void TMesh(int[] tmesh)
		{
			//construct edge chains
			ChainCurve[] edges = new ChainCurve[2];
			//do aft seam
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				edges[nEdg] = new ChainCurve();

				if (Corners[0, 0] == Corners[1, 0])//non-overlapping end segments, add upper to chain
					edges[nEdg].Add(Corners[nEdg, 0].Seams[1], Corners[nEdg, 0].sPos[1], Corners[nEdg, 1].sPos[1]);

				edges[nEdg].Add(Seams[nEdg], 0, 1);//add mainseam to chain

				if (Corners[0, 3] == Corners[1, 3])//non-overlapping end segments, add lower to chain
					edges[nEdg].Add(Corners[nEdg, 3].Seams[1], Corners[nEdg, 2].sPos[1], Corners[nEdg, 3].sPos[1]);
			}

			//place nodes on edge-kinks
			List<double> kinks = new List<double>();
			kinks.Add(0);
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				if (edges[nEdg].Segments.Count > 2) //check for internal kinks
					for (int i = 1; i < edges[nEdg].Segments.Count - 1; i++)
						kinks.Add(edges[nEdg].Segments[i]);
			}
			kinks.Add(1);
			kinks.Sort();//sort and unique
			kinks = new List<double>(kinks.Distinct());

			List<int> iKinks = new List<int>(kinks.Count);
			//iKinks.Add(0);
			kinks.ForEach(k => iKinks.Add((int)Math.Ceiling(k * ( tmesh[1] + 1))));
			//iKinks.Add(tmesh[1] + 1);
			//int nKink = 1;

			//populate TMESH by interpolating nodes from edge positions
			double s, t;
			Vect2 uv0 = new Vect2(), uv1 = new Vect2(), uvi = new Vect2();
			m_mesh = new Vect3[tmesh[0] + 1, tmesh[1] + 1];
			for (int j = 0; j <= tmesh[1]; j++)//create edge uv/xyz points
			{
				//if (j > iKinks[nKink]) nKink++;//overstepped kink braket, increment into new bracket
				//s = BLAS.interpolate(j - iKinks[nKink - 1], iKinks[nKink] - iKinks[nKink - 1], kinks[nKink], kinks[nKink - 1]);

				s = BLAS.interpolant(j, tmesh[1] + 1);
				m_mesh[0, j] = new Vect3();
				edges[0].xVal(s, ref uv0, ref m_mesh[0, j]);//get lower seam point

				m_mesh[tmesh[0], j] = new Vect3();
				edges[1].xVal(s, ref uv1, ref m_mesh[tmesh[0], j]);//get upper seam point

				for (int i = 0; i < tmesh[0]; i++)
				{
					if (j == 0 && Corners[0, 0] == Corners[1, 1] && Corners[0,1] == Corners[1,0])//overlapping upper edge, place points on edge curve
					{
						t = BLAS.interpolate(i, tmesh[0] + 1, Corners[0, 0].sPos[1], Corners[1, 0].sPos[1]);//interpolate edge curve s position between limits
						Corners[0, 0].Seams[1].uVal(t, ref uvi);
					}
					else if (j == tmesh[1] && Corners[0, 3] == Corners[1, 2] && Corners[0, 2] == Corners[1, 3]) //overlapping lower edge, place points on edge curve
					{
						t = BLAS.interpolate(i, tmesh[0] + 1, Corners[0, 3].sPos[1], Corners[1, 3].sPos[1]);//interpolate edge curve s position between limits
						Corners[1,3].Seams[1].uVal(t, ref uvi);
					}
					else//linear interpolation of uv-coords for internal points
					{
						t = BLAS.interpolant(i, tmesh[0] + 1);
						uvi[0] = BLAS.interpolate(t, uv1[0], uv0[0]);
						uvi[1] = BLAS.interpolate(t, uv1[1], uv0[1]);
					}
					//get mesh xyz from mould and store
					m_mesh[i, j] = new Vect3();
					Mould.xVal(uvi, ref m_mesh[i, j]);
				}
			}
		} 

		double TMeshRBF(int[] tmesh)
		{
			//construct edge chains
			ChainCurve[] edges = new ChainCurve[2];
			//do top 
			edges[0] = new ChainCurve();
			edges[0].Add(Corners[0, 1].Seams[1], Corners[0, 1].sPos[1], Corners[0, 0].sPos[1]);
			if (Corners[0, 0] == Corners[1, 0])
				edges[0].Add(Corners[1, 1].Seams[1], Corners[1, 0].sPos[1], Corners[1, 1].sPos[1]);

			//do bottom 
			edges[1] = new ChainCurve();
			edges[1].Add(Corners[0, 2].Seams[1], Corners[0, 2].sPos[1], Corners[0, 3].sPos[1]);
			if (Corners[0, 3] == Corners[1, 3])
				edges[1].Add(Corners[1, 2].Seams[1], Corners[1, 3].sPos[1], Corners[1, 2].sPos[1]);

			//loop uv and interpolate using RBF st->uv map
			List<double[]> panUV = new List<double[]>(4 * 10);//10 points per edge
			List<double[]> sailUV = new List<double[]>(4 * 10);
			double s;
			Vect2 u = new Vect2();
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				for (int i = 0; i < tmesh[0]; i++)//seam points including corners
				{
					s = BLAS.interpolant(i, tmesh[0]);

					// seam point
					Seams[nEdg].uVal(s, ref u);
					panUV.Add(new double[] { s, nEdg });
					sailUV.Add(u.ToArray());
				}
				for (int i = 1; i < tmesh[0]; i++)//internal edge points
				{
					s = BLAS.interpolant(i, tmesh[0] + 1);

					edges[nEdg].uVal(s, ref u);
					panUV.Add(new double[] { nEdg, s });
					sailUV.Add(u.ToArray());
				}
			}
			RBF.RBFNetwork RBFPan = new RBF.RBFNetwork();
			RBFPan.Fit(panUV, sailUV);

			m_mesh = new Vect3[tmesh[0] + 1, tmesh[1] + 1];
			//populate first and last rows
			for (int j = 0; j <= tmesh[1]; j++)
			{
				s = BLAS.interpolant(j, tmesh[1] + 1);
				m_mesh[0, j] = new Vect3();
				Seams[0].xVal(s, ref u, ref m_mesh[0, j]);

				m_mesh[tmesh[0], j] = new Vect3();
				Seams[1].xVal(s, ref u, ref m_mesh[tmesh[0], j]);

			}
			//populate internal nodes
			Vect2 st = new Vect2();
			for (int i = 1; i < tmesh[0]; i++)
			{
				st[1] = BLAS.interpolant(i, tmesh[0]);
				for (int j = 0; j <= tmesh[1]; j++)
				{
					st[0] = BLAS.interpolant(j, tmesh[1] + 1);
					RBFPan.Value(st.m_vec, ref u.m_vec);
					m_mesh[i, j] = new Vect3();
					Mould.xVal(u, ref m_mesh[i, j]);
				}
			}

			//using nodes create strip via edgelengths

			//rotate/translate strip to align with previous 
			//get max/min and calculate width/bounding box

			return 0;
		}
		double TMeshRBF2(int[] tmesh)
		{
			//construct long-seam chains
			ChainCurve[] edges = new ChainCurve[2];
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				edges[nEdg] = new ChainCurve();

				if (Corners[0, 0] == Corners[1, 0])//non-overlapping end segments, add upper to chain
					edges[nEdg].Add(Corners[nEdg, 0].Seams[1], Corners[nEdg, 0].sPos[1], Corners[nEdg, 1].sPos[1]);

				edges[nEdg].Add(Seams[nEdg], 0, 1);//add mainseam to chain

				if (Corners[0, 3] == Corners[1, 3])//non-overlapping end segments, add lower to chain
					edges[nEdg].Add(Corners[nEdg, 3].Seams[1], Corners[nEdg, 2].sPos[1], Corners[nEdg, 3].sPos[1]);
			}

			//loop uv and interpolate using RBF st->uv map
			List<double[]> panST = new List<double[]>(2 * tmesh[1]);//TMESH points per long-seam
			List<double[]> sailUV = new List<double[]>(2 * tmesh[1]);
			double s;
			Vect2 u = new Vect2();
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				for (int i = 0; i <= tmesh[1]; i++)//seam points including corners
				{
					s = BLAS.interpolant(i, tmesh[1]+1);

					// long-seam point
					edges[nEdg].uVal(s, ref u);
					panST.Add(new double[] { s, nEdg });
					sailUV.Add(u.ToArray());
				}
			}
			RBF.RBFNetwork RBFPan = new RBF.RBFNetwork();
			RBFPan.Fit(panST, sailUV);

			m_mesh = new Vect3[tmesh[0] + 1, tmesh[1] + 1];
			//populate nodes
			Vect2 st = new Vect2();
			for (int i = 0; i <= tmesh[0]; i++)
			{
				st[1] = BLAS.interpolant(i, tmesh[0] + 1);
				for (int j = 0; j <= tmesh[1]; j++)
				{
					st[0] = BLAS.interpolant(j, tmesh[1] + 1);
					RBFPan.Value(st.m_vec, ref u.m_vec);
					m_mesh[i, j] = new Vect3();
					Mould.xVal(u, ref m_mesh[i, j]);
				}
			}

			//using nodes create strip via edgelengths

			//rotate/translate strip to align with previous 
			//get max/min and calculate width/bounding box

			return 0;
		}

		#endregion

		#region Flatten

		Point2D[][,] m_flat = null;
		Vect2 m_width = new Vect2(1e9, -1e9);//track max/min for width
		Vect2 m_length = new Vect2(1e9, -1e9);//track max/min for length

		public double Width
		{ get { return m_width[1] - m_width[0]; } }
		public double Length
		{ get { return m_length[1] - m_length[0]; } }

		void Flatten()
		{
			m_width.Set(1e9, -1e9);//track max/min for width/length
			m_length.Set(1e9, -1e9);//track max/min for width/length
			int nRow, nTri;
			//create 2d strips
			m_flat = new Point2D[m_mesh.GetLength(0) - 1][,];
			//for( nRow = 0; nRow < m_flat.Length; nRow++ )
			//{
			//	m_flat[nRow] = new Point2D[2, m_mesh.GetLength(1)];
			//}

			double A, B, C, theta, alpha, beta, gamma;
			Vect2 vprev = new Vect2();

			//offset to seam start
			//Vect3 xyz = new Vect3();
			//m_seams[0].xVal(0, ref vprev, ref xyz);
			//Transformation tran = new Translation(xyz.x, xyz.y, xyz.z);

			Transformation tran = new Translation(0, 0, 0);
			//reset vprev to be horizontal
			vprev[0] = 1; vprev[1] = 0;

			//for (nRow = 0; nRow < 1; nRow++)
			for (nRow = 0; nRow < m_mesh.GetLength(0) - 1; nRow++)
			{
				m_flat[nRow] = new Point2D[2, m_mesh.GetLength(1)];

				//initial edge corners
				A = m_mesh[nRow, 0].Distance(m_mesh[nRow + 1, 0]);
				m_flat[nRow][0, 0] = new Point2D(0, 0);//start at origin
				m_flat[nRow][1, 0] = new Point2D(0, A);//project first point vertically

				for (nTri = 0; nTri < m_mesh.GetLength(1) - 1; nTri++)
				{
					//lower triangle sides
					//A = m_mesh[nRow, nTri].Distance(m_mesh[nRow + 1, nTri]);//use A from previous triangle
					B = m_mesh[nRow, nTri].Distance(m_mesh[nRow, nTri + 1]);
					C = m_mesh[nRow, nTri + 1].Distance(m_mesh[nRow + 1, nTri]);

					//angle between A and B
					theta = A == 0 || B == 0 ? 0 : (A * A + B * B - C * C) / (2 * A * B);
					Utilities.LimitRange(-1, ref theta, 1);//ensure valid acos
					theta = Math.Acos(theta);//get angle

					//angle between A and horizontal
					alpha = A == 0 ? 1 : (m_flat[nRow][1, nTri].Y - m_flat[nRow][0, nTri].Y) / A;
					Utilities.LimitRange(-1, ref alpha, 1);//ensure valid asin
					alpha = Math.Asin(alpha);

					//angle between A and C
					beta = B * Math.Sin(theta) / C; //law of sines
					Utilities.LimitRange(-1, ref beta, 1);//ensure valid asin
					beta = Math.Asin(beta);//get angle

					theta = alpha - theta;//convert to angle-to-horizontal
					//project forward lower point
					m_flat[nRow][0, nTri + 1] = new Point2D(m_flat[nRow][0, nTri].X + B * Math.Cos(theta), m_flat[nRow][0, nTri].Y + B * Math.Sin(theta));

					//upper triangle sides
					A = m_mesh[nRow + 1, nTri + 1].Distance(m_mesh[nRow, nTri + 1]);
					B = m_mesh[nRow + 1, nTri + 1].Distance(m_mesh[nRow + 1, nTri]);
					//C = m_mesh[nRow, nTri + 1].Distance(m_mesh[nRow + 1, nTri]);common edge

					//angle between C and A'
					//gamma = B == 0 || C == 0 ? 0 : (B * B + C * C - A * A) / (2 * B * C);
					gamma = (B * B + C * C - A * A) / (2 * B * C);
					Utilities.LimitRange(-1, ref gamma, 1);//ensure valid acos
					gamma = Math.Acos(gamma);//get angle

					//angle to horizonal
					theta = alpha + beta + gamma - Math.PI;

					//project off upper point
					m_flat[nRow][1, nTri + 1] = new Point2D(m_flat[nRow][1, nTri].X + B * Math.Cos(theta), m_flat[nRow][1, nTri].Y + B * Math.Sin(theta));
				}
				//calculate strip rotation angle
				Vect2 vstrip = new Vect2(m_flat[nRow][0, nTri].ToArray());
				alpha = vstrip.AngleTo(vprev);
				alpha *= Math.Sign(vstrip.Cross(vprev));
				Transformation rot = new Rotation(alpha, Vector3D.AxisZ);//rotate about z axis
				//rotate each point, then translate
				for (nTri = 0; nTri < m_flat[nRow].GetLength(1); nTri++)
				{
					for (int j = 0; j < 2; j++)
					{
						m_flat[nRow][j, nTri].TransformBy(rot);//rotate about origin
						m_flat[nRow][j, nTri].TransformBy(tran);//translate to previous strip

						//track max/min y values for panel width
						m_width[0] = Math.Min(m_width[0], m_flat[nRow][j, nTri].Y);
						m_width[1] = Math.Max(m_width[1], m_flat[nRow][j, nTri].Y);

						//track max/min x values for panel length
						m_length[0] = Math.Min(m_length[0], m_flat[nRow][j, nTri].X);
						m_length[1] = Math.Max(m_length[1], m_flat[nRow][j, nTri].X);
					}
				}
				//get upper-edge cord vector
				vprev.Set((m_flat[nRow][1, --nTri] - m_flat[nRow][1, 0]).ToArray());
				//set next translation point from upper origin point
				tran.Translation(m_flat[nRow][1, 0].X, m_flat[nRow][1, 0].Y, 0);
			}
		}

		internal void AlignFlatPanels(Panel prev)
		{
			//calculate strip rotation angle
			Point2D[,] pstrip = prev.m_flat[prev.m_flat.Length - 1];
			Vect2 vprev = new Vect2((pstrip[1, pstrip.GetLength(1) - 1] - pstrip[1, 0]).ToArray());
			AlignFlatPanels(pstrip[1, 0], vprev);
			//Vect2 vstrip = new Vect2(m_flat[0][0, m_mesh.GetLength(1) - 1].ToArray());
			//double alpha = vstrip.AngleTo(vprev);
			//Translation tran = new Translation(pstrip[1, 0].X - m_flat[0][0, 0].X, pstrip[1, 0].Y - m_flat[0][0, 0].Y, 0);
			//Transformation rot = new Rotation(alpha, Vector3D.AxisZ);//rotate about z axis
			////rotate each point, then translate
			//for (int nRow = 0; nRow < m_flat.Length; nRow++)
			//	for (int nTri = 0; nTri < m_flat[nRow].GetLength(1); nTri++)
			//	{
			//		for (int j = 0; j < 2; j++)
			//		{
			//			m_flat[nRow][j, nTri].TransformBy(rot);//rotate about origin
			//			m_flat[nRow][j, nTri].TransformBy(tran);//translate to previous strip
			//		}
			//	}
		}
		
		internal void AlignFlatPanels(Point2D pnt, Vect2 dir)
		{
			//calculate strip rotation angle

			Vect2 vstrip = new Vect2(m_flat[0][0, m_mesh.GetLength(1) - 1].ToArray());
			double alpha = vstrip.AngleTo(dir);
			alpha *= Math.Sign(vstrip.Cross(dir));
			Translation tran = new Translation(pnt.X - m_flat[0][0, 0].X, pnt.Y - m_flat[0][0, 0].Y, 0);
			Transformation rot = new Rotation(alpha, Vector3D.AxisZ);//rotate about z axis
			//rotate each point, then translate
			for (int nRow = 0; nRow < m_flat.Length; nRow++)
				for (int nTri = 0; nTri < m_flat[nRow].GetLength(1); nTri++)
				{
					for (int j = 0; j < 2; j++)
					{
						m_flat[nRow][j, nTri].TransformBy(rot);//rotate about origin
						m_flat[nRow][j, nTri].TransformBy(tran);//translate to previous strip
					}
				}
		}
		#endregion

		#region IRebuild Members

		string m_label;
		public string Label
		{
			get
			{
				return m_label;
				//return m_seams != null && m_seams.Length > 1 && m_seams[1] != null ? m_seams[1].Label : "";
			}
			set
			{
				m_label = value;
				//if (m_seams != null && m_seams.Length > 1 && m_seams[1] != null)
				//	m_seams[1].Label = value;
			}
		}
		public string Layer
		{
			get { return "Panels"; }
		}

		bool m_locked = false;
		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		TreeNode m_node = null;

		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new TreeNode(Label);
			else
				m_node.Nodes.Clear();
			m_node.ForeColor = Locked ? System.Drawing.Color.Gray : System.Drawing.Color.Black;
			m_node.Tag = this;
			m_node.Text = Label;
			m_node.ImageKey = m_node.SelectedImageKey = GetType().Name;
			m_node.ToolTipText = GetToolTipData();
			if (m_seams != null && m_seams.Length > 1 && m_seams[0] != null && m_seams[1] != null)
			{
				m_node.Nodes.Add(m_seams[0].ToString());
				m_node.Nodes.Add(m_seams[1].ToString());
			}
			return m_node;
		}

		private string GetToolTipData()
		{
			return GetType().Name;
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities()
		{
			if (m_seams != null && m_seams.Length > 1 && m_seams[0] != null && m_seams[1] != null)
			{
				List<Entity> ee = new List<Entity>();

				//ee.AddRange(m_seams[0].CreateEntities());
				//ee.AddRange(m_seams[1].CreateEntities());
				foreach (IMouldCurve seam in m_seams)
					ee.Add(new LinearPath(CurveTools.GetEvenPathPoints(seam, 20)));


				//if (m_sends != null)
				//	for (int i = 0; i < m_sends.Length; i++)
				//		ee.AddRange(m_sends[i].CreateEntities());

				for (int i = 0; i < 2; i++)
				{
					//create and add top seam
					ee.Add(new LinearPath(CurveTools.GetLimitPathPoints(Corners[i, 1].Seams[1], 30, new Vect2(Corners[i, 1].sPos[1], Corners[i, 0].sPos[1]))));
					//if( Corners[0,0] == Corners[1,0] )//corner wrap add second segment
					//ee.Add(new LinearPath(CurveTools.GetLimitPathPoints(Corners[1, 1].Seams[1], 30, new Vect2(Corners[1, 1].sPos[1], Corners[1, 0].sPos[1]))));

					//create and add bottom seam
					ee.Add(new LinearPath(CurveTools.GetLimitPathPoints(Corners[i, 2].Seams[1], 30, new Vect2(Corners[i, 2].sPos[1], Corners[i, 3].sPos[1]))));
					//if (Corners[0, 3] == Corners[1, 3])//corner wrap add second segment
					//ee.Add(new LinearPath(CurveTools.GetLimitPathPoints(Corners[1, 2].Seams[1], 30, new Vect2(Corners[1, 2].sPos[1], Corners[1, 3].sPos[1]))));
				}

				if (m_mesh != null)
				{
					ee.Add(SurfaceTools.GetMesh(m_mesh, null));
					//ee.Add(SurfaceTools.GetPointCloud(m_mesh));
				}

				if (m_flat != null)
				{
					foreach (Point2D[,] strip in m_flat)
					{
						if( strip != null )
						ee.Add(SurfaceTools.GetMesh(strip));
						//ee.Last().Translate(0, 0, m_group.IndexOf(this));//zoffset
					}
				}

				//panel bounding box
				ee.Add(new LinearPath(m_length[0], m_width[0], Length, Width));
				//ee.Last().Translate(0, 0, m_group.IndexOf(this));//zoffset
				
				System.Drawing.Color c = ColorMath.NextColor();

				foreach (Entity e in ee)
				{
					e.EntityData = this;
					e.Color = c;
					e.ColorMethod = colorMethodType.byEntity;
				}

				return ee;
			}
			return null;
			
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				devDept.Eyeshot.Labels.Label[] lbls = new devDept.Eyeshot.Labels.Label[1];
				Point3D center;
				if (m_mesh != null)
					center = Utilities.Vect3ToPoint3D(m_mesh[m_mesh.GetLength(0) / 2, m_mesh.GetLength(1) / 2]);
				else
					return null;
				lbls[0] = new devDept.Eyeshot.Labels.OutlinedText(center, Label,	new Font("Helvectiva", 8.0f), Color.White, Color.Black, ContentAlignment.MiddleCenter);
				return lbls;

				//List<devDept.Eyeshot.Labels.Label> labels = new List<devDept.Eyeshot.Labels.Label>();
				//for (int i = 0; i < 2; i++)
				//	labels.AddRange(m_seams[i].EntityLabel);
				//return labels.ToArray();
			}
			//get { return m_seams != null && m_seams.Length > 1 && m_seams[1] != null ? m_seams[1].EntityLabel: null; }
		}

		public void GetConnected(List<IRebuild> updated)
		{
			if (Affected(updated) && updated != null)
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			foreach (MouldCurve seam in m_seams)
			{
				parents.Add(seam);
			}
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bcon = false;

			foreach (MouldCurve seam in m_seams)
			{
				bcon |= seam.Affected(connected);
			}
			return bcon;
		}

		public bool Update(Sail s)
		{
			bool bcon = false;

			foreach (MouldCurve seam in m_seams)
				bcon |= seam.Update(s);
			return bcon;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IRebuild Members

		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			throw new NotImplementedException();
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
