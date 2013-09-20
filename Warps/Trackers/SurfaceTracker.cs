using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace Warps.Trackers
{
	public class SurfaceTracker : ITracker
	{
		public SurfaceTracker(GuideSurface guide)
		{
			m_surf = guide;
			Edit.UpdatedSurface += Edit_UpdatedSurface;
		}

		GuideSurface m_surf;
		WarpFrame m_frame;
		Warps.Controls.SurfEditor m_edit = new Controls.SurfEditor();
		GuideSurface Surf
		{
			get { return m_surf; }
			set { m_surf = value; }
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
		Warps.Controls.SurfEditor Edit
		{
			get { return m_edit; }
		}
		#region ITracker Members

		public bool IsTracking { get { return false; } }

		public void Track(WarpFrame frame)
		{
			if (frame == null)
				return;

			m_frame = frame;
			m_frame.EditorPanel = Edit;

			Tree.AttachTracker(this);
			View.AttachTracker(this);

			if (Surf != null)
				SelectSurface(Surf);
		}

		public void Cancel()
		{
			//m_frame.okButton.Click -= OnBuild;
			//m_frame.cancelButton.Click -= OnCancel;
			//m_frame.previewButton.Click -= OnPreview;

			if (m_frame != null)
				m_frame.EditorPanel = null;

			if (Tree != null)
				Tree.DetachTracker(this);
			View.DetachTracker(this);

			if (View != null)
			{
				if (m_temp != null)
					View.Remove(m_temp, false);
				View.DeSelect(Surf);
				View.StopSelect();
				View.Refresh();
				View.DetachTracker(this);
			}
		}

		GuideSurface m_temp;
		Entity[][] m_tents;
		int m_index = -1;
		bool bHeight = false;
		private void SelectSurface(GuideSurface surf)
		{
			if (surf == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp, false);

			Surf = surf;

			m_temp = new GuideSurface(surf);

			m_tents = View.AddRange(m_temp.CreateEntities(true));

			foreach (Entity[] ents in m_tents)
				foreach (Entity ee in ents)
				{
					ee.Color = Color.LightSkyBlue;
					ee.ColorMethod = colorMethodType.byEntity;
				}

			//Edit.AutoFill = Sail.Watermark(Comb).ToList<object>();
			Edit.ReadSurf(m_temp);
			//Edit.Label = Comb.Label;
			Edit.Refresh();

			if (Tree.SelectedTag != Surf) Tree.SelectedTag = Surf;
			//if (View.SelectedTag != Curve) 
			//View.Select(Surf);
			//View.SelectLayer(Curve);
			View.Refresh();
		}

		private void Preview()
		{
			if (m_temp == null)
				m_temp = new GuideSurface(Surf);
			Edit.WriteSurf(m_temp);
			UpdatePreview(true);
		}
		void UpdatePreview(bool bEditor)
		{
			m_temp.ReFit();
			List<Entity> verts = m_temp.CreateEntities(true);
			Parallel.ForEach(verts, e => { e.Color = Color.LightSkyBlue; e.ColorMethod = colorMethodType.byEntity; });
			
			if (verts != null)
			{
				View.RemoveRange(m_tents);
				m_tents = View.AddRange(verts);
			}

			//View.Regen();
			View.Refresh();
			if (bEditor)
			{
				Edit.ReadSurf(m_temp);
				//Edit.Label = Comb.Label;
				Edit.Update();
			}
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
						UpdatePreview(true);
				}
				else
				{
					//get mouse point in uv, find closest curve point, add fit point there.
					System.Drawing.Point mpt = new System.Drawing.Point(e.X, View.ActiveView.Height - e.Y);

					int i;
					
					if (m_temp.InsertPoint(mpt, View.ActiveView.WorldToScreen, out i))
						UpdatePreview(true);

				}

			}
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
				return;

			if (sender is ViewportLayout)
			{
				Point3D vert, bot;
				PointF m_mousePnt = (PointF)e.Location;
				m_mousePnt.Y = View.ActiveView.Height - m_mousePnt.Y;
				Point3D ms = new Point3D(m_mousePnt.X, m_mousePnt.Y);
				Vect3 AB = new Vect3(), AC = new Vect3();
				int nview = View.ActiveViewIndex;
				m_index = -1;
				for (int i = 0; i < m_tents.Length && m_index < 0; i++)
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
								bHeight = Control.ModifierKeys == Keys.Shift;
								break;
							}
						}
					}
					else if (m_tents[i][nview] is LinearPath)//check sticks for vertical adjustment
					{
						vert = View.ActiveView.WorldToScreen(m_tents[i][nview].Vertices[1]);//top point in mouse-coords
						bot = View.ActiveView.WorldToScreen(m_tents[i][nview].Vertices[0]);//mould point in mouse-coords
						AB.Set((ms - vert).ToArray());
						AC.Set((ms - bot).ToArray());
						double h = AB.Cross(AC).Magnitude / vert.DistanceTo(bot);
						if (h < 5.0)
						{
							m_index = i - 2;//subtract 2 to offset for the mesh and pointcloud
							bHeight = Control.ModifierKeys == Keys.Shift;
							break;
						}
					}
				}
			}
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left || m_index == -1)
				return;
			Transformer wts = View.ActiveView.WorldToScreen;
			PointF mpt = new PointF(e.X, View.ActiveView.Height - e.Y);
			if (bHeight)
			{
				m_temp.DragHeight(m_index, mpt, wts);
				UpdatePreview(true);
			}
			else
				if (!m_temp.DragPoint(m_index, mpt, wts))
				{
					m_index = -1;
				}
				else
					UpdatePreview(true);
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_temp == null)
				return;
			if (e.Button == MouseButtons.Left)
			{
				m_index = -1;
			}
		}

		public void OnPaste(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnDelete(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnAdd(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			Preview();//update the temp curve
			Surf.Fit(m_temp.FitPoints);//copy the temp group's data back
			Surf.Label = m_temp.Label;//update the label
			//Edit.WriteSurf(m_surf);

			if (sender != null)
				m_frame.Rebuild(Surf);//returns false if AutoBuild is off

			Edit.ReadSurf(Surf);
			Edit.Refresh();
			//View.Refresh();
			Tree.Refresh();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			Preview();
		}

		public void ProcessSelection(object Tag)
		{

		}

		#endregion

		void Edit_UpdatedSurface(object sender, EventArgs e)
		{
			Edit.WriteSurf(m_temp);
			UpdatePreview(false);
		}

	}
}
