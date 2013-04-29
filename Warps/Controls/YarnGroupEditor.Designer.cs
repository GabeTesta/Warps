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
			this.m_label = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_guideListView = new System.Windows.Forms.ListView();
			this.selectGuideButt = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.yarnDenierTB = new System.Windows.Forms.TextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.selectWarpButt = new System.Windows.Forms.Button();
			this.m_warpListView = new System.Windows.Forms.ListView();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.m_densityLocTextBox = new System.Windows.Forms.TextBox();
			this.targetdpiTB = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(56, 3);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(116, 20);
			this.m_labelTextBox.TabIndex = 7;
			// 
			// m_label
			// 
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
			this.groupBox1.Controls.Add(this.m_guideListView);
			this.groupBox1.Controls.Add(this.selectGuideButt);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox1.Location = new System.Drawing.Point(3, 213);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(169, 95);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Guide Comb Selection";
			// 
			// m_guideListView
			// 
			this.m_guideListView.AutoArrange = false;
			this.m_guideListView.Location = new System.Drawing.Point(4, 15);
			this.m_guideListView.Name = "m_guideListView";
			this.m_guideListView.Size = new System.Drawing.Size(159, 38);
			this.m_guideListView.TabIndex = 6;
			this.m_guideListView.UseCompatibleStateImageBehavior = false;
			this.m_guideListView.View = System.Windows.Forms.View.List;
			// 
			// selectGuideButt
			// 
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
			this.groupBox2.Controls.Add(this.yarnDenierTB);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox2.Location = new System.Drawing.Point(31, 314);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(110, 55);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Yarn Denier";
			// 
			// yarnDenierTB
			// 
			this.yarnDenierTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.yarnDenierTB.Location = new System.Drawing.Point(10, 19);
			this.yarnDenierTB.Name = "yarnDenierTB";
			this.yarnDenierTB.Size = new System.Drawing.Size(94, 20);
			this.yarnDenierTB.TabIndex = 1;
			this.yarnDenierTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.selectWarpButt);
			this.groupBox4.Controls.Add(this.m_warpListView);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox4.Location = new System.Drawing.Point(3, 29);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(169, 178);
			this.groupBox4.TabIndex = 10;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Warp Selection";
			// 
			// selectWarpButt
			// 
			this.selectWarpButt.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.selectWarpButt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectWarpButt.Location = new System.Drawing.Point(19, 147);
			this.selectWarpButt.Name = "selectWarpButt";
			this.selectWarpButt.Size = new System.Drawing.Size(134, 23);
			this.selectWarpButt.TabIndex = 5;
			this.selectWarpButt.Text = "Select Warps";
			this.selectWarpButt.UseVisualStyleBackColor = false;
			this.selectWarpButt.Click += new System.EventHandler(this.button1_Click);
			// 
			// m_warpListView
			// 
			this.m_warpListView.AutoArrange = false;
			this.m_warpListView.Location = new System.Drawing.Point(7, 19);
			this.m_warpListView.Name = "m_warpListView";
			this.m_warpListView.Size = new System.Drawing.Size(156, 122);
			this.m_warpListView.TabIndex = 4;
			this.m_warpListView.UseCompatibleStateImageBehavior = false;
			this.m_warpListView.View = System.Windows.Forms.View.List;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.m_densityLocTextBox);
			this.groupBox5.Location = new System.Drawing.Point(3, 436);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(169, 50);
			this.groupBox5.TabIndex = 11;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Density Curve Locations";
			// 
			// m_densityLocTextBox
			// 
			this.m_densityLocTextBox.Location = new System.Drawing.Point(7, 20);
			this.m_densityLocTextBox.Name = "m_densityLocTextBox";
			this.m_densityLocTextBox.Size = new System.Drawing.Size(146, 20);
			this.m_densityLocTextBox.TabIndex = 0;
			this.m_densityLocTextBox.Text = "0.25, 0.5, 0.75";
			this.m_densityLocTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// targetdpiTB
			// 
			this.targetdpiTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.targetdpiTB.Location = new System.Drawing.Point(6, 19);
			this.targetdpiTB.Name = "targetdpiTB";
			this.targetdpiTB.Size = new System.Drawing.Size(98, 20);
			this.targetdpiTB.TabIndex = 2;
			this.targetdpiTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.targetdpiTB);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox3.Location = new System.Drawing.Point(31, 375);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(110, 55);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Target DPI";
			// 
			// YarnGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.m_labelTextBox);
			this.Controls.Add(this.m_label);
			this.Name = "YarnGroupEditor";
			this.Size = new System.Drawing.Size(175, 492);
			this.Load += new System.EventHandler(this.YarnGroupEditor_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox yarnDenierTB;
		private System.Windows.Forms.ListView m_warpListView;
		private System.Windows.Forms.Button selectWarpButt;
		private System.Windows.Forms.Button selectGuideButt;
		private System.Windows.Forms.ListView m_guideListView;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox m_densityLocTextBox;
		private System.Windows.Forms.TextBox targetdpiTB;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}
