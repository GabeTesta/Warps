using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class EquationBoxAuto : UserControl
	{
		public EquationBoxAuto()
		{
			InitializeComponent();
			m_bak = Color.White;
		//	equationBox.ReturnPress += ReturnPress;
		}
		Color m_bak = Color.Empty;
		private Sail m_sail = null;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Sail sail
		{
			get { return m_sail; }
			set { m_sail = value; equationBox.sail = value; }
		}
		public List<object> AutoFillVariables
		{
			get { return equationBox.Values == null ? null : equationBox.Values.ToList(); }
			set { if (value != null) equationBox.Values = value.ToArray(); }
		}

		private void button1_Click(object sender, EventArgs e)
		{
			InputEquationForm ief = new InputEquationForm(AutoFillVariables, equationBox.Text, sail);
			ief.OnVariableAdded += ief_OnVariableAdded;
			ief.ShowDialog();
		}

		public void SelectAll()
		{	
			equationBox.SelectAll();
			equationBox.Focus();
		}

		void ief_OnVariableAdded(object sender, string eqText)
		{
			Text = eqText;
			toolTip1.ToolTipTitle = equationBox.Text;
			if (EQ != null && !equationBox.IsNumber())
				buttonFn.BackColor = Color.Lime;
			else
				buttonFn.BackColor = m_bak;
		}

		public override string Text
		{
			get { return equationBox.Text; }
			set { equationBox.Text = value; toolTip1.ToolTipTitle = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EQ
		{
			get { return equationBox.Text; } // equation.EQ }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Equation Equation
		{
			get { return new Equation(m_label, Text); }
			set
			{
				//if (value.IsNumber())
				//	equationBox.Value = value.Value;
				//else
				m_label = value.Label;
				equationBox.Text = value.EquationText;//text will be either value.tostring() or the eq text
				//Text = value.Label;
				//equationBox.EQ = value.EquationText;
			}
		}
		string m_label = "";
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double Value
		{
			get
			{
				return equationBox.Value;
			}
			set
			{
				equationBox.Value = value;	
			}
		}

		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//	base.OnKeyDown(e);
		//	if (ReturnPress != null && e.KeyCode == Keys.Enter)
		//		ReturnPress(this, e);
		//}

		//public event KeyEventHandler ReturnPress;
		//private void OnReturnPress(object sender, KeyEventArgs e)
		//{
		//	if (ReturnPress != null)
		//		ReturnPress(this, e);
		//}
		private void EquationBoxAuto_Load(object sender, EventArgs e)
		{
			equationBox_TextChanged(sender, e);
		}

		private void equationBox_TextChanged(object sender, EventArgs e)
		{
			if (!equationBox.IsNumber())
				buttonFn.BackColor = Color.Lime;
			else
				buttonFn.BackColor = m_bak;
		}

		/// <summary>
		/// give it a sail reference and the watermark locations
		/// </summary>
		/// <param name="m_sail">Sail reference</param>
		/// <param name="group">watermark group</param>
		internal void Prep(Sail sail, IRebuild group)
		{
			m_sail = sail;
			AutoFillVariables = m_sail.Watermark(group).ToList<object>();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			equationBox.Location = new Point(0, 0);

			//square button
			buttonFn.Size = new System.Drawing.Size(equationBox.Height, equationBox.Height);

			//full height text box
			//equationBox.Height = Height;

			//right-justified button
			equationBox.Width = Width - buttonFn.Width;
			buttonFn.Location = new Point(equationBox.Right, 0);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			equationBox.Height = Height;
			Height = equationBox.Height;
		}
	}
}
