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
			this.m_cs = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_curves
			// 
			this.m_curves.BackColor = System.Drawing.SystemColors.Window;
			this.m_curves.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_curves.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_curves.FormattingEnabled = true;
			this.m_curves.IntegralHeight = false;
			this.m_curves.Location = new System.Drawing.Point(95, 0);
			this.m_curves.Name = "m_curves";
			this.m_curves.Size = new System.Drawing.Size(84, 21);
			this.m_curves.TabIndex = 1;
			this.m_curves.Validating += new System.ComponentModel.CancelEventHandler(this.m_curves_Validating);
			// 
			// m_cs
			// 
			this.m_cs.AutoFillVariables = null;
			this.m_cs.Location = new System.Drawing.Point(0, 0);
			this.m_cs.Name = "m_cs";
			this.m_cs.Size = new System.Drawing.Size(89, 23);
			this.m_cs.TabIndex = 0;
			// 
			// CurvePointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_cs);
			this.Controls.Add(this.m_curves);
			this.Name = "CurvePointEditor";
			this.Size = new System.Drawing.Size(179, 21);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox m_curves;
		private Controls.EquationBoxAuto m_cs;
	}
}
