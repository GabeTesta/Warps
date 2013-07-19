using System;
using System.Collections.Generic;
using RBFPolynomials;
using RBFBasis;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace RBF
{
	public enum PolyTypes
	{
		Plane,
		Paraboloid,
		ParaboloidC,
		Conic
	}
	public static class RBFSolver
	{
		public static void fit_mat(out double[,] A, ICenter[] Centers, IBasisFunction rbf, IRBFPolynomial Poly, double Relaxation)
		{
			// find out
			int dim = Centers.Length;
			int fits = dim;

			fits += Poly.Terms;// for polynomial terms

			A = new double[fits, fits];
			// create A matrix
			double r;
			double a = 0;//for scaling relaxation parameter
			int i, j;
			for (i = 0; i < dim; ++i)
			{
				for (j = i + 1; j < dim; ++j)
				{
					// PHI11 - PHINN
					r = Centers[j].radius(Centers[i].ToArray());
					A[i, j] = A[j, i] = rbf.val(r); // symmetric
					a += 2 * r;
				}
			}
			a /= dim * dim; // calculate relaxation normalizer
			//a /= fits * fits; // fits = dims + Poly.Terms(3)
			//a /= Math.Pow(fits, 2);
			// calculate fit mat diagonals and poly terms
			double relax = Relaxation;
			for (i = 0; i < dim; ++i)
			{
				//A[i, i] = BLAS.is_equal(relax, 0) ? 1 : a * a * relax;
				A[i, i] = a * a * relax;

				for (j = 0; j < Poly.Terms; j++)
					A[i, dim + j] = A[dim + j, i] = Poly.FitMat(i, j);

                #region old
                //A[i, dim + 0] = A[dim + 0, i] = Centers[i][0];
				//A[i, dim + 1] = A[dim + 1, i] = Centers[i][1];

				//// poly values: Ax + By + C POLY
				//A[i, dim + 2] = A[dim + 2, i] = 1;

				// poly values: Ax + By + Cxy POLY
				//A[i, dim + 2] = A[dim + 2, i] = Centers[i][1] * Centers[i][0];

				// parabaloid values: A(x-h)^2 + B(y-k)^2 POLY
				//A[i, dim + 0] = A[dim + 0, i] = Math.Pow(Centers[i][0] - middle[0],2);
                //A[i, dim + 1] = A[dim + 1, i] = Math.Pow(Centers[i][1] - middle[1], 2);
                #endregion
            }
		}

		//public static int solve(double[,] A, double[] fitz, CenterArrayNode CenterNode, IRBFPolynomial Poly)
		//{
		//     System.Diagnostics.Debug.Assert(fitz.Length == A.GetLength(0));
		//     System.Diagnostics.Debug.Assert(A.GetLength(0) == A.GetLength(1));

		//     int[] pivot = new int[A.GetLength(0)];
		//     int i;//,j,k;

		//     //double[] d = new double[A.Length];
		//     //for (j = 0; j < A.GetLength(0); j++)
		//     //     for (k = 0; k < A.GetLength(1); k++)
		//     //          d[k + j * A.GetLength(0)] = A[j, k];

		//     //DumpD(d, A.GetLength(1), A.GetLength(0), "c:\\before.csv");
		//     double[,] fit = new double[fitz.Length, 1];
		//     for (i = 0; i < fitz.GetLength(0); i++)
		//          fit[i, 0] = fitz[i];
		//     double[,] w2 = BLAS.SimultaneousSolver(A, fit);
		//     //var matrixA = new DenseMatrix(A);
		//     //var vectorB = new DenseVector(fitz);
		//     //Vector<double> resultX = matrixA.LU().Solve(vectorB);
		//     //MCroutPPS.Decomp(Tolerance, ref d, ref pivot, ref error, A.GetLength(1), A.GetLength(0));


		//     //DumpD(d, A.GetLength(1), A.GetLength(0), "c:\\after.csv");

		//     //if (error <= 0)
		//     //{
		//     //	return error;
		//     //}

		//     //double[] w = new double[A.GetLength(0)];// create weights vector
		//     double[] w = new double[A.GetLength(0)];
		//     for (i = 0; i < w2.GetLength(0); i++)
		//          w[i] = w2[i, 0];
		//     //MCroutPPS.Desolv(ref d, ref w, ref fitz, ref pivot, ref error, A.GetLength(0));

		//     //DumpW(w, "c:\\w.csv");
		//     i = 0;
		//     foreach (double weight in w)
		//     {
		//          if (i < CenterNode.Centers.Count)
		//               CenterNode[i].w = weight; // set the center's weight
		//          else
		//               Poly[i - CenterNode.Centers.Count] = weight;
		//          //polycofs[i - Centers.Count] = weight; //store the polynomial coefficients

		//          ++i;
		//     }

		//     return 0;
		//}

          public static int solve(double[,] A, double[] fitz, ICenter[] Centers, IRBFPolynomial Poly)
          {
			var matrixA = new DenseMatrix(A);
			var vectorB = new DenseVector(fitz);
			LU<double> decomp = matrixA.LU();
			Vector<double> resultX = decomp.Solve(vectorB);
#if DEBUG 
			List<double> w2 = new List<double>(resultX.ToArray());
#endif

			int i = 0;
			foreach (double weight in resultX )
			{
				if (i < Centers.Length)
					Centers[i].w = weight; // set the center's weight
				else
					Poly[i - Centers.Length] = weight;//store the polynomial coefficients

				++i;
			}

               return 0;
          }
		public static IRBFPolynomial CreatePoly(RBF.PolyTypes type, RBFSurface surf)
		{
			switch (type)
			{
				case PolyTypes.Plane:
					return new RBFPolynomials.Plane(surf);
				case PolyTypes.Paraboloid:
					return new RBFPolynomials.Paraboloid(surf);
				case PolyTypes.ParaboloidC:
					return new RBFPolynomials.ParaboloidConst(surf);
				case PolyTypes.Conic:
					return new RBFPolynomials.Conic(surf);
				default :
					return new RBFPolynomials.Plane(surf);
			}
		}
	}
}
