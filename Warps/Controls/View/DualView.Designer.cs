namespace Warps
{
	partial class DualView
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
			devDept.Eyeshot.BackgroundSettings backgroundSettings1 = new devDept.Eyeshot.BackgroundSettings(devDept.Eyeshot.backgroundStyleType.LinearGradient, System.Drawing.Color.WhiteSmoke, System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(163)))), ((int)(((byte)(210))))), 0.75D, null);
			devDept.Eyeshot.Camera camera1 = new devDept.Eyeshot.Camera(new devDept.Geometry.Point3D(0D, 0D, 0D), 100D, new devDept.Geometry.Quaternion(0.12940952255126034D, 0.22414386804201339D, 0.4829629131445341D, 0.83651630373780794D), devDept.Eyeshot.projectionType.Perspective, 50D, 13.83999784132426D);
			devDept.Eyeshot.ToolBarButton toolBarButton1 = new devDept.Eyeshot.ToolBarButton(null, "Zoom Window", "Zoom Window", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton2 = new devDept.Eyeshot.ToolBarButton(null, "Zoom", "Zoom", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton3 = new devDept.Eyeshot.ToolBarButton(null, "Pan", "Pan", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton4 = new devDept.Eyeshot.ToolBarButton(null, "Rotate", "Rotate", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton5 = new devDept.Eyeshot.ToolBarButton(null, "Zoom Fit", "Zoom Fit", System.Windows.Forms.ToolBarButtonStyle.PushButton, true);
			devDept.Eyeshot.ToolBar toolBar1 = new devDept.Eyeshot.ToolBar(devDept.Eyeshot.toolBarPositionType.HorizontalTopCenter, true, new devDept.Eyeshot.ToolBarButton[] {
            toolBarButton1,
            toolBarButton2,
            toolBarButton3,
            toolBarButton4,
            toolBarButton5});
			devDept.Eyeshot.Legend legend1 = new devDept.Eyeshot.Legend(0D, 100D, "Title", "Subtitle", new System.Drawing.Point(24, 24), new System.Drawing.Size(10, 30), true, false, false, "{0:0.##}", System.Drawing.Color.Transparent, System.Drawing.Color.Black, System.Drawing.Color.Black, new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold), new System.Drawing.Font("Tahoma", 8.25F), new System.Drawing.Color[] {
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(63)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(191)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(191))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(127))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(63))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(191)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(127)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(63)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))))});
			devDept.Eyeshot.Grid grid1 = new devDept.Eyeshot.Grid(new devDept.Geometry.Point3D(-50D, -50D, 0D), new devDept.Geometry.Point3D(100D, 100D, 0D), 1D, new devDept.Geometry.Plane(new devDept.Geometry.Point3D(0D, 0D, 0D), new devDept.Geometry.Vector3D(0D, 0D, 1D)), System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))), System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))), false, true, false, false, 10, 100, 10, System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90))))));
			devDept.Eyeshot.OriginSymbol originSymbol1 = new devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, new System.Drawing.Font("Tahoma", 8.25F), System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", true);
			devDept.Eyeshot.RotateSettings rotateSettings1 = new devDept.Eyeshot.RotateSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.None), 10D, true, 1D, devDept.Eyeshot.rotationStyleType.Trackball, devDept.Eyeshot.rotationCenterType.CursorLocation, new devDept.Geometry.Point3D(0D, 0D, 0D));
			devDept.Eyeshot.ZoomSettings zoomSettings1 = new devDept.Eyeshot.ZoomSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.Shift), 25, true, devDept.Eyeshot.zoomStyleType.AtCursorLocation, false, 1D, System.Drawing.Color.DeepSkyBlue, devDept.Eyeshot.perspectiveFitType.Accurate);
			devDept.Eyeshot.Viewport viewport1 = new devDept.Eyeshot.Viewport(new System.Drawing.Point(0, 0), new System.Drawing.Size(286, 346), backgroundSettings1, camera1, toolBar1, new devDept.Eyeshot.Legend[] {
            legend1}, devDept.Eyeshot.displayType.Rendered, true, false, false, new devDept.Eyeshot.Grid[] {
            grid1}, originSymbol1, false, rotateSettings1, zoomSettings1, new devDept.Eyeshot.PanSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.Ctrl), 25, true));
			devDept.Eyeshot.BackgroundSettings backgroundSettings2 = new devDept.Eyeshot.BackgroundSettings(devDept.Eyeshot.backgroundStyleType.LinearGradient, System.Drawing.Color.WhiteSmoke, System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(163)))), ((int)(((byte)(210))))), 0.75D, null);
			devDept.Eyeshot.Camera camera2 = new devDept.Eyeshot.Camera(new devDept.Geometry.Point3D(0D, 0D, 0D), 100D, new devDept.Geometry.Quaternion(0.12940952255126034D, 0.22414386804201339D, 0.4829629131445341D, 0.83651630373780794D), devDept.Eyeshot.projectionType.Perspective, 50D, 13.83999784132426D);
			devDept.Eyeshot.ToolBarButton toolBarButton6 = new devDept.Eyeshot.ToolBarButton(null, "Zoom Window", "Zoom Window", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton7 = new devDept.Eyeshot.ToolBarButton(null, "Zoom", "Zoom", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton8 = new devDept.Eyeshot.ToolBarButton(null, "Pan", "Pan", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton9 = new devDept.Eyeshot.ToolBarButton(null, "Rotate", "Rotate", System.Windows.Forms.ToolBarButtonStyle.ToggleButton, true);
			devDept.Eyeshot.ToolBarButton toolBarButton10 = new devDept.Eyeshot.ToolBarButton(null, "Zoom Fit", "Zoom Fit", System.Windows.Forms.ToolBarButtonStyle.PushButton, true);
			devDept.Eyeshot.ToolBar toolBar2 = new devDept.Eyeshot.ToolBar(devDept.Eyeshot.toolBarPositionType.HorizontalTopCenter, true, new devDept.Eyeshot.ToolBarButton[] {
            toolBarButton6,
            toolBarButton7,
            toolBarButton8,
            toolBarButton9,
            toolBarButton10});
			devDept.Eyeshot.Legend legend2 = new devDept.Eyeshot.Legend(0D, 100D, "Title", "Subtitle", new System.Drawing.Point(24, 24), new System.Drawing.Size(10, 30), true, false, false, "{0:0.##}", System.Drawing.Color.Transparent, System.Drawing.Color.Black, System.Drawing.Color.Black, new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold), new System.Drawing.Font("Tahoma", 8.25F), new System.Drawing.Color[] {
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(63)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(191)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(191))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(127))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(63))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(191)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(127)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(63)))), ((int)(((byte)(0))))),
            System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))))});
			devDept.Eyeshot.Grid grid2 = new devDept.Eyeshot.Grid(new devDept.Geometry.Point3D(-50D, -50D, 0D), new devDept.Geometry.Point3D(100D, 100D, 0D), 1D, new devDept.Geometry.Plane(new devDept.Geometry.Point3D(0D, 0D, 0D), new devDept.Geometry.Vector3D(0D, 0D, 1D)), System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))), System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))), false, true, false, false, 10, 100, 10, System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90))))));
			devDept.Eyeshot.OriginSymbol originSymbol2 = new devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, new System.Drawing.Font("Tahoma", 8.25F), System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", true);
			devDept.Eyeshot.RotateSettings rotateSettings2 = new devDept.Eyeshot.RotateSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.None), 10D, true, 1D, devDept.Eyeshot.rotationStyleType.Trackball, devDept.Eyeshot.rotationCenterType.CursorLocation, new devDept.Geometry.Point3D(0D, 0D, 0D));
			devDept.Eyeshot.ZoomSettings zoomSettings2 = new devDept.Eyeshot.ZoomSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.Shift), 25, true, devDept.Eyeshot.zoomStyleType.AtCursorLocation, false, 1D, System.Drawing.Color.DeepSkyBlue, devDept.Eyeshot.perspectiveFitType.Accurate);
			devDept.Eyeshot.Viewport viewport2 = new devDept.Eyeshot.Viewport(new System.Drawing.Point(0, 0), new System.Drawing.Size(242, 346), backgroundSettings2, camera2, toolBar2, new devDept.Eyeshot.Legend[] {
            legend2}, devDept.Eyeshot.displayType.Rendered, true, false, false, new devDept.Eyeshot.Grid[] {
            grid2}, originSymbol2, false, rotateSettings2, zoomSettings2, new devDept.Eyeshot.PanSettings(new devDept.Eyeshot.MouseButton(System.Windows.Forms.MouseButtons.Middle, devDept.Eyeshot.ModifierKeys.Ctrl), 25, true));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.LtoR = new System.Windows.Forms.Button();
			this.m_btnLeft = new System.Windows.Forms.Button();
			this.m_viewleft = new devDept.Eyeshot.SingleViewportLayout();
			this.m_dualViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.m_camerastrip = new System.Windows.Forms.ToolStripMenuItem();
			this.m_layersToolStrip = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleArrowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RtoL = new System.Windows.Forms.Button();
			this.m_btnRight = new System.Windows.Forms.Button();
			this.m_viewright = new devDept.Eyeshot.SingleViewportLayout();
			this.saveColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.loadColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_viewleft)).BeginInit();
			this.m_dualViewContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_viewright)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.LtoR);
			this.splitContainer1.Panel1.Controls.Add(this.m_btnLeft);
			this.splitContainer1.Panel1.Controls.Add(this.m_viewleft);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.RtoL);
			this.splitContainer1.Panel2.Controls.Add(this.m_btnRight);
			this.splitContainer1.Panel2.Controls.Add(this.m_viewright);
			this.splitContainer1.Size = new System.Drawing.Size(532, 346);
			this.splitContainer1.SplitterDistance = 286;
			this.splitContainer1.TabIndex = 0;
			// 
			// LtoR
			// 
			this.LtoR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LtoR.BackColor = System.Drawing.Color.Transparent;
			this.LtoR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LtoR.Location = new System.Drawing.Point(270, 0);
			this.LtoR.Name = "LtoR";
			this.LtoR.Size = new System.Drawing.Size(16, 24);
			this.LtoR.TabIndex = 3;
			this.LtoR.Text = ">";
			this.LtoR.UseVisualStyleBackColor = false;
			this.LtoR.Click += new System.EventHandler(this.LtoR_Click);
			// 
			// m_btnLeft
			// 
			this.m_btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_btnLeft.Location = new System.Drawing.Point(255, 323);
			this.m_btnLeft.Name = "m_btnLeft";
			this.m_btnLeft.Size = new System.Drawing.Size(31, 23);
			this.m_btnLeft.TabIndex = 1;
			this.m_btnLeft.Text = "<<";
			this.m_btnLeft.UseVisualStyleBackColor = true;
			this.m_btnLeft.Click += new System.EventHandler(this.m_expandBtn_Click);
			// 
			// m_viewleft
			// 
			this.m_viewleft.AskForFsaa = true;
			this.m_viewleft.ContextMenuStrip = this.m_dualViewContextMenu;
			this.m_viewleft.Cursor = System.Windows.Forms.Cursors.Default;
			this.m_viewleft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_viewleft.FsaaSamples = devDept.Eyeshot.antialiasingSamplesNumberType.x2;
			this.m_viewleft.Location = new System.Drawing.Point(0, 0);
			this.m_viewleft.Name = "m_viewleft";
			this.m_viewleft.PlanarShadowOpacity = 0D;
			this.m_viewleft.Size = new System.Drawing.Size(286, 346);
			this.m_viewleft.TabIndex = 0;
			this.m_viewleft.Text = "Left View";
			viewport1.Legends = new devDept.Eyeshot.Legend[] {
        legend1};
			this.m_viewleft.Viewports.Add(viewport1);
			this.m_viewleft.Enter += new System.EventHandler(this.focus_Enter);
			this.m_viewleft.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_viewleft_KeyUp);
			this.m_viewleft.Leave += new System.EventHandler(this.focus_Leave);
			this.m_viewleft.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_viewleft_MouseDoubleClick);
			// 
			// m_dualViewContextMenu
			// 
			this.m_dualViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_camerastrip,
            this.m_layersToolStrip,
            this.gridToolStripMenuItem,
            this.toolStripSeparator2,
            this.toggleArrowsToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem1,
            this.saveColorsToolStripMenuItem,
            this.loadColorsToolStripMenuItem});
			this.m_dualViewContextMenu.Name = "m_dualViewContextMenu";
			this.m_dualViewContextMenu.Size = new System.Drawing.Size(153, 192);
			this.m_dualViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu_Opening);
			// 
			// m_camerastrip
			// 
			this.m_camerastrip.Name = "m_camerastrip";
			this.m_camerastrip.Size = new System.Drawing.Size(152, 22);
			this.m_camerastrip.Text = "Camera";
			// 
			// m_layersToolStrip
			// 
			this.m_layersToolStrip.Name = "m_layersToolStrip";
			this.m_layersToolStrip.Size = new System.Drawing.Size(152, 22);
			this.m_layersToolStrip.Text = "Layers";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.Control;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem1.Text = "Colors...";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.colorsToolStripMenuItem_Click);
			// 
			// toggleArrowsToolStripMenuItem
			// 
			this.toggleArrowsToolStripMenuItem.Name = "toggleArrowsToolStripMenuItem";
			this.toggleArrowsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.toggleArrowsToolStripMenuItem.Text = "Toggle Arrows";
			this.toggleArrowsToolStripMenuItem.Click += new System.EventHandler(this.toggleArrowsToolStripMenuItem_Click);
			// 
			// RtoL
			// 
			this.RtoL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RtoL.Location = new System.Drawing.Point(0, 0);
			this.RtoL.Name = "RtoL";
			this.RtoL.Size = new System.Drawing.Size(16, 24);
			this.RtoL.TabIndex = 2;
			this.RtoL.Text = "<";
			this.RtoL.UseVisualStyleBackColor = true;
			this.RtoL.Click += new System.EventHandler(this.RtoL_Click);
			// 
			// m_btnRight
			// 
			this.m_btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.m_btnRight.Location = new System.Drawing.Point(0, 323);
			this.m_btnRight.Name = "m_btnRight";
			this.m_btnRight.Size = new System.Drawing.Size(31, 23);
			this.m_btnRight.TabIndex = 1;
			this.m_btnRight.Text = ">>";
			this.m_btnRight.UseVisualStyleBackColor = true;
			this.m_btnRight.Click += new System.EventHandler(this.m_expandBtn_Click);
			// 
			// m_viewright
			// 
			this.m_viewright.AskForFsaa = true;
			this.m_viewright.ContextMenuStrip = this.m_dualViewContextMenu;
			this.m_viewright.Cursor = System.Windows.Forms.Cursors.Default;
			this.m_viewright.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_viewright.FsaaSamples = devDept.Eyeshot.antialiasingSamplesNumberType.x2;
			this.m_viewright.Location = new System.Drawing.Point(0, 0);
			this.m_viewright.Name = "m_viewright";
			this.m_viewright.Size = new System.Drawing.Size(242, 346);
			this.m_viewright.TabIndex = 0;
			this.m_viewright.Text = "Right View";
			viewport2.Legends = new devDept.Eyeshot.Legend[] {
        legend2};
			this.m_viewright.Viewports.Add(viewport2);
			this.m_viewright.Enter += new System.EventHandler(this.focus_Enter);
			this.m_viewright.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_viewleft_KeyUp);
			this.m_viewright.Leave += new System.EventHandler(this.focus_Leave);
			this.m_viewright.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_viewleft_MouseDoubleClick);
			// 
			// saveColorsToolStripMenuItem
			// 
			this.saveColorsToolStripMenuItem.Name = "saveColorsToolStripMenuItem";
			this.saveColorsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.saveColorsToolStripMenuItem.Text = "Save Colors";
			this.saveColorsToolStripMenuItem.Click += new System.EventHandler(this.saveColorsToolStripMenuItem_Click_1);
			// 
			// gridToolStripMenuItem
			// 
			this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
			this.gridToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.gridToolStripMenuItem.Text = "Grid";
			this.gridToolStripMenuItem.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// loadColorsToolStripMenuItem
			// 
			this.loadColorsToolStripMenuItem.Name = "loadColorsToolStripMenuItem";
			this.loadColorsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.loadColorsToolStripMenuItem.Text = "Load Colors";
			this.loadColorsToolStripMenuItem.Click += new System.EventHandler(this.loadColorsToolStripMenuItem_Click);
			// 
			// DualView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "DualView";
			this.Size = new System.Drawing.Size(532, 346);
			this.Load += new System.EventHandler(this.DualView_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_viewleft)).EndInit();
			this.m_dualViewContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_viewright)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private devDept.Eyeshot.SingleViewportLayout m_viewleft;
		private devDept.Eyeshot.SingleViewportLayout m_viewright;
        private System.Windows.Forms.ContextMenuStrip m_dualViewContextMenu;
	   private System.Windows.Forms.ToolStripMenuItem m_camerastrip;
	   private System.Windows.Forms.ToolStripMenuItem m_layersToolStrip;
	   private System.Windows.Forms.Button m_btnLeft;
	   private System.Windows.Forms.Button m_btnRight;
       private System.Windows.Forms.Button LtoR;
       private System.Windows.Forms.Button RtoL;
	  private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	  private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
	  private System.Windows.Forms.ToolStripMenuItem toggleArrowsToolStripMenuItem;
	  private System.Windows.Forms.ToolStripMenuItem saveColorsToolStripMenuItem;
	  private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
	  private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	  private System.Windows.Forms.ToolStripMenuItem loadColorsToolStripMenuItem;
	}
}
