using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;

namespace Warps
{
	internal static class Geodesic
	{
		/// <summary>
		/// Fits a multi-segement geodesic between the specified fitpoints optionally specifying which segments to girth
		/// </summary>
		/// <param name="points">the array of fitpoints, must be more than 2</param>
		/// <param name="bGirths">an array specifying which segments to girth and which to spline 
		/// count is 1 less than the fitpoints count
		/// can be null which defaults to first and last segment girths</param>
		/// <returns>true if successful, false otherwise</returns>
		internal static bool Geo(MouldCurve g, IFitPoint[] points)
		{

			if (points == null || points.Length < 2)
				return false;
			else if (points.Length == 2 && g.IsGirth(0))
				return Geo(g, points[0], points[1]);
			
			
			//compiled piecewise values
			List<double> S = new List<double>();
			List<Vect2> U = new List<Vect2>();

			//segment values
			double[] spos = null;
			Vect2[] upos = null;

			Vect3 xyz = new Vect3(), xyp = new Vect3();

			//initialize S and U
			points[0].S = 0;
			S.Add(0);
			U.Add(points[0].UV);
			for (int i = 1; i < points.Length; i++)
			{
				//span a geo between the fitpoints
				//if (i % 2 == 1)
				if( g.IsGirth(i-1) )
				{
					if (!GeoSegment(g, points[i - 1], points[i], ref spos, ref upos))
						return false;

					for (int nS = 1; nS < spos.Length; nS++)
					{
						//scale the spos by length to get distance
						S.Add(spos[nS] * g.Length + points[i - 1].S);
						//copy the upos
						U.Add(upos[nS]);
					}
				}
				else// if (i % 2 == 0)
				{
					g.xVal(points[i - 1].UV, ref xyp);
					g.xVal(points[i].UV, ref xyz);
					S.Add(xyz.Distance(xyp) + points[i-1].S);
					U.Add(points[i].UV);
				}
				points[i].S = S.Last();//store the curent length for paramterization
			}

			//reparamaterize
			g.Length = S.Last();
			for (int i = 0; i < S.Count; i++)
				S[i] /= g.Length;

			for (int i = 0; i < points.Length; i++)
				points[i].S /= g.Length;

			//spline the combined points
			g.ReSpline(S.ToArray(), U.ToArray());
			g.FitPoints = points;
			return true;
		}

		/// <summary>
		/// Spans a girth between the start and end point
		/// faster than using Geo(IFitPoint[] fits) for 2 points
		/// </summary>
		/// <param name="g">the curve used for splining</param>
		/// <param name="start">the starting fitpoint</param>
		/// <param name="end">the ending fitpoint</param>
		/// <returns>true if successful, false otherwise</returns>
		internal static bool Geo(MouldCurve g, IFitPoint start, IFitPoint end)
		{
			double[] s = null; 
			Vect2[] u = null;
			start.S = 0;
			end.S = 1;
			return GeoSegment(g, start, end, ref s, ref u);
		}

		/// <summary>
		/// Fits a geodesic between the start and end points and returns the geo fit arrays for post-processing
		/// </summary>
		/// <param name="g">the curve object to fit with</param>
		/// <param name="start">the starting point of the segment</param>
		/// <param name="end">the end point of the segment</param>
		/// <param name="sFits">in: null, out: the segment s-positions of each geo fitting point</param>
		/// <param name="uFits">in: null, out: the segment u-positions of each geo fitting point</param>
		/// <returns>true if successful, false otherwise</returns>
		internal static bool GeoSegment(MouldCurve g, IFitPoint start, IFitPoint end, ref double[] sFits, ref Vect2[] uFits)
		{
			if (start == end || start.UV == end.UV || (start is SlidePoint && end is SlidePoint))
				return false;

			//start.S = 0;
			//end.S = 1;
			int REZ = 9;
			int END = REZ - 1;
			int i;

			double d = 0, sguess;
			Vect3 xI = new Vect3(), xs = new Vect3(), xe = new Vect3();

			Vect2 utemp = new Vect2();
			uFits = new Vect2[REZ];
			sFits = new double[REZ];
			//attempt a better starting slide position
			if (start is SlidePoint)
			{
				g.xVal(end.UV, ref xI);
				sguess = (start as SlidePoint).SCurve;
				if ((start as SlidePoint).Curve.xClosest(ref sguess, ref utemp, ref xI, ref d, 1e-9, false))
					(start as SlidePoint).SCurve = sguess;
			}

			sFits[0] = 0;
			uFits[0] = new Vect2(start.UV);

			//attempt a better starting slide position
			if (end is SlidePoint)
			{
				g.xVal(uFits[0], ref xI);
				sguess = (end as SlidePoint).SCurve;
				if ((end as SlidePoint).Curve.xClosest(ref sguess, ref utemp, ref xI, ref d, 1e-9, false))
					(end as SlidePoint).SCurve = sguess;
			}
			sFits[END] = 1;
			uFits[END] = new Vect2(end.UV);

			//interpolate 8 internal points in xyz
			g.xVal(uFits[0], ref xs);
			g.xVal(uFits[END], ref xe);
			for (i = 1; i < END; i++)
			{
				uFits[i] = new Vect2();
				sFits[i] = (double)i / (double)END;
				for (int j = 0; j < 3; j++)
					xI[j] = BLAS.interpolate(sFits[i], xe[j], xs[j]);
				for (int j = 0; j < 2; j++)//interpolate starting uv guess
					uFits[i][j] = BLAS.interpolate(sFits[i], uFits[END][j], uFits[0][j]);
				if (!g.Surface.xClosest(ref uFits[i], ref xI, ref d, 1e-5))
					for (int j = 0; j < 2; j++)//default to uv interp if xyz fails
						uFits[i][j] = BLAS.interpolate(sFits[i], uFits[END][j], uFits[0][j]);
			}

			if (!FitGeo(g, start, end, sFits, uFits))
				return false;

			RefineGeo(g, ref sFits, ref uFits);

			return FitGeo(g, start, end, sFits, uFits);
		}
		
		/// <summary>
		/// using the start, end and passed geo fit points will slide the points along their normals to achieve a geodesic
		/// </summary>
		/// <param name="g">the curve object to spline</param>
		/// <param name="start">the starting point of this segment</param>
		/// <param name="end">the ending point of this segment</param>
		/// <param name="sFits">the s-positons of each geo-point</param>
		/// <param name="uFits">the u-positons of each geo-point</param>
		/// <returns>true if successful, false otherwise</returns>
		static bool FitGeo(MouldCurve g, IFitPoint start, IFitPoint end, double[] sFits, Vect2[] uFits)
		{
			int NumFits = sFits.Length;
			int INC = NumFits - 1;
			int i;

			Vect2[] uNor = new Vect2[NumFits];
			Vect3 xyz = new Vect3(), dxu = new Vect3(), dxv = new Vect3();
			Vect2 ut = new Vect2(), un = new Vect2();
			double a11, a12, a22, det;
			//calculate insurface normals at each fitpoint
			uNor[0] = new Vect2();//fixed endpoint doesnt need normal
			for (i = 1; i < INC; i++)
			{
				g.Surface.xVec(uFits[i], ref xyz, ref dxu, ref dxv);
				//covariant metric tensor components
				a11 = dxu.Norm;
				a12 = dxu.Dot(dxv);
				a22 = dxv.Norm;

				det = Math.Sqrt(a11 * a22 - a12 * a12);

				//tangent(secant) vector u components 
				ut = uFits[i + 1] - uFits[i - 1];

				//contravariant normal vector components in the surface plane
				un[0] = (a12 * ut[0] + a22 * ut[1]) / det;
				un[1] = -(a11 * ut[0] + a12 * ut[1]) / det;

				//store unit normal u components
				un.Magnitude = 1;
				uNor[i] = new Vect2(un);
			}
			uNor[INC] = new Vect2();//fixed endpoint doesnt need normal

			Vect3 xPrev = new Vect3();
			Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();
			Vect3[] xNor = new Vect3[NumFits];
			Vect3[] dxNor = new Vect3[NumFits];
			Vect3[] xTan = new Vect3[NumFits];
			double[] xLen = new double[NumFits];
			bool Conver = false;
			int nNwt; Vector x; DenseVector sNor;
			for (nNwt = 0; nNwt < 50; nNwt++)
			{
				xNor[0] = new Vect3();
				//update startpoint slide position
				if (start is SlidePoint)
				{
					dxNor[0] = new Vect3();
					(start as SlidePoint).Curve.xCvt((start as SlidePoint).SCurve, ref uFits[0], ref xPrev, ref xNor[0], ref dxNor[0]);
					//uFits[0][0] = FitPoints[0][1];
					//uFits[0][1] = FitPoints[0][2];
				}
				else
					g.xVal(uFits[0], ref xPrev);

				//update endpoint slide position
				if (end is SlidePoint)
				{
					uFits[INC][0] = end[1];
					uFits[INC][1] = end[2];
				}

				xLen[0] = 0;
				//calc internal point vectors
				for (i = 1; i < NumFits; i++)
				{
					g.Surface.xCvt(uFits[i], ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

					a11 = uNor[i][0] * uNor[i][0];
					a12 = uNor[i][0] * uNor[i][1] * 2;
					a22 = uNor[i][1] * uNor[i][1];

					// insurface normal x components
					dxu.Scale(uNor[i][0]);
					dxv.Scale(uNor[i][1]);
					xNor[i] = dxu + dxv;

					//insurface normal x derivatives
					ddxu.Scale(a11);
					dduv.Scale(a12);
					ddxv.Scale(a22);
					dxNor[i] = ddxu + dduv + ddxv;

					//forward facing tangent vector
					xTan[i] = xyz - xPrev;
					xPrev.Set(xyz);

					xLen[i] = xTan[i].Magnitude;//segment length
					xLen[0] += xLen[i];//accumulate total length

					xTan[i].Magnitude = 1;//unit tangent vector
				}
				//update endpoint slide position
				if (end is SlidePoint)
				{
					(end as SlidePoint).Curve.xCvt((end as SlidePoint).SCurve, ref uFits[INC], ref xyz, ref xNor[INC], ref dxNor[INC]);
				}

				DenseMatrix A = new DenseMatrix(NumFits);
				sNor = new DenseVector(NumFits);
				double p0, pp, d, d0, dp, gm, g0, gp; int ix;

				//slide startpoint
				if (start is SlidePoint)
				{
					//mid point normal vector dotted with end point tangent vectors
					pp = xNor[0].Dot(xTan[1]);

					for (g0 = gp = 0, ix = 0; ix < 3; ix++)
					{
						//midpoint tangent and curavture variantion
						dp = (xNor[0][ix] - pp * xTan[1][ix]) / xLen[1];

						//mid and top point gradients
						g0 += -dp * xNor[0][ix] + xTan[1][ix] * dxNor[0][ix];
						gp += dp * xNor[1][ix];
					}
					//geodesic residual and gradients
					A[0, 0] = g0;
					A[0, 1] = gp;
					sNor[0] = pp;
				}
				else//fixed start point
				{
					A[0, 0] = 1;
					sNor[0] = 0;
				}

				for (i = 1; i < INC; i++)//internal points
				{
					//midpoint normal dotted with tangents
					p0 = xNor[i].Dot(xTan[i]);// BLAS.dot(xNor[i], xTan[i]);
					pp = xNor[i].Dot(xTan[i + 1]);//BLAS.dot(xNor[i], xTan[i + 1]);

					for (gm = g0 = gp = 0, ix = 0; ix < 3; ix++)
					{
						//midpoint curvature vector
						d = xTan[i + 1][ix] - xTan[i][ix];

						//midpoint tangent and curavture variantion
						d0 = (xNor[i][ix] - p0 * xTan[i][ix]) / xLen[i];
						dp = (xNor[i][ix] - pp * xTan[i + 1][ix]) / xLen[i + 1];

						//bottom, mid and top point gradients
						gm += d0 * xNor[i - 1][ix];
						g0 += (-d0 - dp) * xNor[i][ix] + d * dxNor[i][ix];
						gp += dp * xNor[i + 1][ix];
					}
					A[i, i - 1] = gm;
					A[i, i] = g0;
					A[i, i + 1] = gp;
					sNor[i] = -p0 + pp;
				}

				if (end is SlidePoint)//slide endpoint
				{
					p0 = xNor[i].Dot(xTan[i]);

					for (gm = g0 = 0, ix = 0; ix < 3; ix++)
					{
						//midpoint tangent and curavture variantion
						d0 = (xNor[i][ix] - p0 * xTan[i][ix]) / xLen[i];

						//bottom, mid and top point gradients
						gm += d0 * xNor[i - 1][ix];
						g0 += -d0 * xNor[i][ix] - xTan[i][ix] * dxNor[i][ix];
					}
					A[i, i - 1] = gm;
					A[i, i] = g0;
					sNor[i] = -p0;
				}
				else//fixed endpoint
				{
					A[i, i] = 1;
					sNor[i] = 0;
				}

				LU decomp = A.LU();
				x = (Vector)decomp.Solve(sNor);

				double Reduce = Math.Min(1, .05 / x.AbsoluteMaximum());

				if( start is SlidePoint)
						(start as SlidePoint).SCurve -= x[0] * Reduce;

				if( end is SlidePoint)
						(end as SlidePoint).SCurve -= x[INC] * Reduce;

				for (i = 1; i < NumFits; i++)//increment uv points
				{
					uFits[i][0] -= x[i] * uNor[i][0] * Reduce;
					uFits[i][1] -= x[i] * uNor[i][1] * Reduce;
				}

				if (nNwt < 5)
				{
					//keep initial (s)-increments within bounds
					if (start is SlidePoint)
						(start as SlidePoint).SCurve = Utilities.LimitRange(0, (start as SlidePoint).SCurve, 1);

					if (end is SlidePoint)
						(end as SlidePoint).SCurve = Utilities.LimitRange(0, (end as SlidePoint).SCurve, 1);

					//	keep initial (u)-increments within bounds
					for (i = 1; i < NumFits - 1; i++)
					{
						uFits[i][0] = Utilities.LimitRange(0, uFits[i][0], 1);
						uFits[i][1] = Utilities.LimitRange(-.125, uFits[i][1], 1.125);
					}
				}
				double xmax = x.AbsoluteMaximum();
				double smax = sNor.AbsoluteMaximum();
				if (Conver = (x.AbsoluteMaximum() < 1e-8 && sNor.AbsoluteMaximum() < 1e-7))
					break;
			}

			if (!Conver)
				return false;

			g.Length = xLen[0];//store length

			//calculate unit length (s)-parameter values
			sFits[0] = 0;
			for (i = 1; i < NumFits; i++)
				sFits[i] = sFits[i - 1] + xLen[i] / xLen[0];
			//g.m_uvs = uFits;
			g.ReSpline(sFits, uFits);
			return true;
		}
		/// <summary>
		/// inserts additional points into the geo-point arrays to ensure a smooth, linear geo
		/// </summary>
		/// <param name="g">the curve object to spline</param>
		/// <param name="sFits">in: the initial set of geo-points s-positions, out: an interpolated set of geo-point s-pos</param>
		/// <param name="uFits">in: the initial set of geo-points u-positions, out: an interpolated set of geo-point u-pos</param>
		static void RefineGeo(MouldCurve g, ref double[] sFits, ref Vect2[] uFits)
		{
			int NumFits = sFits.Length;
			List<double> sPos = new List<double>(NumFits * 10);
			int uAdd, xAdd, sAdd;
			double s, dau;
			Vect2 uv = new Vect2();//, um = new Vect2(), up = new Vect2();
			Vect2 dmu = new Vect2(), dpu = new Vect2();
			Vect3 xyz = new Vect3(), dmx = new Vect3(), dpx = new Vect3();

			sPos.Add(sFits[0]);
			for (int i = 1; i < NumFits; i++)
			{
				s = (sFits[i - 1] + sFits[i]) / 2.0;
				g.xVal(s, ref uv, ref xyz);

				dmu = uFits[i - 1] - uv;
				dpu = uFits[i] - uv;

				dau = Math.Acos(-(dmu.Dot(dpu)) / dmu.Magnitude / dpu.Magnitude);
				uAdd = (int)(dau / (Math.PI / 180.0) + 1);

				g.xVal(uFits[i - 1], ref dmx);
				g.xVal(uFits[i], ref dpx);
				dmx = dmx - xyz;
				dpx = dpx - xyz;
	
				dau = Math.Acos(-(dmx.Dot(dpx)) / dmx.Magnitude / dpx.Magnitude);
				xAdd = (int)(dau / (Math.PI / 180.0) + 1);

				sAdd = Math.Max(xAdd, uAdd);

				for (int iS = 1; iS <= sAdd; iS++)
				{
					s = BLAS.interpolate((double)iS / (double)sAdd, sFits[i], sFits[i - 1]);
					sPos.Add(s);//add evenly spaced s points
				}
			}

			uFits = new Vect2[sPos.Count];
			sFits = new double[sPos.Count];
			int iA = 0;
			foreach (double sA in sPos)
			{
				uFits[iA] = new Vect2();
				g.uVal(sA, ref uFits[iA]);
				sFits[iA] = sA;
				iA++;
			}
		}

		//public override bool InsertPoint(System.Drawing.PointF mouse, Transformer WorldToScreen, out int nIndex)
		//{
		//	nIndex = -1;
		//	return false;//dont allow adding points to girths
		//}

		//internal static bool Geo(MouldCurve g, IFitPoint start, IFitPoint end)
		//{
		//	if (start == end || start.UV == end.UV || (start is SlidePoint && end is SlidePoint))
		//		return false;

		//	g.FitPoints = new IFitPoint[] { start, end };

		//	start.S = 0;
		//	end.S = 1;
		//	int REZ = 9;
		//	int END = REZ - 1;
		//	int i;

		//	double d = 0, sguess;
		//	Vect3 xI = new Vect3(), xs = new Vect3(), xe = new Vect3();

		//	Vect2 utemp = new Vect2();
		//	Vect2[] uFits = new Vect2[REZ];
		//	double[] sFits = new double[REZ];
		//	//attempt a better starting slide position
		//	if (start is SlidePoint)
		//	{
		//		g.xVal(end.UV, ref xI);
		//		sguess = (start as SlidePoint).SCurve;
		//		if ((start as SlidePoint).Curve.xClosest(ref sguess, ref utemp, ref xI, ref d, 1e-9, false))
		//			(start as SlidePoint).SCurve = sguess;
		//	}

		//	sFits[0] = 0;
		//	uFits[0] = new Vect2(start.UV);

		//	//attempt a better starting slide position
		//	if (end is SlidePoint)
		//	{
		//		g.xVal(uFits[0], ref xI);
		//		sguess = (end as SlidePoint).SCurve;
		//		if ((end as SlidePoint).Curve.xClosest(ref sguess, ref utemp, ref xI, ref d, 1e-9, false))
		//			(end as SlidePoint).SCurve = sguess;
		//	}
		//	sFits[END] = 1;
		//	uFits[END] = new Vect2(end.UV);

		//	//interpolate 8 internal points in xyz
		//	g.xVal(uFits[0], ref xs);
		//	g.xVal(uFits[END], ref xe);
		//	for (i = 1; i < END; i++)
		//	{
		//		uFits[i] = new Vect2();
		//		sFits[i] = (double)i / (double)END;
		//		for (int j = 0; j < 3; j++)
		//			xI[j] = BLAS.interpolate(sFits[i], xe[j], xs[j]);
		//		for (int j = 0; j < 2; j++)//interpolate starting uv guess
		//			uFits[i][j] = BLAS.interpolate(sFits[i], uFits[END][j], uFits[0][j]);
		//		if (!g.Surface.xClosest(ref uFits[i], ref xI, ref d, 1e-5))
		//			for (int j = 0; j < 2; j++)//default to uv interp if xyz fails
		//				uFits[i][j] = BLAS.interpolate(sFits[i], uFits[END][j], uFits[0][j]);
		//	}

		//	if (!Geo(g, sFits, uFits))
		//		return false;

		//	RefineGeo(g, ref sFits, ref uFits);

		//	return Geo(g, sFits, uFits);
		//}
		//static bool Geo(MouldCurve g, double[] sFits, Vect2[] uFits)
		//{
		//	int NumFits = sFits.Length;
		//	int INC = NumFits - 1;
		//	int i;

		//	Vect2[] uNor = new Vect2[NumFits];
		//	Vect3 xyz = new Vect3(), dxu = new Vect3(), dxv = new Vect3();
		//	Vect2 ut = new Vect2(), un = new Vect2();
		//	double a11, a12, a22, det;
		//	//calculate insurface normals at each fitpoint
		//	uNor[0] = new Vect2();//fixed endpoint doesnt need normal
		//	for (i = 1; i < INC; i++)
		//	{
		//		g.Surface.xVec(uFits[i], ref xyz, ref dxu, ref dxv);
		//		//covariant metric tensor components
		//		a11 = dxu.Norm;
		//		a12 = dxu.Dot(dxv);
		//		a22 = dxv.Norm;

		//		det = Math.Sqrt(a11 * a22 - a12 * a12);

		//		//tangent(secant) vector u components 
		//		ut = uFits[i + 1] - uFits[i - 1];

		//		//contravariant normal vector components in the surface plane
		//		un[0] = (a12 * ut[0] + a22 * ut[1]) / det;
		//		un[1] = -(a11 * ut[0] + a12 * ut[1]) / det;

		//		//store unit normal u components
		//		un.Magnitude = 1;
		//		uNor[i] = new Vect2(un);
		//	}
		//	uNor[INC] = new Vect2();//fixed endpoint doesnt need normal

		//	Vect3 xPrev = new Vect3();
		//	Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();
		//	Vect3[] xNor = new Vect3[NumFits];
		//	Vect3[] dxNor = new Vect3[NumFits];
		//	Vect3[] xTan = new Vect3[NumFits];
		//	double[] xLen = new double[NumFits];
		//	bool Conver = false;
		//	for (int nNwt = 0; nNwt < 25; nNwt++)
		//	{
		//		xNor[0] = new Vect3();
		//		//update startpoint slide position
		//		if (g.FitPoints[0] is SlidePoint)
		//		{
		//			dxNor[0] = new Vect3();
		//			(g.FitPoints[0] as SlidePoint).Curve.xCvt((g.FitPoints[0] as SlidePoint).SCurve, ref uFits[0], ref xPrev, ref xNor[0], ref dxNor[0]);
		//			//uFits[0][0] = FitPoints[0][1];
		//			//uFits[0][1] = FitPoints[0][2];
		//		}	
		//		else
		//			g.xVal(uFits[0], ref xPrev);

		//		//update endpoint slide position
		//		if (g.FitPoints[1] is SlidePoint)
		//		{
		//			uFits[INC][0] = g.FitPoints[1][1];
		//			uFits[INC][1] = g.FitPoints[1][2];
		//		}

		//		xLen[0] = 0;
		//		//calc internal point vectors
		//		for (i = 1; i < NumFits; i++)
		//		{
		//			g.Surface.xCvt(uFits[i], ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

		//			a11 = uNor[i][0] * uNor[i][0];
		//			a12 = uNor[i][0] * uNor[i][1] * 2;
		//			a22 = uNor[i][1] * uNor[i][1];

		//			// insurface normal x components
		//			dxu.Scale(uNor[i][0]);
		//			dxv.Scale(uNor[i][1]);
		//			xNor[i] = dxu + dxv;

		//			//insurface normal x derivatives
		//			ddxu.Scale(a11);
		//			dduv.Scale(a12);
		//			ddxv.Scale(a22);
		//			dxNor[i] = ddxu + dduv + ddxv;

		//			//forward facing tangent vector
		//			xTan[i] = xyz - xPrev;
		//			xPrev.Set(xyz);

		//			xLen[i] = xTan[i].Magnitude;//segment length
		//			xLen[0] += xLen[i];//accumulate total length

		//			xTan[i].Magnitude = 1;//unit tangent vector
		//		}
		//		//update endpoint slide position
		//		if (g.FitPoints[1] is SlidePoint)
		//		{
		//			(g.FitPoints[1] as SlidePoint).Curve.xCvt((g.FitPoints[1] as SlidePoint).SCurve, ref uFits[INC], ref xyz, ref xNor[INC], ref dxNor[INC]);
		//		}

		//		DenseMatrix A = new DenseMatrix(NumFits);
		//		DenseVector sNor = new DenseVector(NumFits);
		//		double p0, pp, d, d0, dp, gm, g0, gp; int ix;

		//		//slide startpoint
		//		if (g.FitPoints[0] is SlidePoint)
		//		{
		//			//mid point normal vector dotted with end point tangent vectors
		//			pp = xNor[0].Dot(xTan[1]);

		//			for (g0 = gp = 0, ix = 0; ix < 3; ix++)
		//			{
		//				//midpoint tangent and curavture variantion
		//				dp = (xNor[0][ix] - pp * xTan[1][ix]) / xLen[1];

		//				//mid and top point gradients
		//				g0 += -dp * xNor[0][ix] + xTan[1][ix] * dxNor[0][ix];
		//				gp += dp * xNor[1][ix];
		//			}
		//			//geodesic residual and gradients
		//			A[0,0] = g0;
		//			A[0, 1] = gp;
		//			sNor[0] = pp;
		//		}
		//		else//fixed start point
		//		{
		//			A[0, 0] = 1;
		//			sNor[0] = 0;
		//		}

		//		for (i = 1; i < INC; i++)//internal points
		//		{
		//			//midpoint normal dotted with tangents
		//			p0 = xNor[i].Dot(xTan[i]);// BLAS.dot(xNor[i], xTan[i]);
		//			pp = xNor[i].Dot(xTan[i + 1]);//BLAS.dot(xNor[i], xTan[i + 1]);

		//			for (gm = g0 = gp = 0, ix = 0; ix < 3; ix++)
		//			{
		//				//midpoint curvature vector
		//				d = xTan[i + 1][ix] - xTan[i][ix];

		//				//midpoint tangent and curavture variantion
		//				d0 = (xNor[i][ix] - p0 * xTan[i][ix]) / xLen[i];
		//				dp = (xNor[i][ix] - pp * xTan[i + 1][ix]) / xLen[i + 1];

		//				//bottom, mid and top point gradients
		//				gm += d0 * xNor[i - 1][ix];
		//				g0 += (-d0 - dp) * xNor[i][ix] + d * dxNor[i][ix];
		//				gp += dp * xNor[i + 1][ix];
		//			}
		//			A[i, i - 1] = gm;
		//			A[i, i] = g0;
		//			A[i, i + 1] = gp;
		//			sNor[i] = -p0 + pp;
		//		}

		//		if (g.FitPoints[1] is SlidePoint)//slide endpoint
		//		{
		//			p0 = xNor[i].Dot(xTan[i]);

		//			for (gm = g0 = 0, ix = 0; ix < 3; ix++)
		//			{
		//				//midpoint tangent and curavture variantion
		//				d0 = (xNor[i][ix] - p0 * xTan[i][ix]) / xLen[i];

		//				//bottom, mid and top point gradients
		//				gm += d0 * xNor[i - 1][ix];
		//				g0 += -d0 * xNor[i][ix] - xTan[i][ix] * dxNor[i][ix];
		//			}
		//			A[i, i - 1] = gm;
		//			A[i, i] = g0;
		//			sNor[i] = -p0;
		//		}
		//		else//fixed endpoint
		//		{
		//			A[i, i] = 1;
		//			sNor[i] = 0;
		//		}

		//		LU decomp = A.LU();
		//		Vector x = (Vector)decomp.Solve(sNor);

		//		double Reduce = Math.Min(1, .05 / x.AbsoluteMaximum());

		//		for (i = 0; i < 2; i++)//increment slide position
		//			if (g.FitPoints[i] is SlidePoint)
		//				(g.FitPoints[i] as SlidePoint).SCurve -= x[i * INC] * Reduce;

		//		for (i = 1; i < NumFits; i++)//increment uv points
		//		{
		//			uFits[i][0] -= x[i] * uNor[i][0] * Reduce;
		//			uFits[i][1] -= x[i] * uNor[i][1] * Reduce;
		//		}

		//		if (nNwt < 5)
		//		{
		//			//keep initial (s)-increments within bounds
		//			for (i = 0; i < 2; i++)
		//				if (g.FitPoints[i] is SlidePoint)
		//					(g.FitPoints[i] as SlidePoint).SCurve = Utilities.LimitRange(0, (g.FitPoints[i] as SlidePoint).SCurve, 1);

		//			//	keep initial (u)-increments within bounds
		//			for (i = 1; i < NumFits - 1; i++)
		//			{
		//				uFits[i][0] = Utilities.LimitRange(0, uFits[i][0], 1);
		//				uFits[i][1] = Utilities.LimitRange(-.125, uFits[i][1], 1.125);
		//			}
		//		}

		//		if (Conver = (x.AbsoluteMaximum() < 1e-8 && sNor.AbsoluteMaximum() < 1e-8))
		//			break;
		//	}

		//	if (!Conver)
		//		return false;

		//	g.Length = xLen[0];//store length

		//	//calculate unit length (s)-parameter values
		//	sFits[0] = 0;
		//	for (i = 1; i < NumFits; i++)
		//		sFits[i] = sFits[i - 1] + xLen[i] / xLen[0];
		//	//g.m_uvs = uFits;
		//	g.ReSpline(sFits, uFits);
		//	return true;
		//}

		//private bool Geo3(IFitPoint[] fits)
		//{
		//	//if (fits[0] is SlidePoint)
		//	//{
		//	//	//single slidepoint only
		//	//	if( fits[1] is SlidePoint || fits[2] is SlidePoint )
		//	//		return false;

		//		double[] spos = null;
		//		Vect2[] upos = null;
		//		//span a geo between the first 2 points
		//		if (!GeoSegment(this, fits[0], fits[1], ref spos, ref upos))
		//			return false;

		//		Vect3 xyz = new Vect3(), xprev = new Vect3();
		//		xVal(upos[0], ref xprev);//initialize xprev for s paramaterization
		//		double[] sPos = new double[spos.Length + fits.Length - 2]; sPos[0] = 0;
		//		Vect2[] uPos = new Vect2[spos.Length + fits.Length - 2];
		//		//add extra fit points to the fitting array
		//		for (int i = 0; i < sPos.Length; i++)
		//		{				
		//			//get the uv pos
		//			if (i < upos.Length)
		//				uPos[i] = upos[i];
		//			else
		//				uPos[i] = fits[i - upos.Length + 2].UV;//additional fit points past the first 2

		//			//accumulate length
		//			xVal(uPos[i], ref xyz);
		//			if( i > 0 )
		//				sPos[i] = sPos[i - 1] + xyz.Distance(xprev);
		//			xprev.Set(xyz);//store previous for next segment length
		//		}
		//		//reparamaterize
		//		for (int i = 0; i < sPos.Length; i++)
		//			sPos[i] /= sPos.Last();

		//		fits[2][0] = 1;//set fitpoint spos
		//		//spline to the 3rd
		//		ReSpline(sPos, uPos);
		//		return true;
		//	//}
		//	//return false;
		//}

		//private bool Geo4(IFitPoint[] fits)
		//{
		//	if (fits.Length != 4)
		//		return false;

		//	double[] spos = null;
		//	Vect2[] upos = null;
		//	//span a geo between the first 2 points
		//	if (!GeoSegment(this, fits[0], fits[1], ref spos, ref upos))
		//		return false;

		//	double[] spo2 = null;
		//	Vect2[] upo2 = null;
		//	//span a geo between the last 2 points
		//	if (!GeoSegment(this, fits[2], fits[3], ref spo2, ref upo2))
		//		return false;

		//	Vect3 xyz = new Vect3(), xprev = new Vect3();
		//	xVal(upos[0], ref xprev);//initialize xprev for s paramaterization
		//	double[] sPos = new double[spos.Length + spo2.Length]; sPos[0] = 0;
		//	Vect2[] uPos = new Vect2[sPos.Length];
		//	//add extra fit points to the fitting array
		//	for (int i = 0; i < sPos.Length; i++)
		//	{				
		//		//get the uv pos
		//		if (i < upos.Length)
		//			uPos[i] = upos[i];
		//		else
		//			uPos[i] = upo2[i - upos.Length];

		//		//accumulate length
		//		xVal(uPos[i], ref xyz);
		//		if( i > 0 )
		//			sPos[i] = sPos[i - 1] + xyz.Distance(xprev);
		//		xprev.Set(xyz);//store previous for next segment length
		//	}
		//	//reparamaterize
		//	for (int i = 0; i < sPos.Length; i++)
		//		sPos[i] /= sPos.Last();

		//	fits[0][0] = 0;
		//	fits[1][0] = sPos[spos.Length - 1];
		//	fits[2][0] = sPos[spos.Length];
		//	fits[3][0] = 1;
		//	//spline the combined points
		//	ReSpline(sPos, uPos);
		//	return true;

		//}
	}
}
