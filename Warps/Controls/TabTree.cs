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

namespace Warps
{
	public partial class TabTree : UserControl
	{
		public TabTree()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			SeqTree.AfterSelect += Tree_AfterSelect;
			SorTree.AfterSelect += Tree_AfterSelect;
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

			SeqTree.ImageList = imageList;
			SorTree.ImageList = imageList;

			SeqTree.BeforeLabelEdit += SeqTree_BeforeLabelEdit;
			SeqTree.AfterLabelEdit += SeqTree_AfterLabelEdit;

			SeqTree.LabelEdit = false;

		}

		public void AttachTracker(ITracker tracker)
		{
			AfterSelect += tracker.OnSelect;
			//SeqTree.AfterSelect += tracker.OnSelect;
			//SorTree.AfterSelect += tracker.OnSelect;
		}

		public void DetachTracker(ITracker tracker)
		{
			AfterSelect -= tracker.OnSelect;
			//SeqTree.AfterSelect -= tracker.OnSelect;
			//SorTree.AfterSelect -= tracker.OnSelect;
		}

		//bool m_editMode = false;

		///// <summary>
		///// if true, we want to color the current node that is being editted with a different color than blue
		///// </summary>
		//public bool EditMode
		//{
		//	get { return m_editMode; }
		//	set
		//	{
		//		m_editMode = value;
		//		if (m_seqtree.SelectedNode != null)
		//			m_seqtree.SelectedNode.BackColor = value ? Color.LightGreen : Color.White;
		//	}
		//}

		void SeqTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{

		}

		void SeqTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{

		}

		ImageList imageList = new ImageList();

		ContextMenu m_context = new ContextMenu();

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

		public event ObjectSelected AfterSelect;

		void Tree_AfterSelect(object sender, TreeViewEventArgs e, bool bypassActionType)
		{
			object tag = FindTag(e.Node);
			if ((e.Action != TreeViewAction.Unknown || bypassActionType) && AfterSelect != null)
				AfterSelect(this, new EventArgs<IRebuild>(tag as IRebuild));
		}

		void Tree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Tree_AfterSelect(sender, e, false);
		}

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

		private void m_treeContext_Opening(object sender, CancelEventArgs e)
		{
			if (SelectedTag == null)
				return;
			this.ContextMenuStrip.Show(MousePosition);
		}

		private void CreateGeoGroupContext()
		{
			m_treeContext = new ContextMenuStrip();

		}

		private void CreateCurveGroupContext()
		{


		}

		#endregion

		internal void Invalidate(IRebuild item)
		{
			TreeNode tn = FindNode(item);
			if (tn != null)
			{
				tn.ForeColor = Color.White;
				tn.BackColor = Color.Firebrick;
				tn.EnsureVisible();
			}
		}

		internal void Revalidate(IRebuild item)
		{
			TreeNode tn = FindNode(item);
			if (tn != null)
			{
				tn.ForeColor = Color.Black;
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
				base.OnKeyUp(e);
			else
				HighLightNodeBeginningWith(e.KeyCode);

		}

		private void HighLightNodeBeginningWith(Keys key)
		{
			List<TreeNode> foundNodes = new List<TreeNode>();
			TreeNode last = FindAllNodesBeginningWith(key.ToString(), null, ref foundNodes);

			TreeNode found = FindNodeBeginningWith(key.ToString(), null, foundNodes);
			if (found != null)
			{
				found.EnsureVisible();
				ActiveTree.SelectedNode = found;

				Tree_AfterSelect(this, new TreeViewEventArgs(found), true);

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
	}
}
