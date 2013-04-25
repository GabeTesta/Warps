namespace Warps
{
	partial class CurveGroupEditor
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
			this.m_list = new System.Windows.Forms.ListBox();
			this.m_count = new System.Windows.Forms.Label();
			this.m_label = new System.Windows.Forms.Label();
			this.m_labelTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// m_list
			// 
			this.m_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_list.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_list.FormattingEnabled = true;
			this.m_list.IntegralHeight = false;
			this.m_list.ItemHeight = 14;
			this.m_list.Location = new System.Drawing.Point(6, 48);
			this.m_list.MultiColumn = true;
			this.m_list.Name = "m_list";
			this.m_list.Size = new System.Drawing.Size(291, 249);
			this.m_list.TabIndex = 2;
			this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
			this.m_list.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_list_KeyUp);
			// 
			// m_count
			// 
			this.m_count.AutoSize = true;
			this.m_count.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_count.Location = new System.Drawing.Point(3, 31);
			this.m_count.Name = "m_count";
			this.m_count.Size = new System.Drawing.Size(56, 14);
			this.m_count.TabIndex = 4;
			this.m_count.Text = "Count: ";
			// 
			// m_label
			// 
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(3, 3);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(56, 14);
			this.m_label.TabIndex = 3;
			this.m_label.Text = "Label: ";
			// 
			// m_labelTextBox
			// 
			this.m_labelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_labelTextBox.Location = new System.Drawing.Point(52, 0);
			this.m_labelTextBox.Name = "m_labelTextBox";
			this.m_labelTextBox.Size = new System.Drawing.Size(100, 20);
			this.m_labelTextBox.TabIndex = 5;
			// 
			// CurveGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_labelTextBox);
			this.Controls.Add(this.m_count);
			this.Controls.Add(this.m_label);
			this.Controls.Add(this.m_list);
			this.Name = "CurveGroupEditor";
			this.Size = new System.Drawing.Size(300, 300);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox m_list;
		private System.Windows.Forms.Label m_count;
		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.TextBox m_labelTextBox;
	}
}
