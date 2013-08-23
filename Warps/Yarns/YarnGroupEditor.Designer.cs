namespace Warps.Controls
{
	partial class YarnGroupEditor
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
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.yarnDenierEQB = new Warps.Controls.EquationBoxAuto();
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.m_densityLocTextBox = new System.Windows.Forms.TextBox();
			this.targetDPIEQB = new Warps.Controls.EquationBoxAuto();
			this.outputGroup = new System.Windows.Forms.GroupBox();
			this.m_yarnCountOut = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.m_achievedDPI = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.m_endingList = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.yarnMat = new System.Windows.Forms.GroupBox();
			this.m_yarnCombo = new System.Windows.Forms.ComboBox();
			this.groupBox5.SuspendLayout();
			this.outputGroup.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.yarnMat.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(3, 3);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(166, 20);
			this.m_labelTextBox.TabIndex = 7;
			// 
			// m_guideListView
			// 
			this.m_guideListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(3, 140);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(166, 22);
			this.m_guideListView.TabIndex = 6;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			this.m_guideListView.DoubleClick += new System.EventHandler(this.m_guideListView_DoubleClick);
			// 
			// selectGuideButt
			// 
			this.selectGuideButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectGuideButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectGuideButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectGuideButt.Location = new System.Drawing.Point(3, 161);
			this.selectGuideButt.Name = "selectGuideButt";
			this.selectGuideButt.Size = new System.Drawing.Size(166, 23);
			this.selectGuideButt.TabIndex = 6;
			this.selectGuideButt.Text = "Select Guide";
			this.selectGuideButt.UseVisualStyleBackColor = false;
			this.selectGuideButt.Click += new System.EventHandler(this.selectGuideButt_Click);
			// 
			// yarnDenierEQB
			// 
			this.yarnDenierEQB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.yarnDenierEQB.AutoFillVariables = null;
			this.yarnDenierEQB.Location = new System.Drawing.Point(3, 203);
			this.yarnDenierEQB.Name = "yarnDenierEQB";
			this.yarnDenierEQB.Size = new System.Drawing.Size(166, 23);
			this.yarnDenierEQB.TabIndex = 2;
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(3, 111);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(166, 23);
			this.selectWarpButt.TabIndex = 5;
			this.selectWarpButt.Text = "Select Warps";
			this.selectWarpButt.UseVisualStyleBackColor = false;
			this.selectWarpButt.Click += new System.EventHandler(this.button1_Click);
			// 
			// m_warpListView
			// 
			this.m_warpListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_warpListView.AutoArrange = false;
			this.m_warpListView.Location = new System.Drawing.Point(3, 29);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(166, 83);
			this.m_warpListView.TabIndex = 4;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			this.m_warpListView.DoubleClick += new System.EventHandler(this.m_warpListView_DoubleClick);
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.m_densityLocTextBox);
			this.groupBox5.Location = new System.Drawing.Point(3, 275);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(169, 50);
			this.groupBox5.TabIndex = 11;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Density Curve Locations";
			// 
			// m_densityLocTextBox
			// 
			this.m_densityLocTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_densityLocTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_densityLocTextBox.Location = new System.Drawing.Point(3, 16);
			this.m_densityLocTextBox.Name = "m_densityLocTextBox";
			this.m_densityLocTextBox.Size = new System.Drawing.Size(163, 20);
			this.m_densityLocTextBox.TabIndex = 0;
			this.m_densityLocTextBox.Text = "0.25, 0.5, 0.75";
			this.m_densityLocTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// targetDPIEQB
			// 
			this.targetDPIEQB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.targetDPIEQB.AutoFillVariables = null;
			this.targetDPIEQB.Location = new System.Drawing.Point(3, 246);
			this.targetDPIEQB.Name = "targetDPIEQB";
			this.targetDPIEQB.Size = new System.Drawing.Size(166, 23);
			this.targetDPIEQB.TabIndex = 3;
			// 
			// outputGroup
			// 
			this.outputGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputGroup.Controls.Add(this.m_yarnCountOut);
			this.outputGroup.Controls.Add(this.label2);
			this.outputGroup.Controls.Add(this.m_achievedDPI);
			this.outputGroup.Controls.Add(this.label1);
			this.outputGroup.Enabled = false;
			this.outputGroup.Location = new System.Drawing.Point(3, 421);
			this.outputGroup.Name = "outputGroup";
			this.outputGroup.Size = new System.Drawing.Size(168, 65);
			this.outputGroup.TabIndex = 12;
			this.outputGroup.TabStop = false;
			this.outputGroup.Text = "Output";
			// 
			// m_yarnCountOut
			// 
			this.m_yarnCountOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_yarnCountOut.Enabled = false;
			this.m_yarnCountOut.Location = new System.Drawing.Point(91, 37);
			this.m_yarnCountOut.Name = "m_yarnCountOut";
			this.m_yarnCountOut.Size = new System.Drawing.Size(68, 20);
			this.m_yarnCountOut.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Yarn Count:";
			// 
			// m_achievedDPI
			// 
			this.m_achievedDPI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_achievedDPI.Enabled = false;
			this.m_achievedDPI.Location = new System.Drawing.Point(91, 13);
			this.m_achievedDPI.Name = "m_achievedDPI";
			this.m_achievedDPI.Size = new System.Drawing.Size(68, 20);
			this.m_achievedDPI.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Archieved DPI:";
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.m_endingList);
			this.groupBox6.Location = new System.Drawing.Point(3, 331);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(168, 39);
			this.groupBox6.TabIndex = 13;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Ending Condition";
			// 
			// m_endingList
			// 
			this.m_endingList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_endingList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_endingList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_endingList.FormattingEnabled = true;
			this.m_endingList.Location = new System.Drawing.Point(3, 16);
			this.m_endingList.Name = "m_endingList";
			this.m_endingList.Size = new System.Drawing.Size(162, 21);
			this.m_endingList.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 187);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Yarn Denier";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 230);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(59, 13);
			this.label4.TabIndex = 14;
			this.label4.Text = "Target DPI";
			// 
			// yarnMat
			// 
			this.yarnMat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.yarnMat.Controls.Add(this.m_yarnCombo);
			this.yarnMat.Location = new System.Drawing.Point(3, 376);
			this.yarnMat.Name = "yarnMat";
			this.yarnMat.Size = new System.Drawing.Size(168, 39);
			this.yarnMat.TabIndex = 13;
			this.yarnMat.TabStop = false;
			this.yarnMat.Text = "Yarn Material";
			// 
			// m_yarnCombo
			// 
			this.m_yarnCombo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_yarnCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_yarnCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_yarnCombo.FormattingEnabled = true;
			this.m_yarnCombo.Location = new System.Drawing.Point(3, 16);
			this.m_yarnCombo.Name = "m_yarnCombo";
			this.m_yarnCombo.Size = new System.Drawing.Size(162, 21);
			this.m_yarnCombo.TabIndex = 0;
			// 
			// YarnGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.targetDPIEQB);
			this.Controls.Add(this.yarnDenierEQB);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.selectGuideButt);
			this.Controls.Add(this.selectWarpButt);
			this.Controls.Add(this.yarnMat);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.m_warpListView);
			this.Controls.Add(this.outputGroup);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.m_labelTextBox);
			this.Controls.Add(this.m_guideListView);
			this.Name = "YarnGroupEditor";
			this.Size = new System.Drawing.Size(175, 486);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.outputGroup.ResumeLayout(false);
			this.outputGroup.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.yarnMat.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.ListView m_guideListView;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox m_densityLocTextBox;
		private EquationBoxAuto yarnDenierEQB;
		private EquationBoxAuto targetDPIEQB;
		private System.Windows.Forms.GroupBox outputGroup;
		private System.Windows.Forms.TextBox m_achievedDPI;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ComboBox m_endingList;
		private System.Windows.Forms.TextBox m_yarnCountOut;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox yarnMat;
		private System.Windows.Forms.ComboBox m_yarnCombo;
	}
}
