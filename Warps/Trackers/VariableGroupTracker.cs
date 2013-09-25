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
	public class VariableGroupTracker : ITracker
	{
		public VariableGroupTracker(VariableGroup group)
		{
			m_group = group;
			m_eqEditor = new EquationEditorForm(m_group);
			m_eqEditor.OnVariableAdded += m_eqEditor_OnVariableAdded;
			m_eqEditor.OnVariableDeleted += m_eqEditor_OnVariableDeleted;

			m_edit = new VariableGroupEditor(m_group);
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null)
			{
				m_frame.EditorPanel = Edit;
				if (Tree != null)
				{
					Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
					Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
					Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
				}
			}
		}
		public void Cancel()
		{
			m_frame.EditorPanel = null;

			Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;
			Tree.KeyUp -= Tree_KeyUp;

			if (m_eqEditor != null)
			{
				m_eqEditor.OnVariableDeleted -= m_eqEditor_OnVariableDeleted;
				m_eqEditor.OnVariableAdded -= m_eqEditor_OnVariableAdded;
				m_eqEditor = null;
			}
		}

		void m_eqEditor_OnVariableDeleted(object sender, Equation DeleteMe)
		{
			//throw new NotImplementedException();
		}

		void m_eqEditor_OnVariableAdded(object sender, Equation addedEq)
		{
			m_frame.Rebuild(m_group);
			m_edit.Equations = m_group.ToArray();
		}


		EquationEditorForm m_eqEditor = null;

		WarpFrame m_frame;
		VariableGroup m_group;
		VariableGroupEditor m_edit;

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

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.TreeContextMenu.Items.Count; i++)
				if (Tree.TreeContextMenu.Items[i].Text == "Paste")
					Tree.TreeContextMenu.Items[i].Enabled = Utilities.GetClipboardObjType() == typeof(Equation);
			Tree.TreeContextMenu.Show();
		}

		void TreeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			Logleton.TheLog.Log("{0}: ContextMenuItem clicked {1}", this.GetType().Name, e.ClickedItem.Name);

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

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}

		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

		VariableGroupEditor Edit
		{
			get { return m_edit; }
		}

		Sail sail
		{
			get { return m_frame == null ? null : m_frame.ActiveSail; }
		}

		void ReadEditor()
		{
			//List<Equation> equs = new List<Equation>();
			//for (int i = 0; i < Edit.Count; i++)
			//{
			//	object fit = null;
			//	if (Edit[i] != null)
			//		fit = Utilities.CreateInstance(Edit[i].FitType.Name);
			//	if (fit != null && fit is IFitPoint)
			//	{
			//		pnts.Add(fit as IFitPoint);
			//		pnts.Last().ReadEditor(Edit[i]);
			//	}
			//}
			//m_temp.FitPoints = pnts.ToArray();
		}

		public void OnAdd(object sender, EventArgs e)
		{
			if (m_eqEditor != null)
				m_eqEditor.Show();
		}

		public void OnDelete(object sender, EventArgs e)
		{
			// HERE WE DELETE

			// Get clicked item (Curve)

			// List<IRebuild> ret = m_frame.Rebuild(Curve)
			// ret is invalid shit
			// call invalidate(ret)-> color themselves SHOW THEY ARE BROKEN
			// View entity keep previous state, color invalid byEntity, when fixed, byLayer
			//
			m_frame.Delete(m_group);

		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (Edit == null )
				return;

			List<Equation> eqs = new List<Equation>();
			for (int i = 0; i < Edit.Count; i++)
			{
				object fit = null;
				if (Edit[i] != null)
					fit = Utilities.CreateInstance(typeof(Equation));
				if (fit != null && fit is Equation)
				{
					//(fit as Equation).sail = sail;
					eqs.Add(fit as Equation);
					eqs.Last().ReadEditor(Edit[i]);
				}
			}

			List<IRebuild> changed = CompareEqs(new List<Equation>(eqs), m_group.Values.ToList());
			m_group.Label = Edit.Label;
			m_group.Clear();
			eqs.ForEach(eq => m_group.Add(eq));

			m_frame.Rebuilds(changed);
			//changed.ForEach(eq => m_frame.Rebuild(eq));
			//m_frame.Rebuild(m_group);
			if (changed.Count > 0)
			{
				(m_group as IGroup).WriteNode();
				m_edit.Equations = eqs.ConvertAll(eq => new KeyValuePair<string, Equation>(eq.Label, eq)).ToArray();
			}
		}
		private List<IRebuild> CompareEqs(List<Equation> eqs, List<Equation> list)
		{
			List<IRebuild> ret = new List<IRebuild>(list);
			List<Equation> removeMe = new List<Equation>();
			foreach (Equation eq in eqs)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (eq.Label == list[i].Label)
					{
						if (eq.EquationText == list[i].EquationText)
						{
							ret.Remove(list[i]);
							removeMe.Add(eq);
							break;
						}
					}
				}
			}

			foreach (Equation rbd in removeMe)
				eqs.Remove(rbd);

			eqs.ForEach(eq=> 
			{
				if (ret.Find(e=> e.Label == eq.Label) == null)
					ret.Add(eq);
			});

			return ret;
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e) { }

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnPreview(object sender, EventArgs e) { }

		//public void OnCopy(object sender, EventArgs e)
		//{
		//IGroup group = Tree.SelectedTag as IGroup;

		//if (group == null)
		//	return;
		////Lets say its my data format
		//Clipboard.Clear();
		////Set data to clipboard
		//Clipboard.SetData(group.GetType().Name, Utilities.Serialize(group.WriteScript()));
		////Get data from clipboard
		//m_frame.Status = String.Format("{0}:{1} Copied", group.GetType().Name, group.Label);

		//}

		public void OnPaste(object sender, EventArgs e)
		{
			//Equations should be able to be pasted into this group

			if (Utilities.GetClipboardObjType() != typeof(Equation))
				return;

			Type type = Utilities.GetClipboardObjType();
			if (type == null)
				return;

			List<string> result = (List<string>)Utilities.DeSerialize(Clipboard.GetData(type.Name).ToString());

			ScriptTools.ModifyScriptToShowCopied(ref result);

			//VariableGroup group = (VariableGroup)Tree.SelectedTag;

			if (m_group == null)
				return;

			Equation eq = new Equation();
			//eq.sail = group.Sail;
			eq.ReadScript(m_group.Sail, result);
			//eq.Evaluate();
			m_group.Add(eq);
			m_frame.Rebuild(m_group);

		}

		internal void HighlightEquation(Equation equation)
		{
			if (m_edit != null)
				m_edit.HighlightEquation(equation);
		}

		#region ITracker Members

		public bool IsTracking { get { return false; } }

		public void ProcessSelection(object Tag)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
