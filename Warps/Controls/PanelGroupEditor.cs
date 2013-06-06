using System;
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

			ImageList imgList2 = new ImageList();
			imgList2.Images.Add("GuideComb", Warps.Properties.Resources.GuideComb);

			m_guideListView.SmallImageList = imgList2;
			m_guideListView.LargeImageList = imgList2;
			m_guideListView.StateImageList = imgList2;

			PanGroup = group;
			sail = PanGroup.Sail;

			widthEQB.Prep(m_sail, PanGroup);
			widthEQB.Text = PanGroup.PanelWidth != null ? PanGroup.PanelWidth.EquationText : "1.0";

			fillEditorWithData();
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

		PanelGroup m_group;
		Sail m_sail = null;

		public Sail sail
		{
			get { return m_sail; }
			set
			{
				m_sail = value;
			}
		}

		public PanelGroup PanGroup
		{
			get { return m_group; }
			set { m_group = value; }
		}

		DualView m_view = null;

		public DualView View
		{
			get { return m_view; }
			set { m_view = value; }
		}

		public List<MouldCurve> Guides
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
				if (m_group.Sail != null && m_guideListView.Items.Count > 0)
				{
					for (int i = 0; i < m_guideListView.Items.Count; i++)
						ret.Add(PanGroup.Sail.FindCurve(m_guideListView.Items[i].Name));
				}

				return ret;
			}
		}

		public List<MouldCurve> SelectedBounds
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
				if (PanGroup.Sail != null && m_warpListView.Items.Count > 0)
				{
					for (int i = 0; i < m_warpListView.Items.Count; i++)
						ret.Add(PanGroup.Sail.FindCurve(m_warpListView.Items[i].Name));

				}

				return ret;
			}
		}

		public List<MouldCurve> Curves
		{
			get
			{
				List<MouldCurve> ret = new List<MouldCurve>();
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
			m_labelTextBox.Text = PanGroup.Label;
			populateWarpBox();
			if (PanGroup.Guides != null)
			{
				PanGroup.Guides.ForEach(guide =>
				{
					m_guideListView.Items.Add(guide.Label, guide.Label, "GuideComb");
				});
			}

			m_orientationList.DataSource = null;
			m_orientationList.DataSource = System.Enum.GetValues(typeof(PanelGroup.ClothOrientations));
			Orientation = PanGroup.ClothAlignment;
			//outputs
			//AchievedDPI = PanGroup.AchievedDpi;
			//AchievedYarnCount = PanGroup.Count;

			//populateDensityCurveLocationBox(PanGroup.DensityPos);
		}
		public void populateWarpBox()
		{
			m_warpListView.Items.Clear();
			PanGroup.Bounds.ForEach(wrp => m_warpListView.Items.Add(wrp.Label, wrp.Label, wrp.GetType().Name));
			//availableCurves.ForEach(cur => m_warpSelectionCheckbox.Items.Add(cur, m_group.Warps.Contains(cur)));
		}

		bool m_selectingWarp = false;
		bool m_selectingGuide = false;

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

		public bool AddRemoveGuide(GuideComb guide)
		{
			if (!m_selectingGuide)
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

		public void Done()
		{
			m_selectingWarp = false;

			selectWarpButt.BackColor = m_selectingWarp ? Color.Green : Color.White;

			m_selectingGuide = false;
			selectGuideButt.BackColor = m_selectingGuide ? Color.Green : Color.White;
		}
		
	}

	
}
