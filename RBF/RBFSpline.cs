using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
namespace RBF
{
	public class RBFSpline
	{
		double[] m_sCenters;
		double[,] m_Weights;
		int Count
		{
			get { return m_sCenters.Length; }
		}

		#region Fitting

		public void Fit(List<double> sPos, List<double[]> fits)
		{
			if (fits == null || fits.Count == 0)
				return;

			int nDim = fits[0].Length, nx;
			double[][] b = new double[nDim][];
			for (nx = 0; nx < nDim; nx++)
				b[nx] = new double[fits.Count + 2];

			m_Weights = new double[fits.Count + 2, nDim];
			m_sCenters = new double[fits.Count];
			m_sCenters[0] = sPos == null ? 0 : sPos[0];

			int nPos;
			for (nPos = 0; nPos < fits.Count; nPos++)
			{
				if (fits[nPos] == null || fits[nPos].Length != nDim)
					continue;

				if (sPos != null)
					m_sCenters[nPos] = sPos[nPos];
				else
					if (nPos > 0)//accumulate distance for spos interpolation
						m_sCenters[nPos] = m_sCenters[nPos - 1] + BLAS.distance(fits[nPos], fits[nPos - 1]);

				for (nx = 0; nx < nDim; nx++)
					b[nx][nPos] = fits[nPos][nx];
			}

			for (nx = 0; nx < nDim; nx++)
				b[nx][nPos] = b[nx][nPos + 1] = 0;//poly conditions

			if(sPos != null )
				for (nPos = 0; nPos < fits.Count; nPos++)
					m_sCenters[nPos] /= m_sCenters.Last(); //normalize sPos

			double[,] A = FitMat();

			for (nx = 0; nx < nDim; nx++)
				Solve(A, b[nx], nx);
		}

		double[,] FitMat()
		{
			int dim = Count;
			int fits = dim;

			fits += 2;// for polynomial terms

			double[,] A = new double[fits, fits];
			// create A matrix
			double r;
			double a = 0;//for scaling relaxation parameter
			int i, j;
			for (i = 0; i < dim; ++i)
			{
				for (j = i + 1; j < dim; ++j)
				{
					// PHI11 - PHINN
					r = Math.Abs(m_sCenters[j] - m_sCenters[i]);
					A[i, j] = A[j, i] = Basis(r); // symmetric
					a += 2 * r;
				}
			}
			a /= dim * dim; // calculate relaxation normalizer

			// calculate fit mat diagonals and poly terms
			double relax = 0;
			for (i = 0; i < dim; ++i)
			{
				A[i, i] = a * a * relax;

				for (j = 0; j < 2; j++)// polyvalues: As + B
					A[i, dim + j] = A[dim + j, i] = j == 0 ? m_sCenters[i] : 1;
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

		#endregion

		double Basis(double r)
		{
			return r == 0 ? 0 : r * r * Math.Log(r);
		}
		public void BsVal(double s, ref double[] x)
		{
			double rad;
			int nx, nPos;

			for (nx = 0; nx < x.Length; nx++)
				x[nx] = 0;//initialize x
			for (nPos = 0; nPos < Count; nPos++)
			{
				rad = Math.Abs(s - m_sCenters[nPos]);
				rad = Basis(rad);
				for (nx = 0; nx < x.Length; nx++)
				{
					x[nx] += rad * m_Weights[nPos, nx];
				}
			}
			for ( nx = 0; nx < x.Length; nx++)//poly terms: linear As + B
				x[nx] += m_Weights[nPos, nx] * s + m_Weights[nPos + 1, nx];
		}


		public bool IsFit { get { return m_sCenters != null && m_Weights != null; } }
	}
}
