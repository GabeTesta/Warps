using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public partial class EquationEditorForm : Form
	{
		public delegate void OnAddHandler(object sender, Equation addedEq);
		public delegate void OnDeleteHandler(object sender, Equation DeleteMe);

		public event OnAddHandler OnVariableAdded;
		public event OnDeleteHandler OnVariableDeleted;

		public EquationEditorForm(VariableGroup group)
		{
			InitializeComponent();
			m_group = group;
			m_sail = group.Sail;
			if (m_sail != null)
			{
				List<object> autoComplete = m_sail.GetAutoFillData(group);
				List<MouldCurve> curves = m_sail.GetCurves(group);
				//curves.ForEach(cur => { autoComplete.Add(cur); });
				curves.ForEach(curve => CurveListBox.Items.Add(curve));
				availableEqs = m_sail.GetEquations(m_group);
				EquationListBox.Items.Clear();
				foreach (KeyValuePair<string, Equation> entry in availableEqs)
				{
					//autoComplete.Add(entry.Value);
					EquationListBox.Items.Add(entry.Key);
					listView1.Items.Add(entry.Key);
					if (!m_group.ContainsKey(entry.Key))
						listView1.Items[listView1.Items.Count - 1].Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Italic);
					//else
					//	listView1.Items[listView1.Items.Count - 1].BackColor = Color.White;
					//	EquationListBox.Items[EquationListBox.Items.Count-1]
				}
				autoCompleteTextBox1.Values = autoComplete.ToArray();
			}
		}

		List<KeyValuePair<string, Equation>> availableEqs = null;
		VariableGroup m_group = null;
		Sail m_sail = null;

		private void addButton_Click(object sender, EventArgs e)
		{
			if (!m_group.ContainsKey(EquationNameBox.Text))
				m_group.Add(EquationNameBox.Text, new Equation(EquationNameBox.Text, autoCompleteTextBox1.Text));
			else
				m_group[EquationNameBox.Text] = new Equation(EquationNameBox.Text, autoCompleteTextBox1.Text);

			if (OnVariableAdded != null)
				OnVariableAdded(this, m_group[EquationNameBox.Text]);

			List<KeyValuePair<string, Equation>> availableEqs = m_sail.GetEquations(m_group);
			EquationListBox.Items.Clear();
			foreach (KeyValuePair<string, Equation> entry in availableEqs)
				EquationListBox.Items.Add(entry.Key);
		}

		void ToggleEditEnable(bool show)
		{
			textBox1.Enabled = show;
			autoCompleteTextBoxEdit.Enabled = show;
			label4.Enabled = show;
			button1.Enabled = show;
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string eqname = EquationListBox.SelectedItem.ToString();
			if (m_group.ContainsKey(eqname))
			{
				textBox1.Text = eqname;
				autoCompleteTextBoxEdit.Text = m_group[eqname].EquationText;
				valuebox.Text = m_group[eqname].Result.ToString();
				ToggleEditEnable(true);
				
			}else
				ToggleEditEnable(false);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string eqname = EquationListBox.SelectedItem.ToString();
			m_group.Remove(eqname);
			m_group.Add(eqname, new Equation(eqname, autoCompleteTextBoxEdit.Text));
			ToggleEditEnable(false);

			if (OnVariableAdded != null)
				OnVariableAdded(this, m_group[eqname]);
		}

		private void EquationNameBox_Enter(object sender, EventArgs e)
		{
			ToggleEditEnable(false);
		}

		private void EquationTester_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

	}
}
