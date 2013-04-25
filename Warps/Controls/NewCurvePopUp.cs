using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class NewCurvePopUp : Form
	{
		public NewCurvePopUp()
		{
			InitializeComponent();

		}

		private void m_geodesicRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (m_geodesicRadio.Checked)
				AddGeoFitpointsToDataGrid();
			else if (m_surfaceRadio.Checked)
				AddSurfaceFitpointsToDataGrid();

		}

		private void AddGeoFitpointsToDataGrid()
		{

		}

		private void AddSurfaceFitpointsToDataGrid()
		{


		}
	}
}
