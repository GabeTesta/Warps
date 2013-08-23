using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBFPolynomials
{
	public interface IRBFPolynomial
	{
		int Terms { get; }
		void Poly(double[] p, double[] d, double[] dd);
		double FitMat(int i, int j);
		double this[int i] { get; set; }
		//RBF.SurfaceRBF Surface { get; set; }
		//RBF.RBFCurve Curve { get; set; }

	}
}
