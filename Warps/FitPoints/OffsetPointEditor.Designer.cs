namespace Warps
{
	partial class OffsetPointEditor
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
			this.m_offset = new Warps.Controls.EquationBoxAuto();
			this.m_cs = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_curves
			// 
			this.m_curves.FormattingEnabled = true;
			this.m_curves.Location = new System.Drawing.Point(88, 0);
			this.m_curves.Name = "m_curves";
			this.m_curves.Size = new System.Drawing.Size(65, 21);
			this.m_curves.TabIndex = 1;
			// 
			// m_offset
			// 
			this.m_offset.AutoFillVariables = null;
			this.m_offset.Location = new System.Drawing.Point(159, 0);
			this.m_offset.Name = "m_offset";
			this.m_offset.Size = new System.Drawing.Size(79, 23);
			this.m_offset.TabIndex = 0;
			// 
			// m_cs
			// 
			this.m_cs.AutoFillVariables = null;
			this.m_cs.Location = new System.Drawing.Point(3, 0);
			this.m_cs.Name = "m_cs";
			this.m_cs.Size = new System.Drawing.Size(79, 23);
			this.m_cs.TabIndex = 0;
			// 
			// OffsetPointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_curves);
			this.Controls.Add(this.m_offset);
			this.Controls.Add(this.m_cs);
			this.Name = "OffsetPointEditor";
			this.Size = new System.Drawing.Size(257, 31);
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.EquationBoxAuto m_cs;
		private Controls.EquationBoxAuto m_offset;
		private System.Windows.Forms.ComboBox m_curves;
	}
}
