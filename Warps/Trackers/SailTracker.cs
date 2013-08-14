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
using Logger;

namespace Warps
{
	public class SailTracker : ITracker
	{
		public SailTracker(bool mode)
		{
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null)
			{
				m_frame = frame;
				m_frame.EditorPanel = null;

				if (Tree != null)
				{
					Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
					Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
					Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				}

				View.DeSelectAllLayers();
				View.AttachTracker(this);
			}
			//View.ShowAll();
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

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
			//	if (Tree.ContextMenuStrip.Items[i].Text == "Paste Group")
			//		Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsCurve();
			Tree.TreeContextMenu.Show();
			//Tree.ContextMenuStrip.Items["Paste Curve"].Enabled = ClipboardContainsCurve();
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

		WarpFrame m_frame;

		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; }
		}

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

		#region ITracker Stuff

		public void OnCancel(object sender, EventArgs e)
		{
			Tree.KeyUp -= Tree_KeyUp;
			Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;
		}

		public void OnAdd(object sender, EventArgs e) { }

		public void OnDelete(object sender, EventArgs e) { }

		public void OnBuild(object sender, EventArgs e) { }

		public void OnSelect(object sender, EventArgs<IRebuild> e) { }

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnPreview(object sender, EventArgs e) { }

	//	public void OnCopy(object sender, EventArgs e) { }

		public void OnPaste(object sender, EventArgs e)
		{
			//any Igroup should be pasteable into here

			Type type = Utilities.GetClipboardObjType();
			if (type == null || type.IsSubclassOf(typeof(IGroup)))
				return;

			List<string> result = (List<string>)Utilities.DeSerialize(Clipboard.GetData(type.Name).ToString());

			string groupname = result[0].Replace(type.Name, "");
			groupname = groupname.Replace(":", "");
			groupname = groupname.TrimStart();
			groupname = groupname.TrimEnd();

			if (Sail.FindGroup(groupname) != null) // doesn't have this group
				ScriptTools.ModifyScriptToShowCopied(ref result);

			IGroup g = Utilities.CreateInstance(type) as IGroup;

			if (g != null)
			{
				g.Sail = Sail;
				g.ReadScript(Sail, result);

				Sail.Add(g);
				//g.Rebuild(null);
				m_frame.Rebuild(g);
				Sail.WriteNode();
			}
		}

		#endregion ITracker Stuff

	}
}
