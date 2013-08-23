using System;
using System.Collections.Generic;
using System.Text;
using RBFPolynomials;
using RBFBasis;

namespace RBF
{
     public class RBFSurface 
     {
          public RBFSurface(IList<double[]> fitpoints, IBasisFunction basis,  PolyTypes poly, double relax)
		{
			switch (poly)
			{
				case PolyTypes.Plane:
					Poly = new RBFPolynomials.Plane(this);
					break;
				case PolyTypes.Paraboloid:
					Poly = new RBFPolynomials.Paraboloid(this);
					break;
				case PolyTypes.ParaboloidC:
					Poly = new RBFPolynomials.ParaboloidConst(this);
					break;
				case PolyTypes.Conic:
					Poly = new RBFPolynomials.Conic(this);
					break;
			}
			//defaults 
			Poly = Poly ?? new RBFPolynomials.Plane(this);
			Basis = basis ?? new ThinPlateSpline();
               Relaxation = relax;

               if (fitpoints != null)
               {
                    Fit(fitpoints, null);
               }

		}
          public RBFSurface(IList<double[]> fitpoints, IBasisFunction basis, IRBFPolynomial poly, double relax)
          {
			Basis = basis != null ? basis : new ThinPlateSpline();
			Poly = poly != null ? poly : new Plane(this);
               Relaxation = relax;

               if (fitpoints != null)
               {
                    Fit(fitpoints, null);
               }
          }
		public RBFSurface(IList<double[]> fitPoints)
			: this(fitPoints, null, null, 0)
		{	}
		public RBFSurface(Vect3[] fitPoints)
			:this(null,null,null,0)
		{
			Fit(Array.ConvertAll<Vect3, double[]>(fitPoints, element => element.m_vec));
		}
		Center3d[] m_centers;
		IBasisFunction m_basis;
		IRBFPolynomial m_poly;
		double m_relax = 0;


		public ICenter[] Centers
		{
			get { return m_centers; }
		}
		public IBasisFunction Basis
		{
			get { return m_basis; }
			set { m_basis = value; }
		}
		public IRBFPolynomial Poly
		{
			get { return m_poly; }
			set { m_poly = value; }
		}
		double Relaxation
		{
			get { return m_relax; }
			set { m_relax = Math.Max(value, 0); }//enforce nonnegative relaxation
		}
		double m_energy = 0;
		double m_error = 0;

		public double BendingEnergy
		{
			get { return m_energy; }
			set { m_energy = value; }
		}
		public double Error
		{
			get { return m_error; }
			set { m_error = value; }
		}

		double[] m_max = new double[3];
		double[] m_min = new double[3];


		public double[] Max
		{
			get { return m_max; }
		}
		public double[] Min
		{
			get { return m_min; }
		}
		public double[] Mid
		{
			get { return new double[] { (m_max[0] + m_min[0]) / 2, (m_max[1] + m_min[1]) / 2, (m_max[2] + m_min[2]) / 2 }; }
		}

		public int Fit(IList<double[]> fitPoints)
		{
			return Fit(fitPoints, null);
		}
		/// <summary>
		/// Fit an rbf surface to the specified points, optionally specify a relaxation value for approximate interpolation
		/// </summary>
		/// <param name="fitPoints">the points to fit to (usually lifter locations)</param>
		/// <param name="pRelax">an optional relaxation parameter, 0 for exact fit, >0 for increasing tolerance. null will use the stored relax or default to 0</param>
		/// <returns>1 if successful, 0>= if error</returns>
		public int Fit(IList<double[]> fitPoints, double? pRelax)
		{
			BendingEnergy = 0; //reset bending energy and error
			Error = 0;
			m_max[0] = m_max[1] = m_max[2] = -1e9; //start max low
			m_min[0] = m_min[1] = m_min[2] = +1e9; //start min high

			m_centers = new Center3d[fitPoints.Count]; //allocate space for center

			if (pRelax != null)
				Relaxation = (double)pRelax; //0 for exact, increase to reduce bending energy

			int i, j; //loops

			double[] fitz = new double[fitPoints.Count + Poly.Terms]; //temp vector for rhs

			int iFits = 0;
			foreach(double[] v in fitPoints)
			{
				Centers[iFits] = new Center3d(v[0], v[1]);
				fitz[iFits] = v[2];

				for (i = 0; i < v.Length; i++) //get fit points' bounding box
				{
					m_max[i] = Math.Max(v[i], m_max[i]);
					m_min[i] = Math.Min(v[i], m_min[i]);
				}
				iFits++;
			}
			// poly conditions
			//for (i = 0; i < Poly.Terms; i++)
			//	fitz.Add(0);

			//create the fitting matrix
			double[,] A;

			RBFSolver.fit_mat(out A, Centers, Basis, Poly, Relaxation);
			// solve the system
			int err = RBFSolver.solve(A, fitz, Centers, Poly);
			if (err != 0)
				return err;

			//calculate bending energy (wT * A * w)
			double be = 0;
			List<double> wtA = new List<double>(new double[Centers.Length]); //temp vector for w-transpose * A
			for (i = 0; i < Centers.Length; i++)
				for (j = 0; j < Centers.Length; j++)
				{
					wtA[i] += Centers[j].w * A[j, i]; //first multiplication
				}
			for (i = 0; i < Centers.Length; i++)
				be += wtA[i] * Centers[i].w; //second multiplication

			BendingEnergy = be;

			//calculate error as the sum of % difference between target z and actual z
			Error = CheckFit(fitPoints);
			return 1;
		}

		/// <summary>
		/// returns the error between the RBF Surface Z locations and the lifters actual Z location
		/// </summary>
		/// <param name="pnts">Lifter locations</param>
		/// <returns>error value</returns>
		public double CheckFit(IList<double[]> pnts)
		{
			double error = 0;
			double[] p = new double[3];
			foreach(double[] v in pnts)
			{
				p = new double[3] { v[0], v[1], v[2] };
				Value(ref p);
				error += Math.Abs(v[2] - p[2]) / v[2];
			}

			return error;
		}


          public void Value(ref double[] p)
          {
               p[2] = 0;

               double r;
               foreach (Center3d c in Centers)
               {
                    r = c.radius(p);
                    p[2] += c.w * Basis.val(r); // sum the weight * rbf values
               }

               Poly.Poly(p, null, null); // add the polynomial
          }
          public void First(ref double[] p, ref double[] d)
          {
			//for (int i = 0; i < 3; i++)
			//	p[i] /= SCALE;
               p[2] = 0;
               d[0] = d[1] = d[2] = 0;
               double r, dr, drdx;
			foreach (Center3d c in Centers)
			{
                    r = c.radius(p); // radius
                    if (BLAS.IsEqual(r, 0)) continue;

                    p[2] += c.w * Basis.val(r); // sum the weight * rbf values

                    dr = Basis.dr(r); // dRBF/dr

                    drdx = (p[0] - c[0]) / r; // dr/dx
                    d[0] += c.w * dr * drdx; // accumulate weighted derivatives

                    drdx = (p[1] - c[1]) / r; // dr/dy
                    d[1] += c.w * dr * drdx; // accumulate weighted derivatives

                    //dx[oz] += c.w()*dr; // accumulate weighted derivatives
                    //rbf()->first(c, p, dx); 
               }
			Poly.Poly(p, d, null); // add the polynomial
			//for (int i = 0; i < 3; i++)
			//{
			//	p[i] *= SCALE;
			//	d[i] *= SCALE;
			//}

          }
          public void Second(ref double[] p, ref double[] d, ref double[] dd)
          {
			//for (int i = 0; i < 3; i++)
			//	p[i] /= SCALE;
               p[2] = 0; //initialize z
               d[0] = d[1] = d[2] = 0;
               dd[0] = dd[1] = dd[2] = 0;
               double r, dr, drdx, ddrdxx;
			foreach (Center3d c in Centers)
			{
                    r = c.radius(p); // radius
                    if (BLAS.IsEqual(r, 0)) continue;
                    p[2] += c.w * Basis.val(r); // sum the weight * rbf values

                    dr = Basis.dr(r); // dRBF/dr

                    drdx = (p[0] - c[0]) / r; // dr/dx
                    d[0] += c.w * dr * drdx; // accumulate weighted first derivatives

                    //ddrdxx = 2*(xc*xc)/(r*r)+dr/r; // d^2r/dx^2
                    ddrdxx = 2 * drdx * drdx + dr / r;// d^2r/dx^2
                    dd[0] += c.w * ddrdxx; // accumulate weighted second derivatives

                    drdx = (p[1] - c[1]) / r; // dr/dy
                    d[1] += c.w * dr * drdx;// accumulate weighted first derivatives

                    //ddrdxx = 2*(xc*xc)/(r*r)+dr/r; // d^2r/dy^2
                    ddrdxx = 2 * drdx * drdx + dr / r;// d^2r/dx^2
                    dd[1] += c.w * ddrdxx; // accumulate weighted second derivatives

                    //dx[oz] = 0; // dphi/dz = 0, tps is a function of x and y only
                    dd[2] += c.w * 2 * (p[0] - c[0]) * (p[1] - c[1]) / (r * r); // d^2r/dxdy
               }
			Poly.Poly(p, d, dd); // add the polynomial
			//for (int i = 0; i < 3; i++)
			//{
			//	p[i] *= SCALE;
			//	d[i] *= SCALE;
			//}
          }

          /// <summary>
          /// return Unit normal vector
          /// </summary>
          /// <param name="p"></param>
          /// <param name="d"></param>
          /// <param name="nor"></param>
          public void Normal(ref double[] p, ref double[] d, ref double[] nor)
          {
               First(ref p, ref d); //get the first derivatives and z value
               double[] dx = d;
               double[] dy;
               BLAS.split(ref dx, out dy);
               nor = BLAS.cross(dx, dy); //cross surface tangents to get normal
               // make unit-normal(magnitude is meaningless anyway)
			BLAS.unitize(ref nor);
          }

          public void Gaussian(ref double[] p, ref double[] d, ref double[] dd, ref double k)
          {
               Second(ref p, ref d, ref dd);
               //calculate unit normal
               double[] dx = d;
               double[] dy;
               BLAS.split(ref dx, out dy);
               double[] nor = BLAS.cross(dx, dy); //cross surface tangents to get normal
               // make unit-normal(magnitude is meaningless anyway)
			BLAS.unitize(ref nor);

               double[] dxx = new double[] { 1, 0, dd[0] };
               double[] dyy = new double[] { 0, 1, dd[1] };
               double[] dxy = new double[] { 0, 0, dd[2] };

               //calculate first fundamental form
               double E = BLAS.dot(dx, dx);
               double F = BLAS.dot(dx, dy);
               double G = BLAS.dot(dy, dy);
               double detI = E * G - F * F;
               //calculate second fundamental form
               double e = BLAS.dot(dxx, nor);
               double f = BLAS.dot(dxy, nor);
               double g = BLAS.dot(dyy, nor);
               double detII = e * g - f * f;
               //calculate Shape Operator
               //double s11 = (e * G - f * F) / detI;
               //double s12 = (f * G - g * F) / detI;
               //double s21 = (f * E - e * F) / detI;
               //double s22 = (g * E - f * F) / detI;
               //curvature is det(Shape Op)
               //k = s11 * s22 - s21 * s12;
               k = detII / detI;
          }


		#region GetMeshPoints

		/// <summary>
		/// this return a grid of points that travel up Y axis
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		public List<double[]> GetMeshPoints(int rows, int columns)
		{
			double[] max = new double[3];
			double[] min = new double[3];
			for (int i = 0; i < 3; i++)
			{
				max[i] = m_max[i];
				min[i] = m_min[i];
			}
			return GetMeshPoints(rows, columns, max, min);
		}

		public List<double[]> GetMeshPoints(int rows, int columns, double[] max, double[] min)
		{
			List<double[]> vals = new List<double[]>();
			double deltax = max[0] - min[0];
			double deltay = max[1] - min[1];

				int i, j;
				double x, y;

				for (i = 0; i < rows; i++)
				{
					x = (double)i / (double)(rows - 1) * deltax + min[0];
					for (j = 0; j < columns; j++)
					{
						y = (double)j / (double)(columns - 1) * deltay + min[1];
						double[] p = new double[3] { x, y, 0 };
						Value(ref p);
						vals.Add(p);
					}
				}
			return vals;
		}

		public List<double[]> GetRowPoints(double locationY, int numPnts)
		{
			double[] max = new double[3];
			double[] min = new double[3];
			for (int i = 0; i < 3; i++)
			{
				max[i] = m_max[i];
				min[i] = m_min[i];
			}

			double deltax = max[0] - min[0];
			double deltay = max[1] - min[1];

			List<double[]> ret = new List<double[]>(numPnts);
			double[] p = new double[3] { 0, locationY * 1000.0, 0 };

			for (int i = 0; i < numPnts; i++)
			{
				p[0] = (double)i / (double)(numPnts - 1) * deltax + min[0];
				Value(ref p);
				ret.Add(new double[] { p[0], p[1], p[2] });
			}

			return ret;
		}

		public List<KeyValuePair<double[], double>> GetMeshPointsCvt(int rows, int columns, double[] max, double[] min)
		{
			List<KeyValuePair<double[], double>> vals = new List<KeyValuePair<double[], double>>();
			double deltax = max[0] - min[0];
			double deltay = max[1] - min[1];

			int i, j;
			double x, y, k = 0;
			double[] d = new double[3];
			double[] dd = new double[3];
			for (i = 0; i < rows; i++)
			{
				x = (double)i / (double)(rows - 1) * deltax + min[0];
				for (j = 0; j < columns; j++)
				{
					y = (double)j / (double)(columns - 1) * deltay + min[1];
					double[] p = new double[3] { x, y, 0 };
					Gaussian(ref p, ref d, ref dd, ref k);
					vals.Add(new KeyValuePair<double[], double>(p, k));
				}
			}

			return vals;
		}

		#endregion

          #region DumpMatrix

          private void DumpA(double[,] A, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    for (int j = 0; j < A.GetLength(0); j++)
                    {
                         for (int k = 0; k < A.GetLength(1); k++)
                         {
                              sw.Write(A[j, k].ToString());
                              sw.Write(",");
                         }
                         sw.WriteLine();
                    }


               }
          }
          private void DumpD(double[] D, int row, int col, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    for (int j = 0; j < row; j++)
                    {
                         for (int k = 0; k < col; k++)
                         {
                              sw.Write(D[j * row + k]);
                              sw.Write(",");
                         }
                         sw.WriteLine();
                    }
               }
          }
          private void DumpW(double[] w, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    foreach (double d in w)
                         sw.WriteLine(d.ToString());
               }
          }

          #endregion
     }
}