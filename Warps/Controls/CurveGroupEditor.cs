﻿using System;
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
			//m_grid.LabelEdit = true;
			//m_grid.MultiSelect = false;
			m_grid.View = View.Details;
			m_group = group;
			Label = group.Label;
			Count = group.Count;
			group.ForEach(c => { this[m_grid.Items.Count] = c; });
		}

		CurveGroup m_group = null;

		public string Label
		{
			set { m_labelTextBox.Text = value; }
			get { return m_labelTextBox.Text; }
		}
		public int Count
		{
			set { m_count.Text = value.ToString("###"); }
		}
		public MouldCurve this[int i]
		{
			get { return m_grid.Items[i].Tag as MouldCurve; }
			set
			{
				//add a new item if necessary
				if (m_grid.Items.Count <= i)
				{
					i = m_grid.Items.Count;
					m_grid.Items.Add(new ListViewItem(value.Label));
				}
				else
					m_grid.Items[i] = new ListViewItem(value.Label);

				m_grid.Items[i].Name = value.Label;
				m_grid.Items[i].Tag = value;
				m_grid.Items[i].SubItems.Add(value.FitPoints.Length.ToString("###"));
				m_grid.Items[i].SubItems.Add(value.Length.ToString("f4"));
				StringBuilder segs = new StringBuilder();
				for (int seg = 0; seg < value.FitPoints.Length - 1; seg++)
				{
					segs.Append(value.IsGirth(seg) ? "-" : "~");
				}
				m_grid.Items[i].SubItems.Add(segs.ToString());
			}
		}

		MouldCurve SelectedCurve
		{
			get { return m_grid.SelectedItems.Count > 0 ? m_grid.SelectedItems[0].Tag as MouldCurve : null; }
		}

		public event ObjectSelected AfterSelect;

		private void m_list_DoubleClick(object sender, EventArgs e)
		{
			MouldCurve mc = SelectedCurve;
			if (mc != null && AfterSelect != null)
			{
				AfterSelect(this, new EventArgs<IRebuild>(mc));
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			//resize columm headers to spread nicely
			m_girCol.Width = Math.Max(50,(int)((double)m_grid.Width * 0.25));
			m_lngthCol.Width = Math.Max(50,(int)((double)m_grid.Width * 0.25));
			m_fitsCol.Width = Math.Max(50, (int)((double)m_grid.Width * 0.20));
			m_lblCol.Width = Math.Max(50, (int)((double)m_grid.Width * 0.30));
		}
	}
}
