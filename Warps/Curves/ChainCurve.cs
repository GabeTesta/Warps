using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept;
using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Warps.Curves
{

	[System.Diagnostics.DebuggerDisplay("{m_curves[0]} {m_curves[1]}", Name = "{m_curves[0]} {m_curves.Count}", Type = "{GetType()}")]
	public class ChainCurve : IMouldCurve
	{

		List<SegmentCurve> m_curves = new List<SegmentCurve>();
		List<double> m_P = new List<double>();
		public void Add(MouldCurve c, double lim0, double lim1)
		{
			m_curves.Add(new SegmentCurve(c, lim0, lim1));
			SetSegments();
		}
		void SetSegments()
		{
			double len = 0;
			m_P.Clear();
			m_P.Add(0);//initial value

			for (int nCur = 0; nCur < m_curves.Count; nCur++)
			{
				Point3D[] pts = CurveTools.GetEvenPathPoints(m_curves[nCur], 20);
				//accumulate length along each segment
				for (int i = 1; i < pts.Length; i++)
					len += pts[i - 1].DistanceTo(pts[i]);
				m_P.Add(len);
			}
			//scale to total length
			for (int i = 0; i < m_P.Count; i++)
				m_P[i] /= m_P.Last();
		}

		double Length
		{
			get
			{
				double len = 0;
				for (int nCur = 0; nCur < m_curves.Count; nCur++)
				{
					Point3D[] pts = CurveTools.GetEvenPathPoints(m_curves[nCur], 20);
					//accumulate length along each segment
					for (int i = 1; i < pts.Length; i++)
						len += pts[i - 1].DistanceTo(pts[i]);
				}
				return len;
			}
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities()
		{
			List<devDept.Eyeshot.Entities.Entity> ents = new List<Entity>(m_curves.Count);
			for (int nCur = 0; nCur < m_curves.Count; nCur++)
				ents.Add( new LinearPath(CurveTools.GetEvenPathPoints( m_curves[nCur], 20)));

			List<Point3D> corners = new List<Point3D>(m_P.Count);
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			foreach (double d in m_P)
			{
				xVal(d, ref uv, ref xyz );
				corners.Add(Utilities.Vect3ToPoint3D(xyz));
			}
			ents.Add(new PointCloud(corners));
			ents.Last().LineWeight = 3f;
			ents.Last().LineWeightMethod = colorMethodType.byEntity;

			ents.ForEach(lin => lin.EntityData = this );
			return ents;
		}



		#region IMouldCurve Members

		/// <summary>
		/// convert from [0,1] to [sLim, sLim]
		/// </summary>
		/// <param name="p">the position along this curve [0,1]</param>
		/// <returns>the position on the base curve [sLim, sLim]</returns>
		int SPos(ref double p)
		{
			int nPos;
			if (p <= 0)//bottom extension
				nPos = 0;
			else if (p >= 1)//top extension
				nPos = m_P.Count - 2;
			else//internal segment, find bracket
			{
				for (nPos = 0; nPos < m_P.Count-1; nPos++)//find p-bracket
					if (m_P[nPos] < p && p < m_P[nPos+1]) break;
			}

			if (nPos == m_P.Count)
				return -1;//failed to find

			p= (p - m_P[nPos]) / (m_P[nPos + 1] - m_P[nPos]);
			return nPos;
		}


		public void uVal(double s, ref Vect2 uv)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].uVal(s, ref uv);
		}

		public void uVec(double s, ref Vect2 uv, ref Vect2 du)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].uVec(s, ref uv, ref du);
		}

		public void uCvt(double s, ref Vect2 uv, ref Vect2 du, ref Vect2 ddu)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].uCvt(s, ref uv, ref du, ref ddu);
		}

		public void uNor(double s, ref Vect2 uv, ref Vect2 un)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].uNor(s, ref uv, ref un);
		}

		public void xVal(double s, ref Vect2 uv, ref Vect3 xyz)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].xVal(s, ref uv, ref xyz);
		}

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			m_curves[0].xVal(uv, ref xyz);
		}

		public void xVec(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].xVec(s, ref uv, ref xyz, ref dx);
		}

		public void xCvt(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 ddx)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].xCvt(s, ref uv, ref xyz, ref dx, ref ddx);
		}

		public void xRad(double s, ref Vect2 uv, ref Vect3 xyz, ref double k)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].xRad(s, ref uv, ref xyz, ref k);
		}

		public void xNor(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 dx, ref Vect3 xn)
		{
			int nBrk = SPos(ref s);
			m_curves[nBrk].xNor(s, ref uv, ref xyz, ref dx, ref xn);
		}

		#endregion
	}
	//public class SeamSegment
	//{
	//	public SeamSegment(MouldCurve seam, Vect2 sLimits)
	//	{
	//		m_seam = seam;
	//		m_sLimits = new Vect2(sLimits);
	//	}
	//	public MouldCurve m_seam;
	//	public Vect2 m_sLimits;
	//}
}
