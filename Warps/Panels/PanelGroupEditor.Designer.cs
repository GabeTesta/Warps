namespace Warps.Panels
{
	partial class PanelGroupEditor
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
			this.label3 = new System.Windows.Forms.Label();
			this.m_orientationList = new System.Windows.Forms.ComboBox();
			this.outputGroup = new System.Windows.Forms.GroupBox();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.widthEQB = new Warps.Controls.EquationBoxAuto();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 268);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 26;
			this.label3.Text = "Panel Width";
			// 
			// m_orientationList
			// 
			this.m_orientationList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_orientationList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_orientationList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_orientationList.FormattingEnabled = true;
			this.m_orientationList.Location = new System.Drawing.Point(0, 326);
			this.m_orientationList.Name = "m_orientationList";
			this.m_orientationList.Size = new System.Drawing.Size(175, 21);
			this.m_orientationList.TabIndex = 0;
			// 
			// outputGroup
			// 
			this.outputGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputGroup.Enabled = false;
			this.outputGroup.Location = new System.Drawing.Point(0, 353);
			this.outputGroup.Name = "outputGroup";
			this.outputGroup.Size = new System.Drawing.Size(175, 70);
			this.outputGroup.TabIndex = 23;
			this.outputGroup.TabStop = false;
			this.outputGroup.Text = "Output";
			// 
			// selectGuideButt
			// 
			this.selectGuideButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectGuideButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectGuideButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectGuideButt.Location = new System.Drawing.Point(0, 235);
			this.selectGuideButt.Name = "selectGuideButt";
			this.selectGuideButt.Size = new System.Drawing.Size(175, 23);
			this.selectGuideButt.TabIndex = 19;
			this.selectGuideButt.Text = "Select Warps";
			this.selectGuideButt.UseVisualStyleBackColor = false;
			this.selectGuideButt.Click += new System.EventHandler(this.selectGuideButt_Click);
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(0, 116);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(175, 23);
			this.selectWarpButt.TabIndex = 18;
			this.selectWarpButt.Text = "Select Boundarys";
			this.selectWarpButt.UseVisualStyleBackColor = false;
			this.selectWarpButt.Click += new System.EventHandler(this.selectWarpButt_Click);
			// 
			// m_warpListView
			// 
			this.m_warpListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_warpListView.AutoArrange = false;
			this.m_warpListView.Location = new System.Drawing.Point(0, 26);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(175, 91);
			this.m_warpListView.TabIndex = 17;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			// 
			// m_guideListView
			// 
			this.m_guideListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(0, 145);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(175, 91);
			this.m_guideListView.TabIndex = 20;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(0, 0);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(175, 20);
			this.m_labelTextBox.TabIndex = 21;
			// 
			// widthEQB
			// 
			this.widthEQB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.widthEQB.AutoFillVariables = null;
			this.widthEQB.Location = new System.Drawing.Point(0, 284);
			this.widthEQB.Name = "widthEQB";
			this.widthEQB.Size = new System.Drawing.Size(175, 23);
			this.widthEQB.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 310);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 26;
			this.label1.Text = "Cloth Orientation";
			// 
			// PanelGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_orientationList);
			this.Controls.Add(this.widthEQB);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.outputGroup);
			this.Controls.Add(this.selectGuideButt);
			this.Controls.Add(this.selectWarpButt);
			this.Controls.Add(this.m_warpListView);
			this.Controls.Add(this.m_guideListView);
			this.Controls.Add(this.m_labelTextBox);
			this.Name = "PanelGroupEditor";
			this.Size = new System.Drawing.Size(175, 426);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Warps.Controls.EquationBoxAuto widthEQB;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox m_orientationList;
		private System.Windows.Forms.GroupBox outputGroup;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.ListView m_guideListView;
		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.Label label1;
	}
}
