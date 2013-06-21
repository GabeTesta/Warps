namespace Warps.Controls
{
	partial class EquationBoxAuto
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
			this.button1 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.equationBox = new Warps.EquationBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackColor = System.Drawing.Color.White;
			this.button1.BackgroundImage = global::Warps.Properties.Resources.equation;
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(81, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(23, 23);
			this.button1.TabIndex = 1;
			this.button1.TabStop = false;
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// equationBox
			// 
			this.equationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.equationBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.equationBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.equationBox.Location = new System.Drawing.Point(0, 0);
			this.equationBox.Margin = new System.Windows.Forms.Padding(0);
			this.equationBox.Name = "equationBox";
			this.equationBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.equationBox.Size = new System.Drawing.Size(78, 23);
			this.equationBox.TabIndex = 0;
			this.equationBox.Text = "0.0000";
			this.equationBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.equationBox.Value = 0D;
			this.equationBox.Values = null;
			this.equationBox.TextChanged += new System.EventHandler(this.equationBox_TextChanged);
			// 
			// EquationBoxAuto
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.equationBox);
			this.Controls.Add(this.button1);
			this.Name = "EquationBoxAuto";
			this.Size = new System.Drawing.Size(101, 24);
			this.Load += new System.EventHandler(this.EquationBoxAuto_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private EquationBox equationBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
