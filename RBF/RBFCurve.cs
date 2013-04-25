using System;
using System.Collections.Generic;
using System.Text;
using RBFPolynomials;
using RBFBasis;

namespace RBF
{
	public class RBFCurve 
	{

		public RBFCurve(string label, List<double[]> fitPoints, IBasisFunction basis, IRBFPolynomial poly, double relaxation)
		{
			Basis = basis;
			Poly = poly;
			Relaxation = relaxation;

			if (fitPoints == null)
				return;

			Fit(fitPoints, relaxation);
		}

		Center2d[] m_centers;
		IBasisFunction m_basis;
		IRBFPolynomial m_poly;
		double m_relax = 0;


		public ICenter[] Centers
		{
			get { return m_centers; }
		}
		IBasisFunction Basis
		{
			get { return m_basis; }
			set { m_basis = value; }
		}
		IRBFPolynomial Poly
		{
			get { return m_poly; }
			set { m_poly = value; }
		}

		public double Relaxation
		{
			get
			{
				return m_relax;
			}
			private set
			{
				m_relax = Math.Max( value, 0 );//enforce nonnegative relaxation
			}
		}

		double m_energy = 0;
		double m_error = 0;

		double[] m_max = new double[2];
		double[] m_min = new double[2];

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

		public double[] Max
		{
			get { return m_max; }
		}
		public double[] Min
		{
			get { return m_min; }
		}
		internal double[] Middle
		{
			get { return new double[] { (m_max[0] + m_min[0]) / 2, (m_max[1] + m_min[1]) / 2, (m_max[2] + m_min[2]) / 2 }; }
		}

		int Fit(IList<double[]> fitPoints, double Relax)
		{
			Relaxation = Relax;
			BendingEnergy = 0; //reset bending energy and error
			Error = 0;
			m_max[0] = m_max[1] = -1e9; //start max low
			m_min[0] = m_min[1] = +1e9; //start min high

			m_centers = new Center2d[fitPoints.Count]; //allocate space for centers

			double[] fitz = new double[fitPoints.Count + Poly.Terms]; //temp vector for rhs

			int i, j;
			int iFit = 0;
			foreach (double[] v in fitPoints)
			{
				fitz[iFit] = v[1];
				Centers[iFit]= new Center2d(v[0]);
				iFit++;
				for (i = 0; i < v.Length; i++) //get fit points' bounding box
				{
					m_max[i] = Math.Max(v[i], m_max[i]);
					m_min[i] = Math.Min(v[i], m_min[i]);
				}
			}
			//polynomial conditons automatically zeroed by array ctor
			//for (i = 0; i < Poly.Terms; i++)
			//	fitz.Add(0);

			//create the fitting matrix
			double[,] A;
			RBFSolver.fit_mat(out A, Centers, Basis, Poly, Relaxation);

			int err = RBFSolver.solve(A, fitz, Centers, Poly);
			if (err != 0)
				return err;

			//calculate bending energy (wT * A * w)
			double be = 0;
			List<double> wtA = new List<double>(Centers.Length); //temp vector for w-transpose * A
			for (i = 0; i < Centers.Length; i++)
				wtA.Add(0);
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
			double[] p = new double[2];
			foreach (double[] v in pnts)
			{
				p = new double[] { v[0], v[1] };
				Value(ref p);
				error += Math.Abs(v[1] - p[1]) / v[1];
			}
			return error;
		}

		private void PolyTerms(double[] p, double[] d, double[] dd)
		{
			Poly.Poly(p, d, dd);
			return;
		}

          /// <summary>
          /// get a value on the curve using 0-1
          /// </summary>
          /// <param name="s">value between 0 and 1</param>
          /// <returns>the xy point on the curve</returns>
          public double[] Value(double s)
          {
               if (s < 0)
                    s = 0;
               if (s > 1)
                    s = 1;

               double[] p = new double[2];
               //interpolate x value
			p[0] = Min[0] + s * ( Max[0] - Min[0]);

			//solve rbf at x
			Value(ref p);
			return p;
          }
		public void Value(ref double[] p)
		{
			p[1] = 0;
			double r;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p);
				p[1] += c.w * Basis.val(r); // sum the weight * rbf values
			}
			PolyTerms(p, null, null); // add the polynomial
		}
		public void First(ref double[] p, ref double[] d)
		{
			p[1] = 0;
			d[0] = 1;
			d[1] = 0;
			double r, dr, drdx;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p); // radius
				if (BLAS.is_equal(r, 0)) continue;

				p[2] += c.w * Basis.val(r); // sum the weight * rbf values

				dr = Basis.dr(r); // dRBF/dr

				drdx = (p[0] - c[0]) / r; // dr/dx
				d[1] += c.w * dr * drdx; // accumulate weighted derivatives
			}
			PolyTerms(p, d, null); // add the polynomial
		}
		public void Second(ref double[] p, ref double[] d, ref double[] dd)
		{
			p[1] = 0; //initialize z
			d[0] = 1;
			d[1] = 0;
			dd[0] = 1;
			dd[1] = 0;
			double r, dr, drdx, ddrdxx;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p); // radius
				if (BLAS.is_equal(r, 0)) continue;


				p[1] += c.w * Basis.val(r); // sum the weight * rbf values

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
				//dd[2] += c.w * 2 * (p[0] - c[0]) * (p[1] - c[1]) / (r * r); // d^2r/dxdy
			}
			PolyTerms(p, d, dd); // add the polynomial
		}

		/// <summary>
		/// return Unit normal vector
		/// </summary>
		/// <param name="p">point to get normal at, y value will be filled on return</param>
		/// <param name="d">tangent at point</param>
		/// <param name="nor">normal at point</param>
		public void Normal(ref double[] p, ref double[] d, ref double[] nor)
		{
			First(ref p, ref d); //get the first derivatives and z value
			nor = new double[] { d[1], -d[0] };
		}

		/// <summary>
		/// finds closest point to given p paramter
		/// </summary>
		/// <param name="p">in: target point, out: closest on curve</param>
		/// <param name="dist">out: distance to curve</param>
		/// <param name="tol">tolerance</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool Closest(ref double[] p, ref double dist, double tol)
		{
			if (Centers.Length == 0)
				return false;

			double[] x = new double[] { p[0], p[1] };
			double[] dx = new double[2];
			double[] ddx = new double[2];

			double[] h = new double[2];
			double[] e = new double[2];
			double dedx;

			int loop = 0, max_loops = 100;
			while (loop++ < max_loops)
			{
				Second(ref x, ref dx, ref ddx);

				h = BLAS.subtract(x, p);
				dist = BLAS.magnitude(h);//.magnitude();

				e[0] = x[0];
				e[1] = BLAS.dot(h, dx); // error, dot product is 0 at pi/2

				if (Math.Abs(e[1]) < tol) // error is less than the tolerance
				{
					x.CopyTo(p,0);// return point to caller
					return true;
				}

				dedx = BLAS.dot(dx, dx) + BLAS.dot(h, ddx);

				// calculate a new x
				x[0] = e[0] - e[1] / dedx;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}
			return false;
		}

		public List<double[]> GetMeshPoints(int p)
		{	
			List<double[]> vals = new List<double[]>();

			double deltax = Max[0] - Min[0];
			double[] x = new double[2];

			for (int i = 0; i < p; i++)
			{
				x = new double[]{ (double)i / (double)(p - 1) * deltax + Min[0], 0 };
				Value(ref x);
				vals.Add(x);
			}
			return vals;
		}
	}
}
