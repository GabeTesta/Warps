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

				AddContextMenu();
				Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v

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

			RemoveContextMenu();

			Tree.KeyUp -= Tree_KeyUp;

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
				if (Tree != null)
					Tree.EditMode = value;
				if (value == false)
					Tree.DeSelect(m_group);

			}
		}

		// handle copy pasting from keyboard here
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

		#region TreePopup

		ContextMenuStrip m_context = new ContextMenuStrip();

		private void AddContextMenu()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();
				Tree.ContextMenuStrip.Items.Add("Add Curve", new Bitmap(Warps.Properties.Resources.glyphicons_190_circle_plus), OnAddCurveItemClick);
				Tree.ContextMenuStrip.Items.Add("Add Guide Comb", new Bitmap(Warps.Properties.Resources.glyphicons_190_circle_plus), OnAddGuideCombClick);
				Tree.ContextMenuStrip.Items.Add("Delete", new Bitmap(Warps.Properties.Resources.glyphicons_192_circle_remove), OnRemoveCurveItemClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Copy Group", new Bitmap(Warps.Properties.Resources.copy), OnCopy);
				Tree.ContextMenuStrip.Items.Add("Paste Curve", new Bitmap(Warps.Properties.Resources.paste), OnPaste);
				Tree.ContextMenuStrip.Items[Tree.ContextMenuStrip.Items.Count - 1].Enabled = ClipboardContainsCurveType();
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Show Only", new Bitmap(Warps.Properties.Resources.showonly), OnShowOnlyClick);
				Tree.ContextMenuStrip.Items.Add("Visible", new Bitmap(Warps.Properties.Resources.SmallEye), OnVisibleToggleClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Color", new Bitmap(Warps.Properties.Resources.showonly), EditColorClick);

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
		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
			{
				if (Tree.ContextMenuStrip.Items[i].Text == "Paste Curve")
					Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsCurveType();
				if (Tree.ContextMenuStrip.Items[i].Text.ToLower().Contains("add") || Tree.ContextMenuStrip.Items[i].Text.ToLower().Contains("delete"))
					Tree.ContextMenuStrip.Items[i].Enabled = EditMode;
			}

			Tree.ContextMenuStrip.Show();
		}

		void EditColorClick(object sender, EventArgs e)
		{
			if (Tree.SelectedTag is IGroup)
			{
				Layer l = View.GetLayer((Tree.SelectedTag as IGroup));
				if (l == null)
					return;

				ColorWheelForm cwf = new ColorWheelForm(l);

				cwf.Show(m_frame);
			}
		}

		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			View.ToggleLayer(m_group);
		}
		void OnShowOnlyClick(object sender, EventArgs e)
		{
			View.ShowOnly(m_group);
		}

		void OnAddCurveItemClick(object sender, EventArgs e)
		{
			//m_frame.Rebuild()
			m_curveTracker = new CurveTracker(new MouldCurve("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));
			m_curveTracker.Track(m_frame);
		}

		void OnAddGuideCombClick(object sender, EventArgs e)
		{
			m_curveTracker = new CurveTracker(new GuideComb("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }, new Vect2[] { new Vect2(0, 1), new Vect2(1, 1) }));
			m_curveTracker.Track(m_frame);
		}

		void OnAddGirthItemClick(object sender, EventArgs e)
		{
			m_curveTracker = new CurveTracker(new MouldCurve("", m_group.Sail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(1, 1) }));
			m_curveTracker.Track(m_frame);
		}
		void OnRemoveCurveItemClick(object sender, EventArgs e)
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

		public void OnCopy(object sender, EventArgs e)
		{
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(m_group.GetType().Name, Utilities.Serialize(m_group.WriteScript()));
			//Get data from clipboard
			m_frame.Status = String.Format("{0}:{1} Copied", m_group.GetType().Name, m_group.Label);
			m_frame.ClearTracker();//clear tracker so user can select sail and paste it
		}

		public void OnPaste(object sender, EventArgs e)
		{
			if (!ClipboardContainsCurveType())
				return;

			Type type = GetClipboardObjType();
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

			//switch (type.Name)
			//{
			//	case "CurveGroup":
			//		{

			//		}
			//		break;
			//	case "MouldCurve":
			//		{
			//			try
			//			{
			//				object cur = Utilities.CreateInstance(result[0].Split(new char[] { ':' })[0].Trim());
			//				if (cur != null && cur is MouldCurve)
			//				{
			//					(cur as MouldCurve).Sail = group.Sail;
			//					(cur as MouldCurve).ReadScript(group.Sail, result);
			//					(group as CurveGroup).Add(cur as MouldCurve);
			//					SetFrameStatus(String.Format("{0}:{1} Pasted into {2}:{3} From Clipboard", cur.GetType().Name, (cur as MouldCurve).Label, group.GetType().Name, group.Label));
			//				}

			//			}
			//			catch (Exception ex) { Warps.Logger.logger.Instance.LogErrorException(ex); return; }
			//			m_frame.Rebuild(group);
			//		}
			//		break;
			//	case "SurfaceCurve":
			//		{
			//			try
			//			{
			//				object cur = Utilities.CreateInstance(result[0].Split(new char[] { ':' })[0].Trim());
			//				if (cur != null && cur is MouldCurve)
			//				{
			//					(cur as MouldCurve).Sail = group.Sail;
			//					(cur as MouldCurve).ReadScript(group.Sail, result);
			//					(group as CurveGroup).Add(cur as MouldCurve);
			//					SetFrameStatus(String.Format("{0}:{1} Pasted into {2}:{3} From Clipboard", Utilities.GetTypeStringForClipboard(cur), (cur as MouldCurve).Label, Utilities.GetTypeStringForClipboard(group), group.Label));

			//				}

			//			}
			//			catch (Exception ex) { Warps.Logger.logger.Instance.LogErrorException(ex); return; }
			//			m_frame.Rebuild(group);
			//		}
			//		break;
			//	case "Geodesic":
			//		{
			//			try
			//			{
			//				object cur = Utilities.CreateInstance(result[0].Split(new char[] { ':' })[0].Trim());
			//				if (cur != null && cur is MouldCurve)
			//				{
			//					(cur as MouldCurve).Sail = group.Sail;
			//					(cur as MouldCurve).ReadScript(group.Sail, result);
			//					(group as CurveGroup).Add(cur as MouldCurve);
			//					SetFrameStatus(String.Format("{0}:{1} Pasted into {2}:{3} From Clipboard", Utilities.GetTypeStringForClipboard(cur), (cur as MouldCurve).Label, Utilities.GetTypeStringForClipboard(group), group.Label));

			//				}

			//			}
			//			catch (Exception ex) { Warps.Logger.logger.Instance.LogErrorException(ex); return; }
			//			m_frame.Rebuild(group);
			//		}
			//		break;
			//}
		}

		public bool IsTracking(object obj)
		{
			return obj == m_group;
		}

		private bool ClipboardContainsCurveType()
		{
			return Clipboard.ContainsData(typeof(CurveGroup).Name)
				|| Clipboard.ContainsData(typeof(MouldCurve).Name)
				|| Clipboard.ContainsData(typeof(GuideComb).Name);
		}

		Type GetClipboardObjType()
		{
			if (Clipboard.ContainsData(typeof(CurveGroup).Name))
				return typeof(CurveGroup);
			else if (Clipboard.ContainsData(typeof(MouldCurve).Name))
				return typeof(MouldCurve);
			else if (Clipboard.ContainsData(typeof(GuideComb).Name))
				return typeof(GuideComb);

			return null;

		}
	}
}