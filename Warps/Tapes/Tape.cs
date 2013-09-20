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
	public class Tape
	{
		public Tape(IMouldCurve cur, int nPix)
		{
			m_centerline = cur;
			m_nPix = nPix;
		}
		IMouldCurve m_centerline;
		int m_nPix = 0;
		double m_width = .2;//20cm tape default

		double Length { get { return m_centerline.Length; } }

		public List<Entity> CreateEntities() { return CreateEntities(false); }
		public List<Entity> CreateEntities(bool bCenterline)
		{
			List<Entity> ents = new List<Entity>();
			if (bCenterline)
			{
				LinearPath lp = new LinearPath(CurveTools.GetEvenPathPoints(m_centerline, 15));
				lp.EntityData = this;
				lp.LineWeight = 3;
				lp.LineWeightMethod = colorMethodType.byEntity;
				lp.Color = ColorMath.IntColor(m_nPix);
				lp.ColorMethod = colorMethodType.byEntity;
				ents.Add(lp);
			}
			ents.Add(CreateTape());
			return ents;
		}
		Mesh CreateTape()
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

				m_centerline.xNor(s, ref uv, ref xyz, ref xm, ref xnor);
				xnor.Magnitude = m_width / 2.0;
				xp = xyz + xnor;
				xm = xyz - xnor;

				pnts[2 * i] = Utilities.Vect3ToPoint3D(xm);
				pnts[2 * i + 1] = Utilities.Vect3ToPoint3D(xp);
			}
			Mesh m = SurfaceTools.GetMesh(pnts, rez);
			m.EntityData = this;
			m.Color = System.Drawing.Color.FromArgb(100, ColorMath.IntColor(m_nPix));
			//m.ColorMethod = colorMethodType.byEntity;
			return m;
		}

		public override string ToString()
		{
			return m_centerline.ToString();
		}
	}
}
