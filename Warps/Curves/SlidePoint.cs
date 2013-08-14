using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	class SlidePoint: CurvePoint
	{
		public SlidePoint() : this(0, null, 0) { }
		public SlidePoint(SlidePoint s)
			: base(s) { }
		//	: this(s.m_sPos, s.m_curve, s.m_sCurve) { }
		public SlidePoint(IMouldCurve curve, double sCurve)
			: this(0, curve, sCurve) { }
		public SlidePoint(double s, IMouldCurve curve, double sCurve)
			: base(s, curve, sCurve) { }

		public override IFitPoint Clone()
		{
			return new SlidePoint(this);
		}
		
	}
}
