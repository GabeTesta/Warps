namespace Warps.Controls
{
	partial class NewCurvePopUp
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_surfaceRadio = new System.Windows.Forms.RadioButton();
			this.m_geodesicRadio = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.curveEditor1 = new Warps.CurveEditor();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.m_surfaceRadio);
			this.groupBox1.Controls.Add(this.m_geodesicRadio);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 57);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select Curve Type";
			// 
			// m_surfaceRadio
			// 
			this.m_surfaceRadio.AutoSize = true;
			this.m_surfaceRadio.Checked = true;
			this.m_surfaceRadio.Location = new System.Drawing.Point(110, 19);
			this.m_surfaceRadio.Name = "m_surfaceRadio";
			this.m_surfaceRadio.Size = new System.Drawing.Size(62, 17);
			this.m_surfaceRadio.TabIndex = 1;
			this.m_surfaceRadio.TabStop = true;
			this.m_surfaceRadio.Text = "Surface";
			this.m_surfaceRadio.UseVisualStyleBackColor = true;
			// 
			// m_geodesicRadio
			// 
			this.m_geodesicRadio.AutoSize = true;
			this.m_geodesicRadio.Location = new System.Drawing.Point(19, 19);
			this.m_geodesicRadio.Name = "m_geodesicRadio";
			this.m_geodesicRadio.Size = new System.Drawing.Size(70, 17);
			this.m_geodesicRadio.TabIndex = 0;
			this.m_geodesicRadio.Text = "Geodesic";
			this.m_geodesicRadio.UseVisualStyleBackColor = true;
			this.m_geodesicRadio.CheckedChanged += new System.EventHandler(this.m_geodesicRadio_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.curveEditor1);
			this.groupBox2.Location = new System.Drawing.Point(15, 71);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 147);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Fitpoints";
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.button1.Location = new System.Drawing.Point(18, 224);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = false;
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.button2.Location = new System.Drawing.Point(137, 224);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = false;
			// 
			// curveEditor1
			// 
			this.curveEditor1.Label = "";
			this.curveEditor1.Location = new System.Drawing.Point(16, 19);
			this.curveEditor1.Name = "curveEditor1";
			this.curveEditor1.Size = new System.Drawing.Size(167, 124);
			this.curveEditor1.TabIndex = 0;
			// 
			// NewCurvePopUp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(227, 258);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "NewCurvePopUp";
			this.Text = "New Curve Menu";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton m_surfaceRadio;
		private System.Windows.Forms.RadioButton m_geodesicRadio;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private CurveEditor curveEditor1;
	}
}