using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Tapes
{
	public partial class TapeGroupEditor : UserControl
	{
		public TapeGroupEditor(TapeGroupTracker tracker)
		{
			InitializeComponent();
			m_tracker = tracker;
			tapeLen.ReadOnly = true;
			tapeCount.ReadOnly = true;
			BAK = selectWarpButt.BackColor;
			selectWarpButt.Tag = "Warps";
			selectGuideButt.Tag = "GuideSurface";
		}
		Color BAK, SEL = Color.SeaGreen;
		TapeGroupTracker m_tracker;
		public void ReadGroup(TapeGroup group)
		{
			m_labelTextBox.Text = group.Label;

			m_warpListView.Clear();
			group.Warps.ForEach(wrp => 
				AddRemoveWarp(wrp));

			SetGuide(group.DensityMap);

			pixLength.Value = group.PixelLength;
			chainTol.Value = group.ChainTolerance;
			angleTol.Value = group.AngleTolerance;

			tapeCount.Text = group.Count.ToString();
			tapeLen.Text = group.TotalLength.ToString("f3");
		}
		public void WriteGroup(TapeGroup group)
		{
			//update label
			group.Label = m_labelTextBox.Text;

			//clear and update warps
			group.Warps.Clear();
			if (group.Sail != null && m_warpListView.Items.Count > 0)
				for (int i = 0; i < m_warpListView.Items.Count; i++)
					group.Warps.Add(group.Sail.FindCurve(m_warpListView.Items[i].Name));

			IRebuild surf = group.Sail.FindItem(m_guideListView.Items[0].Name);
			group.DensityMap = surf as GuideSurface;

			group.PixelLength = pixLength.Value;
			group.ChainTolerance = chainTol.Value;
			group.AngleTolerance = angleTol.Value;
		}

		private void selectWarpButt_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			if (b.BackColor == SEL)
			{
				b.BackColor = BAK;
				m_tracker.SelectMode(null);
			}
			else
			{
				b.BackColor = SEL;
				m_tracker.SelectMode(b.Tag as string);
			}
		}

		internal bool AddRemoveWarp(MouldCurve curve)
		{
			if (m_warpListView.Items.ContainsKey(curve.Label))
			{
				m_warpListView.Items.RemoveByKey(curve.Label);
				m_warpListView.Refresh();
				return false;
			}
			else
			{
				m_warpListView.Items.Add(curve.Label, curve.Label, curve.GetType().Name);
				m_warpListView.Refresh();
				return true;
			}
		}

		internal string SetGuide(GuideSurface guideSurface)
		{
			string old = null;
			if (m_guideListView.Items.Count > 0)
				old = m_guideListView.Items[0].Name;
			m_guideListView.Clear();
			m_guideListView.Items.Add(guideSurface.Label, guideSurface.Label, guideSurface.GetType().ToString());
			return old;
		}
	}
}
