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
			m_listView.LargeImageList = m_listView.SmallImageList = WarpFrame.Images;
		}

		public void ReadGroup(MixedGroup grp)
		{
			m_labelTextBox.Text = grp.Label;
			m_listView.Items.Clear();
			foreach (IRebuild rb in grp)
				m_listView.Items.Add(rb.ToString(), rb.GetType().Name);
		}
		public void WriteGroup(MixedGroup grp)
		{
			grp.Label = m_labelTextBox.Text;
		}

		public event ObjectSelected AfterSelect;

		private void m_listBox_DoubleClick(object sender, EventArgs e)
		{
			if (m_listView.SelectedItems == null || !(m_listView.SelectedItems[0] is IRebuild))
				return;
			if (AfterSelect != null)
				AfterSelect(this, new EventArgs<IRebuild>(m_listView.SelectedItems[0] as IRebuild));
		}
	}
}
