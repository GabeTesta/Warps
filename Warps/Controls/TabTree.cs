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

			imageList = new ImageList();

			imageList.Images.Add("empty", Warps.Properties.Resources.empty);
			imageList.Images.Add("Sail", Warps.Properties.Resources.icon_sail);
			imageList.Images.Add("CurveGroup", Warps.Properties.Resources.curvepath);
			imageList.Images.Add("MouldCurve", Warps.Properties.Resources.glyphicons_098_vector_path_curve);
			imageList.Images.Add("CurvePoint", Warps.Properties.Resources.curvept);
			imageList.Images.Add("SlidePoint", Warps.Properties.Resources.slidept);
			imageList.Images.Add("FixedPoint", Warps.Properties.Resources.fixedpt);
			imageList.Images.Add("Equation", Warps.Properties.Resources.equation);
			imageList.Images.Add("EquationText", Warps.Properties.Resources.EqText);
			imageList.Images.Add("Result", Warps.Properties.Resources.EqNum);
			imageList.Images.Add("VariableGroup", Warps.Properties.Resources.VariableGroup);
			imageList.Images.Add("YarnGroup", Warps.Properties.Resources.YarnGroup);
			imageList.Images.Add("GuideComb", Warps.Properties.Resources.GuideComb);
			imageList.Images.Add("Warps", Warps.Properties.Resources.Warps);
			imageList.Images.Add("PanelGroup", Warps.Properties.Resources.panelGroup);
			imageList.Images.Add("Panel", Warps.Properties.Resources.panel);
			imageList.Images.Add("EndCondition", Warps.Properties.Resources.EndCondition);

			SeqTree.ImageList = imageList;
			SorTree.ImageList = imageList;

			SeqTree.BeforeLabelEdit += SeqTree_BeforeLabelEdit;
			SeqTree.AfterLabelEdit += SeqTree_AfterLabelEdit;

			SeqTree.LabelEdit = false;

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

			Logleton.TheLog.Log("{0}:{1} copied to clipboard", group.GetType().Name, group.Label);
		}

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

		void SeqTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{

		}

		void SeqTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{

		}

		ImageList imageList = new ImageList();

		ContextMenuStrip m_contextStrip = new ContextMenuStrip();

		public ContextMenuStrip TreeContextMenu
		{
			get { return m_contextStrip; }
			set { m_contextStrip = value; }
		}

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

		object FindTag(TreeNode tn)
		{
			if (tn == null)
				return null;
			if (tn.Tag != null)
				return tn.Tag;
			return FindTag(tn.Parent);
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

		//public event ObjectSelected AfterSelect;
		public event TreeViewCancelEventHandler BeforeSelect;

		public event VisibilityToggled VisibilityToggle;

		internal void Select(IRebuild grp)
		{
			SelectedTag = grp;
			if (ActiveTree.SelectedNode != null)
				RaiseBeforeSelect(new TreeViewCancelEventArgs(ActiveTree.SelectedNode, false, TreeViewAction.Unknown));
		}
		//void Tree_AfterSelect(object sender, TreeViewEventArgs e, bool bypassActionType)
		//{
		//	object tag = FindTag(e.Node);
		//	if ((e.Action != TreeViewAction.Unknown || bypassActionType) && AfterSelect != null)
		//		AfterSelect(this, new EventArgs<IRebuild>(tag as IRebuild));
		//}

		//void Tree_AfterSelect(object sender, TreeViewEventArgs e)
		//{
		//	Tree_AfterSelect(sender, e, false);
		//}

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

		//private void m_treeContext_Opening(object sender, CancelEventArgs e)
		//{
		//	if (SelectedTag == null)
		//		return;
		//	this.ContextMenuStrip.Show(MousePosition);
		//}

		#endregion

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

		internal void Add(TreeNode treeNode)
		{
			if (SeqTree.Nodes.Contains(treeNode))
				return;
			else
			{
				SeqTree.Nodes.Add(treeNode);
				//SorTree.Nodes.Add(treeNode);
			}
		}

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

		//internal void DeSelect(IGroup group)
		//{
		//	TreeNode found = FindNode(group);
		//	if (found != null)
		//		found.BackColor = Color.White;
		//}

		//internal void DeSelect(MouldCurve group)
		//{
		//	TreeNode found = FindNode(group);
		//	if (found != null)
		//		found.BackColor = Color.White;

		//	m_seqtree.Refresh();
		//}

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

		private void m_seqtree_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void m_seqtree_DragOver(object sender, DragEventArgs e)
		{
			//TreeNode dragged = (TreeNode)e.Data.GetData(typeof(TreeNode));
			Point pos = SeqTree.PointToClient(new Point(e.X, e.Y));
			//var hit = SeqTree.HitTest(pos);
			//TreeNode node = hit.Node;
			//if (node != null)
			//{
			//bool canDrop = AllowReorder(dragged, node);

			//	e.Effect = canDrop ? DragDropEffects.Move : DragDropEffects.None;
			//}

			SeqTree.SelectedNode = SeqTree.GetNodeAt(pos);
		}

		private void m_seqtree_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the drop location.
			Point targetPoint = SeqTree.PointToClient(new Point(e.X, e.Y));
			
			// Retrieve the node that was dragged.
			TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

			// Retrieve the node at the drop location.
			TreeNode targetNode = SeqTree.GetNodeAt(targetPoint);
			if (targetNode == null)
				goto finish;

			// Confirm that the node at the drop location is not  
			// the dragged node or a descendant of the dragged node. 
			//if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode) && AllowReorder(draggedNode, targetNode))
			if (!draggedNode.Equals(targetNode))
			{
				// If it is a move operation, remove the node from its current  
				// location and add it to the node at the drop location. 
				if (e.Effect == DragDropEffects.Move)
				{
					IRebuild drg = draggedNode.Tag as IRebuild;
					IRebuild trgt = targetNode.Tag as IRebuild;

					if (trgt == null)
						goto finish;

					if (drg.Locked || trgt.Locked)
						goto finish;

					IRebuild aboveMe = null;
					IRebuild belowMe = null;

					bool ToTop = true;

					if (trgt is IGroup)
					{

						//if we put a group into a new group, we just reorder
						if (drg is IGroup)
						{
							if (!CheckWaterMarkIndexParents(draggedItemParents, trgt))// make sure this is allowable first
								goto finish;
							else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt))
								goto finish;

							int newIndex = Sail.Layout.IndexOf(trgt as IGroup);
							Sail.Layout.Remove(drg as IGroup);
							Sail.Layout.Insert(newIndex, drg as IGroup);

							draggedNode.Remove();

						}
						//if we drag a mouldcurve into a new group, then it goes to the bottom
						else if (drg is MouldCurve && trgt is CurveGroup)
						{
							//if we drag a non-Igroup IRebuild into it's respective container that has a 
							//parent in it, we should allow it but place the item at the bottom of the group.
							//aboveMe = null;
							if (!CheckWaterMarkIndexParents(draggedItemParents, trgt, ref aboveMe))// make sure this is allowable first
							{
								//here we check if the parent that is preventing you from reordering belongs
								// to the target group.  If it does, the place the dragged node at the bottom and allow
								if (aboveMe is MouldCurve)
								{
									TreeNode aboveMeParent = FindNode(aboveMe).Parent;
									if ((aboveMeParent.Tag as CurveGroup) != trgt)
										goto finish;
								}
							}
							else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt, ref belowMe))
							{
								if (belowMe is MouldCurve)
								{
									TreeNode belowMeParent = FindNode(belowMe).Parent;
									if ((belowMeParent.Tag as CurveGroup) != trgt)
										goto finish;
									ToTop = false;
								}
							}
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curve = FindNode(drg as MouldCurve);
							IGroup curParent = curve.Parent.Tag as IGroup;
							if (curParent == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParent);
							(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(trgt as IGroup);
							if(ToTop)
								(Sail.Layout[newIndex] as CurveGroup).Add(drg as MouldCurve);
							else
								(Sail.Layout[newIndex] as CurveGroup).Insert(0, drg as MouldCurve);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();

						}
						else if (drg is Equation && trgt is VariableGroup)
						{
							
							if (!CheckWaterMarkIndexParents(draggedItemParents, trgt, ref aboveMe))// make sure this is allowable first
							{
								//here we check if the parent that is preventing you from reordering belongs
								// to the target group.  If it does, the place the dragged node at the bottom and allow
								if (aboveMe is Equation)
								{
									TreeNode aboveMeParent = FindNode(aboveMe).Parent;
									if ((aboveMeParent.Tag as VariableGroup) != trgt)
										goto finish;
								}
							}
							else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt, ref belowMe))
							{
								if (belowMe is Equation)
								{
									TreeNode belowMeParent = FindNode(belowMe).Parent;
									if ((belowMeParent.Tag as VariableGroup) != trgt)
										goto finish;
									ToTop = false;
								}
							}
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curve = FindNode(drg as Equation);
							IGroup curParent = curve.Parent.Tag as IGroup;
							if (curParent == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParent);
							(Sail.Layout[newIndex] as VariableGroup).Remove((drg as Equation).Label);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(trgt as IGroup);
							if(ToTop)
								(Sail.Layout[newIndex] as VariableGroup).Add(drg as Equation);
							else
								(Sail.Layout[newIndex] as VariableGroup).Insert(0,drg as Equation);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();
						}


					}

					else if (trgt is MouldCurve)
					{
						if (!CheckWaterMarkIndexParents(draggedItemParents, trgt))// make sure this is allowable first
							goto finish;
						else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt))
							goto finish;
						//if we drag a mouldcurve onto another mouldcurve
						// then we insert that mouldcurve into the new IGroup containing the target
						// and place the drg there
						// this should work across CurveGroups
						if (drg is MouldCurve)
						{
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curveD = FindNode(drg as MouldCurve);
							IGroup curParentD = curveD.Parent.Tag as IGroup;
							if (curParentD == null)
								goto finish;

							//Second find the index of the target mouldcurves parent
							//so we can add the curve to it
							TreeNode curveT = FindNode(trgt as MouldCurve);
							IGroup curParentT = curveT.Parent.Tag as IGroup;
							if (curParentT == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParentD);
							(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(curParentT);
							int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as MouldCurve);
							(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as MouldCurve);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();
						}
						else if (drg is GuideComb)
						{
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curveD = FindNode(drg as GuideComb);
							IGroup curParentD = curveD.Parent.Tag as IGroup;
							if (curParentD == null)
								goto finish;

							//Second find the index of the target mouldcurves parent
							//so we can add the curve to it
							TreeNode curveT = FindNode(trgt as MouldCurve);
							IGroup curParentT = curveT.Parent.Tag as IGroup;
							if (curParentT == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParentD);
							(Sail.Layout[newIndex] as CurveGroup).Remove(drg as GuideComb);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(curParentT);
							int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as MouldCurve);
							(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as GuideComb);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();
						}
					}

					else if (trgt is GuideComb)
					{
						if (!CheckWaterMarkIndexParents(draggedItemParents, trgt))// make sure this is allowable first
							goto finish;
						else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt))
							goto finish;
						//if we drag a mouldcurve onto another mouldcurve
						// then we insert that mouldcurve into the new IGroup containing the target
						// and place the drg there
						// this should work across CurveGroups
						if (drg is MouldCurve)
						{
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curveD = FindNode(drg as MouldCurve);
							IGroup curParentD = curveD.Parent.Tag as IGroup;
							if (curParentD == null)
								goto finish;

							//Second find the index of the target mouldcurves parent
							//so we can add the curve to it
							TreeNode curveT = FindNode(trgt as GuideComb);
							IGroup curParentT = curveT.Parent.Tag as IGroup;
							if (curParentT == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParentD);
							(Sail.Layout[newIndex] as CurveGroup).Remove(drg as MouldCurve);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(curParentT);
							int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as GuideComb);
							(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as MouldCurve);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();

						}
						else if (drg is GuideComb)
						{
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curveD = FindNode(drg as GuideComb);
							IGroup curParentD = curveD.Parent.Tag as IGroup;
							if (curParentD == null)
								goto finish;

							//Second find the index of the target mouldcurves parent
							//so we can add the curve to it
							TreeNode curveT = FindNode(trgt as GuideComb);
							IGroup curParentT = curveT.Parent.Tag as IGroup;
							if (curParentT == null)
								goto finish;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParentD);
							(Sail.Layout[newIndex] as CurveGroup).Remove(drg as GuideComb);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(curParentT);
							int insertIndex = (Sail.Layout[newIndex] as CurveGroup).IndexOf(trgt as GuideComb);
							(Sail.Layout[newIndex] as CurveGroup).Insert(insertIndex, drg as GuideComb);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();
						}
					}

					else if (trgt is Equation)
					{
						if (!CheckWaterMarkIndexParents(draggedItemParents, trgt))// make sure this is allowable first
							goto finish;
						else if (!CheckWaterMarkIndexChildren(draggedItemChildren, trgt))
							goto finish;
						//if we drag a Equation onto another Equation
						// then we insert that Equation into the new IGroup containing the target
						// and place the drg there
						// this should work across VariableGroups
						if (drg is Equation)
						{
							//first find the index of the dragged mouldcurves parent
							//so we can remove the curve from it
							TreeNode curveD = FindNode(drg as Equation);
							IGroup curParentD = curveD.Parent.Tag as IGroup;
							if (curParentD == null)
								return;

							//Second find the index of the target mouldcurves parent
							//so we can add the curve to it
							TreeNode curveT = FindNode(trgt as Equation);
							IGroup curParentT = curveT.Parent.Tag as IGroup;
							if (curParentT == null)
								return;

							//remove
							int newIndex = Sail.Layout.IndexOf(curParentD);
							(Sail.Layout[newIndex] as VariableGroup).Remove((drg as Equation).Label);

							//insert into new one
							newIndex = Sail.Layout.IndexOf(curParentT);
							int insertIndex = (Sail.Layout[newIndex] as VariableGroup).IndexOf(trgt as Equation);
							(Sail.Layout[newIndex] as VariableGroup).Insert(insertIndex, drg as Equation);

							//need this guy or tree will not refresh.  Weird .Net bug
							draggedNode.Remove();

						}
					}

					Sail.Rebuild();
					SeqTree.Refresh();
				}

			}

			finish:

			if (draggedItemParents != null)
				ColorCollection(draggedNode.Tag as IRebuild, draggedItemParents, Color.White);

			if (draggedItemChildren != null)
				ColorCollection(draggedNode.Tag as IRebuild, draggedItemChildren, Color.White);

			//draggedNode.BackColor = Color.White;

			draggedItemParents = null;
			draggedItemChildren = null;
		}

		/// <summary>
		/// this function is going to check whether the target position index is larger 
		/// than the largest parent index in the tree. 
		/// </summary>
		/// <param name="parents">dragged item parents</param>
		/// <param name="trgt">target IRebuild</param>
		/// <param name="aboveMe">the parent that is higher than the target if there is one</param>
		/// <returns>true if index is above dependents, false otherwise</returns>
		private bool CheckWaterMarkIndexParents(List<IRebuild> parents, IRebuild trgt, ref IRebuild aboveMe)
		{
			if (parents.Contains(trgt))
				return false;

			TreeNode target = FindNode(trgt);
			int tarDepth = 0;
			TheIndexOf(trgt, null, ref tarDepth);

			foreach (IRebuild irb in parents)
			{
				TreeNode parent = FindNode(irb);
				int parDepth = 0;
				TheIndexOf(irb, null, ref parDepth);
				if (tarDepth < parDepth)
				{
					aboveMe = irb;
					return false;//if the target depth
				}
				// is less than the parent depth, then don't allow
			}

			return true;
		}

		private bool CheckWaterMarkIndexParents(List<IRebuild> parents, IRebuild trgt)
		{
			IRebuild throwAway = null;
			return CheckWaterMarkIndexParents(parents, trgt, ref throwAway);
		}

		private bool CheckWaterMarkIndexChildren(List<IRebuild> children, IRebuild trgt)
		{
			IRebuild throwAway = null;
			return CheckWaterMarkIndexChildren(children, trgt, ref throwAway);
		}
		/// <summary>
		/// this function is going to check whether the target position index is smaller 
		/// than the smallest child index in the tree. 
		/// </summary>
		/// <param name="parents">dragged item parents</param>
		/// <param name="trgt">target IRebuild</param>
		/// <param name="aboveMe">the child that is lower than the target if there is one</param>
		/// <returns>true if index is beneath dependents, false otherwise</returns>
		private bool CheckWaterMarkIndexChildren(List<IRebuild> children, IRebuild trgt, ref IRebuild belowMe)
		{
			if (children.Contains(trgt))
				return false;

			TreeNode target = FindNode(trgt);
			int tarDepth = 0;
			TheIndexOf(trgt, null, ref tarDepth);

			foreach (IRebuild irb in children)
			{
				//if((irb is IGroup)
				TreeNode child = FindNode(irb);
				int childDepth = 0;
				TheIndexOf(irb, null, ref childDepth);
				if (trgt is IGroup) // if the target is a group that contains a child, return false because we want to insert
					if ((trgt as IGroup).ContainsItem(irb))
					{
						belowMe = irb;
						return false;
					}

				if (tarDepth > childDepth)
				{
					belowMe = irb;
					return false;//if the target depth
				}
				// is more than the child depth, then don't allow
			}

			return true;
		}

		void TheIndexOf(IRebuild node, List<IGroup> nodes, ref int count)
		{
			if (nodes == null)
				nodes = Sail.FullLayout;

			foreach (IGroup tn in nodes)
			{
				count++;
				if (tn == node)
					return;
				if (tn is CurveGroup)
				{
					if ((tn as CurveGroup).Count > 0 && node is MouldCurve)
					{
						foreach (MouldCurve cur in (tn as CurveGroup))
						{
							count++;
							if (cur == node)
								return;
						}
					}
					else if ((tn as CurveGroup).Count > 0 && node is GuideComb)
					{
						foreach (MouldCurve cur in (tn as CurveGroup))
						{
							count++;
							if ((cur as GuideComb) == node)
								return;
						}
					}
					else
						count += (tn as CurveGroup).Count;
				}
				else if (tn is VariableGroup)
				{
					if ((tn as VariableGroup).Count > 0 && node is Equation)
					{
						foreach (KeyValuePair<string, Equation> eqs in (tn as VariableGroup))
						{
							count++;
							if (eqs.Key == (node as Equation).Label)
								return;
						}
					}
					else
						count += (tn as VariableGroup).Count;
				}
			}
			return;
		}

		List<IRebuild> draggedItemParents = null;
		List<IRebuild> draggedItemChildren = null;

		private void m_seqtree_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (Sail == null)
				return;

			TreeNode dragged = e.Item as TreeNode;

			if (dragged.Tag is IRebuild)
			{
				IRebuild grp = dragged.Tag as IRebuild;
				draggedItemParents = new List<IRebuild>();
				grp.GetParents(Sail, draggedItemParents);

				draggedItemChildren = Sail.GetConnected(grp);

				draggedItemChildren.Remove(grp);

				List<IRebuild> removeMe = new List<IRebuild>();

				draggedItemChildren.ForEach(irb =>
				{
					if ((grp is IGroup))
						if ((grp as IGroup).ContainsItem(irb))
							removeMe.Add(irb);

				});

				removeMe.ForEach(irb => draggedItemChildren.Remove(irb));

				ColorCollection(grp, draggedItemParents, Color.Lime);
				ColorCollection(grp, draggedItemChildren, Color.Yellow);
				DoDragDrop(e.Item, DragDropEffects.Move);
			}

			DoDragDrop(e.Item, DragDropEffects.None);
		}

		private void ColorCollection(IRebuild drg, List<IRebuild> items, Color color)
		{
			if (drg == null)
				return;

			items.ForEach(irb =>
			{
				TreeNode found = FindNode(irb);
				if (found != null)
				{
					if (drg is IGroup)
					{
						while (found.Parent.Tag != Sail)
						{
							found = found.Parent;
							if (found.Parent == null)
								break;
						}
					}
					else if (drg is MouldCurve || drg is Equation)
					{
						if (!(found.Tag is IGroup))
						{
							found.BackColor = color;
							while (!(found.Tag is IGroup))
							{
								found = found.Parent;
								if (found.Parent == null)
									break;
							}
						}
					}

					//if (found.Tag as IRebuild != drg)
					found.BackColor = color;
				}
			});
		}

		internal void BeginUpdate()
		{
			ActiveTree.BeginUpdate();
		}

		internal void EndUpdate()
		{
			ActiveTree.EndUpdate();
		}

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

	}
}
