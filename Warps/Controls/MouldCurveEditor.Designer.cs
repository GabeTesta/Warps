namespace Warps.Controls
{
	partial class MouldCurveEditor
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
			this.components = new System.ComponentModel.Container();
			this.m_label = new System.Windows.Forms.TextBox();
			this.m_length = new System.Windows.Forms.Label();
			this.m_panel = new System.Windows.Forms.Panel();
			this.m_Popup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.m_add = new System.Windows.Forms.ToolStripMenuItem();
			this.m_insert = new System.Windows.Forms.ToolStripMenuItem();
			this.m_delete = new System.Windows.Forms.ToolStripMenuItem();
			this.m_w4lBtn = new System.Windows.Forms.Button();
			this.m_Popup.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_label
			// 
			this.m_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_label.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(0, 3);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(150, 20);
			this.m_label.TabIndex = 0;
			this.m_label.Text = "Label";
			// 
			// m_length
			// 
			this.m_length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_length.AutoSize = true;
			this.m_length.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_length.Location = new System.Drawing.Point(156, 6);
			this.m_length.Name = "m_length";
			this.m_length.Size = new System.Drawing.Size(43, 13);
			this.m_length.TabIndex = 2;
			this.m_length.Text = "0.0000";
			// 
			// m_panel
			// 
			this.m_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_panel.AutoScroll = true;
			this.m_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_panel.Location = new System.Drawing.Point(3, 29);
			this.m_panel.Name = "m_panel";
			this.m_panel.Size = new System.Drawing.Size(244, 405);
			this.m_panel.TabIndex = 1;
			// 
			// m_Popup
			// 
			this.m_Popup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_add,
            this.m_insert,
            this.m_delete});
			this.m_Popup.Name = "m_Popup";
			this.m_Popup.Size = new System.Drawing.Size(108, 70);
			this.m_Popup.Opened += new System.EventHandler(this.m_Popup_Opened);
			// 
			// m_add
			// 
			this.m_add.Name = "m_add";
			this.m_add.Size = new System.Drawing.Size(107, 22);
			this.m_add.Text = "Add";
			this.m_add.Click += new System.EventHandler(this.m_add_Click);
			// 
			// m_insert
			// 
			this.m_insert.Name = "m_insert";
			this.m_insert.Size = new System.Drawing.Size(107, 22);
			this.m_insert.Text = "Insert";
			this.m_insert.Click += new System.EventHandler(this.m_insert_Click);
			// 
			// m_delete
			// 
			this.m_delete.Name = "m_delete";
			this.m_delete.Size = new System.Drawing.Size(107, 22);
			this.m_delete.Text = "Delete";
			this.m_delete.Click += new System.EventHandler(this.m_delete_Click);
			// 
			// m_w4lBtn
			// 
			this.m_w4lBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_w4lBtn.FlatAppearance.BorderSize = 0;
			this.m_w4lBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_w4lBtn.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_w4lBtn.Location = new System.Drawing.Point(205, 1);
			this.m_w4lBtn.Name = "m_w4lBtn";
			this.m_w4lBtn.Size = new System.Drawing.Size(42, 22);
			this.m_w4lBtn.TabIndex = 0;
			this.m_w4lBtn.TabStop = false;
			this.m_w4lBtn.Text = "W4L";
			this.m_w4lBtn.UseVisualStyleBackColor = true;
			this.m_w4lBtn.Click += new System.EventHandler(this.m_w4lBtn_Click);
			// 
			// MouldCurveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ContextMenuStrip = this.m_Popup;
			this.Controls.Add(this.m_w4lBtn);
			this.Controls.Add(this.m_panel);
			this.Controls.Add(this.m_label);
			this.Controls.Add(this.m_length);
			this.Name = "MouldCurveEditor";
			this.Size = new System.Drawing.Size(250, 434);
			this.m_Popup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_label;
		private System.Windows.Forms.Label m_length;
		private System.Windows.Forms.Panel m_panel;
		private System.Windows.Forms.ContextMenuStrip m_Popup;
		private System.Windows.Forms.ToolStripMenuItem m_add;
		private System.Windows.Forms.ToolStripMenuItem m_insert;
		private System.Windows.Forms.ToolStripMenuItem m_delete;
		private System.Windows.Forms.Button m_w4lBtn;
	}
}
