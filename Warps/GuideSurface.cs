using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Geometry;
using System.Drawing;
using devDept.Eyeshot.Entities;
using Warps.Curves;

namespace Warps
{
	public delegate void Gradients(double s, ref Vect2 u, ref Vect3 x, ref Vect3 dx, ref Vect3 ddx);

	public class GuideSurface : IRebuild
	{
		public GuideSurface() : this("", WarpFrame.CurrentSail, new List<Vect3>() { new Vect3(0, 0, 1), new Vect3(1, 0, 1), new Vect3(0, 1, 1) }) { }
		public GuideSurface(string label, Sail s, List<Vect3> fits)
		{
			m_sail = s;
			Label = label;
			Fit(fits);
		}
		public GuideSurface(GuideSurface copy)
		{
			m_sail = copy.m_sail;
			m_label = copy.m_label;
			if (copy.m_fits != null)
				m_fits = new List<Vect3>(copy.m_fits);
			else
				m_fits = null;
			if( m_fits != null)
				m_surf = new RBF.RBFSurface(m_fits.ToArray());
		}

		Sail m_sail;
		string m_label;
		List<Vect3> m_fits;
		RBF.RBFSurface m_surf;

		public RBF.RBFSurface Surf
		{
			get { return m_surf; }
			set { m_surf = value; }
		}

		public List<Vect3> FitPoints
		{
			get { return m_fits; }
			set { m_fits = value; }
		}

		public void Fit(List<Vect3> fits)
		{
			m_fits = fits;
			ReFit();
		}
		public void ReFit()
		{
			List<double[]> fits = new List<double[]>(FitPoints.Count);
			FitPoints.ForEach(f => fits.Add(f.m_vec));
			m_surf = new RBF.RBFSurface(fits);
		}

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
			get { return "Guides"; }
		}

		//public List<string> WriteScript()
		//{
		//	List<string> script = new List<string>(FitPoints.Count+2);
		//	script.Add(ScriptTools.Label(GetType().Name, Label));
		//	FitPoints.ForEach(v => script.Add("\t" + v.ToString(false)));
		//	return script;
		
		//}

		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	ScriptTools.ReadLabel(txt[0]);
		//	FitPoints = new List<Vect3>(txt.Count-1);
		//	for (int i = 1; i < txt.Count; i++)
		//		FitPoints.Add( new Vect3(txt[i].Trim('\t')));
		//	ReFit();
		//	return FitPoints.Count > 3;
		//}

		public bool Locked
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.SelectedImageKey = m_node.ImageKey = "Surface";
			m_node.Nodes.Clear();
			if (m_fits != null)
				foreach (Vect3 e in m_fits)
					m_node.Nodes.Add(new System.Windows.Forms.TreeNode(e.ToString("f3")));
			return m_node;
		}

		double SCALE = 1;

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities() { return CreateEntities(false); }
		public List<devDept.Eyeshot.Entities.Entity> CreateEntities(bool bPnts)
		{
			List<devDept.Eyeshot.Entities.Entity> ents = new List<devDept.Eyeshot.Entities.Entity>();
			int[] MESH = new int[] { 25, 30 };
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Vect3 nor = new Vect3();
			double[] rbf = new double[3];
			Vect3[,] mesh = new Vect3[MESH[0], MESH[1]];
			double[,] color = new double[MESH[0], MESH[1]];
			for (int i = 0; i < MESH[0]; i++)
			{
				uv[0] = rbf[0] = BLAS.interpolant(i, MESH[0]);
				for (int j = 0; j < MESH[1]; j++)
				{
					uv[1] = rbf[1] = BLAS.interpolant(j, MESH[1]);
					m_surf.Value(ref rbf);
					m_sail.Mould.xNor(uv, ref xyz, ref nor);
					mesh[i, j] = new Vect3(xyz);// +(nor * rbf[2] * SCALE);
					color[i,j] = rbf[2];
				}
			}
			Mesh m = SurfaceTools.GetMesh(mesh,color);
			m.EntityData = this;
			ents.Add(m);
			if (!bPnts)
				return ents;

			//add points
			List<LinearPath> paths = new List<LinearPath>(m_fits.Count);
			Vect3[] cloud = new Vect3[m_fits.Count];
			int nPt = 0;
			foreach (Vect3 v in m_fits)
			{
				uv.Set(v);
				m_sail.Mould.xNor(uv, ref xyz, ref nor);
				cloud[nPt++] = xyz + (nor * v[2] * SCALE);
				//draw normal bar
				paths.Add(new LinearPath(new Point3D[] { Utilities.Vect3ToPoint3D(xyz), Utilities.Vect3ToPoint3D(cloud[nPt - 1]) }));
				paths.Last().LineWeight = 2;
				paths.Last().LineWeightMethod = colorMethodType.byEntity;
				paths.Last().EntityData = this;
			}
			ents.Add(SurfaceTools.GetPointCloud(cloud));
			ents.Last().EntityData = this;
			ents.AddRange(paths);
			return ents;
		}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> lbls = new List<devDept.Eyeshot.Labels.Label>(1);
				double[] c = new double[]{0.5,0.5,0};
				m_surf.Value(ref c);
				lbls.Add(new devDept.Eyeshot.Labels.OutlinedText(new Point3D(c), Label, Utilities.Font, Color.White, Color.Black, ContentAlignment.MiddleCenter));
				return lbls;
			}
		}

		public void GetChildren(List<IRebuild> updated)
		{

		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			//throw new NotImplementedException();

		}

		public bool Affected(List<IRebuild> connected)
		{
			throw new NotImplementedException();
		}

		public bool Update(Sail s)
		{
			m_sail = s;
			ReFit();
			return m_fits != null && m_fits.Count > 2 && m_surf != null && !Double.IsNaN(m_surf.BendingEnergy);
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion

		public virtual bool InsertPoint(PointF mouse, Transformer WorldToScreen, out int nIndex)
		{
			double h = 0;
			Vect2 uv = new Vect2();

			Vect3 xyz;
			//find the closest point to the mouse click
			if (!MClosest(mouse, WorldToScreen, ref uv, out xyz, ref h, 1e-3))//failed to find point to insert
			{
				nIndex = -1;
				return false;
			}

			xyz = new Vect3(uv[0], uv[1], 0);
			Surf.Value(ref xyz.m_vec);//get the current height at that location
			
			m_fits.Add(xyz);//fit to it (shouldn't change the surface much)
			nIndex = m_fits.Count-1;
			return true;
		}

		public bool RemovePoint(int index)
		{
			if (index < 0 || FitPoints.Count <= index || FitPoints.Count <= 2)
				return false;

			m_fits.RemoveAt(index);
			ReFit();
			return FitPoints.Count > 2;//3 points for rbf
		}

		public bool MClosest(PointF mTarget, Transformer wts, ref Vect2 uv, out Vect3 xyzTarget, ref double dist, double tol)
		{
			Point3D mouse = new Point3D(mTarget.X, mTarget.Y, 0);

			Point3D mx;
			Point3D dmdu;
			Point3D dmdv;

			Vect3 x = new Vect3();
			Vect3 dx = new Vect3();
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();

			Vect3 h = new Vect3();
			Vect2 c = new Vect2();
			Vect2 res = new Vect2();
			Vect2 a = new Vect2(), b = new Vect2();
			double det, r;
			Vect2 d = new Vect2();
			uv.Set(0.5, 0.5);
			int loop = 0, max_loops = 150;
			while (loop++ < max_loops)
			{
				m_sail.Mould.xCvt(uv, ref x, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

				//calc mouse coords of guess point
				mx = wts(new Point3D(x[0], x[1], x[2]));
				mx.Z = 0;//remove z component from 2d mouse coords
				//calculate mouse coord distance from guess to target
				h.Set((mx - mouse).ToArray());
				dist = mx.DistanceTo(mouse);// h.Magnitude;

				//calculate mouse coord derivatives wrt s-pos
				//dx.Unitize();
				dx = x + dxu;
				dmdu = wts(new Point3D(dx[0], dx[1], dx[2]));
				dmdu -= mx;

				dx = x + dxv;
				dmdv = wts(new Point3D(dx[0], dx[1], dx[2]));
				dmdv -= mx;

				c[0] = h[0];//error, is 0 when on target
				c[1] = h[1];
				//e[0] = s;
				//c[0] = h.Dot(dxu);// BLAS.dot(h, dxu); // error, dot product is 0 at pi/2
				//c[1] = h.Dot(dxv);// BLAS.dot(h, dxv); // error, dot product is 0 at pi/2

				if (Math.Abs(c[0]) < tol && Math.Abs(c[1]) < tol) // error is less than the tolerance
				{
					xyzTarget=x;// return point to caller
					return true;
				}
	
				a[0] = dmdu.X;
				a[1] = dmdu.Y;
				b[0] = dmdv.X;
				b[1] = dmdv.Y;
				//a[0] = dxu.Norm + h.Dot(ddxu);
				//a[1] = b[0] = dxu.Dot(dxv) + h.Dot(dduv);
				//b[1] = dxv.Norm + h.Dot(ddxv);

				det = a.Cross(b);

				d[0] = c.Cross(b) / det;
				d[1] = a.Cross(c) / det;

				c[0] = 0.01 > Math.Abs(d[0]) ? 1 : 0.01 / Math.Abs(d[0]);
				c[1] = 0.01 > Math.Abs(d[1]) ? 1 : 0.01 / Math.Abs(d[1]);
				//enforce maximum increment
				r = Math.Min(c[0], c[1]);

				//increment uv by scaled residuals
				//uv = BLAS.subtract(uv, BLAS.scale(d, r));
				uv = uv - d * r;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}

			xyzTarget = x;
			//s = s0;
			return false;
		}

		public bool DragPoint(int index, PointF mouse, Transformer WorldToScreen)
		{
			Vect2 uv0 = new Vect2(FitPoints[index].x, FitPoints[index].y);
			Vect3 xyz = new Vect3();

			// get the mouse-coord x for the original position
			m_sail.Mould.xVal(uv0, ref xyz);
			Point3D x0 = WorldToScreen(new Point3D(xyz.Array));

			// small offsets
			const double delta_u = .05;
			Vect2 udu = new Vect2(uv0[0] + delta_u, uv0[1]);
			Vect2 udv = new Vect2(uv0[0], uv0[1] + delta_u);

			// get the mouse-coord x for the small offsets
			m_sail.Mould.xVal(udu, ref xyz);
			Point3D xdu = WorldToScreen(new Point3D(xyz.Array));
			m_sail.Mould.xVal(udv, ref xyz);
			Point3D xdv = WorldToScreen(new Point3D(xyz.Array));

			// subtract offsets from initial point to get delta xy from delta u
			Vect2 dx = new Vect2(xdu.X - x0.X, xdu.Y - x0.Y);
			double dxdu = (double)dx.u / delta_u; // divide delta x by delta u to get d(x)/d(u)
			double dydu = (double)dx.v / delta_u;

			dx = new Vect2(xdv.X - x0.X, xdv.Y - x0.Y);
			double dxdv = (double)dx.u / delta_u;
			double dydv = (double)dx.v / delta_u;

			dx = new Vect2(mouse.X - x0.X, mouse.Y - x0.Y);
			//	[ dxdv  dxdu ][delta v]   [delta x]
			//	[ dydv  dydu ][delta u] = [delta y]  solve for delta v, delta u

			double det = dxdv * dydu - dydv * dxdu;
			if (det == 0)
				return false; // if the determinant is 0 quit

			// solve the system for delta u
			Vect2 du = new Vect2();
			//C2dPoint du;
			du[0] = (dxdv * (double)dx.v - dydv * (double)dx.u) / det;
			du[1] = (dydu * (double)dx.u - dxdu * (double)dx.v) / det;

			for (int i = 0; i < 2; i++)
			{

				FitPoints[index][i] = Utilities.LimitRange(0, FitPoints[index][i] + du[i], 1);
				//m_uFits[i][index] += du[i];
				////ensure inbounds
				//m_uFits[i][index] = Math.Max(0, m_uFits[i][index]);
				//m_uFits[i][index] = Math.Min(1, m_uFits[i][index]);
			}

			//ReFit();
			return true;
		}

		internal void DragHeight(int index, PointF mpt, Transformer wts)
		{
			Vect2 uv = new Vect2(FitPoints[index].x, FitPoints[index].y);
			Vect3 x = new Vect3(), nor = new Vect3();
			ProjectedPoint(uv, ref x, ref nor);
			Point3D bot = wts(new Point3D(x.m_vec));
			Point3D top = wts(new Point3D(nor.m_vec));
			Point3D mou = new Point3D(mpt.X, mpt.Y, 0);
			top.Z = bot.Z = 0;//remove z component from 2d mouse coords
			double s = mou.DistanceTo(bot);
			double l = top.DistanceTo(bot);//scale height
			FitPoints[index][2] *= s/l;

		}

		void ProjectedPoint(Vect2 uv, ref Vect3 xyz, ref Vect3 nor)
		{
			m_sail.Mould.xNor(uv, ref xyz, ref nor);
			Vect3 v = new Vect3(uv[0], uv[1], 0);
			Surf.Value(ref v.m_vec);
			nor = xyz + (nor * v[2] * SCALE);
		}

		#region IRebuild Members


		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlNode script = NsXml.MakeNode(doc, this);
			int nCmb = 0;
			foreach (Vect3 pt in FitPoints)
				script.AppendChild(NsXml.MakeNode(doc, "P" + (++nCmb).ToString(), pt.ToString(false)));
			return script;
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			Label = NsXml.ReadLabel(node);
			m_sail = sail;
			List<Vect3> combs = new List<Vect3>();
			foreach (System.Xml.XmlNode child in node.ChildNodes)
			{
				if (child.Name == "Comb")
				{
					foreach (System.Xml.XmlNode pt in child)
						combs.Add(new Vect3(NsXml.ReadLabel(pt)));
				}
			}
			Fit(combs);
		}

		#endregion
	}
}
