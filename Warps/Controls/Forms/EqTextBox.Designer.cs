namespace Warps.Controls.Forms
{
	partial class EqTextBox
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
			this.m_text = new System.Windows.Forms.TextBox();
			this.m_fn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_text
			// 
			this.m_text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_text.Location = new System.Drawing.Point(0, 0);
			this.m_text.Name = "m_text";
			this.m_text.Size = new System.Drawing.Size(173, 20);
			this.m_text.TabIndex = 0;
			this.m_text.WordWrap = false;
			this.m_text.Enter += new System.EventHandler(this.m_text_Enter);
			this.m_text.Leave += new System.EventHandler(this.m_text_Leave);
			this.m_text.Validating += new System.ComponentModel.CancelEventHandler(this.m_text_Validating);
			// 
			// m_fn
			// 
			this.m_fn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_fn.Location = new System.Drawing.Point(179, 0);
			this.m_fn.Name = "m_fn";
			this.m_fn.Size = new System.Drawing.Size(22, 20);
			this.m_fn.TabIndex = 1;
			this.m_fn.TabStop = false;
			this.m_fn.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.m_fn.UseVisualStyleBackColor = true;
			this.m_fn.Click += new System.EventHandler(this.m_fn_Click);
			// 
			// EqTextBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_fn);
			this.Controls.Add(this.m_text);
			this.Name = "EqTextBox";
			this.Size = new System.Drawing.Size(203, 23);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_text;
		private System.Windows.Forms.Button m_fn;
	}
}
