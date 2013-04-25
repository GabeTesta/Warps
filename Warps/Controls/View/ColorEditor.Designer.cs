namespace Warps
{
	partial class ColorEditor
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
			this.m_rTxt = new System.Windows.Forms.TextBox();
			this.m_gTxt = new System.Windows.Forms.TextBox();
			this.m_bTxt = new System.Windows.Forms.TextBox();
			this.m_list = new System.Windows.Forms.ListView();
			this.colorWheel1 = new Warps.ColorWheel();
			this.SuspendLayout();
			// 
			// m_rTxt
			// 
			this.m_rTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.m_rTxt.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_rTxt.Location = new System.Drawing.Point(5, 174);
			this.m_rTxt.Name = "m_rTxt";
			this.m_rTxt.ReadOnly = true;
			this.m_rTxt.Size = new System.Drawing.Size(51, 20);
			this.m_rTxt.TabIndex = 1;
			this.m_rTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_gTxt
			// 
			this.m_gTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.m_gTxt.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_gTxt.Location = new System.Drawing.Point(62, 174);
			this.m_gTxt.Name = "m_gTxt";
			this.m_gTxt.ReadOnly = true;
			this.m_gTxt.Size = new System.Drawing.Size(51, 20);
			this.m_gTxt.TabIndex = 1;
			this.m_gTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_bTxt
			// 
			this.m_bTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.m_bTxt.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_bTxt.Location = new System.Drawing.Point(119, 174);
			this.m_bTxt.Name = "m_bTxt";
			this.m_bTxt.ReadOnly = true;
			this.m_bTxt.Size = new System.Drawing.Size(51, 20);
			this.m_bTxt.TabIndex = 1;
			this.m_bTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_list
			// 
			this.m_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_list.BackgroundImageTiled = true;
			this.m_list.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_list.FullRowSelect = true;
			this.m_list.LabelWrap = false;
			this.m_list.Location = new System.Drawing.Point(5, 200);
			this.m_list.Name = "m_list";
			this.m_list.ShowGroups = false;
			this.m_list.Size = new System.Drawing.Size(165, 247);
			this.m_list.TabIndex = 2;
			this.m_list.TileSize = new System.Drawing.Size(150, 50);
			this.m_list.UseCompatibleStateImageBehavior = false;
			this.m_list.View = System.Windows.Forms.View.List;
			this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
			// 
			// colorWheel1
			// 
			this.colorWheel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.colorWheel1.Hue = ((byte)(0));
			this.colorWheel1.Lightness = ((byte)(0));
			this.colorWheel1.Location = new System.Drawing.Point(5, 3);
			this.colorWheel1.Name = "colorWheel1";
			this.colorWheel1.Saturation = ((byte)(0));
			this.colorWheel1.SecondaryHues = null;
			this.colorWheel1.Size = new System.Drawing.Size(165, 165);
			this.colorWheel1.TabIndex = 0;
			this.colorWheel1.Text = "colorWheel1";
			this.colorWheel1.HueChanged += new System.EventHandler(this.colorWheel1_HueChanged);
			this.colorWheel1.SLChanged += new System.EventHandler(this.colorWheel1_HueChanged);
			// 
			// ColorEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_list);
			this.Controls.Add(this.m_bTxt);
			this.Controls.Add(this.m_gTxt);
			this.Controls.Add(this.m_rTxt);
			this.Controls.Add(this.colorWheel1);
			this.Name = "ColorEditor";
			this.Size = new System.Drawing.Size(175, 450);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ColorWheel colorWheel1;
		private System.Windows.Forms.TextBox m_rTxt;
		private System.Windows.Forms.TextBox m_gTxt;
		private System.Windows.Forms.TextBox m_bTxt;
		private System.Windows.Forms.ListView m_list;
	}
}
