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
using Logger;

namespace Warps.Tapes
{
	public class TapeGroupTracker : ITracker
	{
		public TapeGroupTracker(TapeGroup group)
		{
			m_group = group;
		}

		WarpFrame m_frame;
		TapeGroup m_group;
		TapeGroupEditor m_edit;

		TapeGroupEditor Edit
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

		#region ITracker Members

		public void Track(WarpFrame frame)
		{
			m_frame = frame;
			m_edit = new TapeGroupEditor(this);
			Edit.ReadGroup(m_group);

			if (m_frame != null && m_group != null)
			{
				//m_edit.sail = Sail;
				m_frame.EditorPanel = Edit;
				EditMode = m_frame.EditMode;

				//if (Tree != null)
				//{
				//	Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
				//	Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
				//	Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				//}

			}

			if (View != null)
				View.AttachTracker(this);

			if (Tree != null)
				Tree.AttachTracker(this);

			SelectGroup(m_group);
		}

		private void Cancel()
		{
			if( m_frame != null )
				m_frame.EditorPanel = null;

			if( Tree != null )
				Tree.DetachTracker(this);

			if (View != null)
			{
				View.DetachTracker(this);
				View.Remove(m_temp);
				View.DeSelect(m_group);
				View.StopSelect();
				View.Refresh();
			}
		}

		TapeGroup m_temp;
		Entity[][] m_tents;

		private void Preview()
		{
			if (m_temp == null)
				m_temp = new TapeGroup(m_group);
			Edit.WriteGroup(m_temp);
			UpdatePreview(true);
		}
		void UpdatePreview(bool bEditor)
		{
			m_temp.Update(Sail);
			List<Entity> verts = m_temp.CreateEntities();
			Parallel.ForEach(verts, e => { e.Color = Color.FromArgb(100,Color.LightSkyBlue); e.ColorMethod = colorMethodType.byEntity; });
			if (verts != null)
			{
				View.RemoveRange(m_tents);
				m_tents = View.AddRange(verts);
			}
				//for (int nEnt = 0; nEnt < m_tents.GetLength(0); nEnt++ )
				//	for (int i = 0; i < 2; i++)
				//		m_tents[nEnt][i].Vertices = verts[nEnt].Vertices;//copy vertices to both views

			View.Regen();
			View.Refresh();
			if (bEditor)
			{
				Edit.ReadGroup(m_temp);	//update editor's data with results
				m_edit.Update();
			}
		}
		private void SelectGroup(TapeGroup tapes)
		{
			if (tapes == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp);

			m_group = tapes;
			//copy the tape group
			m_temp = new TapeGroup(tapes);
			//add temporary entites to view
			m_tents = View.AddRange(m_temp.CreateEntities());

			foreach (Entity[] ents in m_tents)
				foreach (Entity ee in ents)
				{
					ee.Color = Color.FromArgb(100, Color.LightSkyBlue);
					ee.ColorMethod = colorMethodType.byEntity;
					if (ee.LineWeight == 1) ee.LineWeight = 2.0f;
					ee.LineWeightMethod = colorMethodType.byEntity;
				}

			//m_edit.AutoFill = Sail.Watermark(tapes).ToList<object>();
			m_edit.ReadGroup(m_temp);
			//m_edit.Label = Curve.Label;
			m_edit.Refresh();

			if (Tree.SelectedTag != m_group)
				Tree.SelectedTag = m_group;

			View.Select(m_group);
			View.Refresh();
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
			View.Select(m_group);
			//View.SelectLayer(m_group);
			if (m_group.Warps != null)
				m_group.Warps.ForEach(curve => View.Select(curve));

			View.Refresh();
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			if (e.Value == null)
				return;

			if (e.Value is TapeGroup)
			{
				if (m_group != null)
					return;//dont allow changing selection

				SelectGroup(e.Value as TapeGroup);
			}
			else if (e.Value is MouldCurve)
			{
				if (Edit.AddRemoveWarp(e.Value as MouldCurve))
					View.Select(e.Value as MouldCurve);
				else
					View.DeSelect(e.Value as MouldCurve);
			}
			else if (e.Value is GuideSurface)
			{
				string old = Edit.SetGuide(e.Value as GuideSurface);
				if (old != null)
				{
					GuideSurface s = Sail.FindItem(old) as GuideSurface;
					if( s!= null )
					View.DeSelect(s);
				}
			}



		}

		public void OnClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;
			//highlight the clicked tape in both the view and the tree
			int nEnt = View.ActiveView.GetEntityUnderMouseCursor(e.Location);
			if (nEnt > 0)
			{
				//check group tapes
				Entity one = View.ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == m_group);
				int nGrp = View.ActiveView.Entities.IndexOf(one);
				int nTp = nEnt - nGrp;
				if (nTp >= 0 && nTp < m_group.Count)
				{
					View.DeSelectAll();
					View.ActiveView.Entities[nEnt].Selected = true;
					Tree.ActiveTree.SelectedNode = m_group.m_node.Nodes[2].Nodes[nTp];
					Tree.ActiveTree.SelectedNode.EnsureVisible();
				}
				////check temp tapes
				//one = View.ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == m_temp);
				//nGrp = View.ActiveView.Entities.IndexOf(one);
				//nTp = nEnt - nGrp;
				//if (nTp >= 0 && nTp < m_temp.Count)
				//{
				//	View.DeSelectAll();
				//	View.ActiveView.Entities[nEnt].Selected = true;
				//	Tree.ActiveTree.SelectedNode = m_group.m_node.Nodes[2].Nodes[nTp];
				//	Tree.ActiveTree.SelectedNode.EnsureVisible();
				//}
			}
		}

		public void OnDown(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnMove(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnUp(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnPaste(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnDelete(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnAdd(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (!EditMode)
				return;

			Preview();//update the temp curve
			m_group.Fit(m_temp);//copy the temp group's data back
			m_group.Label = m_temp.Label;//update the label
			//Edit.WriteGroup(m_group);

			if (sender != null)
				m_frame.Rebuild(m_group);//returns false if AutoBuild is off

			//Edit.ReadGroup(m_group);
			//Edit.Refresh();
			//View.Refresh();
			Tree.Refresh();
		}

		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			Preview();
		}

		#endregion

		internal void SelectMode(string mode)
		{
			View.SetTrackerSelectionMode(mode);
		}
	}
}
