﻿namespace Warps
{
	partial class WarpFrame
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarpFrame));
			this.m_vertsplit = new System.Windows.Forms.SplitContainer();
			this.m_horizsplit = new System.Windows.Forms.SplitContainer();
			this.m_tree = new Warps.TabTree();
			this.previewButton = new System.Windows.Forms.Button();
			this.editPanel = new System.Windows.Forms.Panel();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.m_dualView = new Warps.DualView();
			this.m_toolstrip = new System.Windows.Forms.ToolStrip();
			this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.m_statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.m_curveStrip = new System.Windows.Forms.ToolStrip();
			this.m_addCurve = new System.Windows.Forms.ToolStripButton();
			this.m_modCurve = new System.Windows.Forms.ToolStripButton();
			this.m_delCurve = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.m_editButton = new System.Windows.Forms.ToolStripButton();
			this.m_buildStrip = new System.Windows.Forms.ToolStrip();
			this.m_autoBtn = new System.Windows.Forms.ToolStripButton();
			this.m_buildBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.m_cancelBtn = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.m_vertsplit)).BeginInit();
			this.m_vertsplit.Panel1.SuspendLayout();
			this.m_vertsplit.Panel2.SuspendLayout();
			this.m_vertsplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_horizsplit)).BeginInit();
			this.m_horizsplit.Panel1.SuspendLayout();
			this.m_horizsplit.Panel2.SuspendLayout();
			this.m_horizsplit.SuspendLayout();
			this.m_toolstrip.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.m_curveStrip.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.m_buildStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_vertsplit
			// 
			this.m_vertsplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_vertsplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.m_vertsplit.Location = new System.Drawing.Point(0, 0);
			this.m_vertsplit.Name = "m_vertsplit";
			// 
			// m_vertsplit.Panel1
			// 
			this.m_vertsplit.Panel1.Controls.Add(this.m_horizsplit);
			// 
			// m_vertsplit.Panel2
			// 
			this.m_vertsplit.Panel2.Controls.Add(this.m_dualView);
			this.m_vertsplit.Size = new System.Drawing.Size(1022, 448);
			this.m_vertsplit.SplitterDistance = 405;
			this.m_vertsplit.TabIndex = 0;
			// 
			// m_horizsplit
			// 
			this.m_horizsplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_horizsplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.m_horizsplit.Location = new System.Drawing.Point(0, 0);
			this.m_horizsplit.Name = "m_horizsplit";
			// 
			// m_horizsplit.Panel1
			// 
			this.m_horizsplit.Panel1.Controls.Add(this.m_tree);
			// 
			// m_horizsplit.Panel2
			// 
			this.m_horizsplit.Panel2.Controls.Add(this.previewButton);
			this.m_horizsplit.Panel2.Controls.Add(this.editPanel);
			this.m_horizsplit.Panel2.Controls.Add(this.cancelButton);
			this.m_horizsplit.Panel2.Controls.Add(this.okButton);
			this.m_horizsplit.Size = new System.Drawing.Size(405, 448);
			this.m_horizsplit.SplitterDistance = 205;
			this.m_horizsplit.TabIndex = 0;
			// 
			// m_tree
			// 
			this.m_tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tree.EditMode = false;
			this.m_tree.Location = new System.Drawing.Point(0, 0);
			this.m_tree.Name = "m_tree";
			this.m_tree.SelectedTag = null;
			this.m_tree.Size = new System.Drawing.Size(205, 448);
			this.m_tree.TabIndex = 0;
			// 
			// previewButton
			// 
			this.previewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.previewButton.BackColor = System.Drawing.Color.White;
			this.previewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.previewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.previewButton.Location = new System.Drawing.Point(12, 422);
			this.previewButton.Name = "previewButton";
			this.previewButton.Size = new System.Drawing.Size(75, 23);
			this.previewButton.TabIndex = 3;
			this.previewButton.Text = "Preview";
			this.previewButton.UseVisualStyleBackColor = false;
			this.previewButton.MouseEnter += new System.EventHandler(this.okButton_MouseEnter);
			this.previewButton.MouseLeave += new System.EventHandler(this.okButton_MouseLeave);
			// 
			// editPanel
			// 
			this.editPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.editPanel.Location = new System.Drawing.Point(3, 3);
			this.editPanel.Name = "editPanel";
			this.editPanel.Size = new System.Drawing.Size(193, 413);
			this.editPanel.TabIndex = 2;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.BackColor = System.Drawing.Color.White;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelButton.Location = new System.Drawing.Point(114, 422);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = false;
			this.cancelButton.MouseEnter += new System.EventHandler(this.okButton_MouseEnter);
			this.cancelButton.MouseLeave += new System.EventHandler(this.okButton_MouseLeave);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.BackColor = System.Drawing.Color.White;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.okButton.Location = new System.Drawing.Point(33, 422);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.MouseEnter += new System.EventHandler(this.okButton_MouseEnter);
			this.okButton.MouseLeave += new System.EventHandler(this.okButton_MouseLeave);
			// 
			// m_dualView
			// 
			this.m_dualView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_dualView.EditMode = false;
			this.m_dualView.Location = new System.Drawing.Point(0, 0);
			this.m_dualView.Name = "m_dualView";
			this.m_dualView.Size = new System.Drawing.Size(613, 448);
			this.m_dualView.TabIndex = 0;
			// 
			// m_toolstrip
			// 
			this.m_toolstrip.Dock = System.Windows.Forms.DockStyle.None;
			this.m_toolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.m_toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.printToolStripButton,
            this.toolStripSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.helpToolStripButton});
			this.m_toolstrip.Location = new System.Drawing.Point(3, 75);
			this.m_toolstrip.Name = "m_toolstrip";
			this.m_toolstrip.Size = new System.Drawing.Size(199, 25);
			this.m_toolstrip.TabIndex = 1;
			this.m_toolstrip.Text = "toolStrip1";
			// 
			// newToolStripButton
			// 
			this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
			this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newToolStripButton.Name = "newToolStripButton";
			this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.newToolStripButton.Text = "&New";
			this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
			// 
			// openToolStripButton
			// 
			this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
			this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openToolStripButton.Name = "openToolStripButton";
			this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openToolStripButton.Text = "&Open";
			this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
			// 
			// saveToolStripButton
			// 
			this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
			this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveToolStripButton.Name = "saveToolStripButton";
			this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveToolStripButton.Text = "&Save";
			this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
			// 
			// printToolStripButton
			// 
			this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
			this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.printToolStripButton.Name = "printToolStripButton";
			this.printToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.printToolStripButton.Text = "&Print";
			// 
			// toolStripSeparator
			// 
			this.toolStripSeparator.Name = "toolStripSeparator";
			this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
			// 
			// cutToolStripButton
			// 
			this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
			this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cutToolStripButton.Name = "cutToolStripButton";
			this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.cutToolStripButton.Text = "C&ut";
			// 
			// copyToolStripButton
			// 
			this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
			this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.copyToolStripButton.Name = "copyToolStripButton";
			this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.copyToolStripButton.Text = "&Copy";
			// 
			// pasteToolStripButton
			// 
			this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
			this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.pasteToolStripButton.Name = "pasteToolStripButton";
			this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.pasteToolStripButton.Text = "&Paste";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// helpToolStripButton
			// 
			this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
			this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.helpToolStripButton.Name = "helpToolStripButton";
			this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.helpToolStripButton.Text = "He&lp";
			this.helpToolStripButton.Click += new System.EventHandler(this.helpToolStripButton_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusText});
			this.statusStrip1.Location = new System.Drawing.Point(0, 448);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1022, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// m_statusText
			// 
			this.m_statusText.Name = "m_statusText";
			this.m_statusText.Size = new System.Drawing.Size(39, 17);
			this.m_statusText.Text = "Ready";
			// 
			// toolStripContainer1
			// 
			this.toolStripContainer1.BottomToolStripPanelVisible = false;
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.AutoScroll = true;
			this.toolStripContainer1.ContentPanel.Controls.Add(this.m_vertsplit);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1022, 470);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Size = new System.Drawing.Size(1022, 570);
			this.toolStripContainer1.TabIndex = 3;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.m_toolstrip);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.m_curveStrip);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.m_buildStrip);
			// 
			// m_curveStrip
			// 
			this.m_curveStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.m_curveStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_addCurve,
            this.m_modCurve,
            this.m_delCurve});
			this.m_curveStrip.Location = new System.Drawing.Point(69, 50);
			this.m_curveStrip.Name = "m_curveStrip";
			this.m_curveStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.m_curveStrip.Size = new System.Drawing.Size(112, 25);
			this.m_curveStrip.TabIndex = 2;
			// 
			// m_addCurve
			// 
			this.m_addCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_addCurve.Image = ((System.Drawing.Image)(resources.GetObject("m_addCurve.Image")));
			this.m_addCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_addCurve.Name = "m_addCurve";
			this.m_addCurve.Size = new System.Drawing.Size(23, 22);
			this.m_addCurve.Text = "Add";
			this.m_addCurve.Click += new System.EventHandler(this.m_addCurve_Click);
			// 
			// m_modCurve
			// 
			this.m_modCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_modCurve.Image = ((System.Drawing.Image)(resources.GetObject("m_modCurve.Image")));
			this.m_modCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_modCurve.Name = "m_modCurve";
			this.m_modCurve.Size = new System.Drawing.Size(23, 22);
			this.m_modCurve.Text = "Mod";
			// 
			// m_delCurve
			// 
			this.m_delCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_delCurve.Image = ((System.Drawing.Image)(resources.GetObject("m_delCurve.Image")));
			this.m_delCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_delCurve.Name = "m_delCurve";
			this.m_delCurve.Size = new System.Drawing.Size(23, 22);
			this.m_delCurve.Text = "Del";
			this.m_delCurve.Click += new System.EventHandler(this.m_delCurve_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_editButton});
			this.toolStrip1.Location = new System.Drawing.Point(107, 25);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(43, 25);
			this.toolStrip1.TabIndex = 4;
			// 
			// m_editButton
			// 
			this.m_editButton.CheckOnClick = true;
			this.m_editButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_editButton.ForeColor = System.Drawing.Color.Black;
			this.m_editButton.Image = ((System.Drawing.Image)(resources.GetObject("m_editButton.Image")));
			this.m_editButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_editButton.Name = "m_editButton";
			this.m_editButton.Size = new System.Drawing.Size(31, 22);
			this.m_editButton.Text = "Edit";
			this.m_editButton.CheckedChanged += new System.EventHandler(this.m_editButton_CheckedChanged);
			// 
			// m_buildStrip
			// 
			this.m_buildStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.m_buildStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_autoBtn,
            this.m_buildBtn,
            this.toolStripSeparator2,
            this.m_cancelBtn});
			this.m_buildStrip.Location = new System.Drawing.Point(10, 0);
			this.m_buildStrip.Name = "m_buildStrip";
			this.m_buildStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.m_buildStrip.Size = new System.Drawing.Size(140, 25);
			this.m_buildStrip.TabIndex = 3;
			// 
			// m_autoBtn
			// 
			this.m_autoBtn.CheckOnClick = true;
			this.m_autoBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_autoBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_autoBtn.Image")));
			this.m_autoBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_autoBtn.Name = "m_autoBtn";
			this.m_autoBtn.Size = new System.Drawing.Size(37, 22);
			this.m_autoBtn.Text = "Auto";
			this.m_autoBtn.ToolTipText = "Toggle Rolling Rebuilds";
			this.m_autoBtn.CheckedChanged += new System.EventHandler(this.m_autoBtn_CheckedChanged);
			// 
			// m_buildBtn
			// 
			this.m_buildBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_buildBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_buildBtn.Image")));
			this.m_buildBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_buildBtn.Name = "m_buildBtn";
			this.m_buildBtn.Size = new System.Drawing.Size(38, 22);
			this.m_buildBtn.Text = "Build";
			this.m_buildBtn.Click += new System.EventHandler(this.m_buildBtn_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// m_cancelBtn
			// 
			this.m_cancelBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_cancelBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_cancelBtn.Image")));
			this.m_cancelBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_cancelBtn.Name = "m_cancelBtn";
			this.m_cancelBtn.Size = new System.Drawing.Size(47, 22);
			this.m_cancelBtn.Text = "Cancel";
			this.m_cancelBtn.Click += new System.EventHandler(this.m_cancelBtn_Click);
			// 
			// WarpFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1022, 570);
			this.Controls.Add(this.toolStripContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "WarpFrame";
			this.Text = "Warps";
			this.m_vertsplit.Panel1.ResumeLayout(false);
			this.m_vertsplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_vertsplit)).EndInit();
			this.m_vertsplit.ResumeLayout(false);
			this.m_horizsplit.Panel1.ResumeLayout(false);
			this.m_horizsplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_horizsplit)).EndInit();
			this.m_horizsplit.ResumeLayout(false);
			this.m_toolstrip.ResumeLayout(false);
			this.m_toolstrip.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.ContentPanel.PerformLayout();
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.m_curveStrip.ResumeLayout(false);
			this.m_curveStrip.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.m_buildStrip.ResumeLayout(false);
			this.m_buildStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer m_vertsplit;
		private System.Windows.Forms.SplitContainer m_horizsplit;
		private System.Windows.Forms.ToolStrip m_toolstrip;
		private System.Windows.Forms.ToolStripButton newToolStripButton;
		private System.Windows.Forms.ToolStripButton openToolStripButton;
		private System.Windows.Forms.ToolStripButton saveToolStripButton;
		private System.Windows.Forms.ToolStripButton printToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
		private System.Windows.Forms.ToolStripButton cutToolStripButton;
		private System.Windows.Forms.ToolStripButton copyToolStripButton;
		private System.Windows.Forms.ToolStripButton pasteToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton helpToolStripButton;
		private TabTree m_tree;
        private DualView m_dualView;
        private System.Windows.Forms.StatusStrip statusStrip1;
	   private System.Windows.Forms.ToolStripStatusLabel m_statusText;
	   private System.Windows.Forms.ToolStripContainer toolStripContainer1;
	   private System.Windows.Forms.ToolStrip m_curveStrip;
	   private System.Windows.Forms.ToolStripButton m_modCurve;
	   private System.Windows.Forms.ToolStripButton m_delCurve;
	   private System.Windows.Forms.ToolStripButton m_addCurve;
	   private System.Windows.Forms.ToolStrip m_buildStrip;
	   private System.Windows.Forms.ToolStripButton m_autoBtn;
	   private System.Windows.Forms.ToolStripButton m_buildBtn;
	   private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	   private System.Windows.Forms.ToolStripButton m_cancelBtn;
	   private System.Windows.Forms.Panel editPanel;
	   public System.Windows.Forms.Button cancelButton;
	   public System.Windows.Forms.Button okButton;
	   public System.Windows.Forms.Button previewButton;
	   private System.Windows.Forms.ToolStrip toolStrip1;
	   private System.Windows.Forms.ToolStripButton m_editButton;
	}
}

