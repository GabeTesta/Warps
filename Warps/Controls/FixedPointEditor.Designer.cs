namespace Warps
{
	partial class FixedPointEditor
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
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.m_vEq = new Warps.Controls.EquationBoxAuto();
			this.m_uEq = new Warps.Controls.EquationBoxAuto();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(30, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "FixedPoint";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Warps.Properties.Resources.FixedPoint;
			this.pictureBox1.Location = new System.Drawing.Point(3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(21, 19);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// m_vEq
			// 
			this.m_vEq.AutoFillVariables = null;
			this.m_vEq.Location = new System.Drawing.Point(208, 0);
			this.m_vEq.MaximumSize = new System.Drawing.Size(101, 24);
			this.m_vEq.Name = "m_vEq";
			this.m_vEq.sail = null;
			this.m_vEq.Size = new System.Drawing.Size(89, 24);
			this.m_vEq.TabIndex = 4;
			this.m_vEq.Value = 0D;
			// 
			// m_uEq
			// 
			this.m_uEq.AutoFillVariables = null;
			this.m_uEq.Location = new System.Drawing.Point(113, 0);
			this.m_uEq.MaximumSize = new System.Drawing.Size(101, 24);
			this.m_uEq.Name = "m_uEq";
			this.m_uEq.sail = null;
			this.m_uEq.Size = new System.Drawing.Size(89, 24);
			this.m_uEq.TabIndex = 3;
			this.m_uEq.Value = 0D;
			// 
			// FixedPointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.m_vEq);
			this.Controls.Add(this.m_uEq);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.Name = "FixedPointEditor";
			this.Size = new System.Drawing.Size(300, 25);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private Controls.EquationBoxAuto m_uEq;
		private Controls.EquationBoxAuto m_vEq;
	}
}
