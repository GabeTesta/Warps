using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using Warps.Controls;
using Warps.Panels;
using Warps.Curves;

namespace Warps.Panels
{
	class PanelGroupTracker: ITracker
	{
		public PanelGroupTracker(PanelGroup group)
		{
			m_group = group;
			m_edit = new PanelGroupEditor(m_group);// m_group.Editor;
		}

		#region Members

		WarpFrame m_frame;
		PanelGroup m_group;
		PanelGroupEditor m_edit;

		PanelGroup Group
		{
			get { return m_group; }
			set { m_group = value; }
		}
		PanelGroupEditor Edit
		{
			get { return m_edit; }
		}

		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}
		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

		#endregion

		public bool IsTracking { get { return m_edit.IsWarp || m_edit.IsGuide; } }

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_frame.EditorPanel = Edit;
			}


			if (View != null)
			{
				View.AttachTracker(this);
				Edit.View = View;
			}

		}

		public void Cancel()
		{
			m_frame.EditorPanel = null;
			View.SetTrackerSelectionMode(null);
			View.DetachTracker(this);
		}

		public void OnBuild(object sender, EventArgs e)
		{
			Edit.IsWarp = false;
			Edit.IsGuide = false;

			View.SetTrackerSelectionMode(null);
			OnPreview(sender, null);
			Group.Label = Edit.GroupLabel;
			Group.Bounds = Edit.SelectedBounds;
			Group.PanelWidth = Edit.PanelWidth;
			Group.Guides = Edit.Guides;
			Group.ClothAlignment = Edit.Orientation;
			if (sender != null)
				m_frame.Rebuild(Group);//returns false if AutoBuild is off

			//View.Refresh();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}
		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}
		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void ProcessSelection(object selected)
		{
			//if (e is TreeViewEventArgs)
			//	selected = (e as TreeViewEventArgs).Node.Tag;
			//else if (e is EventArgs<object>)
			//	selected = (e as EventArgs<object>).Value;

			if (selected == null)
				return;
			//object selected = 

			if (Edit.IsGuide)
			{
				if (Edit.AddRemoveGuide(selected as MouldCurve))
					View.Select(selected as MouldCurve);
				else
					View.DeSelect(selected as MouldCurve);
			}
			else if (Edit.IsWarp)
			{
				if (Edit.AddRemoveWarp(selected as MouldCurve))
					View.Select(selected as MouldCurve);
				else
					View.DeSelect(selected as MouldCurve);
			}

			//View.DeSelectAll();

			//foreach (IMouldCurve cur in Edit.Curves)
			//	View.Select(cur);


			View.Refresh();
		}


	}
}
