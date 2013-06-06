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

			YarGroup = group;
			sail = YarGroup.Sail;

			targetDPIEQB.Prep(m_sail, YarGroup);
			yarnDenierEQB.Prep(m_sail, YarGroup);
			yarnDenierEQB.Text = YarGroup.YarnDenierEqu != null ? YarGroup.YarnDenierEqu.EquationText : "0";
			targetDPIEQB.Text = YarGroup.TargetDenierEqu != null ? YarGroup.TargetDenierEqu.EquationText : "0";

			fillEditorWithData();
		}

		YarnGroup m_group;
		Sail m_sail = null;

		public Sail sail
		{
			get { return m_sail; }
			set
			{
				m_sail = value;
			}
		}

		public YarnGroup YarGroup
		{
			get { return m_group; }
			set { m_group = value; }
		}

		public double AchievedDPI
		{
			set { m_achievedDPI.Text = value.ToString("n"); }
		}

		public int AchievedYarnCount
		{
			set { m_yarnCountOut.Text = value.ToString(); }
		}

		DualView m_view = null;

		public DualView View
		{
			get { return m_view; }
			set { m_view = value; }
		}

		List<Vect2> m_guideCombVals = new List<Vect2>();

		public void fillEditorWithData()
		{
			m_labelTextBox.Text = YarGroup.Label;
			populateWarpBox();
			if (YarGroup.Guide != null)
				m_guideListView.Items.Add(YarGroup.Guide.Label, m_group.Guide.Label, "GuideComb");

			m_endingList.DataSource = null;
			m_endingList.DataSource = System.Enum.GetValues(typeof(YarnGroup.Ending));
			Ending = YarGroup.EndCondition;
			//outputs
			AchievedDPI = YarGroup.AchievedDpi;
			AchievedYarnCount = YarGroup.Count;

			populateDensityCurveLocationBox(YarGroup.DensityPos);
		}
		public YarnGroup.Ending Ending
		{
			get { return (YarnGroup.Ending)m_endingList.SelectedValue; }
			set { m_endingList.Text = value.ToString(); }
		}
		private void populateDensityCurveLocationBox(List<double> spos)
		{
			if (spos == null)
				m_densityLocTextBox.Text = "";
			List<string> sdens = new List<string>(spos.Count);
			spos.ForEach(s => sdens.Add(s.ToString("0.000")));
			m_densityLocTextBox.Text = string.Join("; ", sdens);
			//m_densityLocTextBox.Text = string.Join(", ", spos);
		}

		public void populateWarpBox()
		{
			m_warpListView.Items.Clear();
			YarGroup.Warps.ForEach(wrp => m_warpListView.Items.Add(wrp.Label, wrp.Label, wrp.GetType().Name));
			//availableCurves.ForEach(cur => m_warpSelectionCheckbox.Items.Add(cur, m_group.Warps.Contains(cur)));
		}

		public bool AddRemoveWarp(MouldCurve curve)
		{
			if (!m_selectingWarp)
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
		public void AddGuide(GuideComb guide)
		{
			if (!m_selectingGuide)
				return;

			m_guideListView.Items.Clear();

			m_guideListView.Items.Add(guide.Label, guide.Label, "GuideComb");

			m_guideListView.Refresh();
		}

		public void Done()
		{
			m_selectingWarp = false;

			selectWarpButt.BackColor = m_selectingWarp ? Color.Green : Color.White;

			m_selectingGuide = false;
			selectGuideButt.BackColor = m_selectingGuide ? Color.Green : Color.White;
		}

		public bool EditMode
		{

			get { return this.Enabled; }

			set
			{
				this.Enabled = value;
				outputGroup.Enabled = false; // always false
			}
		}

		bool m_selectingWarp = false;
		bool m_selectingGuide = false;
		private void button1_Click(object sender, EventArgs e)
		{
			m_selectingWarp = !m_selectingWarp;

			selectWarpButt.BackColor = m_selectingWarp ? Color.Green : Color.White;

			m_selectingGuide = false;
			selectGuideButt.BackColor = m_selectingGuide ? Color.Green : Color.White;


			View.SetTrackerSelectionMode(m_selectingWarp ? "warps" : null);
		}

		private void selectGuideButt_Click(object sender, EventArgs e)
		{
			m_selectingGuide = !m_selectingGuide;

			selectGuideButt.BackColor = m_selectingGuide ? Color.Green : Color.White;

			m_selectingWarp = false;

			selectWarpButt.BackColor = m_selectingWarp ? Color.Green : Color.White;

			View.SetTrackerSelectionMode(m_selectingGuide ? "guides" : null);
		}

		public Equation YarnDenierEqu
		{
			get
			{
				return yarnDenierEQB.Equation;
			}
		}

		public Equation TargetDPIEqu
		{
			get
			{
				return targetDPIEQB.Equation;
			}
		}

		public GuideComb Guide
		{
			get
			{
				if (m_group.Sail != null && m_guideListView.Items.Count > 0)
					return YarGroup.Sail.FindCurve(m_guideListView.Items[0].Name) as GuideComb;

				return null;
			}
		}

		public List<MouldCurve> SelectedWarps
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
				if (YarGroup.Sail != null && m_warpListView.Items.Count > 0)
				{
					for (int i = 0; i < m_warpListView.Items.Count; i++)
						ret.Add(YarGroup.Sail.FindCurve(m_warpListView.Items[i].Name));

				}

				return ret;
			}
		}

		public List<double> sPos
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
				populateDensityCurveLocationBox(value);
			}
		}

		private void m_warpListView_DoubleClick(object sender, EventArgs e)
		{
			if (m_warpListView.SelectedIndices.Count > 0)
			{
				foreach (var v in m_warpListView.SelectedIndices)
					m_warpListView.Items.RemoveAt(Convert.ToInt32(v));
				m_warpListView.Refresh();
			}
		}

		private void m_guideListView_DoubleClick(object sender, EventArgs e)
		{
			if (m_guideListView.SelectedIndices.Count > 0)
			{
				foreach (var v in m_guideListView.SelectedIndices)
					m_guideListView.Items.RemoveAt(Convert.ToInt32(v));
				m_guideListView.Refresh();
			}
		}

		public List<MouldCurve> Curves
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
				ret.AddRange(SelectedWarps);
				if(Guide!=null)
					ret.Add(Guide);
				return ret;
			}
		}
	}
}
