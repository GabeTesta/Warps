using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Warps.Controls;

namespace Warps
{
	public class VariableTracker : ITracker
	{
		public VariableTracker(Equation equ)
		{
			m_equ = equ;
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null)
			{
				var edit = new VariableEditor(m_equ.Label, m_equ);
				edit.sail = frame.ActiveSail;
				m_frame.EditorPanel = edit;


				if (Tree != null)
				{
					//Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
					//Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
					//Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				}

			}
		}


		WarpFrame m_frame;
		Equation m_equ;

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}

		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

		//void Tree_KeyUp(object sender, KeyEventArgs e)
		//{
		//	// the modifier key CTRL is pressed by the time it gets here
		//	switch (e.KeyCode)
		//	{
		//		//case Keys.C:
		//		//	OnCopy(Tree.SelectedTag, new EventArgs());
		//		//	break;
		//		case Keys.V:
		//			OnPaste(Tree.SelectedTag, new EventArgs());
		//			break;
		//	}
		//}
		//void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		//{
		//	for (int i = 0; i < Tree.TreeContextMenu.Items.Count; i++)
		//	{
		//		if (Tree.TreeContextMenu.Items[i].Text == "Paste")
		//			Tree.TreeContextMenu.Items[i].Enabled = false;
		//		if (Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("add"))
		//			Tree.TreeContextMenu.Items[i].Enabled = false;
		//		if(Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("delete"))
		//			Tree.TreeContextMenu.Items[i].Enabled = true;
					
		//	}
		//	Tree.TreeContextMenu.Show();
		//}

		//void TreeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		//{
		//	Logleton.TheLog.Log("{0}: ContextMenuItem clicked {1}", this.GetType().Name, e.ClickedItem.Name);

		//	//if (e.ClickedItem.Text == "Copy")
		//	//{
		//	//	OnCopy(sender, new EventArgs());
		//	//}
		//	if (e.ClickedItem.Text == "Paste")
		//	{
		//		OnPaste(sender, new EventArgs());
		//	}
		//	else if (e.ClickedItem.Text == "Delete")
		//	{
		//		OnDelete(sender, new EventArgs());
		//	}
		//	else if (e.ClickedItem.Text == "Add")
		//	{
		//		OnAdd(sender, new EventArgs());
		//	}
		//}

		public void Cancel()
		{
			m_frame.EditorPanel = null;
			//Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			//Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;
			//Tree.KeyUp -= Tree_KeyUp;
		}

		public void OnBuild(object sender, EventArgs e)
		{
			VariableEditor edit = m_frame.EditorPanel as VariableEditor;
			if (edit == null)
				return;
			m_equ.ReadEditor(edit);
			m_equ.WriteEditor(edit);//rewrite the editor to update the result
			m_equ.WriteNode();//update the tree
		}

		public void OnPreview(object sender, EventArgs e) 
		{
			VariableEditor edit = m_frame.EditorPanel as VariableEditor;
			if (edit == null)
				return;
			Equation temp = new Equation();
			temp.ReadEditor(edit);
			temp.WriteEditor(edit);
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e) { }

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e) { }


		//public void OnCopy(object sender, EventArgs e)
		//{
		//	Equation equ = Tree.SelectedTag as Equation;

		//	if (equ == null)
		//		return;
		//	//Lets say its my data format
		//	Clipboard.Clear();
		//	//Set data to clipboard
		//	Clipboard.SetData(equ.GetType().Name, Utilities.Serialize(equ.WriteScript()));
		//	//Get data from clipboard
		//	m_frame.Status = String.Format("{0}:{1} Copied", equ.GetType().Name, equ.Label);

		//}


		#region ITracker Members

		public bool IsTracking { get { return false; } }

		public void ProcessSelection(object Tag)
		{
			//throw new NotImplementedException();
		}

		#endregion
	}
}
