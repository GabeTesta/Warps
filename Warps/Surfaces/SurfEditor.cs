using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class SurfEditor : UserControl
	{
		public SurfEditor()
		{
			InitializeComponent();
			InitializeGrid();
		}
		private void InitializeGrid()
		{
				m_grid.Columns.Add("uCol", "u");
				m_grid.Columns.Add("vCol", "v");
				m_grid.Columns.Add("dCol", "Density");

			
			int i = 0;
			for (i = 0; i < 3; i++)
			{
				m_grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
				m_grid.Columns[i].FillWeight = 25;
				m_grid.Columns[i].DefaultCellStyle.Font = new Font("Consolas", 8f);
				m_grid.Columns[i].ValueType = typeof(double);
				m_grid.Columns[i].DefaultCellStyle.Format = "f4";
			}
			i--;
			m_grid.Columns[i].FillWeight = 50;
			m_grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}

		public string Label
		{
			get { return m_labelTextBox.Text; }
			set { m_labelTextBox.Text = value; }
		}
		public void ReadSurf(GuideSurface surf)
		{
			Label = surf.Label;
			//m_grid.DataSource = surf.FitPoints;
			m_grid.Rows.Clear();
			surf.FitPoints.ForEach(pt => 	m_grid.Rows.Add(pt[0], pt[1], pt[2]));
		}
		public void WriteSurf(GuideSurface surf)
		{
			surf.Label = Label;
			surf.FitPoints.Clear();
			Vect3 v;
			foreach (DataGridViewRow row in m_grid.Rows)
			{
				v = ParseRow(row);
				if( v != null )
				surf.FitPoints.Add(v);
			}
		}

		private Vect3 ParseRow(DataGridViewRow row)
		{
			try
			{
				return new Vect3((double)row.Cells[0].Value, (double)row.Cells[1].Value, (double)row.Cells[2].Value);
			}
			catch { return null; }
		}

		public event EventHandler UpdatedSurface;
		//private void m_grid_UserAddedRow(object sender, DataGridViewRowEventArgs e)
		//{
		//	if (UpdatedSurface != null)
		//		UpdatedSurface(this, new EventArgs());
		//}

		private void m_grid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			if (UpdatedSurface != null)
				UpdatedSurface(this, new EventArgs());
		}

		private void m_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if( ParseRow(m_grid.Rows[e.RowIndex]) != null )
				if (UpdatedSurface != null)
					UpdatedSurface(this, new EventArgs());
		}
	}
}
