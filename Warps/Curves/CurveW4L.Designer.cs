﻿namespace Warps.Curves
{
	partial class CurveW4L
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
			this.m_scriptBox = new System.Windows.Forms.TextBox();
			this.m_import = new System.Windows.Forms.Button();
			this.m_copy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.m_scriptBox.AcceptsReturn = true;
			this.m_scriptBox.AllowDrop = true;
			this.m_scriptBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_scriptBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_scriptBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_scriptBox.Location = new System.Drawing.Point(0, 29);
			this.m_scriptBox.Multiline = true;
			this.m_scriptBox.Name = "textBox1";
			this.m_scriptBox.Size = new System.Drawing.Size(150, 121);
			this.m_scriptBox.TabIndex = 0;
			this.m_scriptBox.WordWrap = false;
			this.m_scriptBox.Enter += new System.EventHandler(this.scriptBox_Enter);
			// 
			// m_import
			// 
			this.m_import.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_import.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_import.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_import.Location = new System.Drawing.Point(75, 0);
			this.m_import.Name = "m_import";
			this.m_import.Size = new System.Drawing.Size(69, 23);
			this.m_import.TabIndex = 1;
			this.m_import.Text = "Import";
			this.m_import.UseVisualStyleBackColor = true;
			this.m_import.Click += new System.EventHandler(this.m_import_Click);
			// 
			// m_copy
			// 
			this.m_copy.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_copy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_copy.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_copy.Location = new System.Drawing.Point(0, 0);
			this.m_copy.Name = "m_copy";
			this.m_copy.Size = new System.Drawing.Size(69, 23);
			this.m_copy.TabIndex = 1;
			this.m_copy.Text = "Copy Txt";
			this.m_copy.UseVisualStyleBackColor = true;
			this.m_copy.Click += new System.EventHandler(this.m_copy_Click);
			// 
			// CurveW4L
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_copy);
			this.Controls.Add(this.m_import);
			this.Controls.Add(this.m_scriptBox);
			this.Name = "CurveW4L";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_scriptBox;
		private System.Windows.Forms.Button m_import;
		private System.Windows.Forms.Button m_copy;
	}
}
