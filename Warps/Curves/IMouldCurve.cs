using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Warps.Curves
{
	public interface IMouldCurve
	{
		string Label { get; }

		double Length { get; }

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
}
