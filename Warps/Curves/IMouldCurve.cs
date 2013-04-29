using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Geometry;

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
		public static bool CrossPoint(IMouldCurve A, IMouldCurve B, ref Vect2 uv, ref Vect3 xyz, ref Vect2 sPos, int nRez)
		{
			//h(x) = 1st curve (this)
			//g(x) = 2nd curve
			//h(x) - g(x) = 0
			//
			//therefore:
			//f(x) = h(x) - g(x)
			//Xn+1 = Xn - f(Xn)/f'(Xn);

			// move in s on each curve

			Vect2 uv1, uv2, uvtmp, duv1, duv2;
			uv1 = new Vect2(); uv2 = new Vect2(); uvtmp = new Vect2(); duv1 = new Vect2(); duv2 = new Vect2();

			double length = 0;
			//const int NUMPTS = 101;
			const double ALI_EPSILON = 0.1e-11;
			const double TOLERANCE = 0.1e-8;

			// check endpoints first
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					B.uVec(i, ref uv1, ref duv1); // evaluate s's to get distance between

					A.uVec(j, ref uv2, ref duv2); // evaluate s's to get distance between

					length = uv1.Distance(uv2); // get distance

					if (length < TOLERANCE)
					{ // intersection found
						A.xVal(j, ref uv, ref xyz);
						return true;
					}
				}
			}

			List<Vect2> uas = new List<Vect2>(nRez);
			List<Vect2> ubs = new List<Vect2>(nRez);

			List<Vect3> xas = new List<Vect3>(nRez);
			List<Vect3> xbs = new List<Vect3>(nRez);

			int ia100, ib100, iNwt;

			Vect2 tmpU = new Vect2(); Vect3 tmpX = new Vect3();
			for (ia100 = 0; ia100 < nRez; ia100++)
			{
				A.xVal(BLAS.interpolant(ia100, nRez), ref tmpU, ref tmpX);
				uas.Add(new Vect2(tmpU));
				xas.Add(new Vect3(tmpX));

				B.xVal(BLAS.interpolant(ia100, nRez), ref tmpU, ref tmpX);
				ubs.Add(new Vect2(tmpU));
				xbs.Add(new Vect3(tmpX));
			}

			double dx;
			double dus, dMin, ta, tb, td, ad, bd, r1, r2;
			ta = 0; tb = 0;
			Vect2 du, ua, ub, da, db;
			du = new Vect2(); ua = new Vect2(); ub = new Vect2();
			da = new Vect2(); db = new Vect2();

			dMin = 1e6;
			//	initialize minimum distance squared
			for (ia100 = 0; ia100 < nRez; ia100++)
			{
				for (ib100 = 0; ib100 < nRez; ib100++)
				{
					du = uas[ia100] - ubs[ib100];

					//	calculate (u)-distance squared between permutation of 1/100 points
					dus = du.Norm;
					dx = xas[ia100].Distance(xbs[ib100]);
					//	track the minimum distance
					if (dMin > dx)
					{
						dMin = dx;
						ta = BLAS.interpolant(ia100, nRez);
						tb = BLAS.interpolant(ib100, nRez);
					}
				}
			}
			Vect3 xa, xb; xa = new Vect3(); xb = new Vect3();
			bool bLimit = true;
			//	Newton-Raphson iteration from closest 1/100 points
			for (iNwt = 0; iNwt < 250; iNwt++)
			{
				//	enforce end point limits

				if (bLimit)
				{
					ta = ta > 0 ? ta : 0;
					ta = ta < 1 ? ta : 1;
					tb = tb > 0 ? tb : 0;
					tb = tb < 1 ? tb : 1;
				}

				A.uVec(ta, ref ua, ref da);
				A.xVal(ta, ref ua, ref xa);

				B.uVec(tb, ref ub, ref db);
				B.xVal(tb, ref ub, ref xb);

				r1 = ua[0] - ub[0];
				r2 = ua[1] - ub[1];

				if (Math.Abs(r1) < ALI_EPSILON && Math.Abs(r2) < ALI_EPSILON)
				{
					//	enforce end point limits
					if (bLimit)
					{
						if (ta < 0 && Math.Pow(ta, 2) > TOLERANCE)
							return false; // TR 29 Jan 09 ta = 0;
						if (ta > 1 && Math.Pow(1.0 - ta, 2) > TOLERANCE)
							return false; // TR 29 Jan 09 ta = 1;

						if (tb < 0 && Math.Pow(tb, 2) > TOLERANCE)
							return false; // TR 29 Jan 09 tb = 0;
						if (tb > 1 && Math.Pow(1.0 - tb, 2) > TOLERANCE)
							return false; // TR 29 Jan 09 tb = 1;
					}
					//	chop off round off errors
					//*(pa) = ta;
					//*(pb) = tb;
					//	which may effect the (u)-coordinates
					A.uVec(ta, ref ua, ref da);
					uv = new Vect2(ua);
					A.xVal(uv, ref xyz);
					//	return with updated calling parameters
					//us[0] = ua[0];
					//us[1] = ua[1];
					sPos[0] = ta;
					sPos[1] = tb;

					return true;
				}//if( fabs( r1 ) < ALI_EPSILON && fabs( r2 ) < ALI_EPSILON )

				td = da[1] * db[0] - da[0] * db[1];

				if (Math.Abs(td) < .1e-9)
				{
					break;
				}

				ad = (db[0] * r2 - db[1] * r1) / td;
				bd = (da[0] * r2 - da[1] * r1) / td;

				td = Math.Max(Math.Abs(ad), Math.Abs(bd));
				//	enforce maximum increment
				if (td > .1)
				{
					ad *= .1 / td;
					bd *= .1 / td;
				}

				ta -= ad;
				tb -= bd;
			}//for( iNwt=0; iNwt<250; iNwt++ )

			//	no_intersection terminate in error
			return false;
		}

		public static Point3D[] GetEvenPathPoints(IMouldCurve c, int CNT)
		{
			double s;
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Point3D[] d = new Point3D[CNT];
			for (int i = 0; i < CNT; i++)
			{
				s = (double)i / (double)(CNT - 1);
				c.xVal(s, ref uv, ref xyz);
				Utilities.Vect3ToPoint3D(ref d[i], xyz);
			}
			return d;
		}
		public static Point3D[] GetCloudPoints(MouldCurve c)
		{
			Point3D[] pnts;
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();

			pnts = new Point3D[c.FitPoints.Length];
			double s;
			for (int i = 0; i < c.FitPoints.Length; i++)
			{
				s = c[i].S;
				c.xVal(s, ref uv, ref xyz);
				Utilities.Vect3ToPoint3D(ref pnts[i], xyz);
			}
			return pnts;
		}
		//public Point3D[] GetCombPoints(IMouldCurve c, int CNT, out Point3D[] combs, bool bNor)
		//{
		//	bool useRad = false;
		//	double s, k = 0;
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3(), dx = new Vect3(), ddx = new Vect3(), xnor = new Vect3();
		//	Point3D[] d = new Point3D[CNT];
		//	combs = new Point3D[CNT];
		//	for (int i = 0; i < CNT; i++)
		//	{
		//		s = (double)i / (double)(CNT - 1);
		//		if (useRad)
		//			c.xRad(s, ref uv, ref xyz, ref k);
		//		else
		//			c.xCvt(s, ref uv, ref xyz, ref dx, ref ddx);

		//		if (useRad)
		//		{
		//			c.Surface.xNor(uv, ref xyz, ref xnor);
		//			xnor.Scale(k * 5.0);
		//			dx = xyz + xnor;
		//			//BLAS.scale(ref xnor, k * 5);
		//			//dx = BLAS.add(xyz, xnor);
		//		}
		//		else
		//			if (bNor)
		//			{
		//				c.Surface.xNor(uv, ref xyz, ref xnor);
		//				k = ddx.Magnitude / 10.0;
		//				//xnor.Scale(k);
		//				dx = xyz + xnor;
		//				//k = BLAS.magnitude(ddx) / 10.0;
		//				//BLAS.scale(ref xnor, k);
		//				//dx = BLAS.add(xyz, xnor);
		//			}
		//			else
		//			{
		//				ddx.Scale(.01);
		//				dx = xyz + ddx;
		//				//BLAS.scale(ref ddx, .01);
		//				//dx = BLAS.add(xyz, ddx);
		//			}

		//		Utilities.Vect3ToPoint3D(ref d[i], xyz);
		//		Utilities.Vect3ToPoint3D(ref combs[i], dx);
		//	}
		//	return d;
		//}

	}
}
