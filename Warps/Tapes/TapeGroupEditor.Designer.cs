namespace Warps.Tapes
{
	partial class TapeGroupEditor
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
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.outputGroup = new System.Windows.Forms.GroupBox();
			this.tapeLen = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tapeCount = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.m_stagger = new System.Windows.Forms.CheckBox();
			this.angleTol = new Warps.Controls.EquationBoxAuto();
			this.chainTol = new Warps.Controls.EquationBoxAuto();
			this.pixLength = new Warps.Controls.EquationBoxAuto();
			this.m_tapeMat = new System.Windows.Forms.GroupBox();
			this.m_tapeCombo = new System.Windows.Forms.ComboBox();
			this.outputGroup.SuspendLayout();
			this.m_tapeMat.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(3, 3);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(169, 20);
			this.m_labelTextBox.TabIndex = 22;
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(3, 175);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(169, 23);
			this.selectWarpButt.TabIndex = 24;
			this.selectWarpButt.Text = "Select Warps";
			this.selectWarpButt.UseVisualStyleBackColor = false;
			this.selectWarpButt.Click += new System.EventHandler(this.selectWarpButt_Click);
			// 
			// m_warpListView
			// 
			this.m_warpListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_warpListView.AutoArrange = false;
			this.m_warpListView.Location = new System.Drawing.Point(3, 74);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(169, 104);
			this.m_warpListView.TabIndex = 23;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			// 
			// selectGuideButt
			// 
			this.selectGuideButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectGuideButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectGuideButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectGuideButt.Location = new System.Drawing.Point(3, 224);
			this.selectGuideButt.Name = "selectGuideButt";
			this.selectGuideButt.Size = new System.Drawing.Size(169, 23);
			this.selectGuideButt.TabIndex = 25;
			this.selectGuideButt.Text = "Select Density Surface";
			this.selectGuideButt.UseVisualStyleBackColor = false;
			this.selectGuideButt.Click += new System.EventHandler(this.selectWarpButt_Click);
			// 
			// m_guideListView
			// 
			this.m_guideListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(3, 203);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(169, 22);
			this.m_guideListView.TabIndex = 26;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 297);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 13);
			this.label4.TabIndex = 29;
			this.label4.Text = "Chain Tolerance";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 254);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 30;
			this.label3.Text = "Pixel Length";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 340);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "Angle Tolerance";
			// 
			// outputGroup
			// 
			this.outputGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputGroup.Controls.Add(this.tapeLen);
			this.outputGroup.Controls.Add(this.label2);
			this.outputGroup.Controls.Add(this.tapeCount);
			this.outputGroup.Controls.Add(this.label5);
			this.outputGroup.Enabled = false;
			this.outputGroup.Location = new System.Drawing.Point(3, 386);
			this.outputGroup.Name = "outputGroup";
			this.outputGroup.Size = new System.Drawing.Size(168, 65);
			this.outputGroup.TabIndex = 31;
			this.outputGroup.TabStop = false;
			this.outputGroup.Text = "Output";
			// 
			// tapeLen
			// 
			this.tapeLen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tapeLen.Enabled = false;
			this.tapeLen.Location = new System.Drawing.Point(91, 37);
			this.tapeLen.Name = "tapeLen";
			this.tapeLen.Size = new System.Drawing.Size(68, 20);
			this.tapeLen.TabIndex = 3;
			this.tapeLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Tape Length:";
			// 
			// tapeCount
			// 
			this.tapeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tapeCount.Enabled = false;
			this.tapeCount.Location = new System.Drawing.Point(91, 13);
			this.tapeCount.Name = "tapeCount";
			this.tapeCount.Size = new System.Drawing.Size(68, 20);
			this.tapeCount.TabIndex = 1;
			this.tapeCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(66, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Tape Count:";
			// 
			// m_stagger
			// 
			this.m_stagger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_stagger.AutoSize = true;
			this.m_stagger.Location = new System.Drawing.Point(109, 253);
			this.m_stagger.Name = "m_stagger";
			this.m_stagger.Size = new System.Drawing.Size(63, 17);
			this.m_stagger.TabIndex = 4;
			this.m_stagger.Text = "Stagger";
			this.m_stagger.UseVisualStyleBackColor = true;
			// 
			// angleTol
			// 
			this.angleTol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.angleTol.AutoFillVariables = null;
			this.angleTol.Location = new System.Drawing.Point(3, 356);
			this.angleTol.Name = "angleTol";
			this.angleTol.Size = new System.Drawing.Size(169, 23);
			this.angleTol.TabIndex = 28;
			// 
			// chainTol
			// 
			this.chainTol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chainTol.AutoFillVariables = null;
			this.chainTol.Location = new System.Drawing.Point(3, 313);
			this.chainTol.Name = "chainTol";
			this.chainTol.Size = new System.Drawing.Size(169, 23);
			this.chainTol.TabIndex = 28;
			// 
			// pixLength
			// 
			this.pixLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pixLength.AutoFillVariables = null;
			this.pixLength.Location = new System.Drawing.Point(3, 270);
			this.pixLength.Name = "pixLength";
			this.pixLength.Size = new System.Drawing.Size(169, 23);
			this.pixLength.TabIndex = 27;
			// 
			// m_tapeMat
			// 
			this.m_tapeMat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tapeMat.Controls.Add(this.m_tapeCombo);
			this.m_tapeMat.Location = new System.Drawing.Point(3, 29);
			this.m_tapeMat.Name = "m_tapeMat";
			this.m_tapeMat.Size = new System.Drawing.Size(169, 39);
			this.m_tapeMat.TabIndex = 32;
			this.m_tapeMat.TabStop = false;
			this.m_tapeMat.Text = "Tape Material";
			// 
			// m_tapeCombo
			// 
			this.m_tapeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tapeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_tapeCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_tapeCombo.FormattingEnabled = true;
			this.m_tapeCombo.Location = new System.Drawing.Point(3, 16);
			this.m_tapeCombo.Name = "m_tapeCombo";
			this.m_tapeCombo.Size = new System.Drawing.Size(163, 21);
			this.m_tapeCombo.TabIndex = 0;
			// 
			// TapeGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_tapeMat);
			this.Controls.Add(this.m_stagger);
			this.Controls.Add(this.outputGroup);
			this.Controls.Add(this.angleTol);
			this.Controls.Add(this.chainTol);
			this.Controls.Add(this.pixLength);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.selectGuideButt);
			this.Controls.Add(this.m_guideListView);
			this.Controls.Add(this.selectWarpButt);
			this.Controls.Add(this.m_warpListView);
			this.Controls.Add(this.m_labelTextBox);
			this.Name = "TapeGroupEditor";
			this.Size = new System.Drawing.Size(175, 450);
			this.outputGroup.ResumeLayout(false);
			this.outputGroup.PerformLayout();
			this.m_tapeMat.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.ListView m_guideListView;
		private Controls.EquationBoxAuto chainTol;
		private Controls.EquationBoxAuto pixLength;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private Controls.EquationBoxAuto angleTol;
		private System.Windows.Forms.GroupBox outputGroup;
		private System.Windows.Forms.TextBox tapeLen;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tapeCount;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox m_stagger;
		private System.Windows.Forms.GroupBox m_tapeMat;
		private System.Windows.Forms.ComboBox m_tapeCombo;
	}
}
