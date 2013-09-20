using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Mixed
{
	public partial class MixedGroupEditor : UserControl
	{
		public MixedGroupEditor()
		{
			InitializeComponent();
		}

		public void ReadGroup(MixedGroup grp)
		{
			m_labelTextBox.Text = grp.Label;
			m_listBox.Items.Clear();
			foreach (IRebuild rb in grp)
				m_listBox.Items.Add(rb);
		}
		public void WriteGroup(MixedGroup grp)
		{
			grp.Label = m_labelTextBox.Text;
		}

		public event ObjectSelected AfterSelect;

		private void m_listBox_DoubleClick(object sender, EventArgs e)
		{
			if (m_listBox.SelectedItem == null || !(m_listBox.SelectedItem is IRebuild))
				return;
			if (AfterSelect != null)
				AfterSelect(this, new EventArgs<IRebuild>(m_listBox.SelectedItem as IRebuild));
		}
	}
}
