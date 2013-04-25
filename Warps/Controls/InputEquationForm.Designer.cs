namespace Warps.Controls
{
	partial class InputEquationForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label2 = new System.Windows.Forms.Label();
			this.CurveListBox = new System.Windows.Forms.ListBox();
			this.EquationListBox = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.addButton = new System.Windows.Forms.Button();
			this.resultBox = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.autoCompleteTextBox1 = new Warps.Controls.AutoCompleteTextBox();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Available Curves:";
			// 
			// CurveListBox
			// 
			this.CurveListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CurveListBox.FormattingEnabled = true;
			this.CurveListBox.Location = new System.Drawing.Point(12, 94);
			this.CurveListBox.Name = "CurveListBox";
			this.CurveListBox.Size = new System.Drawing.Size(136, 95);
			this.CurveListBox.TabIndex = 7;
			this.CurveListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CurveListBox_MouseDoubleClick);
			// 
			// EquationListBox
			// 
			this.EquationListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.EquationListBox.FormattingEnabled = true;
			this.EquationListBox.Location = new System.Drawing.Point(167, 94);
			this.EquationListBox.Name = "EquationListBox";
			this.EquationListBox.Size = new System.Drawing.Size(136, 95);
			this.EquationListBox.TabIndex = 15;
			this.EquationListBox.DoubleClick += new System.EventHandler(this.EquationListBox_DoubleClick);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(164, 78);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Available Equations:";
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.BackColor = System.Drawing.Color.White;
			this.addButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.addButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.addButton.Location = new System.Drawing.Point(263, 202);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(38, 23);
			this.addButton.TabIndex = 19;
			this.addButton.Text = "Use";
			this.addButton.UseVisualStyleBackColor = false;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// resultBox
			// 
			this.resultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.resultBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.resultBox.Enabled = false;
			this.resultBox.Location = new System.Drawing.Point(235, 38);
			this.resultBox.Name = "resultBox";
			this.resultBox.Size = new System.Drawing.Size(76, 20);
			this.resultBox.TabIndex = 15;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackColor = System.Drawing.Color.White;
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button1.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(135, 38);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 20;
			this.button1.Text = "Calculate";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// autoCompleteTextBox1
			// 
			this.autoCompleteTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.autoCompleteTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.autoCompleteTextBox1.Location = new System.Drawing.Point(12, 12);
			this.autoCompleteTextBox1.Name = "autoCompleteTextBox1";
			this.autoCompleteTextBox1.Size = new System.Drawing.Size(299, 20);
			this.autoCompleteTextBox1.TabIndex = 18;
			this.autoCompleteTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.autoCompleteTextBox1.Values = null;
			// 
			// InputEquationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(313, 237);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.resultBox);
			this.Controls.Add(this.autoCompleteTextBox1);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.EquationListBox);
			this.Controls.Add(this.CurveListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "InputEquationForm";
			this.Text = "Input Equation";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox CurveListBox;
		private System.Windows.Forms.ListBox EquationListBox;
		private System.Windows.Forms.Label label1;
		private AutoCompleteTextBox autoCompleteTextBox1;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.TextBox resultBox;
		private System.Windows.Forms.Button button1;
	}
}