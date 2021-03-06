﻿namespace Warps.Controls
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
			Warps.Equation equation2 = new Warps.Equation();
			Warps.Equation equation1 = new Warps.Equation();
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.m_label = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.m_densityLocTextBox = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.outputGroup = new System.Windows.Forms.GroupBox();
			this.m_achievedDPI = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.m_endingList = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.m_yarnCountOut = new System.Windows.Forms.TextBox();
			this.targetDPIEQB = new Warps.Controls.EquationBoxAuto();
			this.yarnDenierEQB = new Warps.Controls.EquationBoxAuto();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.outputGroup.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(56, 3);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(116, 20);
			this.m_labelTextBox.TabIndex = 7;
			// 
			// m_label
			// 
			this.m_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(7, 6);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(56, 14);
			this.m_label.TabIndex = 6;
			this.m_label.Text = "Label: ";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.m_guideListView);
			this.groupBox1.Controls.Add(this.selectGuideButt);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox1.Location = new System.Drawing.Point(3, 176);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(169, 95);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Guide Comb Selection";
			// 
			// m_guideListView
			// 
			this.m_guideListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(4, 15);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(159, 38);
			this.m_guideListView.TabIndex = 6;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			this.m_guideListView.DoubleClick += new System.EventHandler(this.m_guideListView_DoubleClick);
			// 
			// selectGuideButt
			// 
			this.selectGuideButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.selectGuideButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectGuideButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectGuideButt.Location = new System.Drawing.Point(19, 59);
			this.selectGuideButt.Name = "selectGuideButt";
			this.selectGuideButt.Size = new System.Drawing.Size(134, 23);
			this.selectGuideButt.TabIndex = 6;
			this.selectGuideButt.Text = "Select Guide";
			this.selectGuideButt.UseVisualStyleBackColor = false;
			this.selectGuideButt.Click += new System.EventHandler(this.selectGuideButt_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.yarnDenierEQB);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox2.Location = new System.Drawing.Point(3, 277);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(169, 55);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Yarn Denier";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.selectWarpButt);
			this.groupBox4.Controls.Add(this.m_warpListView);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox4.Location = new System.Drawing.Point(3, 29);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(169, 141);
			this.groupBox4.TabIndex = 10;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Warp Selection";
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(19, 110);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(134, 23);
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
			this.m_warpListView.Location = new System.Drawing.Point(7, 19);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(156, 85);
			this.m_warpListView.TabIndex = 4;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			this.m_warpListView.DoubleClick += new System.EventHandler(this.m_warpListView_DoubleClick);
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.m_densityLocTextBox);
			this.groupBox5.Location = new System.Drawing.Point(3, 394);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(169, 50);
			this.groupBox5.TabIndex = 11;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Density Curve Locations";
			// 
			// m_densityLocTextBox
			// 
			this.m_densityLocTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_densityLocTextBox.Location = new System.Drawing.Point(3, 16);
			this.m_densityLocTextBox.Name = "m_densityLocTextBox";
			this.m_densityLocTextBox.Size = new System.Drawing.Size(163, 20);
			this.m_densityLocTextBox.TabIndex = 0;
			this.m_densityLocTextBox.Text = "0.25, 0.5, 0.75";
			this.m_densityLocTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.targetDPIEQB);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox3.Location = new System.Drawing.Point(3, 333);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(172, 55);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Target DPI";
			// 
			// outputGroup
			// 
			this.outputGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputGroup.Controls.Add(this.m_yarnCountOut);
			this.outputGroup.Controls.Add(this.label2);
			this.outputGroup.Controls.Add(this.m_achievedDPI);
			this.outputGroup.Controls.Add(this.label1);
			this.outputGroup.Enabled = false;
			this.outputGroup.Location = new System.Drawing.Point(4, 496);
			this.outputGroup.Name = "outputGroup";
			this.outputGroup.Size = new System.Drawing.Size(168, 65);
			this.outputGroup.TabIndex = 12;
			this.outputGroup.TabStop = false;
			this.outputGroup.Text = "Output";
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
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.m_endingList);
			this.groupBox6.Location = new System.Drawing.Point(4, 451);
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
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Yarn Count:";
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
			// targetDPIEQB
			// 
			this.targetDPIEQB.AutoFillVariables = null;
			this.targetDPIEQB.Dock = System.Windows.Forms.DockStyle.Fill;
			equation2.EquationText = "0.0000";
			equation2.Label = "eq";
			//equation2.Result = 0D;
		//	equation2.sail = null;
			this.targetDPIEQB.Equation = equation2;
			this.targetDPIEQB.Location = new System.Drawing.Point(3, 16);
			this.targetDPIEQB.MaximumSize = new System.Drawing.Size(201, 24);
			this.targetDPIEQB.Name = "targetDPIEQB";
			this.targetDPIEQB.sail = null;
			this.targetDPIEQB.Size = new System.Drawing.Size(166, 24);
			this.targetDPIEQB.TabIndex = 3;
			this.targetDPIEQB.Value = 0D;
			// 
			// yarnDenierEQB
			// 
			this.yarnDenierEQB.AutoFillVariables = null;
			this.yarnDenierEQB.Dock = System.Windows.Forms.DockStyle.Fill;
			equation1.EquationText = "0.0000";
			equation1.Label = "eq";
			//equation1.Result = 0D;
			//equation1.sail = null;
			this.yarnDenierEQB.Equation = equation1;
			this.yarnDenierEQB.Location = new System.Drawing.Point(3, 16);
			this.yarnDenierEQB.MaximumSize = new System.Drawing.Size(201, 24);
			this.yarnDenierEQB.Name = "yarnDenierEQB";
			this.yarnDenierEQB.sail = null;
			this.yarnDenierEQB.Size = new System.Drawing.Size(163, 24);
			this.yarnDenierEQB.TabIndex = 2;
			this.yarnDenierEQB.Value = 0D;
			// 
			// YarnGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.outputGroup);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.m_labelTextBox);
			this.Controls.Add(this.m_label);
			this.Name = "YarnGroupEditor";
			this.Size = new System.Drawing.Size(175, 564);
			this.Load += new System.EventHandler(this.YarnGroupEditor_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.outputGroup.ResumeLayout(false);
			this.outputGroup.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.ListView m_guideListView;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox m_densityLocTextBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private EquationBoxAuto yarnDenierEQB;
		private EquationBoxAuto targetDPIEQB;
		private System.Windows.Forms.GroupBox outputGroup;
		private System.Windows.Forms.TextBox m_achievedDPI;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ComboBox m_endingList;
		private System.Windows.Forms.TextBox m_yarnCountOut;
		private System.Windows.Forms.Label label2;
	}
}
