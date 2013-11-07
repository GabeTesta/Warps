using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Warps;
using Warps.Curves;

namespace Warps
{
	public class AnglePoint : CurvePoint
	{
		public AnglePoint() : this(0, null, 0) { }
		public AnglePoint(AnglePoint s)
			: base(s) { m_Angle = new Equation(s.m_Angle); }
		//	: this(s.m_sPos, s.m_curve, s.m_sCurve) { }
		public AnglePoint(MouldCurve curve, double angle)
			: this(0, curve, angle) {  }
		public AnglePoint(double s, IMouldCurve curve, double angle)
			: base(s, curve, 0) { m_Angle.Value = angle; }

		Equation m_Angle = new Equation("Angle", 0);

		public override IFitPoint Clone()
		{
			return new AnglePoint(this);
		}
		public override bool Update(MouldCurve cur)
		{
			if (cur == null)
				return false;
			List<IFitPoint> pts = new List<IFitPoint>(cur.FitPoints);
			int index = pts.IndexOf(this);
			if( index == 0 )
				index++;//get the next point if this is the first point
			else
				index--;//otherwise get the previous point
			double sCur = m_curvePos.Value;
			Vect2 uv = new Vect2();
			Vect3 xyzT = new Vect3();
			Vect3 dxyz = new Vect3(1,0,0);//horizontal reference
			cur.xVal(pts[index].UV, ref xyzT);//reference point

			bool ret = CurveTools.AnglePoint(m_curve, ref sCur, ref uv, ref xyzT, dxyz, m_Angle.Value, true);
			if (ret)
				m_curvePos.Value = sCur;
			return ret;
		}

		public override void ReadEditor(IFitEditor edit)
		{
			base.ReadEditor(edit);
			if (edit == null)
				throw new ArgumentNullException();
			if (!(edit is CurvePointEditor))
				throw new ArgumentException("Type must be CurvePointEditor");
			CurvePointEditor cdit = edit as CurvePointEditor;
			m_curve = cdit.Curve;
			m_Angle = cdit.CS;
		}
		public override Control WriteEditor(ref IFitEditor edit)
		{
			if (edit == null || !(edit is CurvePointEditor))
				edit = new CurvePointEditor();
			CurvePointEditor cdit = edit as CurvePointEditor;
			cdit.Tag = GetType();
			cdit.Curve = m_curve;
			cdit.CS = m_Angle;

			return cdit;
		}

		public override TreeNode Node
		{
			get
			{
				TreeNode node = base.Node;
				node.Nodes.Insert(0, new TreeNode(string.Format("Angle: {0}", m_curve.Label)){ImageKey = "empty", SelectedImageKey = "empty"});
				return node;
			}
		}
		public override System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlNode node = NsXml.MakeNode(doc, GetType().Name);
			NsXml.AddAttribute(node, "Curve", m_curve.Label);
			node.AppendChild(m_Angle.WriteXScript(doc));
			node.AppendChild(m_curvePos.WriteXScript(doc));
			return node;
		}

		public override void ReadXScript(Sail s, System.Xml.XmlNode node)
		{
			m_curve = s.FindCurve(NsXml.ReadString(node, "Curve"));
			m_Angle.ReadXScript(s, node.FirstChild);
			m_curvePos.ReadXScript(s, node.LastChild);
		}
	}
}
