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
using Warps.Curves;

namespace Warps
{
	public partial class DualView : UserControl
	{
		public DualView()
		{
			InitializeComponent();
			//LoadColors();

			for (int i = 0; i < 2; i++)
			{
				this[i].Unlock("EYENBS-621M-0505K-8XJMF-P818Q");
				this[i].Rendered.EnvironmentMapping = false;
				this[i].Rendered.PlanarReflections = false;

				//this[i].AnimateCamera = true;

				this[i].PlanarShadowOpacity = 0;
				this[i].Units = unitSystemType.Meters;

				this[i].ProgressBar.Visible = false;
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
				this[i].OriginSymbol.Size = 0;
				//this[i].OriginSymbol.Visible = false;

				////set user defined colors
				//this[i].Background.TopColor = Colors["Background", this[i].Background.TopColor];
				//this[i].Background.BottomColor = Colors["Backgrad", this[i].Background.BottomColor];
				
				//this[i].Grid.MajorLineColor = Colors["GridLines"];
				//this[i].SelectionColor = Colors["Selection", this[i].SelectionColor];

				//this[i].Layers[0].Color = Color.FromArgb(100, Colors["Default", Color.LightSkyBlue]);
				//this[i].Layers[0].LineWeight = 2.0f;

				//enable parallel processing for entity regen
				this[i].Entities.Parallel = true;

				//turn down reflections
				//this[i].DefaultMaterial.Shininess = 0.001f;
				this[i].DefaultMaterial.Environment = 0f;
				this[i].DefaultMaterial.Specular = Color.Gray;
				//this[i].PlanarShadowOpacity = 1.0;

				this[i].SelectionChanged += DualView_SelectionChanged;
				this[i].MouseMove += DualView_MouseMove;

				this[i].Groups.Add(new List<int>());


				this[i].Legends[0].Visible = false;
				this[i].Legends[0].AlignValuesRight = true;
				this[i].Legends[0].Title = "Yarn DPI";
				this[i].Legends[0].Subtitle = "sub";
				this[i].Legends[0].FormatString = "{0:f3}";
				this[i].Legends[0].TitleFont = new Font("Tahoma", 12f);
				this[i].Legends[0].TextFont = new Font("Tahoma", 12f);

				//this[i].Legends = new Legend[] { this[i].Legends[0], new Legend(this[i].Legends[0]) };
				//this[i].Legends[1].Visible = true;
				//this[i].Legends[1].ColorTable = new Color[0];
				//this[i].Legends[1].Min = this[i].Legends[1].Max = 0;

				this[i].Legends[0].ColorTable = ColorMath.CreateColorScale();
				//this[i].Legends[0].ColorTable = Legend.RedToBlue17;
				//this[i].Legends[0].Position = new System.Drawing.Point(0, 0);
				//this[i].Legends[0].SetRange(0, 0);
				//this[i].Legends[0].ItemSize = new Size(15, (int)Math.Ceiling(this[i].Height / 33.0));
				//this[i].Legends[0].ItemSize = new Size(0,0);
				this[i].Legends[0].Slave = false;

			}
			//this[0].ProgressBar.CancellingText = "CANCELELEELS";
			//this[0].ProgressBar.HideCancelButton = false;
			//this[0].ProgressBar.Visible = true;
			CreateContextMenu();
			m_split.SplitterDistance = (int)((m_split.ClientRectangle.Width - m_split.SplitterWidth) / 2.0);
		}

		internal void ReadConfigFile()
		{
			string layers = Utilities.Settings["View/Left/Layers", "Default,Mould"];
			string[] splits = layers.Split(',');
			ShowLayers(0, splits);
			layers = Utilities.Settings["View/Right/Layers", "Default,Mould,Curves,Yarns,Panels,Tapes"];
			splits = layers.Split(',');
			ShowLayers(1, splits);

			LoadColors(Utilities.Settings["ColorTable", null]);
		}

		bool IsConfigVisible(int nView, string layer)
		{
			string layers = Utilities.Settings["View/" + ( nView == 0 ? "Left" : "Right") + "/Layers", ""];
			if (layers == "")
				return true;//default on?
			string[] splits = layers.Split(',');
			return splits.Contains(layer);
		}


		public SplitContainer Splitter
		{
			get { return m_split; }
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
				return OtherView(ActiveView);
			}
		}
		public SingleViewportLayout OtherView(SingleViewportLayout view)
		{
			return view == m_viewleft ? m_viewright : view == m_viewright ? m_viewleft : null;
		}

		public List<string> ActiveLayers(int nView)
		{
			List<string> layers = new List<string>();
			foreach (Layer l in this[nView].Layers)
				if (l.Visible)
					layers.Add(l.Name);
			return layers;
		}
		public void Select(IRebuild tag)
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (Entity e in this[i].Entities)
					if (e.EntityData == tag)
						e.Selected = true;


				if (tag.EntityLabel != null)
				foreach (devDept.Eyeshot.Labels.Label lbl in tag.EntityLabel)
				{
					if (!(lbl is OutlinedText))
						continue;
					foreach (devDept.Eyeshot.Labels.Label e in this[i].Labels)
						if (e is OutlinedText && (e as OutlinedText).Text == (lbl as OutlinedText).Text)
							e.Visible = true;
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
		public void DeSelect(IRebuild tag)
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (Entity e in this[i].Entities)
					if (e.EntityData == tag)
						e.Selected = false;
				
				if( tag.EntityLabel != null )
				foreach(devDept.Eyeshot.Labels.Label lbl in tag.EntityLabel )
				{
					if( !(lbl is OutlinedText ) ) 
						continue;
					foreach (devDept.Eyeshot.Labels.Label e in this[i].Labels)
						if(e is OutlinedText && (e as OutlinedText).Text == (lbl as OutlinedText).Text)
							e.Visible =  false;
				}
			}
		}

		/// <summary>
		/// this method uses FirstOrDefault right now.  That's not a very good solution
		/// </summary>
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

		public void VisibilityToggled(object sender, EventArgs<IRebuild> args)
		{
			ToggleVisibility(args.Value);
			//ToggleLayer(args.Value);
		}

		public void AttachTracker(ITracker tracker)
		{
			//SelectionChanged += tracker.OnSelect;

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
			//SelectionChanged -= tracker.OnSelect;
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

		/// <summary>
		/// allows trackers to setup the view for actions.
		/// I could have done this with an enum but it seemed like more 
		/// work to manage
		/// </summary>
		/// <param name="type">tracker type: "warps", "guides", etc</param>
		internal void SetTrackerSelectionMode(string type)
		{
			if (type == null)
			{
				StopSelect();
				RestoreVisState();
				Refresh();
				return;
			}

			switch (type.ToLower())
			{
				case "warps":

					//hide everything that isn't a warp
					RestoreVisState();
					SaveVisState();

					for (int i = 0; i < ActiveView.Entities.Count; i++)
					{
						if (!(ActiveView.Entities[i].EntityData is IRebuild))
							continue;

						IRebuild irb = ActiveView.Entities[i].EntityData as IRebuild;
						if (!(irb is MouldCurve) || (irb is GuideComb))
							ActiveView.Entities[i].Visible = false;
					}
					StartSelect();
					Refresh();
					break;

				case "guides":
					//hide everything that isn't a guide
					RestoreVisState();
					SaveVisState();

					for (int i = 0; i < ActiveView.Entities.Count; i++)
					{
						if (!(ActiveView.Entities[i].EntityData is IRebuild))
							continue;

						IRebuild irb = ActiveView.Entities[i].EntityData as IRebuild;
						if (!(irb is GuideComb))
							ActiveView.Entities[i].Visible = false;
					}
					StartSelect();
					Refresh();
					break;
				case "guidesurface":
					RestoreVisState();
					SaveVisState();

					for (int i = 0; i < ActiveView.Entities.Count; i++)
					{
						if (!(ActiveView.Entities[i].EntityData is IRebuild))
							continue;

						IRebuild irb = ActiveView.Entities[i].EntityData as IRebuild;
						if (!(irb is GuideSurface))
							ActiveView.Entities[i].Visible = false;
					}
					StartSelect();
					Refresh();
					break;

				default:
					RestoreVisState();
					StopSelect();
					Refresh();
					break;
			}
		}

		public void ShowTypes(params Type[] types)
		{
			if (types == null)
			{
				RestoreVisState();
				StopSelect();
				Refresh();
			}
			//hide everything that isn't a warp
			RestoreVisState();
			SaveVisState();

			for (int i = 0; i < ActiveView.Entities.Count; i++)
			{
				if (!(ActiveView.Entities[i].EntityData is IRebuild))
					continue;

				IRebuild irb = ActiveView.Entities[i].EntityData as IRebuild;
				ActiveView.Entities[i].Visible = types.Contains(irb.GetType());
			}
			StartSelect();
			Refresh();
		}
		Dictionary<IRebuild, bool[]> m_visSelOld = new Dictionary<IRebuild, bool[]>();
		private void SaveVisState()
		{
			m_visSelOld.Clear();
			for (int i = 0; i < ActiveView.Entities.Count; i++)
			{
				if (!(ActiveView.Entities[i].EntityData is IRebuild))
					continue;
				if (!m_visSelOld.ContainsKey(ActiveView.Entities[i].EntityData as IRebuild))
					m_visSelOld.Add(ActiveView.Entities[i].EntityData as IRebuild, new bool[] { ActiveView.Entities[i].Visible, ActiveView.Entities[i].Selected });
			}
		}
		private void RestoreVisState()
		{
			if (m_visSelOld.Count > 0)
			{
				for (int i = 0; i < ActiveView.Entities.Count; i++)
				{
					if (!(ActiveView.Entities[i].EntityData is IRebuild))
						continue;
					if (m_visSelOld.ContainsKey(ActiveView.Entities[i].EntityData as IRebuild))
					{
						ActiveView.Entities[i].Visible = m_visSelOld[ActiveView.Entities[i].EntityData as IRebuild][0];
						ActiveView.Entities[i].Selected = m_visSelOld[ActiveView.Entities[i].EntityData as IRebuild][1];
					}
				}
			}
		}

		#region Add Functions (Layers & Entities)

		/// <summary>
		/// Adds an IRebuild object to the views, optionally adding the labels as well
		/// </summary>
		/// <param name="g">the object to add</param>
		/// <param name="bLabels">true to add labels</param>

		public List<Entity[]> Add(IRebuild g, bool bLabels)
		{
			List<Entity> groupEntities = g.CreateEntities();
			if (groupEntities == null)
				return null;

			List<Entity[]> rets = new List<Entity[]>();

			int defIndex = 0;
			if( g.Layer != null )
				defIndex = AddLayer(g.Layer);

			for (int i = 0; i < groupEntities.Count; i++)
			{
				if (groupEntities[i].LayerIndex == 0)
				{
					if (groupEntities[i].EntityData is IRebuild)//place groups in their custom layers
						groupEntities[i].LayerIndex = AddLayer((groupEntities[i].EntityData as IRebuild).Layer);
					else//default to parent group's layer
						groupEntities[i].LayerIndex = defIndex;
				}

				if (groupEntities[i] is Text)//add text to legend
				{
					this[0].Legends[0].Subtitle = (groupEntities[i] as Text).TextString + "\n";
					this[0].Legends[0].Visible = true;
				}
				else if (groupEntities[i] is devDept.Eyeshot.Entities.Point)//use points as min/max legend
				{
					devDept.Eyeshot.Entities.Point pt = groupEntities[i] as devDept.Eyeshot.Entities.Point;
					this[0].Legends[0].SetRange(pt.EndPoint.X, pt.EndPoint.Y);
					this[0].Legends[0].Visible = true;
				}
				else
					rets.Add(Add(groupEntities[i]));
			}
			if (bLabels)
				AddLabels(g.EntityLabel);

			return rets;
		}

		/// <summary>
		/// Add Layer to the views
		/// </summary>
		/// <param name="layer">layer label</param>
		/// <param name="c">Color</param>
		/// <param name="visible">visibility value</param>
		/// <returns>index of new layer</returns>
		public int AddLayer(string layer)
		{
			int n1, n2;

			Layer lay = ActiveView.Layers.FirstOrDefault(ly => ly.Name == layer);

			if (!this[0].Layers.Contains(lay) && !this[1].Layers.Contains(lay))
			{
				n1 = this[0].Layers.Add(new Layer(layer, Colors[layer], IsConfigVisible(0, layer)));
				n2 = this[1].Layers.Add(new Layer(layer, Colors[layer], IsConfigVisible(1, layer)));
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
			Entity[] ents = new Entity[2];
			m_viewleft.Entities.Add(ents[0] = (Entity)e.Clone());
			m_viewright.Entities.Add(ents[1] = (Entity)e.Clone());
			ents[0].EntityData = ents[1].EntityData = e.EntityData;
			ents[0].GroupIndex = ents[1].GroupIndex = e.GroupIndex;

			//AddLabels(labels);

			return ents;
		}

		public Entity[][] AddRange(IEnumerable<Entity> e)
		{
			Entity[][] ents = new Entity[e.Count()][];
			int i = 0;
			foreach (Entity ent in e)
				ents[i++] = Add(ent);

			return ents;
		}
		private void AddLabels(List<devDept.Eyeshot.Labels.Label> labels)
		{
			if (labels != null)
			{
				foreach (devDept.Eyeshot.Labels.Label label in labels)
				{
					label.Visible = false;
					m_viewleft.Labels.Add(label);
					m_viewright.Labels.Add(label);
				}
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

		//public Entity[][] Add(MouldCurve curve)
		//{
		//	IRebuild g = curve.Group;
		//	int layerIndex = AddLayer(g.Label,true);
		//	List<Entity> es = curve.CreateEntities().ToList();
		//	devDept.Eyeshot.Labels.Label[] labels = curve.EntityLabel;

		//	foreach (Entity e in es)
		//		e.LayerIndex = layerIndex;
		//	if (labels == null)
		//		return AddRange(es);
		//	return AddRange(es, labels);
		//}

		public void Remove(IRebuild tag, bool bLabels)
		{
			for (int i = 0; i < 2; i++)
			{
				List<Entity> ents = new List<Entity>();
				List<devDept.Eyeshot.Labels.Label> labels = new List<devDept.Eyeshot.Labels.Label>();
				foreach (Entity e in this[i].Entities)
					if (e.EntityData == tag)
						ents.Add(e);
				if (bLabels)
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

		public void RemoveRange(IEnumerable<Entity[]> temps)
		{
			int i;
			foreach (Entity[] temp in temps)
			{
				if( temp != null )
				for (i = 0; i < 2; i++)
					this[i].Entities.Remove(temp[i]);
			}
		}


		#endregion Add Functions (Layers & Entities)

		#region Coloring

		ColorMap m_colors = new ColorMap();
		public ColorMap Colors
		{
			get { return m_colors; }
		}

		public void LoadColors(string path)
		{
			if( path == null )
				path = System.IO.Path.Combine(Utilities.ExeDir, "colors.txt");

			if (System.IO.File.Exists(path))
			{
				Logleton.TheLog.Log("loading color.txt file from " + Utilities.ExeDir);
				Colors.ReadIniFile(path);
				for (int i = 0; i < 2; i++)
				{
					//set user defined colors
					this[i].Background.TopColor = Colors["Background", this[i].Background.TopColor];
					this[i].Background.BottomColor = Colors["Backgrad", this[i].Background.BottomColor];

					this[i].Grid.MajorLineColor = Colors["GridLines"];
					this[i].SelectionColor = Colors["Selection", this[i].SelectionColor];

					this[i].Layers[0].Color = Color.FromArgb(100, Colors["Default", Color.LightSkyBlue]);
					this[i].Layers[0].LineWeight = 2.0f;
					for (int nL = 1; nL < this[i].Layers.Count; nL++)
						this[i].Layers[nL].Color = Colors[this[i].Layers[nL].Name];
				}
			}
			else
			{
				Logleton.TheLog.Log("No color.txt file found at " + Utilities.ExeDir);
			}

		}

		public string SaveColors()
		{
			//string path = Colors.IniPath;//store existing path for cancel
			//Colors.IniPath = null;//prompt user for new save location
			string path = null;
			if (!Colors.HasIniFile)
				path = System.IO.Path.Combine(Utilities.ExeDir, "colors.txt");
			return Colors.WriteIniFile(path);
			//if (!Colors.WriteIniFile(null))
			//Colors.IniPath = path;//retore previous path
		}

		public void SetColorScale(Mesh m)
		{
			ISurface s = m.EntityData as ISurface;
			if (s == null || s.ColorValues == null)
				return;
			double ave, max, min, stddev = BLAS.StandardDeviation(s.ColorValues, out ave, out max, out min);
			Color[] scale = ColorMath.CreateColorScale();
			for (int i = 0; i < 2; i++)
			{
				this[i].Legends[0].ColorTable = scale;
				this[i].Legends[0].Max = ave + 2 * stddev;
				this[i].Legends[0].Min = ave - 2 * stddev;
				this[i].Legends[0].FormatString = max < .001 ? "{0:e4}" : max < .01 ? "{0:f5}" : max < .1 ? "{0:f4}" : max < 1 ? "{0:f3}" : "{0:f2}";
				this[i].Legends[0].ItemSize = new Size(15, (int)Math.Floor((double)this[i].Height / (double)(scale.Length + 1)));
				this[i].Legends[0].Position = new System.Drawing.Point(0, -this[i].Legends[0].ItemSize.Height);
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
		private void toggleGridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveView.Grid.Visible = !ActiveView.Grid.Visible;
			ActiveView.Refresh();
		}
		private void toggleArrowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleArrows(ActiveViewIndex);
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
					if (l.Name == "Gauss")
						ActiveView.Legends[0].Visible = l.Visible;				
					Refresh();
				}
			}
		}
		private void showAllToolstrip_Click(object sender, EventArgs e)
		{
			ShowAll();
			Refresh();
		}
		private void hideAllToolstrip_Click(object sender, EventArgs e)
		{
			HideAll();
			Refresh();
		}

		void Value_ColorChanged(object sender, EventArgs<string[], Color> e)
		{
			for (int i = 0; i < 2; i++)
			{
				if (e.ValueT.Contains("Background"))
					this[i].Background.TopColor = e.ValueP;
				if (e.ValueT.Contains("Backgrad"))
					this[i].Background.BottomColor = e.ValueP;
				if (e.ValueT.Contains("GridLines"))
					this[i].Grid.MajorLineColor = e.ValueP;
				if (e.ValueT.Contains("Selection"))
					this[i].SelectionColor = e.ValueP;
				IEnumerable<Layer> layers = this[i].Layers.Where(ly => e.ValueT.Contains(ly.Name));
				foreach (Layer l in layers)
					l.Color = e.ValueP;
				if (e.ValueT.Contains("Default"))
					this[i].Layers[0].Color = Color.FromArgb(100, e.ValueP);//keep default layer's transparency

				//Layer l = this[i].Layers.FirstOrDefault(ly => ly.Name == e.ValueT);
				//if (l != null)
				//{
				//	l.Color = e.ValueP;
				//}
				this[i].Refresh();
			}
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
		private void saveColorsToolStripMenuItem_Click(object sender, EventArgs e)
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
				view.ZoomOut(20);
				//ZoomFit(true);
				//view.Legends[0].Position = new System.Drawing.Point(0, -10);
				//view.Legends[0].ItemSize = new Size(15, (int)(view.Height * .08));

			}
		}

		private void m_viewleft_KeyUp(object sender, KeyEventArgs e)
		{
			var view = sender as SingleViewportLayout;
			if (view == null || ModifierKeys == Keys.Control)
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
							view.DisplayMode = displayType.Wireframe;//skip hidden lines mode
							break;
					}
					break;
				case Keys.P:
					view.ActionMode = view.ActionMode != actionType.SelectByPick ? actionType.SelectByPick : actionType.None;
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
					if (m_split.Panel1Collapsed)
						m_split.Panel1Collapsed = false;
					else
						m_split.Panel2Collapsed = true;
					break;
				case "<<":
					if (m_split.Panel2Collapsed)
						m_split.Panel2Collapsed = false;
					else
						m_split.Panel1Collapsed = true;
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

		#region MouseOverHighlighting

		IRebuild m_mousePrev = null;
		bool m_prevSel = false;
		void DualView_MouseMove(object sender, MouseEventArgs e)
		{
			if (sender != ActiveView || ActiveView.ActionMode != actionType.SelectByPick)
				return;

			int underMouse = ActiveView.GetEntityUnderMouseCursor(e.Location);

			if (-1 < underMouse && underMouse < ActiveView.Entities.Count)
			{
				if (m_mousePrev != null)
					Highlight(m_mousePrev, m_prevSel);

				m_mousePrev = ActiveView.Entities[underMouse].EntityData as IRebuild;
				m_prevSel = ActiveView.Entities[underMouse].Selected;
				Highlight(ActiveView.Entities[underMouse].EntityData as IRebuild, true);
				ActiveView.Refresh();
			}
			else if (m_mousePrev != null)
			{
				Highlight(m_mousePrev, m_prevSel);
				m_mousePrev = null;
			}
		}
		void DualView_SelectionChanged(object sender, EventArgs e)
		{
			int underMouse = ActiveView.GetEntityUnderMouseCursor( ActiveView.PointToClient(MousePosition));

			if (underMouse > -1)
			{
				if (SelectionChanged != null)
					SelectionChanged(this, new EventArgs<IRebuild>(ActiveView.Entities[underMouse].EntityData as IRebuild));
			}
		}
		/// <summary>
		/// highlight an object in the view from mouseover
		/// </summary>
		/// <param name="tag"></param>
		private void Highlight(IRebuild tag, bool bHighLight)
		{
			if (tag == null)
				return;
			foreach (Entity e in ActiveView.Entities)
				if (e.EntityData == tag)
					e.Selected = bHighLight;

			foreach (devDept.Eyeshot.Labels.Label lbl in ActiveView.Labels)
			{
				if (lbl is OutlinedText)
					if ((lbl as OutlinedText).Text.Contains(tag.ToString()))
						lbl.Visible = bHighLight;
			}
		}
		//void DualView_MouseMove(object sender, MouseEventArgs e)
		//{
		//	mousePnt = e.Location;
		//	if (ActiveView.ActionMode == actionType.SelectVisibleByPick)
		//	{
		//		int underMouse = ActiveView.GetEntityUnderMouseCursor(mousePnt);

		//		if (-1 < underMouse && underMouse < ActiveView.Entities.Count)
		//		{
		//			if (prevMousedOverObj != null)
		//				UnHighLight(prevMousedOverObj);

		//			if (ActiveView.Entities[underMouse].EntityData is YarnGroup)
		//				return;

		//			Highlight(ActiveView.Entities[underMouse].EntityData);
		//			ActiveView.Refresh();
		//			prevMousedOverObj = ActiveView.Entities[underMouse].EntityData;
		//		}
		//		else if (prevMousedOverObj != null)
		//		{
		//			UnHighLight(prevMousedOverObj);
		//			prevMousedOverObj = null;
		//		}

		//	}
		//}

		//private void Highlight(object tag)
		//{
		//	if (tag == null)
		//		return;
		//	foreach (Entity e in ActiveView.Entities)
		//	{
		//		if (e.EntityData == tag && !e.Selected)
		//		{
		//			if (e.EntityData == tag)
		//			{
		//				if (!(e is PointCloud))
		//				{
		//					e.LineWeightMethod = colorMethodType.byEntity;
		//					e.LineWeight = 2.0f;
		//				}
		//				e.ColorMethod = colorMethodType.byEntity;
		//				e.Color = Color.FromArgb(255, ActiveView.Layers[e.LayerIndex].Color);
		//			//	break;
		//			}
		//		}
		//	}


		//	foreach (devDept.Eyeshot.Labels.Label e in ActiveView.Labels)
		//	{
		//		if (e is OutlinedText)
		//		{
		//			if ((e as OutlinedText).Text.Contains(tag.ToString()))
		//			{
		//				if (!e.Selected)
		//					e.Visible = true;
		//			}
		//		}
		//	}
		//}
		//private void UnHighLight(object tag)
		//{
		//	int entIndex = -1;
		//	foreach (Entity e in ActiveView.Entities)
		//	{
		//		if (e.EntityData == tag && !e.Selected)
		//		{
		//			if (!(e is PointCloud))
		//			{
		//				e.LineWeightMethod = colorMethodType.byLayer;
		//				e.ColorMethod = colorMethodType.byLayer;
		//			}
		//			entIndex = ActiveView.Entities.IndexOf(e);
		//		}
		//	}

		//	if (entIndex == -1)
		//		return;
		//	foreach (devDept.Eyeshot.Labels.Label e in ActiveView.Labels)
		//	{
		//		if (e is OutlinedText)
		//		{
		//			if ((e as OutlinedText).Text.Contains(tag.ToString()))
		//			{
		//				e.Visible = ActiveView.Entities[entIndex].Selected;
		//			}
		//		}
		//	}

		//}

		#endregion

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
			for (int i = 0; i < 1; i++)
			{
				this[i].ZoomFit();
				this[i].ZoomOut(20);
				this[i].Grid.Max = this[i].BoundingBox.Max;
				this[i].Grid.Min = this[i].BoundingBox.Min;
			}
			LtoR_Click(null, null);//match zooms
			Refresh();
		}

		//internal void HideLayer(IRebuild p)
		//{
		//	Entity e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == p);

		//	if (e == null)
		//		return;

		//	this[0].Layers.TurnOff(e.LayerIndex);// = !this[0].Layers[e.LayerIndex].Visible;
		//	this[1].Layers.TurnOff(e.LayerIndex);// = !this[1].Layers[e.LayerIndex].Visible;

		//	Refresh();
		//}

		internal void ShowAll()
		{
			ActiveView.Layers.TurnAllOn();
			//for (int i = 0; i < 2; i++)
			//	this[i].Layers.TurnAllOn();

		}

		internal void HideAll()
		{
			ActiveView.Layers.TurnAllOff();
			//for (int i = 0; i < 2; i++)
			//	this[i].Layers.TurnAllOff();
		}

		internal void ShowOnly(IRebuild obj)
		{
			HideAll();
			ToggleVisibility(obj);
			//ToggleLayer(obj);
		}

		public void ShowLayers(int nView, params string[] layers)
		{
			this[nView % 2].Layers.TurnAllOff();
			foreach(string s in layers)
			{
					Layer l = this[nView % 2].Layers.FirstOrDefault(ly => ly.Name == s);
					if( l != null )
						l.Visible = true;
				}
		}

		internal void ToggleLayer(IRebuild p)
		{
			Entity e;
			e = ActiveView.Entities.FirstOrDefault(ent => ent.EntityData == p);

			if (e == null)
				return;

			this[0].Layers[e.LayerIndex].Visible = !this[0].Layers[e.LayerIndex].Visible;
			this[1].Layers[e.LayerIndex].Visible = !this[1].Layers[e.LayerIndex].Visible;

			Refresh();
		}

		public void ToggleVisibility(IRebuild rbd)
		{
			List<Entity> ents = rbd.CreateEntities();
			List<object> eds = new List<object>();
			ents.ForEach(e =>
			{
				if (!eds.Contains(e.EntityData))
					eds.Add(e.EntityData);
			});

			for (int i = 0; i < 2; i++)
			{
				foreach (Entity e in this[i].Entities)
					if (eds.Contains(e.EntityData))
						e.Visible = !e.Visible;
			}
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

		int opacityLevel = 100;

		internal void DeSelectAllLayers()
		{
			for (int i = 1; i < ActiveView.Layers.Count; i++)
			{
				this[0].Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, this[0].Layers[i].Color);
				this[1].Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, this[1].Layers[i].Color);
			}
		}

		internal void SelectLayer(IGroup grp)
		{
			Layer l = GetLayer(grp);

			if (l == null)
				return;

			SelectLayer(ActiveView.Layers.IndexOf(l));
		}
		internal void SelectLayer(int p)
		{
			for (int i = 0; i < ActiveView.Layers.Count; i++)
				if (i != p)
				{
					m_viewleft.Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, ActiveView.Layers[i].Color);
					m_viewright.Layers[i].Color = Color.FromArgb(EditMode ? opacityLevel : 255, ActiveView.Layers[i].Color);
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
			return ActiveView.Layers.FirstOrDefault(ly => ly.Name == group.Layer);
		}
		internal Layer GetLayer(string name)
		{
			return ActiveView.Layers.FirstOrDefault(ly => ly.Name == name);
		}

		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; }
		}

		//internal void ToggleGroup(int p)
		//{
		//	for (int i = 0; i < 2; i++)
		//	{
		//		foreach (int nE in this[i].Groups[p])
		//			this[i].Entities[nE].Visible = !this[i].Entities[nE].Visible;
		//	}
		//}

		internal void ClearAll()
		{
			for (int i = 0; i < 2; i++)
			{
				this[i].Entities.Clear();
				this[i].Layers.Clear();
			}

			Refresh();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{

			SaveFileDialog mySaveFileDialog = new SaveFileDialog();

			mySaveFileDialog.InitialDirectory = ".";
			mySaveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|" +
				"Portable Network Graphics (*.png)|*.png|" +
				"Windows metafile (*.wmf)|*.wmf|" +
				"Enhanced Windows Metafile (*.emf)|*.emf|" +
				"Joint Photographic Experts Group (*.jpeg) |*.jpeg";

			mySaveFileDialog.FilterIndex = 2;
			mySaveFileDialog.RestoreDirectory = true;

			if (mySaveFileDialog.ShowDialog() == DialogResult.OK)
			{
#if DEBUG
				ActiveView.WriteIGES(System.IO.Path.ChangeExtension(mySaveFileDialog.FileName, "iges"),false);
#endif
				switch (mySaveFileDialog.FilterIndex)
				{

					case 1: ActiveView.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
						break;
					case 2: ActiveView.WriteToFileRaster(3, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case 3: ActiveView.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Wmf);
						break;
					case 4: ActiveView.WriteToFileVector(false, mySaveFileDialog.FileName);
				//	case 4: ActiveView.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Emf);
						break;
					case 5: ActiveView.WriteToFileRaster(4, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
				}
			}
		}
		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveView.CopyToClipboardRaster();
		}

		Bitmap DualBitmap(float imageScale, int dividerWidth)
		{
			//create each view's bmp
			this[0].CopyToClipboardRaster(imageScale);
			Bitmap left = Clipboard.GetImage() as Bitmap;
			this[1].CopyToClipboardRaster(imageScale);
			Bitmap right = Clipboard.GetImage() as Bitmap;
			//create target bmp and graphics
			Bitmap bmp = new Bitmap(left.Width + right.Width + dividerWidth, left.Height);
			Graphics g = Graphics.FromImage(bmp);

			//draw each image
			g.DrawImage(left, new PointF(0, 0));
			g.DrawImage(right, new PointF(left.Width + dividerWidth, 0));

			//draw center divider if specified
			if( dividerWidth > 0 )
				g.FillRectangle(Brushes.Black, left.Width, 0, dividerWidth, left.Height);

			return bmp;
		}
		private void saveDualToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog mySaveFileDialog = new SaveFileDialog();

			mySaveFileDialog.InitialDirectory = ".";
			mySaveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|" +
				"Portable Network Graphics (*.png)|*.png|" +
				//"Windows metafile (*.wmf)|*.wmf|" +
				//"Enhanced Windows Metafile (*.emf)|*.emf|" +
				"Joint Photographic Experts Group (*.jpeg) |*.jpeg";

			mySaveFileDialog.FilterIndex = 2;
			mySaveFileDialog.RestoreDirectory = true;

			if (mySaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				System.Drawing.Imaging.ImageFormat frmt;
				switch (mySaveFileDialog.FilterIndex)
				{
					case 1: frmt = System.Drawing.Imaging.ImageFormat.Bmp;
						break;
					case 2: frmt = System.Drawing.Imaging.ImageFormat.Png;
						break;
					//case 3: frmt =System.Drawing.Imaging.ImageFormat.Wmf;
					//	break;
					//case 4: frmt =System.Drawing.Imaging.ImageFormat.Emf;
					//	break;
					case 3: frmt = System.Drawing.Imaging.ImageFormat.Jpeg;
						break;
					default: frmt = System.Drawing.Imaging.ImageFormat.Bmp;
						break;
				}

				DualBitmap(2, 3).Save(mySaveFileDialog.FileName, frmt);
			}
		}
		private void copyDualToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetImage(DualBitmap(1, 2));

		}

		//private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
		//{
		//	this[0].CopyToClipboardRaster(1);
		//	Bitmap left = Clipboard.GetImage() as Bitmap;
		//	this[1].CopyToClipboardRaster(1);
		//	Bitmap right = Clipboard.GetImage() as Bitmap;

		//	Bitmap bmp = new Bitmap(left.Width + right.Width, left.Height);
		//	for( int j = 0 ; j < bmp.Height; j++ )
		//	for (int i = 0; i < bmp.Width; i++)
		//	{
		//		bmp.SetPixel(i, j, i == left.Width ? Color.Black : i > left.Width ? right.GetPixel(i - left.Width, j) : left.GetPixel(i, j));
		//	}
		//	Clipboard.SetImage(bmp);
		//}
	}
}
