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
				List<IRebuild> watermark = m_sail.Watermark(group);
				//availableEqs = new List<Equation>();
				CurveListBox.Items.Clear();
				EquationListBox.Items.Clear();
				listView1.Items.Clear();
				foreach (IRebuild entry in watermark)
				{
					if (entry is Equation)
					{
					//	availableEqs.Add(entry as Equation);
						EquationListBox.Items.Add(entry.Label);
						listView1.Items.Add(entry.Label);
						if (!m_group.ContainsKey(entry.Label))
							listView1.Items[listView1.Items.Count - 1].Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Italic);
					}
					else if( entry is MouldCurve )
						CurveListBox.Items.Add(entry as MouldCurve);
				}
				autoCompleteTextBox1.Values = watermark.ToArray<object>();
			}
		}

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

			List<Equation> availableEqs = m_sail.WatermarkEqs(m_group);
			EquationListBox.Items.Clear();
			foreach ( Equation entry in availableEqs)
				EquationListBox.Items.Add(entry.Label);
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
