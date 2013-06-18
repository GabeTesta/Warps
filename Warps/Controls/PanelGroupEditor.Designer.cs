namespace Warps.Controls
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
			Warps.Equation equation1 = new Warps.Equation();
			this.label3 = new System.Windows.Forms.Label();
			this.m_orientationList = new System.Windows.Forms.ComboBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.outputGroup = new System.Windows.Forms.GroupBox();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.widthEQB = new Warps.Controls.EquationBoxAuto();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 289);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 26;
			this.label3.Text = "Panel Width";
			// 
			// m_orientationList
			// 
			this.m_orientationList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_orientationList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_orientationList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_orientationList.FormattingEnabled = true;
			this.m_orientationList.Location = new System.Drawing.Point(3, 16);
			this.m_orientationList.Name = "m_orientationList";
			this.m_orientationList.Size = new System.Drawing.Size(162, 21);
			this.m_orientationList.TabIndex = 0;
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.m_orientationList);
			this.groupBox6.Location = new System.Drawing.Point(3, 335);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(168, 39);
			this.groupBox6.TabIndex = 24;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Cloth Orientation";
			// 
			// outputGroup
			// 
			this.outputGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputGroup.Enabled = false;
			this.outputGroup.Location = new System.Drawing.Point(3, 380);
			this.outputGroup.Name = "outputGroup";
			this.outputGroup.Size = new System.Drawing.Size(168, 65);
			this.outputGroup.TabIndex = 23;
			this.outputGroup.TabStop = false;
			this.outputGroup.Text = "Output";
			// 
			// selectGuideButt
			// 
			this.selectGuideButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectGuideButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectGuideButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectGuideButt.Location = new System.Drawing.Point(3, 263);
			this.selectGuideButt.Name = "selectGuideButt";
			this.selectGuideButt.Size = new System.Drawing.Size(166, 23);
			this.selectGuideButt.TabIndex = 19;
			this.selectGuideButt.Text = "Select Guide";
			this.selectGuideButt.UseVisualStyleBackColor = false;
			this.selectGuideButt.Click += new System.EventHandler(this.selectGuideButt_Click);
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(3, 174);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(166, 23);
			this.selectWarpButt.TabIndex = 18;
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
			this.m_warpListView.Location = new System.Drawing.Point(3, 28);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(166, 149);
			this.m_warpListView.TabIndex = 17;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			// 
			// m_guideListView
			// 
			this.m_guideListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(3, 203);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(166, 63);
			this.m_guideListView.TabIndex = 20;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(3, 2);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(166, 20);
			this.m_labelTextBox.TabIndex = 21;
			// 
			// widthEQB
			// 
			this.widthEQB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.widthEQB.AutoFillVariables = null;
			equation1.EquationText = "0.0000";
			equation1.Label = "eq";
			equation1.Locked = false;
			this.widthEQB.Equation = equation1;
			this.widthEQB.Location = new System.Drawing.Point(3, 305);
			this.widthEQB.MaximumSize = new System.Drawing.Size(201, 24);
			this.widthEQB.Name = "widthEQB";
			this.widthEQB.sail = null;
			this.widthEQB.Size = new System.Drawing.Size(166, 24);
			this.widthEQB.TabIndex = 15;
			this.widthEQB.Value = 0D;
			// 
			// PanelGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.widthEQB);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.outputGroup);
			this.Controls.Add(this.selectGuideButt);
			this.Controls.Add(this.selectWarpButt);
			this.Controls.Add(this.m_warpListView);
			this.Controls.Add(this.m_guideListView);
			this.Controls.Add(this.m_labelTextBox);
			this.Name = "PanelGroupEditor";
			this.Size = new System.Drawing.Size(175, 484);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private EquationBoxAuto widthEQB;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox m_orientationList;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.GroupBox outputGroup;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.ListView m_guideListView;
		private System.Windows.Forms.TextBox m_labelTextBox;
	}
}
