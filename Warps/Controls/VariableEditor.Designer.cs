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
			Warps.Equation equation1 = new Warps.Equation();
			this.m_variableTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.m_selectedCheckbox = new System.Windows.Forms.CheckBox();
			this.m_eqBox = new Warps.Controls.EquationBoxAuto();
			this.SuspendLayout();
			// 
			// m_variableTextBox
			// 
			this.m_variableTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_variableTextBox.Location = new System.Drawing.Point(73, 2);
			this.m_variableTextBox.Name = "m_variableTextBox";
			this.m_variableTextBox.Size = new System.Drawing.Size(82, 20);
			this.m_variableTextBox.TabIndex = 0;
			this.m_variableTextBox.Text = "Variable name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Name:";
			// 
			// m_selectedCheckbox
			// 
			this.m_selectedCheckbox.AutoSize = true;
			this.m_selectedCheckbox.Location = new System.Drawing.Point(4, 4);
			this.m_selectedCheckbox.Name = "m_selectedCheckbox";
			this.m_selectedCheckbox.Size = new System.Drawing.Size(15, 14);
			this.m_selectedCheckbox.TabIndex = 4;
			this.m_selectedCheckbox.UseVisualStyleBackColor = true;
			// 
			// m_eqBox
			// 
			this.m_eqBox.AutoFillVariables = null;
			equation1.EquationText = "0.0000";
			equation1.Label = "eq";
			//equation1.Result = 0D;
			equation1.sail = null;
			this.m_eqBox.Equation = equation1;
			this.m_eqBox.Location = new System.Drawing.Point(4, 23);
			this.m_eqBox.MaximumSize = new System.Drawing.Size(201, 24);
			this.m_eqBox.Name = "m_eqBox";
			this.m_eqBox.sail = null;
			this.m_eqBox.Size = new System.Drawing.Size(160, 24);
			this.m_eqBox.TabIndex = 2;
			this.m_eqBox.Value = 0D;
			// 
			// VariableEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_selectedCheckbox);
			this.Controls.Add(this.m_variableTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.m_eqBox);
			this.Name = "VariableEditor";
			this.Size = new System.Drawing.Size(167, 50);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_variableTextBox;
		private EquationBoxAuto m_eqBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox m_selectedCheckbox;
	}
}
