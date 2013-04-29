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
	public partial class VariableEditor : UserControl
	{
		public VariableEditor()
		{
			InitializeComponent();
		}

		public VariableEditor(string label, string EqText)
		{
			InitializeComponent();
			Label = label;
			if (EqText.Length > 0)
			{
				//if (EqText[0] == '=')
					m_eqBox.Text = EqText;
				//else
				//	m_eqBox.Text = "= " + EqText;
			}
		}

		public event EventHandler<KeyEventArgs> ReturnPress;

		void c_ReturnPress(object sender, KeyEventArgs e)
		{
			if (ReturnPress != null)
				ReturnPress(sender, e);
		}

		public string Label
		{
			set
			{
				if (value == null)
					m_variableTextBox.Text = "Variable Name";
				else
					m_variableTextBox.Text = value;
			}
			get
			{
				return m_variableTextBox.Text;
			}
		}

		public string EquationText
		{
			get { return m_eqBox.Text; }
			set { m_eqBox.Text = value; }
		}

		public void FocusEditBox()
		{
			m_variableTextBox.SelectAll();
			//m_eqBox.Focus();
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
				m_eqBox.AutoFillVariables = value.ToList();
			}
		}
		Sail m_sail = null;
		public Sail sail
		{
			get { return m_sail; }
			set
			{
				m_sail = value;
				m_eqBox.sail = value;
			}
		}
	}



}
