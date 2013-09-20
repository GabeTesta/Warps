using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warps.Curves;

namespace Warps.Panels
{
	[System.Diagnostics.DebuggerDisplay("{Seams[0].Label} {Seams[1].Label} {sPos.ToString(\"f4\")}", Name = "{Seams[0].Label} {Seams[1].Label}", Type = "{GetType()}")]
	public class PanelCorner
	{
		public PanelCorner(PanelCorner cor) : this(cor.Seams[0], cor.Seams[1], cor.sPos, cor.uPos, cor.xPos) { }
		public PanelCorner(IMouldCurve c1, IMouldCurve c2)
		{
			Seams.Add(c1);
			Seams.Add(c2);
			if (!CurveTools.CrossPoint(c1, c2, ref uPos, ref xPos, ref sPos))
				throw new Exception(string.Format("Edge [{0}] does not intersect Edge [{1}]", Seams[0].Label, Seams[1].Label));
		}
		public PanelCorner(IMouldCurve c1, IMouldCurve c2, Vect2 s, Vect2 u, Vect3 x)
		{
			Seams.Add(c1);
			Seams.Add(c2);
			if (s != null)
				sPos.Set(s);
			if (u != null)
				uPos.Set(u);
			if (x != null)
				xPos.Set(x);
		}
		public PanelCorner Clone() { return new PanelCorner(this); }

		public static bool operator ==(PanelCorner a, PanelCorner b)
		{
			if (System.Object.ReferenceEquals(a, null))
				return System.Object.ReferenceEquals(b, null);//a and b are null, thus ==
			return a.Equals(b);
		}
		public static bool operator !=(PanelCorner a, PanelCorner b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is PanelCorner))
				return false;
			PanelCorner b = obj as PanelCorner;
			return b.xPos == xPos//same position, same seams
				&&
				((b.Seams[0] == Seams[0] && b.Seams[1] == Seams[1])
				|| (b.Seams[0] == Seams[1] && b.Seams[1] == Seams[0]));
		}
		public override int GetHashCode()
		{
			return sPos.GetHashCode() ^ Seams.GetHashCode();
		}
		public void Copy(PanelCorner parent)
		{
			Seams.AddRange(parent.Seams);
			sPos.Set(parent.sPos);
			uPos.Set(parent.uPos);
			xPos.Set(parent.xPos);
		}

		public List<IMouldCurve> Seams = new List<IMouldCurve>(2);
		public Vect2 sPos = new Vect2();//seam positions
		public Vect2 uPos = new Vect2();
		public Vect3 xPos = new Vect3();

		public double Distance(PanelCorner c)
		{
			return xPos.Distance(c.xPos);
		}

		public void SlidePos(int nEdge, double s)
		{
			Seams[nEdge].xVal(s, ref uPos, ref xPos);
			sPos[nEdge] = s;//record new values and update cache'd data

			sPos[1 - nEdge] = -1;//nullify other curve's position
		}
		public double GetSeamPos(IMouldCurve seam)
		{
			return sPos[Seams.IndexOf(seam)];
		}

		public CurvePoint GetSeamPoint(int nPnt)
		{
			return new CurvePoint(Seams[nPnt], sPos[nPnt]);
		}

		public void Swap()
		{
			double d;
			d = sPos[0];
			sPos[0] = sPos[1];
			sPos[1] = d;

			IMouldCurve c = Seams[0];
			Seams[0] = Seams[1];
			Seams[1] = c;
		}
	}
}
