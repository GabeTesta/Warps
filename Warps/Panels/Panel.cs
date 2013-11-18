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



			TMeshStraightSeams();
			//TMeshRBFStraightSeams();

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
		public int[] TMESH = new int[]{6,9};//strips,cols

		internal Vect2[,] m_uMesh = null;
		Vect3[,] m_xMesh = null;
		/// <summary>
		/// gets the TMESH array size
		/// </summary>
		/// <param name="nDir">0 for cross-seam, 1 for along-seam</param>
		/// <returns>the number of panels in the specified mesh direction</returns>
		//int TMESH(int nDir) { return m_mesh.GetLength(nDir); }

		/// <summary>
		/// constructs a panel mesh using chain-curves for the ends and single-segement seam curves
		/// result is a mesh that more closely resembles a grid
		/// seems to be more stable when dealing with 5 and 6 sided panels
		/// </summary>
		void TMeshStraightSeams()
		{
			//construct end chains
			ChainCurve[] ends = new ChainCurve[2];
			//do top 
			ends[0] = new ChainCurve();
			ends[0].Add(Corners[0, 1].Seams[1], Corners[0, 1].sPos[1], Corners[0, 0].sPos[1]);
			if (Corners[0, 0] == Corners[1, 0])
				ends[0].Add(Corners[1, 1].Seams[1], Corners[1, 0].sPos[1], Corners[1, 1].sPos[1]);

			//do bottom 
			ends[1] = new ChainCurve();
			ends[1].Add(Corners[0, 2].Seams[1], Corners[0, 2].sPos[1], Corners[0, 3].sPos[1]);
			if (Corners[0, 3] == Corners[1, 3])
				ends[1].Add(Corners[1, 2].Seams[1], Corners[1, 3].sPos[1], Corners[1, 2].sPos[1]);

			//get end-chain node positions
			List<double>[] sEnds = new List<double>[2]{new List<double>(), new List<double>()};
			int nLow, nHi;
			for( int nEnd = 0; nEnd <2; nEnd++)
				if (ends[nEnd].Curves.Count > 1)//end has a kink
				{
					double pKink = ends[nEnd].Segments[1];
					nLow = (int)Math.Floor(pKink * TMESH[0]);
					nHi = TMESH[0] - nLow ;
					//add start pos
					sEnds[nEnd].Add(0);
					//add first segment internal pos
					for (int i = 0; i < nLow -1; i++)
						sEnds[nEnd].Add( BLAS.interpolate( (double)(i + 1) / (double)nLow, pKink, 0));
					//add kink pos
					sEnds[nEnd].Add(pKink);
					//add second segment internal pos
					for (int i = 0; i < nHi-1; i++)
						sEnds[nEnd].Add(BLAS.interpolate((double)(i + 1) / (double)nHi, 1, pKink));
					//add end pos
					sEnds[nEnd].Add(1);
				}
				else//smooth end, simply interpolate
					for (int i = 0; i < TMESH[0]+1; i++)
						sEnds[nEnd].Add(BLAS.interpolant(i, TMESH[0]+1));
	
			//populate TMESH by interpolating nodes from edge positions
			double t;

			Vect2 uv0 = new Vect2(), uv1 = new Vect2();//, uvi = new Vect2();
			m_uMesh = new Vect2[TMESH[0] + 1, TMESH[1] + 1];
			m_xMesh = new Vect3[TMESH[0] + 1, TMESH[1] + 1];
			for (int nStrip = 0; nStrip <= TMESH[0]; nStrip++)//create end uv/xyz points
			{
				//get first end's bracketed point
				//if (j <= nLows[0])
				//	s = BLAS.interpolate(j, nLows[0], sEnds[0][1], sEnds[0][0] );
				//else
				//	s = BLAS.interpolate(j- nLows[0], nHighs[0] - nLows[0], sEnds[0][2], sEnds[0][1]);

				m_xMesh[nStrip, 0] = new Vect3();
				ends[0].xVal(sEnds[0][nStrip], ref uv0, ref m_xMesh[nStrip, 0]);//get lower end point

				//get second end's bracketed point
				//if (j <= nLows[1])
				//	s = BLAS.interpolate(j, nLows[1], sEnds[1][1], sEnds[1][0]);
				//else
				//	s = BLAS.interpolate(j - nLows[1], nHighs[1] - nLows[1], sEnds[1][2], sEnds[1][1]);

				m_xMesh[nStrip, TMESH[1]] = new Vect3();
				ends[1].xVal(sEnds[1][nStrip], ref uv1, ref m_xMesh[nStrip, TMESH[1]]);//get upper end point
				m_uMesh[nStrip, TMESH[1]] = new Vect2(uv1);//store upper uv point

				//fill internal points
				for (int nCol = 0; nCol < TMESH[1]; nCol++)
				{
					m_uMesh[nStrip, nCol] = new Vect2();
					m_xMesh[nStrip, nCol] = new Vect3();
					if (nStrip == 0 )//use first seam  && Corners[0, 0] == Corners[1, 1] && Corners[0, 1] == Corners[1, 0])//overlapping upper edge, place points on edge curve
					{
						t = BLAS.interpolant(nCol, TMESH[1] + 1);
						Seams[0].uVal(t, ref m_uMesh[nStrip, nCol]); 
						//t = BLAS.interpolate(i, TMESH[0] + 1, Corners[0, 0].sPos[1], Corners[1, 0].sPos[1]);//interpolate edge curve s position between limits
						//Corners[0, 0].Seams[1].uVal(t, ref m_uMesh[i, j]);
					}
					else if (nStrip == TMESH[0])//use endseam && Corners[0, 3] == Corners[1, 2] && Corners[0, 2] == Corners[1, 3]) //overlapping lower edge, place points on edge curve
					{
						t = BLAS.interpolant(nCol, TMESH[1] + 1);
						Seams[1].uVal(t, ref m_uMesh[nStrip, nCol]);
						//t = BLAS.interpolate(i, TMESH[0] + 1, Corners[0, 3].sPos[1], Corners[1, 3].sPos[1]);//interpolate edge curve s position between limits
						//Corners[1, 3].Seams[1].uVal(t, ref m_uMesh[i, j]);
					}
					else//linear interpolation of uv-coords for internal points
					{
						t = BLAS.interpolant(nCol, TMESH[1] + 1);
						m_uMesh[nStrip, nCol][0] = BLAS.interpolate(t, uv1[0], uv0[0]);
						m_uMesh[nStrip, nCol][1] = BLAS.interpolate(t, uv1[1], uv0[1]);
					}
					//get mesh xyz from mould and store
					Mould.xVal(m_uMesh[nStrip, nCol], ref m_xMesh[nStrip, nCol]);
				}
			}
		}
		/// <summary>
		/// constructs a panel mesh using chain-curves for the seams which incorporate the end segements
		/// result is a mesh with converging corner elements creating flatpanels with triangular ends
		/// seems less stable for 5 and 6 sided panels, but is the original algorithm used in 3DLayout
		/// </summary>
		void TMeshConvergingCorners()
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
			kinks.ForEach(k => iKinks.Add((int)Math.Ceiling(k * ( TMESH[1] + 1))));
			//iKinks.Add(tmesh[1] + 1);
			//int nKink = 1;

			//populate TMESH by interpolating nodes from edge positions
			double s, t;

			Vect2 uv0 = new Vect2(), uv1 = new Vect2();//, uvi = new Vect2();
			m_uMesh = new Vect2[TMESH[0] + 1, TMESH[1] + 1];
			m_xMesh = new Vect3[TMESH[0] + 1, TMESH[1] + 1];
			for (int j = 0; j <= TMESH[1]; j++)//create edge uv/xyz points
			{
				//if (j > iKinks[nKink]) nKink++;//overstepped kink braket, increment into new bracket
				//s = BLAS.interpolate(j - iKinks[nKink - 1], iKinks[nKink] - iKinks[nKink - 1], kinks[nKink], kinks[nKink - 1]);

				s = BLAS.interpolant(j, TMESH[1] + 1);
				m_xMesh[0, j] = new Vect3();
				edges[0].xVal(s, ref uv0, ref m_xMesh[0, j]);//get lower seam point

				m_xMesh[TMESH[0], j] = new Vect3();
				edges[1].xVal(s, ref uv1, ref m_xMesh[TMESH[0], j]);//get upper seam point
				m_uMesh[TMESH[0], j] = new Vect2(uv1);//store upper uv point

				for (int i = 0; i < TMESH[0]; i++)
				{
					m_uMesh[i, j] = new Vect2();
					m_xMesh[i, j] = new Vect3();
					if (j == 0 && Corners[0, 0] == Corners[1, 1] && Corners[0,1] == Corners[1,0])//overlapping upper edge, place points on edge curve
					{
						t = BLAS.interpolate(i, TMESH[0] + 1, Corners[0, 0].sPos[1], Corners[1, 0].sPos[1]);//interpolate edge curve s position between limits
						Corners[0, 0].Seams[1].uVal(t, ref m_uMesh[i, j]);
					}
					else if (j == TMESH[1] && Corners[0, 3] == Corners[1, 2] && Corners[0, 2] == Corners[1, 3]) //overlapping lower edge, place points on edge curve
					{
						t = BLAS.interpolate(i, TMESH[0] + 1, Corners[0, 3].sPos[1], Corners[1, 3].sPos[1]);//interpolate edge curve s position between limits
						Corners[1, 3].Seams[1].uVal(t, ref m_uMesh[i, j]);
					}
					else//linear interpolation of uv-coords for internal points
					{
						t = BLAS.interpolant(i, TMESH[0] + 1);
						m_uMesh[i, j][0] = BLAS.interpolate(t, uv1[0], uv0[0]);
						m_uMesh[i, j][1] = BLAS.interpolate(t, uv1[1], uv0[1]);
					}
					//get mesh xyz from mould and store
					Mould.xVal(m_uMesh[i, j], ref m_xMesh[i, j]);
				}
			}
		}

		/// <summary>
		/// constructs a panel mesh using chain-curves for the ends and single-segement seam curves
		/// fits a unit-RBF to the edges which is then used to interpolate internal meshpoints
		/// result is a mesh that more closely resembles a grid
		/// </summary>
		void TMeshRBFStraightSeams()
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
				for (int i = 0; i < TMESH[0]; i++)//seam points including corners
				{
					s = BLAS.interpolant(i, TMESH[0]);

					// seam point
					Seams[nEdg].uVal(s, ref u);
					panUV.Add(new double[] { s, nEdg });
					sailUV.Add(u.ToArray());
				}
				for (int i = 1; i < TMESH[0]; i++)//internal edge points
				{
					s = BLAS.interpolant(i, TMESH[0] + 1);

					edges[nEdg].uVal(s, ref u);
					panUV.Add(new double[] { nEdg, s });
					sailUV.Add(u.ToArray());
				}
			}
			RBF.RBFNetwork RBFPan = new RBF.RBFNetwork();
			RBFPan.Fit(panUV, sailUV);

			m_uMesh = new Vect2[TMESH[0] + 1, TMESH[1] + 1];
			m_xMesh = new Vect3[TMESH[0] + 1, TMESH[1] + 1];
			//populate first and last rows
			for (int j = 0; j <= TMESH[1]; j++)
			{
				s = BLAS.interpolant(j, TMESH[1] + 1);

				m_uMesh[0, j] = new Vect2();
				m_xMesh[0, j] = new Vect3();
				Seams[0].xVal(s, ref m_uMesh[0, j], ref m_xMesh[0, j]);

				m_uMesh[TMESH[0], j] = new Vect2();
				m_xMesh[TMESH[0], j] = new Vect3();
				Seams[1].xVal(s, ref m_uMesh[TMESH[0], j], ref m_xMesh[TMESH[0], j]);

			}
			//populate internal nodes
			Vect2 st = new Vect2();
			for (int i = 1; i < TMESH[0]; i++)
			{
				st[1] = BLAS.interpolant(i, TMESH[0]);
				for (int j = 0; j <= TMESH[1]; j++)
				{
					m_uMesh[i, j] = new Vect2();
					m_xMesh[i, j] = new Vect3();

					st[0] = BLAS.interpolant(j, TMESH[1] + 1);
					RBFPan.Value(st.m_vec, ref m_uMesh[i, j].m_vec);
					Mould.xVal(m_uMesh[i, j], ref m_xMesh[i, j]);
				}
			}

			//using nodes create strip via edgelengths

			//rotate/translate strip to align with previous 
			//get max/min and calculate width/bounding box;
		}
		/// <summary>
		/// constructs a panel mesh using chain-curves for the seams which incorporate the end segements
		/// fits a unit-RBF to these edges which is then used to interpolate internal meshpoints
		/// result is a mesh with converging corner elements creating flatpanels with triangular ends
		/// </summary>
		void TMeshRBFConvergingCorners()
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
			List<double[]> panST = new List<double[]>(2 * TMESH[1]);//TMESH points per long-seam
			List<double[]> sailUV = new List<double[]>(2 * TMESH[1]);
			double s;
			Vect2 u = new Vect2();
			for (int nEdg = 0; nEdg < 2; nEdg++)
			{
				for (int i = 0; i <= TMESH[1]; i++)//seam points including corners
				{
					s = BLAS.interpolant(i, TMESH[1]+1);

					// long-seam point
					edges[nEdg].uVal(s, ref u);
					panST.Add(new double[] { s, nEdg });
					sailUV.Add(u.ToArray());
				}
			}
			RBF.RBFNetwork RBFPan = new RBF.RBFNetwork();
			RBFPan.Fit(panST, sailUV);

			m_uMesh = new Vect2[TMESH[0] + 1, TMESH[1] + 1];
			m_xMesh = new Vect3[TMESH[0] + 1, TMESH[1] + 1];
			//populate nodes
			Vect2 st = new Vect2();
			for (int i = 0; i <= TMESH[0]; i++)
			{
				st[1] = BLAS.interpolant(i, TMESH[0] + 1);
				for (int j = 0; j <= TMESH[1]; j++)
				{
					m_uMesh[i, j] = new Vect2();
					m_xMesh[i, j] = new Vect3();
					st[0] = BLAS.interpolant(j, TMESH[1] + 1);
					RBFPan.Value(st.m_vec, ref m_uMesh[i, j].m_vec);
					Mould.xVal(m_uMesh[i, j], ref m_xMesh[i, j]);
				}
			}
		}

		#endregion

		#region Flatten

		internal Point2D[][,] m_strips = null;
		Vect2 m_width = new Vect2(1e9, -1e9);//track max/min for width
		Vect2 m_length = new Vect2(1e9, -1e9);//track max/min for length

		public double Width
		{ get { return m_width[1] - m_width[0]; } }
		public double Length
		{ get { return m_length[1] - m_length[0]; } }

		/// <summary>
		/// flattens the TMesh panels by creating flat-strips
		/// these strips are then stacked to determine the panel broadseam/fan-curves and the total width/height
		/// </summary>
		void Flatten()
		{
			m_width.Set(1e9, -1e9);//track max/min for width/length
			m_length.Set(1e9, -1e9);//track max/min for width/length
			int nRow, nTri;
			//create 2d strips
			m_strips = new Point2D[m_xMesh.GetLength(0) - 1][,];

			double A, B, C, gamma;
			double Ba, Bn;
			Vect2 vA = new Vect2(), vN, //A-frame vector and normal
			vB;//B vector

			//reset vprev to be horizontal
			Vect2 vprev = new Vect2(1,0);

			//initial translation is 0
			Transformation tran = new Translation(0, 0, 0);
			//vprev[0] = 1; vprev[1] = 0;

			//for (nRow = 0; nRow < 1; nRow++)
			for (nRow = 0; nRow < m_xMesh.GetLength(0) - 1; nRow++)
			{
				m_strips[nRow] = new Point2D[2, m_xMesh.GetLength(1)];

				//initial edge corners
				A = m_xMesh[nRow, 0].Distance(m_xMesh[nRow + 1, 0]);
				m_strips[nRow][0, 0] = new Point2D(0, 0);//start at origin
				m_strips[nRow][1, 0] = new Point2D(0, A);//project first point vertically

				for (nTri = 0; nTri < m_xMesh.GetLength(1) - 1; nTri++)
				{
					//lower triangle sides
					A = m_xMesh[nRow, nTri].Distance(m_xMesh[nRow + 1, nTri]);
					B = m_xMesh[nRow, nTri].Distance(m_xMesh[nRow, nTri + 1]);
					C = m_xMesh[nRow, nTri + 1].Distance(m_xMesh[nRow + 1, nTri]);

					vA.Set((m_strips[nRow][1, nTri] - m_strips[nRow][0, nTri]).ToArray());//get A-direction vector
					vA.Unitize();//convert to unit vector
					vN = vA.Normal() * -1;//get A-normal unit vector
					if (vN.Magnitude == 0)
						vN.Set(1, 0);//default to horizontal projection for converging corner
					

					//angle between A and B
					gamma = A == 0 || B == 0 ? 0 : (A * A + B * B - C * C) / (2 * A * B);
					Utilities.LimitRange(-1, ref gamma, 1);//ensure valid acos
					gamma = Math.Acos(gamma);//get angle

					//B components along A-frame
					Ba = B * Math.Cos(gamma);
					Bn = B * Math.Sin(gamma);

					//determine B-vector from B-components along A-frame
					vB = vA * Ba + vN * Bn;

					//project forward lower point using B-vector and inital point
					m_strips[nRow][0, nTri + 1] = new Point2D(m_strips[nRow][0, nTri].X + vB[0], m_strips[nRow][0, nTri].Y + vB[1]);

					//upper triangle sides
					A = m_xMesh[nRow + 1, nTri + 1].Distance(m_xMesh[nRow + 1, nTri]);
					B = m_xMesh[nRow + 1, nTri + 1].Distance(m_xMesh[nRow, nTri + 1]);
					//C = m_mesh[nRow, nTri + 1].Distance(m_mesh[nRow + 1, nTri]);common edge

					//C-frame unit vectors
					vA.Set((m_strips[nRow][1, nTri] - m_strips[nRow][0, nTri + 1]).ToArray());
					vA.Unitize();//convert to unit vector
					vN = vA.Normal() * -1;//get C-normal unit vector
					if (vN.Magnitude == 0)
						vN.Set(1,0);//default to horizontal projection for converging corner

					//angle between C and B'
					//gamma = B == 0 || C == 0 ? 0 : (B * B + C * C - A * A) / (2 * B * C);
					gamma = (B * B + C * C - A * A) / (2 * B * C);
					Utilities.LimitRange(-1, ref gamma, 1);//ensure valid acos
					gamma = Math.Acos(gamma);//get angle

					//B components along C-frame
					Ba = B * Math.Cos(gamma);
					Bn = B * Math.Sin(gamma);

					//determine B-vector from B-components along C-frame
					vB = vA * Ba + vN * Bn;

					//project off upper point
					m_strips[nRow][1, nTri + 1] = new Point2D(m_strips[nRow][0, nTri + 1].X + vB[0], m_strips[nRow][0, nTri + 1].Y + vB[1]);
				}
				//calculate strip rotation angle
				Vect2 vstrip = new Vect2(m_strips[nRow][0, nTri].ToArray());
				gamma = vstrip.AngleTo(vprev);
				gamma *= Math.Sign(vstrip.Cross(vprev));
				Transformation rot = new Rotation(gamma, Vector3D.AxisZ);//rotate about z axis
				//rotate each point, then translate
				for (nTri = 0; nTri < m_strips[nRow].GetLength(1); nTri++)
				{
					for (int j = 0; j < 2; j++)
					{
						m_strips[nRow][j, nTri].TransformBy(rot);//rotate about origin
						m_strips[nRow][j, nTri].TransformBy(tran);//translate to previous strip

						//track max/min y values for panel width
						m_width[0] = Math.Min(m_width[0], m_strips[nRow][j, nTri].Y);
						m_width[1] = Math.Max(m_width[1], m_strips[nRow][j, nTri].Y);

						//track max/min x values for panel length
						m_length[0] = Math.Min(m_length[0], m_strips[nRow][j, nTri].X);
						m_length[1] = Math.Max(m_length[1], m_strips[nRow][j, nTri].X);
					}
				}
				//get upper-edge cord vector
				vprev.Set((m_strips[nRow][1, --nTri] - m_strips[nRow][1, 0]).ToArray());
				//set next translation point from upper origin point
				tran.Translation(m_strips[nRow][1, 0].X, m_strips[nRow][1, 0].Y, 0);
			}
		}

		/// <summary>
		///aligns this panel's flattened strips to the previous panel's
		/// </summary>
		/// <param name="prev">the previous panel to align to</param>
		internal void AlignFlatPanels(Panel prev)
		{
			//calculate strip rotation angle
			Point2D[,] pstrip = prev.m_strips[prev.m_strips.Length - 1];
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
		/// <summary>
		/// aligns this panel's flattened strips to the specified point and direction vector
		/// </summary>
		/// <param name="pnt">the point of the bottom-left corner</param>
		/// <param name="dir">the direction vector to align the bottom-chord to</param>
		internal void AlignFlatPanels(Point2D pnt, Vect2 dir)
		{
			//calculate strip rotation angle

			Vect2 vstrip = new Vect2(m_strips[0][0, m_xMesh.GetLength(1) - 1].ToArray());
			double alpha = vstrip.AngleTo(dir);
			alpha *= Math.Sign(vstrip.Cross(dir));
			Translation tran = new Translation(pnt.X - m_strips[0][0, 0].X, pnt.Y - m_strips[0][0, 0].Y, 0);
			Transformation rot = new Rotation(alpha, Vector3D.AxisZ);//rotate about z axis
			//rotate each point, then translate
			for (int nRow = 0; nRow < m_strips.Length; nRow++)
				for (int nTri = 0; nTri < m_strips[nRow].GetLength(1); nTri++)
				{
					for (int j = 0; j < 2; j++)
					{
						m_strips[nRow][j, nTri].TransformBy(rot);//rotate about origin
						m_strips[nRow][j, nTri].TransformBy(tran);//translate to previous strip
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

		TreeNode m_node = null;
		public TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.ToolTipText = GetToolTipData();
			m_node.Nodes.Clear();
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

				foreach (IMouldCurve seam in m_seams)
					ee.Add(new LinearPath(CurveTools.GetEvenPathPoints(seam, 20)));

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

				if (m_xMesh != null)
				{
					//draw the xyz mesh on the sail
					ee.Add(SurfaceTools.GetMesh(m_xMesh));
					//ee.Add(SurfaceTools.GetPointCloud(m_mesh));
				}

				//draw the flattened strips
				if (m_strips != null)
				{
					foreach (Point2D[,] strip in m_strips)
					{
						if( strip != null )
						ee.Add(SurfaceTools.GetMesh(strip));
						ee.Last().Translate(10, 0,0);//xoffset
						//ee.Last().Translate(0, 0, m_group.IndexOf(this));//zoffset
					}
				}

#if DEBUG
				//draw the uv-mesh
				if (m_uMesh != null)
				{
					ee.Add(SurfaceTools.GetMesh(m_uMesh));
					ee.Last().Translate(-2, -2, 0);
				}

				//panel bounding box
				ee.Add(new LinearPath(m_length[0], m_width[0], Length, Width));
				ee.Last().Translate(10, 0, 0);//xoffset
				//ee.Last().Translate(0, 0, m_group.IndexOf(this));//zoffset

				//color the panels uniquly for easier debugging
				System.Drawing.Color c = ColorMath.NextColor();
				c = ColorMath.NextColor();
				foreach (Entity e in ee)
				{
					e.EntityData = this;
					e.Color = c;
					e.ColorMethod = colorMethodType.byEntity;
				}
#endif

				return ee;
			}
			return null;
			
		}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> lbls = new List<devDept.Eyeshot.Labels.Label>(1);
				Point3D center;
				if (m_xMesh != null)
					center = Utilities.Vect3ToPoint3D(m_xMesh[m_xMesh.GetLength(0) / 2, m_xMesh.GetLength(1) / 2]);
				else
					return null;
				lbls.Add(new devDept.Eyeshot.Labels.OutlinedText(center, Label,	Utilities.Font, Color.White, Color.Black, ContentAlignment.MiddleCenter));
				return lbls;

				//List<devDept.Eyeshot.Labels.Label> labels = new List<devDept.Eyeshot.Labels.Label>();
				//for (int i = 0; i < 2; i++)
				//	labels.AddRange(m_seams[i].EntityLabel);
				//return labels.ToArray();
			}
			//get { return m_seams != null && m_seams.Length > 1 && m_seams[1] != null ? m_seams[1].EntityLabel: null; }
		}

		public void GetChildren(List<IRebuild> updated)
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

		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			throw new NotImplementedException();
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			throw new NotImplementedException();
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Label);
		}
	}
}
