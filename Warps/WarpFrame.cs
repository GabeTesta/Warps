using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using Warps.Controls;
using Warps.Trackers;
using Warps.Curves;
using Warps.Yarns;
using Warps.Panels;
using Warps.Tapes;
using System.Xml;

namespace Warps
{
	public delegate void ObjectSelected(object sender, EventArgs<IRebuild> e);

	public delegate void VisibilityToggled(object sender, EventArgs<IRebuild> e);

	public delegate void UpdateUI(int nLyt, string msg);

	public partial class WarpFrame : Form
	{
		public static WarpFrame TheFrame
		{
			get { return ActiveForm as WarpFrame; }
		}
		public static Sail CurrentSail
		{
			get
			{
				if (WarpFrame.ActiveForm is WarpFrame)
					return (WarpFrame.ActiveForm as WarpFrame).m_sail;

				foreach (Form f in Application.OpenForms)
				{
					if (f is WarpFrame)
						return (f as WarpFrame).m_sail;
				}
				return null;
			}
		}

		public static ImageList Images
		{
			get
			{
				ImageList imageList = new ImageList();

				imageList.Images.Add("empty", Warps.Properties.Resources.empty);
				imageList.Images.Add("Sail", Warps.Properties.Resources.sail);
				imageList.Images.Add("Rig", Warps.Properties.Resources.rig);
				imageList.Images.Add("Main", Warps.Properties.Resources.main);
				imageList.Images.Add("Wire", Warps.Properties.Resources.wire);

				imageList.Images.Add("VariableGroup", Warps.Properties.Resources.VariableGroup);
				imageList.Images.Add("CurveGroup", Warps.Properties.Resources.curvegroup);
				imageList.Images.Add("YarnGroup", Warps.Properties.Resources.yarngrp);
				imageList.Images.Add("PanelGroup", Warps.Properties.Resources.PanelGroup);
				imageList.Images.Add("TapeGroup", Warps.Properties.Resources.TapeGroup);
				imageList.Images.Add("MixedGroup", Warps.Properties.Resources.folder);

				imageList.Images.Add("MouldCurve", Warps.Properties.Resources.curve);
				imageList.Images.Add("GuideComb", Warps.Properties.Resources.GuideComb);

				imageList.Images.Add("CurvePoint", Warps.Properties.Resources.FitCurve);
				imageList.Images.Add("SlidePoint", Warps.Properties.Resources.FitSlide);
				imageList.Images.Add("FixedPoint", Warps.Properties.Resources.FitFixed);
				imageList.Images.Add("CrossPoint", Warps.Properties.Resources.FitCross);
				imageList.Images.Add("OffsetPoint", Warps.Properties.Resources.FitOffset);
				imageList.Images.Add("AnglePoint", Warps.Properties.Resources.FitAngle);

				imageList.Images.Add("Equation", Warps.Properties.Resources.equation);
				imageList.Images.Add("EquationText", Warps.Properties.Resources.EqText);
				imageList.Images.Add("Result", Warps.Properties.Resources.EqNum);

				imageList.Images.Add("Warps", Warps.Properties.Resources.Warps);
				imageList.Images.Add("Surface", Warps.Properties.Resources.ContourSurf);
				//	imageList.Images.Add("Panel", Warps.Properties.Resources.panel);
				imageList.Images.Add("EndCondition", Warps.Properties.Resources.EndCondition);
				imageList.Images.Add("Limits", Warps.Properties.Resources.Limits);
				imageList.Images.Add("MaxMin", Warps.Properties.Resources.MaxMin);

				return imageList;
			}
		}

		static MaterialDatabase m_MatDB;// = new MaterialDatabase(@"c:\Materials.csv");
		public static MaterialDatabase Mats
		{
			get { return WarpFrame.m_MatDB; }
		}

		public WarpFrame(params string[] args)
		{

			InitializeComponent();

			//ReadConfigFile();
#if DEBUG
			Logleton.TheLog.CreateLogLocal("Warps");
			Logleton.TheLog.Log("new instance loaded", Logleton.LogPriority.Debug);
#endif
			//set background color from existing icon
			//ButtonUnSelected = m_modCurve.BackColor;
			Title = "";
			m_statusProgress.Visible = false;
			m_statusProgress.Step = 1;
			m_statusProgress.MarqueeAnimationSpeed = 1;

			//SetStyle(ControlStyles.OptimizedDoubleBuffer |
			//	    ControlStyles.AllPaintingInWmPaint, true);

			EditorPanel = null;//collapse edit panel

			//toggle auto mode to ensure coloring
			//m_autoBtn.Checked = true;
			//m_autoBtn.Checked = false;//enable automode

			m_tree.BeforeSelect += OnSelectionChanging;
			View.SelectionChanged += OnSelectionChanging;
			m_cancel.Click += cancelButton_Click;

			Tree.VisibilityToggle += View.VisibilityToggled;

			//m_horizsplit.SplitterDistance = m_horizsplit.ClientRectangle.Width - 250;
			//try
			//{
			//	m_MatDB = new MaterialDatabase(@"c:\Materials.csv");
			//}
			//catch (Exception e) { MessageBox.Show(e.Message); }
			if (args != null && args.Length > 0 && args[0] != null)
				LoadSail(args[0]);

		}

		public string Title
		{ set { Text = "Warps " + Version + ((value != null && value != "") ? " - " + value : ""); } } // display the version number in the title bar
		public string Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			ReadConfigFile();

			if (m_sail != null)
				View.ZoomFit(true);

			if (Mats != null)//show the material db in the tree
				Tree.SorTree.Nodes.Add(Mats.WriteNode());

			PopulateDropdowns();
		}

		private void ReadConfigFile()
		{
			Utilities.OpenConfigFile("Warps.xml");

			//m_ConfigFile = new VAkos.Xmlconfig(Path.Combine(Utilities.ExeDir, "Warps.xml"), true);
			this.SuspendLayout();

			Top = Utilities.Settings["WarpFrame/Location/Top", 0];
			Left = Utilities.Settings["WarpFrame/Location/Left", 0];

			Width = Utilities.Settings["WarpFrame/Width", 1000];
			Height = Utilities.Settings["WarpFrame/Height", 700];

			m_horizsplit.SplitterDistance = Utilities.Settings["Splitters/Tree", 250];
			m_vertsplit.SplitterDistance = Utilities.Settings["Splitters/Edit", 500];

			AutoBuild = Utilities.Settings["AutoBuild", true];

			View.ReadConfigFile();
			//SetLayers();

			m_MatDB = new MaterialDatabase(Utilities.Settings["MaterialTable", null]);

			this.ResumeLayout(true);
			this.PerformLayout();
		}
		//private void SetLayers()
		//{
		//	string layers = Utilities.Settings["View/Left/Layers", "Default,Mould"];
		//	string[] splits = layers.Split(',');
		//	View.ShowLayers(0, splits);
		//	layers = Utilities.Settings["View/Right/Layers", "Default,Mould,Curves,Yarns,Panels,Tapes"];
		//	splits = layers.Split(',');
		//	View.ShowLayers(1, splits);
		//}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			Utilities.Settings["WarpFrame/Location/Top"].intValue = Top;
			Utilities.Settings["WarpFrame/Location/Left"].intValue = Left;


			Utilities.Settings["WarpFrame/Width"].intValue = Width;
			Utilities.Settings["WarpFrame/Height"].intValue = Height;

			Utilities.Settings["Splitters/Tree"].intValue = m_horizsplit.SplitterDistance;
			Utilities.Settings["Splitters/Edit"].intValue = m_vertsplit.SplitterDistance;

			Utilities.Settings["AutoBuild"].boolValue = AutoBuild;


			StringBuilder lyrs = new StringBuilder();
			View.ActiveLayers(0).ForEach(l => lyrs.AppendFormat("{0},", l));
			Utilities.Settings["View/Left/Layers"].Value = lyrs.ToString();
			lyrs.Clear();
			View.ActiveLayers(1).ForEach(l => lyrs.AppendFormat("{0},", l));
			Utilities.Settings["View/Right/Layers"].Value = lyrs.ToString();

			Utilities.Settings["MaterialTable"].Value = m_MatDB.Label;
			Utilities.Settings["ColorTable"].Value = View.SaveColors();

			Utilities.CloseSettingsFile();
		}

		public string Status
		{
			get { return m_statusText.Text; }
			set { m_statusText.Text = value; }
		}
		void UpdateStatusStrip(string msg)
		{
			UpdateStatusStrip(0, msg);
		}
		void UpdateStatusStrip(int nLyt, string msg)
		{
			if (m_statusStrip.InvokeRequired)
			{
				m_statusStrip.Invoke(new UpdateUI(UpdateStatusStrip), new object[] { nLyt, msg });
			}
			else
			{
				m_statusText.Text = msg;
				if (nLyt < 0)//send negative value to set max
				{
					m_statusProgress.ProgressBar.Maximum = -nLyt;
					m_statusProgress.ProgressBar.Value = 0;
					m_statusProgress.ProgressBar.Minimum = 0;
				}
				else
					m_statusProgress.ProgressBar.PerformStep();
				if (nLyt != 0 && !m_statusProgress.Visible)
					m_statusProgress.Visible = true;
				m_statusStrip.Refresh();
			}
		}

		public DualView View
		{
			get { return m_dualView; }
		}
		public TabTree Tree
		{
			get { return m_tree; }
		}

		#region OpenFile

		Sail m_sail = null;
		public Sail ActiveSail
		{
			get { return m_sail; }
		}

		void OpenFile(int preselect)
		{
			string[] files = WarpFrame.OpenFileDlg(preselect);
			if (files != null && files.Length > 0)
			{
				foreach (string cof in files)
					LoadSail(cof);
			}
		}

		public static string[] OpenFileDlg(int extension)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "spiral files (*.spi)|*.spi|cof files (*.cof)|*.cof|warp files (*.wrp)|*.wrp|xml files (*.xml)|*.xml|binary files|*_wrp.bin|All files (*.*)|*.*";
			ofd.Multiselect = true;
			ofd.FilterIndex = Math.Min(extension, 3);
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return ofd.FileNames;
			return null;
		}


		void LoadSail(string path)
		{
			if (m_sail != null)
				clearAll_Click(null, null);

			Status = String.Format("Loading {0}", path);

			m_sail = new Sail();
			m_sail.updateStatus += UpdateStatusStrip;

			if (!m_sail.ReadFile(path))
			{
				Status = String.Format("{0} Load Failed", m_sail.FilePath);
				return;
			}

			m_tree.Add(m_sail);

			AddSailtoView(m_sail);

			Tree.ExpandToDepth(0);

			Status = String.Format("{0} Loaded Successfully", m_sail.FilePath);
			Title = m_sail.FilePath; // display the version number in the title bar
			m_statusProgress.Visible = false;

			//SetLayers();
			View.Refresh();
		}

		void LoadSailAsync(string path)
		{
			Status = String.Format("Loading {0}", path);

			Sail s = new Sail();

			//load the sail on a background thread for niceness
			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += LoadSailAsync;
			thread.RunWorkerCompleted += SailLoadedAsync;
			thread.WorkerReportsProgress = true;
			thread.WorkerSupportsCancellation = true;
			thread.RunWorkerAsync(new object[] { s, path });
		}
		void LoadSailAsync(object sender, DoWorkEventArgs e)
		{
			object[] args = e.Argument as object[];
			Sail s = args[0] as Sail;
			s.updateStatus += UpdateStatusStrip;
			string path = args[1] as string;

			if (s.ReadFile(path))
				e.Result = s;
			else
				e.Cancel = true;
		}
		void SailLoadedAsync(object sender, RunWorkerCompletedEventArgs e)
		{
			UpdateStatusStrip(0, "Generating Viewport Geometry");
			m_sail = e.Result as Sail;
			if (m_sail == null)
				Status = String.Format("{0} Load Failed", m_sail.FilePath);

			m_tree.Add(m_sail);

			AddSailtoView(m_sail);

			Tree.ExpandToDepth(0);

			Status = String.Format("{0} Loaded Successfully", m_sail.FilePath);
			Title = m_sail.FilePath; // display the version number in the title bar
			m_statusProgress.Visible = false;
		}

		private void AddSailtoView(Sail s)
		{
			int nlayer = View.AddLayer("Rig");
			nlayer = View.AddLayer("Mould");
			s.Mould.CreateEntities(null, false).ForEach(ent => { if (ent.LayerIndex == 0)ent.LayerIndex = nlayer; View.Add(ent); });

			//if (s.m_type != SurfaceType.OBJ)
			//{
			nlayer = View.AddLayer("Gauss");
			s.Mould.CreateEntities(null, true).ForEach(ent =>
			{
				ent.LayerIndex = nlayer; View.Add(ent);
				//if (ent is Mesh)
				//	View.SetColorScale(ent as Mesh);
			});


			nlayer = View.AddLayer("Extension");
			s.Mould.CreateEntities(new double[,] { { -.2, 1.2 }, { -.2, 1.2 } }, false).ForEach(ent => { ent.LayerIndex = nlayer; View.Add(ent); });
			//}

			//add the groups attached to the sail file if any
			if (s.Mould.Groups != null)
				s.Mould.Groups.ForEach(group => UpdateViews(group));

			s.Layout.ForEach(group => UpdateViews(group));
			//View.ShowLayers(0, "Mould", "Default");
			//View.ShowLayers(1, "Mould", "Yarns", "Curves", "Tapes");
			if (View.Created)
				View.ZoomFit(true);
		}

		#endregion

		#region Rebuild

		public bool Rebuilds(List<IRebuild> tags)
		{
			if (tags != null)
			{
				tags.ForEach(tag => tag.Update(ActiveSail));
			}
			if (AutoBuild)
			{
				//List<IRebuild> updated = ActiveSail.Rebuild(tag);
				List<IRebuild> connected = ActiveSail.GetConnected(tags);
				List<IRebuild> succeeded = new List<IRebuild>();
				List<IRebuild> failed = new List<IRebuild>();
				DateTime before = DateTime.Now;
				if (connected != null)
				{
					foreach (IRebuild item in connected)
					{
						if (tags.Contains(item) || item.Update(ActiveSail))
							succeeded.Add(item);
						else
							failed.Add(item);
					}
					//connected.ForEach(item => item.Update(ActiveSail));
				}
				//two lists (failed and succeeded)
				// update succeeded
				// invalidate failed
				DateTime after = DateTime.Now;
				Console.WriteLine("{0} ms", (after - before).TotalMilliseconds);
				StringBuilder b = new StringBuilder("Rebuilt Succeeded:\n");
				if (connected != null)
				{
					foreach (IRebuild item in succeeded)//succeeded
					{
						UpdateViews(item);
						b.AppendLine(item.ToString());
					}
					if (failed.Count > 0)
						b.AppendLine("\nRebuilt Failed:\n");
					foreach (IRebuild item in failed)//failed
					{
#if DEBUG
						UpdateViews(item);
#else
						View.Invalidate(item);
#endif
						Tree.Invalidate(item);
						b.AppendLine(item.ToString());
					}
					View.Refresh();
				}
#if DEBUG
				MessageBox.Show(b.ToString());
#endif
			}
			else if (tags != null)
				tags.ForEach(tag => UpdateViews(tag));
			return AutoBuild;
		}
		public bool Rebuild(IRebuild tag)
		{
			if (tag != null)
				tag.Update(ActiveSail);
			if (AutoBuild)
			{
				//List<IRebuild> updated = ActiveSail.Rebuild(tag);
				List<IRebuild> connected = ActiveSail.GetConnected(tag);
				List<IRebuild> succeeded = new List<IRebuild>();
				List<IRebuild> failed = new List<IRebuild>();
				DateTime before = DateTime.Now;
				if (connected != null)
				{
					foreach (IRebuild item in connected)
					{
						if (item == tag || item.Update(ActiveSail))
							succeeded.Add(item);
						else
							failed.Add(item);
					}
					//connected.ForEach(item => item.Update(ActiveSail));
				}
				//two lists (failed and succeeded)
				// update succeeded
				// invalidate failed
				DateTime after = DateTime.Now;
				Console.WriteLine("{0} ms", (after - before).TotalMilliseconds);
				StringBuilder b = new StringBuilder("Rebuilt Succeeded:\n");
				if (connected != null)
				{
					foreach (IRebuild item in succeeded)//succeeded
					{
						UpdateViews(item);
						b.AppendLine(item.ToString());
					}
					if (failed.Count > 0)
						b.AppendLine("\nRebuilt Failed:\n");
					foreach (IRebuild item in failed)//failed
					{
#if DEBUG
						UpdateViews(item);
#else
						View.Invalidate(item);
#endif
						Tree.Invalidate(item);
						b.AppendLine(item.ToString());
					}
					View.Refresh();
				}
#if DEBUG
				MessageBox.Show(b.ToString());
#endif
			}
			else if (tag != null)
			{
				UpdateViews(tag);
				View.Refresh();
			}
			return AutoBuild;
		}

		//private void parallelRebuild(List<IRebuild> updated)
		//{
		//	Parallel.ForEach<IRebuild>(updated, item =>
		//	{
		//		item.Update();
		//	});
		//}

		public void UpdateViews(IRebuild item)
		{
			View.Remove(item, true);

			Tree.BeginUpdate();

			View.Add(item, true);
			item.WriteNode();

			//if (item is IGroup)
			//{
			//	View.Add(item as IGroup);
			//	(item as IGroup).WriteNode();
			//}
			//else if (item is MouldCurve)
			//{
			//	View.Add(item as MouldCurve);
			//	(item as MouldCurve).WriteNode();
			//}
			//else if (item is Equation)
			//{
			//	(item as Equation).WriteNode();
			//}
			Tree.Revalidate(item);
			Tree.EndUpdate();
		}

		public int Delete(IRebuild tag)
		{
			if (AutoBuild)
			{
				//List<IRebuild> updated = ActiveSail.Rebuild(tag);
				List<IRebuild> connected = ActiveSail.GetConnected(tag);
				StringBuilder b = new StringBuilder("Invalid:\n");

				foreach (IRebuild item in connected)
				{
					View.Invalidate(item);
					Tree.Invalidate(item);
					b.AppendLine(item.ToString());
				}
#if DEBUG
				MessageBox.Show(b.ToString());
#endif
				Tree.Remove(tag);
				ActiveSail.Remove(tag);
				View.Refresh();
				//EditMode = false;
				return connected.Count;
			}

			return -1;
		}

		public bool AutoBuild
		{
			get { return !m_autoBtn.Checked; }
			set
			{
				m_autoBtn.Checked = !value;//invert for better display
				m_autoBtn_CheckedChanged(this, null);
			}
		}
		private void m_autoBtn_CheckedChanged(object sender, EventArgs e)
		{
			m_autoBtn.BackColor = AutoBuild ? ButtonSelected : ButtonUnSelected;
			m_autoBtn.ForeColor = AutoBuild ? Color.White : Color.Black;
			m_autoBtn.Invalidate();
		}

		private void m_buildBtn_Click(object sender, EventArgs e)
		{
			if (ActiveSail == null)
				return;
			//if (Tracker != null)
			//{
			//	Tracker = null;
			//	EditPanel = null;
			//}
			//m_modCurve.BackColor = ButtonUnSelected;
			if (Tree.SelectedTag is IRebuild)
				Rebuild(Tree.SelectedTag as IRebuild);
			else
				ActiveSail.Rebuild();
		}
		//private void m_cancelBtn_Click(object sender, EventArgs e)
		//{
		//	if (m_Tracker != null)
		//	{
		//		m_Tracker.OnCancel(sender, e);
		//		m_Tracker = null;
		//		EditorPanel = null;
		//	}
		//	//m_modCurve.BackColor = ButtonUnSelected;
		//}

		#endregion

		private void AddGroup_Click(object sender, EventArgs e)
		{
			if (ActiveSail == null)
				return;
			AddItemDialog dlg = new AddItemDialog();
			dlg.Name = "enter name";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IGroup grp = dlg.CreateIRebuild() as IGroup;
				if (grp != null)
				{
					ActiveSail.Add(grp);
					ActiveSail.Rebuild();
					Tree.SelectedItem = grp;
				}
			}
		}

		//private void m_editButton_Click(object sender, EventArgs e)
		//{
		//	m_editButton.Checked = !m_editButton.Checked;

		//	m_editButton.BackColor = AutoBuild ? Color.SeaGreen : Color.IndianRed;
		//	m_editButton.ForeColor = AutoBuild ? Color.White : Color.DarkRed;


		//	okButton.Enabled = m_editButton.Checked;
		//	cancelButton.Enabled = m_editButton.Checked;
		//	previewButton.Enabled = m_editButton.Checked;

		//	if (m_Tracker != null)
		//		m_Tracker.EditMode = m_editButton.Checked;

		//}

		Color ButtonSelected = Color.SeaGreen;
		Color ButtonUnSelected = Color.FromKnownColor(KnownColor.Control);

		#region Tracker

		//entry point for trackers
		public void OnSelectionChanging(object sender, EventArgs ea)
		{
			//	if ((sender == Tree || sender == View) && m_Tracker != null && m_Tracker.EditMode)
			//		return; //dont do anything if we are already edit-tracking

			object Tag = ea is EventArgs<IRebuild> ? (ea as EventArgs<IRebuild>).Value : ea is TreeViewCancelEventArgs ? (ea as TreeViewCancelEventArgs).Node.Tag : null;
			if (m_Tracker != null && m_Tracker.IsTracking)//pass the selection on to the tracker when actively tracking something (warps, guides, etc)
			{
				if (ea is TreeViewCancelEventArgs)
					(ea as TreeViewCancelEventArgs).Cancel = true;//cancel the tree selection change if tracking
				m_Tracker.ProcessSelection(Tag);
				return;
			}

			Status = "";
			View.DeSelectAll();

			ITracker track = null;
			if (Tag == null)
				return;
			else if (Tag is MouldCurve)
			{
				if (Tag is GuideComb)
					track = new GuideCombTracker(Tag as GuideComb);
				else
					track = new CurveTracker(Tag as MouldCurve);
			}
			else if (Tag is IGroup)
			{
				switch (Tag.GetType().Name)
				{
					case "CurveGroup":
						track = new CurveGroupTracker(Tag as CurveGroup);
						break;

					case "VariableGroup":
						track = new VariableGroupTracker(Tag as VariableGroup);
						break;

					case "YarnGroup":
						track = new YarnGroupTracker(Tag as YarnGroup);
						break;

					case "PanelGroup":
						track = new PanelGroupTracker(Tag as PanelGroup);
						break;

					case "TapeGroup":
						track = new Tapes.TapeGroupTracker(Tag as Tapes.TapeGroup);
						break;

					case "MixedGroup":
						track = new Mixed.MixedGroupTracker(Tag as Mixed.MixedGroup);
						break;
				}
			}
			else if (Tag is Equation)
			{
				VariableGroup parent;
				if (ActiveSail.FindParent(Tag as Equation, out parent))
					track = new VariableGroupTracker(parent);
				else
					track = new VariableTracker(Tag as Equation);
			}
			else if (Tag is GuideSurface)
			{
				track = new SurfaceTracker(Tag as GuideSurface);
			}

			//else// if (e.Value is Sail)
			//	track = new SailTracker();


			if (track != null)
			{
				Logleton.TheLog.Log(String.Format("Creating new {0} from {1}", track.GetType().Name, Tag == null ? "null" : Tag.GetType().Name), Logleton.LogPriority.Debug);
				PostTracker(track);
			}

		}

		//public bool EditMode
		//{
		//	get { return !m_editButton.Checked; }
		//	set
		//	{
		//		m_editButton.Checked = !value;
		//		//m_editButton_CheckedChanged(this, new EventArgs());
		//	}
		//}
		//private void m_editButton_CheckedChanged(object sender, EventArgs e)
		//{
		//	m_editButton.BackColor = EditMode ? ButtonSelected : ButtonUnSelected;
		//	m_editButton.ForeColor = EditMode ? Color.White : Color.Black;

		//	if (m_Tracker != null)
		//	{
		//		SuspendLayout();
		//		ITracker tracker = m_Tracker;
		//		ClearTracker();
		//		tracker.EditMode = EditMode;
		//		PostTracker(tracker);
		//		ResumeLayout(true);
		//	}

		//	//cancelButton.Enabled = EditMode;

		//	//okButton.Enabled = EditMode;
		//	//previewButton.Enabled = EditMode;

		//	//if (m_Tracker != null)
		//	//{
		//	//	m_Tracker.EditMode = EditMode;
		//	//	if (EditMode)
		//	//	{
		//	//		okButton.Click += m_Tracker.OnBuild;
		//	//		cancelButton.Click += m_Tracker.OnCancel;
		//	//		previewButton.Click += m_Tracker.OnPreview;
		//	//	}
		//	//	else
		//	//	{
		//	//		okButton.Click -= m_Tracker.OnBuild;
		//	//		cancelButton.Click -= m_Tracker.OnCancel;
		//	//		previewButton.Click -= m_Tracker.OnPreview;
		//	//	}
		//	//}
		//}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			ClearTracker();
		}

		ITracker m_Tracker;

		private void PostTracker(ITracker tracker)
		{
			if (tracker == null)
				return;

			if (m_Tracker != null)
				ClearTracker();

			Status = "Tracking " + tracker.ToString();
			m_Tracker = tracker;//post the new tracker

			m_Tracker.Track(this);

			okButton.Click += m_Tracker.OnBuild;
			previewButton.Click += m_Tracker.OnPreview;

			//if (EditMode)
			//	EditTracker(tracker);
			//else
			//	ReadonlyTracker(tracker);
		}

		//ITracker EditTracker(ITracker tracker)
		//{
		//	if (tracker == null)
		//		return null;

		//	if (m_Tracker != null && m_Tracker.EditMode)
		//		ClearTracker();

		//	Status = "Editing " + tracker.GetType().Name;
		//	m_Tracker = tracker;//post the new tracker

		//	m_Tracker.Track(this);
		//	okButton.Click += m_Tracker.OnBuild;
		//	//cancelButton.Click += m_Tracker.OnCancel;
		//	previewButton.Click += m_Tracker.OnPreview;

		//	return m_Tracker;//return it
		//}
		//ITracker ReadonlyTracker(ITracker tracker)
		//{
		//	if (tracker == null)
		//		return null;

		//	if (m_Tracker != null && m_Tracker.EditMode)
		//		return m_Tracker;//dont overwrite an existing edit-mode tracker

		//	if (m_Tracker != null)//cancel any exising readonly-tracker
		//		ClearTracker();

		//	Status = "Inspecting " + tracker.GetType().Name;
		//	m_Tracker = tracker;//post the new tracker
		//	m_Tracker.Track(this);
		//	return m_Tracker;//return it
		//}
		internal void ClearTracker()
		{
			if (m_Tracker == null)
			{
				if (EditorPanel != null)
					EditorPanel = null;
				return;
			}

			if (m_Tracker != null)
			{
				m_Tracker.Cancel();//clear any existing tracker

				okButton.Click -= m_Tracker.OnBuild;
				previewButton.Click -= m_Tracker.OnPreview;
			}

			m_Tracker = null;
			View.EditMode = false;//force all items to be opaque
			View.DeSelectAll();
			View.DeSelectAllLayers();
			View.Refresh();
		}

		public Control EditorPanel
		{
			get
			{
				return editPanel.Controls.Count > 0 ? editPanel.Controls[0] : null;
			}
			set
			{
				editPanel.SuspendLayout();

				editPanel.Controls.Clear();
				if (value != null)
				{
					editPanel.Controls.Add(value);
					value.Dock = DockStyle.Fill;
					if (m_horizsplit.Panel2Collapsed)
						m_horizsplit.Panel2Collapsed = false;
				}
				//else
				//	m_horizsplit.Panel2Collapsed = true;

				editPanel.ResumeLayout();

			}
		}

		#endregion

		//		private void helpToolStripButton_Click(object sender, EventArgs e)
		//		{

		//			//Warps.Surfaces.RBFMesh mesh = new Surfaces.RBFMesh(@"C:\Users\Mikker\Desktop\enginecoverin2meshes.obj");
		//			//Warps.Surfaces.RBFMesh mesh = new Surfaces.RBFMesh(@"C:\Users\Mikker\Desktop\single.obj");
		//			//mesh.m_obj.AddToScene(m_dualView.ActiveView);
		//			//m_sail = new Sail();
		//			//LoadSail(@"C:\Users\Mikker\Desktop\small.obj");
		//			//View.Refresh();
		//			//return;
		//			if (ActiveSail != null)
		//				clearAll_Click(null, null);
		//			if (ActiveSail == null)
		//			{
		//				//LoadSail(@"C:\Users\Mikker\Desktop\small.obj");
		//				//LoadSail(@"C:\Users\Mikker\Desktop\single.obj");
		//				//View.Refresh();
		//				//return;
		//				if(ModifierKeys == Keys.Control )
		//				LoadSail(@"C:\Users\Mikker\Desktop\TS\WARPS\Main.sail");
		//				else
		//				LoadSail(@"C:\Users\Mikker\Desktop\TS\WARPS\df.sail");
		//			}

		//			if (Tree.SelectedTag != null)
		//			{
		//				IRebuild tag = Tree.SelectedTag as IRebuild;
		//				List<IRebuild> rebuilds = new List<IRebuild>();
		//				if (tag != null)
		//					tag.GetParents(ActiveSail, rebuilds);

		//				StringBuilder sb = new StringBuilder();
		//				foreach (IRebuild rb in rebuilds)
		//					sb.AppendLine(rb.Label);
		//				MessageBox.Show(sb.ToString());
		//				return;
		//			}

		//			CurveGroup fills = new CurveGroup("Fills", ActiveSail);
		//			//MouldCurve guide = fills.Add(new MouldCurve("Up", ActiveSail, new IFitPoint[] {new FixedPoint(.7,.7), new FixedPoint(.4,.15) }));
		//			MouldCurve guide = fills.Add(new MouldCurve("Guide", ActiveSail, new IFitPoint[] { new FixedPoint(.5, -.1), new FixedPoint(.5, 1)}));
		//			ActiveSail.Add(fills);

		//			PanelGroup mids = new PanelGroup("MidPan", ActiveSail, new Equation("w", 1), PanelGroup.ClothOrientations.FILLS);
		//			mids.Bounds.Add(ActiveSail.FindCurve("Luff"));
		//			//mids.Bounds.Add(ActiveSail.FindCurve("M-spl"));//ensure curves are ordered correctly
		//			mids.Bounds.Add(ActiveSail.FindCurve("Head"));//ensure curves are ordered correctly
		//			mids.Bounds.Add(ActiveSail.FindCurve("Leech"));
		////			mids.Bounds.Add(ActiveSail.FindCurve("L-spl"));
		//			mids.Bounds.Add(ActiveSail.FindCurve("Foot"));


		//			mids.Guides.Add(guide);
		//			ActiveSail.Add(mids);

		//			if (false)
		//			{

		//			//IGroup outer = ActiveSail.CreateOuterCurves();
		//			PanelGroup pans = new PanelGroup("TackPan", ActiveSail);
		//			pans.Bounds.Add(ActiveSail.FindCurve("Foot"));//ensure curves are ordered correctly
		//			pans.Bounds.Add(ActiveSail.FindCurve("Luff"));
		//			pans.Bounds.Add(ActiveSail.FindCurve("L-spl"));
		//			pans.Bounds.Add(ActiveSail.FindCurve("1-spl"));

		//			//Vect2 end = new Vect2(), sPos = new Vect2();
		//			//Vect3 xyz = new Vect3();
		//			//CurveTools.CrossPoint(pans.Bounds[2], pans.Bounds[3], ref end, ref xyz, ref sPos, 10);

		//			MouldCurve tac = fills.Add(new MouldCurve("Tack", ActiveSail,
		//				new IFitPoint[]{
		//				new FixedPoint(0,0),
		//				new OffsetPoint(.5, pans.Bounds[1], 2 )}));
		//			MouldCurve clw = fills.Add(new MouldCurve("Clew", ActiveSail,
		//				new IFitPoint[]{
		//				new FixedPoint(1, 0),
		//				new CrossPoint(pans.Bounds[2], pans.Bounds[3])}));

		//			//ActiveSail.Add(fills);

		//			pans.Guides.Add(tac);
		//			ActiveSail.Add(pans);


		//				PanelGroup clew = new PanelGroup("ClewPan", ActiveSail);
		//				clew.Bounds.Add(ActiveSail.FindCurve("Foot"));//ensure curves are ordered correctly
		//				clew.Bounds.Add(ActiveSail.FindCurve("Leech"));
		//				clew.Bounds.Add(ActiveSail.FindCurve("L-spl"));
		//				clew.Bounds.Add(ActiveSail.FindCurve("1-spl"));
		//				clew.Guides.Add(clw);

		//				ActiveSail.Add(clew);

		//				//Warps.Panels.PanelGroup mids = new Panels.PanelGroup("MidPan", ActiveSail);
		//				//mids.Bounds.Add(ActiveSail.FindCurve("L-spl"));
		//				//mids.Bounds.Add(ActiveSail.FindCurve("Leech"));
		//				//mids.Bounds.Add(ActiveSail.FindCurve("M-spl"));//ensure curves are ordered correctly
		//				//mids.Bounds.Add(ActiveSail.FindCurve("Luff"));

		//				//fills.Add(new MouldCurve("Up", ActiveSail, new IFitPoint[] { new FixedPoint(end), new SlidePoint(mids.Bounds[2], 0.5) }));


		//				//mids.Guides.Add(fills[2]);

		//				//ActiveSail.Add(mids);


		//				PanelGroup tops = new PanelGroup("TopPan", ActiveSail);
		//				tops.Bounds.Add(ActiveSail.FindCurve("M-spl"));//ensure curves are ordered correctly
		//				tops.Bounds.Add(ActiveSail.FindCurve("Leech"));
		//				tops.Bounds.Add(ActiveSail.FindCurve("Head"));//ensure curves are ordered correctly
		//				tops.Bounds.Add(ActiveSail.FindCurve("Luff"));
		//				tops.Guides.Add(ActiveSail.FindCurve("Leech"));

		//				ActiveSail.Add(tops);
		//				//MouldCurve miter = new MouldCurve("Miter", ActiveSail,
		//				//	new IFitPoint[]{
		//				//		new CurvePoint(ActiveSail.FindCurve("Foot"), 0.5),
		//				//		new CurvePoint(ActiveSail.FindCurve("Head"), 0.5)});

		//				//(outer as CurveGroup).Add(miter);
		//				//List<MouldCurve> guider = new List<MouldCurve>();
		//				//guider.Add(miter);

		//			}

		//			ActiveSail.Rebuild(null);
		//			UpdateViews(fills);
		//		//	UpdateViews(pans);
		//		//	UpdateViews(clew);
		//			UpdateViews(mids);
		//		//	UpdateViews(tops);
		//			View.Refresh();
		//			return;

		//			//VariableGroup varGroup = new VariableGroup("Vars", ActiveSail);
		//			//varGroup.Add(new Equation("yarScale", 1.0));
		//			//varGroup.Add(new Equation("yarnDPI", "yarScale * 12780"));
		//			//varGroup.Add(new Equation("targetScale", 1.0));
		//			//varGroup.Add(new Equation("targetDPI", "targetScale * 14416"));
		//			//ActiveSail.Add(varGroup);

		//			//UpdateViews(ActiveSail.CreateOuterCurves());

		//			////Geodesic geo = new Geodesic("Geo", ActiveSail, new IFitPoint[] { new FixedPoint(.1, .1), new FixedPoint(.1, .9) });
		//			//MouldCurve v1 = new MouldCurve("v1", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.3, .4), new FixedPoint(.1, .8), new FixedPoint(0, 1) });

		//			//MouldCurve v2 = new MouldCurve("v2", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(0, 1) });

		//			//MouldCurve v3 = new MouldCurve("v3", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.95, .25), new FixedPoint(.9, .55), new FixedPoint(.65, .85), new FixedPoint(0, 1) });
		//			////MouldCurve v4 = new MouldCurve("v4", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.8, .5), new FixedPoint(1, 1) });
		//			////MouldCurve v5 = new MouldCurve("v5", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(1, 1) });
		//			//CurveGroup grp = new CurveGroup("Warps", ActiveSail);
		//			//grp.Add(v1);
		//			//grp.Add(v2);
		//			//grp.Add(v3);
		//			//grp.Add(new MouldCurve("g3", ActiveSail,
		//			//	new IFitPoint[] { 
		//			//		new FixedPoint(0,0), 
		//			//		new SlidePoint(v1, 0), 
		//			//		new FixedPoint(1,.5) }));
		//			//grp.Add(new MouldCurve("g4", ActiveSail,
		//			//	new IFitPoint[] { 
		//			//		new FixedPoint(1, 0), 
		//			//		new FixedPoint(.4, .4), 
		//			//		new FixedPoint(.3, .7),
		//			//		new FixedPoint(0,1)}));
		//			////grp.Add(v4);
		//			////grp.Add(v5);
		//			////grp.Add(guide);

		//			//CurveGroup guides = new CurveGroup("Guides", ActiveSail);
		//			//GuideComb guide = new GuideComb("Guide", ActiveSail,
		//			//	new IFitPoint[] {
		//			//		new FixedPoint(0, .5), 
		//			//		new SlidePoint(v2, .5),
		//			//		new FixedPoint(1, .5) },
		//			//	new Vect2[] { 
		//			//		new Vect2(0, 1), 
		//			//		new Vect2(.3, .55),
		//			//		new Vect2(.5, .5), 
		//			//		new Vect2(.7, .55), 
		//			//		new Vect2(1, 1) });
		//			//guides.Add(guide);

		//			//YarnGroup yar = new YarnGroup("yar1", ActiveSail, varGroup["yarnDPI"], varGroup["targetDPI"]);
		//			//yar.Warps.Add((ActiveSail.FindGroup("Outer") as CurveGroup)[0]);
		//			//yar.Warps.Add((ActiveSail.FindGroup("Outer") as CurveGroup)[1]);
		//			//yar.Guide = guide;
		//			//yar.DensityPos = new List<double>() { 0.2, 0.8 };
		//			//ActiveSail.Add(grp);
		//			//ActiveSail.Add(guides);
		//			//ActiveSail.Add(yar);
		//			//UpdateViews(grp);
		//			//UpdateViews(guides);


		//			////YarnGroup LuYar = new YarnGroup("LuYar", ActiveSail, 12780);
		//			////LuYar.DensityPos.AddRange(new double[] { 0.25, 0.5, 0.75 });
		//			////LuYar.YarnsUpdated += LuYar_YarnsUpdated;
		//			//////if (LuYar.LayoutYarns(new List<MouldCurve>() { lu, mi, le }, guide, 14416) > 0)
		//			//////DateTime now = DateTime.Now;
		//			//////LuYar.LayoutYarns(grp, guide, 14416, LuYar.SpreadYarnsAlongGuide);
		//			//////TimeSpan gde = DateTime.Now - now;
		//			//////now = DateTime.Now;

		//			//////LuYar.LayoutYarns(grp, guide, 14416, LuYar.SpreadYarnsAcrossWarps);
		//			//////TimeSpan wrps = DateTime.Now - now;
		//			//////now = DateTime.Now;
		//			//////MessageBox.Show(string.Format("AcrossWarps: {0}\nAlongGuide: {1}", wrps.TotalMilliseconds, gde.TotalMilliseconds));

		//			//UpdateViews(guides);
		//			//UpdateViews(grp);
		//			//yar.Update(ActiveSail);
		//			//UpdateViews(yar);
		//			////if (LuYar.LayoutYarns(grp, guide, 14416) > 0
		//			////	|| MessageBox.Show(String.Format("Failed to match Target Dpi\nTarget: {0}\nAchieved: {1}\nContinue Anyway?", LuYar.TargetDpi, LuYar.AchievedDpi), "Yarn Generation Failed", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes )
		//			////	ActiveSail.Add(LuYar);

		//			//////Yarns.YarnGroup LeYar = new Yarns.YarnGroup("LeYar", ActiveSail, 12780);
		//			//////if (LeYar.LayoutYarns(new List<MouldCurve>() { mi, le }, guide, 14416) > 0)
		//			//////	ActiveSail.Add(LeYar);

		//			//////Rebuild(null);

		//			////UpdateViews(LuYar);
		//			////Rebuild(grp);
		//			////Rebuild(grp);
		//			////Rebuild(guides);
		//			////Rebuild(LuYar);
		//			//View.Refresh();
		//			//ActiveSail.Rebuild(null);
		//		}

		void YarnsUpdated(object sender, EventArgs<YarnGroup> e)
		{
			UpdateViews(e.Value);
			View.Refresh();
		}

		//handles program-scope shortcut keys
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.S | Keys.Control://save existing file
					SaveFile(false);
					break;
				case Keys.O | Keys.Control://load a new file
				case Keys.L | Keys.Control://load a new file
					OpenFile(3);
					break;
				case Keys.N | Keys.Control://create a new project from a .spi/.cof
					OpenFile(1);
					break;
				case Keys.P | Keys.Control://write 3dl file on print
					printToolStripButton_Click(null, null);
					break;
				case Keys.F1:
				case Keys.F1 | Keys.Control:
					helpToolStripButton_Click(null, null);
					break;
				//case Keys.E | Keys.Control://toggle edit
				//	EditMode = !EditMode;
				//	break;
				case Keys.B | Keys.Control://run build
				case Keys.F5:
					m_buildBtn_Click(null, null);
					break;
				case Keys.A | Keys.Control://toggle autobuild
					AutoBuild = !AutoBuild;
					break;

			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		#region Toolbar


		#region New/Open/Save

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			OpenFile(1);
		}
		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			//#if DEBUG
			//			string[] files = Utilities.OpenFileDlg("xml", "Open Yarn xml", null);
			//			if( files != null )
			//			{
			//				XmlDocument doc = new XmlDocument();
			//				doc.Load(files[0]);

			//				//VAkos.Xmlconfig yarns = new VAkos.Xmlconfig(files[0], false);
			//				foreach(XmlNode group in doc.DocumentElement.ChildNodes)
			//				{
			//					IGroup grp = Utilities.CreateInstance(group.Name) as IGroup;
			//					if (grp is YarnGroup)
			//					{
			//						(grp as YarnGroup).ReadXScript(ActiveSail, group);
			//						ActiveSail.Add(grp);
			//					}
			//				}
			//			}
			//#else
			OpenFile(3);
			//#endif
		}
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			//Sail s;
			//if (m_sails != null && m_sails.Count > 0)
			//	s = m_sails[0];
			//else
			//	return;
			SaveFile(sender == saveAsToolStripMenuItem);
			//	m_tree.SaveScriptFile(sfd.FileName);
		}
		private void saveBinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveSail == null)
			{
				MessageBox.Show("Open a project before saving bin");
				return;
			}
			string path = ActiveSail.Mould.Label.Substring(0, ActiveSail.Mould.Label.Length - 4) + "_wrp.bin";
			Task.Factory.StartNew(() => ActiveSail.WriteBinFile(path));
			return;
		}

		private void printToolStripButton_Click(object sender, EventArgs e)
		{
			//Write 3dl file
			Logleton.TheLog.Log("Saving project to 3dl file");
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.DefaultExt = ".3dl";
			dlg.AddExtension = true;
			dlg.Filter = "3dl files (*.3dl)|*.3dl|All files (*.*)|*.*";
			dlg.FileName = Path.GetFileNameWithoutExtension(ActiveSail.Mould.Label);
			dlg.InitialDirectory = Path.GetDirectoryName(ActiveSail.FilePath);
			if (dlg.ShowDialog() == DialogResult.OK)
				Save3dlFile(dlg.FileName);
		}

		private void SaveFile(bool saveAs)
		{
			if (ActiveSail == null)
				return;
			if (saveAs || ActiveSail.FilePath == null)
			{
				string path = Utilities.SaveFileDialog("wrp", "Save Script File?", ActiveSail.FilePath);
				//SaveFileDialog sfd = new SaveFileDialog();
				//sfd.DefaultExt = ".wrp";
				//sfd.AddExtension = true;
				//if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)'
				if (path != null)
				{
					Logleton.TheLog.Log("saving file: " + path, Logleton.LogPriority.Debug);
					//	ActiveSail.WriteScriptFile(path);
					ActiveSail.WriteXScript(path);
				}
			}
			else
				ActiveSail.WriteXScript(Path.ChangeExtension(Path.GetFullPath(ActiveSail.FilePath), "wrp"));
			//ActiveSail.WriteScriptFile(Path.ChangeExtension(Path.GetFullPath(ActiveSail.FilePath), "wrp"));
		}
		void Save3dlFile(string tdipath)
		{
			Logleton.TheLog.Log("saving 3dl file...");
			ActiveSail.Save3DLFile(tdipath);
			return;
			//Task.Factory.StartNew(() =>
			//{
			//	logger.Instance.Log("saving {0} to {1}...", Path.GetFileName(tdipath), Path.GetDirectoryName(tdipath));
			//	using (StreamWriter sw = new StreamWriter(tdipath))
			//	{
			//		//write header
			//		//OUS102439-001, Fat26, EnergySolution ITA14313, Main ORCi, 10850 dpi, 3Dl 680, Capitani/NSI,//3DLayOut_Release 1.1.0.171
			//		sw.WriteLine("{0} //Warps v{1}", ActiveSail.ToString(), Utilities.CurVersion);
			//		foreach (YarnGroup yar in ActiveSail.Layout.FindAll(grp => grp is YarnGroup))
			//		{
			//			List<List<double>> sPos;
			//			List<List<Point3D>> ents = yar.CreateYarnPaths(out sPos);

			//			Vect2 uv = new Vect2(); Vect3 xyz = new Vect3();
			//			for (int i = 0; i < ents.Count; i++)
			//			{
			//				//FOOT   1.0000   FT_IN  0.0000  spacing  0.0853    48 offsets on yarn #1
			//				sw.WriteLine("{0}   {1:0.0000}   {2}  {3:0.0000}  {4}  {5:0.0000}    {6} offsets on yarn #{7}"
			//						, yar[i].m_Warps[1].Label, 1.0, yar[i].m_Warps[0].Label, 0, "spacing", 0, ents[i].Count - 1, i);

			//				for (int j = 0; j < ents[i].Count; j++)
			//				{
			//					yar[i].uVal(sPos[i][j], ref uv);

			//					sw.WriteLine(" {0:#0.00000} {1:#0.00000}    {2}  {3:#0.000000}  {4:#0.000000}  {5:#0.000000}  {6:#0.000000}  {7:#0.000000}  {8:#0.000000}  {9:#0.000000}",
			//						uv[0], uv[1], j
			//						, ents[i][j].X, ents[i][j].Y, ents[i][j].Z
			//						, 0, 0, 0
			//						, 0);
			//					//0.98647-0.00093    0  3.726569 -0.020588  0.007790  0.155863  0.003351  0.987773  0.000000
			//				}
			//			}
			//		}
			//		//read the yarn points from the entities
			//		//out s, uv, xyz
			//		//just use 100 points for now
			//		//createlinearpath
			//		//this will be what we used eventually
			//		//List<Entity> CreateEntities(bool bFitPoints, double TolAngle, out double[] sPos)
			//		//for now use 1/100 evenly spaced sPos

			//		/*
			//		-we need the header to tokenize the layout version
			//			tokenized with "//"

			//		- we tokenize each yarns header to get the count
			//			currently written on index 6 (int count = atoi(tmp[6].c_str()))

			//		///function that parses each line of the yarn.3dl file
			//		void	YarnGantPass::SplitLine(std::string line)
			//		{
			//			std::vector<std::string> shifter;
			//			std::vector<int> widths;
			//			widths.push_back(8);
			//			widths.push_back(8);
			//			widths.push_back(5);
			//			widths.push_back(10);
			//			widths.push_back(10);
			//			widths.push_back(10);
			//			widths.push_back(10);
			//			widths.push_back(10);
			//			widths.push_back(10);
			//			widths.push_back(10);

			//			shifter = Tokenize(line, widths);

			//			AddUV(atof(shifter[0].c_str()),atof(shifter[1].c_str()));
			//			AddXYZ(atof(shifter[3].c_str()),atof(shifter[4].c_str()),atof(shifter[5].c_str()));
			//		}

			//		  OUS102439-001, Fat26, EnergySolution ITA14313, Main ORCi, 10850 dpi, 3Dl 680, Capitani/NSI,//3DLayOut_Release 1.1.0.171
			//		  FOOT   1.0000   FT_IN  0.0000  spacing  0.0853    48 offsets on yarn #1
			//		  0.98647-0.00093    0  3.726569 -0.020588  0.007790  0.155863  0.003351  0.987773  0.000000
			//		 */
			//	}
			//	logger.Instance.Log("saving done");
			//});
		}

		#endregion

		#region ShowDir

		private void projectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveSail != null)
			{
				Utilities.HandleProcess(Path.GetDirectoryName(ActiveSail.FilePath), null);
			}
		}

		private void exeDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Utilities.HandleProcess(Utilities.ExeDir, null);
		}

		private void configFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Utilities.HandleProcess("Warps.xml", null);
		}

		#endregion

		#region Item DropDowns
		/// <summary>
		/// populates the group and item dropdown lists for adding new items
		/// </summary>
		private void PopulateDropdowns()
		{
			List<Type> groups = Utilities.GetAllOf(typeof(IGroup), false);
			groups.RemoveAll(gr => gr.IsAbstract || gr.IsInterface);
			groups.Sort((a, b) => a.Name.CompareTo(b.Name));
			groups.Reverse(groups.Count - 2, 2);
			groups.ForEach(grp => m_groupsDD.DropDownItems.Add(grp.Name.Replace("Group", ""), Images.Images[grp.Name]).Tag = grp);

			string[] excludes = new string[] { "Panel", "DensityComb" };
			List<Type> items = Utilities.GetAllOf(typeof(IRebuild), false);
			items.RemoveAll(it => it.IsAbstract || it.IsInterface || groups.Contains(it) || excludes.Contains(it.Name));
			items.Sort((a, b) => a.Name.CompareTo(b.Name));
			items.ForEach(it => m_itemsDD.DropDownItems.Add(it.Name, Images.Images[(it.Name.Contains("Surface") || it.Name.Contains("Mesh")) ? "Surface" : it.Name]).Tag = it);
		}

		/// <summary>
		/// Enables/disables each dropdown item depending on the target group's CanInsert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DropDownOpening(object sender, EventArgs e)
		{
			IGroup target = Tree.SelectedItem as IGroup;
			if (target == null && ActiveSail != null)
				ActiveSail.FindParent(Tree.SelectedItem, out target);

			ToolStripDropDownItem dd = sender as ToolStripDropDownItem;
			if (target != null)
				foreach (ToolStripItem item in dd.DropDownItems)
					item.Enabled = target.CanInsert(item.Tag as Type);
			else
				foreach (ToolStripItem item in dd.DropDownItems)
					item.Enabled = true;
		}

		/// <summary>
		/// occurs when either Groups or Items drop down items are clicked. creates and inserts the desired item into the currently selected group
		/// </summary>
		/// <param name="sender">the sending control</param>
		/// <param name="e">the event args including the clicked item</param>
		private void DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (ActiveSail == null)
				return;
			Type tomake = e.ClickedItem.Tag as Type;
			if (tomake == null)
				return;
			else
			{
				//construct a default item
				IRebuild item = Utilities.CreateInstance<IRebuild>(tomake, null);
				if (item == null)
					return;
				if (item is IGroup)
					(item as IGroup).Sail = ActiveSail;
				//drop it on the selected item or it's parent
				IRebuild target = Tree.SelectedItem;
				IGroup targetGroup;
				if (target is IGroup && (target as IGroup).CanInsert(item.GetType()))
					targetGroup = target as IGroup;
				else
					ActiveSail.FindParent(target, out targetGroup);

				//insert the item and refresh the drop targets tree
				if (targetGroup == null)//use sail if no group
				{
					ActiveSail.Insert(item, target);
					ActiveSail.WriteNode();
				}
				else
				{
					targetGroup.Insert(item, target);
					targetGroup.WriteNode();
				}

				//track it
				Tree.SelectedItem = item;
			}
		}


		#endregion

		private void clearAll_Click(object sender, EventArgs e)
		{
			if (m_sail == null || DialogResult.Yes == MessageBox.Show(string.Format("Are you sure you want to close\n{0}\nUnsaved changes will be lost.", m_sail.FilePath), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
			{
				Tree.ClearAll();
				View.ClearAll();
				ClearTracker();
				if (m_sail != null)
					m_sail.Layout.Clear();
				m_sail = null;
			}
		}

		private void baxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveSail.CreateBaxCurves();
			ActiveSail.Rebuild();
		}


		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
#if !DEBUG
			WarpAbout about = new WarpAbout();
			about.ShowDialog(this);
			return;
#endif
			//EquationEditorForm frm = new EquationEditorForm(null);
			//frm.ShowDialog();

			//InputEquationForm inp = new InputEquationForm();
			//inp.ShowDialog();
			//if (ActiveSail != null)
			//	clearAll_Click(null, null);
			if (ActiveSail == null)
			{
				//LoadSail(@"C:\Users\Mikker\Desktop\small.obj");
				//LoadSail(@"C:\Users\Mikker\Desktop\single.obj");
				//View.Refresh();
				//return;
				if (ModifierKeys == Keys.Control)
				{
					LoadSailAsync(@"C:\Users\Mikker\Desktop\TS\WARPS\Bin Test\DummyJib2.wrp");
					return;
				}
				else
				{
					LoadSail(@"C:\Users\Mikker\Desktop\TS\WARPS\Bin Test\99821022-01.spi");
					//LoadSailAsync(@"C:\Users\Mikker\Desktop\TS\WARPS\Bin Test\99821022-01.spi");
				}
				//LoadSail(@"C:\Users\Mikker\Desktop\TS\WARPS\Main.spi");
			}

			CurveGroup warps = new CurveGroup("Warps", ActiveSail);
			//warps.Add(new MouldCurve("v1", ActiveSail, new Vect2(0, 0), new Vect2(0.5,0.5), new Vect2(1, 0.6)));
			warps.Add(new MouldCurve("v1", ActiveSail, new Vect2(0, 0), new Vect2(1, 1)));
			warps.Add(new MouldCurve("v2", ActiveSail, new CrossPoint("Foot", "Luff"), new OffsetPoint(.5, ActiveSail.FindCurve("Leech"), 1)));
			warps.Add(new MouldCurve("v3", ActiveSail, new Vect2(0, 0), new Vect2(1, 0)));
			warps.Add(new MouldCurve("Angler", ActiveSail, new FixedPoint(0,0), new AnglePoint(ActiveSail.FindCurve("Leech"), 45)));

			MouldCurve ext = new MouldCurve("Extender", ActiveSail, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(0.4, 0.2), new SlidePoint(ActiveSail.FindCurve("Leech"), 0.5) });
			//Geodesic.FitExtensionGeo(ext, new IFitPoint[] { new FixedPoint(0, 0), new FixedPoint(0.4, 0.2), new SlidePoint(ActiveSail.FindCurve("Leech"), 0.5) });
			warps.Add(ext);



			double scale = 3;
			List<Vect3> rbfss = new List<Vect3>(6);
			//luff
			rbfss.Add(new Vect3(0, 0.5, 1 * scale));
			rbfss.Add(new Vect3(0, 1, 1 * scale));
			//leech
			rbfss.Add(new Vect3(1, 0, 1 * scale));
			rbfss.Add(new Vect3(1, 0.5, 1 * scale));

			rbfss.Add(new Vect3(1, .1, 1 * scale));
			rbfss.Add(new Vect3(.6, .1, 1 * scale));

			rbfss.Add(new Vect3(0, 0, 3 * scale));
			rbfss.Add(new Vect3(0.5, 0.5, 2.5 * scale));
			rbfss.Add(new Vect3(0.7, 0.7, 2.0 * scale));
			rbfss.Add(new Vect3(.85, .9, 1.5 * scale));
			rbfss.Add(new Vect3(1, 1, 1 * scale));

			GuideSurface surf = new GuideSurface("StructSurf", ActiveSail, rbfss);

			//Tapes.TapeGroup struc = new Tapes.TapeGroup("Structural", warps.ToList(), surf, 1, .05, Utilities.DegToRad(3));

			scale = 10;
			rbfss = new List<Vect3>(4);
			//luff
			rbfss.Add(new Vect3(0, 0, 1 * scale));
			rbfss.Add(new Vect3(0, 1, 1 * scale));
			//leech
			rbfss.Add(new Vect3(1, 0, 1 * scale));
			rbfss.Add(new Vect3(1, 1, 1 * scale));

			GuideSurface csurf = new GuideSurface("CompSurf", ActiveSail, rbfss);
			CurveGroup cwarps = new CurveGroup("CompWarps", ActiveSail);
			cwarps.Add(new MouldCurve("c1", ActiveSail, new Vect2(0, 0), new Vect2(1, 0)));
			cwarps.Add(new MouldCurve("c2", ActiveSail, new Vect2(0, 1), new Vect2(1, 1)));

			Tapes.TapeGroup comp = new Tapes.TapeGroup("Compressive", cwarps, surf, 3, .1, Utilities.DegToRad(7));

			//geometry groups
			Mixed.MixedGroup Geom = new Mixed.MixedGroup("Geometry", ActiveSail);
			Geom.Add(new MouldCurve("AllFits", ActiveSail,
				new FixedPoint(0, 0),
				new OffsetPoint(0.25, ActiveSail.FindCurve("Foot"), -1),
				new CrossPoint("Leech", "Bat#6"),
				new CurvePoint(ActiveSail.FindCurve("Bat#1"), 0.5),
				new SlidePoint(ActiveSail.FindCurve("Head"), 0.5)));

			Mixed.MixedGroup Curs = new Mixed.MixedGroup("Curves", ActiveSail);
			Mixed.MixedGroup Surfs = new Mixed.MixedGroup("Surfaces", ActiveSail);
			Geom.Add(Curs);
			Geom.Add(Surfs);

			Curs.Add(warps);
			Curs.Add(cwarps);

			Surfs.Add(surf);
			Surfs.Add(csurf);

			//tape groups
			Mixed.MixedGroup taper = new Mixed.MixedGroup("Tapes", ActiveSail);
			//taper.Add(struc);
			taper.Add(comp);

			//ActiveSail.Add(warps);
			ActiveSail.Add(Geom);
			ActiveSail.Add(taper);

			Mixed.MixedGroup iff = new Mixed.MixedGroup("IF (DPI > 5000)", ActiveSail);
			Mixed.MixedGroup tr = new Mixed.MixedGroup("True", ActiveSail, "Trues");
			Mixed.MixedGroup fa = new Mixed.MixedGroup("False", ActiveSail);
			Mixed.MixedGroup l4 = new Mixed.MixedGroup("High DPI stuff", ActiveSail);
			Mixed.MixedGroup l5 = new Mixed.MixedGroup("Low DPI stuff", ActiveSail);

			ActiveSail.Add(iff);
			iff.Add(tr);
			iff.Add(fa);
			tr.Add(new CurveGroup("TrueCurves", ActiveSail));
			fa.Add(new CurveGroup("FalseCurves", ActiveSail));
			tr.Add(new Mixed.MixedGroup("IF (REEFSPLS)", ActiveSail, "Curves"));
			(tr[1] as Mixed.MixedGroup).Add(new Mixed.MixedGroup("True", ActiveSail, "Trues"));
			((tr[1] as Mixed.MixedGroup)[0] as Mixed.MixedGroup).Add(new YarnGroup("ReefGrp", ActiveSail, 1000));
			//ActiveSail.Add(cTapes);
			ActiveSail.Rebuild();

			//UpdateViews(ActiveSail.CreateOuterCurves());
			UpdateViews(Geom);
			UpdateViews(taper);
			//UpdateViews(struc);
			//UpdateViews(cTapes);
			//this.WindowState = FormWindowState.Maximized;
			View.ZoomFit(true);
			//View.Refresh();

		}


		//List<Entity[]> m_dpiTents = null;
		private async void m_dpiButton_Click(object sender, EventArgs e)
		{
			DensityMesh dpi = new DensityMesh();
			IRebuild old = ActiveSail.Add(dpi) as DensityMesh;
			if (old != null && old is DensityMesh)
				dpi = old as DensityMesh;

			DateTime start = DateTime.Now;

			await Task.Factory.StartNew(() =>
			{
				if (ModifierKeys == Keys.Control)
					dpi.DelaunayMesh(ActiveSail);
				else
					dpi.MeshSail(ActiveSail);
			});
			TimeSpan mesh = DateTime.Now - start;

			UpdateStatusStrip((dpi.Label ?? dpi.GetType().Name) + " " + mesh.ToString());
			ActiveSail.WriteNode();
			UpdateViews(dpi);

			//if (m_dpiTents != null)
			//	View.RemoveRange(m_dpiTents);
			//m_dpiTents = View.Add(dpi, false);
			//View.Refresh();
		}

		#endregion
	}

}
