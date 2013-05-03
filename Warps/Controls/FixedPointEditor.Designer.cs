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
			Warps.Equation equation1 = new Warps.Equation();
			Warps.Equation equation2 = new Warps.Equation();
			this.m_vEq = new Warps.Controls.EquationBoxAuto();
			this.m_uEq = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_vEq
			// 
			this.m_vEq.AutoFillVariables = null;
			equation1.EquationText = "0.0000";
			equation1.Label = "eq";
			//equation1.Result = 0D;
			//equation1.sail = null;
			this.m_vEq.Equation = equation1;
			this.m_vEq.Location = new System.Drawing.Point(95, 0);
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
			equation2.EquationText = "0.0000";
			equation2.Label = "eq";
			//equation2.Result = 0D;
			//equation2.sail = null;
			this.m_uEq.Equation = equation2;
			this.m_uEq.Location = new System.Drawing.Point(0, 0);
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
			this.Name = "FixedPointEditor";
			this.Size = new System.Drawing.Size(184, 23);
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.EquationBoxAuto m_uEq;
		private Controls.EquationBoxAuto m_vEq;
	}
}
