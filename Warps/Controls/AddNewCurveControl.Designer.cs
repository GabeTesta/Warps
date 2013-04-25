namespace Warps.Controls
{
	partial class AddNewCurveControl
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.surfaceTab = new System.Windows.Forms.TabPage();
			this.m_addBtn = new System.Windows.Forms.Button();
			this.m_delBtn = new System.Windows.Forms.Button();
			this.geotab = new System.Windows.Forms.TabPage();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pointTypeSwitcher4 = new Warps.PointTypeSwitcher();
			this.pointTypeSwitcher5 = new Warps.PointTypeSwitcher();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tabControl1.SuspendLayout();
			this.surfaceTab.SuspendLayout();
			this.geotab.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.surfaceTab);
			this.tabControl1.Controls.Add(this.geotab);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(406, 316);
			this.tabControl1.TabIndex = 0;
			// 
			// surfaceTab
			// 
			this.surfaceTab.Controls.Add(this.flowLayoutPanel1);
			this.surfaceTab.Controls.Add(this.m_addBtn);
			this.surfaceTab.Controls.Add(this.m_delBtn);
			this.surfaceTab.Location = new System.Drawing.Point(4, 22);
			this.surfaceTab.Name = "surfaceTab";
			this.surfaceTab.Padding = new System.Windows.Forms.Padding(3);
			this.surfaceTab.Size = new System.Drawing.Size(398, 290);
			this.surfaceTab.TabIndex = 0;
			this.surfaceTab.Text = "Surface";
			this.surfaceTab.UseVisualStyleBackColor = true;
			// 
			// m_addBtn
			// 
			this.m_addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_addBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_addBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_addBtn.Location = new System.Drawing.Point(343, 6);
			this.m_addBtn.Name = "m_addBtn";
			this.m_addBtn.Size = new System.Drawing.Size(49, 26);
			this.m_addBtn.TabIndex = 6;
			this.m_addBtn.Text = "+";
			this.m_addBtn.UseVisualStyleBackColor = true;
			this.m_addBtn.Enter += new System.EventHandler(this.button_MouseEnter);
			this.m_addBtn.Leave += new System.EventHandler(this.button_Leave);
			this.m_addBtn.MouseEnter += new System.EventHandler(this.button_MouseEnter);
			this.m_addBtn.MouseLeave += new System.EventHandler(this.button_Leave);
			// 
			// m_delBtn
			// 
			this.m_delBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_delBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_delBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_delBtn.Location = new System.Drawing.Point(343, 38);
			this.m_delBtn.Name = "m_delBtn";
			this.m_delBtn.Size = new System.Drawing.Size(49, 26);
			this.m_delBtn.TabIndex = 4;
			this.m_delBtn.Text = "-";
			this.m_delBtn.UseVisualStyleBackColor = true;
			this.m_delBtn.Enter += new System.EventHandler(this.button_MouseEnter);
			this.m_delBtn.Leave += new System.EventHandler(this.button_Leave);
			this.m_delBtn.MouseEnter += new System.EventHandler(this.button_MouseEnter);
			this.m_delBtn.MouseLeave += new System.EventHandler(this.button_Leave);
			// 
			// geotab
			// 
			this.geotab.Controls.Add(this.button3);
			this.geotab.Controls.Add(this.button4);
			this.geotab.Controls.Add(this.panel2);
			this.geotab.Location = new System.Drawing.Point(4, 22);
			this.geotab.Name = "geotab";
			this.geotab.Padding = new System.Windows.Forms.Padding(3);
			this.geotab.Size = new System.Drawing.Size(316, 154);
			this.geotab.TabIndex = 1;
			this.geotab.Text = "Geodesic";
			this.geotab.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button3.Location = new System.Drawing.Point(209, 128);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(49, 23);
			this.button3.TabIndex = 8;
			this.button3.Text = "+";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button4.Location = new System.Drawing.Point(264, 128);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(49, 23);
			this.button4.TabIndex = 7;
			this.button4.Text = "-";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.Controls.Add(this.pointTypeSwitcher4);
			this.panel2.Controls.Add(this.pointTypeSwitcher5);
			this.panel2.Location = new System.Drawing.Point(7, 7);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(303, 100);
			this.panel2.TabIndex = 0;
			// 
			// pointTypeSwitcher4
			// 
			this.pointTypeSwitcher4.Location = new System.Drawing.Point(3, 3);
			this.pointTypeSwitcher4.Name = "pointTypeSwitcher4";
			this.pointTypeSwitcher4.Size = new System.Drawing.Size(306, 36);
			this.pointTypeSwitcher4.TabIndex = 1;
			// 
			// pointTypeSwitcher5
			// 
			this.pointTypeSwitcher5.Location = new System.Drawing.Point(3, 36);
			this.pointTypeSwitcher5.Name = "pointTypeSwitcher5";
			this.pointTypeSwitcher5.Size = new System.Drawing.Size(303, 36);
			this.pointTypeSwitcher5.TabIndex = 2;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 6);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(331, 278);
			this.flowLayoutPanel1.TabIndex = 7;
			// 
			// AddNewCurveControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Name = "AddNewCurveControl";
			this.Size = new System.Drawing.Size(406, 316);
			this.tabControl1.ResumeLayout(false);
			this.surfaceTab.ResumeLayout(false);
			this.geotab.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage surfaceTab;
		private System.Windows.Forms.TabPage geotab;
		private System.Windows.Forms.Button m_delBtn;
		private System.Windows.Forms.Button m_addBtn;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Panel panel2;
		private PointTypeSwitcher pointTypeSwitcher5;
		private PointTypeSwitcher pointTypeSwitcher4;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
