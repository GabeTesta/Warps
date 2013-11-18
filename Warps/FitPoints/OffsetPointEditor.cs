using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Curves;

namespace Warps
{
	public partial class OffsetPointEditor : UserControl, IFitEditor
	{
		public OffsetPointEditor()
		{
			InitializeComponent();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation CurvePos
		{
			get
			{
				return m_cs.Equation;
				//double u;
				//if (double.TryParse(m_cs.Text, out u))
				//	return u;
				//else return double.MaxValue;
			}
			set
			{
				m_cs.Equation = value;
				//m_cs.Text = value.ToString("0.0000");
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IMouldCurve Curve
		{
			get
			{
				return m_curves.SelectedItem as MouldCurve;
			}
			set
			{
				if (value != null && !m_curves.Items.Contains(value))
					m_curves.Items.Add(value);
				m_curves.SelectedItem = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation Offset
		{
			get
			{
				return m_offset.Equation;
			}
			set
			{
				m_offset.Equation = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string CurvePosText
		{
			get
			{
				return m_cs.Text;
			}
			set
			{
				m_cs.Text = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string OffsetText
		{
			get
			{
				return m_offset.Text;
			}
			set
			{
				m_offset.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<MouldCurve> Curves
		{
			set
			{
				m_curves.Items.Clear();
				m_curves.Items.AddRange(value.ToArray());
			}
		}

		#region IFitEditor Members

		//public IFitPoint CreatePoint()
		//{
		//	OffsetPoint fit = Utilities.CreateInstance<OffsetPoint>(FitType);
		//	if (fit != null)
		//	{
		//		fit.CurvePosEq = CurvePos;
		//		fit.Curve = Curve;
		//		fit.OffsetEq = Offset;
		//		return fit as IFitPoint;
		//	}
		//	return null;
		//}

		public Type FitType
		{
			get { return typeof(OffsetPoint); }
		}

		public List<object> AutoFillData
		{
			set
			{
				if (value == null)
					return;

				m_cs.AutoFillVariables = value;
				IMouldCurve c = Curve;//backup current curve
				m_curves.Items.Clear();
				foreach (object o in value)
				{
					if (o is MouldCurve)
					{
						if (!m_curves.Items.Contains(o))
							m_curves.Items.Add(o);
					}
				}
				Curve = c;//select current curve
			}
		}

		//[-.400;sli00;0.250]
		//[<offset from reference curve as percentage of reference curve's length>; <referece curve>; <position along refernece curve>]
		public string W4LText
		{
			get
			{
				string type = FitType.Name.ToString();
				type = type.ToUpper().Substring(0, 5);
				string lbl = Curve.Label.Length > 5 ? Curve.Label.Substring(0, 5) : Curve.Label;

				return String.Format("[{0,5};{1,5};{2,5}]",
					(Offset.Value / Curve.Length).ToString("f3"),
					lbl,
					CurvePos.Value.ToString("f3"));
			}
		}

		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 3 - (2*Padding.Horizontal);

			m_curves.Width = m_cs.Width = m_offset.Width = wid;
			m_cs.Location = new Point(0, 0);
			m_curves.Location = new Point(wid + Padding.Horizontal, 0);
			m_offset.Location = new Point(2 * wid + 2 * Padding.Horizontal, 0);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			m_curves.Height = Height;
			m_cs.Height = Height;
			m_offset.Height = Height;
			Height = Math.Max(m_offset.Height, Math.Max(m_curves.Height, m_cs.Height));
		}

		private void m_curves_Validating(object sender, CancelEventArgs e)
		{
			if (m_curves.SelectedItem != null)
				return;//valid selection already

			//search curve list for specified curve
			foreach (Object o in m_curves.Items)
				if (o.ToString() == m_curves.Text)
				{
					m_curves.SelectedItem = o;
					return;
				}

			//prompt user on fail
			MessageBox.Show("Please select a valid curve");
			m_curves.Focus();
		}

	}
}
