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
	public partial class FixedPointEditor : UserControl, IFitEditor
	{
		public FixedPointEditor()
		{
			InitializeComponent();
			//m_uEq.ReturnPress += ReturnPress;
			//m_vEq.ReturnPress += ReturnPress;
		}


		public Vect2 UV
		{
			get { return new Vect2(u, v); }
			set
			{
				u = value.u;
				v = value.v;
			}
		}
		public Equation U
		{
			get { return m_uEq.Equation; }
			set { m_uEq.Equation = value; }
		}
		public Equation V
		{
			get { return m_vEq.Equation; }
			set { m_vEq.Equation = value; }
		}

		[DefaultValue(0)]
		double u
		{
			get
			{
				return m_uEq.Value;
			}
			set
			{
				m_uEq.Text = value.ToString("0.0000");
			}
		}
		[DefaultValue(0)]
		double v
		{
			get
			{ 
				return m_vEq.Value;
			}
			set
			{
				m_vEq.Text = value.ToString("0.0000");
			}
		}


		#region IFitEditor Members

		public IFitPoint CreatePoint()
		{
			object fit = Utilities.CreateInstance(FitType.Name);
			if (fit != null && fit is FixedPoint)
			{
				(fit as FixedPoint).U = U;
				(fit as FixedPoint).V = V;
				return fit as IFitPoint;
			}
			return null;
		}

		//public event KeyEventHandler ReturnPress;
		//void OnReturnPress(object sender, KeyEventArgs e)
		//{
		//	if (ReturnPress != null)
		//		ReturnPress(sender, e);
		//}

		public Type FitType
		{
			get { return typeof(FixedPoint); }
		}

		public List<object> AutoFillData 
		{ 
			set 
			{ 
				m_uEq.AutoFillVariables = value;
				m_vEq.AutoFillVariables = value;
			} 
		}

		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 2 - (2 * Padding.Horizontal);

			m_uEq.Width = wid;
			m_vEq.Left = wid +Padding.Horizontal;
			m_vEq.Width = wid;
			Height = 23;
			m_vEq.Height = m_uEq.Height = 23;
		}
	}
}
 