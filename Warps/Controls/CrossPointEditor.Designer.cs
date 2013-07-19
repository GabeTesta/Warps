namespace Warps
{
	partial class CrossPointEditor
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
			this.m_curve1 = new System.Windows.Forms.ComboBox();
			this.m_curve2 = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// m_curve1
			// 
			this.m_curve1.BackColor = System.Drawing.SystemColors.Window;
			this.m_curve1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_curve1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_curve1.FormattingEnabled = true;
			this.m_curve1.Location = new System.Drawing.Point(3, 3);
			this.m_curve1.Name = "m_curve1";
			this.m_curve1.Size = new System.Drawing.Size(84, 21);
			this.m_curve1.TabIndex = 2;
			this.m_curve1.Validating += new System.ComponentModel.CancelEventHandler(this.m_curve2_Validating);
			// 
			// m_curve2
			// 
			this.m_curve2.BackColor = System.Drawing.SystemColors.Window;
			this.m_curve2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_curve2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_curve2.FormattingEnabled = true;
			this.m_curve2.Location = new System.Drawing.Point(93, 3);
			this.m_curve2.Name = "m_curve2";
			this.m_curve2.Size = new System.Drawing.Size(84, 21);
			this.m_curve2.TabIndex = 3;
			this.m_curve2.Validating += new System.ComponentModel.CancelEventHandler(this.m_curve2_Validating);
			// 
			// CrossPointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_curve2);
			this.Controls.Add(this.m_curve1);
			this.Name = "CrossPointEditor";
			this.Size = new System.Drawing.Size(181, 35);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox m_curve1;
		private System.Windows.Forms.ComboBox m_curve2;
	}
}
