using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public partial class CurvePointEditor : UserControl, IFitEditor
	{
		public CurvePointEditor()
		{
			InitializeComponent();
			//m_cs.ReturnPress += ReturnPress;
		}

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
		public MouldCurve Curve
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

		[DefaultValue("")]
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

		public IEnumerable<MouldCurve> Curves 
		{
			set 
			{ 
				m_curves.Items.Clear();
				m_curves.Items.AddRange(value.ToArray());
			} 
		}

		#region IFitEditor Members

		public IFitPoint CreatePoint()
		{
			object fit = Utilities.CreateInstance(FitType);
			if (fit != null && fit is CurvePoint)
			{
				(fit as CurvePoint).S_Equ = CS;
				(fit as CurvePoint).m_curve = Curve;
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

		public List<object> AutoFillData
		{
			set
			{
				if (value == null)
					return;

				m_cs.AutoFillVariables = value;
				MouldCurve c = Curve;//backup current curve
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

		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 2 - (2 * Padding.Horizontal);

			m_cs.Width = wid;
			m_curves.Left = wid+Padding.Horizontal;
			m_curves.Width = wid;
			//m_cs.Height = 23;
			//m_curves.Height = 23;
			//Height = 23;

		}
	}
}
