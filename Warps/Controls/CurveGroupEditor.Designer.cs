namespace Warps
{
	partial class CurveGroupEditor
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
			this.components = new System.ComponentModel.Container();
			this.m_count = new System.Windows.Forms.Label();
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.m_grid = new System.Windows.Forms.ListView();
			this.m_lblCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.m_fitsCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.m_lngthCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.m_girCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.m_popup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addCur = new System.Windows.Forms.ToolStripMenuItem();
			this.delCur = new System.Windows.Forms.ToolStripMenuItem();
			this.m_popup.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_count
			// 
			this.m_count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_count.AutoSize = true;
			this.m_count.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_count.Location = new System.Drawing.Point(219, 9);
			this.m_count.Name = "m_count";
			this.m_count.Size = new System.Drawing.Size(25, 13);
			this.m_count.TabIndex = 4;
			this.m_count.Text = "###";
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_labelTextBox.Location = new System.Drawing.Point(3, 6);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(210, 20);
			this.m_labelTextBox.TabIndex = 5;
			// 
			// m_grid
			// 
			this.m_grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_grid.BackColor = System.Drawing.SystemColors.Control;
			this.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_grid.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_lblCol,
            this.m_fitsCol,
            this.m_lngthCol,
            this.m_girCol});
			this.m_grid.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_grid.FullRowSelect = true;
			this.m_grid.GridLines = true;
			this.m_grid.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_grid.Location = new System.Drawing.Point(6, 32);
			this.m_grid.Name = "m_grid";
			this.m_grid.Size = new System.Drawing.Size(244, 265);
			this.m_grid.TabIndex = 6;
			this.m_grid.UseCompatibleStateImageBehavior = false;
			this.m_grid.View = System.Windows.Forms.View.Details;
			this.m_grid.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
			// 
			// m_lblCol
			// 
			this.m_lblCol.Text = "Label";
			this.m_lblCol.Width = 98;
			// 
			// m_fitsCol
			// 
			this.m_fitsCol.Text = "# Fits";
			this.m_fitsCol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.m_fitsCol.Width = 69;
			// 
			// m_lngthCol
			// 
			this.m_lngthCol.Text = "Length";
			this.m_lngthCol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.m_lngthCol.Width = 66;
			// 
			// m_girCol
			// 
			this.m_girCol.Text = "Girths";
			this.m_girCol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// m_popup
			// 
			this.m_popup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCur,
            this.delCur});
			this.m_popup.Name = "m_popup";
			this.m_popup.Size = new System.Drawing.Size(153, 70);
			this.m_popup.Opening += new System.ComponentModel.CancelEventHandler(this.m_popup_Opening);
			// 
			// addCur
			// 
			this.addCur.Name = "addCur";
			this.addCur.Size = new System.Drawing.Size(152, 22);
			this.addCur.Text = "Add Curve";
			// 
			// delCur
			// 
			this.delCur.Name = "delCur";
			this.delCur.Size = new System.Drawing.Size(152, 22);
			this.delCur.Text = "Delete Curve";
			// 
			// CurveGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.m_popup;
			this.Controls.Add(this.m_grid);
			this.Controls.Add(this.m_labelTextBox);
			this.Controls.Add(this.m_count);
			this.Name = "CurveGroupEditor";
			this.Size = new System.Drawing.Size(250, 300);
			this.m_popup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_count;
		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.ListView m_grid;
		private System.Windows.Forms.ColumnHeader m_lblCol;
		private System.Windows.Forms.ColumnHeader m_fitsCol;
		private System.Windows.Forms.ColumnHeader m_lngthCol;
		private System.Windows.Forms.ColumnHeader m_girCol;
		private System.Windows.Forms.ContextMenuStrip m_popup;
		internal System.Windows.Forms.ToolStripMenuItem addCur;
		internal System.Windows.Forms.ToolStripMenuItem delCur;
	}
}
