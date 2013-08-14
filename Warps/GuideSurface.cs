using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Geometry;
using System.Drawing;

namespace Warps
{
	public class GuideSurface : IRebuild
	{
		public GuideSurface(string label, Sail s, List<Vect3> fits)
		{
			m_sail = s;
			Label = label;
			Fit(fits);
		}

		Sail m_sail;
		string m_label;
		List<Vect3> m_fits;
		RBF.RBFSurface m_surf;

		public RBF.RBFSurface RBF
		{
			get { return m_surf; }
			set { m_surf = value; }
		}

		public List<Vect3> FitPoints
		{
			get { return m_fits; }
			set { m_fits = value; }
		}

		private void Fit(List<Vect3> fits)
		{
			m_fits = fits;
			ReFit();
		}
		private void ReFit()
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

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

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
			if( m_node == null )
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = Label;
			m_node.Tag = this;
			m_node.Nodes.Clear();
			if (m_fits != null)
				foreach (Vect3 e in m_fits)
					m_node.Nodes.Add(new System.Windows.Forms.TreeNode(e.ToString("f3")));
			return m_node;
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities()
		{
			List<devDept.Eyeshot.Entities.Entity> ents = new List<devDept.Eyeshot.Entities.Entity>();
			int[] MESH = new int[] { 25, 30 };
			double SCALE = .1;
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
			devDept.Eyeshot.Entities.Mesh m = SurfaceTools.GetMesh(mesh,color);
			m.EntityData = this;
			ents.Add(m);
			Vect3[] cloud = new Vect3[m_fits.Count];
			int nPt = 0;
			foreach (Vect3 v in m_fits)
			{
				uv.Set(v);
				m_sail.Mould.xNor(uv, ref xyz, ref nor);
				cloud[nPt++] = xyz + (nor * v[2] * SCALE);
			}
			ents.Add(SurfaceTools.GetPointCloud(cloud));
			ents.Last().EntityData = this;
			return ents;
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				devDept.Eyeshot.Labels.Label[] lbls = new devDept.Eyeshot.Labels.Label[1];
				double[] c = new double[]{0.5,0.5,0};
				m_surf.Value(ref c);
				lbls[0] = new devDept.Eyeshot.Labels.OutlinedText(new Point3D(c), Label, new Font("Helvectiva", 8.0f), Color.White, Color.Black, ContentAlignment.MiddleCenter);
				return lbls;
			}
		}

		public void GetConnected(List<IRebuild> updated)
		{

		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			throw new NotImplementedException();
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
	}
}
