namespace Warps.Controls
{
	partial class VariableGroupEditor
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
			this.m_count = new System.Windows.Forms.Label();
			this.m_label = new System.Windows.Forms.Label();
			this.m_flow = new System.Windows.Forms.FlowLayoutPanel();
			this.m_labelBox = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_count
			// 
			this.m_count.AutoSize = true;
			this.m_count.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_count.Location = new System.Drawing.Point(3, 33);
			this.m_count.Name = "m_count";
			this.m_count.Size = new System.Drawing.Size(56, 14);
			this.m_count.TabIndex = 7;
			this.m_count.Text = "Count: ";
			// 
			// m_label
			// 
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(3, 10);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(56, 14);
			this.m_label.TabIndex = 6;
			this.m_label.Text = "Label: ";
			// 
			// m_flow
			// 
			this.m_flow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_flow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.m_flow.Location = new System.Drawing.Point(6, 70);
			this.m_flow.Name = "m_flow";
			this.m_flow.Size = new System.Drawing.Size(220, 227);
			this.m_flow.TabIndex = 8;
			// 
			// m_labelBox
			// 
			this.m_labelBox.Location = new System.Drawing.Point(53, 7);
			this.m_labelBox.Name = "m_labelBox";
			this.m_labelBox.Size = new System.Drawing.Size(170, 20);
			this.m_labelBox.TabIndex = 9;
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(124, 33);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(45, 28);
			this.button1.TabIndex = 0;
			this.button1.Text = "New";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Location = new System.Drawing.Point(175, 33);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(48, 28);
			this.button2.TabIndex = 10;
			this.button2.Text = "Delete";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// VariableGroupEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.m_labelBox);
			this.Controls.Add(this.m_flow);
			this.Controls.Add(this.m_count);
			this.Controls.Add(this.m_label);
			this.Name = "VariableGroupEditor";
			this.Size = new System.Drawing.Size(229, 300);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_count;
		private System.Windows.Forms.Label m_label;
		private System.Windows.Forms.FlowLayoutPanel m_flow;
		private System.Windows.Forms.TextBox m_labelBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}
