using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Panels;
using Warps.Curves;

namespace Warps.Panels
{
	public partial class PanelGroupEditor : UserControl
	{
		public PanelGroupEditor(PanelGroup group)
		{
			InitializeComponent();

			ImageList imageList = new ImageList();
			imageList.Images.Add("MouldCurve", Warps.Properties.Resources.glyphicons_098_vector_path_curve);
			imageList.Images.Add("Geodesic", Warps.Properties.Resources.glyphicons_097_vector_path_line);

			m_warpListView.SmallImageList = imageList;
			m_warpListView.LargeImageList = imageList;
			m_warpListView.StateImageList = imageList;

			//ImageList imgList2 = new ImageList();
			//imgList2.Images.Add("GuideComb", Warps.Properties.Resources.GuideComb);

			m_guideListView.SmallImageList = imageList;
			m_guideListView.LargeImageList = imageList;
			m_guideListView.StateImageList = imageList;

			PanGroup = group;

			widthEQB.Prep(WarpFrame.CurrentSail, PanGroup);
			widthEQB.Text = PanGroup.PanelWidth != null ? PanGroup.PanelWidth.EquationText : "1.0";
			outputGroup.Enabled = false;
			fillEditorWithData();

			BAK = selectWarpButt.BackColor;
			selectWarpButt.Tag = "Warps";
			selectGuideButt.Tag = "GuideSurface";
		}
		Color BAK, SEL = Color.SeaGreen;


		PanelGroup m_group;
		DualView m_view = null;

		public PanelGroup PanGroup
		{
			get { return m_group; }
			set { m_group = value; }
		}

		public DualView View
		{
			get { return m_view; }
			set { m_view = value; }
		}

		public string GroupLabel
		{
			get { return m_labelTextBox.Text; }
			set { m_labelTextBox.Text = value; }
		}
		public List<IMouldCurve> SelectedBounds
		{
			get
			{
				List<IMouldCurve> ret = new List<IMouldCurve>();
				if (PanGroup.Sail != null && m_warpListView.Items.Count > 0)
				{
					for (int i = 0; i < m_warpListView.Items.Count; i++)
						ret.Add(PanGroup.Sail.FindCurve(m_warpListView.Items[i].Name));

				}

				return ret;
			}
		}
		public List<IMouldCurve> Guides
		{
			get
			{
				List<IMouldCurve> ret = new List<IMouldCurve>();
				if (m_group.Sail != null && m_guideListView.Items.Count > 0)
				{
					for (int i = 0; i < m_guideListView.Items.Count; i++)
						ret.Add(PanGroup.Sail.FindCurve(m_guideListView.Items[i].Name));
				}

				return ret;
			}
		}
		public List<IMouldCurve> Curves
		{
			get
			{
				List<IMouldCurve> ret = new List<IMouldCurve>();
				ret.AddRange(SelectedBounds);
				if (Guides != null)
					ret.AddRange(Guides);
				return ret;
			}
		}
		public Equation PanelWidth
		{
			get
			{
				return widthEQB.Equation;
			}
		}
		public PanelGroup.ClothOrientations Orientation
		{
			get { return (PanelGroup.ClothOrientations)m_orientationList.SelectedValue; }
			set { m_orientationList.Text = value.ToString(); }
		}

		public void fillEditorWithData()
		{
			GroupLabel = PanGroup.Label;
			m_warpListView.Items.Clear();
			PanGroup.Bounds.ForEach(wrp => m_warpListView.Items.Add(wrp.Label, wrp.Label, wrp.GetType().Name));
			if (PanGroup.Guides != null)
			{
				PanGroup.Guides.ForEach(guide =>
				{
					m_guideListView.Items.Add(guide.Label, guide.Label, guide.GetType().Name);
				});
			}

			m_orientationList.DataSource = null;
			m_orientationList.DataSource = System.Enum.GetValues(typeof(PanelGroup.ClothOrientations));
			Orientation = PanGroup.ClothAlignment;

			widthEQB.Equation = PanGroup.PanelWidth;
			//outputs
			//AchievedDPI = PanGroup.AchievedDpi;
			//AchievedYarnCount = PanGroup.Count;

			//populateDensityCurveLocationBox(PanGroup.DensityPos);
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

		public bool AddRemoveGuide(MouldCurve guide)
		{
			if (!IsGuide)
				return false;
			if (m_guideListView.Items.ContainsKey(guide.Label))
			{
				m_guideListView.Items.RemoveByKey(guide.Label);
				m_guideListView.Refresh();
				return false;
			}
			else
			{
				m_guideListView.Items.Add(guide.Label, guide.Label, guide.GetType().Name);
				m_guideListView.Refresh();
				return true;
			}
		}

		private void selectWarpButt_Click(object sender, EventArgs e)
		{
			IsWarp = !IsWarp;
			IsGuide = false;

			View.SetTrackerSelectionMode(IsWarp ? "warps" : null);
		}

		private void selectGuideButt_Click(object sender, EventArgs e)
		{
			IsGuide = !IsGuide;
			IsWarp = false;

			View.SetTrackerSelectionMode(IsGuide ? "warps" : null);
		}
	
	}

	
}
