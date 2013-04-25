namespace Warps
{
	partial class CurveEditor
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
			this.m_label = new System.Windows.Forms.Label();
			this.m_length = new System.Windows.Forms.Label();
			this.m_flow = new System.Windows.Forms.FlowLayoutPanel();
			this.m_lblBox = new System.Windows.Forms.TextBox();
			this.m_add = new System.Windows.Forms.Button();
			this.m_delete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_label
			// 
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(3, 5);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(56, 14);
			this.m_label.TabIndex = 0;
			this.m_label.Text = "Label: ";
			// 
			// m_length
			// 
			this.m_length.AutoSize = true;
			this.m_length.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_length.Location = new System.Drawing.Point(3, 30);
			this.m_length.Name = "m_length";
			this.m_length.Size = new System.Drawing.Size(63, 14);
			this.m_length.TabIndex = 1;
			this.m_length.Text = "Length: ";
			// 
			// m_flow
			// 
			this.m_flow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_flow.AutoScroll = true;
			this.m_flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.m_flow.Location = new System.Drawing.Point(0, 54);
			this.m_flow.Name = "m_flow";
			this.m_flow.Size = new System.Drawing.Size(326, 246);
			this.m_flow.TabIndex = 2;
			this.m_flow.WrapContents = false;
			this.m_flow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_flow_MouseDown);
			this.m_flow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_flow_MouseMove);
			this.m_flow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_flow_MouseUp);
			// 
			// m_lblBox
			// 
			this.m_lblBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_lblBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblBox.Location = new System.Drawing.Point(65, 3);
			this.m_lblBox.Name = "m_lblBox";
			this.m_lblBox.Size = new System.Drawing.Size(261, 20);
			this.m_lblBox.TabIndex = 3;
			// 
			// m_add
			// 
			this.m_add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_add.BackColor = System.Drawing.Color.White;
			this.m_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_add.Location = new System.Drawing.Point(209, 25);
			this.m_add.Name = "m_add";
			this.m_add.Size = new System.Drawing.Size(44, 23);
			this.m_add.TabIndex = 4;
			this.m_add.Text = "Add";
			this.m_add.UseVisualStyleBackColor = false;
			this.m_add.Click += new System.EventHandler(this.m_add_Click);
			// 
			// button1
			// 
			this.m_delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_delete.BackColor = System.Drawing.Color.White;
			this.m_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_delete.Location = new System.Drawing.Point(270, 25);
			this.m_delete.Name = "button1";
			this.m_delete.Size = new System.Drawing.Size(53, 23);
			this.m_delete.TabIndex = 5;
			this.m_delete.Text = "Delete";
			this.m_delete.UseVisualStyleBackColor = false;
			this.m_delete.Click += new System.EventHandler(this.button1_Click);
			// 
			// CurveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_delete);
			this.Controls.Add(this.m_add);
			this.Controls.Add(this.m_lblBox);
			this.Controls.Add(this.m_flow);
			this.Controls.Add(this.m_length);
			this.Controls.Add(this.m_label);
			this.Name = "CurveEditor";
			this.Size = new System.Drawing.Size(326, 300);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.Label m_length;
		private System.Windows.Forms.FlowLayoutPanel m_flow;
		private System.Windows.Forms.TextBox m_lblBox;
		private System.Windows.Forms.Button m_add;
		private System.Windows.Forms.Button m_delete;
	}
}
