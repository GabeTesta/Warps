using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using Warps;
using Warps.Curves;

namespace Warps.Tapes
{
	public class Tape : IMouldCurve
	{
		public Tape(Sail s, int nT, Vect2 uStart, Vect2 uEnd)
		{
			MouldCurve cl = new MouldCurve("CL" + nT.ToString(), s, uStart, uEnd);
			m_centerline = cl;
			m_nPix = nT;
		}
		public Tape(MouldCurve cur, int nPix)
		{
			m_centerline = cur;
			m_nPix = nPix;
		}
		MouldCurve m_centerline;

		public MouldCurve Centerline
		{
			get { return m_centerline; }
		}
		int m_nPix = 0;

		public List<Entity> CreateEntities(bool bCenterline, double width, bool bOutline)
		{
			List<Entity> ents = new List<Entity>();
			if (bCenterline)
			{
				LinearPath lp = new LinearPath(CurveTools.GetEvenPathPoints(Centerline, 15));
				lp.EntityData = this;
				lp.LineWeight = 3;
				lp.LineWeightMethod = colorMethodType.byEntity;
				lp.Color = ColorMath.IntColor(m_nPix);
				lp.ColorMethod = colorMethodType.byEntity;
				ents.Add(lp);
			}
			ents.Add(CreateTape(width, bOutline));
			return ents;
		}
		Entity CreateTape(double width, bool bOutline)
		{
			int rez = 10;
			double s;
			//	double[] uv = new double[2], du = new double[2];
			Vect2 uv = new Vect2(), du = new Vect2();
			Vect2 up = new Vect2(), um = new Vect2();
			Vect3 xp = new Vect3(), xm = new Vect3();
			Vect3 xnor = new Vect3(), xyz = new Vect3();
			Vect2 nor = new Vect2();
			Point3D[] pnts = new Point3D[2 * rez];
			for (int i = 0; i < rez; i++)
			{
				s = BLAS.interpolant(i, rez);
				//cur.uNor(s, ref uv, ref nor);
				//set magnitude equal to 1/2 width
				//nor.Magnitude = WIDTH / 2.0;
				////nor[0] *= WIDTH / (2.0 * s);
				////nor[1] *= WIDTH / (2.0 * s);
				//up = uv + nor;
				//um = uv - nor;// BLAS.subtract(uv, nor);
				//cur.xVal(up, ref xp);
				//cur.xVal(um, ref xm);

				Centerline.xNor(s, ref uv, ref xyz, ref xm, ref xnor);
				xnor.Magnitude = width / 2.0;
				xp = xyz + xnor;
				xm = xyz - xnor;

				pnts[2 * i] = Utilities.Vect3ToPoint3D(xm);
				pnts[2 * i + 1] = Utilities.Vect3ToPoint3D(xp);
			}
			Entity m;
			if (bOutline)
			{
				List<Point3D> outline = new List<Point3D>(pnts.Length);
				int j;
				for (int i = 0; i < pnts.Length; i++)
					if (i < pnts.Length / 2)
						outline.Add(pnts[2 * i]);
					else
					{
						j = pnts.Length - i;
						j *= 2;
						j -= 1;
						outline.Add(pnts[j]);
					}
						//outline.Add(pnts[2 * (i - pnts.Length / 2) + 1]);

				outline.Add(outline[0]);//close the loop
				m = new LinearPath(outline);
			}
			else
			{
				m = SurfaceTools.GetMesh(pnts, rez);
			}
			m.EntityData = this;
			m.Color = System.Drawing.Color.FromArgb(100, ColorMath.IntColor(m_nPix));
			return m;
			//m.ColorMethod = colorMethodType.byEntity;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Label);
		}
		#region IMouldCurve Members

		public string Label
		{
			get { return Centerline.Label; }
		}

		public double Length { get { return Centerline.Length; } }

		public void uVal(double s, ref Vect2 uv)
		{
			Centerline.uVal(s, ref uv);
		}

		public void uVec(double s, ref Vect2 uv, ref Vect2 du)
		{
			Centerline.uVec(s, ref uv, ref du);
		}

		public void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu)
		{
			Centerline.uCvt(s, ref uv, ref du, ref ddu);
		}

		public void uNor(double s, ref Vect2 uv, ref Vect2 un)
		{
			Centerline.uNor(s, ref uv, ref un);
		}

		public void xVal(double s, ref Vect2 uv, ref Vect3 xyz)
		{
			Centerline.xVal(s, ref uv, ref xyz);
		}

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			Centerline.xVal(uv, ref xyz);
		}

		public void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx)
		{
			Centerline.xVec(s, ref uv, ref xyz, ref dx);
		}

		public void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx)
		{
			Centerline.xCvt(s, ref uv, ref xyz, ref dx, ref ddx);
		}

		public void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k)
		{
			Centerline.xRad(s, ref uv, ref xyz, ref k);
		}

		public void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn)
		{
			Centerline.xNor(s, ref uv, ref xyz, ref dx, ref xn);
		}

		#endregion
	}
}
