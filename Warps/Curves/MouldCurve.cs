using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace Warps
{
	public delegate Point3D Transformer(Point3D pt);

	abstract public class MouldCurve : IRebuild, IMouldCurve
	{
		public MouldCurve()
		{ }
		public MouldCurve(string label, Sail sail)
		{
			m_sail = sail;
			m_label = label;
		}

		#region Members

		string m_label;
		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

		Sail m_sail;
		public Sail Sail
		{
			get { return m_sail; }
			internal set { m_sail = value; }
		}
		public ISurface Surface
		{
			get { return m_sail.Mould; }
			//internal set { m_surf = value; }
		}
		public IGroup Group
		{
			get { return Sail.FindGroup(this); }
		}

		BSpline m_bSpline = new BSpline(2);
		internal BSpline Spline
		{
			get { return m_bSpline; }
			set { m_bSpline = value; }
		}

		double m_Length = -1;
		public double Length
		{
			get
			{
				if (m_Length <= 0 && AllFitPointsValid())
				{
					m_Length = 0;
					double s;
					Vect2 uv = new Vect2();
					Vect3 x0 = new Vect3(), xi = new Vect3();
					xVal(0, ref uv, ref x0);
					int CNT = 100;
					for (int i = 1; i < CNT; i++)
					{
						s = BLAS.interpolant(i, CNT);
						xVal(s, ref uv, ref xi);
						m_Length += xi.Distance(x0);
						x0.Set(xi);
					}
				}
				return m_Length;
			}
			set { m_Length = value; }
		}

		IFitPoint[] m_fits;
		public IFitPoint[] FitPoints
		{
			get { return m_fits; }
			set { m_fits = value; }
		}
		public Vect2[] uFits
		{
			get
			{
				Vect2[] u = new Vect2[FitPoints.Length];
				int i = 0;
				foreach (IFitPoint fp in FitPoints)
					u[i++] = new Vect2(fp.UV);
				return u;
			}
		}
		public Vect3[] xFits
		{
			get
			{
				Vect3[] x = new Vect3[FitPoints.Length];
				int i = 0;
				foreach (IFitPoint fp in FitPoints)
				{
					x[i] = new Vect3();
					xVal(fp.UV, ref x[i]);
					i++;
				}
				return x;
			}
		}
		public IFitPoint this[int i]
		{
			get { return FitPoints[i]; }
		}

		public override string ToString()
		{
			return Label;
		}

		#endregion

		#region Fitting

		public void ReFit()
		{
			Fit(FitPoints);
		}
		public void Fit(Vect2 uStart, Vect2 uEnd)
		{
			Fit(new IFitPoint[] { new FixedPoint(uStart), new FixedPoint(uEnd) });
		}
		public abstract void Fit(IFitPoint[] points);
		public void ReSpline(double[] sFits, Vect2[] uFits)
		{
			double[][] u = new double[2][];
			u[0] = new double[uFits.Length];
			u[1] = new double[uFits.Length];
			int i = 0;
			foreach (Vect2 v in uFits)
			{
				u[0][i] = v[0];
				u[1][i] = v[1];
				i++;
			}
			ReSpline(sFits, u);
			//store arrays for debugging
			m_uSplines = new Vect2[uFits.Length];
			uFits.CopyTo(m_uSplines, 0);
		}
		public void ReSpline(double[] sFits, double[][] uFits)
		{
			Length = -1;//reset length
			Spline.Fit(sFits, uFits);//fit spline
		}

		Vect2[] m_uSplines;
		/// <summary>
		/// Debugging array, returns the most recently splined uv points
		/// </summary>
		public Vect2[] uSplines
		{
			get { return m_uSplines; }
			set { m_uSplines = value; }
		}
		/// <summary>
		/// Debugging array, returns the most recently splined xyz points
		/// </summary>
		public Vect3[] xSplines
		{
			get
			{
				if (m_uSplines == null || m_uSplines.Length == 0)
					return null;
				Vect3[] x = new Vect3[m_uSplines.Length];
				for (int i = 0; i < m_uSplines.Length; i++)
				{
					x[i] = new Vect3();
					xVal(m_uSplines[i], ref x[i]);
				}
				return x;
			}
		}

		#endregion

		#region Evaluate

		public virtual void uVal(double s, ref Vect2 uv)
		{
			Spline.BsVal(s, ref uv.m_vec);
		}
		public virtual void uVec(double s, ref Vect2 uv, ref Vect2 du)
		{
			Spline.BsVec(s, ref uv.m_vec, ref du.m_vec);
		}
		public virtual void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu)
		{
			Spline.BsCvt(s, ref uv.m_vec, ref du.m_vec, ref ddu.m_vec);
		}
		public virtual void uNor(double s, ref Vect2 uv, ref Vect2 un)
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

		public virtual void xVal(double s, ref Vect2 uv, ref Vect3 xyz)
		{
			uVal(s, ref uv);
			xVal(uv, ref xyz);
		}
		public virtual void xVal(Vect2 uv, ref Vect3 xyz)
		{
			Surface.xVal(uv, ref xyz);
		}

		public virtual void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx)
		{

			Vect2 du = new Vect2();
			Vect3 dxu = new Vect3(), dxv = new Vect3();

			uVec(s, ref uv, ref du);
			Surface.xVec(uv, ref xyz, ref dxu, ref dxv);

			//dx = dxu * du[0] + dxv * du[1];
			for (int ix = 0; ix < 3; ix++)
				dx[ix] = dxu[ix] * du[0] + dxv[ix] * du[1];

		}
		public virtual void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx)
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

		public virtual void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k)
		{
			Vect3 dx = new Vect3(), ddx = new Vect3();

			xCvt(s, ref uv, ref xyz, ref dx, ref ddx);

			Vect3 cross = dx.Cross(ddx);
			k = cross.Magnitude / Math.Pow(dx.Magnitude, 3);
		}
		public virtual void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn)
		{
			xVec(s, ref uv, ref xyz, ref dx);
			Vect3 xNor = new Vect3();
			Surface.xNor(uv, ref xyz, ref xNor);
			xn = dx.Cross(xNor);
			xn.Magnitude = 1;//unitize
		}

		public bool CrossPoint(IMouldCurve otherCurve, ref Vect2 uv, ref Vect3 xyz, ref Vect2 sPos)
		{
			return CrossPoint(otherCurve, ref uv, ref xyz, ref sPos, 101);
		}
		public bool CrossPoint(IMouldCurve otherCurve, ref Vect2 uv, ref Vect3 xyz,  ref Vect2 sPos, int nRez)
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
					otherCurve.uVec(i, ref uv1, ref duv1); // evaluate s's to get distance between

					this.uVec(j, ref uv2, ref duv2); // evaluate s's to get distance between

					length = uv1.Distance(uv2); // get distance

					if (length < TOLERANCE)
					{ // intersection found
						this.xVal(j, ref uv, ref xyz);
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
				xVal(BLAS.interpolant(ia100, nRez), ref tmpU, ref tmpX);
				uas.Add(new Vect2(tmpU));
				xas.Add(new Vect3(tmpX));

				otherCurve.xVal(BLAS.interpolant(ia100, nRez), ref tmpU, ref tmpX);
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

				uVec(ta, ref ua, ref da);
				xVal(ta, ref ua, ref xa);

				otherCurve.uVec(tb, ref ub, ref db);
				otherCurve.xVal(tb, ref ub, ref xb);

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
					uVec(ta, ref ua, ref da);
					uv = new Vect2(ua);
					xVal(uv, ref xyz);
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

		#endregion

		#region Closest
		public bool uClosest(ref double s, ref Vect2 uvTarget, ref double dist, double tol)
		{
			Vect2 u = new Vect2(uvTarget);
			Vect2 du = new Vect2();
			Vect2 ddu = new Vect2();

			Vect2 h = new Vect2();
			Vect2 e = new Vect2();
			double dedx;

			double s0 = s;
			int loop = 0, max_loops = 100;
			while (loop++ < max_loops)
			{
				uCvt(s, ref u, ref du, ref ddu);

				h = u - uvTarget;
				dist = h.Magnitude;

				e[0] = s;
				e[1] = h.Dot(du); // error, dot product is 0 at pi/2

				if (Math.Abs(e[1]) < tol) // error is less than the tolerance
				{
					uvTarget.Set(u);// return point to caller
					return true;
				}

				dedx = du.Norm + h.Dot(ddu);
				//dedx = BLAS.dot(dx, dx) + BLAS.dot(h, ddx);

				// calculate a new s
				s = e[0] - e[1] / dedx;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}
			s = s0;
			return false;

		}

		public bool xClosest(ref double s, ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol, bool bUseGuess)
		{
			return CurveTools.xClosest(this, ref s, ref uv, ref xyzTarget, ref dist, tol, bUseGuess);
		}
		//public bool xClosest(ref double s, ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol)
		//{
		//	Vect3 x = new Vect3(xyzTarget);
		//	Vect3 dx = new Vect3(), ddx = new Vect3();

		//	Vect3 h = new Vect3();
		//	Vect2 e = new Vect2();
		//	double deds;

		//	//try at each 18th point to ensure global solution
		//	double s0 = s;
		//	double[] sGuesses = new double[] { 0, .125, .25, .375, .5, .625, .75, .875, 1 };
		//	double sCur = -1, hCur = 1e9;
		//	for (int nGuess = 0; nGuess < sGuesses.Length; nGuess++)
		//	{
		//		s = sGuesses[nGuess];//starting guess
		//		int loop = 0, max_loops = 100;
		//		while (loop++ < max_loops)
		//		{
		//			xCvt(s, ref uv, ref x, ref dx, ref ddx);

		//			h = x - xyzTarget;
		//			dist = h.Magnitude;

		//			e[0] = s;
		//			e[1] = h.Dot(dx); // error, dot product is 0 at pi/2

		//			if (Math.Abs(e[1]) < tol) // error is less than the tolerance
		//			{
		//				if (dist < hCur)//store best result
		//				{
		//					sCur = s;
		//					hCur = dist;
		//					break;
		//				}
		//				//xyzTarget.Set(x);// return point to caller
		//				//return true;
		//			}

		//			deds = dx.Norm + h.Dot(ddx);
		//			deds = e[1] / deds;
		//			// calculate a new s (enforce maximum increment)
		//			deds = 0.1 > Math.Abs(deds) ? deds : 0.1 * Math.Sign(deds);
		//			s = e[0] - deds;
		//			//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
		//		}
		//	}
		//	if (sCur != -1) //if successful return parameters to caller
		//	{
		//		xVal(sCur, ref uv, ref xyzTarget);
		//		dist = hCur;
		//		s = sCur;
		//		return true;
		//	}
		//	//s = s0;
		//	return false;
		//}
		#endregion

		/// <summary>
		/// Check to see that all fitpoints in the curve are valid
		/// </summary>
		/// <returns>True if valid, otherwise false</returns>
		internal bool AllFitPointsValid()
		{
			bool valid = true;
			foreach (IFitPoint pnt in FitPoints)
				valid &= pnt.ValidFitPoint;

			return valid;
		}

		//public Point3D[] GetPathPoints(IMouldCurve c, int CNT)
		//{
		//	double s;
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3();
		//	Point3D[] d = new Point3D[CNT];
		//	for (int i = 0; i < CNT; i++)
		//	{
		//		s = (double)i / (double)(CNT - 1);
		//		c.xVal(s, ref uv, ref xyz);
		//		Utilities.Vect3ToPoint3D(ref d[i], xyz);
		//	}
		//	return d;
		//}
		//public Point3D[] GetCloudPoints(IMouldCurve c)
		//{
		//	Point3D[] pnts;
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3();

		//	pnts = new Point3D[c.NumFits];
		//	double s;
		//	for (int i = 0; i < c.NumFits; i++)
		//	{
		//		s = c.GetFitPos(i);
		//		c.xVal(s, ref uv, ref xyz);
		//		Utilities.Vect3ToPoint3D(ref pnts[i], xyz);
		//	}
		//	return pnts;
		//}
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

		#region IRebuild Members

		public bool Affected(List<IRebuild> connected)
		{
			foreach (IFitPoint fp in FitPoints)
				if (fp.Affected(connected))
				{
					//fp.Update(
					return true;
				}

			return false;
		}

		public void GetConnected(List<IRebuild> connected)
		{
			foreach (IFitPoint fp in FitPoints)
				if (fp.Affected(connected))
				{
					connected.Add(this);
					break;
				}
			
		}

		public bool Delete() { return false; }

		public bool Update(Sail s)
		{
			foreach (IFitPoint fp in FitPoints)
				fp.Update(s);

			if (AllFitPointsValid())
				ReFit();
			return AllFitPointsValid();
		}

		public virtual bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt == null || txt.Count == 0)
				return false;

			List<IFitPoint> fits = new List<IFitPoint>();
			string[] splits = txt[0].Split(':');
			Label = "";
			if (splits.Length > 0)//extract label
				Label = splits[1];
			if (splits.Length > 1)//incase label contains ":"
				for (int i = 2; i < splits.Length; i++)
					Label += ":" + splits[i];
			Label = Label.Trim();

			for (int nLine = 1; nLine < txt.Count; )
			{
				IList<string> lines = ScriptTools.Block(ref nLine, txt);
				//nLine += lines.Count;

				object cur = null;
				splits = lines[0].Split(':');
				if (splits.Length > 0)
					cur = Utilities.CreateInstance(splits[0].Trim('\t'));
				if (cur != null && cur is IFitPoint)
				{
					(cur as IFitPoint).ReadScript(Sail, lines);
					fits.Add(cur as IFitPoint);
				}
			}
			FitPoints = fits.ToArray();

			if (AllFitPointsValid())
				ReFit();

			return true;
		}

		public virtual List<string> WriteScript()
		{
			List<string> script = new List<string>();
			script.Add(GetType().Name + ": " + Label);
			foreach (IFitPoint fp in FitPoints)
			{
				foreach (string s in fp.WriteScript())
					script.Add("\t" + s);
			}
			
			return script;
		}

		#endregion

		#region Forms

		//public IList<string> Script
		//{
		//	get
		//	{
		//		List<string> script = new List<string>();
		//		script.Add(GetType().Name + ": " + Label);
		//		foreach (IFitPoint fp in FitPoints)
		//		{
		//			foreach (string s in fp.Script)
		//				script.Add("\t" + s);
		//		}
		//		return script;
		//	}
		//	set
		//	{
		//		List<IFitPoint> fits = new List<IFitPoint>();
		//		IList<string> txt = value;

		//		if (txt == null || txt.Count == 0)
		//			return;
		//		string[] splits = txt[0].Split(':');
		//		Label = "";
		//		if (splits.Length > 0)//extract label
		//			Label = splits[1];
		//		if (splits.Length > 1)//incase label contains ":"
		//			for (int i = 2; i < splits.Length; i++)
		//				Label += ":" + splits[i];
		//		Label = Label.Trim();

		//		for (int nLine = 1; nLine < txt.Count;)
		//		{
		//			IList<string> lines = ScriptTools.Block(ref nLine, txt);
		//			//nLine += lines.Count;

		//			object cur = null;
		//			splits = lines[0].Split(':');
		//			if (splits.Length > 0)
		//				cur = Utilities.CreateInstance(splits[0].Trim('\t'));
		//			if (cur != null && cur is IFitPoint)
		//			{
		//				(cur as IFitPoint).Script = lines;
		//				fits.Add(cur as IFitPoint);
		//			}
		//		}
		//		FitPoints = fits.ToArray();
		//		ReFit();
		//		//Fit(fits.ToArray());
		//	}
		//}

		System.Windows.Forms.TreeNode m_node = null;

		public virtual TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = string.Format("{0}: {1} [{2:0.000}]", GetType().Name, Label, Length);
			m_node.Tag = this;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.Nodes.Clear();
			foreach (IFitPoint fp in FitPoints)
				m_node.Nodes.Add(fp.Node);
			return m_node;
		}

		public virtual CurveEditor UpdateEditor(CurveEditor edit)
		{
			if (edit == null)
				edit = new CurveEditor();
			edit.Tag = this;
			edit.Label = Label;
			edit.Length = Length;
			edit.FitPoints = FitPoints;
			return edit;
		}
		public virtual void ReadEditor(CurveEditor edit)
		{
			for (int i = 0; i < FitPoints.Length; i++)
				FitPoints[i].ReadEditor(edit[i]);

		}

		public virtual Entity[] CreateEntities()
		{
			double[] sPos;
			return CreateEntities(false, Math.PI/180.0, out sPos).ToArray();
		}

		public virtual Entity[] CreateEntities(bool bFitPoints)
		{
			double[] sPos;
			return CreateEntities(bFitPoints, Math.PI / 180.0, out sPos).ToArray();
		}
		public virtual List<Entity> CreateEntities(bool bFitPoints, double TolAngle, out double[] sPos)
		{
			if (!AllFitPointsValid())
			{
				sPos = null;
				return null;
			}

			List<double> spos = new List<double>();
			const int TEST = 8;
			int FitLength = FitPoints.Length;
			double[] stest = new double[(FitLength - 1) * TEST];
			Vect3[] xtest = new Vect3[(FitLength - 1) * TEST];
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Vect3 dxp = new Vect3(), dxm = new Vect3();
			//initial 8 subdivisions per segment
			int nFit, nTest = 0;
			for (nFit = 1; nFit < FitLength; nFit++)
			{
				for (int i = 0; i < TEST; i++, nTest++)
				{
					stest[nTest] = BLAS.interpolate(i, TEST, FitPoints[nFit].S, FitPoints[nFit - 1].S);
					xtest[nTest] = new Vect3();
					xVal(stest[nTest], ref uv, ref xtest[nTest]);
				}
			}

			//test the midpoint of each subsegment to determine required # of points
			int[] nAdd = new int[stest.Length];
			double cosA;
			double smid;
			int nTotal = FitLength;
			for (nTest = 1; nTest < stest.Length; nTest++)
			{
				//midpoint position
				smid = (stest[nTest] + stest[nTest - 1]) / 2.0;
				xVal(smid, ref uv, ref xyz);
				//forward and backward tangents
				dxp = xtest[nTest] - xyz;
				dxm = xtest[nTest - 1] - xyz;
				//change in angle between for and aft tans
				cosA = -(dxp.Dot(dxm)) / (dxp.Magnitude * dxm.Magnitude);
				Utilities.LimitRange(-1, ref cosA, 1);
				cosA = Math.Acos(cosA);
				//determine additional points and sum total
				nTotal += nAdd[nTest] = (int)(cosA / TolAngle + 1);
			}

			m_Length = 0;
			Vect3 xprev = new Vect3();
			Vect3 dx = new Vect3();
			List<Point3D> pnts = new List<Point3D>();
			List<Vect3> tans = new List<Vect3>();
			xVal(stest[0], ref uv, ref xprev);
			pnts.Add(new Point3D(xprev.ToArray()));
			spos.Add(stest[0]);
			for (nTest = 1; nTest < stest.Length; nTest++)
			{
				for (int i = 1; i <= nAdd[nTest]; i++)
				{
					smid = ((nAdd[nTest] - i) * stest[nTest - 1] + i * stest[nTest]) / nAdd[nTest];
					xVec(smid, ref uv, ref xyz, ref dx);
					spos.Add(smid);
					pnts.Add(new Point3D(xyz.ToArray()));
					tans.Add(new Vect3(dx));
					m_Length += xyz.Distance(xprev);
					xprev = xyz;
				}
			}


#if DEBUG
			//add for-cast/back-cast points
			for (int i = 0; i < 2; i++)
			{
				for (nTest = 0; nTest < 10; nTest++)
				{
					smid = BLAS.interpolant(nTest, 10) * 0.1;//scale down to .3 cast
					if (i == 0)
						smid = -smid;
					else
						smid += 1.0;

					xVal(smid, ref uv, ref xyz);
					if (i == 0)
						pnts.Insert(0, new Point3D(xyz.ToArray()));
					else
						pnts.Add(new Point3D(xyz.ToArray()));
				}
			}
#endif
			LinearPath lp = new LinearPath(pnts);
			lp.EntityData = this;
			//lp.LineWeight = 3.0f;
			//lp.LineWeightMethod = colorMethodType.byEntity;
			List<Entity> tanpaths = new List<Entity>();
			tanpaths.Add(lp);
			if (bFitPoints)
			{
				//LinearPath path;
				//int npnt = 0;
				//foreach (Vect3 pnt in tans)
				//{
				//	pnt.Magnitude = 2;
				//	xyz = new Vect3(pnts[npnt].ToArray());
				//	xyz += pnt;
				//	path = new LinearPath(pnts[npnt], new Point3D(xyz.ToArray()));
				//	path.EntityData = this;
				//	tanpaths.Add(path);
				//	npnt++;
				//	//xVal(pnt.UV, ref xyz);
				//	//pnts.Add(new Point3D(xyz.ToArray()));
				//}
				pnts = new List<Point3D>();
				foreach (IFitPoint pnt in FitPoints)
				{
					xVal(pnt.UV, ref xyz);
					pnts.Add(new Point3D(xyz.ToArray()));
				}
				PointCloud pc = new PointCloud(pnts, 8f);
				pc.EntityData = this;
				tanpaths.Add(pc);
			}
			sPos = spos.ToArray();
			return tanpaths;
		}

		public virtual devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> ret = new List<devDept.Eyeshot.Labels.Label>();
				ret.Add(new devDept.Eyeshot.Labels.OutlinedText(GetLabelPoint3D(0.5), Label,
					new Font("Helvectiva", 8.0f), Color.White, Color.Black, ContentAlignment.MiddleCenter));
				return ret.ToArray();
			}
		}

		public Point3D GetLabelPoint3D(double s)
		{
			Vect3 xyz = new Vect3();
			Vect2 uv = new Vect2();
			xVal(s, ref uv, ref xyz);
			return new Point3D(xyz.x, xyz.y, xyz.z);
		}

		/// <summary>
		/// returns CNT evenly spaced points, inferior to CreateEntity
		/// </summary>
		/// <param name="CNT">the number of points desired on the line</param>
		/// <returns>the array of points</returns>
		public Point3D[] GetPathPoints(int CNT)
		{
			double s;
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Point3D[] d = new Point3D[CNT];
			for (int i = 0; i < CNT; i++)
			{
				s = (double)i / (double)(CNT - 1);
				xVal(s, ref uv, ref xyz);
				d[i] = new Point3D();
				Utilities.Vect3ToPoint3D(ref d[i], xyz);
			}
			return d;
		}

		#endregion

		#region Dragging

		public bool DragPoint(int index, PointF mouse, Transformer WorldToScreen)
		{
			if (FitPoints[index] is CurvePoint)
				return SlidePoint(index, mouse, WorldToScreen);
			else if (FitPoints[index] is SlidePoint)
				return false;
			Vect2 uv0 = FitPoints[index].UV;
			Vect3 xyz = new Vect3();

			// get the mouse-coord x for the original position
			xVal(uv0, ref xyz);
			Point3D x0 = WorldToScreen(new Point3D(xyz.Array));

			// small offsets
			const double delta_u = .05;
			Vect2 udu = new Vect2(uv0[0] + delta_u, uv0[1]);
			Vect2 udv = new Vect2(uv0[0], uv0[1] + delta_u);

			// get the mouse-coord x for the small offsets
			xVal(udu, ref xyz);
			Point3D xdu = WorldToScreen(new Point3D(xyz.Array));
			xVal(udv, ref xyz);
			Point3D xdv = WorldToScreen(new Point3D(xyz.Array));

			// subtract offsets from initial point to get delta xy from delta u
			Vect2 dx = new Vect2(xdu.X - x0.X, xdu.Y - x0.Y);
			double dxdu = (double)dx.u / delta_u; // divide delta x by delta u to get d(x)/d(u)
			double dydu = (double)dx.v / delta_u;

			dx = new Vect2(xdv.X - x0.X, xdv.Y - x0.Y);
			double dxdv = (double)dx.u / delta_u;
			double dydv = (double)dx.v / delta_u;

			dx = new Vect2(mouse.X - x0.X, mouse.Y - x0.Y);
			//	[ dxdv  dxdu ][delta v]   [delta x]
			//	[ dydv  dydu ][delta u] = [delta y]  solve for delta v, delta u

			double det = dxdv * dydu - dydv * dxdu;
			if (det == 0)
				return false; // if the determinant is 0 quit

			// solve the system for delta u
			Vect2 du = new Vect2();
			//C2dPoint du;
			du[0] = (dxdv * (double)dx.v - dydv * (double)dx.u) / det;
			du[1] = (dydu * (double)dx.u - dxdu * (double)dx.v) / det;

			for (int i = 0; i < 2; i++)
			{
				FitPoints[index][i + 1] = Utilities.LimitRange(0, FitPoints[index][i + 1] + du[i], 1);
				//m_uFits[i][index] += du[i];
				////ensure inbounds
				//m_uFits[i][index] = Math.Max(0, m_uFits[i][index]);
				//m_uFits[i][index] = Math.Min(1, m_uFits[i][index]);
			}

			//ReFit();
			return true;
		}

		public bool SlidePoint(int index, PointF mouse, Transformer WorldToScreen)
		{
			//// get the mouse-coord x for the originally clicked fitpoint position
			Vect2 uv = new Vect2();
			Vect3 xyz0 = new Vect3();
			//xVal(FitPoints[index].UV, ref xyz0);
			//Point3D m0 = WorldToScreen(new Point3D(xyz0.Array));

			//get the mouse-coord for the starting fixed point
			CurvePoint warp = FitPoints[index] as CurvePoint;
			warp.m_curve.xVal(warp.sCurve, ref uv, ref xyz0);
			Vect3 xyz = new Vect3(xyz0);
			Point3D x0 = WorldToScreen(new Point3D(xyz0.Array));

			// small offset
			double delta_s = .005;
			//			for (; xyz.Distance(xyz0) < 1e-4 && delta_s < 1; delta_s +=.05 )//ensure nonzero tangent
			//			{
			warp.m_curve.xVal(warp.sCurve + delta_s, ref uv, ref xyz);

			//			}
			//convert to mouse coords
			Point3D pnt = WorldToScreen(new Point3D(xyz.ToArray()));
			Vect2 dmds = new Vect2(pnt.X - x0.X, pnt.Y - x0.Y);
			//dmds -= new Vect2(x0.X, x0.Y);//get deltaxy/deltas

			Vect2 dm = new Vect2(mouse);
			dm -= new Vect2(x0.X, x0.Y);//get delta mouse

			double reduce = Math.Max(dm.Magnitude, dmds.Magnitude);
			double dot = dmds.Dot(dm);//mouse.X * dmds.X + mouse.Y * dmds.Y;
			dot *= delta_s / reduce;// / dmds.Magnitude;
			//	Utilities.LimitRange(-.005, ref dot, .005);
			if (warp.S_Equ.IsNumber())
			{
				warp.S_Equ.Value = dot + warp.S_Equ.Result;
				warp.S_Equ.Value = Utilities.LimitRange(-.2, dot + warp.S_Equ.Result, 1.2);
				//warp.m_sCurve = Utilities.LimitRange(-.2, warp.m_sCurve + dot, 1.2);
				//ReFit();
			}
			return true;
		}

		public virtual bool InsertPoint(PointF mouse, Transformer WorldToScreen, out int nIndex)
		{
			double s = 0.5, h = 0;
			Vect2 uv = new Vect2();

			//find the closest point to the mouse click
			if (!mClosest(ref s, ref mouse, ref uv, ref h, WorldToScreen, 1e-3))//failed to find point to insert
			{
				nIndex = -1;
				return false;
			}

			//find bracketing fitpoints
			int i;
			for (i = 0; i < FitPoints.Length; i++)
			{
				if (FitPoints[i].S > s)
					break;
			}

			//uVal(s, ref uv);
			//create a new fixed point for insertion
			FixedPoint fp = new FixedPoint(s, uv);
			List<IFitPoint> fits = new List<IFitPoint>(FitPoints.Length + 1);
			//copy the existing fitpoint array
			fits.AddRange(FitPoints);
			//insert the new point
			fits.Insert(i, fp);
			//store the new fitpoint array for refitting
			FitPoints = fits.ToArray();
			nIndex = i;
			return true;
		}
		public bool mClosest(ref double s, ref PointF mTarget, ref Vect2 u, ref double dist, Transformer wts, double tol)
		{
			Point3D mouse = new Point3D(mTarget.X, mTarget.Y);

			Point3D mx;
			Point3D dmds;

			//Vect2 u = new Vect2();
			Vect3 x = new Vect3();
			Vect3 dx = new Vect3();
			Vect3 ddx = new Vect3();

			Point3D h = new Point3D();
			Vect2 e = new Vect2();
			double deds;

			double s0 = s;
			double[] sGuesses = new double[] { 0, .125, .25, .375, .5, .625, .75, .875, 1 };
			double sCur = -1, hCur = 1e9;
			PointF mCur = new PointF();
			for (int nGuess = 0; nGuess < sGuesses.Length; nGuess++)
			{
				s = sGuesses[nGuess];//starting guess
				int loop = 0, max_loops = 100;
				while (loop++ < max_loops)
				{
					xCvt(s, ref u, ref x, ref dx, ref ddx);
					//calc mouse coords of guess point
					mx = wts(new Point3D(x[0], x[1], x[2]));

					//calculate mouse coord distance from guess to target
					h = mx - mouse;
					dist = mx.DistanceTo(mouse);// h.Magnitude;

					//calculate mouse coord derivatives wrt s-pos
					//dx.Unitize();
					dx = x + dx;
					dmds = wts(new Point3D(dx[0], dx[1], dx[2]));
					dmds -= mx;

					e[0] = s;
					e[1] = h.X * dmds.X + h.Y * dmds.Y; // error, dot product is 0 at pi/2

					if (Math.Abs(e[1]) < tol) // error is less than the tolerance
					{
						if (dist < hCur)
						{
							mCur.X = (float)mx.X;
							mCur.Y = (float)mx.Y;
							sCur = s;
							hCur = dist;
						}
						//uvTarget.Set(x);// return point to caller
						break;
					}

					deds = e[1] / (dmds.X * dmds.X + dmds.Y * dmds.Y);
					deds = 0.1 > Math.Abs(deds) ? deds : 0.1 * Math.Sign(deds);
					//dedx = dx.Norm + h.Dot(ddx);
					//dedx = BLAS.dot(dx, dx) + BLAS.dot(h, ddx);

					// calculate a new s
					s = e[0] - deds;
					//s = e[0] - e[1] / dedx;
					//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
				}
			}
			if (sCur != -1) //if successful return parameters to caller
			{
				xVal(sCur, ref u, ref x);
				mTarget = mCur;
				s = sCur;
				dist = hCur;
				return true;
			}

			s = s0;
			return false;

		}

		internal bool InsertPoint(Vect3 target, out int nIndex)
		{
			double s = 0.5, h = 0;
			Vect2 uv = new Vect2();

			//find the closest point to the mouse click
			if (!xClosest(ref s, ref uv, ref target, ref h, 1e-9, false))//failed to find point to insert
			{
				nIndex = -1;
				return false;
			}

			//find bracketing fitpoints
			int i;
			for (i = 0; i < FitPoints.Length; i++)
			{
				if (FitPoints[i].S > s)
					break;
			}

			//create a new fixed point for insertion
			FixedPoint fp = new FixedPoint(s, uv);
			List<IFitPoint> fits = new List<IFitPoint>(FitPoints.Length + 1);
			//copy the existing fitpoint array
			fits.AddRange(FitPoints);
			//insert the new point
			fits.Insert(i, fp);
			//store the new fitpoint array for refitting
			FitPoints = fits.ToArray();
			nIndex = i;
			return true;
		}

		#endregion

		internal bool RemovePoint(int index)
		{
			if (index < 0 || FitPoints.Length <= index || FitPoints.Length <= 2)
				return false;

			List<IFitPoint> fits = FitPoints.ToList();
			fits.RemoveAt(index);
			FitPoints = fits.ToArray();
			return FitPoints.Length > 1;
		}

		internal bool Contains(IFitPoint p)
		{
			return FitPoints.Contains(p);
		}
	}
}
