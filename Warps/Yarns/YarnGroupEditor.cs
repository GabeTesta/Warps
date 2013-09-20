using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Yarns;
using Warps.Curves;

namespace Warps.Controls
{
	public partial class YarnGroupEditor : UserControl
	{
		public YarnGroupEditor(YarnGroup group)
		{
			InitializeComponent();
			ImageList imageList = new ImageList();
			imageList.Images.Add("MouldCurve", Warps.Properties.Resources.glyphicons_098_vector_path_curve);
			imageList.Images.Add("Geodesic", Warps.Properties.Resources.glyphicons_097_vector_path_line);

			m_warpListView.SmallImageList = imageList;
			m_warpListView.LargeImageList = imageList;
			m_warpListView.StateImageList = imageList;

			ImageList imgList2 = new ImageList();
			imgList2.Images.Add("GuideComb", Warps.Properties.Resources.GuideComb);

			m_guideListView.SmallImageList = imgList2;
			m_guideListView.LargeImageList = imgList2;
			m_guideListView.StateImageList = imgList2;

			targetDPIEQB.Prep(group.Sail, group);
			yarnDenierEQB.Prep(group.Sail, group);

			m_endingList.DataSource = System.Enum.GetValues(typeof(YarnGroup.Ending));
			m_yarnCombo.DataSource = WarpFrame.Mats.Materials(MaterialDatabase.TableTypes.Yarns);

			BAK = selectWarpButt.BackColor;
		}
		Color BAK, SEL = Color.SeaGreen;
		DualView m_view = null;
		public DualView View
		{
			get { return m_view; }
			set { m_view = value; }
		}

		#region Properties

		public string Label
		{
			get { return m_labelTextBox.Text; }
			set { m_labelTextBox.Text = value; }
		}

		public List<MouldCurve> WarpCurves
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
				//	if (YarGroup.Sail != null && m_warpListView.Items.Count > 0)
				if (m_warpListView.Items.Count > 0)
				{
					for (int i = 0; i < m_warpListView.Items.Count; i++)
						ret.Add(WarpFrame.CurrentSail.FindCurve(m_warpListView.Items[i].Name));
				}
				return ret;
			}
			set
			{
				m_warpListView.Items.Clear();
				value.ForEach(wrp =>
					m_warpListView.Items.Add(wrp.Label, wrp.Label, wrp.GetType().Name));
			}
		}
		public GuideComb Guide
		{
			get
			{
				//if (m_group.Sail != null && m_guideListView.Items.Count > 0)
				//	return YarGroup.Sail.FindCurve(m_guideListView.Items[0].Name) as GuideComb;
				if (m_guideListView.Items.Count > 0)
					return WarpFrame.CurrentSail.FindCurve(m_guideListView.Items[0].Name) as GuideComb;

				return null;
			}
			set
			{
				m_guideListView.Clear();
				if( value != null)
					m_guideListView.Items.Add(value.Label, value.Label, "GuideComb");
			}
		}

		public Equation YarnDenierEqu
		{
			get
			{
				return yarnDenierEQB.Equation;
			}
			set
			{
				yarnDenierEQB.Equation = value;
			}
		}
		public Equation TargetDPIEqu
		{
			get
			{
				return targetDPIEQB.Equation;
			}
			set
			{
				targetDPIEQB.Equation = value;
			}
		}

		public List<double> DensityPos
		{
			get
			{
				List<double> ret = new List<double>();
				string[] split = m_densityLocTextBox.Text.Split(new char[] { ';' });
				double outie = -1;

				foreach (string s in split)
				{
					if (double.TryParse(s, out outie))
						ret.Add(outie);
				}

				return ret;
			}
			set
			{
				if (value == null)
					m_densityLocTextBox.Text = "";
				List<string> sdens = new List<string>(value.Count);
				value.ForEach(s => sdens.Add(s.ToString("0.000")));
				m_densityLocTextBox.Text = string.Join("; ", sdens);
			}
		}

		public YarnGroup.Ending Ending
		{
			get { return (YarnGroup.Ending)m_endingList.SelectedValue; }
			set { m_endingList.Text = value.ToString(); }
		}
		public string YarnMaterial
		{
			get { return m_yarnCombo.SelectedItem == null ? "" : m_yarnCombo.SelectedItem as string; }
			set { m_yarnCombo.SelectedItem = value; }
		}

		public double AchievedDPI
		{
			set { m_achievedDPI.Text = value.ToString("n"); }
		}
		public int AchievedYarnCount
		{
			set { m_yarnCountOut.Text = value.ToString(); }
		}
		
		#endregion

		#region Read/WriteGroup
		
		internal void ReadGroup(YarnGroup m_temp)
		{
			Label = m_temp.Label;

			WarpCurves = m_temp.Warps;
			Guide = m_temp.Guide;

			YarnDenierEqu = m_temp.YarnDenierEqu;
			TargetDPIEqu = m_temp.TargetDenierEqu;

			DensityPos = m_temp.DensityPos;
			Ending = m_temp.EndCondition;


			YarnMaterial = m_temp.YarnMaterial;
		}

		internal void WriteGroup(YarnGroup Group)
		{
			Group.Label = Label;

			Group.Warps = WarpCurves;
			Group.Guide = Guide;

			Group.YarnDenierEqu = YarnDenierEqu;
			Group.TargetDenierEqu = TargetDPIEqu;

			Group.DensityPos = DensityPos;
			Group.EndCondition = Ending;

			Group.YarnMaterial = YarnMaterial;
		}

 
		#endregion		
		
		#region Warps/Guide

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

		public bool AddRemoveWarp(MouldCurve curve)
		{
			if (!IsWarp)
				return false;

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
		public bool AddGuide(GuideComb guide)
		{
			if (!IsGuide)
				return false;

			m_guideListView.Items.Clear();
			m_guideListView.Items.Add(guide.Label, guide.Label, "GuideComb");
			m_guideListView.Refresh();

			return true;
		}

		private void selectGuideButt_Click(object sender, EventArgs e)
		{
			IsGuide = !IsGuide;
			IsWarp = false;
			//View.SetTrackerSelectionMode(IsGuide ? "guides" : null);
			View.ShowTypes(IsGuide ? typeof(GuideComb): null);
		}
		private void selectWarpsButt_Click(object sender, EventArgs e)
		{
			IsWarp = !IsWarp;
			IsGuide = false;
			View.ShowTypes(IsWarp ? typeof(MouldCurve) : null);
			//View.SetTrackerSelectionMode(IsWarp ? "warps" : null);
		}

		private void m_warpListView_DoubleClick(object sender, EventArgs e)
		{
			if (m_warpListView.SelectedIndices.Count > 0)
			{
				foreach (int v in m_warpListView.SelectedIndices)
					m_warpListView.Items.RemoveAt(v);
				m_warpListView.Refresh();
			}
		}
		private void m_guideListView_DoubleClick(object sender, EventArgs e)
		{
			if (m_guideListView.SelectedIndices.Count > 0)
			{
				foreach (int v in m_guideListView.SelectedIndices)
					m_guideListView.Items.RemoveAt(v);
				m_guideListView.Refresh();
			}
		}

		#endregion

	}
}
