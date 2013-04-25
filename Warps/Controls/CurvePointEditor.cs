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
			m_cs.ReturnPress += m_cs_ReturnPress;
		}

		public string Label
		{
			set
			{
				if (value == null)
				{
					m_label.Text = "";
					image.Image = Warps.Properties.Resources.CurvePoint;
					return;
				}
				else if (value == "SlidePoint")
					image.Image = Warps.Properties.Resources.SlidePoint;
				else
					image.Image = Warps.Properties.Resources.CurvePoint;
				m_label.Text = value;
			}
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

		public IEnumerable<MouldCurve> Curves 
		{
			set 
			{ 
				m_curves.Items.Clear();
				m_curves.Items.AddRange(value.ToArray());
			} 
		}

		#region IFitEditor Members

		public event EventHandler<KeyEventArgs> ReturnPress;
		void m_cs_ReturnPress(object sender, KeyEventArgs e)
		{
			if (ReturnPress != null)
				ReturnPress(sender, e);
		}

		public Type FitType
		{
			get 
			{ 
				return Tag == null ? null : Tag as Type;
			}
		}

		object[] m_auto = null;

		public object[] AutoFillData
		{
			get { return m_auto; }
			set
			{
				if (value == null)
					return;

				m_auto = value;
				m_cs.AutoFillVariables = value.ToList();
			} 
		}
		Sail m_sail = null;
		public Sail sail
		{
			get { return m_sail; }
			set
			{
				m_sail = value;
				m_cs.sail = value;
			}
		}

		#endregion

		private void CurvePointEditor_Load(object sender, EventArgs e)
		{
			//if(m_curves!=null)


		}

		private void SwitchEditor(string p)
		{
			
		}

		private void m_cs_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
