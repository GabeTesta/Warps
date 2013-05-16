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
using Warps.Logger;

namespace Warps
{
	public class CurveGroupTracker : ITracker
	{
		public CurveGroupTracker(CurveGroup group)
		{
			m_group = group;
			m_edit = new CurveGroupEditor(m_group);
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_frame.EditorPanel = Edit;
				EditMode = m_frame.EditMode;

				//m_frame.okButton.Click += OnBuild;
				//m_frame.cancelButton.Click += OnCancel;
				//m_frame.previewButton.Click += OnPreview;

				if (Tree != null)
				{
					Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
					Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
					Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				}

				View.DeSelectAllLayers();
				View.SelectLayer(m_group);
				foreach (MouldCurve curve in m_group)
					View.Select(curve);

				View.Refresh();
			}

		}
		public void Cancel()
		{
			View.DeSelectAllLayers();
			foreach (MouldCurve curve in m_group)
				View.DeSelect(curve);

			View.Refresh();

			Tree.KeyUp -= Tree_KeyUp; // handle ctrl-c ctrl-v	
			Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;

			if (m_curveTracker != null)
			{
				m_curveTracker.Cancel();
				m_curveTracker = null;
			}
		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set
			{
				Edit.Enabled = value;

				m_editMode = value;
				if (View != null)
				{
					View.EditMode = value;
					View.DeSelectAllLayers();
					View.SelectLayer(m_group);
					foreach (MouldCurve curve in m_group)
						View.Select(curve);
					View.Refresh();
				}
				//if (Tree != null)
				//	Tree.EditMode = value;
				//if (value == false)
				//	Tree.DeSelect(m_group);

			}
		}

		// handle copy pasting from keyboard here
		void Tree_KeyUp(object sender, KeyEventArgs e)
		{
			// the modifier key CTRL is pressed by the time it gets here
			switch (e.KeyCode)
			{
				//case Keys.C:
				//	OnCopy(Tree.SelectedTag, new EventArgs());
				//	break;
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}

		#region TreePopup

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.TreeContextMenu.Items.Count; i++)
			{
				if (Tree.TreeContextMenu.Items[i].Text == "Paste")
					Tree.TreeContextMenu.Items[i].Enabled = (Utilities.GetClipboardObjType() == typeof(CurveGroup) || Utilities.GetClipboardObjType() == typeof(MouldCurve));
				if (Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("add") || Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("delete"))
					Tree.TreeContextMenu.Items[i].Enabled = EditMode;
			}

			Tree.TreeContextMenu.Show();
		}

		void TreeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			logger.Instance.Log("{0}: ContextMenuItem clicked {1}", this.GetType().Name, e.ClickedItem.Name);

			//if (e.ClickedItem.Text == "Copy")
			//{
			//	OnCopy(sender, new EventArgs());
			//}
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

		

		#endregion

		CurveTracker m_curveTracker = null;

		WarpFrame m_frame;
		CurveGroup m_group;
		CurveGroupEditor m_edit;
		CurveGroupEditor Edit
		{
			get { return m_edit; }
		}
		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}
		Sail sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}

		public void OnAdd(object sender, EventArgs e)
		{
			if (sail == null)
				return;

			List<Type> useThese = new List<Type>();

			useThese.Add(typeof(MouldCurve));
			useThese.Add(typeof(GuideComb));

			AddGroup dlg = new AddGroup(useThese);
			dlg.Name = "enter name";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IRebuild cur = dlg.CreateIRebuild();
				if (cur.GetType().Name == "GuideComb")
				{
					m_curveTracker = new CurveTracker(new GuideComb(dlg.Label, m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }, new Vect2[] { new Vect2(0, 1), new Vect2(1, 1) }));
					m_curveTracker.Track(m_frame);
				}
				else
				{
					m_curveTracker = new CurveTracker(new MouldCurve(dlg.Label, m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));
					m_curveTracker.Track(m_frame);
				}

			}
			//here we need to ask if we want a GuideComb or a normal curve

		}

		public void OnDelete(object sender, EventArgs e)
		{
			// HERE WE DELETE

			// Get clicked item (Curve)

			// List<IRebuild> ret = m_frame.Rebuild(Curve)
			// ret is invalid shit
			// call invalidate(ret)-> color themselves SHOW THEY ARE BROKEN
			// View entity keep previous state, color invalid byEntity, when fixed, byLayer

			for (int i = 0; i < m_group.Count; i++)
				View.Remove(m_group[i]);

			m_group.Clear();

			m_frame.Rebuild(m_group);
			m_frame.Delete(m_group);
			//m_frame.Rebuild(null);

		}

		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (!EditMode)
				return;

			if (Edit.Label != m_group.Label)
			{
				m_group.Label = Edit.Label;
				m_frame.Rebuild(m_group);
			}

			if (m_curveTracker != null)
			{
				//if tracker exists
				// add curve to group
				// call rebuild with the group
				// should update the tree and view

				//passing a null sender will skip the rebuild call
				m_curveTracker.OnBuild(null, e);//create the curve from the fitpoints

				m_group.Add(m_curveTracker.Curve);//add it to the group
				m_frame.Rebuild(m_group);//rebuild the objects and update the views
				m_curveTracker.Cancel();//destory the temporary curve tracker
				m_frame.EditorPanel = Edit;//restore the curve group editor that was displaced by the curve editor
			}

		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			//throw new NotImplementedException();
		}

		public void OnClick(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
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

		public void OnPreview(object sender, EventArgs e)
		{

			//throw new NotImplementedException();
		}

		//public void OnCopy(object sender, EventArgs e)
		//{
		//	//Lets say its my data format
		//	Clipboard.Clear();
		//	//Set data to clipboard
		//	Clipboard.SetData(m_group.GetType().Name, Utilities.Serialize(m_group.WriteScript()));
		//	//Get data from clipboard
		//	m_frame.Status = String.Format("{0}:{1} Copied", m_group.GetType().Name, m_group.Label);
		//	m_frame.ClearTracker();//clear tracker so user can select sail and paste it
		//}

		public void OnPaste(object sender, EventArgs e)
		{
			//mouldcurves and guidecombs should be pasted into here

			if (Utilities.GetClipboardObjType() != typeof(MouldCurve)
				&& Utilities.GetClipboardObjType() != typeof(GuideComb))
				return;

			Type type = Utilities.GetClipboardObjType();
			if (type == null)
				return;

			List<string> result = (List<string>)Utilities.DeSerialize(Clipboard.GetData(type.Name).ToString());

			ScriptTools.ModifyScriptToShowCopied(ref result);

			IGroup group = (IGroup)Tree.SelectedTag;

			if (group == null)
				return;
			if (type.Name == "CurveGroup")
				return;//can't paste a group into this group...
			try
			{
				object cur = Utilities.CreateInstance(result[0].Split(new char[] { ':' })[0].Trim());
				if (cur != null && cur is GuideComb)
				{
					(cur as GuideComb).Sail = group.Sail;
					(cur as GuideComb).ReadScript(group.Sail, result);
					(group as CurveGroup).Add(cur as GuideComb);
					m_frame.Status = String.Format("{0}:{1} Pasted into {2}:{3} From Clipboard", cur.GetType().Name, (cur as MouldCurve).Label, group.GetType().Name, group.Label);
				}
				else if (cur != null && cur is MouldCurve)
				{
					(cur as MouldCurve).Sail = group.Sail;
					(cur as MouldCurve).ReadScript(group.Sail, result);
					(group as CurveGroup).Add(cur as MouldCurve);
					m_frame.Status = String.Format("{0}:{1} Pasted into {2}:{3} From Clipboard", cur.GetType().Name, (cur as MouldCurve).Label, group.GetType().Name, group.Label);
				}
			}
			catch (Exception ex) { Warps.Logger.logger.Instance.LogErrorException(ex); return; }
			m_frame.Rebuild(group);
		}

		public bool IsTracking(object obj)
		{
			return obj == m_group;
		}
	}
}