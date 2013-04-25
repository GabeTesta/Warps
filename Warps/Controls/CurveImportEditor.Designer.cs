namespace Warps
{
	partial class CurveImportEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurveImportEditor));
			this.m_scriptBox = new System.Windows.Forms.TextBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.m_import = new System.Windows.Forms.ToolStripButton();
			this.m_curveEdit = new Warps.CurveEditor();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_scriptBox
			// 
			this.m_scriptBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_scriptBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_scriptBox.Location = new System.Drawing.Point(0, 0);
			this.m_scriptBox.Multiline = true;
			this.m_scriptBox.Name = "m_scriptBox";
			this.m_scriptBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.m_scriptBox.Size = new System.Drawing.Size(300, 150);
			this.m_scriptBox.TabIndex = 0;
			this.m_scriptBox.WordWrap = false;
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_import});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(300, 25);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// m_import
			// 
			this.m_import.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_import.Image = ((System.Drawing.Image)(resources.GetObject("m_import.Image")));
			this.m_import.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_import.Name = "m_import";
			this.m_import.Size = new System.Drawing.Size(47, 22);
			this.m_import.Text = "Import";
			// 
			// m_curveEdit
			// 
			this.m_curveEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_curveEdit.Location = new System.Drawing.Point(0, 25);
			this.m_curveEdit.Name = "m_curveEdit";
			this.m_curveEdit.Size = new System.Drawing.Size(300, 121);
			this.m_curveEdit.TabIndex = 1;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.m_scriptBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.m_curveEdit);
			this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
			this.splitContainer1.Size = new System.Drawing.Size(300, 300);
			this.splitContainer1.SplitterDistance = 150;
			this.splitContainer1.TabIndex = 3;
			// 
			// CurveImportEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "CurveImportEditor";
			this.Size = new System.Drawing.Size(300, 300);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox m_scriptBox;
		private CurveEditor m_curveEdit;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton m_import;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
