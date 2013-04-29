using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public class NsPlot : NPlot.Windows.PlotSurface2D
	{
		public NsPlot()
		{
			//InitializeComponent();
			this.MouseDown += nplot_MouseDown;
			this.MouseMove += nplot_MouseMove;
			this.MouseUp += nplot_MouseUp;
			this.DoubleClick += NsPlot_DoubleClick;
		}

		void NsPlot_DoubleClick(object sender, EventArgs e)
		{
			this.OriginalDimensions();
		}

		#region Mouse Zoom/Pan

		NPlot.Windows.PlotSurface2D.Interactions.RubberBandSelection rbs = new NPlot.Windows.PlotSurface2D.Interactions.RubberBandSelection();
		NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag horiz = new NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag();
		NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag vert = new NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag();

		private void nplot_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (ModifierKeys == Keys.Control)
					rbs.DoMouseDown(e, this);
				else if (ModifierKeys == Keys.Shift)
				{
					horiz.DoMouseDown(e, this);
					vert.DoMouseDown(e, this);
				}
			}
		}
		private void nplot_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (ModifierKeys == Keys.Control)
				{
					rbs.DoMouseMove(e, this, null);
				}
				else if (ModifierKeys == Keys.Shift)
				{
					horiz.DoMouseMove(e, this, null);
					vert.DoMouseMove(e, this, null);
					Refresh();
				}
			}
		}
		private void nplot_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (ModifierKeys == Keys.Control)
				{
					rbs.DoMouseUp(e, this);
					Refresh();
				}
				else if (ModifierKeys == Keys.Shift)
				{
					horiz.DoMouseUp(e, this);
					vert.DoMouseUp(e, this);
					Refresh();
				}
			}

		}

		#endregion
	}
}
