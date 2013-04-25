using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public partial class CurveGroupEditor : UserControl
	{
		public CurveGroupEditor()
		{
			InitializeComponent();
		}
		public CurveGroupEditor(CurveGroup group)
		{
			InitializeComponent();
			m_group = group;
			Label = group.Label;
			Count = group.Count;
			List<MouldCurve> curves = group.GetAllCurves().ToList();
			curves.ForEach(c => { m_list.Items.Add(c); });
		}

		CurveGroup m_group = null;

		public string Label
		{
			set { m_labelTextBox.Text = value; }
			get { return m_labelTextBox.Text; }
		}
		public int Count
		{
			set { m_count.Text = string.Format("Count: {0}", value); }
			get { return m_list.Items.Count; }
		}
		public MouldCurve this[int i]
		{
			get { return m_list.Items[i] as MouldCurve; }
			set
			{
				if (m_list.Items.Count > i)
					m_list.Items[i] = value;
				else
					m_list.Items.Add(value);
			}
		}

		public event ObjectSelected AfterSelect;

		private void m_list_DoubleClick(object sender, EventArgs e)
		{
			MouldCurve mc = m_list.SelectedItem as MouldCurve;
			if (mc != null && AfterSelect != null)
			{
				AfterSelect(this, new EventArgs<IRebuild>(mc));
			}
		}

		private void m_list_KeyUp(object sender, KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.Delete)
			//{
			//	if (m_list.SelectedItem != null)
			//	{
			//		m_list.Items.Remove(m_list.SelectedItem);	
			//	}
			//}
			Count = m_list.Items.Count;
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		public bool Editable
		{
			get
			{
				return m_labelTextBox.Enabled;
			}
			set
			{
				m_labelTextBox.Enabled = value;
				m_list.Enabled = value;
			}
		}
	}
}
