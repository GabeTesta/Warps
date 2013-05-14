using devDept;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Labels;

namespace Warps
{
	public partial class DualView : UserControl
	{
		public DualView()
		{
			InitializeComponent();
			PopulateColors();
			
			for (int i = 0; i < 2; i++)
			{

				this[i].Unlock("EYENBS-6216-4FJGA-JUE2S-L8MPA");
				this[i].Rendered.EnvironmentMapping = false;
				this[i].Rendered.PlanarReflections = false;

				this[i].PlanarShadowOpacity = 0;
				this[i].Units = unitSystemType.Meters;

				this[i].ProgressBar.HideCancelButton = true;
				//this[i].ProgressBar.Style = progressBarStyleType.Circular;

				//intialize the grid
				this[i].Grid.AlwaysBehind = true;
				this[i].Grid.AutoStep = false;
				this[i].Grid.Step = 1; //1 meter gridlines
				this[i].Grid.MajorLinesEvery = 5; //5 meter major lines
				this[i].Grid.Step = 1;
				this[i].Grid.MajorLinesEvery = 5;
				this[i].Grid.Max.X = 10;
				this[i].Grid.Max.Y = 15;
				this[i].Grid.Min.X = 0;
				this[i].Grid.Min.Y = 0;
				this[i].Grid.AutoSize = true; //this is trash and makes a huge grid
				//this[i].Grid.MajorLineColor = Color.White;

				//hide the rotation cube
				this[i].ViewCubeIcon.Visible = false;

				//hide the origin ball
				this[i].OriginSymbol.BallSize = 0;
				//this[i].OriginSymbol.Visible = false;

				//set user defined colors
				this[i].Background.TopColor = Colors["Background", this[i].Background.TopColor];
				this[i].Grid.MajorLineColor = Colors["GridLines"];
				this[i].SelectionColor = Colors["Selection", this[i].SelectionColor];
				//enable parallel processing for entity regen
				this[i].Entities.Parallel = true;

				//turn down reflections
				this[i].DefaultMaterial.Shininess = 0.1f;
				this[i].DefaultMaterial.Environment = 0f;
				this[i].DefaultMaterial.Specular = Color.Gray;
				//this[i].PlanarShadowOpacity = 1.0;

				this[i].SelectionChanged += DualView_SelectionChanged;
				this[i].MouseMove += DualView_MouseMove;

				this[i].Groups.Add(new List<int>());

			}

			CreateContextMenu();
			splitContainer1.SplitterDistance = (int)((splitContainer1.ClientRectangle.Width - splitContainer1.SplitterWidth) / 2.0);
		}

		object prevMousedOverObj = null;
		System.Drawing.Point mousePnt = System.Drawing.Point.Empty;
		void DualView_MouseMove(object sender, MouseEventArgs e)
		{
			mousePnt = e.Location;
			if (ActiveView.ActionMode == actionType.SelectVisibleByPick)
			{
				int underMouse = ActiveView.GetEntityUnderMouseCursor(mousePnt);

				if (-1 < underMouse && underMouse < ActiveView.Entities.Count)
				{
					if (prevMousedOverObj != null)
						UnHighLight(prevMousedOverObj);

					if (ActiveView.Entities[underMouse].EntityData is YarnGroup)
						return;

					Highlight(ActiveView.Entities[underMouse].EntityData);
					ActiveView.Refresh();
					prevMousedOverObj = ActiveView.Entities[underMouse].EntityData;
				}
				else if (prevMousedOverObj != null)
				{
					UnHighLight(prevMousedOverObj);
					prevMousedOverObj = null;
				}

			}
		}
		
		/// <summary>
		/// highlight an object in the view from mouseover
		/// </summary>
		/// <param name="tag"></param>
		private void Highlight(object tag)
		{
			//if (EditMode)
			//{
				foreach (Entity e in ActiveView.Entities)
				{
					if (e.EntityData == tag)
					{
						e.LineWeightMethod = colorMethodType.byEntity;
						e.LineWeight = 2.0f;
						e.ColorMethod = colorMethodType.byEntity;
						e.Color = Color.FromArgb(255, ActiveView.Layers[e.LayerIndex].Color);
					//	break;
					}
				}


				foreach (devDept.Eyeshot.Labels.Label e in ActiveView.Labels)
				{
					if (e is OutlinedText)
					{
						if ((e as OutlinedText).Text.Contains(tag.ToString()))
						{
							e.Visible = true;

							//break;
						}
					}
				}

			//}
		}
		private void UnHighLight(object tag)
		{
			int entIndex = -1;
			foreach (Entity e in ActiveView.Entities)
			{
				if (e.EntityData == tag)
				{
					e.LineWeightMethod = colorMethodType.byLayer;
					e.ColorMethod = colorMethodType.byLayer;
					entIndex = ActiveView.Entities.IndexOf(e);
					//break;
				}
			}

			foreach (devDept.Eyeshot.Labels.Label e in ActiveView.Labels)
			{
				if (e is OutlinedText)
				{
					if ((e as OutlinedText).Text.Contains(tag.ToString()))
					{
						e.Visible = ActiveView.Entities[entIndex].Selected;

						//break;
					}
				}
			}

		}

		public int ActiveViewIndex
		{
			get { return ActiveView == m_viewright ? 1 : 0; }
		}
		public SingleViewportLayout this[int index]
		{
			get { return index > 0 ? m_viewright : m_viewleft; }
		}
		public SingleViewportLayout ActiveView
		{
			get
			{
				if (m_viewright.Focused)
					return m_viewright;
				else //if(m_viewleft.Focused)
					return m_viewleft;
				//return null;
			}
		}
		public SingleViewportLayout InActiveView
		{
			get
			{
				if (m_viewleft.Focused)
					return m_viewright;
				else if (m_viewright.Focused)
					return m_viewleft;
				return null;
			}
		}
		public SingleViewportLayout OtherView(SingleViewportLayout view)
		{
			return view == m_viewleft ? m_viewright : view == m_viewright ? m_viewleft : null;
		}

		public void Select(object tag)
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (Entity e in this[i].Entities)
				{
					if (e.EntityData == tag)
					{
						e.Selected = true;
						SelectLayer(e.LayerIndex);
						break;
					}
				}
				foreach (devDept.Eyeshot.Labels.Label e in this[i].Labels)
				{
					if (e is OutlinedText)
					{
						if ((e as OutlinedText).Text.Contains(tag.ToString()))
						{
							e.Visible = true;
							break;
						}
					}
				}
			}

		}

		public void DeSelectAll()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (Entity e in this[i].Entities)
					e.Selected = false;
				foreach (devDept.Eyeshot.Labels.Label e in this[i].Labels)
					e.Visible = false;
			}
		}
		public void DeSelect(object tag)
		{
			for (int i = 0; i < 2; i++)
				foreach (Entity e in this[i].Entities)
					if (e.EntityData == tag)
						e.Selected = false;
		}
		public object SelectedTag
		{
			get
			{
				Entity e = ActiveView.Entities.FirstOrDefault(en => en.Selected);
				return e != null ? e.EntityData : null;
			}
			//set
			//{
			//	for (int i = 0; i < 2; i++)
			//		foreach (Entity e in this[i].Entities)
			//			if (e.EntityData == value)
			//				e.Selected = true;
			//}
		}
		public void Remove(object tag)
		{
			for (int i = 0; i < 2; i++)
			{
				List<Entity> ents = new List<Entity>();
				List<devDept.Eyeshot.Labels.Label> labels = new List<devDept.Eyeshot.Labels.Label>();
				foreach (Entity e in this[i].Entities)
					if (e.EntityData == tag)
						ents.Add(e);

				foreach (devDept.Eyeshot.Labels.Label e in this[i].Labels)
					if (e is OutlinedText)
						if ((e as OutlinedText).Text.Contains(tag.ToString()))
							labels.Add(e);

				for (int j = 0; j < ents.Count(); j++)
					if (!this[i].Entities.Remove(ents[j]))
						ents[j].Color = Color.CadetBlue;

				for (int j = 0; j < labels.Count; j++)
					if (!this[i].Labels.Remove(labels[j]))
						labels[j].Color = Color.CadetBlue;

				//this[i].Refresh();
			}
			//Regen();
			//Refresh();
		}

		Vector3D CameraDirection
		{
			get
			{
				Vector3D dir = new Vector3D(ActiveView.Camera.Location, ActiveView.Camera.Target);
				dir.Normalize();
				return dir;
			}
		}

		public EventHandler<EventArgs<IRebuild>> SelectionChanged;

		void DualView_SelectionChanged(object sender, EventArgs e)
		{
			int underMouse = ActiveView.GetEntityUnderMouseCursor(mousePnt);

			if (underMouse > -1)
			{
				if (SelectionChanged != null)
					SelectionChanged(this, new EventArgs<IRebuild>(ActiveView.Entities[underMouse].EntityData as IRebuild));
			}
		}

		public void AttachTracker(ITracker tracker)
		{
			SelectionChanged += tracker.OnSelect;

			for (int i = 0; i < 2; i++)
			{
				//this[i].SelectionChanged += tracker.OnSelect;
				this[i].MouseClick += tracker.OnClick;
				this[i].MouseDown += tracker.OnDown;
				this[i].MouseMove += tracker.OnMove;
				this[i].MouseUp += tracker.OnUp;
			}
		}
		public void DetachTracker(ITracker tracker)
		{
			SelectionChanged -= tracker.OnSelect;
			for (int i = 0; i < 2; i++)
			{
				//this[i].SelectionChanged -= tracker.OnSelect;
				this[i].MouseClick -= tracker.OnClick;
				this[i].MouseDown -= tracker.OnDown;
				this[i].MouseMove -= tracker.OnMove;
				this[i].MouseUp -= tracker.OnUp;
				//StopSelect();
			}
		}

		public void StartSelect()
		{
			this[0].ActionMode = actionType.SelectByPick;
			this[1].ActionMode = actionType.SelectByPick;
		}
		public void StopSelect() { StopSelect(actionType.None); }
		public void StopSelect(actionType restore)
		{
			this[0].ActionMode = restore;
			this[1].ActionMode = restore;
		}

		internal void SetActionMode(actionType actionType)
		{
			ActiveView.ActionMode = actionType;
			//this[1].ActionMode = actionType;
		}

		#region Add Functions (Layers & Entities)

		/// <summary>
		/// Add Layer to the views
		/// </summary>
		/// <param name="layer">layer label</param>
		/// <param name="c">Color</param>
		/// <param name="visible">visibility value</param>
		/// <returns>index of new layer</returns>
		public int AddLayer(string layer, Color c, bool visible)
		{
			int n1, n2;

			Layer lay = ActiveView.Layers.FirstOrDefault(ly => ly.Name == layer);

			if (!this[0].Layers.Contains(lay) && !this[1].Layers.Contains(lay))
			{
				n1 = this[0].Layers.Add(new Layer(layer, Colors[layer], visible));
				n2 = this[1].Layers.Add(new Layer(layer, Colors[layer], visible));
			}
			else
				return this[0].Layers.IndexOf(lay);

			return n1 == n2 ? n1 : -1;
		}

		/// <summary>
		/// Add a single entity
		/// </summary>
		/// <param name="e">the entity to add</param>
		/// <returns>the 2 entity clones actually used in the view</returns>
		public Entity[] Add(Entity e)
		{
			return Add(e, null);
		}
		/// <summary>
		/// Add a single entity
		/// </summary>
		/// <param name="e">the entity</param>
		/// <param name="label">optional label, null if none</param>
		/// <returns>the 2 entity clones actually used in the view</returns>
		public Entity[] Add(Entity e, devDept.Eyeshot.Labels.Label[] labels)
		{
			Entity[] ents = new Entity[2];
			m_viewleft.Entities.Add(ents[0] = (Entity)e.Clone());
			m_viewright.Entities.Add(ents[1] = (Entity)e.Clone());
			ents[0].EntityData = ents[1].EntityData = e.EntityData;
			ents[0].GroupIndex = ents[1].GroupIndex = e.GroupIndex;

			if (e is Mesh)
			{
				e.ColorMethod = colorMethodType.byEntity;
				Color baseColor = e.Color;
				if (e.LayerIndex > 0 && e.LayerIndex < this[0].Layers.Count)
					baseColor = this[0].Layers[e.LayerIndex].Color;
				e.Color = Color.FromArgb(50, baseColor);
			}
			if (labels != null)
			{
				foreach (devDept.Eyeshot.Labels.Label label in labels)
				{
					label.Visible = false;
					m_viewleft.Labels.Add(label);
					m_viewright.Labels.Add(label);
				}
			}

			return ents;
		}
		public Entity[][] AddRange(IEnumerable<Entity> e)
		{
			Entity[][] ents = new Entity[e.Count()][];
			int i = 0;
			foreach (Entity ent in e)
			{
				ents[i++] = Add(ent, null);
			}
			return ents;
		}
		public Entity[][] AddRange(IEnumerable<Entity> e, devDept.Eyeshot.Labels.Label[] label)
		{
			Entity[][] ents = new Entity[e.Count()][];
			int i = 0;
			foreach (Entity ent in e)
			{
				ents[i++] = Add(ent, label);
			}
			return ents;
		}
		public void ZoomFit()
		{
			ZoomFit(false);
		}
		public void ZoomFit(bool bZ)
		{
			if (bZ)
			{
				this[0].SetView(viewType.Top);
				this[1].SetView(viewType.Top);
			}
			for (int i = 0; i < 2; i++)
			{
				this[i].ZoomFit();
				this[i].ZoomOut(50);
				this[i].Grid.Max = this[i].BoundingBox.Max;
				this[i].Grid.Min = this[i].BoundingBox.Min;
			}
			Refresh();
		}

		public void Add(IRebuild g)
		{
			Entity[] groupEntities = g.CreateEntities();
			devDept.Eyeshot.Labels.Label[] groupLabels = g.EntityLabel;

			if (groupEntities == null)
				return;

			int layerIndex = AddLayer(g.Label, Color.Black, true);

			for (int i = 0; i < groupEntities.Length; i++)
			{
				if (groupEntities[i].LayerIndex == 0)
					groupEntities[i].LayerIndex = layerIndex;
				Add(groupEntities[i], groupLabels);
			}
		}
		//public void Add(IGroup g)
		//{
		//	Entity[] groupEntities = g.CreateEntities();
		//	devDept.Eyeshot.Labels.Label[] groupLabels = g.EntityLabel;

		//	if (groupEntities == null)
		//		return;

		//	int layerIndex = AddLayer(g.Label, true);

		//	for (int i = 0; i < groupEntities.Length; i++)
		//	{
		//		if( groupEntities[i].LayerIndex == 0 )
		//			groupEntities[i].LayerIndex = layerIndex;
		//		Add(groupEntities[i], groupLabels);
		//	}
		//}

		public Entity[][] Add(MouldCurve curve)
		{
			IRebuild g = curve.Group;
			int layerIndex = AddLayer(g.Label, Color.Black, true);
			List<Entity> es = curve.CreateEntities().ToList();
			devDept.Eyeshot.Labels.Label[] labels = curve.EntityLabel;

			foreach (Entity e in es)
				e.LayerIndex = layerIndex;
			if (labels == null)
				return AddRange(es);
			return AddRange(es, labels);
		}

		#endregion Add Functions (Layers & Entities)

		#region Coloring

		ColorMap m_colors = new ColorMap();
		public ColorMap Colors
		{
			get { return m_colors; }
		}

		private void PopulateColors()
		{
			string path = System.IO.Path.Combine(Utilities.ExeDir, "colors.txt");

			if (System.IO.File.Exists(path))
			{
				Warps.Logger.logger.Instance.Log("loading color.txt file from " + Utilities.ExeDir);
				Colors.ReadIniFile(path);
			}
			else
			{
				Warps.Logger.logger.Instance.Log("No color.txt file found at " + Utilities.ExeDir);
			}
		}

		private void SaveColors()
		{
			//string path = Colors.IniPath;//store existing path for cancel
			//Colors.IniPath = null;//prompt user for new save location
			string path = null;
			if( !Colors.HasIniFile )
				path = System.IO.Path.Combine(Utilities.ExeDir, "colors.txt");
			Colors.WriteIniFile(path);
			//if (!Colors.WriteIniFile(null))
				//Colors.IniPath = path;//retore previous path
		}

		private void saveColorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.DefaultExt = ".txt";
			dlg.AddExtension = true;
			dlg.Filter = "text files (*.txt)|*.txt|All files (*.*)|*.*";
			dlg.InitialDirectory = Utilities.ExeDir;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Colors.WriteIniFile(dlg.FileName);
#if DEBUG
				Utilities.HandleProcess(dlg.FileName, null);
#endif
			}
		}

		#endregion

		#region Context Menu Stuff

		private void CreateContextMenu()
		{
			int camcount = 7;
			int count = 0;
			foreach (int value in System.Enum.GetValues(typeof(viewType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(viewType), value), null, CameraClicked);
				ts.Tag = (viewType)value;
				m_camerastrip.DropDownItems.Add(ts);
				if (count == camcount)
					break;
				count++;
			}

			m_camerastrip.DropDownItems.Add("-");

			//ProjectionMode
			foreach (int value in System.Enum.GetValues(typeof(projectionType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(projectionType), value), null, CameraClicked);
				ts.Tag = (projectionType)value;
				m_camerastrip.DropDownItems.Add(ts);
			}
			m_camerastrip.DropDownItems.Add("-");

			foreach (int value in System.Enum.GetValues(typeof(backgroundStyleType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(backgroundStyleType), value), null, CameraClicked);
				ts.Tag = (backgroundStyleType)value;
				if (ts.Tag.ToString() != "Image")
					m_camerastrip.DropDownItems.Add(ts);
			}
			m_camerastrip.DropDownItems.Add("-");

			foreach (int value in System.Enum.GetValues(typeof(displayType)))
			{
				ToolStripMenuItem ts = new ToolStripMenuItem(Enum.GetName(typeof(displayType), value), null, CameraClicked);
				ts.Tag = (displayType)value;
				m_camerastrip.DropDownItems.Add(ts);
			}

		}

		private void ContextMenu_Opening(object sender, CancelEventArgs e)
		{
			//set checkmarks
			projectionType vt;
			ToolStripMenuItem ts;
			displayType vd;
			backgroundStyleType bs;

			for (int i = 0; i < m_camerastrip.DropDownItems.Count; i++)
			{
				if (m_camerastrip.DropDownItems[i] is ToolStripMenuItem)
				{
					ts = (ToolStripMenuItem)m_camerastrip.DropDownItems[i];
					if (ts.Tag is projectionType)
					{
						vt = (projectionType)ts.Tag;
						ts.Checked = vt == ActiveView.Camera.ProjectionMode;
					}
					if (ts.Tag is displayType)
					{
						vd = (displayType)ts.Tag;
						ts.Checked = vd == ActiveView.DisplayMode;
					}
					else if (ts.Tag is backgroundStyleType)
					{
						bs = (backgroundStyleType)ts.Tag;
						ts.Checked = bs == ActiveView.Background.Style;
					}
				}
			}
			m_layersToolStrip.DropDownItems.Clear();
			foreach (Layer l in ActiveView.Layers)
			{
				ToolStripItem item = m_layersToolStrip.DropDownItems.Add(l.Name, null, LayerClick);
				item.Tag = l;
				(item as ToolStripMenuItem).Checked = l.Visible;
			}
			gridToolStripMenuItem.Checked = ActiveView.Grid.Visible;
		}

		void CameraClicked(object sender, EventArgs e)
		{
			ToolStripItem popup = sender as ToolStripItem;
			object Tag = popup.Tag;
			if (Tag is viewType)
			{
				ActiveView.SetView((viewType)Tag);
				Refresh();
			}
			else if (Tag is projectionType)
			{
				projectionType v = (projectionType)Tag;
				ActiveView.Camera.ProjectionMode = v;
				Refresh();
			}
			else if (Tag is displayType)
			{
				displayType v = (displayType)Tag;
				ActiveView.DisplayMode = v;
				Refresh();
			}
			else if (Tag is backgroundStyleType)
			{
				backgroundStyleType v = (backgroundStyleType)Tag;
				ActiveView.Background.Style = v;
				Refresh();
			}
		}

		void LayerClick(object sender, EventArgs e)
		{
			//ToolStripDropDownItem popup = sender as ToolStripDropDownItem;
			if (sender is ToolStripItem)
			{
				Layer l = (sender as ToolStripItem).Tag as Layer;
				if (l != null)
				{
					l.Visible = !l.Visible;
					Refresh();
				}
			}
		}

		#endregion Context Menu Stuff

		#region View Handlers

		private void m_viewleft_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (sender is SingleViewportLayout)
			{
				var view = sender as SingleViewportLayout;
				if (ModifierKeys != Keys.Control)
					view.SetView(viewType.Top);
				view.ZoomFit();
				view.ZoomOut(50);
			}
		}

		private void m_viewleft_KeyUp(object sender, KeyEventArgs e)
		{
			var view = sender as SingleViewportLayout;
			if (view == null)
				return;
			switch (e.KeyCode)
			{
				case Keys.W:
					switch (view.DisplayMode)
					{
						case displayType.HiddenLines:
							view.DisplayMode = displayType.Wireframe;
							break;
						case displayType.Wireframe:
							view.DisplayMode = displayType.Rendered;
							break;
						case displayType.Rendered:
							view.DisplayMode = displayType.Shaded;
							break;
						case displayType.Shaded:
							view.DisplayMode = displayType.HiddenLines;
							break;
					}
					break;
				case Keys.P:
					view.ActionMode = view.ActionMode != actionType.SelectVisibleByPick ? actionType.SelectVisibleByPick : actionType.None;
					break;

				case Keys.Z:
					if (CameraDirection.Z < -0.95)
					{
						view.SetView(viewType.Bottom);
						view.RotateCamera(360, 0, false);
					}
					else view.SetView(viewType.Top);
					view.ZoomFit();
					break;

				case Keys.X:
					if (CameraDirection.X < -0.95)
					{
						view.SetView(viewType.Left);
						//view.RotateCamera(360, 0, false);
					}
					else view.SetView(viewType.Right);
					view.ZoomFit();
					break;

				case Keys.C:
					if (CameraDirection.Y > 0.95)
					{
						view.SetView(viewType.Rear);
						//view.RotateCamera(360, 0, false);
					}
					else view.SetView(viewType.Front);
					view.ZoomFit();
					break;
				case Keys.E:
					exploded = !exploded;
					ExplodeLayers(exploded);
					break;
				//case Keys.V:
				//	view.SetView(viewType.Left);
				//	view.ZoomFit();
				//	break;

				//case Keys.B:
				//	view.SetView(viewType.Front);
				//	view.ZoomFit();
				//	break;
			}

			view.Refresh();
		}

		bool exploded = false;

		private void ExplodeLayers(bool explode)
		{
			List<int> usedLayerIndices = new List<int>();
			for (int i = 0; i < this[0].Entities.Count; i++)
			{
				if (!usedLayerIndices.Contains(this[0].Entities[i].LayerIndex))
					usedLayerIndices.Add(this[0].Entities[i].LayerIndex);
			}

			usedLayerIndices.Sort();

			double count = 0;
			usedLayerIndices.ForEach(layer =>
			{
				for (int i = 0; i < this[0].Entities.Count; i++)
				{
					if (this[0].Entities[i].LayerIndex == layer)
					{
						this[0].Entities[i].Translate(0, 0, count);
						this[1].Entities[i].Translate(0, 0, count);
					}

				}

				count = explode ? count + 1 : count - 1;
			});

			Regen();
		}

		private void m_expandBtn_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			if (b == null)
				return;
			switch (b.Text)
			{
				case ">>":
					if (splitContainer1.Panel1Collapsed)
						splitContainer1.Panel1Collapsed = false;
					else
						splitContainer1.Panel2Collapsed = true;
					break;
				case "<<":
					if (splitContainer1.Panel2Collapsed)
						splitContainer1.Panel2Collapsed = false;
					else
						splitContainer1.Panel1Collapsed = true;
					break;
			}
		}

		private void LtoR_Click(object sender, EventArgs e)
		{
			m_viewright.Camera = (Camera)m_viewleft.Camera.Clone();
			m_viewright.Refresh();
		}
		private void RtoL_Click(object sender, EventArgs e)
		{
			m_viewleft.Camera = (Camera)m_viewright.Camera.Clone();
			m_viewleft.Refresh();
		}

		#endregion View Handlers

		#region Eyeshot Toolbars

		private void DualView_Load(object sender, EventArgs e)
		{
			this[0].ToolBar.Visible = false;
			this[1].ToolBar.Visible = false;
		}
		private void focus_Enter(object sender, EventArgs e)
		{
			if (sender is SingleViewportLayout)
			{
				var active = sender as SingleViewportLayout;
				active.ToolBar.Visible = true;
				active.ViewCubeIcon.Visible = true;
				OtherView(active).ToolBar.Visible = false;
				OtherView(active).ViewCubeIcon.Visible = false;
			}
		}
		private void focus_Leave(object sender, EventArgs e)
		{
			this[0].ToolBar.Visible = false;
			this[0].ViewCubeIcon.Visible = false;
			this[1].ToolBar.Visible = false;
			this[1].ViewCubeIcon.Visible = false;
		}


		#endregion

		public void Regen()
		{
			this[0].Entities.Regen();
			this[1].Entities.Regen();
			Refresh();
		}

		internal void HideLayer(IRebuild p)
		{
			Entity e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == p);

			if (e == null)
				return;

			this[0].Layers.TurnOff(e.LayerIndex);// = !this[0].Layers[e.LayerIndex].Visible;
			this[1].Layers.TurnOff(e.LayerIndex);// = !this[1].Layers[e.LayerIndex].Visible;

			Refresh();
		}

		//void ToggleAll(bool show)
		//{
		//	for (int i = 0; i < 2; i++)
		//		for (int j = 0; j < this[i].Entities.Count; j++)
		//			this[i].Entities[j].Visible = show;// = colorMethodType.byLayer;
		//}

		internal void ShowAll()
		{
			for (int i = 0; i < 2; i++)
				this[i].Layers.TurnAllOn();

			Refresh();
		}

		internal void HideAll()
		{
			for (int i = 0; i < 2; i++)
				this[i].Layers.TurnAllOff();
		}

		internal void ShowOnly(IRebuild obj)
		{
			HideAll();
			ToggleLayer(obj);
		}

		internal void ToggleLayer(IRebuild p)
		{
			Entity e;

			if (p is IMouldCurve)
			{
				e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == p);

				if (e == null)
					return;

				this[0].Layers[e.LayerIndex].Visible = !this[0].Layers[e.LayerIndex].Visible;
				this[1].Layers[e.LayerIndex].Visible = !this[1].Layers[e.LayerIndex].Visible;
			}
			else if (p is CurveGroup)
			{
				foreach (IRebuild ir in (p as CurveGroup))
				{
					e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == ir);

					if (e == null)
						return;

					this[0].Layers[e.LayerIndex].Visible = !this[0].Layers[e.LayerIndex].Visible;
					this[1].Layers[e.LayerIndex].Visible = !this[1].Layers[e.LayerIndex].Visible;
					break;
				}
			}
			else
			{
				e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == p);

				if (e == null)
					return;

				this[0].Layers[e.LayerIndex].Visible = !this[0].Layers[e.LayerIndex].Visible;
				this[1].Layers[e.LayerIndex].Visible = !this[1].Layers[e.LayerIndex].Visible;
			}

			Refresh();
		}

		internal void ToggleLayer(Sail sail)
		{
			this[0].Layers[1].Visible = !this[0].Layers[1].Visible;
			this[1].Layers[1].Visible = !this[1].Layers[1].Visible;

			Refresh();
		}

		internal void Invalidate(IRebuild item)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < this[i].Entities.Count; j++)
				{
					if (this[i].Entities[j].EntityData == item)
					{
						this[i].Entities[j].ColorMethod = colorMethodType.byEntity;
						this[i].Entities[j].Color = InvalidColor;
					}
				}
			}
		}

		Color InvalidColor = Color.Red;

		internal bool ToggleArrows(int nView)
		{
			int l, h;
			if (nView == -1)
			{
				l = 0;
				h = 2;
			}
			else
			{
				l = nView;
				h = nView + 1;
			}
			for (int i = l; i < h; i++)
			{
				this[i].ShowCurveDirection = !this[i].ShowCurveDirection;
				this[i].Refresh();
			}
			return this[0].ShowCurveDirection;
		}

		int opacityLevel = 25;

		internal void SelectLayer(IRebuild curve)
		{
			Entity e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == curve);

			if (e == null)
				return;

			SelectLayer(e.LayerIndex);
		}

		internal void DeSelectAllLayers()
		{
			for (int i = 0; i < ActiveView.Layers.Count; i++)
			{
				this[0].Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, this[0].Layers[i].Color);
				this[1].Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, this[1].Layers[i].Color);
			}
		}

		internal void SelectLayer(int p)
		{
			for (int i = 0; i < ActiveView.Layers.Count; i++)
				if (i != p)
				{
					m_viewleft.Layers[i].Color = Color.FromArgb(EditMode ? 50 : 255, ActiveView.Layers[i].Color);
					m_viewright.Layers[i].Color = Color.FromArgb(EditMode ? 50 : 255, ActiveView.Layers[i].Color);
				}
			//ActiveView.Layers[i].Color = Color.FromArgb(EditMode ? 50 : 255, ActiveView.Layers[i].Color);

		}

		//private void toolStripMenuItem1_Click(object sender, EventArgs e)
		//{
		//	Warps.Controls.ColorWheelForm cwf = new Controls.ColorWheelForm(ActiveView);
		//	cwf.Location = MousePosition;
		//	cwf.Show(this);
		//}

		internal Layer GetLayer(IRebuild group)
		{
			return ActiveView.Layers.FirstOrDefault(ly => ly.Name == group.Label);
		}
		internal Layer GetLayer(string name)
		{
			return ActiveView.Layers.FirstOrDefault(ly => ly.Name == name);
		}
		internal Layer GetLayer(ISurface surface)
		{
			return ActiveView.Layers.FirstOrDefault(ly => ly.Name == surface.Label);
		}

		private void toggleArrowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleArrows(ActiveViewIndex);
		}

		//bool m_editMode = false;
		//public bool EditMode { get { return m_editMode; } set { m_editMode = value; Update(); } }
		private void gridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveView.Grid.Visible = !ActiveView.Grid.Visible;

			ActiveView.Refresh();
		}

		private void colorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			KeyValuePair<Form, ColorEditor> p = ColorEditor.Show(Colors);
			p.Key.Owner = this.ParentForm;
			System.Drawing.Point loc = this.PointToScreen(ActiveView.Location);
			loc.X -= p.Key.Width;
			p.Key.Location = loc;
			p.Value.ColorChanged += Value_ColorChanged;
			//Warps.Controls.ColorWheelForm cwf = new Controls.ColorWheelForm(ActiveView);
			//cwf.Location = MousePosition;
			//cwf.Show(this);
		}

		void Value_ColorChanged(object sender, EventArgs<string[], Color> e)
		{
			for (int i = 0; i < 2; i++)
			{
				if (e.ValueT.Contains("Background"))
					this[i].Background.TopColor = e.ValueP;
				if (e.ValueT.Contains("GridLines"))
					this[i].Grid.MajorLineColor = e.ValueP;
				if (e.ValueT.Contains("Selection"))
					this[i].SelectionColor = e.ValueP;
				IEnumerable<Layer> layers = this[i].Layers.Where(ly => e.ValueT.Contains(ly.Name));
				foreach (Layer l in layers)
					l.Color = e.ValueP;
				//Layer l = this[i].Layers.FirstOrDefault(ly => ly.Name == e.ValueT);
				//if (l != null)
				//{
				//	l.Color = e.ValueP;
				//}
				this[i].Refresh();
			}
		}
		private void saveColorsToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			SaveColors();
		}
		private void loadColorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Colors.ReadIniFile(null);

			for (int i = 0; i < 2; i++)
			{
				this[i].Background.TopColor = Colors["Background", this[i].Background.TopColor];
				this[i].Grid.MajorLineColor = Colors["GridLines"];
				this[i].SelectionColor = Colors["Selection", this[i].SelectionColor];
				foreach (Layer L in this[i].Layers)
					L.Color = Colors[L.Name, L.Color];
				this[i].Refresh();
			}
		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; }
		}

		internal void ToggleGroup(int p)
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (int nE in this[i].Groups[p])
					this[i].Entities[nE].Visible = !this[i].Entities[nE].Visible;
			}

		}

		internal void ClearAll()
		{
			for (int i = 0; i < 2; i++)
			{
				this[i].Entities.Clear();
				this[i].Layers.Clear();
				
			}
			Refresh();
		}
	}
}
