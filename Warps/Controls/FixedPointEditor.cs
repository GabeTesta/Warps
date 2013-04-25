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
			m_uEq.ReturnPress += OnReturnPress;
			m_vEq.ReturnPress += OnReturnPress;
		}

		public string Label
		{
			set { label1.Text = value; }
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

		public Equation U
		{
			get { return m_uEq.Equation; }
			set { m_uEq.Equation = value; }
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

		public Equation V
		{
			get { return m_vEq.Equation; }
			set { m_vEq.Equation = value; }
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

		#region IFitEditor Members

		public event EventHandler<KeyEventArgs> ReturnPress;
		void OnReturnPress(object sender, KeyEventArgs e)
		{
			if (ReturnPress != null)
				ReturnPress(sender, e);
		}

		public Type FitType
		{
			get { return typeof(FixedPoint); }
		}

		object[] m_auto = null;

		public object[] AutoFillData 
		{ 
			get { return m_auto; } 
			set 
			{ 
				m_auto = value;
				m_uEq.AutoFillVariables = value.ToList();
				m_vEq.AutoFillVariables = value.ToList();
			} 
		}
		Sail m_sail = null;
		public Sail sail
		{
			get { return m_sail; }
			set 
			{ 
				m_sail = value;
				m_uEq.sail = value;
				m_vEq.sail = value;
			}
		}

		#endregion
	}
}
 