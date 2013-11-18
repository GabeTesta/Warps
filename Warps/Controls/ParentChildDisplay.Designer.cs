namespace Warps.Controls
{
	partial class ParentChildDisplay
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.m_parentList = new System.Windows.Forms.ListBox();
			this.parentLab = new System.Windows.Forms.Label();
			this.m_childList = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.m_itemLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.m_parentList);
			this.splitContainer1.Panel1.Controls.Add(this.parentLab);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.m_childList);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Size = new System.Drawing.Size(471, 250);
			this.splitContainer1.SplitterDistance = 223;
			this.splitContainer1.TabIndex = 0;
			// 
			// m_parentList
			// 
			this.m_parentList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_parentList.FormattingEnabled = true;
			this.m_parentList.Location = new System.Drawing.Point(0, 13);
			this.m_parentList.Name = "m_parentList";
			this.m_parentList.Size = new System.Drawing.Size(223, 237);
			this.m_parentList.TabIndex = 1;
			// 
			// parentLab
			// 
			this.parentLab.AutoSize = true;
			this.parentLab.Dock = System.Windows.Forms.DockStyle.Top;
			this.parentLab.Location = new System.Drawing.Point(0, 0);
			this.parentLab.Name = "parentLab";
			this.parentLab.Size = new System.Drawing.Size(43, 13);
			this.parentLab.TabIndex = 0;
			this.parentLab.Text = "Parents";
			// 
			// m_childList
			// 
			this.m_childList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_childList.FormattingEnabled = true;
			this.m_childList.Location = new System.Drawing.Point(0, 13);
			this.m_childList.Name = "m_childList";
			this.m_childList.Size = new System.Drawing.Size(244, 237);
			this.m_childList.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Children";
			// 
			// m_itemLabel
			// 
			this.m_itemLabel.AutoSize = true;
			this.m_itemLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_itemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_itemLabel.Location = new System.Drawing.Point(0, 0);
			this.m_itemLabel.Name = "m_itemLabel";
			this.m_itemLabel.Size = new System.Drawing.Size(60, 24);
			this.m_itemLabel.TabIndex = 1;
			this.m_itemLabel.Text = "label1";
			// 
			// ParentChildDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.m_itemLabel);
			this.Name = "ParentChildDisplay";
			this.Size = new System.Drawing.Size(471, 274);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label m_itemLabel;
		private System.Windows.Forms.ListBox m_parentList;
		private System.Windows.Forms.Label parentLab;
		private System.Windows.Forms.ListBox m_childList;
		private System.Windows.Forms.Label label2;
	}
}
