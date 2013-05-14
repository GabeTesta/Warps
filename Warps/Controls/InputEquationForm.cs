using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class InputEquationForm : Form
	{
		public delegate void OnAddHandler(object sender, string eqText);

		public event OnAddHandler OnVariableAdded;

		public InputEquationForm()
		{
			InitializeComponent();
		}

		public InputEquationForm(List<object> AutoFillVariables, string currentText, Sail s)
		{
			InitializeComponent();

			m_sail = s;

			CalculateButton.Enabled = m_sail != null;
			// TODO: Complete member initialization

			autoCompleteTextBox1.Text = currentText;

			if (AutoFillVariables != null)
			{
				autoCompleteTextBox1.Values = AutoFillVariables.ToArray();

				AutoFillVariables.ForEach(var =>
				{
					if (var is MouldCurve)
						CurveListBox.Items.Add(var);
					else if (var is string)
						EquationListBox.Items.Add(var);
					//else if (var is Equation)
					//	EquationListBox.Items.Add((var as Equation).Label);
				});
			}
		}

		Sail m_sail = null;

		private void button1_Click(object sender, EventArgs e)
		{
			if (m_sail == null)
				return;

			double result = 0;

			if (EquationEvaluator.Evaluate(new Equation("Test", autoCompleteTextBox1.Text), m_sail, out result, true))
				resultBox.Text = result.ToString();
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			if (OnVariableAdded != null)
				OnVariableAdded(this, autoCompleteTextBox1.Text.TrimStart(new char[] { ' ' }));
			this.Close();
		}

		private void EquationListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			autoCompleteTextBox1.Text += EquationListBox.SelectedItem.ToString();
		}

		private void CurveListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			autoCompleteTextBox1.Text += CurveListBox.SelectedItem.ToString();
		}
	}
}
