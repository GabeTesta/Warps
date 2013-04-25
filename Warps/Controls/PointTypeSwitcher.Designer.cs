namespace Warps
{
	partial class PointTypeSwitcher
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.dragImage = new System.Windows.Forms.PictureBox();
			this.forwardBut = new System.Windows.Forms.Button();
			this.backBut = new System.Windows.Forms.Button();
			this.m_selectedCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dragImage)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Location = new System.Drawing.Point(42, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(214, 24);
			this.panel1.TabIndex = 0;
			this.panel1.Enter += new System.EventHandler(this.panel1_Enter);
			this.panel1.MouseEnter += new System.EventHandler(this.PointTypeSwitcher_MouseEnter);
			this.panel1.MouseLeave += new System.EventHandler(this.PointTypeSwitcher_MouseLeave);
			// 
			// dragImage
			// 
			this.dragImage.Image = global::Warps.Properties.Resources.dragger;
			this.dragImage.Location = new System.Drawing.Point(0, 0);
			this.dragImage.Name = "dragImage";
			this.dragImage.Size = new System.Drawing.Size(15, 22);
			this.dragImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.dragImage.TabIndex = 5;
			this.dragImage.TabStop = false;
			this.dragImage.Visible = false;
			this.dragImage.MouseEnter += new System.EventHandler(this.PointTypeSwitcher_MouseEnter);
			this.dragImage.MouseLeave += new System.EventHandler(this.PointTypeSwitcher_MouseLeave);
			this.dragImage.MouseHover += new System.EventHandler(this.dragImage_MouseHover);
			// 
			// forwardBut
			// 
			this.forwardBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.forwardBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.forwardBut.Image = global::Warps.Properties.Resources.glyphicons_223_chevron_right;
			this.forwardBut.Location = new System.Drawing.Point(290, 0);
			this.forwardBut.Name = "forwardBut";
			this.forwardBut.Size = new System.Drawing.Size(20, 22);
			this.forwardBut.TabIndex = 2;
			this.forwardBut.Text = ">";
			this.forwardBut.UseVisualStyleBackColor = true;
			this.forwardBut.Click += new System.EventHandler(this.forwardBut_Click);
			this.forwardBut.MouseEnter += new System.EventHandler(this.PointTypeSwitcher_MouseEnter);
			this.forwardBut.MouseLeave += new System.EventHandler(this.PointTypeSwitcher_MouseLeave);
			// 
			// backBut
			// 
			this.backBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.backBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.backBut.Image = global::Warps.Properties.Resources.glyphicons_224_chevron_left;
			this.backBut.Location = new System.Drawing.Point(264, 0);
			this.backBut.Name = "backBut";
			this.backBut.Size = new System.Drawing.Size(20, 22);
			this.backBut.TabIndex = 1;
			this.backBut.Text = "<";
			this.backBut.UseVisualStyleBackColor = true;
			this.backBut.Click += new System.EventHandler(this.backBut_Click);
			this.backBut.MouseEnter += new System.EventHandler(this.PointTypeSwitcher_MouseEnter);
			this.backBut.MouseLeave += new System.EventHandler(this.PointTypeSwitcher_MouseLeave);
			// 
			// m_selectedCheckBox
			// 
			this.m_selectedCheckBox.AutoSize = true;
			this.m_selectedCheckBox.Location = new System.Drawing.Point(21, 5);
			this.m_selectedCheckBox.Name = "m_selectedCheckBox";
			this.m_selectedCheckBox.Size = new System.Drawing.Size(15, 14);
			this.m_selectedCheckBox.TabIndex = 6;
			this.m_selectedCheckBox.UseVisualStyleBackColor = true;
			// 
			// PointTypeSwitcher
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_selectedCheckBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.backBut);
			this.Controls.Add(this.dragImage);
			this.Controls.Add(this.forwardBut);
			this.Name = "PointTypeSwitcher";
			this.Size = new System.Drawing.Size(313, 24);
			this.Enter += new System.EventHandler(this.PointTypeSwitcher_Enter);
			this.MouseEnter += new System.EventHandler(this.PointTypeSwitcher_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.PointTypeSwitcher_MouseLeave);
			((System.ComponentModel.ISupportInitialize)(this.dragImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button backBut;
		private System.Windows.Forms.Button forwardBut;
		private System.Windows.Forms.PictureBox dragImage;
		public System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox m_selectedCheckBox;
	}
}
