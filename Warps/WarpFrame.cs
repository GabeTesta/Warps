using System;
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
using Warps.Logger;
using Warps.Yarns;
using Warps.Trackers;
using System.Threading;

namespace Warps
{
	public delegate void ObjectSelected(object sender, EventArgs<IRebuild> e);

	public delegate void VisibilityToggled(object sender, EventArgs<IRebuild> e);

	public delegate void WriteStatus(string status);

	public partial class WarpFrame : Form
	{
		public WarpFrame()
		{
			InitializeComponent();
#if DEBUG
			logger.Instance.CreateLogLocal("Warps");
			logger.Instance.Log("new instance loaded", LogPriority.Debug);
#endif
			//set background color from existing icon
			//ButtonUnSelected = m_modCurve.BackColor;

			SetStyle(ControlStyles.OptimizedDoubleBuffer |
				    ControlStyles.AllPaintingInWmPaint, true);

			EditorPanel = null;//collapse edit panel

			//toggle edit mode to ensure coloring
			m_editButton.Checked = false;
			m_editButton.Checked = true;//disable edit mode

			//toggle auto mode to ensure coloring
			m_autoBtn.Checked = true;
			m_autoBtn.Checked = false;//enable automode

			m_tree.AfterSelect += m_tree_AfterSelect;
			View.SelectionChanged += m_tree_AfterSelect;
			cancelButton.Click += cancelButton_Click;

			Tree.VisibilityToggle += View.VisibilityToggled;

			m_horizsplit.SplitterDistance = m_horizsplit.ClientRectangle.Width - 250;

		}

		public string Status
		{
			get { return m_statusText.Text; }
			set { m_statusText.Text = value; }
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
			ofd.Filter = "sail files (*.sail)|*.sail|cof files (*.cof)|*.cof|warp files (*.wrp)|*.wrp|All files (*.*)|*.*";
			ofd.Multiselect = true;
			ofd.FilterIndex = Math.Min(extension, 3);
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return ofd.FileNames;
			return null;
		}

		void LoadSail(string path)
		{
			Status = String.Format("Loading {0}", path);

			Sail s = new Sail();

			s.ReadFile(path);

			m_sail = s;
			//if (s.Layout == null || s.Layout.Count == 0)
			//	CreateOuterCurves(s);
			//else

			m_tree.Add(s.WriteNode());

			AddSailtoView(s);
			Tree.ExpandToDepth(0);

			//EquationEditor.Instance.SetSail(s);

			//s.Rebuild(null);
			Status = String.Format("{0} Loaded Successfully", path);
		}

		private void AddSailtoView(Sail s)
		{
			int nlayer = View.AddLayer("Mould", Color.Beige, true);
			s.Mould.CreateEntities(null, false).ForEach(ent => { ent.LayerIndex = nlayer; View.Add(ent); });

			nlayer = View.AddLayer("Gauss", Color.Beige, false);
			s.Mould.CreateEntities(new double[,] { { -.2, 1.2 }, { -.2, 1.2 } }, true).ForEach(ent => { ent.LayerIndex = nlayer; View.Add(ent); });

			s.Layout.ForEach(group => View.Add(group));

			View.ZoomFit(true);
		}

		#endregion

		#region Rebuild

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
						if (item.Update(ActiveSail))
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
						View.Invalidate(item);
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
				UpdateViews(tag);
			return AutoBuild;
		}

		//private void parallelRebuild(List<IRebuild> updated)
		//{
		//	Parallel.ForEach<IRebuild>(updated, item =>
		//	{
		//		item.Update();
		//	});
		//}

		private void UpdateViews(IRebuild item)
		{
			View.Remove(item);

			Tree.BeginUpdate();

			if (item is IGroup)
			{
				View.Add(item as IGroup);
				(item as IGroup).WriteNode();
			}
			else if (item is MouldCurve)
			{
				View.Add(item as MouldCurve);
				(item as MouldCurve).WriteNode();
			}
			else if (item is Equation)
			{
				(item as Equation).WriteNode();
			}
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
				EditMode = false;
				return connected.Count;
			}

			return -1;
		}

		public bool AutoBuild
		{
			get { return !m_autoBtn.Checked; }
		}
		private void m_autoBtn_CheckedChanged(object sender, EventArgs e)
		{
			m_autoBtn.BackColor = AutoBuild ? ButtonSelected : ButtonUnSelected;
			m_autoBtn.ForeColor = AutoBuild ? Color.White : Color.Black;
		}

		private void m_buildBtn_Click(object sender, EventArgs e)
		{
			//if (Tracker != null)
			//{
			//	Tracker = null;
			//	EditPanel = null;
			//}
			//m_modCurve.BackColor = ButtonUnSelected;
			if (Tree.SelectedTag is IRebuild)
				Rebuild(Tree.SelectedTag as IRebuild);
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

		#region Toolbar New/Open/Save

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			OpenFile(1);
		}
		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			OpenFile(3);
		}
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			//Sail s;
			//if (m_sails != null && m_sails.Count > 0)
			//	s = m_sails[0];
			//else
			//	return;
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.DefaultExt = ".wrp";
			sfd.AddExtension = true;
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
#if DEBUG
				logger.Instance.Log("saving file: " + sfd.FileName, LogPriority.Debug);
#endif
				ActiveSail.WriteScriptFile(sfd.FileName);
			}
			//	m_tree.SaveScriptFile(sfd.FileName);
		}

		#endregion

		private void m_addCurve_Click(object sender, EventArgs e)
		{
			if (ActiveSail == null)
				return;
			AddGroup dlg = new AddGroup();
			dlg.Name = "enter name";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IGroup grp = dlg.CreateGroup();
				if (grp != null)
				{
					ActiveSail.Add(grp);
					ActiveSail.Rebuild(null);
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
		public void m_tree_AfterSelect(object sender, EventArgs<IRebuild> e)
		{
			if ( (sender == Tree || sender == View) && m_Tracker != null && m_Tracker.EditMode)
				return; //dont do anything if we are already edit-tracking

			Status = "";
			View.DeSelectAll();

			ITracker track = null;

			if (e.Value is MouldCurve)
			{
				if (e.Value is GuideComb)
					track = new GuideCombTracker(e.Value as GuideComb);
				else
					track = new CurveTracker(e.Value as MouldCurve);
			}
			else if (e.Value is IGroup)
			{
				switch (e.Value.GetType().Name)
				{
					case "CurveGroup":
						track = new CurveGroupTracker(e.Value as CurveGroup);
						break;

					case "VariableGroup":
						track = new VariableGroupTracker(e.Value as VariableGroup);
						break;

					case "YarnGroup":
						track = new YarnGroupTracker(e.Value as YarnGroup);
						break;
				}

			}
			else if (e.Value is Equation)
			{
				IGroup parent = ActiveSail.FindGroup(e.Value);
				if (parent != null)
					track = new VariableGroupTracker(parent as VariableGroup);

			}
			else// if (e.Value is Sail)
				track = new SailTracker(EditMode);


			if (track != null)
			{
#if DEBUG
				logger.Instance.Log(String.Format("Creating new {0} from {1}", track.GetType().Name, e.Value == null ? "null" : e.Value.GetType().Name), LogPriority.Debug);
#endif
				PostTracker(track);
			}

		}

		public bool EditMode
		{
			get { return !m_editButton.Checked; }
			set
			{
				m_editButton.Checked = !value;
				m_editButton_CheckedChanged(this, new EventArgs());
			}
		}
		private void m_editButton_CheckedChanged(object sender, EventArgs e)
		{
			m_editButton.BackColor = EditMode ? ButtonSelected : ButtonUnSelected;
			m_editButton.ForeColor = EditMode ? Color.White : Color.Black;

			if (m_Tracker != null)
			{
				ITracker tracker = m_Tracker;
				ClearTracker();
				tracker.EditMode = EditMode;
				PostTracker(tracker);
			}

			//cancelButton.Enabled = EditMode;

			//okButton.Enabled = EditMode;
			//previewButton.Enabled = EditMode;

			//if (m_Tracker != null)
			//{
			//	m_Tracker.EditMode = EditMode;
			//	if (EditMode)
			//	{
			//		okButton.Click += m_Tracker.OnBuild;
			//		cancelButton.Click += m_Tracker.OnCancel;
			//		previewButton.Click += m_Tracker.OnPreview;
			//	}
			//	else
			//	{
			//		okButton.Click -= m_Tracker.OnBuild;
			//		cancelButton.Click -= m_Tracker.OnCancel;
			//		previewButton.Click -= m_Tracker.OnPreview;
			//	}
			//}
		}
		void cancelButton_Click(object sender, EventArgs e)
		{
			ClearTracker();
		}
		private void PostTracker(ITracker tracker)
		{
			if (EditMode)
				EditTracker(tracker);
			else
				ReadonlyTracker(tracker);
		}

		ITracker m_Tracker;
		ITracker EditTracker(ITracker tracker)
		{
			if (tracker == null)
				return null;

			if (m_Tracker != null && m_Tracker.EditMode)
				ClearTracker();

			Status = "Editing " + tracker.GetType().Name;
			m_Tracker = tracker;//post the new tracker

			m_Tracker.Track(this);
			okButton.Click += m_Tracker.OnBuild;
			//cancelButton.Click += m_Tracker.OnCancel;
			previewButton.Click += m_Tracker.OnPreview;

			return m_Tracker;//return it
		}
		ITracker ReadonlyTracker(ITracker tracker)
		{
			if (tracker == null)
				return null;

			if (m_Tracker != null && m_Tracker.EditMode)
				return m_Tracker;//dont overwrite an existing edit-mode tracker

			if (m_Tracker != null)//cancel any exising readonly-tracker
				ClearTracker();

			Status = "Inspecting " + tracker.GetType().Name;
			m_Tracker = tracker;//post the new tracker
			m_Tracker.Track(this);
			return m_Tracker;//return it
		}
		internal void ClearTracker()
		{
			if (m_Tracker == null)
			{
				EditorPanel = null;
				return;
			}

			if (m_Tracker != null)
				m_Tracker.OnCancel(null, null);//clear any existing tracker

			okButton.Click -= m_Tracker.OnBuild;
			//cancelButton.Click -= m_Tracker.OnCancel;
			previewButton.Click -= m_Tracker.OnPreview;

			m_Tracker = null;
			EditorPanel = null;
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
				else
					m_horizsplit.Panel2Collapsed = true;

				editPanel.ResumeLayout();

			}
		}

		#region Ok/Cancel/Preview Buttons

		private void okButton_MouseEnter(object sender, EventArgs e)
		{
			//(sender as Button).ForeColor = Color.White;
			if (sender == okButton)
				okButton.BackColor = Color.SeaGreen;
			else if (sender == previewButton)
				previewButton.BackColor = Color.LightSkyBlue;
		}
		private void okButton_MouseLeave(object sender, EventArgs e)
		{
			if (sender is Button) (sender as Button).BackColor = Color.White;
		}

		#endregion

		#endregion

		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
			if (ActiveSail == null)
				return;

			if (Tree.SelectedTag != null)
			{
				IRebuild tag = Tree.SelectedTag as IRebuild;
				List<IRebuild> rebuilds = new List<IRebuild>();
				if (tag != null)
					tag.GetParents(ActiveSail, rebuilds);
				
				StringBuilder sb = new StringBuilder();
				foreach (IRebuild rb in rebuilds)
					sb.AppendLine(rb.Label);
				MessageBox.Show(sb.ToString());
			return;
			}


			VariableGroup varGroup = new VariableGroup("Vars", ActiveSail);
			varGroup.Add(new Equation("yarScale", 1.0));
			varGroup.Add(new Equation("yarnDPI", "yarScale * 12780"));
			varGroup.Add(new Equation("targetScale", 1.0));
			varGroup.Add(new Equation("targetDPI", "targetScale * 14416"));
			ActiveSail.Add(varGroup);

			UpdateViews(ActiveSail.CreateOuterCurves());

			//Geodesic geo = new Geodesic("Geo", ActiveSail, new IFitPoint[] { new FixedPoint(.1, .1), new FixedPoint(.1, .9) });
			MouldCurve v1 = new MouldCurve("v1", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.3, .4), new FixedPoint(.1, .8), new FixedPoint(0, 1) });

			MouldCurve v2 = new MouldCurve("v2", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(0, 1) });

			MouldCurve v3 = new MouldCurve("v3", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.95, .25), new FixedPoint(.9, .55), new FixedPoint(.65, .85), new FixedPoint(0, 1) });
			//MouldCurve v4 = new MouldCurve("v4", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(.8, .5), new FixedPoint(1, 1) });
			//MouldCurve v5 = new MouldCurve("v5", ActiveSail, new IFitPoint[] { new FixedPoint(1, 0), new FixedPoint(1, 1) });
			CurveGroup grp = new CurveGroup("Warps", ActiveSail);
			grp.Add(v1);
			grp.Add(v2);
			grp.Add(v3);
			grp.Add(new MouldCurve("g3", ActiveSail,
				new IFitPoint[] { 
					new FixedPoint(0,0), 
					new SlidePoint(v1, 0), 
					new FixedPoint(1,.5) }));
			grp.Add(new MouldCurve("g4", ActiveSail,
				new IFitPoint[] { 
					new FixedPoint(1, 0), 
					new FixedPoint(.4, .4), 
					new FixedPoint(.3, .7),
					new FixedPoint(0,1)}));
			//grp.Add(v4);
			//grp.Add(v5);
			//grp.Add(guide);

			CurveGroup guides = new CurveGroup("Guides", ActiveSail);
			GuideComb guide = new GuideComb("Guide", ActiveSail,
				new IFitPoint[] {
					new FixedPoint(0, .5), 
					new SlidePoint(v2, .5),
					new FixedPoint(1, .5) },
				new Vect2[] { 
					new Vect2(0, 1), 
					new Vect2(.3, .55),
					new Vect2(.5, .5), 
					new Vect2(.7, .55), 
					new Vect2(1, 1) });
			guides.Add(guide);

			YarnGroup yar = new YarnGroup("yar1", ActiveSail, varGroup["yarnDPI"], varGroup["targetDPI"]);
			yar.Warps.Add((ActiveSail.FindGroup("Outer") as CurveGroup)[0]);
			yar.Warps.Add((ActiveSail.FindGroup("Outer") as CurveGroup)[1]);
			yar.Guide = guide;
			yar.DensityPos = new List<double>() { 0.2, 0.8 };
			ActiveSail.Add(grp);
			ActiveSail.Add(guides);
			ActiveSail.Add(yar);
			UpdateViews(grp);
			UpdateViews(guides);


			//YarnGroup LuYar = new YarnGroup("LuYar", ActiveSail, 12780);
			//LuYar.DensityPos.AddRange(new double[] { 0.25, 0.5, 0.75 });
			//LuYar.YarnsUpdated += LuYar_YarnsUpdated;
			////if (LuYar.LayoutYarns(new List<MouldCurve>() { lu, mi, le }, guide, 14416) > 0)
			////DateTime now = DateTime.Now;
			////LuYar.LayoutYarns(grp, guide, 14416, LuYar.SpreadYarnsAlongGuide);
			////TimeSpan gde = DateTime.Now - now;
			////now = DateTime.Now;

			////LuYar.LayoutYarns(grp, guide, 14416, LuYar.SpreadYarnsAcrossWarps);
			////TimeSpan wrps = DateTime.Now - now;
			////now = DateTime.Now;
			////MessageBox.Show(string.Format("AcrossWarps: {0}\nAlongGuide: {1}", wrps.TotalMilliseconds, gde.TotalMilliseconds));

			UpdateViews(guides);
			UpdateViews(grp);
			yar.Update(ActiveSail);
			UpdateViews(yar);
			//if (LuYar.LayoutYarns(grp, guide, 14416) > 0
			//	|| MessageBox.Show(String.Format("Failed to match Target Dpi\nTarget: {0}\nAchieved: {1}\nContinue Anyway?", LuYar.TargetDpi, LuYar.AchievedDpi), "Yarn Generation Failed", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes )
			//	ActiveSail.Add(LuYar);

			////Yarns.YarnGroup LeYar = new Yarns.YarnGroup("LeYar", ActiveSail, 12780);
			////if (LeYar.LayoutYarns(new List<MouldCurve>() { mi, le }, guide, 14416) > 0)
			////	ActiveSail.Add(LeYar);

			////Rebuild(null);

			//UpdateViews(LuYar);
			//Rebuild(grp);
			//Rebuild(grp);
			//Rebuild(guides);
			//Rebuild(LuYar);
			View.Refresh();
			ActiveSail.Rebuild(null);
		}

		void LuYar_YarnsUpdated(object sender, EventArgs<YarnGroup> e)
		{
			UpdateViews(e.Value);
			View.Refresh();
		}

		private void clearAll_Click(object sender, EventArgs e)
		{
			if (DialogResult.Yes == MessageBox.Show("Are you sure you want to clear all?", String.Format("Warps v{0}", Utilities.CurVersion), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
			{
				Tree.ClearAll();
				View.ClearAll();
				m_sail.Layout.Clear();
				m_sail = null;
				EditorPanel = null;
			}
		}

		private void printToolStripButton_Click(object sender, EventArgs e)
		{
			//Write 3dl file
			logger.Instance.Log("Saving project to 3dl file");
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.DefaultExt = ".3dl";
			dlg.AddExtension = true;
			dlg.Filter = "3dl files (*.3dl)|*.3dl|All files (*.*)|*.*";
			dlg.InitialDirectory = Utilities.ExeDir;
			if (dlg.ShowDialog() == DialogResult.OK)
				Save3dlFile(dlg.FileName);
			
		}

		void Save3dlFile(string fullfilename)
		{
			logger.Instance.Log("saving 3dl file...");
			Thread tw = new Thread(() =>
			{
				logger.Instance.Log("saving {0} to {1}...", Path.GetFileName(fullfilename), Path.GetDirectoryName(fullfilename));
				using (StreamWriter sw = new StreamWriter(fullfilename))
				{
					//write header
					//OUS102439-001, Fat26, EnergySolution ITA14313, Main ORCi, 10850 dpi, 3Dl 680, Capitani/NSI,//3DLayOut_Release 1.1.0.171
					sw.WriteLine("{0} //Warps v{1}", ActiveSail.ToString(), Utilities.CurVersion);
					foreach (YarnGroup yar in ActiveSail.Layout.FindAll(grp => grp is YarnGroup))
					{
						Entity[] ents = yar.CreateOnlyYarnEntities();

						double sPos = 0;
						Vect2 uv = new Vect2(); Vect3 xyz = new Vect3();
						for (int i = 0; i < yar.Count; i++)
						{
							//FOOT   1.0000   FT_IN  0.0000  spacing  0.0853    48 offsets on yarn #1
							sw.WriteLine("{0}   {1}   {2}  {3}  {4}  {5}    {6} offsets on yarn #{7}"
									, yar.Label, 1.0, "name", 0, "spacing", 0, ents[i].Vertices.Length-1, i);

							for (int j = 0; j < ents[i].Vertices.Length; j++)
							{
								sPos = (double)j / (double)(ents[i].Vertices.Length - 1);
								yar[i].uVal(sPos, ref uv);

								sw.WriteLine(" {0:#0.00000} {1:#0.00000}    {2}  {3:#0.000000}  {4:#0.000000}  {5:#0.000000}  {6:#0.000000}  {7:#0.000000}  {8:#0.000000}  {9:#0.000000}",
									uv[0], uv[1], j
									, ents[i].Vertices[j].X, ents[i].Vertices[j].Y, ents[i].Vertices[j].Z
									, 0, 0, 0
									, 0);
								//0.98647-0.00093    0  3.726569 -0.020588  0.007790  0.155863  0.003351  0.987773  0.000000
							}
						}

					}
					//read the yarn points from the entities
					//out s, uv, xyz
					//just use 100 points for now
					//createlinearpath
					//this will be what we used eventually
					//List<Entity> CreateEntities(bool bFitPoints, double TolAngle, out double[] sPos)
					//for now use 1/100 evenly spaced sPos

					/*
					-we need the header to tokenize the layout version
						tokenized with "//"
	  
					- we tokenize each yarns header to get the count
						currently written on index 6 (int count = atoi(tmp[6].c_str()))
	   
					///function that parses each line of the yarn.3dl file
					void	YarnGantPass::SplitLine(std::string line)
					{
						std::vector<std::string> shifter;
						std::vector<int> widths;
						widths.push_back(8);
						widths.push_back(8);
						widths.push_back(5);
						widths.push_back(10);
						widths.push_back(10);
						widths.push_back(10);
						widths.push_back(10);
						widths.push_back(10);
						widths.push_back(10);
						widths.push_back(10);

						shifter = Tokenize(line, widths);

						AddUV(atof(shifter[0].c_str()),atof(shifter[1].c_str()));
						AddXYZ(atof(shifter[3].c_str()),atof(shifter[4].c_str()),atof(shifter[5].c_str()));
					}
					 
					  OUS102439-001, Fat26, EnergySolution ITA14313, Main ORCi, 10850 dpi, 3Dl 680, Capitani/NSI,//3DLayOut_Release 1.1.0.171
					  FOOT   1.0000   FT_IN  0.0000  spacing  0.0853    48 offsets on yarn #1
					  0.98647-0.00093    0  3.726569 -0.020588  0.007790  0.155863  0.003351  0.987773  0.000000
					 */
				}
				logger.Instance.Log("saving done");
			});
			tw.Start();
			
		}
	}
}
