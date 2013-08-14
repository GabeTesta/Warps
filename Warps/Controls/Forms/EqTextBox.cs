using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls.Forms
{
	public partial class EqTextBox : UserControl
	{
		public EqTextBox()
		{
			InitializeComponent();
			BAK = m_fn.BackColor;
			m_fn.Paint += m_fn_Paint;
		}

		Sail m_sail;
		Equation m_equation = new Equation();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Sail Sail
		{
			get { return m_sail; }
			set { m_sail = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation Eqn
		{
			get { return m_equation; }
			set
			{
				m_equation.EquationText = value.EquationText;
				m_text.Text = m_equation.EquationText;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double Value
		{
			get { return m_equation.Value; }
			set
			{
				m_equation.Value = value;
				m_text.Text = m_equation.EquationText;
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			m_text.Height = Height;
			m_fn.Size = new Size(m_text.Height, m_text.Height);
			m_text.Width = Width - m_fn.Width;
			m_fn.Location = new Point(m_text.Width, 0);
			m_text.Location = new Point(0, 0);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			m_text.Height = Height;
			Height = m_text.Height;
		}

		private void m_text_Enter(object sender, EventArgs e)
		{
			if (m_equation != null && !m_equation.IsNumber())
				m_text.Text = m_equation.EquationText;
		}

		private void m_text_Leave(object sender, EventArgs e)
		{
			if (m_equation == null)
				m_equation = new Equation(m_text.Text);
			m_text.Text = m_equation.Value.ToString("f4");
	
		}

		private void m_text_Validating(object sender, CancelEventArgs e)
		{
			m_equation.EquationText = m_text.Text;
			if (double.IsNaN(m_equation.Evaluate(m_sail)))
			{
				//prompt user on fail
				MessageBox.Show("Please enter a valid Equation");
				m_text.Focus();
			}
			m_fn.BackColor = m_equation.IsNumber() ? BAK : FN;
		}
		Color BAK, FN = Color.SeaGreen;

		private void m_fn_Click(object sender, EventArgs e)
		{

		}

		void m_fn_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawString("Fn", new Font(FontFamily.GenericSansSerif, 9), Brushes.Black, 0, 0);
		}
	}
}
