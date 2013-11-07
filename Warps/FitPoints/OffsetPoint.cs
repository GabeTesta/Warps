using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Curves;
using System.Xml;
namespace Warps
{
	class OffsetPoint: IFitPoint
	{
		public OffsetPoint():this(0,null,0){}
		public OffsetPoint(OffsetPoint cloneme) : this(cloneme.m_sCurve, cloneme.m_curve, cloneme.m_xOffset) {  }
		public OffsetPoint(double sCurve, IMouldCurve curve, double offset)
		{
			m_sCurve.Value = sCurve;
			m_curve = curve;
			m_xOffset.Value = offset;
		}
		public OffsetPoint(Equation sCurve, IMouldCurve curve, Equation offset)
		{
			m_sCurve = sCurve;
			m_curve = curve;
			m_xOffset = offset;
			//Update(null);
		}

		Equation m_sCurve = new Equation("Curve Position", 0);
		IMouldCurve m_curve;
		Equation m_xOffset = new Equation("Offset", 0);

		public Equation OffsetEq
		{
			get { return m_xOffset; }
			set { m_xOffset = value; }
		}
		public IMouldCurve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
		}
		public Equation CurvePosEq
		{
			get { return m_sCurve; }
			set { m_sCurve = value; }
		}

		public double CurvePos { get { return CurvePosEq.Value; } }
		public double OffsetVal { get { return OffsetEq.Value; } }

		#region IFitPoint Members

		double m_sPos;
		public double S
		{
			get
			{
				return m_sPos;
			}
			set
			{
				m_sPos = value;
			}
		}

		Vect2 m_uv = new Vect2();
		public Vect2 UV
		{
			get
			{
				return m_uv;
			}
			//set
			//{
			//	throw new NotImplementedException();
			//}
		}

		public IFitPoint Clone()
		{
			return new OffsetPoint(this);
		}

		public double this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:
						return m_sPos;
					case 1:
						return UV[0];
					case 2:
						return UV[1];
					default:
						return 0;
				}
			}
			set
			{
				switch (i)
				{
					case 0:
						m_sPos = value;
						break;
					case 1:
						//U.Value = value;
						break;
					case 2:
						//V.Value = value;
						break;
				}
			}
		}
		
		public TreeNode Node
		{
			get
			{
				TreeNode point = new TreeNode(string.Format("{0:0.0000} [{1}]", S, UV.ToString("0.0000")));
				point.ImageKey = point.SelectedImageKey = GetType().Name;
				point.Tag = this;

				TreeNode tmp;

				tmp = new TreeNode(string.Format("Offset: {0:f4}", OffsetEq.Value));
				tmp.ImageKey = tmp.SelectedImageKey = typeof(Equation).Name;
				point.Nodes.Add(tmp);

				tmp = new TreeNode(string.Format("Curve: {0}", Curve == null ? "empty" : Curve.Label));
				tmp.ImageKey = tmp.SelectedImageKey = typeof(MouldCurve).Name;
				point.Nodes.Add(tmp);

				tmp = new TreeNode(string.Format("Position: {0:0.0000}", CurvePosEq.Value));
				tmp.ImageKey = tmp.SelectedImageKey = typeof(Equation).Name;
				point.Nodes.Add(tmp);

				return point;
			}
		}

		public Control WriteEditor(ref IFitEditor edit)
		{
			if (edit == null || !(edit is OffsetPointEditor))
				edit = new OffsetPointEditor();
			OffsetPointEditor cdit = edit as OffsetPointEditor;
			cdit.Tag = GetType();

			cdit.Offset = OffsetEq;
			cdit.Curve = m_curve;
			cdit.CurvePos = CurvePosEq;

			return cdit;
		}

		public void ReadEditor(IFitEditor edit)
		{
			if (edit == null)
				throw new ArgumentNullException();
			if (!(edit is OffsetPointEditor))
				throw new ArgumentException("Type must be CurvePointEditor");
			OffsetPointEditor cdit = edit as OffsetPointEditor;

			OffsetEq = cdit.Offset;
			Curve = cdit.Curve;
			CurvePosEq = cdit.CurvePos;
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			if( Curve is IRebuild )
				parents.Add(Curve as IRebuild);

			CurvePosEq.GetParents(s, parents);
			OffsetEq.GetParents(s, parents);
		}

		public bool Affected(List<IRebuild> connected)
		{
			if (connected.Contains(Curve as IRebuild))
				return true;
			if (CurvePosEq.Affected(connected))
				return true;
			if (OffsetEq.Affected(connected))
				return true;
			return false;
		}

		public bool Update(MouldCurve cur)
		{
			OffsetEq.Update(cur.Sail);
			CurvePosEq.Update(cur.Sail);
			int nNwt;
			Vect3 x = new Vect3(), xn = new Vect3();
			Vect2 un = new Vect2();
			//get unit inplane normal in u-coords
			Curve.uNor(CurvePos, ref m_uv, ref un);
			for( nNwt = 0; nNwt < 25; nNwt++ )
			{
				//curve point and normal offset in x-coords
				Curve.xVal(m_uv, ref x);
				Curve.xVal(un + m_uv, ref xn);

				//x-offset from unit normal
				double dn = xn.Distance(x);
				if (BLAS.IsEqual(dn, OffsetVal, 1e-6))
					break;
				//scale normal to match target offset
				dn = OffsetVal / dn;
				un.Magnitude *= dn;
			}
			//offset uv coords using scaled normal
			m_uv += un;

			return nNwt < 25;
		}

		public bool ValidFitPoint
		{
			get { return Curve != null;  }
		}

		public XmlNode WriteXScript(XmlDocument doc)
		{
			XmlNode node = NsXml.MakeNode(doc, GetType().Name);
			node.AppendChild(CurvePosEq.WriteXScript(doc));
			//NsXml.AddAttribute(node, "S-Curve", m_sCurve.ToString());
			NsXml.AddAttribute(node, "Curve", Curve.Label);
			node.AppendChild(OffsetEq.WriteXScript(doc));
			//NsXml.AddAttribute(node, "Offset", m_xOffset.ToString());

			return node;
		}

		public void ReadXScript(Sail s, XmlNode node)
		{
			CurvePosEq.ReadXScript(s, node.FirstChild);
			//m_sCurve = NsXml.ReadDouble(node, "S-Curve");
			Curve = s.FindCurve(NsXml.ReadString(node, "Curve"));
			OffsetEq.ReadXScript(s, node.LastChild);
			//m_xOffset = NsXml.ReadDouble(node, "Offset");
		}

		#endregion
	}
}
