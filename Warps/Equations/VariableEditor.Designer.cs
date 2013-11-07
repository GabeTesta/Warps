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
			this.m_selectedCheckbox = new System.Windows.Forms.CheckBox();
			this.m_resultTB = new System.Windows.Forms.TextBox();
			this.m_eqBox = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_variableTextBox
			// 
			this.m_variableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_variableTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_variableTextBox.Location = new System.Drawing.Point(21, 0);
			this.m_variableTextBox.Name = "m_variableTextBox";
			this.m_variableTextBox.Size = new System.Drawing.Size(105, 20);
			this.m_variableTextBox.TabIndex = 0;
			this.m_variableTextBox.Text = "Variable name";
			// 
			// m_selectedCheckbox
			// 
			this.m_selectedCheckbox.AutoSize = true;
			this.m_selectedCheckbox.Location = new System.Drawing.Point(0, 3);
			this.m_selectedCheckbox.Name = "m_selectedCheckbox";
			this.m_selectedCheckbox.Size = new System.Drawing.Size(15, 14);
			this.m_selectedCheckbox.TabIndex = 4;
			this.m_selectedCheckbox.UseVisualStyleBackColor = true;
			// 
			// m_resultTB
			// 
			this.m_resultTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_resultTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_resultTB.Enabled = false;
			this.m_resultTB.Location = new System.Drawing.Point(132, 0);
			this.m_resultTB.Name = "m_resultTB";
			this.m_resultTB.Size = new System.Drawing.Size(88, 20);
			this.m_resultTB.TabIndex = 5;
			this.m_resultTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_eqBox
			// 
			this.m_eqBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_eqBox.AutoFillVariables = null;
			this.m_eqBox.Location = new System.Drawing.Point(0, 21);
			this.m_eqBox.Name = "m_eqBox";
			this.m_eqBox.Size = new System.Drawing.Size(220, 23);
			this.m_eqBox.TabIndex = 2;
			// 
			// VariableEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.m_resultTB);
			this.Controls.Add(this.m_selectedCheckbox);
			this.Controls.Add(this.m_eqBox);
			this.Controls.Add(this.m_variableTextBox);
			this.Name = "VariableEditor";
			this.Size = new System.Drawing.Size(220, 45);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_variableTextBox;
		private EquationBoxAuto m_eqBox;
		private System.Windows.Forms.CheckBox m_selectedCheckbox;
		private System.Windows.Forms.TextBox m_resultTB;
	}
}
