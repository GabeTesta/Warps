namespace Warps
{
	partial class TabTree
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
			this.m_tab = new System.Windows.Forms.TabControl();
			this.seqtab = new System.Windows.Forms.TabPage();
			this.m_seqtree = new System.Windows.Forms.TreeView();
			this.sortab = new System.Windows.Forms.TabPage();
			this.m_sortree = new System.Windows.Forms.TreeView();
			this.m_tab.SuspendLayout();
			this.seqtab.SuspendLayout();
			this.sortab.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_tab
			// 
			this.m_tab.Controls.Add(this.seqtab);
			this.m_tab.Controls.Add(this.sortab);
			this.m_tab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tab.Location = new System.Drawing.Point(0, 0);
			this.m_tab.Name = "m_tab";
			this.m_tab.SelectedIndex = 0;
			this.m_tab.Size = new System.Drawing.Size(241, 393);
			this.m_tab.TabIndex = 0;
			// 
			// seqtab
			// 
			this.seqtab.Controls.Add(this.m_seqtree);
			this.seqtab.Location = new System.Drawing.Point(4, 22);
			this.seqtab.Name = "seqtab";
			this.seqtab.Padding = new System.Windows.Forms.Padding(3);
			this.seqtab.Size = new System.Drawing.Size(233, 367);
			this.seqtab.TabIndex = 0;
			this.seqtab.Text = "Sequential";
			this.seqtab.UseVisualStyleBackColor = true;
			// 
			// m_seqtree
			// 
			this.m_seqtree.AllowDrop = true;
			this.m_seqtree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_seqtree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_seqtree.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_seqtree.HideSelection = false;
			this.m_seqtree.Location = new System.Drawing.Point(3, 3);
			this.m_seqtree.Name = "m_seqtree";
			this.m_seqtree.Size = new System.Drawing.Size(227, 361);
			this.m_seqtree.TabIndex = 0;
			this.m_seqtree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.m_seqtree_ItemDrag);
			this.m_seqtree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.m_seqtree_BeforeSelect);
			this.m_seqtree.DragDrop += new System.Windows.Forms.DragEventHandler(this.m_seqtree_DragDrop);
			this.m_seqtree.DragEnter += new System.Windows.Forms.DragEventHandler(this.m_seqtree_DragEnter);
			this.m_seqtree.DragOver += new System.Windows.Forms.DragEventHandler(this.m_seqtree_DragOver);
			this.m_seqtree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_seqtree_KeyDown);
			this.m_seqtree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.m_seqtree_KeyPress);
			this.m_seqtree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_seqtree_KeyUp);
			// 
			// sortab
			// 
			this.sortab.Controls.Add(this.m_sortree);
			this.sortab.Location = new System.Drawing.Point(4, 22);
			this.sortab.Name = "sortab";
			this.sortab.Padding = new System.Windows.Forms.Padding(3);
			this.sortab.Size = new System.Drawing.Size(233, 367);
			this.sortab.TabIndex = 1;
			this.sortab.Text = "Sorted";
			this.sortab.UseVisualStyleBackColor = true;
			// 
			// m_sortree
			// 
			this.m_sortree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_sortree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_sortree.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_sortree.HideSelection = false;
			this.m_sortree.Location = new System.Drawing.Point(3, 3);
			this.m_sortree.Name = "m_sortree";
			this.m_sortree.Size = new System.Drawing.Size(227, 361);
			this.m_sortree.TabIndex = 0;
			this.m_sortree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.m_seqtree_BeforeSelect);

			// 
			// TabTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_tab);
			this.Name = "TabTree";
			this.Size = new System.Drawing.Size(241, 393);
			this.m_tab.ResumeLayout(false);
			this.seqtab.ResumeLayout(false);
			this.sortab.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl m_tab;
		private System.Windows.Forms.TabPage seqtab;
		private System.Windows.Forms.TreeView m_seqtree;
		private System.Windows.Forms.TabPage sortab;
		private System.Windows.Forms.TreeView m_sortree;
	}
}
