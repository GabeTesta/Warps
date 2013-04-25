using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;


namespace Warps
{
	class BSpline
	{
		public BSpline(int degree)
		{
			m_degree = degree;
		}

		int m_degree;
		public int Deg
		{ get { return m_degree; } }
		double[] m_xKnot;
		double[,] m_xCof;

		public void Fit(double[] sPos, double[][] xPnts)
		{
			if (sPos.Length < 5)
			{
				TriFit(sPos, xPnts);
				return;
			}

			int nKnot = sPos.Length -2;
			m_xKnot = new double[sPos.Length+2];
			m_xCof = new double[sPos.Length, Deg];

			int nKnt = 0;
			m_xKnot[nKnt++] = sPos[0];
			m_xKnot[nKnt++] = sPos[0];
			m_xKnot[nKnt++] = sPos[0];
			//	trible up Knot end points but leave extra internal end points
			for (int nOff = 2; nOff <nKnot; nOff++) m_xKnot[nKnt++] = sPos[nOff];
			m_xKnot[nKnt++] = sPos[sPos.Length - 1];
			m_xKnot[nKnt++] = sPos[sPos.Length - 1];
			m_xKnot[nKnt++] = sPos[sPos.Length - 1];

			DenseMatrix A = new DenseMatrix(sPos.Length);
			int nS;
			double[,] basis = null;
			int iRow = 0;//, iCol = 0;

			for (iRow = 0; iRow < sPos.Length; iRow++)
			{
				BsCil(0, sPos[iRow], m_xKnot, out nS, ref basis);
				for (int j = 0; j < 4; j++)
					A[iRow, nS + j] = basis[0, j];
			}

			LU decomp = A.LU();
			DenseVector b;
			for( int j =0; j < Deg; j++ )
			{
				b = new DenseVector(xPnts[j]);
				Vector x = (Vector)decomp.Solve(b);
				for (int i = 0; i < sPos.Length; i++)
					m_xCof[i, j] = x[i];
			}
		}

		private void TriFit(double[] sPos, double[][] xPnts)
		{
			//int nKnot = sPos.Length - 1;
			//m_xKnot = new double[sPos.Length + 4];
			//m_xCof = new double[sPos.Length, Deg];

			//int nKnt = 0;
			//m_xKnot[nKnt++] = sPos[0];
			//m_xKnot[nKnt++] = sPos[0];
			//m_xKnot[nKnt++] = sPos[0];
			////	trible up Knot end points but leave extra internal end points
			//for (int nOff = 1; nOff < nKnot; nOff++) m_xKnot[nKnt++] = sPos[nOff];
			//m_xKnot[nKnt++] = sPos[sPos.Length - 1];
			//m_xKnot[nKnt++] = sPos[sPos.Length - 1];
			//m_xKnot[nKnt++] = sPos[sPos.Length - 1];

			//DenseMatrix A = new DenseMatrix(sPos.Length);
			//int nS;
			//double[,] basis = null;
			//int iRow = 0;//, iCol = 0;

			//for (iRow = 0; iRow < sPos.Length; iRow++)
			//{
			//	BsCil(0, sPos[iRow], m_xKnot, out nS, ref basis);
			//	for (int j = 0; j < 4; j++)
			//		A[iRow, nS + j] = basis[0, j];
			//}

			//LU decomp = A.LU();
			//DenseVector b;
			//for (int j = 0; j < Deg; j++)
			//{
			//	b = new DenseVector(xPnts[j]);
			//	Vector x = (Vector)decomp.Solve(b);
			//	for (int i = 0; i < sPos.Length; i++)
			//		m_xCof[i, j] = x[i];
			//}
		}

		static void BsCil(int iDer, double t, double[] xKnot, out int nInterval, ref double[,] basis)
		{
			if (basis == null)
				basis = new double[iDer + 1, 4];
			int k;
			//find knot interval
			for (k = 3; k <= xKnot.Length - 3; k++)
				if (t <= xKnot[k]) break;
			//catch overrun
			if (k > xKnot.Length - 3)
				k = xKnot.Length - 3;

			nInterval = k - 3;

			double sm3, sm2, sm1, s00, sp1, sp2, sm10, sm22, sm21, sm32, sm31, sm30, a1, a2, a3;

			sm3 = xKnot[k - 3]; sm2 = xKnot[k - 2]; sm1 = xKnot[k - 1];
			s00 = xKnot[k]; sp1 = xKnot[k + 1]; sp2 = xKnot[k + 2];

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
		public void BsVal(double s, ref double[] pnt)
		{
			int nS;
			double[,] basis = null;
			BsCil(0, s, m_xKnot, out nS, ref basis);

			int ix;
			for (ix = 0; ix < Deg; ix++)
				//	calculate m_nx degree (x)-components
				pnt[ix] = m_xCof[nS, ix] * basis[0, 0]
					+ m_xCof[nS + 1, ix] * basis[0, 1]
					+ m_xCof[nS + 2, ix] * basis[0, 2]
					+ m_xCof[nS + 3, ix] * basis[0, 3];
		}
		public void BsVec(double s, ref double[] pnt, ref double[] dpnt)
		{
			int nS;
			double[,] basis = null;
			BsCil(1, s, m_xKnot, out nS, ref basis);

			int ix;
			for (ix = 0; ix < Deg; ix++)
			{
				//	calculate m_nx degree (x)-components
				pnt[ix] = m_xCof[nS, ix] * basis[0, 0]
					+ m_xCof[nS + 1, ix] * basis[0, 1]
					+ m_xCof[nS + 2, ix] * basis[0, 2]
					+ m_xCof[nS + 3, ix] * basis[0, 3];

				//	calculate 1st order (x)-derivatives
				dpnt[ix] = m_xCof[nS, ix] * basis[1, 0]
					+ m_xCof[nS + 1, ix] * basis[1, 1]
					+ m_xCof[nS + 2, ix] * basis[1, 2]
					+ m_xCof[nS + 3, ix] * basis[1, 3];
			}
		}
		public void BsCvt(double s, ref double[] pnt, ref double[] dpnt, ref double[] ddpnt)
		{
			int nS;
			double[,] basis = null;
			BsCil(2, s, m_xKnot, out nS, ref basis);

			int ix;
			for (ix = 0; ix < Deg; ix++)
			{
				//	calculate m_nx degree (x)-components
				pnt[ix] = m_xCof[nS, ix] * basis[0, 0]
					+ m_xCof[nS + 1, ix] * basis[0, 1]
					+ m_xCof[nS + 2, ix] * basis[0, 2]
					+ m_xCof[nS + 3, ix] * basis[0, 3];

				//	calculate 1st order (x)-derivatives
				dpnt[ix] = m_xCof[nS, ix] * basis[1, 0]
					+ m_xCof[nS + 1, ix] * basis[1, 1]
					+ m_xCof[nS + 2, ix] * basis[1, 2]
					+ m_xCof[nS + 3, ix] * basis[1, 3];

				//	calculate 2nd order (x)-derivatives
				ddpnt[ix] = m_xCof[nS, ix] * basis[2, 0]
					+ m_xCof[nS + 1, ix] * basis[2, 1]
					+ m_xCof[nS + 2, ix] * basis[2, 2]
					+ m_xCof[nS + 3, ix] * basis[2, 3];			}
		}
	}
}
