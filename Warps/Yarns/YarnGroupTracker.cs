﻿using System;
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
using Warps.Logger;

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

				if (Tree != null)
				{
					Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
					Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
					Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				}
				
			}

			if (View != null)
			{
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
			//View.SelectLayer(m_group);
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
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.TreeContextMenu.Items.Count; i++)
			{
				if (Tree.TreeContextMenu.Items[i].Text == "Paste")
					Tree.TreeContextMenu.Items[i].Enabled = Utilities.GetClipboardObjType() == typeof(YarnGroup);
				if (Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("add"))
					Tree.TreeContextMenu.Items[i].Enabled = false; //can't add anything to yarns
				if(Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("delete"))
					Tree.TreeContextMenu.Items[i].Enabled = EditMode; 
			}

			Tree.TreeContextMenu.Show();
		}

		void TreeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			logger.Instance.Log("{0}: ContextMenuItem clicked {1}", this.GetType().Name, e.ClickedItem.Name);
			
			if (e.ClickedItem.Text == "Paste")
			{
				OnPaste(sender, new EventArgs());
			}
			else if (e.ClickedItem.Text == "Delete")
			{
				OnDelete(sender, new EventArgs());
			}
			else if (e.ClickedItem.Text == "Add")
			{
				OnAdd(sender, new EventArgs());
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

		public void OnDelete(object sender, EventArgs e)
		{
			if (!EditMode)
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

		public void OnAdd(object sender, EventArgs e) { }

		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}

		private void Cancel()
		{
			m_frame.EditorPanel = null;
			View.SetActionMode(devDept.Eyeshot.actionType.None);
			Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;
			Tree.KeyUp -= Tree_KeyUp;

			Tree.DetachTracker(this);

			if(View!=null)
				View.DetachTracker(this);

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
			yarGroup.EndCondition = Edit.Ending;
			if (sender != null)
				m_frame.Rebuild(yarGroup);//returns false if AutoBuild is off
			Edit.AchievedDPI = yarGroup.AchievedDpi;
			Edit.AchievedYarnCount = yarGroup.Count;
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
					View.SelectEntity(selected as MouldCurve);	
				else
					View.DeSelect(selected as MouldCurve);	
			}

			View.DeSelectAll();

			foreach (MouldCurve cur in Edit.Curves)
				View.SelectEntity(cur);


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

		public void OnPaste(object sender, EventArgs e)
		{

		}

		void SetFrameStatus(string status)
		{
			if (m_frame != null)
				m_frame.Status = status;
		}
	}
}
