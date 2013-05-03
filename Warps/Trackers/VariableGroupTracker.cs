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

			if (m_frame != null )
			{
				//m_frame.okButton.Click += OnBuild;
				//m_frame.cancelButton.Click += OnCancel;
				//m_frame.previewButton.Click += OnPreview;

				m_frame.EditorPanel = Edit;
				EditMode = m_frame.EditMode;
				AddContextMenu();
				Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v
			}
		}
		public void Cancel()
		{
			//m_frame.okButton.Click -= OnBuild;
			//m_frame.cancelButton.Click -= OnCancel;
			//m_frame.previewButton.Click -= OnPreview;

			m_frame.EditorPanel = null;

			RemoveContextMenu();
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
			throw new NotImplementedException();
		}

		void m_eqEditor_OnVariableAdded(object sender, Equation addedEq)
		{
			m_frame.Rebuild(m_group);
			m_edit.Equations = m_group.ToArray();
		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; m_edit.Editable = value; }
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
				case Keys.C:
					OnCopy(Tree.SelectedTag, new EventArgs());
					break;
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}


		ContextMenuStrip m_context = new ContextMenuStrip();
		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
				if (Tree.ContextMenuStrip.Items[i].Text == "Paste Group")
					Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsVariableType();
			Tree.ContextMenuStrip.Show();
		}

		private void AddContextMenu()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();
				Tree.ContextMenuStrip.Items.Add("Add Variable", new Bitmap(Warps.Properties.Resources.glyphicons_190_circle_plus), OnAddVariableItemClick);
				Tree.ContextMenuStrip.Items.Add("Delete", new Bitmap(Warps.Properties.Resources.glyphicons_192_circle_remove), OnRemoveVariableGroupItemClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Copy Variable Group", new Bitmap(Warps.Properties.Resources.copy), OnCopy);
				Tree.ContextMenuStrip.Items.Add("Paste", new Bitmap(Warps.Properties.Resources.paste), OnPaste);
				Tree.ContextMenuStrip.Items[Tree.ContextMenuStrip.Items.Count - 1].Enabled = ClipboardContainsVariableType();

				Tree.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
			}
		}
		private void RemoveContextMenu()
		{
			if (Tree.ContextMenu != null)
			{
				Tree.ContextMenuStrip.Opening -= ContextMenuStrip_Opening;
				Tree.ContextMenuStrip.Items.Clear();
			}
			Tree.ContextMenuStrip = null;
		}
		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			//if (Tree.SelectedTag is IGroup)
			//{
			//	View.ToggleLayer((IGroup)Tree.SelectedTag);
			//}
			//if (curveTracker == null)
			//	return;

			//curveTracker
			//curveTracker = new CurveTracker(m_frame, new SurfaceCurve("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));

		}
		void OnShowOnlyClick(object sender, EventArgs e)
		{
			//View.ShowOnly((IGroup)Tree.SelectedTag);
			//curveTracker = new CurveTracker(m_frame, new SurfaceCurve("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));

		}

		void OnAddVariableItemClick(object sender, EventArgs e)
		{
			m_eqEditor.Show();
			//m_frame.Rebuild()
			//m_curveTracker = new CurveTracker(m_frame, new SurfaceCurve("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));

		}

		void OnAddGirthItemClick(object sender, EventArgs e)
		{
			//m_curveTracker = new CurveTracker(m_frame, new Geodesic("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));
		}
		void OnRemoveVariableGroupItemClick(object sender, EventArgs e)
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

		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}


		public void OnBuild(object sender, EventArgs e)
		{
			if (Edit == null || !EditMode)
				return;

			List<Equation> eqs = new List<Equation>();
			for (int i = 0; i < Edit.Count; i++)
			{
				object fit = null;
				if (Edit[i] != null)
					fit = Utilities.CreateInstance(typeof(Equation));
				if (fit != null && fit is Equation)
				{
					(fit as Equation).sail = sail;
					eqs.Add(fit as Equation);
					eqs.Last().ReadEditor(Edit[i]);
				}
			}

			m_group.Label = Edit.Label;
			m_group.Clear();
			eqs.ForEach(eq => m_group.Add(eq));
			m_frame.Rebuild(m_group);
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{

		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{

		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{

		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{

		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{

		}

		public void OnPreview(object sender, EventArgs e)
		{

		}

		public void OnCopy(object sender, EventArgs e)
		{
			IGroup group = Tree.SelectedTag as IGroup;

			if (group == null)
				return;
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(group.GetType().Name, Utilities.Serialize(group.WriteScript()));
			//Get data from clipboard
			m_frame.Status = String.Format("{0}:{1} Copied", group.GetType().Name, group.Label);

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

			VariableGroup group = (VariableGroup)Tree.SelectedTag;

			if (group == null)
				return;

			Equation eq = new Equation();
			eq.sail = group.Sail;
			eq.ReadScript(group.Sail, result);
			//eq.Evaluate();
			group.Add(eq);
			m_frame.Rebuild(m_group);

		}

		public bool IsTracking(object obj)
		{
			return obj == m_group;
		}

		private bool ClipboardContainsVariableType()
		{
			return Clipboard.ContainsData(typeof(Equation).Name);
		}

		internal void HighlightEquation(Equation equation)
		{
			if (m_edit != null)
				m_edit.HighlightEquation(equation);
		}
	}
}
