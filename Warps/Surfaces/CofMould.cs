using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Windows.Forms;

namespace Warps
{
	public class CofMould : ISurface
	{
		public CofMould() { }
		public CofMould(Sail sail, string cofPath)
		{
			//ReadMFCCofFile(cofPath);
			ReadCofFile(sail, cofPath);
		}

		#region Cof

		string m_cofPath;
		public string Label
		{
			get { return m_cofPath == null ? "" : Path.GetFileNameWithoutExtension(m_cofPath); }
		}
		public string CofPath
		{
			get { return m_cofPath; }
		}

		public List<IGroup> Groups
		{
			get { return m_moucurves; }
		}		

		public void ReadCofFile(Sail sail, string cofPath)
		{
			if( cofPath != null )
				m_cofPath = cofPath;

			if (Path.GetExtension(m_cofPath).ToLower() == ".cof")
				ReadMFCCofFile(cofPath);
			else
				using (BinaryReader bin = new BinaryReader(File.Open(cofPath, FileMode.Open, FileAccess.Read), Encoding.ASCII))
				{
					m_text = Utilities.ReadCString(bin);

					ReadBinShape(bin);

					ReadBinCurves(bin, sail);
				}

		}

		void ReadMFCCofFile(string cofPath)
		{
			m_cofPath = cofPath;
			using (BinaryReader bin = new BinaryReader(File.Open(cofPath, FileMode.Open, FileAccess.Read), Encoding.ASCII))
			{
				string name = new string(bin.ReadChars(100));
				m_text = name;// new string(name);
				int end = m_text.IndexOf('\0');
				if (end > 0)
					m_text = m_text.Substring(0, end);

				ReadBinShape(bin);

				double[,] xMat = new double[3, 3];
				double[] xVec = new double[3];
				for (int i = 0; i < 3; i++)
					for (int j = 0; j < 3; j++)
						xMat[i, j] = bin.ReadDouble();
				for (int i = 0; i < 3; i++)
					xVec[i] = bin.ReadDouble();

				int iC = bin.ReadInt32();
				byte[] DesMsm = bin.ReadBytes(iC);

			}
		}

		#region .Sail Bin file

		void ReadBinShape(BinaryReader bin)
		{
			m_Kn0t[0] = bin.ReadInt32();
			m_Kn0t[1] = bin.ReadInt32();
			m_Kn0t[2] = bin.ReadInt32();

			m_Kn2t[0] = bin.ReadInt32();
			m_Kn2t[1] = bin.ReadInt32();
			m_Kn2t[2] = bin.ReadInt32();

			m_Kn3t[0] = bin.ReadInt32();
			m_Kn3t[1] = bin.ReadInt32();
			m_Kn3t[2] = bin.ReadInt32();

			m_Kn5t[0] = bin.ReadInt32();
			m_Kn5t[1] = bin.ReadInt32();
			m_Kn5t[2] = bin.ReadInt32();

			for (int j = 0; j < 3; j++)
				for (int i = 0; i < 21; i++)
					m_xKnot[j, i] = bin.ReadDouble();

			for (int j = 0; j < 3; j++)
				for (int i = 0; i < 3; i++)
					for (int k = 0; k < m_Kn3t[0]; k++)
						m_CofCur[j, i, k] = bin.ReadDouble();

			for (int j = 0; j < m_Kn3t[2]; j++)
				for (int i = 0; i < 2; i++)
					for (int k = 0; k < m_Kn3t[1]; k++)
						m_CofSur[i, j, k] = bin.ReadDouble();

			for (int j = 0; j < m_Kn3t[1]; j++)
				for (int i = 0; i < 2; i++)
					m_CofFot[i, j] = bin.ReadDouble();

		}
		List<IGroup> m_moucurves;
		List<RigLine> m_rigcurves;

		public List<RigLine> RigCurves
		{
			get { return m_rigcurves; }
			set { m_rigcurves = value; }
		}
		void ReadBinCurves(BinaryReader bin, Sail sail)
		{
			//read 3d curve count
			int iC = bin.ReadInt32();
			m_rigcurves = new List<RigLine>(iC);
			//read curves from bin file
			for (int nC = 0; nC < iC; nC++)
				m_rigcurves.Add(new RigLine(bin));

			//read group count
			iC = bin.ReadInt32();
			m_moucurves = new List<IGroup>(iC);
			//read curvegroups from bin file
			for (int nC = 0; nC < iC; nC++)
				m_moucurves.Add(new CurveGroup(bin, sail));

		}

		#endregion

		#endregion

		#region Coefficients
		string m_text;

		int[] m_Kn0t = new int[3];
		int[] m_Kn2t = new int[3];
		int[] m_Kn3t = new int[3];
		int[] m_Kn5t = new int[3];

		double[,] m_xKnot = new double[3, 21];
		double[, ,] m_CofCur = new double[3, 3, 19];
		double[, ,] m_CofSur = new double[2, 19, 19];
		double[,] m_CofFot = new double[2, 19];

		//double[,] m_xMat = new double[3, 3];
		//double[] m_xVec = new double[3]; 
		#endregion

		#region ISurface Members

		private void BsCil(int iDer, double t, int iK, out int nInterval, ref double[,] basis)
		{
			int k;
			//find knot interval
			for (k = 3; k <= m_Kn2t[iK]; k++)
				if (t < m_xKnot[iK, k]) break;
			//catch overrun
			if (k > m_Kn2t[iK])
				k = m_Kn2t[iK];

			nInterval = k - 3;

			double sm3, sm2, sm1, s00, sp1, sp2, sm10, sm22, sm21, sm32, sm31, sm30, a1, a2, a3;

			sm3 = m_xKnot[iK, k - 3]; sm2 = m_xKnot[iK, k - 2]; sm1 = m_xKnot[iK, k - 1];
			s00 = m_xKnot[iK, k]; sp1 = m_xKnot[iK, k + 1]; sp2 = m_xKnot[iK, k + 2];

			sm10 = 1.0 / (s00 - sm1);
			//	recursion-formula
			sm22 = (s00 - t) * sm10 / (s00 - sm2);
			sm21 = (t - sm1) * sm10 / (sp1 - sm1);
			//	recursion-formula
			sm32 = (s00 - t) * sm22 / (s00 - sm3);
			sm31 = ((t - sm2) * sm22 + (sp1 - t) * sm21) / (sp1 - sm2);
			sm30 = (t - sm1) * sm21 / (sp2 - sm1);

			basis[0, 0] = (s00 - t) * sm32;
			basis[0, 1] = (t - sm3) * sm32 + (sp1 - t) * sm31;
			basis[0, 2] = (t - sm2) * sm31 + (sp2 - t) * sm30;
			basis[0, 3] = (t - sm1) * sm30;
			//	zero order B-splines
			if (iDer == 0) return;

			basis[1, 0] = 3.0 * (-sm32);
			basis[1, 1] = 3.0 * (sm32 - sm31);
			basis[1, 2] = 3.0 * (sm31 - sm30);
			basis[1, 3] = 3.0 * (sm30);
			//	zero & first order B-splines
			if (iDer == 1) return;

			a1 = 6.0 * (-sm22) / (s00 - sm3);
			a2 = 6.0 * (sm22 - sm21) / (sp1 - sm2);
			a3 = 6.0 * (sm21) / (sp2 - sm1);
			basis[2, 0] = -a1;
			basis[2, 1] = a1 - a2;
			basis[2, 2] = a2 - a3;
			basis[2, 3] = a3;
			//	zero & first & second order B-splines
			if (iDer == 2) return;

			sm22 = -sm10 / (s00 - sm2);
			sm21 = sm10 / (sp1 - sm1);
			//	calculate  sm22,sm21  derivatives for  a1,a2,a3
			a1 = 6.0 * (-sm22) / (s00 - sm3);
			a2 = 6.0 * (sm22 - sm21) / (sp1 - sm2);
			a3 = 6.0 * (sm21) / (sp2 - sm1);
			basis[3, 0] = -a1;
			basis[3, 1] = a1 - a2;
			basis[3, 2] = a2 - a3;
			basis[3, 3] = a3;
			//	zero & first & second & third order B-splines
		}
		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			double[,] basisU = new double[4, 4];
			double[,] basisV = new double[4, 4];
			int nS, nB, nU, nV, nVal;

			double[,] xz = new double[m_CofCur.GetLength(0), 3];
			double[] st = new double[m_CofSur.GetLength(0)];

			BsCil(0, uv[1], 0, out nS, ref basisU);

			for (int ix = 0; ix < 3; ix++)
				for (nB = 0, nVal = nS; nB < 4; nB++, nVal++)
					for (int i = 0; i < xz.GetLength(0); i++)
						xz[i, ix] += m_CofCur[i, ix, nVal] * basisU[0, nB];

			BsCil(0, uv[0], 1, out nVal, ref basisU);
			BsCil(0, uv[1], 2, out nV, ref basisV);

			for (nB = 0; nB < 4; nB++, nV++)
				for (nU = nVal, nS = 0; nS < 4; nS++, nU++)
					for (int i = 0; i < st.Length; i++)
						st[i] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[0, nB];

			for (int ix = 0; ix < 3; ix++)
			{
				xyz[ix] = xz[0, ix];
				for (int i = 0; i < st.Length; i++)
					xyz[ix] += st[i] * xz[i + 1, ix];
			}

		}
		public void xVec(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv)
		{
			double[,] basisU = new double[4, 4];
			double[,] basisV = new double[4, 4];
			int nS, nB, nU, nV, nVal;
			double[, ,] xz = new double[m_CofCur.GetLength(0), 2, 3];
			double[,] st = new double[m_CofSur.GetLength(0), 3];

			BsCil(1, uv[1], 0, out nS, ref basisU);

			for (int ix = 0; ix < 3; ix++)
				for (nB = 0, nVal = nS; nB < 4; nB++, nVal++)
					for (int i = 0; i < xz.GetLength(0); i++)
					{
						xz[i, 0, ix] += m_CofCur[i, ix, nVal] * basisU[0, nB];
						xz[i, 1, ix] += m_CofCur[i, ix, nVal] * basisU[1, nB];
					}

			BsCil(1, uv[0], 1, out nVal, ref basisU);
			BsCil(1, uv[1], 2, out nV, ref basisV);

			for (nB = 0; nB < 4; nB++, nV++)
				for (nU = nVal, nS = 0; nS < 4; nS++, nU++)
					for (int i = 0; i < st.GetLength(0); i++)
					{
						st[i, 0] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[0, nB];
						st[i, 1] += m_CofSur[i, nV, nU] * basisU[1, nS] * basisV[0, nB];
						st[i, 2] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[1, nB];
					}

			for (int ix = 0; ix < 3; ix++)
			{
				xyz[ix] = xz[0, 0, ix];
				dxu[ix] = 0;
				dxv[ix] = xz[0, 1, ix];
				for (int i = 0; i < st.GetLength(0); i++)
				{
					xyz[ix] += st[i, 0] * xz[i + 1, 0, ix];

					dxu[ix] += st[i, 1] * xz[i + 1, 0, ix];

					dxv[ix] += st[i, 2] * xz[i + 1, 0, ix];
					dxv[ix] += st[i, 0] * xz[i + 1, 1, ix];
				}
			}

		}
		public void xCvt(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv, ref Vect3 ddxu, ref Vect3 ddxv, ref Vect3 dduv)
		{
			double[,] basisU = new double[4, 4];
			double[,] basisV = new double[4, 4];
			int nS, nB, nU, nV, nVal;
			double[, ,] xz = new double[m_CofCur.GetLength(0), 3, 3];
			double[, ,] st = new double[m_CofSur.GetLength(0), 2, 3];



			BsCil(2, uv[1], 0, out nS, ref basisU);

			for (int ix = 0; ix < 3; ix++)
				for (nB = 0, nVal = nS; nB < 4; nB++, nVal++)
					for (int i = 0; i < xz.GetLength(0); i++)
					{
						xz[i, 0, ix] += m_CofCur[i, ix, nVal] * basisU[0, nB];
						xz[i, 1, ix] += m_CofCur[i, ix, nVal] * basisU[1, nB];
						xz[i, 2, ix] += m_CofCur[i, ix, nVal] * basisU[2, nB];
					}

			BsCil(2, uv[0], 1, out nVal, ref basisU);
			BsCil(2, uv[1], 2, out nV, ref basisV);

			for (nB = 0; nB < 4; nB++, nV++)
				for (nU = nVal, nS = 0; nS < 4; nS++, nU++)
					for (int i = 0; i < st.GetLength(0); i++)
					{
						st[i, 0, 0] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[0, nB];//s00

						st[i, 0, 1] += m_CofSur[i, nV, nU] * basisU[1, nS] * basisV[0, nB];//s01
						st[i, 0, 2] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[1, nB];//s02

						st[i, 1, 0] += m_CofSur[i, nV, nU] * basisU[2, nS] * basisV[0, nB];//s11
						st[i, 1, 1] += m_CofSur[i, nV, nU] * basisU[1, nS] * basisV[1, nB];//s12
						st[i, 1, 2] += m_CofSur[i, nV, nU] * basisU[0, nS] * basisV[2, nB];//s22
					}

			for (int ix = 0; ix < 3; ix++)
			{
				xyz[ix] = xz[0, 0, ix];
				dxu[ix] = 0;
				dxv[ix] = xz[0, 1, ix];
				ddxu[ix] = 0;
				ddxv[ix] = xz[0, 2, ix];
				dduv[ix] = 0;
				for (int i = 0; i < st.GetLength(0); i++)
				{
					xyz[ix] += st[i, 0, 0] * xz[i + 1, 0, ix];

					dxu[ix] += st[i, 0, 1] * xz[i + 1, 0, ix];

					dxv[ix] += st[i, 0, 2] * xz[i + 1, 0, ix];
					dxv[ix] += st[i, 0, 0] * xz[i + 1, 1, ix];

					ddxu[ix] += st[i, 1, 0] * xz[i + 1, 0, ix];
					ddxv[ix] += st[i, 1, 2] * xz[i + 1, 0, ix]
						   + st[i, 0, 2] * xz[i + 1, 1, ix]
						   + st[i, 0, 2] * xz[i + 1, 1, ix]
						   + st[i, 0, 0] * xz[i + 1, 2, ix];
					dduv[ix] += st[i, 0, 1] * xz[i + 1, 1, ix]
						   + st[i, 1, 1] * xz[i + 1, 0, ix];
				}
			}
		}
		public void xNor(Vect2 uv, ref Vect3 xyz, ref Vect3 xnor)
		{
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			xVec(uv, ref xyz, ref dxu, ref dxv);

			xnor = dxu.Cross(dxv);
			xnor.Magnitude = 1;
			//BLAS.unitize(ref xnor);
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
			Vect3 x = new Vect3(xyzTarget);
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();

			Vect3 h = new Vect3();
			Vect2 c = new Vect2();
			Vect2 res = new Vect2();
			Vect2 a = new Vect2(), b = new Vect2();
			double det, r;
			Vect2 d = new Vect2();

			int loop = 0, max_loops = 150;
			while (loop++ < max_loops)
			{
				xCvt(uv, ref x, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

				h = x - xyzTarget;
				//h = BLAS.subtract(x, xyzTarget);
				dist = h.Magnitude;

				//e[0] = s;
				c[0] = h.Dot(dxu);// BLAS.dot(h, dxu); // error, dot product is 0 at pi/2
				c[1] = h.Dot(dxv);// BLAS.dot(h, dxv); // error, dot product is 0 at pi/2

				if (Math.Abs(c[0]) < tol && Math.Abs(c[1]) < tol) // error is less than the tolerance
				{
					xyzTarget.Set(x);// return point to caller
					return true;
				}

				a[0] = dxu.Norm + h.Dot(ddxu);
				a[1] = b[0] = dxu.Dot(dxv) + h.Dot(dduv);
				b[1] = dxv.Norm + h.Dot(ddxv);

				//a[0] = BLAS.dot(dxu, dxu) + BLAS.dot(h, ddxu);
				//a[1] = BLAS.dot(dxu, dxv) + BLAS.dot(h, dduv);
				//b[0] = a[1];
				//b[1] = BLAS.dot(dxv, dxv) + BLAS.dot(h, ddxv);

				det = a.Cross(b);
				//det = BLAS.cross2d(a, b);

				d[0] = c.Cross(b) / det;
				d[1] = a.Cross(c) / det;
				//d[0] = BLAS.cross2d(c, b) / det;
				//d[1] = BLAS.cross2d(a, c) / det;

				c[0] = 0.01 > Math.Abs(d[0]) ? 1 : 0.01 / Math.Abs(d[0]);
				c[1] = 0.01 > Math.Abs(d[1]) ? 1 : 0.01 / Math.Abs(d[1]);
				//enforce maximum increment
				r = Math.Min(c[0], c[1]);

				//increment uv by scaled residuals
				//uv = BLAS.subtract(uv, BLAS.scale(d, r));
				uv = uv - d * r;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}
			//s = s0;
			return false;
		}

		public List<Entity> CreateEntities(double[,] uvLims, bool bGauss)
		{
			List<Entity> ents = new List<Entity>();
			ents.Add(SurfaceTools.GetMesh(this, uvLims, bGauss));
			List<Entity> grp;
			if (!bGauss)//only add curves to non-gauss layer
			{
				if (m_rigcurves != null)
					foreach (RigLine rig in m_rigcurves)
					{
						ents.Add(rig.CreateEntities());
						ents[ents.Count - 1].EntityData = this;
					}
				if (m_moucurves != null)
					foreach (IGroup g in m_moucurves)
					{
						grp = g.CreateEntities();
						foreach (Entity e in grp)
							e.EntityData = this;
						ents.AddRange(grp);	
					}
			}
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
				ReadMFCCofFile(line);
				return true;
			}
			return false;
		}
		public List<string> WriteScript()
		{
			List<string> s = new List<string>();
			s.Add(ScriptTools.Label(GetType().Name, m_cofPath));
			return s;
		}

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = CofPath;
			m_node.Tag = this;
			m_node.ToolTipText = m_node.ImageKey = m_node.SelectedImageKey = GetType().Name;
			m_node.Nodes.Clear();
			if( m_moucurves != null )
			foreach (IGroup g in m_moucurves)
				m_node.Nodes.Add(g.WriteNode());
			var tn = m_node.Nodes.Add("Rig Curves");
			if( m_rigcurves != null )
			foreach (RigLine rig in m_rigcurves)
				tn.Nodes.Add(rig.ToString());
			return m_node;
		}

		#endregion

		public override string ToString()
		{
			return Label;
			//return string.Format("{0}: {1} [{2}]", GetType().Name, Label, CofPath); 
		}
		//List<Entity> m_entities = new List<Entity>();

		//public List<Entity> Entities
		//{
		//	get { CreateEntities(); return m_entities; }
		//	set { m_entities = value; }
		//}
		//private void CreateEntities()
		//{
		//	m_entities.Clear();
		//	m_entities.Add(GetMesh(120, 120));
		//}

		//Mesh GetMesh(int rows, int cols)
		//{
		//	Mesh mesh = new Mesh(meshNatureType.RichPlain);
		//	//mesh.ColorMethod = colorMethodType.byEntity;
		//	mesh.RegenMode = regenType.RegenAndCompile;
		//	mesh.Vertices = SurfaceTools.GetMeshPoints(this, rows, cols);
		//	//mesh.Color = Color.Silver;
		//	mesh.Triangles = new IndexTriangle[(rows - 1) * (cols - 1) * 2];
		//	int count = 0;
		//	for (int j = 0; j < (rows - 1); j++)
		//	{
		//		for (int i = 0; i < (cols - 1); i++)
		//		{

		//			mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
		//														   i + j * cols + 1,
		//														   i + (j + 1) * cols + 1);
		//			mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
		//														   i + (j + 1) * cols + 1,
		//														   i + (j + 1) * cols);
		//		}
		//	}

		//	mesh.ComputeEdges();
		//	//mesh.ComputeNormals();
		//	mesh.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;
		//	mesh.EntityData = this;
		//	mesh.Selectable = false;
		//	return mesh;
		//}

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
}
