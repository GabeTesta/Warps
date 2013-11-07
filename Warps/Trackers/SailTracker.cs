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

				View.AttachTracker(this);
			}
		}

		public void Cancel()
		{
			View.DetachTracker(this);
		}

		public void OnBuild(object sender, EventArgs e) { }

		public void OnPreview(object sender, EventArgs e) { }

		public void OnSelect(object sender, EventArgs<IRebuild> e) { }

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e) { }

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e) { }

	//	public void OnCopy(object sender, EventArgs e) { }

		public void ProcessSelection(object Tag)
		{
		}

		#endregion
	}
}
