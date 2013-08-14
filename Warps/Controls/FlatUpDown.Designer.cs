namespace Warps.Controls
{
	partial class FlatUpDown
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
			this.m_up = new System.Windows.Forms.Button();
			this.m_down = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_up
			// 
			this.m_up.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_up.FlatAppearance.BorderSize = 0;
			this.m_up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_up.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.m_up.Location = new System.Drawing.Point(0, 0);
			this.m_up.Name = "m_up";
			this.m_up.Size = new System.Drawing.Size(107, 47);
			this.m_up.TabIndex = 0;
			this.m_up.TabStop = false;
			this.m_up.Text = "up";
			this.m_up.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_up.UseVisualStyleBackColor = false;
			// 
			// m_down
			// 
			this.m_down.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_down.FlatAppearance.BorderSize = 0;
			this.m_down.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_down.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.m_down.Location = new System.Drawing.Point(0, 47);
			this.m_down.Name = "m_down";
			this.m_down.Size = new System.Drawing.Size(107, 43);
			this.m_down.TabIndex = 0;
			this.m_down.TabStop = false;
			this.m_down.Text = "down";
			this.m_down.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_down.UseVisualStyleBackColor = false;
			// 
			// FlatUpDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_down);
			this.Controls.Add(this.m_up);
			this.Name = "FlatUpDown";
			this.Size = new System.Drawing.Size(107, 90);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_up;
		private System.Windows.Forms.Button m_down;
	}
}
