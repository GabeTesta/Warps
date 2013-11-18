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


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Vect2 UV
		{
			get { return new Vect2(u, v); }
			set
			{
				u = value.u;
				v = value.v;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation U
		{
			get { return m_uEq.Equation; }
			set { m_uEq.Equation = value; }
		}
				
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation V
		{
			get { return m_vEq.Equation; }
			set { m_vEq.Equation = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		double u
		{
			get
			{
				return m_uEq.Value;
			}
			set
			{
				m_uEq.Value = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		double v
		{
			get
			{ 
				return m_vEq.Value;
			}
			set
			{
				m_vEq.Value = value;
			}
		}


		#region IFitEditor Members

		//public IFitPoint CreatePoint()
		//{
		//	FixedPoint pnt = new FixedPoint();
		//	pnt.ReadEditor(this);
		//	return pnt;
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
		public string W4LText
		{
			get
			{
				return String.Format("POINT [{0};{1}]",
					u.ToString("f3"),
					v.ToString("f3"));
			}
		}
		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 2 - Padding.Horizontal;
			m_vEq.Width = m_uEq.Width = wid;
			m_uEq.Location = new Point(0, 0);		
			m_vEq.Location = new Point(wid +Padding.Horizontal,0);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			m_uEq.Height = Height;
			m_vEq.Height = m_uEq.Height;
			Height = m_uEq.Height;
		}
	}
}
 