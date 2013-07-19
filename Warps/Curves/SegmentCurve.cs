using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps.Curves
{
	[System.Diagnostics.DebuggerDisplay("{m_curve.Label} {m_sLimit}", Name = "{m_curve.Label} {m_sLimit}", Type = "{GetType()}")]
	public class SegmentCurve: IMouldCurve
	{
		public SegmentCurve(IMouldCurve curve, double smin, double smax)
		{
			m_curve = curve;
			m_sLimit = new Vect2(smin, smax);//ensure clean copy
		}
		public SegmentCurve(IMouldCurve curve, Vect2 sLimits)
			: this(curve, sLimits[0], sLimits[1])
		{
		}

		#region Properties

		public double Min
		{
			get { return m_sLimit[0]; }
			set { m_sLimit[0] = value; }
		}
		public double Max
		{
			get { return m_sLimit[1]; }
			set { m_sLimit[1] = value; }
		}
		public IMouldCurve Curve
		{
			get { return m_curve; }
			private set { m_curve = value; }
		}

		IMouldCurve m_curve;
		Vect2 m_sLimit;
		#endregion

		#region IMouldCurve Members

		/// <summary>
		/// convert from [0,1] to [sLim, sLim]
		/// </summary>
		/// <param name="p">the position along this curve [0,1]</param>
		/// <returns>the position on the base curve [sLim, sLim]</returns>
		double SPos(double p)
		{
			return BLAS.interpolate(p, Max, Min);
		}

		public void uVal(double s, ref Vect2 uv)
		{
			m_curve.uVal(SPos(s), ref uv);
		}

		public void uVec(double s, ref Vect2 uv, ref Vect2 du)
		{
			m_curve.uVec(SPos(s), ref uv, ref du);
		}

		public void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu)
		{
			m_curve.uCvt(SPos(s), ref uv, ref du, ref ddu);
		}

		public void uNor(double s, ref Vect2 uv, ref Vect2 un)
		{
			m_curve.uNor(SPos(s), ref uv,ref un);
		}

		public void xVal(double s, ref Vect2 uv, ref Vect3 xyz)
		{
			m_curve.xVal(SPos(s), ref uv, ref xyz);
		}

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			m_curve.xVal(uv, ref xyz);
		}

		public void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx)
		{
			m_curve.xVec(SPos(s), ref uv, ref xyz, ref dx);
		}

		public void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx)
		{
			m_curve.xCvt(SPos(s), ref uv, ref xyz, ref dx, ref ddx);
		}

		public void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k)
		{
			m_curve.xRad(SPos(s), ref uv, ref xyz, ref k);
		}

		public void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn)
		{
			m_curve.xNor(SPos(s), ref uv, ref xyz, ref dx, ref xn);
		}

		#endregion
	}
}
