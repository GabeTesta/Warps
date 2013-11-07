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
	public partial class CurvePointEditor : UserControl, IFitEditor
	{
		public CurvePointEditor()
		{
			InitializeComponent();
			//m_cs.ReturnPress += ReturnPress;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation CS
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
		public string CSText
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
		public IEnumerable<MouldCurve> Curves 
		{
			set 
			{ 
				m_curves.Items.Clear();
				m_curves.Items.AddRange(value.ToArray());
			} 
		}

		#region IFitEditor Members

		public virtual IFitPoint CreatePoint()
		{
			CurvePoint fit = Utilities.CreateInstance<CurvePoint>(FitType);
			if (fit != null)
			{
				fit.PosEQ = CS;
				fit.m_curve = Curve;
				return fit as IFitPoint;
			}
			return null;
		}
		
		//public event KeyEventHandler ReturnPress;
		//void m_cs_ReturnPress(object sender, KeyEventArgs e)
		//{
		//	if (ReturnPress != null)
		//		ReturnPress(sender, e);
		//}

		public Type FitType
		{
			get 
			{ 
				return Tag == null ? null : Tag as Type;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
				Curve = c;//select currecnt curve
			} 
		}

		public string W4LText
		{
			get
			{
				string type = FitType.Name.ToString();
				type = type.ToUpper().Substring(0, 5);
				string lbl = Curve.Label.Length > 5 ? Curve.Label.Substring(0, 5) : Curve.Label;

				return String.Format("{0,5} [{1,5};{2}]",
					type,
					lbl,
					CS.Value.ToString("f3"));
			}
		}
		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 2 - Padding.Horizontal;

			m_curves.Width = m_cs.Width = wid;
			m_cs.Location = new Point(0, 0);
			m_curves.Location = new Point(wid + Padding.Horizontal, 0);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			m_curves.Height = Height;
			m_cs.Height = Height;
			Height = Math.Max(m_curves.Height, m_cs.Height);
		}

		private void m_curves_Validating(object sender, CancelEventArgs e)
		{
			if (m_curves.SelectedItem != null)
				return;//valid selection already

			//search curve list for specified curve
			foreach( Object o in m_curves.Items )
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
