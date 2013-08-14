using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Windows.Forms;
using System.Drawing;
using Warps.Controls;
using Logger;

namespace Warps
{
	public class CurveTracker : ITracker
	{
		public CurveTracker(MouldCurve curve)
		{
			m_curve = curve;
		}

		public void Track(WarpFrame frame)
		{
			if (frame == null)
				return;

			m_frame = frame;

			m_frame.EditorPanel = m_edit;
			EditMode = frame.EditMode;

			if (Tree != null)
			{
				Tree.KeyUp += Tree_KeyUp; // handle ctrl-c ctrl-v	
				Tree.TreeContextMenu.Opening += ContextMenuStrip_Opening;
				Tree.AttachTracker(this);
				//Tree.TreeContextMenu.ItemClicked += TreeContextMenu_ItemClicked;
			}

			View.AttachTracker(this);
			
			if (Curve != null)
				SelectCurve(Curve);
		}
		public void Cancel()
		{
			//m_frame.okButton.Click -= OnBuild;
			//m_frame.cancelButton.Click -= OnCancel;
			//m_frame.previewButton.Click -= OnPreview;

			m_frame.EditorPanel = null;

			Tree.KeyUp -= Tree_KeyUp;
			Tree.TreeContextMenu.Opening -= ContextMenuStrip_Opening;
			//Tree.TreeContextMenu.ItemClicked -= TreeContextMenu_ItemClicked;
			Tree.DetachTracker(this);

			View.DetachTracker(this);

			if (m_temp != null)
				View.Remove(m_temp);
			View.DeSelect(Curve);
			//View.StopSelect();
			View.Refresh();
		}

		bool m_editMode = false;
		public bool EditMode
		{
			get { return m_editMode; }
			set 
			{
				if (m_curve.Locked && value)
					return;

				m_editMode = value;
				//if (Edit != null)
				//	Edit.Enabled = value;
				if (m_edit != null)
					m_edit.Enabled = value;

				if (View != null)
				{
					View.EditMode = value;
					View.DeSelectAllLayers();
					//if (Curve != null)
					//	SelectCurve(Curve);
				}
				//if (Tree != null)
				//	Tree.EditMode = value;

				//if (value == false)
				//	Tree.DeSelect(m_curve);
			}
		}

		MouldCurve m_curve;
		//CurveEditor m_edit;
		MouldCurveEditor m_edit = new MouldCurveEditor();
		WarpFrame m_frame;

		public MouldCurve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
		}
		//CurveEditor Edit
		//{
		//	get { return m_edit; }
		//}
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

		MouldCurve m_temp;
		Entity[][] m_tents;
		int m_index = -1;

		#region ITracker Members

		public void OnAdd(object sender, EventArgs e) { }
		public void OnDelete(object sender, EventArgs e)
		{
			if (!EditMode)
				return;

			View.Remove(Curve);
			CurveGroup g = Curve.Group as CurveGroup;
			if (g != null)
			{
				g.Remove(Curve);
				m_frame.Rebuild(g);
			}
			m_frame.Delete(Curve);
			if (g != null)
				Tree.SelectedTag = g;
		}

		public void OnCancel(object sender, EventArgs e)
		{
			Cancel();
		}
		public void OnBuild(object sender, EventArgs e)
		{
			if (m_temp == null || !EditMode)
				return;
			//Detach();
			OnPreview(sender, null);
			//View.Remove(m_temp);
			//View.Remove(Curve);

			Curve.Fit(m_temp);
			Curve.Label = m_edit.Label;

			if( sender != null )
				m_frame.Rebuild(Curve);//returns false if AutoBuild is off
			
			//View.Refresh();
		}
		public void OnPreview(object sender, EventArgs e)
		{
			if (m_temp == null || !EditMode)
				return;

			ReadEditor();
			UpdateViewCurve(true);
		}
		void ReadEditor()
		{
			m_edit.WriteCurve(m_temp);

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
			//		pnts.Last().Update(Sail);
			//	}
			//}
			//m_temp.FitPoints = pnts.ToArray();
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			if (Curve != null)
				return;//dont allow changing selection
			object tag = null;
			if (sender is SingleViewportLayout)
				tag = View.SelectedTag;
			else if (sender is TreeView)
				tag = Tree.SelectedTag;

			if (tag != null && tag is MouldCurve)
			{
				SelectCurve(tag as MouldCurve);
			}
		}

		private void SelectCurve(MouldCurve cur)
		{
			if (cur == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp);

			Curve = cur;

			//IFitPoint[] pts = new IFitPoint[Curve.FitPoints.Length];
			//for (int i = 0; i < pts.Length; i++)
			//	pts[i] = Curve[i].Clone();

			m_temp = new MouldCurve(cur);
			m_temp.Label += "[preview]";
			m_tents = View.AddRange(m_temp.CreateEntities(true));

			foreach (Entity[] ents in m_tents)
				foreach (Entity ee in ents)
				{
					ee.Color = Color.LightSkyBlue;
					ee.ColorMethod = colorMethodType.byEntity;
					if ( ee.LineWeight == 1 ) ee.LineWeight = 2.0f;
					ee.LineWeightMethod = colorMethodType.byEntity;
				}

			m_edit.AutoFill = Sail.Watermark(Curve, Tree.SelectedTag as IGroup).ToList<object>();
			m_edit.ReadCurve(m_temp);
			m_edit.Label = Curve.Label;
			m_edit.Refresh();

			if (Tree.SelectedTag != Curve) 
				Tree.SelectedTag = Curve;

			View.Select(Curve);
			View.Refresh();
		}

		public void OnClick(object sender, MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
				return;
			if (Control.ModifierKeys == Keys.Control)
			{
				if( m_index >= 0 )
				{
					if( m_temp.RemovePoint(m_index) )
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
					if ( m_temp.InsertPoint(mpt, View.ActiveView.WorldToScreen, out i) )
						UpdateViewCurve(true);
					
				}

			}
		}

		public void OnDown(object sender, MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
				return;

			if (!EditMode)
				return;

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

		public void OnMove(object sender, MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left || m_index == -1)
				return;
			if (!EditMode)
				return;
			Transformer wts = View.ActiveView.WorldToScreen;
			PointF mpt = new PointF(e.X, View.ActiveView.Height - e.Y);
			if (!m_temp.DragPoint(m_index, mpt, wts))
			{
				m_index = -1;
			}
			else
				UpdateViewCurve(true);
		}

		public void OnUp(object sender, MouseEventArgs e)
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

		//public void OnCopy(object sender, EventArgs e)
		//{
		//	MouldCurve group = Tree.SelectedTag as MouldCurve;
		//	//Lets say its my data format
		//	if (group == null)
		//		return;

		//	Clipboard.Clear();

		//	// Set data to clipboard

		//	// typeof(MouldCurve).ToString() doesn't work here because its string is Warps.CurveGroup
		//	// this is a problem for the clipboard for some reason

		//	Clipboard.SetData(group.GetType().Name, Utilities.Serialize(group.WriteScript()));
		//	m_frame.Status = String.Format("{0}:{1} Copied", group.GetType().Name, group.Label);
		//}

		public void OnPaste(object sender, EventArgs e)
		{
			//the Curve tracker shouldn't do anything with pasted data
		}

		//public bool IsTracking(object obj)
		//{
		//	return obj == m_curve;
		//}

		void UpdateViewCurve(bool bEditor)
		{
			m_temp.ReFit();
			List<Entity> verts = m_temp.CreateEntities(true);
			if( verts != null && verts[0] != null && verts[1] != null )
			foreach (Entity[] ents in m_tents)
			{
				for (int i = 0; i < 2; i++)
				{
					if (ents[i] is LinearPath)
						ents[i].Vertices = verts[0].Vertices;
					else if (ents[i] is PointCloud)
						ents[i].Vertices = verts[1].Vertices;
				}
			}
			View.Regen();
			View.Refresh();
			if (bEditor)
			{
				m_edit.ReadCurve(m_temp);
				//m_edit.Label = Curve.Label;
				m_edit.Update();
				//Edit.Count = m_temp.FitPoints.Length; 
				//for (int i = 0; i < m_temp.FitPoints.Length; i++)
				//{
				//	m_temp[i].WriteEditor(Edit[i]);
				//	Edit[i].Invalidate();
				//}
				//Edit.Length = m_temp.Length;
				//Edit.Update();
			}
		}

		#endregion

		#region TreePopup

		void Tree_KeyUp(object sender, KeyEventArgs e)
		{
			// the modifier key CTRL is pressed by the time it gets here
			switch (e.KeyCode)
			{
				//case Keys.C:
					//OnCopy(Tree.SelectedTag, new EventArgs());
				//	break;
				case Keys.V:
					OnPaste(Tree.SelectedTag, new EventArgs());
					break;
			}
		}

		//void TreeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		//{
		//	logger.Instance.Log("{0}: ContextMenuItem clicked {1}", this.GetType().Name, e.ClickedItem.Name);

		//	//if (e.ClickedItem.Text == "Copy")
		//	//{
		//	//	OnCopy(sender, new EventArgs());
		//	//}
		//	if (e.ClickedItem.Text == "Paste")
		//	{
		//		OnPaste(sender, new EventArgs());
		//	}
		//	else if (e.ClickedItem.Text == "Delete")
		//	{
		//		OnDelete(sender, new EventArgs());
		//	}
		//	else if (e.ClickedItem.Text == "Add")
		//	{
		//		OnAdd(sender, new EventArgs());
		//	}
		//}

		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Tree.TreeContextMenu.Items.Count; i++)
			{
				if (Tree.TreeContextMenu.Items[i].Text == "Paste")
					Tree.TreeContextMenu.Items[i].Enabled = Utilities.GetClipboardObjType() == typeof(MouldCurve);
				if (Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("add"))
					Tree.TreeContextMenu.Items[i].Enabled = false;
				if (Tree.TreeContextMenu.Items[i].Text.ToLower().Contains("delete"))
					Tree.TreeContextMenu.Items[i].Enabled = EditMode;
			}
			Tree.TreeContextMenu.Show();
		}


		#endregion
	}
}
