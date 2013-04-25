﻿using System;
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
			m_group = varGroup;
			Count = 0;
			Equations = varGroup.ToArray();
			Label = varGroup.Label;
		}

		VariableGroup m_group;

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

				Count = value.Length;

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
			ve.Size = new System.Drawing.Size(406, 28);
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

		public bool Editable
		{
			get
			{
				return m_labelBox.Enabled;
			}
			set
			{
				m_labelBox.Enabled = value;
				m_flow.Enabled = value;
			}
		}
	}
}
