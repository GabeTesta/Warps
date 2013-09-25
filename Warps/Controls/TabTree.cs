using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Warps.Curves;
using Warps.Tapes;
using Warps.Yarns;
using Warps.Panels;

namespace Warps
{
	public partial class TabTree : UserControl
	{
		public TabTree()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			SeqTree.ShowNodeToolTips = true;
			SorTree.ShowNodeToolTips = true;
			//SeqTree.AfterSelect += Tree_AfterSelect;
			//SorTree.AfterSelect += Tree_AfterSelect;

			BuildContextMenu();

			SeqTree.ImageList = WarpFrame.Images;
			SorTree.ImageList = WarpFrame.Images;

			SeqTree.LabelEdit = false;
		}

		#region BeforeSelect

		/// <summary>
		/// Raised before the tree attempts to change the selected node, can be canceled
		/// </summary>
		public event TreeViewCancelEventHandler BeforeSelect;
		//public event ObjectSelected AfterSelect;

		private void m_seqtree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Action != TreeViewAction.Unknown)
				RaiseBeforeSelect(e);
		}
		private void RaiseBeforeSelect(TreeViewCancelEventArgs e)
		{
			if (BeforeSelect != null)
				BeforeSelect(this, e);
		}
		
		#endregion

		#region Trackers
		
		public void AttachTracker(ITracker tracker)
		{
			//AfterSelect += tracker.OnSelect;
			//BeforeSelect += tracker.OnSelecting;

			TreeContextMenu.Items[0].Click += tracker.OnAdd;
			TreeContextMenu.Items[1].Click += tracker.OnDelete;

			//TreeContextMenu.Items[3].Click += tracker.OnCopy;//handled here
			TreeContextMenu.Items[4].Click += tracker.OnPaste;


			//SeqTree.AfterSelect += tracker.OnSelect;
			//SorTree.AfterSelect += tracker.OnSelect;
		}
		public void DetachTracker(ITracker tracker)
		{
			//AfterSelect -= tracker.OnSelect;
			//BeforeSelect -= tracker.OnSelecting;
			TreeContextMenu.Items[0].Click -= tracker.OnAdd;
			TreeContextMenu.Items[1].Click -= tracker.OnDelete;

			//TreeContextMenu.Items[3].Click -= tracker.OnCopy;//handled here
			TreeContextMenu.Items[4].Click -= tracker.OnPaste;

			//SeqTree.AfterSelect -= tracker.OnSelect;
			//SorTree.AfterSelect -= tracker.OnSelect;
		}
 
		#endregion

		#region Trees

		public TreeView ActiveTree
		{
			get { return m_tab.SelectedIndex == 0 ? m_seqtree : m_sortree; }
		}
		public TreeView SeqTree
		{
			get { return m_seqtree; }
		}
		public TreeView SorTree
		{
			get { return m_sortree; }
		}

		public Sail Sail
		{
			get
			{
				if (SeqTree.Nodes.Count > 0)
					return SeqTree.Nodes[0].Tag as Sail;
				return null;

			}
		}

		#endregion

		#region ExpandToDepth

		public void ExpandToDepth(int depth)
		{
			SeqTree.BeginUpdate();
			SeqTree.CollapseAll();
			if (depth > -1)
			{
				foreach (TreeNode tn in SeqTree.Nodes)
					RecursiveExpand(tn, depth);
			}
			SeqTree.EndUpdate();
		}

		void RecursiveExpand(TreeNode tn, int depth)
		{
			tn.Expand();
			if (depth == 0)
				return;
			else
				foreach (TreeNode child in tn.Nodes)
					RecursiveExpand(child, depth - 1);
		}

		#endregion

		#region Tags

		public object SelectedTag
		{
			get { return FindTag(ActiveTree.SelectedNode); }
			set
			{
				TreeNode tn = FindNode(value);
				if (tn != null)
				{
					tn.TreeView.SelectedNode = tn;
					tn.EnsureVisible();
				}
			}
		}

		internal void SelectTag(IRebuild grp)
		{
			SelectedTag = grp;
			if (ActiveTree.SelectedNode != null)
				RaiseBeforeSelect(new TreeViewCancelEventArgs(ActiveTree.SelectedNode, false, TreeViewAction.Unknown));
		}
		object FindTag(TreeNode tn)
		{
			if (tn == null)
				return null;
			if (tn.Tag != null)
				return tn.Tag;
			return FindTag(tn.Parent);
		}
		IRebuild FindItem(TreeNode node)
		{
			if (node == null)
				return null;
			if (node.Tag != null && node.Tag is IRebuild)
				return node.Tag as IRebuild;
			return FindItem(node.Parent);	
		}
		IGroup FindParentGroup(TreeNode node)
		{
			while (node.Parent != null && !(node.Parent.Tag is IGroup))
				node = node.Parent;
			return node.Parent != null ? node.Parent.Tag as IGroup : null;
		}

		TreeNode FindNode(object tag)
		{
			return FindNode(tag, null);
		}
		TreeNode FindNode(object tag, TreeNodeCollection nodes)
		{
			if (tag == null)
				return null;
			if (nodes == null)
				nodes = ActiveTree.Nodes;
			foreach (TreeNode tn in nodes)
			{
				if (tn.Tag == tag)
					return tn;
				TreeNode tre = FindNode(tag, tn.Nodes);
				if (tre != null)
					return tre;
			}
			return null;
		}

		TreeNode FindAllNodesBeginningWith(string key, TreeNodeCollection nodes, ref List<TreeNode> found)
		{
			if (key == null)
				return null;
			if (nodes == null)
				nodes = ActiveTree.Nodes;
			foreach (TreeNode tn in nodes)
			{
				if (tn.Tag == null)
					continue;
				if (tn.Tag is IGroup)
				{
					if ((tn.Tag as IGroup).Label.ToLower().StartsWith(key.ToLower()))
						found.Add(tn);
				}

				TreeNode tre = FindAllNodesBeginningWith(key, tn.Nodes, ref found);

				if (tre != null)
					return tre;
			}
			return null;
		}
		TreeNode FindNodeBeginningWith(string key, TreeNodeCollection nodes, List<TreeNode> foundNodes)
		{
			if (key == null)
				return null;
			if (nodes == null)
				nodes = ActiveTree.Nodes;
			foreach (TreeNode tn in nodes)
			{
				if (tn.Tag == null)
					continue;
				if (tn.Tag is IGroup)
				{
					if ((tn.Tag as IGroup).Label.ToLower().StartsWith(key.ToLower()) && !tn.IsSelected)
					{
						if (ActiveTree.SelectedNode == null)
							return tn;
						if (ActiveTree.SelectedNode.Tag is IGroup)
						{
							if ((ActiveTree.SelectedNode.Tag as IGroup).Label.ToLower().StartsWith(key.ToLower()))
							{
								if (foundNodes.IndexOf(ActiveTree.SelectedNode) == foundNodes.Count - 1)
								{
									if (foundNodes.IndexOf(tn) == 0)
										return tn;
								}
								else if (foundNodes.IndexOf(ActiveTree.SelectedNode) < foundNodes.IndexOf(tn))
									return tn;
							}
							else
								return tn;
						}
						else
							return tn;

					}

				}

				TreeNode tre = FindNodeBeginningWith(key, tn.Nodes, foundNodes);

				if (tre != null)
					return tre;
			}
			return null;
		}

		#endregion

		#region ContextMenuStrip Items

		ContextMenuStrip m_contextStrip = new ContextMenuStrip();
		public ContextMenuStrip TreeContextMenu
		{
			get { return m_contextStrip; }
			set { m_contextStrip = value; }
		}

		private void BuildContextMenu()
		{
			TreeContextMenu = new ContextMenuStrip();

			TreeContextMenu.Items.Add("Add");
			TreeContextMenu.Items.Add("Delete");
			TreeContextMenu.Items.Add(new ToolStripSeparator());
			TreeContextMenu.Items.Add("Copy", null, OnCopy);
			TreeContextMenu.Items.Add("Paste");
			TreeContextMenu.Items.Add(new ToolStripSeparator());
			TreeContextMenu.Items.Add("Show/Hide", null, OnVisibleToggleClick);

			SeqTree.ContextMenuStrip = TreeContextMenu;
			//SorTree.ContextMenuStrip = TreeContextMenu;
		}

		public event VisibilityToggled VisibilityToggle;
		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			if (VisibilityToggle != null)
				VisibilityToggle(sender, new EventArgs<IRebuild>(SelectedTag as IRebuild));
		}

		void OnCopy(object sender, EventArgs e)
		{
			IRebuild group = SelectedTag as IRebuild;
			if (group == null)
			{
				Logleton.TheLog.Log("{0}:{1} failed to copy", SelectedTag.GetType().Name, SelectedTag.ToString());
				return;
			}
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(group.GetType().Name, Utilities.Serialize(group.WriteScript()));
			//Clipboard.SetText(group.WriteXScript(NsXml.MakeDoc("Clipboard")).OuterXml);
			Logleton.TheLog.Log("{0}:{1} copied to clipboard", group.GetType().Name, group.Label);
		}

		#endregion
	
		#region Save

		public void SaveScriptFile(string path)
		{
			//SaveTree(SeqTree, path);
			TreeNode root = ActiveTree.SelectedNode;
			if (root == null)
			{
				if (ActiveTree != null && ActiveTree.Nodes.Count > 0)
					root = ActiveTree.Nodes[0];
				else if (SeqTree != null && SeqTree.Nodes.Count > 0)
					root = SeqTree.Nodes[0];
			}
			if (root == null)
				return;
			while (root.Parent != null)
				root = root.Parent;
			using (StreamWriter text = new StreamWriter(path))
				SaveNode(text, root);

		}
		private void SaveNode(StreamWriter text, TreeNode node)
		{
			text.Write(new string('\t', node.Level));//indent properly
			text.WriteLine(node.Text);
			foreach (TreeNode tn in node.Nodes)
				SaveNode(text, tn);
		}

		#endregion

		internal void Add(Sail sail)
		{
			TreeNode treeNode = sail.WriteNode();
			if (SeqTree.Nodes.Contains(treeNode))
				return;
			else
			{
				SeqTree.Nodes.Add(treeNode);
				//SorTree.Nodes.Add(treeNode);
			}
		}

		internal void Invalidate(IRebuild item)
		{
			TreeNode tn = FindNode(item);
			if (tn != null)
			{
				//tn.ForeColor = Color.White;
				tn.BackColor = Color.Firebrick;
				tn.EnsureVisible();
			}
		}

		internal void Revalidate(IRebuild item)
		{
			TreeNode tn = FindNode(item);
			if (tn != null)
			{
				//tn.ForeColor = Color.Black;
				tn.BackColor = Color.White;
			}
		}

		#region Key Traversing

		private void m_seqtree_KeyUp(object sender, KeyEventArgs e)
		{
			if (ModifierKeys == Keys.Control)
			{
				if (e.KeyCode == Keys.C)
					OnCopy(sender, new EventArgs());
				else
					base.OnKeyUp(e);
			}
			else
				HighLightNodeBeginningWith(e.KeyCode);

		}

		private void HighLightNodeBeginningWith(Keys key)
		{
			List<TreeNode> foundNodes = new List<TreeNode>();
			TreeNode last = FindAllNodesBeginningWith(key.ToString(), null, ref foundNodes);

			TreeNode prev = ActiveTree.SelectedNode;
			TreeNode found = FindNodeBeginningWith(key.ToString(), null, foundNodes);
			if (found != null)
			{
				found.EnsureVisible();
				ActiveTree.SelectedNode = found;
				TreeViewCancelEventArgs cancel = new TreeViewCancelEventArgs(found, false, TreeViewAction.ByKeyboard);
				RaiseBeforeSelect(cancel);
				if (cancel.Cancel)
				{
					ActiveTree.SelectedNode = prev;
					prev.EnsureVisible();
				}
			}
		}

		private void m_seqtree_KeyDown(object sender, KeyEventArgs e)
		{
			if (ModifierKeys == Keys.Control)
				e.Handled = true;
		}

		private void m_seqtree_KeyPress(object sender, KeyPressEventArgs e)
		{
			//this is required to get rid of the beep sound when ctrl-c and v are pressed
			//if (ModifierKeys == Keys.Control)
			e.Handled = true;
		}

		#endregion

		internal void Remove(IRebuild tag)
		{
			TreeNode found = FindNode(tag);
			if (found != null)
				SeqTree.Nodes.Remove(found);
			SeqTree.Refresh();
		}

		internal void ClearAll()
		{
			SeqTree.Nodes.Clear();
			SorTree.Nodes.Clear();
			SeqTree.Refresh();
			SorTree.Refresh();
		}

		#region Drag-n-Drop

		#region Members

		Color CPARENTS = Color.LightSteelBlue;
		Color CCHILDREN = Color.PaleVioletRed;

		List<IRebuild> m_dragParents = null;
		List<IRebuild> m_dragChildren = null;
		int m_watermarkParent, m_watermarkChild; 

		#endregion

		private void OnItemDrag(object sender, ItemDragEventArgs e)
		{
			if (Sail == null)
				return;

			TreeNode dragged = e.Item as TreeNode;

			if (dragged.Tag is IRebuild)
			{
				IRebuild item = dragged.Tag as IRebuild;

				m_dragParents = new List<IRebuild>();
				item.GetParents(Sail, m_dragParents);

				m_dragChildren = Sail.GetConnected(item);
				m_dragChildren.Remove(item);

				if (item is IGroup)
				{
					IGroup group = item as IGroup;
					m_dragChildren.RemoveAll(r => group.ContainsItem(r));
				}

				//find drag-bounds
				m_watermarkParent = -1; m_watermarkChild = int.MaxValue;
				m_dragParents.ForEach(par => m_watermarkParent = Math.Max(m_watermarkParent, FlatIndexOf(par)));
				m_dragChildren.ForEach(chi => m_watermarkChild = Math.Min(m_watermarkChild, FlatIndexOf(chi)));

				ColorDrag(false);
				DoDragDrop(e.Item, DragDropEffects.Move);
			}
			else
				DoDragDrop(e.Item, DragDropEffects.None);
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			ColorDrag(false);
			//if (e is ItemDragEventArgs)
			//	OnItemDrag(sender, e as ItemDragEventArgs);
			//else
				//e.Effect = DragDropEffects.None;//allow dragging from external sources?
		}
		private void OnDragOver(object sender, DragEventArgs e)
		{
			TreeNode dragged = (TreeNode)e.Data.GetData(typeof(TreeNode));
			Point pos = SeqTree.PointToClient(new Point(e.X, e.Y));
			//var hit = SeqTree.HitTest(pos);
			TreeNode node = SeqTree.GetNodeAt(pos);
			if (node != null)
			{
				IRebuild tag = FindItem(node);
				bool canDrop = Utilities.IsBetween(m_watermarkParent + 1, FlatIndexOf(tag), m_watermarkChild - 1);

				e.Effect = canDrop ? DragDropEffects.Move : DragDropEffects.None;
			}

			SeqTree.SelectedNode = SeqTree.GetNodeAt(pos);
		}
		private void OnDragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the node that was dragged.
			TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

			// Retrieve the client coordinates of the drop location.
			Point targetPoint = SeqTree.PointToClient(new Point(e.X, e.Y));
			// Retrieve the node at the drop location.
			TreeNode targetNode = SeqTree.GetNodeAt(targetPoint);

			//refresh parent colors on drop regardless of outcome
			ColorDrag(true);
			m_dragParents = null;
			m_dragChildren = null;

			if (targetNode == null || draggedNode == null)
				return;

			// Confirm that the node at the drop location is not  
			// the dragged node or a descendant of the dragged node. 
			//if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode) && AllowReorder(draggedNode, targetNode))
			if (draggedNode.Equals(targetNode) || targetNode.FullPath.StartsWith(draggedNode.FullPath))
				return;//dont drop onto yourself

			// If it is a move operation, remove the node from its current  
			// location and add it to the node at the drop location. 
			if (e.Effect != DragDropEffects.Move)
				return;//for now only allow reordering via drag, no copy-paste

			IRebuild drag = FindItem(draggedNode);
			IRebuild targ = FindItem(targetNode);
			IGroup dragGrp = drag as IGroup;
			IGroup targGrp = targ as IGroup;
			IGroup dragParent = FindParentGroup(draggedNode);//need this for removal
			IGroup targParent = FindParentGroup(targetNode);

			if (drag == null || targ == null)
				return;

			//shift key forces reorder instead of insert, required for mixedgroup nesting
			if (!(ModifierKeys == Keys.Shift) && targGrp != null && targGrp.CanInsert(drag))
				targGrp = targ as IGroup;
			else if (targParent == null || targParent.CanInsert(drag))
				targGrp = targParent;

			//append the dragged item into its new group or sail
			if (targGrp == null)
				Utilities.Insert(Sail.Layout, drag, targ);
			else
				targGrp.Insert(drag, targ);

			if (dragParent == null && targGrp != null)//check sail if no parent
			{
				Sail.Layout.Remove(drag);
				Sail.WriteNode();
			}
			else if (dragParent != targGrp)//remove it from its previous group
			{
				dragParent.Remove(drag);
				dragParent.WriteNode();
			}

			//update nodes
			if (targGrp == null)
				Sail.WriteNode();
			else
				targGrp.WriteNode();

			////if the target is a group that can contain the drag type, then insert/reorder it
			//if (!(ModifierKeys == Keys.Shift) && targGrp != null && targGrp.CanInsert(drag))
			//{
			//	//append the dragged item into its new group
			//	targGrp.Insert(drag, null);
			//	if (dragParent == null)//check sail if no parent
			//	{
			//		Sail.Layout.Remove(drag);
			//		Sail.WriteNode();
			//	}
			//	else if (dragParent != targGrp)//remove it from its previous group
			//	{
			//		dragParent.Remove(drag);
			//		dragParent.WriteNode();
			//	}

			//	//update nodes
			//	targGrp.WriteNode();
			//}
			//else if (targParent != null && targParent.CanInsert(drag))
			//{
			//	//insert the dragged item into its new group above the target item
			//	targParent.Insert(drag, targ);
			//	if (dragParent == null)//check sail if no parent
			//	{
			//		Sail.Layout.Remove(drag);
			//		Sail.WriteNode();
			//	}
			//	else if (dragParent != targParent)//remove the item from its previous group
			//	{
			//		dragParent.Remove(drag);
			//		dragParent.WriteNode();
			//	}
			//	targParent.WriteNode();
			//	//update nodes
			//}
		}
		private void OnDragLeave(object sender, EventArgs e)
		{
			ColorDrag(true);
		}

		void ColorDrag(bool bWhite)
		{
			if (m_dragParents != null) m_dragParents.ForEach(p => p.WriteNode().BackColor = bWhite ? Color.White : CPARENTS);
			if (m_dragChildren != null) m_dragChildren.ForEach(p => p.WriteNode().BackColor = bWhite ? Color.White : CCHILDREN);
		}

		int FlatIndexOf(IRebuild item)
		{
			List<IRebuild> FullLayout = Sail.FlatLayout();
			return FullLayout.IndexOf(item);
		}

		#region olddrag
		//private void OnItemDrag2(object sender, ItemDragEventArgs e)
		//{
		//	if (Sail == null)
		//		return;

		//	TreeNode dragged = e.Item as TreeNode;

		//	if (dragged.Tag is IRebuild)
		//	{
		//		IRebuild grp = dragged.Tag as IRebuild;
		//		m_dragParents = new List<IRebuild>();
		//		grp.GetParents(Sail, m_dragParents);

		//		m_dragChildren = Sail.GetConnected(grp);

		//		m_dragChildren.Remove(grp);

		//		List<IRebuild> removeMe = new List<IRebuild>();

		//		m_dragChildren.ForEach(irb =>
		//		{
		//			if ((grp is IGroup))
		//				if ((grp as IGroup).ContainsItem(irb))
		//					removeMe.Add(irb);

		//		});
		//		removeMe.ForEach(irb => m_dragChildren.Remove(irb));

		//		//find drag-bounds
		//		m_watermarkParent = 0; m_watermarkChild = int.MaxValue;
		//		m_dragParents.ForEach(par => m_watermarkParent = Math.Max(m_watermarkParent, FlatIndexOf(par)));
		//		m_dragChildren.ForEach(chi => m_watermarkChild = Math.Min(m_watermarkChild, FlatIndexOf(chi)));

		//		ColorCollection(grp, m_dragParents, CPARENTS);
		//		ColorCollection(grp, m_dragChildren, CCHILDREN);
		//		DoDragDrop(e.Item, DragDropEffects.Move);
		//	}
		//	else
		//		DoDragDrop(e.Item, DragDropEffects.None);
		//}


		//private void m_seqtree_DragDrop2(object sender, DragEventArgs e)
		//{
		//	// Retrieve the client coordinates of the drop location.
		//	Point targetPoint = SeqTree.PointToClient(new Point(e.X, e.Y));

		//	// Retrieve the node that was dragged.
		//	TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

		//	// Retrieve the node at the drop location.
		//	TreeNode targetNode = SeqTree.GetNodeAt(targetPoint);
		//	if (targetNode == null)
		//		goto finish;

		//	// Confirm that the node at the drop location is not  
		//	// the dragged node or a descendant of the dragged node. 
		//	//if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode) && AllowReorder(draggedNode, targetNode))
		//	if (!draggedNode.Equals(targetNode))
		//	{
		//		// If it is a move operation, remove the node from its current  
		//		// location and add it to the node at the drop location. 
		//		if (e.Effect == DragDropEffects.Move)
		//		{
		//			IRebuild drg = draggedNode.Tag as IRebuild;
		//			IRebuild trgt = targetNode.Tag as IRebuild;

		//			if (trgt == null)
		//				goto finish;

		//			if (drg.Locked || trgt.Locked)
		//				goto finish;

		//			IRebuild aboveMe = null;
		//			IRebuild belowMe = null;

		//			bool ToTop = true;

		//			if (trgt is IGroup)
		//			{

		//				//if we put a group into a new group, we just reorder
		//				if (drg is IGroup)
		//				{
		//					if (!CheckWaterMarkIndexParents(m_dragParents, trgt))// make sure this is allowable first
		//						goto finish;
		//					else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt))
		//						goto finish;

		//					int newIndex = Sail.Layout.IndexOf(trgt as IGroup);
		//					Sail.Layout.Remove(drg as IGroup);
		//					Sail.Layout.Insert(newIndex, drg as IGroup);

		//					draggedNode.Remove();

		//				}
		//				//if we drag a mouldcurve into a new group, then it goes to the bottom
		//				else if (drg is MouldCurve && trgt is CurveGroup)
		//				{
		//					//if we drag a non-Igroup IRebuild into it's respective container that has a 
		//					//parent in it, we should allow it but place the item at the bottom of the group.
		//					//aboveMe = null;
		//					if (!CheckWaterMarkIndexParents(m_dragParents, trgt, ref aboveMe))// make sure this is allowable first
		//					{
		//						//here we check if the parent that is preventing you from reordering belongs
		//						// to the target group.  If it does, the place the dragged node at the bottom and allow
		//						if (aboveMe is MouldCurve)
		//						{
		//							TreeNode aboveMeParent = FindNode(aboveMe).Parent;
		//							if ((aboveMeParent.Tag as CurveGroup) != trgt)
		//								goto finish;
		//						}
		//					}
		//					else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt, ref belowMe))
		//					{
		//						if (belowMe is MouldCurve)
		//						{
		//							TreeNode belowMeParent = FindNode(belowMe).Parent;
		//							if ((belowMeParent.Tag as CurveGroup) != trgt)
		//								goto finish;
		//							ToTop = false;
		//						}
		//					}
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curve = FindNode(drg as MouldCurve);
		//					IGroup curParent = curve.Parent.Tag as IGroup;
		//					if (curParent == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParent);
		//					(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(trgt as IGroup);
		//					if(ToTop)
		//						(Sail.Layout[newIndex] as CurveGroup).Add(drg as MouldCurve);
		//					else
		//						(Sail.Layout[newIndex] as CurveGroup).Insert(0, drg as MouldCurve);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();

		//				}
		//				else if (drg is Equation && trgt is VariableGroup)
		//				{

		//					if (!CheckWaterMarkIndexParents(m_dragParents, trgt, ref aboveMe))// make sure this is allowable first
		//					{
		//						//here we check if the parent that is preventing you from reordering belongs
		//						// to the target group.  If it does, the place the dragged node at the bottom and allow
		//						if (aboveMe is Equation)
		//						{
		//							TreeNode aboveMeParent = FindNode(aboveMe).Parent;
		//							if ((aboveMeParent.Tag as VariableGroup) != trgt)
		//								goto finish;
		//						}
		//					}
		//					else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt, ref belowMe))
		//					{
		//						if (belowMe is Equation)
		//						{
		//							TreeNode belowMeParent = FindNode(belowMe).Parent;
		//							if ((belowMeParent.Tag as VariableGroup) != trgt)
		//								goto finish;
		//							ToTop = false;
		//						}
		//					}
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curve = FindNode(drg as Equation);
		//					IGroup curParent = curve.Parent.Tag as IGroup;
		//					if (curParent == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParent);
		//					(Sail.Layout[newIndex] as VariableGroup).Remove((drg as Equation).Label);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(trgt as IGroup);
		//					if(ToTop)
		//						(Sail.Layout[newIndex] as VariableGroup).Add(drg as Equation);
		//					else
		//						(Sail.Layout[newIndex] as VariableGroup).Insert(0,drg as Equation);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();
		//				}


		//			}

		//			else if (trgt is MouldCurve)
		//			{
		//				if (!CheckWaterMarkIndexParents(m_dragParents, trgt))// make sure this is allowable first
		//					goto finish;
		//				else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt))
		//					goto finish;
		//				//if we drag a mouldcurve onto another mouldcurve
		//				// then we insert that mouldcurve into the new IGroup containing the target
		//				// and place the drg there
		//				// this should work across CurveGroups
		//				if (drg is MouldCurve)
		//				{
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curveD = FindNode(drg as MouldCurve);
		//					IGroup curParentD = curveD.Parent.Tag as IGroup;
		//					if (curParentD == null)
		//						goto finish;

		//					//Second find the index of the target mouldcurves parent
		//					//so we can add the curve to it
		//					TreeNode curveT = FindNode(trgt as MouldCurve);
		//					IGroup curParentT = curveT.Parent.Tag as IGroup;
		//					if (curParentT == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParentD);
		//					(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(curParentT);
		//					int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as MouldCurve);
		//					(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as MouldCurve);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();
		//				}
		//				else if (drg is GuideComb)
		//				{
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curveD = FindNode(drg as GuideComb);
		//					IGroup curParentD = curveD.Parent.Tag as IGroup;
		//					if (curParentD == null)
		//						goto finish;

		//					//Second find the index of the target mouldcurves parent
		//					//so we can add the curve to it
		//					TreeNode curveT = FindNode(trgt as MouldCurve);
		//					IGroup curParentT = curveT.Parent.Tag as IGroup;
		//					if (curParentT == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParentD);
		//					(Sail.Layout[newIndex] as CurveGroup).Remove(drg as GuideComb);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(curParentT);
		//					int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as MouldCurve);
		//					(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as GuideComb);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();
		//				}
		//			}

		//			else if (trgt is GuideComb)
		//			{
		//				if (!CheckWaterMarkIndexParents(m_dragParents, trgt))// make sure this is allowable first
		//					goto finish;
		//				else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt))
		//					goto finish;
		//				//if we drag a mouldcurve onto another mouldcurve
		//				// then we insert that mouldcurve into the new IGroup containing the target
		//				// and place the drg there
		//				// this should work across CurveGroups
		//				if (drg is MouldCurve)
		//				{
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curveD = FindNode(drg as MouldCurve);
		//					IGroup curParentD = curveD.Parent.Tag as IGroup;
		//					if (curParentD == null)
		//						goto finish;

		//					//Second find the index of the target mouldcurves parent
		//					//so we can add the curve to it
		//					TreeNode curveT = FindNode(trgt as GuideComb);
		//					IGroup curParentT = curveT.Parent.Tag as IGroup;
		//					if (curParentT == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParentD);
		//					(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(curParentT);
		//					int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as GuideComb);
		//					(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as MouldCurve);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();

		//				}
		//				else if (drg is GuideComb)
		//				{
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curveD = FindNode(drg as GuideComb);
		//					IGroup curParentD = curveD.Parent.Tag as IGroup;
		//					if (curParentD == null)
		//						goto finish;

		//					//Second find the index of the target mouldcurves parent
		//					//so we can add the curve to it
		//					TreeNode curveT = FindNode(trgt as GuideComb);
		//					IGroup curParentT = curveT.Parent.Tag as IGroup;
		//					if (curParentT == null)
		//						goto finish;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParentD);
		//					(Sail.Layout[newIndex] as CurveGroup).Remove(drg as GuideComb);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(curParentT);
		//					int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as GuideComb);
		//					(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as GuideComb);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();
		//				}
		//			}

		//			else if (trgt is Equation)
		//			{
		//				if (!CheckWaterMarkIndexParents(m_dragParents, trgt))// make sure this is allowable first
		//					goto finish;
		//				else if (!CheckWaterMarkIndexChildren(m_dragChildren, trgt))
		//					goto finish;
		//				//if we drag a Equation onto another Equation
		//				// then we insert that Equation into the new IGroup containing the target
		//				// and place the drg there
		//				// this should work across VariableGroups
		//				if (drg is Equation)
		//				{
		//					//first find the index of the dragged mouldcurves parent
		//					//so we can remove the curve from it
		//					TreeNode curveD = FindNode(drg as Equation);
		//					IGroup curParentD = curveD.Parent.Tag as IGroup;
		//					if (curParentD == null)
		//						return;

		//					//Second find the index of the target mouldcurves parent
		//					//so we can add the curve to it
		//					TreeNode curveT = FindNode(trgt as Equation);
		//					IGroup curParentT = curveT.Parent.Tag as IGroup;
		//					if (curParentT == null)
		//						return;

		//					//remove
		//					int newIndex = Sail.Layout.IndexOf(curParentD);
		//					(Sail.Layout[newIndex] as VariableGroup).Remove((drg as Equation).Label);

		//					//insert into new one
		//					newIndex = Sail.Layout.IndexOf(curParentT);
		//					int insertIndex = (Sail.Layout[newIndex] as VariableGroup).IndexOf(trgt as Equation);
		//					(Sail.Layout[newIndex] as VariableGroup).Insert(insertIndex, drg as Equation);

		//					//need this guy or tree will not refresh.  Weird .Net bug
		//					draggedNode.Remove();

		//				}
		//			}

		//			Sail.Rebuild();
		//			SeqTree.Refresh();
		//		}

		//	}

		//	finish:

		//	if (m_dragParents != null)
		//		ColorCollection(draggedNode.Tag as IRebuild, m_dragParents, Color.White);

		//	if (m_dragChildren != null)
		//		ColorCollection(draggedNode.Tag as IRebuild, m_dragChildren, Color.White);

		//	//draggedNode.BackColor = Color.White;

		//	m_dragParents = null;
		//	m_dragChildren = null;
		//} 


		///// <summary>
		///// this function is going to check whether the target position index is larger 
		///// than the largest parent index in the tree. 
		///// </summary>
		///// <param name="parents">dragged item parents</param>
		///// <param name="trgt">target IRebuild</param>
		///// <param name="aboveMe">the parent that is higher than the target if there is one</param>
		///// <returns>true if index is above dependents, false otherwise</returns>
		//private bool CheckWaterMarkIndexParents(List<IRebuild> parents, IRebuild trgt, ref IRebuild aboveMe)
		//{
		//	if (parents.Contains(trgt))
		//		return false;

		//	TreeNode target = FindNode(trgt);
		//	int tarDepth = FlatIndexOf(trgt);

		//	foreach (IRebuild irb in parents)
		//	{
		//		TreeNode parent = FindNode(irb);
		//		int parDepth = FlatIndexOf(irb);
		//		if (tarDepth < parDepth)
		//		{
		//			aboveMe = irb;
		//			return false;//if the target depth
		//		}
		//		// is less than the parent depth, then don't allow
		//	}

		//	return true;
		//}

		//private bool CheckWaterMarkIndexParents(List<IRebuild> parents, IRebuild trgt)
		//{
		//	IRebuild throwAway = null;
		//	return CheckWaterMarkIndexParents(parents, trgt, ref throwAway);
		//}

		//private bool CheckWaterMarkIndexChildren(List<IRebuild> children, IRebuild trgt)
		//{
		//	IRebuild throwAway = null;
		//	return CheckWaterMarkIndexChildren(children, trgt, ref throwAway);
		//}

		///// <summary>
		///// this function is going to check whether the target position index is smaller 
		///// than the smallest child index in the tree. 
		///// </summary>
		///// <param name="parents">dragged item parents</param>
		///// <param name="trgt">target IRebuild</param>
		///// <param name="aboveMe">the child that is lower than the target if there is one</param>
		///// <returns>true if index is beneath dependents, false otherwise</returns>
		//private bool CheckWaterMarkIndexChildren(List<IRebuild> children, IRebuild trgt, ref IRebuild belowMe)
		//{
		//	if (children.Contains(trgt))
		//		return false;

		//	TreeNode target = FindNode(trgt);
		//	int tarDepth = FlatIndexOf(trgt);

		//	foreach (IRebuild irb in children)
		//	{
		//		//if((irb is IGroup)
		//		TreeNode child = FindNode(irb);
		//		int childDepth = FlatIndexOf(irb);
		//		if (trgt is IGroup) // if the target is a group that contains a child, return false because we want to insert
		//			if ((trgt as IGroup).ContainsItem(irb))
		//			{
		//				belowMe = irb;
		//				return false;
		//			}

		//		if (tarDepth > childDepth)
		//		{
		//			belowMe = irb;
		//			return false;//if the target depth
		//		}
		//		// is more than the child depth, then don't allow
		//	}

		//	return true;
		//}

		//int TheIndexOf(IRebuild node)
		//{
		//	int count = 0;
		//	List<IRebuild> nodes = Sail.FullLayout;

		//	foreach (IRebuild tn in nodes)
		//	{
		//		count++;
		//		if (tn == node)
		//			return count;
		//		if (tn is CurveGroup)
		//		{
		//			if ((tn as CurveGroup).Count > 0 && node is MouldCurve)
		//			{
		//				foreach (MouldCurve cur in (tn as CurveGroup))
		//				{
		//					count++;
		//					if (cur == node)
		//						return count;
		//				}
		//			}
		//			else if ((tn as CurveGroup).Count > 0 && node is GuideComb)
		//			{
		//				foreach (MouldCurve cur in (tn as CurveGroup))
		//				{
		//					count++;
		//					if ((cur as GuideComb) == node)
		//						return count;
		//				}
		//			}
		//			else
		//				count += (tn as CurveGroup).Count;
		//		}
		//		else if (tn is VariableGroup)
		//		{
		//			if ((tn as VariableGroup).Count > 0 && node is Equation)
		//			{
		//				foreach (KeyValuePair<string, Equation> eqs in (tn as VariableGroup))
		//				{
		//					count++;
		//					if (eqs.Key == (node as Equation).Label)
		//						return count;
		//				}
		//			}
		//			else
		//				count += (tn as VariableGroup).Count;
		//		}
		//	}
		//	return count;
		//}
		//private void ColorCollection(IRebuild drg, List<IRebuild> items, Color color)
		//{
		//	if (drg == null)
		//		return;

		//	items.ForEach(irb =>
		//	{
		//		TreeNode item = irb.WriteNode();//FindNode(irb);
		//		if (item != null)
		//		{
		//			if (drg is IGroup)
		//			{
		//				while (item.Parent.Tag != Sail)
		//				{
		//					item = item.Parent;
		//					if (item.Parent == null)
		//						break;
		//				}
		//			}
		//			else if (drg is MouldCurve || drg is Equation)
		//			{
		//				if (!(item.Tag is IGroup))
		//				{
		//					item.BackColor = color;//color the item
		//					while ( (!(item.Tag is IGroup) && item.Parent != null) )//then color its group 
		//					{
		//						item = item.Parent;
		//						//if (item.Parent == null)
		//						//	break;
		//					}
		//				}
		//			}

		//			//if (found.Tag as IRebuild != drg)
		//			item.BackColor = color;
		//		}
		//	});
		//}
		#endregion
		
		#endregion

		#region BeginUpdate

		internal void BeginUpdate()
		{
			ActiveTree.BeginUpdate();
		}

		internal void EndUpdate()
		{
			ActiveTree.EndUpdate();
		}
		
		#endregion
	}
}
