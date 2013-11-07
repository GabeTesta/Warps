using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept;
using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using Warps.Yarns;
using System.Windows.Forms;

namespace Warps
{
	public class DensityMesh : IRebuild
	{
		public DensityMesh()
		{
			Label = "Yarn Density";
		}

		const int UREZ = 20, VREZ = 40;

		double m_radius = .5;//default 1/2 meter circle
		public double Radius
		{
			get { return m_radius; }
			set { m_radius = value; }
		}

		Sail m_sail;
		string m_label;
		List<YarnGroup> m_Yarns = new List<YarnGroup>();
		Vect2 m_maxmin = new Vect2(1e9, -1e9);

		public List<YarnGroup> Yarns
		{
			get { return m_Yarns; }
			set { m_Yarns = value; }
		}
		public Vect2 MaxMin { get { return m_maxmin; } }

		bool IsGrid
		{
			get { return m_mesh != null; }
		}

		#region GridMesh

		Vect3[,] m_mesh;
		double[,] m_dpi;
		int Rows { get { return m_mesh.GetLength(0); } }
		int Cols { get { return m_mesh.GetLength(1); } }
		void Allocate(int nRow, int nCol)
		{
			if (nRow == 0 || nCol == 0)
			{
				m_mesh = null;
				m_dpi = null;
			}
			else
			{
				m_mesh = new Vect3[nRow, nCol];
				m_dpi = new double[nRow, nCol];
			}
			m_maxmin = new Vect2(1e9, -1e9);
		}
		public void MeshSail(Sail sail)
		{
			m_sail = sail;
			List<IRebuild> yarns = sail.FlatLayout().FindAll(grp => grp is Yarns.YarnGroup);
			Yarns = yarns.ConvertAll(rb => rb as YarnGroup);

			Allocate(UREZ, VREZ);
			for (int nRow = 0; nRow < Rows; nRow++)
			{
				Parallel.For(0, Cols, nCol =>
				//for (int nCol = 0; nCol < Cols; nCol++)
				{
					double s = 0.5, h = 0, dpi = 0;
					Vect2 uv = new Vect2();
					Vect3 xyz = new Vect3();

					uv[0] = BLAS.interpolant(nRow, Rows);
					uv[1] = BLAS.interpolant(nCol, Cols);
					m_mesh[nRow, nCol] = new Vect3();
					sail.Mould.xVal(uv, ref m_mesh[nRow, nCol]);
					Yarns.ForEach(grp =>
					{
						grp.ForEach(yar =>
							{
								xyz.Set(m_mesh[nRow, nCol]);
								if (Curves.CurveTools.xClosest(yar, ref s, ref uv, ref xyz, ref h, 1e-3, true) && h < Radius)
									dpi += Radius - h;//accumulate yarns in area
							});
					});
					m_dpi[nRow, nCol] = dpi;
					m_maxmin[0] = Math.Min(m_maxmin[0], dpi);
					m_maxmin[1] = Math.Max(m_maxmin[1], dpi);
				});
			}
		}

		#endregion

		#region DelaunayMesh

		double Tolerance = 5;//5dpi tolerance
		devDept.Eyeshot.Triangulation.Delaunay m_delaunay = null;
		List<double> m_delDPI = new List<double>();

		public void DelaunayMesh(Sail sail)
		{
			Allocate(0, 0);
			m_sail = sail;
			List<IRebuild> yarns = sail.FlatLayout().FindAll(grp => grp is Yarns.YarnGroup);
			Yarns = yarns.ConvertAll(rb => rb as YarnGroup);

			List<Point3D> dels = new List<Point3D>(5 * 10);
			for (int i = 0; i < UREZ; i++)
			{
				for (int j = 0; j < VREZ; j++)
				{
					dels.Add(new Point3D(BLAS.interpolant(i, UREZ), BLAS.interpolant(j, VREZ)));
				}
			}
			//get initial triangulization
			m_delaunay = new devDept.Eyeshot.Triangulation.Delaunay(dels);
			m_delaunay.OutputType = meshNatureType.Plain;
			m_delaunay.DoWork();

			//return;//start with this


			//create final triangulization array
			List<Point3D> points = new List<Point3D>();
			points.AddRange(m_delaunay.Result.Vertices);//include all existing points

			//get the dpi at each point
			Parallel.For(0, m_delaunay.Result.Vertices.Length, i =>
			{
				m_delaunay.Result.Vertices[i].Z = CheckDPI(m_delaunay.Result.Vertices[i]);
			});

			//subdivide triangles as necessary
			Parallel.ForEach(m_delaunay.Result.Triangles, tri =>
			//foreach( IndexTriangle tri in m_delaunay.Result.Triangles)
			{
				List<Point3D> trs = CheckTriangle(m_delaunay.Result.Vertices[tri.V1], m_delaunay.Result.Vertices[tri.V2], m_delaunay.Result.Vertices[tri.V3]);
				points.AddRange(trs);
			});
			m_delDPI.Clear();
			m_maxmin = new Vect2(1e9, -1e9);
			points.ForEach(pt =>
			{
				m_maxmin[0] = Math.Min(m_maxmin[0], pt.Z);
				m_maxmin[1] = Math.Max(m_maxmin[1], pt.Z);
				m_delDPI.Add(pt.Z);
				pt.Z = 0;
			});
			m_delaunay = new devDept.Eyeshot.Triangulation.Delaunay(points);
			m_delaunay.OutputType = meshNatureType.Plain;
			m_delaunay.DoWork();

			//check neighboring vertices
			//if dpi difference add center point and recurse
		}


		double CheckDPI(Point3D uvpt)
		{
			double s = 0.5, h = 0;
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3(), targ = new Vect3();

			uv[0] = uvpt.X;
			uv[1] = uvpt.Y;
			uvpt.Z = 0;

			//get the vertex target point
			m_sail.Mould.xVal(uv, ref xyz);
			Yarns.ForEach(grp =>
			{
				grp.ForEach(yar =>
				{
					targ.Set(xyz);
					if (Curves.CurveTools.xClosest(yar, ref s, ref uv, ref targ, ref h, 1e-5, true) && h < Radius)
						uvpt.Z += Radius - h;//accumulate yarns in area
				});
			});
			return uvpt.Z;
		}

		List<Point3D> CheckTriangle(Point3D a, Point3D b, Point3D c)
		{
			double max, min;
			max = Math.Max(c.Z, Math.Max(b.Z, a.Z));
			min = Math.Min(c.Z, Math.Min(b.Z, a.Z));
			double AB = BLAS.distance(a.X, a.Y, b.X, b.Y);
			double AC = BLAS.distance(a.X, a.Y, c.X, c.Y);
			double BC = BLAS.distance(c.X, c.Y, b.X, b.Y);
			if (AB < 1e-2 || BC < 1e-2 || AC < 1e-2 //minimum size criteria
				|| max - min < Tolerance)//balanced criteria
				return new List<Point3D>();
			else
				return Subdivide(a, b, c);
		}
		private List<Point3D> Subdivide(Point3D a, Point3D b, Point3D c)
		{
			List<Point3D> rets = new List<Point3D>();
			//sort corners
			Point3D head, max, min;
			max = a.Z > c.Z ? (a.Z > b.Z ? a : b) : (c.Z > b.Z ? c : b);
			min = a.Z < c.Z ? (a.Z < b.Z ? a : b) : (c.Z < b.Z ? c : b);
			head = max == a ? (min == b ? c : b) : (min == a ? (max == b ? c : b) : a);


			Point3D pt = new Point3D((max.X + min.X) / 2.0, (max.Y + min.Y) / 2.0);

			//Point3D pt = new Point3D((a.X + b.X + c.X )/3.0, (a.Y + b.Y + c.Y )/3.0, 0.0);
			rets.Add(pt);//add the new subdivision point
			CheckDPI(pt);//get the new points dpi

			//recursively subdivide to get the final point collection 
			rets.AddRange(CheckTriangle(head, pt, max));
			rets.AddRange(CheckTriangle(head, min, pt));

			return rets;
		}

		#endregion

		#region IRebuild Members

		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		public string Layer
		{
			get { return Label; }
		}

		//public List<string> WriteScript()
		//{
		//	throw new NotImplementedException();
		//}

		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	throw new NotImplementedException();
		//}

		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlNode node = NsXml.MakeNode(doc, this);
			NsXml.AddAttribute(node, "YarnGroups", Yarns);
			return node;
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			m_sail = sail;
			Label = NsXml.ReadLabel(node);
			var groups = NsXml.ReadStrings(node, "YarnGroups");
			Yarns.Clear();
			foreach (string s in groups)
			{
				YarnGroup grp = sail.FindGroup(s) as YarnGroup;
				if (grp == null)
					throw new System.Xml.XmlException(string.Format("Invalid YarnGroup [{0}] in DensityMesh [{1}]", s, Label), new System.Xml.XmlException(node.InnerText));
				Yarns.Add(grp);
			}
			Update(sail);
		}

		public bool Locked
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		private string GetToolTipData()
		{
			return String.Format("{0}\n#{1} Groups\nMaxDPI: {2}\nMinDPI: {3}",
				!IsGrid ? "Delaunay Mesh" : "Regular Mesh",
				Yarns.Count,
				m_maxmin[1].ToString("n4"),
				m_maxmin[0].ToString("n4"));
		}

		TreeNode m_node = null;

		public virtual TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.ImageKey = m_node.SelectedImageKey = "Surface";
			m_node.ToolTipText = GetToolTipData();

			m_node.Nodes.Clear();
			TreeNode n = m_node.Nodes.Add(m_maxmin.ToString(false, "n4"));
			n.ImageKey = n.SelectedImageKey = "MaxMin";
			if (Yarns != null)
			{
				foreach (YarnGroup fp in Yarns)
				{
					n = new TreeNode(fp.WriteNode().Text);
					n.ImageKey = n.SelectedImageKey = fp.GetType().Name;
					n.ForeColor = m_node.ForeColor;
					m_node.Nodes.Add(n);
				}
			}
			return m_node;
		}

		public List<Entity> CreateEntities()
		{
			var e = new Point(m_maxmin[0], m_maxmin[1]);
			e.EntityData = this;

			StringBuilder sb = new StringBuilder();
			Yarns.ForEach(y=>sb.AppendLine(y.Label));
			var txt = new Text(0, 0, sb.ToString(), 10);
			if (IsGrid)
				return new List<Entity>() { SurfaceTools.GetMesh(m_mesh, m_dpi, true), e, txt };

			Mesh m = new Mesh(meshNatureType.MulticolorSmooth);
			m.Triangles = m_delaunay.Result.Triangles.Clone() as IndexTriangle[];
			List<PointRGB> verts = new List<PointRGB>(m_delaunay.Result.Vertices.Length);
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();

			double ave, q1, q3, stddev = BLAS.StandardDeviation(m_delDPI, out ave, out q3, out q1);

			for (int i = 0; i < m_delaunay.Result.Vertices.Length; i++)
			{
				uv.Set(m_delaunay.Result.Vertices[i].X, m_delaunay.Result.Vertices[i].Y);
				m_sail.Mould.xVal(uv, ref xyz);
				//verts.Add(Utilities.Vect3ToPointRGB(xyz, ColorMath.GetScaleColor(ave + 2 * stddev, ave - 2 * stddev, m_delDPI[i])));
				verts.Add(Utilities.Vect3ToPointRGB(xyz, ColorMath.GetScaleColor(Math.Log(m_maxmin[1]), Math.Log(m_maxmin[0]), Math.Log(m_delDPI[i]))));
			}
			m.Vertices = verts.ToArray();
			m.ColorMethod = colorMethodType.byEntity;
			m.RegenMode = regenType.RegenAndCompile;
			m.ComputeEdges();
			//mesh.ComputeNormals();
			m.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;

			m.EntityData = this;
#if DEBUG
			m_delaunay.Result.EntityData = this;
			m_delaunay.Result.Translate(-1, -1, 0);
			//return new List<Entity>() { SurfaceTools.GetMesh(m_mesh, m_dpi) };
#endif
			return new List<Entity>() { m,  e, txt };
		}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				return new List<devDept.Eyeshot.Labels.Label>();
			}
		}

		public void GetChildren(List<IRebuild> updated)
		{
			if (Affected(updated))
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			Yarns.ForEach(w => { parents.Add(w); w.GetParents(s, parents); });
		}

		public bool Affected(List<IRebuild> connected)
		{
			foreach (YarnGroup yar in Yarns)
				if (connected.Contains(yar))
					return true;
			return false;
		}

		public bool Update(Sail s)
		{
			if (IsGrid)
				MeshSail(s);
			else
				DelaunayMesh(s);
			return IsGrid || m_delaunay != null;
		}

		public bool Delete()
		{
			return false;
			//Sail.Remove(this);
		}

		#endregion
	}
}
