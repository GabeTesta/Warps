using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	public class YarnCurve: IMouldCurve
	{
		public YarnCurve(double p, MouldCurve warp1, MouldCurve warp2)
		{
			m_h = 0;
			m_p = p;
			m_Warps[0] = warp1;
			m_Warps[1] = warp2;
		}

		internal MouldCurve[] m_Warps = new MouldCurve[2];
		internal double m_p;
		internal double m_h;
		internal double m_length;

		ISurface Surface 
		{get { return m_Warps[0] == null ? null : m_Warps[0].Surface; }}

		///// <summary>
		///// returns CNT evenly spaced points, inferior to CreateEntity
		///// </summary>
		///// <param name="CNT">the number of points desired on the line</param>
		///// <returns>the array of points</returns>
		//public Vect3[] GetPathPoints(int CNT)
		//{
		//	double s;
		//	Vect2 uv = new Vect2();
		//	Vect3[] d = new Vect3[CNT];
		//	for (int i = 0; i < CNT; i++)
		//	{
		//		s = (double)i / (double)(CNT - 1);
		//		d[i] = new Vect3();
		//		xVal(s, ref uv, ref d[i]);
		//	}
		//	return d;
		//}

		public double Length
		{
			get
			{
				if (m_length == 0)
				{
					List<double> sPos;
					CurveTools.GetPathPoints(this, 2 * Math.PI / 180.0, null, false, out m_length, out sPos);
				}
				return m_length;
			}
		}
		#region IMouldCurve Members

		public void uVal(double s, ref Vect2 uv)
		{
			Vect2 uv1 = new Vect2();
			m_Warps[0].uVal(s, ref uv);
			m_Warps[1].uVal(s, ref uv1);

			for( int i =0; i < 2; i++ )
				uv[i] = BLAS.interpolate(m_p, uv1[i], uv[i]);
		}

		public void uVec(double s, ref Vect2 uv, ref Vect2 du)
		{
			Vect2 uv1 = new Vect2(), du1 = new Vect2();
			m_Warps[0].uVec(s, ref uv, ref du);
			m_Warps[1].uVec(s, ref uv1, ref du1);

			//interpolate u point and derivatives
			for (int i = 0; i < 2; i++)
			{
				uv[i] = BLAS.interpolate(m_p, uv1[i], uv[i]);
				du[i] = BLAS.interpolate(m_p, du1[i], du[i]);
			}
		}

		public void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu)
		{
			Vect2 uv1 = new Vect2(), du1 = new Vect2(), ddu1 = new Vect2() ;
			m_Warps[0].uCvt(s, ref uv, ref du, ref ddu);
			m_Warps[1].uCvt(s, ref uv1, ref du1, ref ddu1);

			//interpolate u point and derivatives
			for (int i = 0; i < 2; i++)
			{
				uv[i] = BLAS.interpolate(m_p, uv1[i], uv[i]);
				du[i] = BLAS.interpolate(m_p, du1[i], du[i]);
				ddu[i] = BLAS.interpolate(m_p, ddu1[i], ddu[i]);
			}
		}
		public void uNor(double s, ref Vect2 uv, ref Vect2 un)
		{
			Vect2 ut = new Vect2();
			Vect3 xyz = new Vect3(), dxu = new Vect3(), dxv = new Vect3();
			uVec(s, ref uv, ref ut);
			Surface.xVec(uv, ref xyz, ref dxu, ref dxv);
			//covariant metric tensor components
			double a11 = dxu.Norm;
			double a12 = dxu.Dot(dxv);
			double a22 = dxv.Norm;

			double det = Math.Sqrt(a11 * a22 - a12 * a12);

			//contravariant normal vector components in the surface plane
			un[0] = (a12 * ut[0] + a22 * ut[1]) / det;
			un[1] = -(a11 * ut[0] + a12 * ut[1]) / det;

			//return unit normal u components
			un.Magnitude = 1;
		}

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			Surface.xVal(uv, ref xyz);
		}
		public void xVal(double s, ref Vect2 uv, ref Vect3 xyz)
		{
			uVal(s, ref uv);
			xVal(uv, ref xyz);
		}
		public void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx)
		{
			Vect2 du = new Vect2();
			uVec(s, ref uv, ref du);

			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Surface.xVec(uv, ref xyz, ref dxu, ref dxv);
			//calculate derivatives and xyz point from the warp's surface
			for (int ix = 0; ix < 3; ix++)
			{
				dx[ix] = dxu[ix] * du[0]
					  + dxv[ix] * du[1];
			}
		}
		public void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx)
		{
			Vect2 du = new Vect2(), ddu = new Vect2();
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();

			uCvt(s, ref uv, ref du, ref ddu);
			Surface.xCvt(uv, ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

			for (int ix = 0; ix < 3; ix++)
			{
				dx[ix] = dxu[ix] * du[0] + dxv[ix] * du[1];

				ddx[ix] = dxu[ix] * ddu[0] + dxv[ix] * ddu[1]
					   + ddxu[ix] * du[0] * du[0]
					   + dduv[ix] * du[0] * du[1] * 2.0
					   + ddxv[ix] * du[1] * du[1];
			}
		}

		public void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k)
		{
			Vect3 dx = new Vect3(), ddx = new Vect3();

			xCvt(s, ref uv, ref xyz, ref dx, ref ddx);

			Vect3 cross = dx.Cross(ddx);
			k = cross.Magnitude / Math.Pow(dx.Magnitude, 3);
		}
		public void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn)
		{
			xVec(s, ref uv, ref xyz, ref dx);
			Vect3 xNor = new Vect3();
			Surface.xNor(uv, ref xyz, ref xNor);
			xn = dx.Cross(xNor);
			xn.Magnitude = 1;//unitize
		}

		#endregion

		public string Label
		{
			get { return ToString(); }
		}


		public override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", m_p.ToString("f3"), m_Warps[0].Label, m_Warps[1].Label, m_h.ToString("f4"));
		}
	}
}
