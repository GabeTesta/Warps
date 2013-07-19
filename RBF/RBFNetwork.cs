using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBFBasis;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace RBF
{
	public class RBFNetwork
	{
		public RBFNetwork()
			: this(null)//null defaults to Thin Plate Spline
		{ }
		private RBFNetwork(IBasisFunction basis)
		{
			m_basis = basis == null ? new ThinPlateSpline() : basis;
		}

		IBasisFunction m_basis;
		double[,] m_Weights;
		List<double[]> m_Centers;
		int CenterDims { get { return m_Centers != null && m_Centers.Count > 0 ? m_Centers[0].Length : 0; } }
		int Dimensions {get{ return m_Weights.GetLength(1); } }
		int Count { get { return m_Centers.Count; } }

		public void Fit(IList<double[]> uvs, IList<double[]> xyz)
		{
			if (uvs.Count != xyz.Count || xyz.Count == 0 || uvs.Count == 0 || xyz[0].Length == 0)
				return;

			m_Centers = new List<double[]>(uvs);
			double[,] A = FitMat();

			m_Weights = new double[A.GetLength(0), xyz[0].Length];
			double[][] b = new double[Dimensions][];
			int nx = 0, i;
			for (nx = 0; nx < Dimensions; nx++)
			{
				b[nx] = new double[A.GetLength(0)];//include 0's for polyterms
				for (i = 0; i < xyz.Count; i++)
					b[nx][i] = xyz[i][nx];
	
			}

			for (nx = 0; nx < Dimensions; nx++)
				Solve(A, b[nx], nx);
		}

		double[,] FitMat()
		{
			int fits = Count + CenterDims + 1;// for polynomial terms

			double[,] A = new double[fits, fits];
			// create A matrix
			double r;
			double a = 0;//for scaling relaxation parameter
			int i, j;
			for (i = 0; i < Count; ++i)
			{
				for (j = i + 1; j < Count; ++j)
				{
					// PHI11 - PHINN
					r = BLAS.distance(m_Centers[i], m_Centers[j]);
					A[i, j] = A[j, i] = m_basis.val(r); // symmetric
					a += 2 * r;
				}
			}
			a /= Count * Count; // calculate relaxation normalizer

			// calculate fit mat diagonals and poly terms
			double relax = 0;
			for (i = 0; i < Count; ++i)
			{
				A[i, i] = a * a * relax;

				for (j = 0; j <= CenterDims; j++)// polyvalues: Au + Bv + C
					A[i, Count + j] = A[Count + j, i] = j < CenterDims ? m_Centers[i][j] : 1;
			}
			return A;
		}
		void Solve(double[,] A, double[] b, int nW)
		{
			var matrixA = new DenseMatrix(A);
			var vectorB = new DenseVector(b);
			Vector<double> resultX = matrixA.LU().Solve(vectorB);

			for (int nPos = 0; nPos < resultX.Count; nPos++)
			{
				m_Weights[nPos, nW] = resultX[nPos];
			}
		}





		public void Value(double[] uv, ref double[] xyz)
		{
			if (uv.Length != CenterDims || xyz.Length != Dimensions)
				return;

			double rad;
			int nx, nCent;
			for (nx = 0; nx < xyz.Length; nx++)
				xyz[nx] = 0;//initialize x
			for (nCent = 0; nCent < Count; nCent++)
			{
				rad = m_basis.val( BLAS.distance(m_Centers[nCent], uv) );
				for (nx = 0; nx < Dimensions; nx++)
				{
					xyz[nx] += m_Weights[nCent, nx] * rad;
				}
			}
			for (int nPoly = 0; nPoly <= CenterDims; nPoly++)
			{
				for (nx = 0; nx < Dimensions; nx++)//poly terms: linear Au + Bv + ... + D
					xyz[nx] += nPoly < CenterDims ? m_Weights[nCent + nPoly, nx] * uv[nPoly] : m_Weights[nCent + nPoly, nx];
			}

		}
		public void First(double[] uv, ref double[] xyz, ref double[,] dxyz)
		{
			if (uv.Length != CenterDims || xyz.Length != Dimensions || dxyz.GetLength(0) != CenterDims || dxyz.GetLength(1) != Dimensions)
				return;

			double rad, drad;
			int nx, nCent;
			for (nx = 0; nx < xyz.Length; nx++)
			{
				xyz[nx] = 0;//initialize x
				for (nCent = 0; nCent < CenterDims; nCent++)
					dxyz[nCent,nx] =0;
			}
			for (nCent = 0; nCent < Count; nCent++)
			{
				rad = BLAS.distance(m_Centers[nCent], uv);
				//drad = m_basis.dr(rad);
				rad = m_basis.val(rad);
				for (nx = 0; nx < Dimensions; nx++)
				{
					xyz[nx] += m_Weights[nCent, nx] * rad;
					//dxyz[nCent, nx] += m_Weights[nCent, nx] * drad;
				}
			}
			for (int nPoly = 0; nPoly <= CenterDims; nPoly++)
			{
				for (nx = 0; nx < Dimensions; nx++)//poly terms: linear Au + Bv + ... + D
					xyz[nx] += nPoly < CenterDims ? m_Weights[nCent + nPoly, nx] * uv[nPoly] : m_Weights[nCent + nPoly, nx];
			}
		}

	}

	//public class RBFNetwork2
	//{
	//	public RBFNetwork2(uint nDim)
	//		: this(nDim, null, PolyTypes.Plane)//null defaults to Thin Plate Spline
	//	{ }
	//	private RBFNetwork2(uint nDim, IBasisFunction basis, PolyTypes poly)
	//	{
	//		m_rbfs = new RBFSurface[nDim];
	//		for (uint i = 0; i < nDim; i++)
	//		{
	//			m_rbfs[i] = new RBFSurface(null, basis, poly, 0);
	//		}
	//	}

	//	RBFSurface[] m_rbfs;// = new RBFSurface[3];
	//	int Dimensions
	//	{
	//		get { return m_rbfs.Length; }
	//	}
	//	public void Fit(IList<double[]> uvs, IList<double[]> xyz)
	//	{
	//		if (uvs.Count != xyz.Count || xyz.Count == 0 || xyz[0].Length != Dimensions)
	//			return;


	//		List<double[]>[] fits = new List<double[]>[Dimensions];
	//		int nDim = 0;
	//		for (nDim = 0; nDim < Dimensions; nDim++)
	//		{
	//			fits[nDim] = new List<double[]>(uvs.Count);
	//			for (int i = 0; i < uvs.Count; i++)
	//				fits[nDim].Add(new double[] { uvs[i][0], uvs[i][1], xyz[i][nDim] });
	//		}

	//		for (nDim = 0; nDim < Dimensions; nDim++)
	//			m_rbfs[nDim].Fit(fits[nDim]);
	//	}

	//	public void Value(double[] uv, ref double[] xyz)
	//	{
	//		double[] p = new double[3];
	//		p[0] = uv[0]; p[1] = uv[1];
	//		for (int i = 0; i < Dimensions; i++)
	//		{
	//			m_rbfs[i].Value(ref p);
	//			xyz[i] = p[2];

	//		}
	//	}
	//	public void First(double[] uv, ref double[] xyz, ref double[,] dxyz)
	//	{
	//		double[] p = new double[3];
	//		double[] d = new double[3];
	//		p[0] = uv[0]; p[1] = uv[1];
	//		for (int i = 0; i < Dimensions; i++)
	//		{
	//			m_rbfs[i].First(ref p, ref d);
	//			xyz[i] = p[2];
	//			dxyz[i, 0] = d[0];
	//			dxyz[i, 1] = d[1];
	//		}
	//	}

	//}
}
