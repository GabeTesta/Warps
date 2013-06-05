using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	class AnglePoint : CurvePoint
	{
		public AnglePoint() : this(0, null, 0, 0) { }
		public AnglePoint(AnglePoint s)
			: base(s) { m_angle = s.m_angle; }
		//	: this(s.m_sPos, s.m_curve, s.m_sCurve) { }
		public AnglePoint(MouldCurve curve, double angle)
			: this(0, curve, 0, angle) { }
		public AnglePoint(double s, MouldCurve curve, double sCurve, double angle)
			: base(s, curve, sCurve) { m_angle = angle; }

		double m_angle = 0;
		public double Angle
		{
			get { return m_angle; }
			set { m_angle = value; }
		}

		public override IFitPoint Clone()
		{
			return new AnglePoint(this);
		}

		/// <summary>
		/// Sets the AnglePoint's sCurve value to match the desired angle and starting point
		/// </summary>
		/// <param name="g"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		internal static bool SetAnglePoint(ISurface s, IFitPoint start, AnglePoint end)
		{
			double d = 0;
			Vect2 uv = new Vect2();
			Vect3 xs = new Vect3(), xyz = new Vect3(), dx = new Vect3();
			Vect3 dxRef = new Vect3(0, 1, 0);//default ref to horizontal?
			if (start is CurvePoint)//ref to curve tangent if available
				(start as CurvePoint).Curve.xVec((start as CurvePoint).SCurve, ref uv, ref xs, ref dxRef);

			//get a better starting guess endpoint
			s.xVal(start.UV, ref xs);
			double sguess = end.SCurve;
			xyz.Set(xs);//copy xyz to xend
			if (end.Curve.xClosest(ref sguess, ref uv, ref xyz, ref d, 1e-7, false))
				end.SCurve = sguess;

			//slide endpoint until cord is at desired angle 
			int nNwt = 0;
			for (nNwt = 0; nNwt < 50; nNwt++)
			{
				end.Curve.xVec(end.SCurve, ref uv, ref xyz, ref dx);
				xyz -= xs; //get vector from point to point
				double angle = xyz.AngleTo(dxRef);
				if (BLAS.is_equal(angle, end.Angle, 1e-5)) //cord angle matches target
					break;
				
				//angle = end.Angle - angle;//angle error

				//tangent pertibation
				double r = xyz.Magnitude;
				//double S = Math.Sign(dx.Dot(xyz));//ensure proper step direction
				dx += xyz;
				r += dx.Magnitude;
				r /= 2;//get average radius for arc-length approx

				double dtheta = dx.AngleTo(dxRef)-angle;//get angle change from tangent step
				//dt = T / dt;//dTan/dAngle

				double ds = (end.Angle - angle) / dtheta ;//get s-step from desired angle change
				//Utilities.LimitRange(-0.1, ref ds, 0.1);//max/min step limits
				end.SCurve += ds;

				if (nNwt < 5)//keep inbounds initially
					end.SCurve = Utilities.LimitRange(0, end.SCurve, 1);
			}
			return nNwt < 50;
		}


	}
}
