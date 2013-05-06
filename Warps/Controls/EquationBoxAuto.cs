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

		public void SetText(string txt)
		{
			//txt = "= " + txt;
			equationBox.Text = txt;
		}

		void ief_OnVariableAdded(object sender, string eqText)
		{
			SetText(eqText);
			toolTip1.ToolTipTitle = equationBox.Text;
			if (EQ != null)
				button1.BackColor = Color.Lime;
			else
				button1.BackColor = m_bak;
		}

		public override string Text
		{
			get { return equationBox.Text; }
			set { SetText(value); toolTip1.ToolTipTitle = value; }
		}
		public string EQ
		{
			get { return equationBox.Text; } // equation.EQ }
		}

		public Equation Equation
		{
			get { return new Equation("eq", Text); }
			set
			{
				Text = value.Label;
				equationBox.EQ = value.EquationText;
			}
		}

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
				button1.BackColor = Color.Lime;
			else
				button1.BackColor = m_bak;
		}

		/// <summary>
		/// give it a sail reference and the watermark locations
		/// </summary>
		/// <param name="m_sail">Sail reference</param>
		/// <param name="group">watermark group</param>
		internal void Prep(Sail sail, IRebuild group)
		{
			m_sail = sail;
			AutoFillVariables = m_sail.GetAutoFillData(group);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			button1.Top = 0;
			equationBox.Top = 0;

			//square button
			button1.Height = button1.Width = Height;

			//full height text box
			equationBox.Height = Height;

			//right-justified button
			button1.Left = Width - button1.Width-3;
			equationBox.Width = Width - button1.Width-3;
		}
	}
}
