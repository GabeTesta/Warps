using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	public interface IMouldCurve
	{
		void uVal(double s, ref Vect2 uv);
		void uVec(double s, ref Vect2 uv, ref Vect2 du);
		void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu);
		void uNor(double s, ref Vect2 uv, ref Vect2 un);

		void xVal(double s, ref Vect2 uv, ref Vect3 xyz);
		void xVal(Vect2 uv, ref Vect3 xyz);

		void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx);
		void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx);

		void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k);
		void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn);
	}
	public static class CurveTools
	{
		public static bool xClosest(IMouldCurve c, ref double s, ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol, bool bUseGuess)
		{
			Vect3 x = new Vect3(xyzTarget);
			Vect3 dx = new Vect3(), ddx = new Vect3();

			Vect3 h = new Vect3();
			Vect2 e = new Vect2();
			double deds;

			//try at each 18th point to ensure global solution
			double s0 = s;
			double[] sGuesses;// = new double[] { 0, .125, .25, .375, .5, .625, .75, .875, 1 };
			if (bUseGuess)
				sGuesses = new double[] { s };
			else
				sGuesses = new double[] { 0, .125, .25, .375, .5, .625, .75, .875, 1 };

			double sCur = -1, hCur = 1e9;
			for (int nGuess = 0; nGuess < sGuesses.Length; nGuess++)
			{
				s = sGuesses[nGuess];//starting guess
				int loop = 0, max_loops = 100;
				while (loop++ < max_loops)
				{
					c.xCvt(s, ref uv, ref x, ref dx, ref ddx);

					h = x - xyzTarget;
					dist = h.Magnitude;

					e[0] = s;
					e[1] = h.Dot(dx); // error, dot product is 0 at pi/2

					if (Math.Abs(e[1]) < tol) // error is less than the tolerance
					{
						if (dist < hCur)//store best result
						{
							sCur = s;
							hCur = dist;
							break;
						}
						//xyzTarget.Set(x);// return point to caller
						//return true;
					}

					deds = dx.Norm + h.Dot(ddx);
					deds = e[1] / deds;
					// calculate a new s (enforce maximum increment)
					deds = 0.1 > Math.Abs(deds) ? deds : 0.1 * Math.Sign(deds);
					s = e[0] - deds;
					//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
				}
			}
			if (sCur != -1) //if successful return parameters to caller
			{
				c.xVal(sCur, ref uv, ref xyzTarget);
				dist = hCur;
				s = sCur;
				return true;
			}
			//s = s0;
			return false;
		}
	}
}
