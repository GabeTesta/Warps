﻿namespace Warps.Controls
{
	partial class GuideEditor
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_dgv = new System.Windows.Forms.DataGridView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.m_nsPlot = new Warps.NsPlot();
			this.m_curveEditor = new Warps.Controls.MouldCurveEditor();
			((System.ComponentModel.ISupportInitialize)(this.m_dgv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_dgv
			// 
			this.m_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_dgv.Location = new System.Drawing.Point(0, 0);
			this.m_dgv.Margin = new System.Windows.Forms.Padding(0);
			this.m_dgv.Name = "m_dgv";
			this.m_dgv.Size = new System.Drawing.Size(250, 125);
			this.m_dgv.TabIndex = 0;
			this.m_dgv.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dgv_CellValueChanged);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 159);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.m_nsPlot);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.m_dgv);
			this.splitContainer1.Size = new System.Drawing.Size(250, 420);
			this.splitContainer1.SplitterDistance = 291;
			this.splitContainer1.TabIndex = 6;
			// 
			// m_nsPlot
			// 
			this.m_nsPlot.AutoScaleAutoGeneratedAxes = false;
			this.m_nsPlot.AutoScaleTitle = false;
			this.m_nsPlot.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.m_nsPlot.Cursor = System.Windows.Forms.Cursors.Cross;
			this.m_nsPlot.DateTimeToolTip = false;
			this.m_nsPlot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_nsPlot.Legend = null;
			this.m_nsPlot.LegendZOrder = -1;
			this.m_nsPlot.Location = new System.Drawing.Point(0, 0);
			this.m_nsPlot.Name = "m_nsPlot";
			this.m_nsPlot.RightMenu = null;
			this.m_nsPlot.ShowCoordinates = true;
			this.m_nsPlot.Size = new System.Drawing.Size(250, 291);
			this.m_nsPlot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			this.m_nsPlot.TabIndex = 5;
			this.m_nsPlot.Text = "Comb";
			this.m_nsPlot.Title = "";
			this.m_nsPlot.TitleFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.m_nsPlot.XAxis1 = null;
			this.m_nsPlot.XAxis2 = null;
			this.m_nsPlot.YAxis1 = null;
			this.m_nsPlot.YAxis2 = null;
			// 
			// m_curveEditor
			// 
			this.m_curveEditor.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_curveEditor.Label = "";
			this.m_curveEditor.Location = new System.Drawing.Point(0, 0);
			this.m_curveEditor.Name = "m_curveEditor";
			this.m_curveEditor.Size = new System.Drawing.Size(250, 159);
			this.m_curveEditor.TabIndex = 0;
			// 
			// GuideEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.m_curveEditor);
			this.Name = "GuideEditor";
			this.Size = new System.Drawing.Size(250, 579);
			((System.ComponentModel.ISupportInitialize)(this.m_dgv)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView m_dgv;
		private NsPlot m_nsPlot;
		private MouldCurveEditor m_curveEditor;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
