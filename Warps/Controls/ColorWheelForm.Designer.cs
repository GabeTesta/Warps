namespace Warps.Controls
{
	partial class ColorWheelForm
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
			this.colorWheel1 = new Warps.ColorWheel();
			this.SuspendLayout();
			// 
			// colorWheel1
			// 
			this.colorWheel1.BackColor = System.Drawing.Color.Transparent;
			this.colorWheel1.Hue = ((byte)(0));
			this.colorWheel1.Lightness = ((byte)(0));
			this.colorWheel1.Location = new System.Drawing.Point(12, 3);
			this.colorWheel1.Name = "colorWheel1";
			this.colorWheel1.Saturation = ((byte)(0));
			this.colorWheel1.SecondaryHues = null;
			this.colorWheel1.Size = new System.Drawing.Size(169, 159);
			this.colorWheel1.TabIndex = 0;
			this.colorWheel1.Text = "colorWheel1";
			this.colorWheel1.HueChanged += new System.EventHandler(this.colorWheel1_HueChanged);
			this.colorWheel1.SLChanged += new System.EventHandler(this.colorWheel1_SLChanged);
			// 
			// ColorWheelForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(183, 164);
			this.Controls.Add(this.colorWheel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ColorWheelForm";
			this.Text = "ColorWheelForm";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorWheelForm_Paint);
			this.ResumeLayout(false);

		}

		#endregion

		private ColorWheel colorWheel1;
	}
}