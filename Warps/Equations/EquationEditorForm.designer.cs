namespace Warps
{
	partial class EquationEditorForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EquationEditorForm));
			this.addButton = new System.Windows.Forms.Button();
			this.CurveListBox = new System.Windows.Forms.ListBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteEquationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.EquationNameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.EquationListBox = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.valuebox = new System.Windows.Forms.TextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.autoCompleteTextBoxEdit = new Warps.Controls.AutoCompleteTextBox();
			this.autoCompleteTextBox1 = new Warps.Controls.AutoCompleteTextBox();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.BackColor = System.Drawing.Color.White;
			this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.addButton.Location = new System.Drawing.Point(369, 79);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(38, 23);
			this.addButton.TabIndex = 2;
			this.addButton.Text = "Add";
			this.addButton.UseVisualStyleBackColor = false;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			this.addButton.Enter += new System.EventHandler(this.EquationNameBox_Enter);
			// 
			// CurveListBox
			// 
			this.CurveListBox.FormattingEnabled = true;
			this.CurveListBox.Location = new System.Drawing.Point(12, 30);
			this.CurveListBox.Name = "CurveListBox";
			this.CurveListBox.Size = new System.Drawing.Size(136, 108);
			this.CurveListBox.TabIndex = 3;
			this.CurveListBox.Enter += new System.EventHandler(this.EquationNameBox_Enter);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteEquationToolStripMenuItem,
            this.recalculateToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(158, 48);
			// 
			// deleteEquationToolStripMenuItem
			// 
			this.deleteEquationToolStripMenuItem.Name = "deleteEquationToolStripMenuItem";
			this.deleteEquationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.deleteEquationToolStripMenuItem.Text = "Delete Equation";
			// 
			// recalculateToolStripMenuItem
			// 
			this.recalculateToolStripMenuItem.Name = "recalculateToolStripMenuItem";
			this.recalculateToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.recalculateToolStripMenuItem.Text = "Recalculate";
			// 
			// EquationNameBox
			// 
			this.EquationNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.EquationNameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.EquationNameBox.Location = new System.Drawing.Point(307, 27);
			this.EquationNameBox.Name = "EquationNameBox";
			this.EquationNameBox.Size = new System.Drawing.Size(100, 20);
			this.EquationNameBox.TabIndex = 0;
			this.EquationNameBox.Enter += new System.EventHandler(this.EquationNameBox_Enter);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(218, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Variable Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Available Curves:";
			// 
			// EquationListBox
			// 
			this.EquationListBox.FormattingEnabled = true;
			this.EquationListBox.Location = new System.Drawing.Point(15, 167);
			this.EquationListBox.Name = "EquationListBox";
			this.EquationListBox.Size = new System.Drawing.Size(133, 186);
			this.EquationListBox.TabIndex = 3;
			this.EquationListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 151);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Available Equations:";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(291, 183);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(116, 20);
			this.textBox1.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackColor = System.Drawing.Color.White;
			this.button1.Enabled = false;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(369, 264);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(38, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "Edit";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Enabled = false;
			this.label4.Location = new System.Drawing.Point(202, 186);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "Variable Name:";
			// 
			// valuebox
			// 
			this.valuebox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.valuebox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.valuebox.Enabled = false;
			this.valuebox.Location = new System.Drawing.Point(331, 238);
			this.valuebox.Name = "valuebox";
			this.valuebox.Size = new System.Drawing.Size(76, 20);
			this.valuebox.TabIndex = 13;
			// 
			// listView1
			// 
			this.listView1.Enabled = false;
			this.listView1.Location = new System.Drawing.Point(170, 256);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(121, 97);
			this.listView1.TabIndex = 14;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.List;
			this.listView1.Visible = false;
			// 
			// autoCompleteTextBoxEdit
			// 
			this.autoCompleteTextBoxEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.autoCompleteTextBoxEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.autoCompleteTextBoxEdit.Location = new System.Drawing.Point(170, 212);
			this.autoCompleteTextBoxEdit.Name = "autoCompleteTextBoxEdit";
			this.autoCompleteTextBoxEdit.Size = new System.Drawing.Size(237, 20);
			this.autoCompleteTextBoxEdit.TabIndex = 5;
			this.autoCompleteTextBoxEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.autoCompleteTextBoxEdit.Values = null;
			// 
			// autoCompleteTextBox1
			// 
			this.autoCompleteTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.autoCompleteTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.autoCompleteTextBox1.Location = new System.Drawing.Point(170, 53);
			this.autoCompleteTextBox1.Name = "autoCompleteTextBox1";
			this.autoCompleteTextBox1.Size = new System.Drawing.Size(237, 20);
			this.autoCompleteTextBox1.TabIndex = 1;
			this.autoCompleteTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.autoCompleteTextBox1.Values = null;
			// 
			// EquationEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(409, 361);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.autoCompleteTextBoxEdit);
			this.Controls.Add(this.autoCompleteTextBox1);
			this.Controls.Add(this.valuebox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.EquationListBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.EquationNameBox);
			this.Controls.Add(this.CurveListBox);
			this.Controls.Add(this.addButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EquationEditorForm";
			this.Text = "Equation Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EquationTester_FormClosing);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.ListBox CurveListBox;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteEquationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recalculateToolStripMenuItem;
		private System.Windows.Forms.TextBox EquationNameBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox EquationListBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox valuebox;
		private Controls.AutoCompleteTextBox autoCompleteTextBox1;
		private Controls.AutoCompleteTextBox autoCompleteTextBoxEdit;
		private System.Windows.Forms.ListView listView1;
	}
}