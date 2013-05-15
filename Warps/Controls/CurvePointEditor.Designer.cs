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
			Warps.Equation equation1 = new Warps.Equation();
			this.m_curves = new System.Windows.Forms.ComboBox();
			this.m_cs = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_curves
			// 
			this.m_curves.BackColor = System.Drawing.SystemColors.Window;
			this.m_curves.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_curves.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_curves.FormattingEnabled = true;
			this.m_curves.Location = new System.Drawing.Point(95, 0);
			this.m_curves.Name = "m_curves";
			this.m_curves.Size = new System.Drawing.Size(84, 21);
			this.m_curves.TabIndex = 1;
			// 
			// m_cs
			// 
			this.m_cs.AutoFillVariables = null;
			equation1.EquationText = "0.0000";
			equation1.Label = "eq";
			this.m_cs.Equation = equation1;
			this.m_cs.Location = new System.Drawing.Point(0, 0);
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
			this.Controls.Add(this.m_curves);
			this.Name = "CurvePointEditor";
			this.Size = new System.Drawing.Size(179, 23);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox m_curves;
		private Controls.EquationBoxAuto m_cs;
	}
}
