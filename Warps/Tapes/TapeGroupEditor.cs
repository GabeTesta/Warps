﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Curves;

namespace Warps.Tapes
{
	public partial class TapeGroupEditor : UserControl
	{
		public TapeGroupEditor(TapeGroupTracker tracker)
		{
			InitializeComponent();
			tapeLen.ReadOnly = true;
			tapeCount.ReadOnly = true;
			BAK = selectWarpButt.BackColor;
			selectWarpButt.Tag = "Warps";
			selectGuideButt.Tag = "GuideSurface";
			SelectionMode += tracker.OnSelectionMode;

			m_tapeCombo.DataSource = WarpFrame.Mats.Materials(MaterialDatabase.TableTypes.Tapes);

		}
		Color BAK, SEL = Color.SeaGreen;
		event EventHandler<EventArgs<string>> SelectionMode;

		public string TapeMaterial
		{
			get { return m_tapeCombo.SelectedItem == null ? "" : m_tapeCombo.SelectedItem as string; }
			set { m_tapeCombo.SelectedItem = value; }
		}

		public void ReadGroup(TapeGroup group)
		{
			m_labelTextBox.Text = group.Label;

			TapeMaterial = group.TapeMaterial;

			m_warpListView.Clear();
			if (group.Warps != null)
				group.Warps.ForEach(wrp =>
					AddRemoveWarp(wrp));

			SetGuide(group.DensityMap);


			pixLength.Value = group.PixelLength;
			chainTol.Value = group.ChainTolerance;
			angleTol.Value = group.AngleTolerance;

			m_stagger.Checked = group.Stagger;

			tapeCount.Text = group.Count.ToString();
			tapeLen.Text = group.TotalLength.ToString("f3");
		}
		public void WriteGroup(TapeGroup group)
		{
			//update label
			group.Label = m_labelTextBox.Text;

			group.TapeMaterial = TapeMaterial;

			//clear and update warps
			group.Warps.Clear();
			if (group.Sail != null && m_warpListView.Items.Count > 0)
				for (int i = 0; i < m_warpListView.Items.Count; i++)
					group.Warps.Add(group.Sail.FindCurve(m_warpListView.Items[i].Name));

			IRebuild surf = WarpFrame.CurrentSail.FindItem(m_guideListView.Items[0].Name);
			group.DensityMap = surf as GuideSurface;

			group.PixelLength = pixLength.Equation.Evaluate(group.Sail);
			group.ChainTolerance = chainTol.Equation.Evaluate(group.Sail);
			group.AngleTolerance = angleTol.Equation.Evaluate(group.Sail);

			group.Stagger = m_stagger.Checked;
		}
		void SetSelectionMode(string mode)
		{
			if (SelectionMode != null)
				SelectionMode(this, new EventArgs<string>(mode));
		}
		private void selectWarpButt_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			if (b.BackColor == SEL)
			{
				b.BackColor = BAK;
				SetSelectionMode(null);
			}
			else
			{
				b.BackColor = SEL;
				SetSelectionMode(b.Tag as string);
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
			if (guideSurface != null)
				m_guideListView.Items.Add(guideSurface.Label, guideSurface.Label, guideSurface.GetType().ToString());
			return old;
		}

		public bool IsWarp
		{
			get { return selectWarpButt.BackColor == SEL; }
			set { selectWarpButt.BackColor = value ? SEL : BAK; }
		}
		public bool IsGuide
		{
			get { return selectGuideButt.BackColor == SEL; }
			set { selectGuideButt.BackColor = value ? SEL : BAK; }
		}

	}
}