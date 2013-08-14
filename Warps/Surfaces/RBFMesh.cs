using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace Warps.Surfaces
{
	class RBFMesh : ISurface
	{
		public RBFMesh(string path)
		{
			ReadObjFile(path);
		}
		public devDept.Eyeshot.ReadOBJ m_obj;
		/// <summary>
		/// Wavefront Obj file reader
		/// v x y z
		/// vt u v
		/// </summary>
		/// <param name="path">the path to the file</param>
		private void ReadObjFile(string path)
		{
			m_obj = new devDept.Eyeshot.ReadOBJ(path, false);
			m_obj.DoWork();
			if (!m_obj.Result)
				throw new Exception("Failed to read obj file");

			Mesh m = m_obj.Entities[0] as Mesh;
			if( m == null )
				throw new Exception("Failed to mesh obj file");
			
			List<double[]> uvs = new List<double[]>(m.Vertices.Length);
			List<double[]> xyz = new List<double[]>(m.Vertices.Length);
			for(int nV =0; nV < m.Vertices.Length; nV++)
			{			
				uvs.Add(m.TextureCoords[nV].ToArray());
				xyz.Add(m.Vertices[nV].ToArray());
			}
			m_path = path;
			m_rbf.Fit(uvs, xyz);
		}

		string m_path = null;
		RBF.RBFNetwork m_rbf = new RBF.RBFNetwork();
		
		#region ISurface Members

		public string Label
		{
			get { return m_path == null? "null" : Path.GetFileNameWithoutExtension(m_path); }
		}

		public List<IGroup> Groups
		{
			get { return null; }//no groups in a mesh file
		}

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			m_rbf.Value(uv.m_vec, ref xyz.m_vec);
		}

		public void xVec(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv)
		{
			throw new NotImplementedException("RBFNetwork derivatives do not work");
			//double[,] dxy = new double[3,2];
			//m_rbf.First(uv.m_vec, ref xyz.m_vec, ref dxy);
			//for (int i = 0; i < 3; i++)
			//{
			//	dxu[i] = dxy[i, 0];
			//	dxv[i] = dxy[i, 1];
			//}
		}

		public void xCvt(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv, ref Vect3 ddxu, ref Vect3 ddxv, ref Vect3 dduv)
		{
			throw new NotImplementedException("xCvt");
		}

		public void xNor(Vect2 uv, ref Vect3 xyz, ref Vect3 xnor)
		{
			throw new NotImplementedException("xNor");
		}

		public void xRad(Vect2 uv, ref Vect3 xyz, ref double k)
		{
			throw new NotImplementedException("xRad");
		}

		public bool xClosest(ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol)
		{
			throw new NotImplementedException("xClosest");
		}

		public List<string> WriteScript()
		{
			throw new NotImplementedException("WriteScript");
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException("ReadScript");
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities(double[,] uvLims, bool bGauss)
		{
			//return m_obj.Entities.ToList() ;
			List<Entity> ents = new List<Entity>();
			ents.Add(SurfaceTools.GetMesh(this, uvLims, bGauss));
			return ents;
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = Label;
			m_node.Tag = this;
			m_node.ToolTipText = m_node.ImageKey = m_node.SelectedImageKey = GetType().Name;
			return m_node;
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}

		#region ISurface Members


		double[] m_colors = null;
		public double[] ColorValues
		{
			get
			{
				return m_colors;
			}
			set
			{
				m_colors = value;
			}
		}


		#endregion
	}

	class RBFOBJ : ISurface
	{
		RBF.RBFSurface[] m_rbfs = new RBF.RBFSurface[3];
		string m_label;

		public RBFOBJ(string path)
		{
			ReadObjFile(path);
		}
		public devDept.Eyeshot.ReadOBJ m_obj;
		/// <summary>
		/// Wavefront Obj file reader
		/// v x y z
		/// vt u v
		/// </summary>
		/// <param name="path">the path to the file</param>
		private void ReadObjFile(string path)
		{
			m_obj = new devDept.Eyeshot.ReadOBJ(path, false);
			m_obj.DoWork();
			if (!m_obj.Result)
				throw new Exception("Failed to read obj file");

			Mesh m = m_obj.Entities[0] as Mesh;
			if( m == null )
				throw new Exception("Failed to mesh obj file");

			int i, hash, xhash, nDbl = 0;
			HashSet<int> hashs = new HashSet<int>();
			List<double[]>[] uvxs = new List<double[]>[3];
			for (i = 0; i < 3; i++)
				uvxs[i] = new List<double[]>(m.Vertices.Length);
			for(int nV =0; nV < m.Vertices.Length; nV++)
			{
				hash = m.TextureCoords[nV].GetHashCode();
				xhash = m.Vertices[nV].GetHashCode();
				if (hashs.Contains(hash) || hashs.Contains(xhash))
				{
					nDbl++;
					continue;//skip non-unique uv coords
				}
				hashs.Add(xhash);
				hashs.Add(hash);

				for (i = 0; i < 3; i++)
					uvxs[i].Add(new double[] { m.TextureCoords[nV].X, m.TextureCoords[nV].Y, m.Vertices[nV][i] });
			}


			for (i = 0; i < 3; i++)
				m_rbfs[i] = new RBF.RBFSurface(uvxs[i]);
			m_label = path;
			
		}

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
			//ents.Add( SurfaceTools.GetMesh(m_rbfs[0].GetMeshPoints(10,10), 10) ); 
			//ents.Add( SurfaceTools.GetMesh(m_rbfs[1].GetMeshPoints(10,10), 10) ); 
			//ents.Add( SurfaceTools.GetMesh(m_rbfs[2].GetMeshPoints(10,10), 10) ); 
			return ents;
			//m_entities.Add(SurfaceTools.GetMesh(this, true));
			//m_entities.Last().LayerIndex 
		}


		#region ISurface Members


		public List<IGroup> Groups
		{
			get { return null; }
		}

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = Label;
			m_node.Tag = this;
			m_node.ToolTipText = m_node.ImageKey = m_node.SelectedImageKey = GetType().Name;
			return m_node;
		}

		double[] m_colors = null;
		public double[] ColorValues
		{
			get
			{
				return m_colors;
			}
			set
			{
				m_colors = value;
			}
		}

		#endregion
	}
}
