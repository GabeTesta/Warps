namespace Warps.Mixed
{
	partial class MixedGroupEditor
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
			this.m_listBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(0, 0);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(150, 20);
			this.m_labelTextBox.TabIndex = 0;
			// 
			// m_listBox
			// 
			this.m_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_listBox.FormattingEnabled = true;
			this.m_listBox.IntegralHeight = false;
			this.m_listBox.Location = new System.Drawing.Point(0, 26);
			this.m_listBox.Name = "m_listBox";
			this.m_listBox.Size = new System.Drawing.Size(150, 124);
			this.m_listBox.TabIndex = 1;
			this.m_listBox.DoubleClick += new System.EventHandler(this.m_listBox_DoubleClick);
			// 
			// MixedGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_listBox);
			this.Controls.Add(this.m_labelTextBox);
			this.Name = "MixedGroupEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_labelTextBox;
		private System.Windows.Forms.ListBox m_listBox;
	}
}
