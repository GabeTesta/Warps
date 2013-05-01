using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Windows.Forms;
using System.Drawing;
using Warps.Controls;

namespace Warps.Yarns
{
	class YarnGroupTracker: ITracker
	{
		public YarnGroupTracker(YarnGroup group)
		{
			m_group = group;
			m_edit = new YarnGroupEditor(m_group);// m_group.Editor;
		}
		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_edit.sail = Sail;
				m_frame.EditorPanel = Edit;
				EditMode = m_frame.EditMode;

				AddContextMenu();
				Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
			}



			if (View != null)
			{
				View.DeSelectAll();
				View.AttachTracker(this);
				Edit.View = View;
			}

			if (Tree != null)
				Tree.AttachTracker(this);

		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; toggleEditMode(value); }
		}

		void toggleEditMode(bool state)
		{
			View.EditMode = state;
			m_edit.Enabled = state;
			View.DeSelectAllLayers();
			View.SelectLayer(m_group);
			foreach (MouldCurve curve in m_group.Warps)
				View.Select(curve);

			View.Refresh();
		}

		WarpFrame m_frame;
		YarnGroup m_group;

		public YarnGroup yarGroup
		{
			get { return m_group; }
			set { m_group = value; }
		}
		YarnGroupEditor m_edit;

		YarnGroupEditor Edit
		{
			get { return m_edit; }
		}

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}

		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}
		
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

		// handle copy pasting from keyboard here
		void Tree_KeyUp(object sender, KeyEventArgs e)
		{
			// the modifier key CTRL is pressed by the time it gets here
			switch (e.KeyCode)
			{
				case Keys.C:
					OnCopy(Tree.SelectedTag, new EventArgs());
					break;
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
			//	if (Tree.ContextMenuStrip.Items[i].Text == "Paste Group")
			//		Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsCurveType();
			Tree.ContextMenuStrip.Show();
		}

		ContextMenuStrip m_context = new ContextMenuStrip();

		private void AddContextMenu()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();

				Tree.ContextMenuStrip.Items.Add("Show Only", new Bitmap(Warps.Properties.Resources.showonly), OnShowOnlyClick);
				Tree.ContextMenuStrip.Items.Add("Visible", new Bitmap(Warps.Properties.Resources.SmallEye), OnVisibleToggleClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Color", new Bitmap(Warps.Properties.Resources.showonly), EditColorClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Delete", new Bitmap(Warps.Properties.Resources.glyphicons_191_circle_minus), DeleteClick);

				Tree.ContextMenuStrip.Opening += ContextMenuStrip_Opening;

			}

		}
		void EditColorClick(object sender, EventArgs e)
		{
			if (Tree.SelectedTag is IGroup)
			{
				Layer l = View.GetLayer((Tree.SelectedTag as IGroup));
				if (l == null)
					return;

				ColorWheelForm cwf = new ColorWheelForm(l);

				cwf.Show(m_frame);
			}
		}

		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			View.ToggleLayer(m_group);
		}
		void OnShowOnlyClick(object sender, EventArgs e)
		{
			View.ShowOnly(m_group);
		}
		void DeleteClick(object sender, EventArgs e)
		{
			if (EditMode)
				return;

			View.Remove(yarGroup);
			//CurveGroup g = Curve.Group as CurveGroup;
			//if (yarGroup != null)
			//{
				//g.Remove(yarGroup);		
			//}
			m_frame.Delete(yarGroup);
			//if (yarGroup != null)
			//	Tree.SelectedTag = yarGroup;
		}
		private void RemoveContextMenu()
		{
			if (Tree.ContextMenu != null)
			{
				Tree.ContextMenuStrip.Opening -= ContextMenuStrip_Opening;
				Tree.ContextMenuStrip.Items.Clear();
			}
			Tree.ContextMenuStrip = null;
		}

		public void OnCancel(object sender, EventArgs e)
		{
			
			Cancel();
			//throw new NotImplementedException();
		}

		private void Cancel()
		{
			
			m_frame.EditorPanel = null;

			RemoveContextMenu();
			Tree.KeyUp -= Tree_KeyUp; // handle ctrl-c ctrl-v
			Tree.DetachTracker(this);
			//if (Tree != null)
			//	Tree.AfterSelect += OnSelect;
			if(View!=null)
				View.DetachTracker(this);
			//if (m_temp != null)
			//	View.Remove(m_temp);
			//View.DeSelect(Comb);
			////View.StopSelect();
			//View.Refresh();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (!EditMode)
				return;

			Edit.Done();
			OnPreview(sender, null);
			yarGroup.Warps = Edit.SelectedWarps;
			yarGroup.YarnDenierEqu = Edit.YarnDenierEqu;
			yarGroup.TargetDenierEqu = Edit.TargetDPIEqu;
			yarGroup.Guide = Edit.Guide;
			yarGroup.DensityPos = Edit.sPos;

			if (sender != null)
				m_frame.Rebuild(yarGroup);//returns false if AutoBuild is off
			Edit.AchievedDPI = yarGroup.AchievedDpi;
			View.Refresh(); 
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			if (!EditMode)
				return;

			object selected = e.Value;

			//if (e is TreeViewEventArgs)
			//	selected = (e as TreeViewEventArgs).Node.Tag;
			//else if (e is EventArgs<object>)
			//	selected = (e as EventArgs<object>).Value;

			if (selected == null)
				return;
			//object selected = 

			if (selected is GuideComb)
			{
				Edit.AddGuide(selected as GuideComb);
				View.Select(selected as GuideComb);
			}
			else if (selected is MouldCurve)
			{
				if (Edit.AddRemoveWarp(selected as MouldCurve))
					View.Select(selected as MouldCurve);	
				else
					View.DeSelect(selected as MouldCurve);	
			}

			View.Refresh();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!EditMode)
				return;
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!EditMode)
				return;
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!EditMode)
				return;
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!EditMode)
				return;
		}

		public void OnPreview(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnCopy(object sender, EventArgs e)
		{
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(yarGroup.GetType().Name, Utilities.Serialize(yarGroup.WriteScript()));
			//Get data from clipboard
			m_frame.Status = String.Format("{0}:{1} Copied", yarGroup.GetType().Name, yarGroup.Label);
		}

		public void OnPaste(object sender, EventArgs e)
		{
			Type type = Utilities.GetClipboardObjType();

		}

		void SetFrameStatus(string status)
		{
			if (m_frame != null)
				m_frame.Status = status;
		}

		

	}
}
