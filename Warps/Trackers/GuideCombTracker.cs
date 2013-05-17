using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warps.Controls;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Windows.Forms;
using System.Drawing;

namespace Warps.Trackers
{
	public class GuideCombTracker: ITracker
	{
		public GuideCombTracker(GuideComb comb)
		{
			m_guide = comb;
			m_edit = new GuideEditor(this);
		}

		public void Track(WarpFrame frame)
		{
			if (frame == null)
				return;

			m_frame = frame;
			//m_frame.okButton.Click += OnBuild;
			//m_frame.cancelButton.Click += OnCancel;
			//m_frame.previewButton.Click += OnPreview;

			m_frame.EditorPanel = Edit;
			EditMode = frame.EditMode;

			CreateTreePopup();
			Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v
			Tree.ContextMenuStrip.Opening += TreePopup_Opening;

			View.AttachTracker(this);
			//if (Curve == null && EditMode)
			//	View.StartSelect();
			if (Comb != null)
				SelectCurve(Comb);
		}

		GuideComb m_guide;
		GuideEditor m_edit;
		WarpFrame m_frame;

		public GuideComb Comb
		{
			get { return m_guide; }
			set { m_guide = value; }
		}
		GuideEditor Edit
		{
			get { return m_edit; }
		}
		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		}
		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}

		private void SelectCurve(GuideComb cur)
		{
			if (cur == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp);

			Comb = cur;
			
			IFitPoint[] pts = new IFitPoint[Comb.FitPoints.Length];
			for (int i = 0; i < pts.Length; i++)
				pts[i] = Comb[i].Clone();

			m_temp = new GuideComb(cur.Label + "[preview]", Comb.Sail, pts, Comb.CombPnts);

			m_tents = View.AddRange(m_temp.CreateEntities(true));

			foreach (Entity[] ents in m_tents)
				foreach (Entity ee in ents)
				{
					ee.Color = Color.LightSkyBlue;
					ee.ColorMethod = colorMethodType.byEntity;
				}

			//PointTypeSwitcher.SetCurves(mc);
			//PointTypeSwitcher.SetAutofill(Sail.GetAutoFillData(Comb));
			//PointTypeSwitcher.SetSail(Sail);

			Edit.AutoFill = Sail.Watermark(Comb).ToList<object>();
			Edit.ReadComb(m_temp);
			Edit.Label = Comb.Label;
			Edit.Refresh();

			//if( EditMode ) View.StopSelect();
			//Edit.Label = Comb.Label;
			//Edit.Length = m_temp.Length;
			////Edit.FitPoints = m_temp.FitPoints;
			//Edit.CombPnts = m_temp.CombPnts;
			//Edit.Refresh();
			if (Tree.SelectedTag != Comb) Tree.SelectedTag = Comb;
			//if (View.SelectedTag != Curve) 
			View.Select(Comb);
			//View.SelectLayer(Curve);
			View.Refresh();
		}

		GuideComb m_temp;
		Entity[][] m_tents;
		int m_index = -1;

		bool m_editMode = false;
		public bool EditMode
		{
			get { return m_editMode; }
			set
			{
				m_editMode = value;
				if (Edit != null)
					Edit.Enabled = value;

				if (View != null)
				{
					View.EditMode = value;
					View.DeSelectAllLayers();
					if (Comb != null)
						SelectCurve(Comb);
				}
				//if (Tree != null)
				//	Tree.EditMode = value;

				//if (value == false)
				//	Tree.DeSelect(Comb);
			}
		}

		#region TreePopup

		void Tree_KeyUp(object sender, KeyEventArgs e)
		{
			// the modifier key CTRL is pressed by the time it gets here
			switch (e.KeyCode)
			{
				case Keys.C:
					OnCopy(Tree.SelectedTag, new EventArgs());
					break;
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}

		private void CreateTreePopup()
		{
			if (Tree != null)
			{
				Tree.ContextMenuStrip = new ContextMenuStrip();

				Tree.ContextMenuStrip.Items.Add("Show Only", new Bitmap(Warps.Properties.Resources.showonly), OnShowOnlyClick);
				Tree.ContextMenuStrip.Items.Add("Visible", new Bitmap(Warps.Properties.Resources.SmallEye), OnVisibleToggleClick);
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Copy Curve", new Bitmap(Warps.Properties.Resources.copy), OnCopy);
				Tree.ContextMenuStrip.Items.Add("Paste Curve", new Bitmap(Warps.Properties.Resources.paste), OnPaste);
				Tree.ContextMenuStrip.Items[Tree.ContextMenuStrip.Items.Count - 1].Enabled = ClipboardContainsComb();
				Tree.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				Tree.ContextMenuStrip.Items.Add("Delete", new Bitmap(Warps.Properties.Resources.glyphicons_192_circle_remove), DeleteClick);

				Tree.ContextMenuStrip.Opening += TreePopup_Opening;
			}

		}


		void DeleteClick(object sender, EventArgs e)
		{
			if (EditMode)
				return;

			View.Remove(Comb);
			CurveGroup g = Comb.Group as CurveGroup;
			if (g != null)
			{
				g.Remove(Comb);
				m_frame.Rebuild(g);
			}
			m_frame.Delete(Comb);
			if (g != null)
				Tree.SelectedTag = g;
		}

		private void DeleteTreePopup()
		{
			if (Tree.ContextMenu != null)
			{
				Tree.ContextMenuStrip.Opening -= TreePopup_Opening;
				Tree.ContextMenuStrip.Items.Clear();
				Tree.ContextMenuStrip = null;
			}
		}
		void TreePopup_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.ContextMenuStrip.Items.Count; i++)
				if (Tree.ContextMenuStrip.Items[i].Text == "Paste Curve")
					Tree.ContextMenuStrip.Items[i].Enabled = ClipboardContainsComb();
			Tree.ContextMenuStrip.Show();
			//Tree.ContextMenuStrip.Items["Paste Curve"].Enabled = ClipboardContainsCurve();
		}

		void OnVisibleToggleClick(object sender, EventArgs e)
		{
			View.ToggleLayer(m_guide);

		}
		void OnShowOnlyClick(object sender, EventArgs e)
		{
			View.ShowOnly(m_guide);

		}

		void UpdateViewCurve(bool bEditor)
		{
			m_temp.ReFit();
			List<Entity> verts = m_temp.CreateEntities(true).ToList();
			int nMsh = verts.Count;
			for( nMsh = 0; nMsh < verts.Count; nMsh++ )
				if( verts[nMsh] is Mesh) break;

			foreach (Entity[] ents in m_tents)
			{
				for (int i = 0; i < 2; i++)
				{
					if (ents[i] is LinearPath)
						ents[i].Vertices = verts[0].Vertices;
					else if (ents[i] is PointCloud)
						ents[i].Vertices = verts[1].Vertices;
					else if (ents[i] is Mesh && nMsh < verts.Count)
						SurfaceTools.UpdateMesh(ents[i] as Mesh, verts[nMsh] as Mesh);
				}
			}
			View.Regen();
			View.Refresh();
			if (bEditor)
			{
				Edit.ReadComb(m_temp);
				Edit.Label = Comb.Label;
				Edit.Update();


				//bEditor = Edit.Enabled;//store the exising edit state
				//Edit.Enabled = true;//set enabled to allow controls to update

				//Edit.Count = m_temp.FitPoints.Length;
				//for (int i = 0; i < m_temp.FitPoints.Length; i++)
				//{
				//	m_temp[i].WriteEditor(Edit[i]);
				//	Edit[i].Invalidate();
				//}
				//Edit.Length = m_temp.Length;
				//Edit.CombPnts = m_temp.CombPnts;
				//Edit.Update();

				//Edit.Enabled = bEditor;//restore previous edit state
			}
		}
		public void Cancel()
		{
			//m_frame.okButton.Click -= OnBuild;
			//m_frame.cancelButton.Click -= OnCancel;
			//m_frame.previewButton.Click -= OnPreview;

			m_frame.EditorPanel = null;

			DeleteTreePopup();
			Tree.KeyUp -= Tree_KeyUp; // handle ctrl-c ctrl-v

			View.DetachTracker(this);
			if (m_temp != null)
				View.Remove(m_temp);
			View.DeSelect(Comb);
			//View.StopSelect();
			View.Refresh();
		}
		void ReadEditor()
		{
			//List<IFitPoint> pnts = new List<IFitPoint>();
			//for (int i = 0; i < Edit.Count; i++)
			//{
			//	object fit = null;
			//	if (Edit[i] != null)
			//		fit = Utilities.CreateInstance(Edit[i].FitType.Name);
			//	if (fit != null && fit is IFitPoint)
			//	{
			//		pnts.Add(fit as IFitPoint);
			//		pnts.Last().ReadEditor(Edit[i]);
			//	}
			//}
			//m_temp.FitPoints = pnts.ToArray();
			Edit.WriteComb(m_temp);
			//m_temp.CombPnts = Edit.CombPnts;
			m_temp.FitComb(Edit.CombPnts);
		}

		#endregion
		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			//throw new NotImplementedException();
		}
		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
				return;
			if (Control.ModifierKeys == Keys.Control)
			{
				if (m_index >= 0)
				{
					if (m_temp.RemovePoint(m_index))
						UpdateViewCurve(true);
				}
				else
				{
					//get mouse point in uv, find closest curve point, add fit point there.
					System.Drawing.Point mpt = new System.Drawing.Point(e.X, View.ActiveView.Height - e.Y);
					//mpt = View.ActiveView.PointToScreen(mpt);
					//Point3D target = View.ActiveView.ScreenToWorld(mpt);
					int i;
					//if ( target != null && m_temp.InsertPoint(new Vect3(target.ToArray()), out i))
					if (m_temp.InsertPoint(mpt, View.ActiveView.WorldToScreen, out i))
					{
						UpdateViewCurve(true);
						//Vect3 xyz = new Vect3();
						//m_temp.xVal(m_temp.FitPoints[i].UV, ref xyz);
						//Point3D target = View.ActiveView.ScreenToWorld(mpt);
						//LinearPath p = new LinearPath(target, new Point3D(xyz.ToArray()));
						//p.Color = Color.HotPink;
						//p.ColorMethod = colorMethodType.byEntity;
						//View.Add(p);
					}
				}

			}
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
				return;

			if (!EditMode)
				return;

			if (sender is ViewportLayout)
			{
				Point3D vert;
				PointF m_mousePnt = (PointF)e.Location;
				m_mousePnt.Y = View.ActiveView.Height - m_mousePnt.Y;

				int nview = View.ActiveViewIndex;
				for (int i = 0; i < m_tents.Length; i++)
				{
					if (m_tents[i][nview] is PointCloud)
					{
						for (int nVert = 0; nVert < m_tents[i][nview].Vertices.Length; nVert++)
						{
							vert = View.ActiveView.WorldToScreen(m_tents[i][nview].Vertices[nVert]);
							double dis = Math.Pow(vert.X - m_mousePnt.X, 2) + Math.Pow(vert.Y - m_mousePnt.Y, 2);
							if (dis < Math.Pow(10, 2))
							{
								m_index = nVert;
								break;
							}
						}
					}
				}
			}
			else if (sender is NsPlot)
			{
				PickComb(sender, e);
			}
		}
		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left || m_index == -1)
				return;
			if (!EditMode)
				return;

			if (sender is ViewportLayout)
			{
				Transformer wts = View.ActiveView.WorldToScreen;
				PointF mpt = new PointF(e.X, View.ActiveView.Height - e.Y);
				if (!m_temp.DragPoint(m_index, mpt, wts))
				{
					m_index = -1;
				}
				else
				{
					UpdateViewCurve(true);
					//m_temp[m_index].WriteEditor(Edit[m_index]);
					//Edit[m_index].Refresh();
					//for (int i = 0; i < m_temp.FitPoints.Length; i++)
					//{
					//	m_temp[i].WriteEditor(Edit[i]);
					//	Edit[i].Refresh();
					//}
					//Edit.Refresh();
					//m_mousePnt = new PointF(e.X, Viewport.Height - e.Y);
				}		
			}
			else if (sender is NsPlot)
			{
				DragComb(sender, e);
			}
		}	
		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null)
				return;
			if (!EditMode)
				return;
			if (e.Button == MouseButtons.Left)
			{
				m_index = -1;
			}
		}

		private void PickComb(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control)
				return;//dont drag on shift or ctrl drag
			if (e.Button == MouseButtons.Left)
			{
				NsPlot plot = sender as NsPlot;
				double x = plot.PhysicalXAxis1Cache.PhysicalToWorld(e.Location, true);
				double y = plot.PhysicalYAxis1Cache.PhysicalToWorld(e.Location, true);

				int i = 0;
				foreach (Vect2 v in m_edit.CombPnts)
				{
					if (BLAS.is_equal(v.u, x, .02))
					{
						m_index = i;
						break;
					}
					i++;
				}
			}
		}
		private void DragComb(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || m_index == -1)
				return;//dont drag on shift or ctrl drag
			if (e.Button == MouseButtons.Left)
			{
				NsPlot plot = sender as NsPlot;
				double x = plot.PhysicalXAxis1Cache.PhysicalToWorld(e.Location, true);
				double y = plot.PhysicalYAxis1Cache.PhysicalToWorld(e.Location, true);

				//enforce endpoints
				if (m_index == 0)
					x = 0;
				if (m_index == m_temp.CombPnts.Length - 1)
					x = 1;

				m_temp.CombPnts[m_index].u = Utilities.LimitRange(0, x, 1);//unit-length
				m_temp.CombPnts[m_index].v = Utilities.LimitRange(0, y, 1);//unit-length

				m_temp.FitComb(null);
				UpdateViewCurve(true);
			}
		}

		public void OnCopy(object sender, EventArgs e)
		{
			GuideComb group = Tree.SelectedTag as GuideComb;

			if (group == null)
				return;
			//Lets say its my data format
			Clipboard.Clear();
			//Set data to clipboard
			Clipboard.SetData(group.GetType().Name, Utilities.Serialize(group.WriteScript()));
			//Get data from clipboard
			m_frame.Status = String.Format("{0}:{1} Copied", group.GetType().Name, group.Label);

		}
		public void OnPaste(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (m_temp == null || !EditMode)
				return;
			//Detach();
			OnPreview(sender, null);
			//View.Remove(m_temp);
			View.Remove(Comb);

			Comb.Fit(m_temp.FitPoints);
			Comb.FitComb(m_temp.CombPnts);
			Comb.Label = Edit.Label;

			if (sender != null)
				m_frame.Rebuild(Comb);//returns false if AutoBuild is off

			View.Refresh(); 
		}
		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}
		public void OnPreview(object sender, EventArgs e)
		{
			if (m_temp == null || !EditMode)
				return;
			//if (e.Value >= 0)
			//{
			//	m_temp[e.Value].ReadEditor(Edit[e.Value]);
			//	m_temp[e.Value].WriteEditor(Edit[e.Value]);
			//}
			//else
			ReadEditor();
			UpdateViewCurve(true);
		}

		bool ClipboardContainsComb()
		{
			return Clipboard.ContainsData(typeof(GuideComb).Name);
		}
	}
}
