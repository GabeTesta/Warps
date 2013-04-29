using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBF;
using devDept.Geometry;
using devDept.Eyeshot.Entities;

namespace Warps
{
	public class RBFMould : ISurface
	{
		RBFSurface[] m_rbfs = new RBFSurface[3];
		string m_label;
		string m_path = null;
		double m_error = -1;

		public RBFMould() { }
		public RBFMould(Sail sail, string cofpath)
		{
			ReadCofFile(sail, cofpath);
		}
		public RBFMould(string label, ISurface cof)
		{
			m_label = label;
			Fit(cof);
		}
		public RBFMould(ISurface cof)
		{
			m_label = "RBF" + cof.Label;
			Fit(cof);
		}

		public void ReadCofFile(Sail sail, string cofpath)
		{
			CofMould cof = new CofMould(sail, cofpath);
			m_label = "RBF" + cof.Label;
			m_path = cof.CofPath;
			Fit(cof);
		}

		double Fit(ISurface cof)
		{
			if( cof == null )
			{
				m_rbfs = null;
				return -1;
			}
			int i, j, k;
			int ROWS =15, COLS =15;

			List<double[]>[] uvxs = new List<double[]>[3];
			for(i =0; i<3;i++ )
				uvxs[i] = new List<double[]>(ROWS*COLS);
			
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			for(i =0; i<ROWS;i++ )
			{
				uv[0] = BLAS.interpolant(i, ROWS);
				for( j=0; j<COLS; j++ )
				{
					uv[1] = BLAS.interpolant(j, COLS);
					cof.xVal(uv, ref xyz);
					for( k =0; k<3;k++ )
						uvxs[k].Add(new double[]{ uv[0], uv[1], xyz[k]});	
				}
			}
			for (i = 0; i < 3; i++)
				m_rbfs[i] = new RBFSurface(uvxs[i]);

			return m_error = CheckError(cof);
		}

		double CheckError(ISurface cof)
		{
			int ROWS = 30, COLS = 30;

			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			int i, j;
			double kThis=0, kCof=0, err=0;
			for (i = 0; i < ROWS; i++)
			{
				uv[0] = BLAS.interpolant(i, ROWS);
				for (j = 0; j < COLS; j++)
				{
					uv[1] = BLAS.interpolant(j, COLS);
					cof.xRad(uv, ref xyz, ref kCof);
					xRad(uv, ref xyz, ref kThis);
					err = Math.Pow(kCof - kThis, 2);
				}
			}
			err /= (ROWS*COLS);
			return err;
		}

		#region ISurface Members

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			double[] p = new double[3];
			uv.m_vec.CopyTo(p, 0);
			for (int i = 0; i < m_rbfs.Length; i++)
			{
				p[2] = 0;
				m_rbfs[i].Value(ref p);
				xyz[i] = p[2];
			}
		}

		public void xVec(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv)
		{
			double[] p = new double[3], d = new double[3];
			uv.m_vec.CopyTo(p, 0);
			for (int i = 0; i < m_rbfs.Length; i++)
			{
				p[2] = 0;
				m_rbfs[i].First(ref p, ref d);
				xyz[i] = p[2];
				dxu[i] = d[0];
				dxv[i] = d[1];
			}
		}

		public void xCvt(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv, ref Vect3 ddxu, ref Vect3 ddxv, ref Vect3 dduv)
		{
			double[] p = new double[3], d = new double[3], dd = new double[3];
			uv.m_vec.CopyTo(p, 0);
			for (int i = 0; i < m_rbfs.Length; i++)
			{
				p[2] = 0;
				m_rbfs[i].Second(ref p, ref d, ref dd);
				xyz[i] = p[2];
				dxu[i] = d[0];
				dxv[i] = d[1];
				ddxu[i] = dd[0];
				ddxv[i] = dd[1];
				dduv[i] = dd[2];
			}
		}

		public void xNor(Vect2 uv, ref Vect3 xyz, ref Vect3 xnor)
		{
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			xVec(uv, ref xyz, ref dxu, ref dxv);

			xnor = dxu.Cross(dxv);
			xnor.Magnitude = 1;
		}

		public void xRad(Vect2 uv, ref Vect3 xyz, ref double k)
		{
			Vect3 dxu = new Vect3(), dxv = new Vect3(), ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3(), xnor = new Vect3();
			xCvt(uv, ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);
			xNor(uv, ref xyz, ref xnor);

			//calculate first fundamental form
			double E = dxu.Norm;
			double F = dxu.Dot(dxv);
			double G = dxv.Norm;
			//double E = BLAS.dot(dxu, dxu);
			//double F = BLAS.dot(dxu, dxv);
			//double G = BLAS.dot(dxv, dxv);
			double detI = E * G - F * F;
			//calculate second fundamental form
			double e = xnor.Dot(ddxu);
			double f = xnor.Dot(dduv);
			double g = xnor.Dot(ddxv);
			//double e = BLAS.dot(ddxu, xnor);
			//double f = BLAS.dot(dduv, xnor);
			//double g = BLAS.dot(ddxv, xnor);
			double detII = e * g - f * f;

			k = detII / detI;
		}

		public bool xClosest(ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol)
		{
			return SurfaceTools.xClosest(this, ref uv, ref xyzTarget, ref dist, tol);
		}

		public string Label
		{
			get { return m_label; }
		}

		public List<Entity> CreateEntities(double[,] uvLims, bool bGauss)
		{
			List<Entity> ents = new List<Entity>();
			ents.Add(SurfaceTools.GetMesh(this, uvLims, bGauss));
			return ents;
			//m_entities.Add(SurfaceTools.GetMesh(this, true));
			//m_entities.Last().LayerIndex 
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count == 0)
				return false;
			string line = ScriptTools.ReadLabel(txt[0]);
			if (line != null)
			{
				ReadCofFile(sail, line);
				return true;
			}
			return false;
		}
		public List<string> WriteScript()
		{
			List<string> s = new List<string>();
			s.Add(ScriptTools.Label(GetType().Name, m_path == null ? Label : m_path));
			return s;
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = string.Format("{0}: {1}", GetType().Name, Label);
			m_node.Tag = this;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.Nodes.Clear();
			if( m_path != null && m_path.Length > 0 )
				m_node.Nodes.Add("Path: " + m_path);
			m_node.Nodes.Add("Error: " + m_error);
			return m_node;
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}


	}
}
