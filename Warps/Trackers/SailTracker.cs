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
	public class SailTracker : ITracker
	{
		//public SailTracker(WarpFrame frame)
		//{
		//	m_frame = frame;
		//	m_sail = m_frame != null ? m_frame.ActiveSail : null;
		//	AddContextMenu();

		//	m_frame.okButton.Click += OnBuild;
		//	m_frame.cancelButton.Click += OnCancel;
		//	m_frame.previewButton.Click += OnPreview;
		//	Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v

		//	View.AttachTracker(this);

		//	Tree.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
		//}

		public SailTracker(bool mode)
		{
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;
			m_frame.EditorPanel = null;
			AddContextMenu();

			//m_frame.okButton.Click += OnBuild;
			//m_frame.cancelButton.Click += OnCancel;
			//m_frame.previewButton.Click += OnPreview;
			Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v
			Tree.ContextMenuStrip.Opening += ContextMenuStrip_Opening;

			View.AttachTracker(this);

			//View.ShowAll();
		}

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
				if (Tree.ContextMenuStrip.Items[i].Text == "Paste Curve")
					Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsCurve();
			Tree.ContextMenuStrip.Show();
			//Tree.ContextMenuStrip.Items["Paste Curve"].Enabled = ClipboardContainsCurve();
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

		private void AddContextMenu()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();

				//Tree.ContextMenuStrip.Items.Add("Show Only", new Bitmap(Warps.Properties.Resources.showonly), OnShowOnlyClick);
				Tree.ContextMenuStrip.Items.Add("Visible", new Bitmap(Warps.Properties.Resources.SmallEye), OnVisibleToggleClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				//Tree.ContextMenuStrip.Items.Add("Copy Curve", new Bitmap(Warps.Properties.Resources.copy), OnCopy);
				Tree.ContextMenuStrip.Items.Add("Paste Curve", new Bitmap(Warps.Properties.Resources.paste), OnPaste);
				Tree.ContextMenuStrip.Items[Tree.ContextMenuStrip.Items.Count - 1].Enabled = ClipboardContainsCurve();
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Color", new Bitmap(Warps.Properties.Resources.showonly), EditColorClick);
			
			}
		}

		void EditColorClick(object sender, EventArgs e)
		{
			if (Tree.SelectedTag is Sail)
			{
				Layer l = View.GetLayer("Mould");
				if (l == null)
					return;

				ColorWheelForm cwf = new ColorWheelForm(l);

				cwf.Show(m_frame);
			}
		}

		private void RemoveContextMenu()
		{
			if(Tree.ContextMenu != null)
				Tree.ContextMenuStrip.Items.Clear();
			Tree.ContextMenuStrip = null;
		}

		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			View.ToggleLayer(Sail);

		}
		void OnShowOnlyClick(object sender, EventArgs e)
		{
			//View.ShowOnly(Sail);

		}

		#region ITracker Stuff

		public void OnCancel(object sender, EventArgs e)
		{
			RemoveContextMenu();
			//m_frame.okButton.Click -= OnBuild;
			//m_frame.cancelButton.Click -= OnCancel;
			//m_frame.previewButton.Click -= OnPreview;
			Tree.KeyUp -= Tree_KeyUp;
		}

		public void OnBuild(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			//throw new NotImplementedException();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnCopy(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnPaste(object sender, EventArgs e)
		{
			if (!ClipboardContainsCurve())
				return;

			Type type = Utilities.GetClipboardObjType();
			if (type == null)
				return;

			List<string> result = (List<string>)Utilities.DeSerialize(Clipboard.GetData(type.Name).ToString());

			string groupname = result[0].Replace(type.Name, "");
			groupname = groupname.Replace(":","");
			groupname = groupname.TrimStart();
			groupname = groupname.TrimEnd();

			if(Sail.FindGroup(groupname) != null) // doesn't have this group
				ScriptTools.ModifyScriptToShowCopied(ref result);

			Sail sail = (Sail)Tree.SelectedTag;

			if (sail == null)
				return;
			if (type == typeof(CurveGroup))
			{
				CurveGroup g = new CurveGroup();
				g.Sail = sail;
				g.ReadScript(sail, result);

				sail.Add(g);
				//g.Rebuild(null);
				m_frame.Rebuild(g);
			}
			else if (type == typeof(VariableGroup))
			{
				VariableGroup g = new VariableGroup();
				g.Sail = sail;
				g.ReadScript(sail, result);

				sail.Add(g);
				//g.Rebuild(null);
				m_frame.Rebuild(g);
			}
			else if (type == typeof(YarnGroup))
			{
				YarnGroup g = new YarnGroup();
				g.Sail = sail;
				g.ReadScript(sail, result);

				sail.Add(g);
				//g.Rebuild(null);
				m_frame.Rebuild(g);
			}

		}

		#endregion ITracker Stuff

		bool ClipboardContainsCurve()
		{
			return Clipboard.ContainsData(typeof(CurveGroup).Name);
		}
	}
}
