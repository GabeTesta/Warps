using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public interface ITracker
	{
		/// <summary>
		/// Should attach tracker to the view/tree and populate the edit
		/// </summary>
		/// <param name="frame">the warp frame to attach to, should always be "this"</param>
		void Track(WarpFrame frame);

		/// <summary>
		/// Returns true if the tracker is enabled, false if readonly
		/// </summary>
		bool EditMode { get; set; }

		void OnSelect(object sender, EventArgs<IRebuild> e);
		void OnClick(object sender, MouseEventArgs e);
		void OnDown(object sender, MouseEventArgs e);
		void OnMove(object sender, MouseEventArgs e);
		void OnUp(object sender, MouseEventArgs e);

		void OnCopy(object sender, EventArgs e);

		void OnPaste(object sender, EventArgs e);

		/// <summary>
		/// test to see if tracker is already tracking what you want
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
	//	bool IsTracking(object obj);

		/// <summary>
		/// Called when the user clicks "Build" in the toolbar, should rebuild the editing objects and rebuild the sail if Autobuild
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBuild(object sender, EventArgs e);
		/// <summary>
		/// Called when the user clicks "Cancel" in the toolbar, should remove all temporary tracker objects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCancel(object sender, EventArgs e);

		void OnPreview(object sender, EventArgs e);
	}
}
