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
	public class VariableTracker: ITracker
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
				m_frame.EditorPanel = null;//no editor
				//m_frame.okButton.Click += OnBuild;
				//m_frame.cancelButton.Click += OnCancel;
				//m_frame.previewButton.Click += OnPreview;

				AddContextMenu();
				Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v

				//View.DeSelectAll();
				//if (m_equ != null)
				//{
					//List<MouldCurve> referencedCurves = m_equ.ExtractReferencedCurves();
					//foreach (MouldCurve curve in referencedCurves)
					//{
					//	View.Select(curve);
					//}
				//}
				//View.Refresh();
			}
		}
		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; }
		}

		WarpFrame m_frame;
		Equation m_equ;
		//Testing.EquationTester m_eqEditor = null;

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}

		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}

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
			for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
				if (Tree.ContextMenuStrip.Items[i].Text == "Paste Variable")
					Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsVariableType();
			Tree.ContextMenuStrip.Show();
		}

		ContextMenuStrip m_context = new ContextMenuStrip();

		private void RemoveContextMenu()
		{
			if (Tree.ContextMenu != null)
			{
				Tree.ContextMenuStrip.Opening -= ContextMenuStrip_Opening;
				Tree.ContextMenuStrip.Items.Clear();
			}
			Tree.ContextMenuStrip = null;
		}

		private void AddContextMenu()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();
				//Tree.ContextMenuStrip.Items.Add("Add Variable", new Bitmap(Warps.Properties.Resources.glyphicons_190_circle_plus), OnAddVariableItemClick);
				Tree.ContextMenuStrip.Items.Add("Delete", new Bitmap(Warps.Properties.Resources.glyphicons_192_circle_remove), OnRemoveVariableItemClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Copy Variable", new Bitmap(Warps.Properties.Resources.copy), OnCopy);
				Tree.ContextMenuStrip.Items.Add("Paste Variable", new Bitmap(Warps.Properties.Resources.paste), OnPaste);
				Tree.ContextMenuStrip.Items[Tree.ContextMenuStrip.Items.Count - 1].Enabled = ClipboardContainsVariableType();

				Tree.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
			}
		}

		void OnRemoveVariableItemClick(object sender, EventArgs e)
		{
			//View.Remove(m_equ);
			//Equation g = m_equ as Equation;
			//if (g != null)
			//{
			//	g.Remove(Curve);
			//	m_frame.Rebuild(g);
			//}
			//m_frame.Delete(Curve);
			//if (g != null)
			//	Tree.SelectedTag = g;

		}

		public void OnCancel(object sender, EventArgs e)
		{
			//foreach (MouldCurve curve in m_equ.ExtractReferencedCurves())
			//	View.DeSelect(curve);

			//View.Refresh();

			RemoveContextMenu();
			m_frame.okButton.Click -= OnBuild;
			m_frame.cancelButton.Click -= OnCancel;
			m_frame.previewButton.Click -= OnPreview;
			Tree.KeyUp -= Tree_KeyUp;
		//	if (m_eqEditor != null)
		//	{
		//		m_eqEditor.OnVariableDeleted -= m_eqEditor_OnVariableDeleted;
		//		m_eqEditor.OnVariableAdded -= m_eqEditor_OnVariableAdded;
		//		m_eqEditor = null;
		//	}
		}

		public void OnBuild(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			throw new NotImplementedException();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnCopy(object sender, EventArgs e)
		{
			Equation equ = Tree.SelectedTag as Equation;

			if (equ == null)
				return;
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(equ.GetType().Name, Utilities.Serialize(equ.WriteScript()));
			//Get data from clipboard
			m_frame.Status = String.Format("{0}:{1} Copied", equ.GetType().Name, equ.Label);

		}

		public void OnPaste(object sender, EventArgs e)
		{
			if (!ClipboardContainsVariableType())
				return;

			Type type = Utilities.GetClipboardObjType();
			if (type == null)
				return;

			List<string> result = (List<string>)Utilities.DeSerialize(Clipboard.GetData(type.Name).ToString());

			ScriptTools.ModifyScriptToShowCopied(ref result);

			Equation group = (Equation)Tree.SelectedTag;

			if (group == null)
				return;
		}

		public bool IsTracking(object obj)
		{
			return obj == m_equ;
		}

		private bool ClipboardContainsVariableType()
		{
			return Clipboard.ContainsData(typeof(Equation).Name);
		}

	}
}
