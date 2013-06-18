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
	public partial class VariableGroupEditor : UserControl
	{
		public VariableGroupEditor(VariableGroup varGroup)
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			VarGroup = varGroup;
			Count = 0;
			Equations = varGroup.ToArray();
			Label = varGroup.Label;
		}

		VariableGroup m_group;

		public VariableGroup VarGroup
		{
			get { return m_group; }
			set { m_group = value; }
		}

		public string Label
		{
			get { return m_labelBox.Text; }
			set { m_labelBox.Text = value; }
		}

		public int Count
		{
			set { m_count.Text = string.Format("Count: {0}", value); }
			get { return m_flow.Controls.Count; }
		}

		public KeyValuePair<string, Equation>[] Equations
		{
			set
			{
				m_flow.SuspendLayout();

				m_flow.Controls.Clear();

				foreach (KeyValuePair<string,Equation> eq in value)
					Add(eq.Key, eq.Value);

				Count = VarGroup.Count;
				m_flow.ResumeLayout();
			}
		}

		public VariableEditor this[int index]
		{
			get { return m_flow.Controls[index] as VariableEditor; }
		}

		void Add(string label, Equation eq)
		{
			VariableEditor ve = eq.WriteEditor(null);
			ve.sail = VarGroup.Sail;
			ve.AutoFillData = VarGroup.Sail.Watermark(eq).ToList<object>();
			m_flow.Controls.Add(ve);
			ve.ReturnPress += ve_ReturnPress;
		}

		void ve_ReturnPress(object sender, KeyEventArgs e)
		{
			int index = -1;
			if (sender is IFitEditor)
				index = m_flow.Controls.IndexOf(sender as Control);
			else if (sender is EquationBox)
				index = m_flow.Controls.IndexOf((sender as EquationBox).Parent);
			if (ReturnPress != null)
				ReturnPress(sender, new EventArgs<int>(index));
		}

		public event EventHandler<EventArgs<int>> ReturnPress;

		internal void HighlightEquation(Equation equation)
		{
			for (int i = 0; i < m_flow.Controls.Count; i++)
				if ((m_flow.Controls[i] as VariableEditor).Label == equation.Label)
					(m_flow.Controls[i] as VariableEditor).FocusEditBox();
		}

		public bool EditMode
		{
			get
			{
				return this.Enabled;
			}
			set
			{
				this.Enabled = value;
				for (int i = 0; i < Count; i++)
					this[i].EditMode = value;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Add("new var", new Equation());
			Count = VarGroup.Count;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Delete selected variables?","", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				List<VariableEditor> toBremoved = new List<VariableEditor>();
				foreach (VariableEditor ved in m_flow.Controls)
				{
					if (ved.Selected)
					{
						ved.ReturnPress -= ve_ReturnPress;
						toBremoved.Add(ved);
					}
				}
				m_flow.SuspendLayout();
				toBremoved.ForEach(cntl => m_flow.Controls.Remove(cntl));
				m_flow.ResumeLayout();
			}
		}
	}
}
