namespace Warps
{
	partial class CurvePointEditor
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
			this.m_curves = new System.Windows.Forms.ComboBox();
			this.m_label = new System.Windows.Forms.Label();
			this.image = new System.Windows.Forms.PictureBox();
			this.m_cs = new Warps.Controls.EquationBoxAuto();
			((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
			this.SuspendLayout();
			// 
			// m_curves
			// 
			this.m_curves.BackColor = System.Drawing.SystemColors.Window;
			this.m_curves.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_curves.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_curves.FormattingEnabled = true;
			this.m_curves.Location = new System.Drawing.Point(208, 0);
			this.m_curves.Name = "m_curves";
			this.m_curves.Size = new System.Drawing.Size(84, 21);
			this.m_curves.TabIndex = 1;
			// 
			// m_label
			// 
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(30, 4);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(77, 14);
			this.m_label.TabIndex = 2;
			this.m_label.Text = "CurvePoint";
			// 
			// image
			// 
			this.image.Image = global::Warps.Properties.Resources.CurvePoint;
			this.image.Location = new System.Drawing.Point(3, 3);
			this.image.Name = "image";
			this.image.Size = new System.Drawing.Size(21, 19);
			this.image.TabIndex = 3;
			this.image.TabStop = false;
			// 
			// m_cs
			// 
			this.m_cs.AutoFillVariables = null;
			this.m_cs.Location = new System.Drawing.Point(113, 0);
			this.m_cs.MaximumSize = new System.Drawing.Size(101, 24);
			this.m_cs.Name = "m_cs";
			this.m_cs.sail = null;
			this.m_cs.Size = new System.Drawing.Size(89, 24);
			this.m_cs.TabIndex = 4;
			this.m_cs.Value = 0D;
			// 
			// CurvePointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_cs);
			this.Controls.Add(this.m_label);
			this.Controls.Add(this.m_curves);
			this.Controls.Add(this.image);
			this.Name = "CurvePointEditor";
			this.Size = new System.Drawing.Size(300, 25);
			this.Load += new System.EventHandler(this.CurvePointEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox m_curves;
		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.PictureBox image;
		private Controls.EquationBoxAuto m_cs;
	}
}
