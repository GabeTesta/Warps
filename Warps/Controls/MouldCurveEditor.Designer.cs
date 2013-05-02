namespace Warps.Controls
{
	partial class MouldCurveEditor
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
			this.m_label = new System.Windows.Forms.TextBox();
			this.m_length = new System.Windows.Forms.Label();
			this.m_panel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// m_label
			// 
			this.m_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_label.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(0, 3);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(150, 20);
			this.m_label.TabIndex = 0;
			this.m_label.Text = "Label";
			// 
			// m_length
			// 
			this.m_length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_length.AutoSize = true;
			this.m_length.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_length.Location = new System.Drawing.Point(156, 6);
			this.m_length.Name = "m_length";
			this.m_length.Size = new System.Drawing.Size(43, 13);
			this.m_length.TabIndex = 1;
			this.m_length.Text = "0.0000";
			// 
			// m_panel
			// 
			this.m_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_panel.Location = new System.Drawing.Point(3, 29);
			this.m_panel.Name = "m_panel";
			this.m_panel.Size = new System.Drawing.Size(244, 405);
			this.m_panel.TabIndex = 2;
			// 
			// MouldCurveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_panel);
			this.Controls.Add(this.m_label);
			this.Controls.Add(this.m_length);
			this.Name = "MouldCurveEditor";
			this.Size = new System.Drawing.Size(250, 434);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_label;
		private System.Windows.Forms.Label m_length;
		private System.Windows.Forms.Panel m_panel;
	}
}
