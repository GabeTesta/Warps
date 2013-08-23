using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBFBasis
{
	//public interface IBasisFunction
	//{
	//	double val(double r);
	//	/* returns the value of the first derivative of this function with respect to the radius */
	//	double dr(double r);
	//	/* returns the value of the second derivative of this function with respect to the radius */
	//	double ddr(double r);
	//}

	/// <summary>
	/// Thin Plate Spline Basis Function (r*r*log(r))
	/// </summary>
	public class ThinPlateSpline : IBasisFunction
	{
		public IBasisFunction Clone()
		{
			return new ThinPlateSpline();
		}
		#region IBasisFunction Members

		public double val(double r)
		{
			return r == 0 ? 0 : r * r * Math.Log(r);
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : (2 * r * Math.Log(r) + r);
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : (2 * Math.Log(r) + 3);
		}

		#endregion

		public override string ToString()
		{
			return "r*r*ln(r)";
		}
	}

	public class Gaussian : IBasisFunction
	{
		public IBasisFunction Clone()
		{
			return new Gaussian(B);
		}
		double B = 0.01;

		public Gaussian(double beta)
		{
			B = beta;
		}

		#region IBasisFunction Members

		public double val(double r)
		{
			return r == 0 ? 0 : Math.Exp(-Math.Pow(B, 2) * Math.Pow(r, 2));
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : -2*r*Math.Exp(-Math.Pow(r,2)*Math.Pow(B,2))*Math.Pow(B,2);
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : (4 * Math.Pow(r, 2) * Math.Pow(B, 2) - 2 * Math.Pow(B, 2)) * Math.Exp(-Math.Pow(r, 2) * Math.Pow(B, 2));
		}

		#endregion

		public override string ToString()
		{
			return "e^[-(B*B) * (r*r)]";
		}
	}

	public class Multiquadratic : IBasisFunction
	{
		public IBasisFunction Clone()
		{
			return new Multiquadratic(B);
		}
		double B = 0.5;

		public Multiquadratic(double beta)
		{
			B = beta;
		}

		#region IBasisFunction Members

		public double val(double r)
		{
			return r == 0 ? 0 : Math.Sqrt((r * r) + (B * B));
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : r / Math.Sqrt(B * B + r * r); //r/(B**2 + r**2)**(1/2)
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : -(r*r)/Math.Pow(B*B+r*r,3/2) + Math.Pow((B*B + r*r),-1/2); //-r**2/(B**2 + r**2)**(3/2) + (B**2 + r**2)**(-1/2)
		}

		#endregion

		public override string ToString()
		{
			return "SQRT[(r*r) + (B*B)]";
		}
	}

	public class InversMultiquadratic : IBasisFunction
	{
		public IBasisFunction Clone()
		{
			return new InversMultiquadratic(B);
		}

		double B = 0.5;

		public InversMultiquadratic(double beta)
		{
			B = beta;
		}
		#region IBasisFunction Members

		public double val(double r)
		{
			return r == 0 ? 0 : 1/Math.Sqrt((r * r) + (B * B)); 
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : -r / Math.Pow(B * B + r * r,3/2); 
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : (2*Math.Pow(r,2)-Math.Pow(B,2))/Math.Pow(r*r+B*B,5/2); 
		}

		#endregion
	}

	public class PolyHarmonic : IBasisFunction
	{
		public IBasisFunction Clone()
		{
			return new PolyHarmonic(P);
		}

		uint P = 3;
		public PolyHarmonic(uint power)
		{
			P = power;
		}
		#region IBasisFunction Members

		public double val(double r)
		{
			return r == 0 ? 0 : Math.Pow(r, P);
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : P < 1 ? 0 : P * Math.Pow(r, P - 1);
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : P < 2 ? 0 : P * (P - 1) * Math.Pow(r, P - 2);
		}

		#endregion

		public override string ToString()
		{
			return "r^" + P.ToString();
		}
	}
}
