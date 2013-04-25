using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBF;

namespace RBFPolynomials
{
	/////////////////////
	// 3D
	////////////////////
	public class ParaboloidConst: IRBFPolynomial
	{
		public ParaboloidConst(RBFSurface surf)
		{
			m_surf = surf;
		}

		RBFSurface m_surf;
		double[] polycofs = new double[3];

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 3; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Mid[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Mid[1], 2) + polycofs[2]; // A(x-h)^2 - B(y-k)^2 + C
			if (d != null)
			{
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Mid[0]);
				d[1] += 2 * polycofs[1] * (p[1] - m_surf.Mid[1]);
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : Math.Pow(m_surf.Centers[i][j] - m_surf.Mid[j], 2);
		}


		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2 + {2:g5}", polycofs[0], polycofs[1], polycofs[2]);
		}
	}
	public class Paraboloid : IRBFPolynomial
	{
		public Paraboloid(RBFSurface surf)
		{
			m_surf = surf;
		}
		RBFSurface m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 2; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Mid[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Mid[1], 2); // A(x-h)^2 - B(y-k)^2
			if (d != null)
			{
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Mid[0]);
				d[1] += 2 * polycofs[1] * (p[1] - m_surf.Mid[1]);
			}
		}

		public double FitMat(int i, int j)
		{
			return Math.Pow(m_surf.Centers[i][j] - m_surf.Mid[j], 2);
		}

		double[] polycofs = new double[2];

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2", polycofs[0], polycofs[1]);
		}

	}
	public class Conic : IRBFPolynomial
	{
		public Conic(RBFSurface surf)
		{
			m_surf = surf;
		}
		RBFSurface m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 6; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			//p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Middle[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Middle[1], 2); // A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			//p[2] += polycofs[0] * Math.Pow(p[0], 2) + polycofs[1] * p[0] * p[1] + polycofs[2] * Math.Pow(p[1], 2) + polycofs[3] * p[0] + polycofs[4] * p[1] + polycofs[5]; // Ax^2+Bxy+Cy^2+2Dx+Ey+F = 0
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Mid[0], 2) + polycofs[1] * (p[0] - m_surf.Mid[0]) * (p[1] - m_surf.Mid[1]) + polycofs[2] * Math.Pow(p[1] - m_surf.Mid[1], 2) + polycofs[3] * (p[0] - m_surf.Mid[0]) + polycofs[4] * (p[1] - m_surf.Mid[1]) + polycofs[5];
			// A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			// Ax^2+Bxy+Cy^2+2Dx+Ey+F = 0
			if (d != null)
			{
				//d[0] += 2* polycofs[0]* p[0] + p[1]*polycofs[1] + polycofs[3];
				//d[1] += polycofs[1]*p[0] + 2 * polycofs[2] * p[1] + polycofs[4];
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Mid[0]) + (p[1] - m_surf.Mid[1]) * polycofs[1] + polycofs[3];
				d[1] += polycofs[1] * (p[0] - m_surf.Mid[0]) + 2 * polycofs[2] * (p[1] - m_surf.Mid[1]) + polycofs[4];
			}
		}

		public double FitMat(int i, int j)
		{
			// A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			switch (j)
			{
				case 0:
					return Math.Pow(m_surf.Centers[i][0] - m_surf.Mid[0], 2);
				case 1:
					return (m_surf.Centers[i][0] - m_surf.Mid[0]) * (m_surf.Centers[i][1] - m_surf.Mid[1]);
				case 2:
					return Math.Pow(m_surf.Centers[i][1] - m_surf.Mid[1], 2);
				case 3:
					return m_surf.Centers[i][0] - m_surf.Mid[0];
				case 4:
					return m_surf.Centers[i][1] - m_surf.Mid[1];
				default:
					return 1;
			}
		}

		double[] polycofs = new double[6];

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2", polycofs[0], polycofs[1]);
		}

	}
	public class Plane : IRBFPolynomial
	{

		public Plane(RBFSurface surf)
		{
			m_surf = surf;
		}

		double[] polycofs = new double[3];
		RBFSurface m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 3; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			p[2] += polycofs[0] * p[0] + polycofs[1] * p[1] + polycofs[2]; // Ax + By + C
			if (d != null)
			{
				d[0] += polycofs[0];
				d[1] += polycofs[1];
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : m_surf.Centers[i][j];
		}

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}x + {1:g5}y + {2:g5}", polycofs[0], polycofs[1], polycofs[2]);
		}
	}

	/////////////////////
	// 2D
	////////////////////
	public class Linear : IRBFPolynomial
	{
		public Linear(RBFCurve curve)
		{
			m_curve = curve;
		}

		double[] polycofs = new double[2];//mx + b
		RBFCurve m_curve;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 2; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			p[1] += polycofs[0] * p[0] + polycofs[1]; // Ax + B
			if (d != null)
			{
				d[1] += polycofs[0];
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : m_curve.Centers[i][j];
		}

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion
	}
}
