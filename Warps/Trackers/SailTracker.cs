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
		public SailTracker()
		{
		}

		WarpFrame m_frame;
		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
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

		public bool IsTracking { get { return false; } }

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null)
			{
				m_frame = frame;
				m_frame.EditorPanel = null;

				Tree.AttachTracker(this);
				View.AttachTracker(this);
			}
		}

		public void Cancel()
		{
			Tree.DetachTracker(this);
			View.DetachTracker(this);
		}

		public void OnBuild(object sender, EventArgs e) { }

		public void OnPreview(object sender, EventArgs e) { }

		public void OnAdd(object sender, EventArgs e) { }

		public void OnDelete(object sender, EventArgs e) { }

		public void OnSelect(object sender, EventArgs<IRebuild> e) { }

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e) { }

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

		public void ProcessSelection(object Tag)
		{
		}

		#endregion
	}
}
