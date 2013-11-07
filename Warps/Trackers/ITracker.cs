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
		/// Returns true if any changes have been made or the tracker is currently tracking something
		/// </summary>
		bool IsTracking { get; }

		/// <summary>
		/// Attaches the tracker to the view/tree and populates the editor
		/// </summary>
		/// <param name="frame">the warp frame to attach to, should always be "this"</param>
		void Track(WarpFrame frame);
		/// <summary>
		/// Detaches the tracker from the view/tree and removes the editor
		/// </summary>
		void Cancel();

		/// <summary>
		/// Called when the user clicks "Build" in the toolbar, should rebuild the editing objects and rebuild the sail if Autobuild
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBuild(object sender, EventArgs e);

		void OnPreview(object sender, EventArgs e);

		//void OnSelect(object sender, EventArgs<IRebuild> e);
		void OnClick(object sender, MouseEventArgs e);
		void OnDown(object sender, MouseEventArgs e);
		void OnMove(object sender, MouseEventArgs e);
		void OnUp(object sender, MouseEventArgs e);

		//void OnPaste(object sender, EventArgs e);
		//void OnDelete(object sender, EventArgs e);
		//void OnAdd(object sender, EventArgs e);

		/// <summary>
		/// Handles the selected item appropriately
		/// </summary>
		/// <param name="Tag">The newly selected object</param>
		void ProcessSelection(object Tag);

	}
}
