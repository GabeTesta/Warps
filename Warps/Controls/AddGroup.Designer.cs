namespace Warps.Controls
{
	partial class AddGroup
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
			this.m_ok = new System.Windows.Forms.Button();
			this.m_cancel = new System.Windows.Forms.Button();
			this.m_name = new System.Windows.Forms.TextBox();
			this.m_type = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_ok
			// 
			this.m_ok.BackColor = System.Drawing.Color.White;
			this.m_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_ok.Location = new System.Drawing.Point(15, 93);
			this.m_ok.Name = "m_ok";
			this.m_ok.Size = new System.Drawing.Size(75, 23);
			this.m_ok.TabIndex = 3;
			this.m_ok.Text = "Ok";
			this.m_ok.UseVisualStyleBackColor = false;
			// 
			// m_cancel
			// 
			this.m_cancel.BackColor = System.Drawing.Color.White;
			this.m_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_cancel.Location = new System.Drawing.Point(106, 93);
			this.m_cancel.Name = "m_cancel";
			this.m_cancel.Size = new System.Drawing.Size(75, 23);
			this.m_cancel.TabIndex = 4;
			this.m_cancel.Text = "Cancel";
			this.m_cancel.UseVisualStyleBackColor = false;
			// 
			// m_name
			// 
			this.m_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_name.Location = new System.Drawing.Point(12, 27);
			this.m_name.Name = "m_name";
			this.m_name.Size = new System.Drawing.Size(169, 20);
			this.m_name.TabIndex = 1;
			// 
			// m_type
			// 
			this.m_type.Cursor = System.Windows.Forms.Cursors.Hand;
			this.m_type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_type.FormattingEnabled = true;
			this.m_type.Location = new System.Drawing.Point(12, 66);
			this.m_type.Name = "m_type";
			this.m_type.Size = new System.Drawing.Size(166, 21);
			this.m_type.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Label";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Type";
			// 
			// AddGroup
			// 
			this.AcceptButton = this.m_ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(195, 130);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.m_type);
			this.Controls.Add(this.m_name);
			this.Controls.Add(this.m_cancel);
			this.Controls.Add(this.m_ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AddGroup";
			this.Text = "Add Group";
			this.Load += new System.EventHandler(this.AddGroup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_ok;
		private System.Windows.Forms.Button m_cancel;
		private System.Windows.Forms.TextBox m_name;
		private System.Windows.Forms.ComboBox m_type;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}