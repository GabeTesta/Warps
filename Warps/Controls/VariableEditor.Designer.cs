namespace Warps.Controls
{
	partial class VariableEditor
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
			this.m_variableTextBox = new System.Windows.Forms.TextBox();
			this.m_eqBox = new Warps.Controls.EquationBoxAuto();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_variableTextBox
			// 
			this.m_variableTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_variableTextBox.Location = new System.Drawing.Point(42, 2);
			this.m_variableTextBox.Name = "m_variableTextBox";
			this.m_variableTextBox.Size = new System.Drawing.Size(82, 20);
			this.m_variableTextBox.TabIndex = 0;
			this.m_variableTextBox.Text = "Variable name";
			// 
			// m_eqBox
			// 
			this.m_eqBox.AutoFillVariables = null;
			this.m_eqBox.Location = new System.Drawing.Point(130, 0);
			this.m_eqBox.MaximumSize = new System.Drawing.Size(201, 24);
			this.m_eqBox.Name = "m_eqBox";
			this.m_eqBox.sail = null;
			this.m_eqBox.Size = new System.Drawing.Size(167, 24);
			this.m_eqBox.TabIndex = 2;
			this.m_eqBox.Value = 0D;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Name:";
			// 
			// VariableEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_variableTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.m_eqBox);
			this.Name = "VariableEditor";
			this.Size = new System.Drawing.Size(300, 25);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_variableTextBox;
		private EquationBoxAuto m_eqBox;
		private System.Windows.Forms.Label label1;
	}
}
