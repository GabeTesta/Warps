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
		public SlidePoint(MouldCurve curve, double sCurve)
			: this(0, curve, sCurve) { }
		public SlidePoint(double s, MouldCurve curve, double sCurve)
			: base(s, curve, sCurve) { }

		public override IFitPoint Clone()
		{
			return new SlidePoint(this);
		}
		
		public MouldCurve Curve
		{
			get { return m_curve; }
		}

		public double SCurve
		{
			get { return m_sCurve; }
			set 
			{ 
				m_sCurve = value;
				S_Equ.Value = value;
			}
		}
	}
}
