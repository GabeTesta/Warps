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
using Warps.Curves;

namespace Warps
{
	public class CurveTracker : ITracker
	{
		public CurveTracker(MouldCurve curve)
		{
			m_curve = curve;
			m_edit = new MouldCurveEditor();
		}

		#region Members

		MouldCurve m_curve;
		MouldCurveEditor m_edit;
		WarpFrame m_frame;

		public MouldCurve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
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

		MouldCurve m_temp;
		Entity[][] m_tents;
		int m_index = -1;

		#endregion

		#region ITracker Members

		public bool IsTracking { get { return !m_curve.IsEqual(m_temp); } }

		public void Track(WarpFrame frame)
		{
			if (frame == null)
				return;

			m_frame = frame;

			m_frame.EditorPanel = m_edit;
			//EditMode = frame.EditMode;


			View.AttachTracker(this);

			if (Curve != null)
				SelectCurve(Curve);
		}
		public void Cancel()
		{
			m_frame.EditorPanel = null;

			View.DetachTracker(this);

			if (m_temp != null)
			{
				View.Remove(m_temp, false);
				m_temp = null;
			}
			if (m_tents != null)
			{
				View.RemoveRange(m_tents);
				m_tents = null;
			}
			View.DeSelect(Curve);
			//View.StopSelect();
			View.DetachTracker(this);
			View.Refresh();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (m_temp == null)
				return;
			//Detach();
			OnPreview(sender, null);
			//View.Remove(m_temp);
			//View.Remove(Curve);

			Curve.Fit(m_temp);
			Curve.Label = m_edit.Label;

			if (sender != null)
				m_frame.Rebuild(Curve);//returns false if AutoBuild is off

			//View.Refresh();
		}
		public void OnPreview(object sender, EventArgs e)
		{
			if (m_temp == null)
				return;

			m_edit.WriteCurve(m_temp);
			UpdateViewCurve(true);
		}

		public void ProcessSelection(object Tag)
		{
			//should attempt to pass on the selection to the active fitpoint editor
			//ideally selecting the desired curve/equation from the tree/view instead of the dropdown
		}

		public void OnClick(object sender, MouseEventArgs e)
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
						UpdateViewCurve(true);

				}

			}
		}
		public void OnDown(object sender, MouseEventArgs e)
		{
			if (m_temp == null || e.Button != MouseButtons.Left)
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

			if (e.Button == MouseButtons.Left)
			{
				m_index = -1;
			}
		}

		public void OnDelete(object sender, EventArgs e)
		{
			View.Remove(Curve, true);
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

		#endregion

		private void SelectCurve(MouldCurve cur)
		{
			if (cur == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp, false);

			Curve = cur;
			if (cur.Sail == null)
				cur.Sail = Sail;
			//IFitPoint[] pts = new IFitPoint[Curve.FitPoints.Length];
			//for (int i = 0; i < pts.Length; i++)
			//	pts[i] = Curve[i].Clone();

			m_temp = new MouldCurve(cur);
			//	m_temp.Label += "[preview]";
			m_tents = View.AddRange(m_temp.CreateEntities(true));

			//foreach (Entity[] ents in m_tents)
			//	foreach (Entity ee in ents)
			//	{
			//		//ee.Color = Color.LightSkyBlue;
			//		//ee.ColorMethod = colorMethodType.byEntity;
			//		//if (ee.LineWeight == 1) ee.LineWeight = 2.0f;
			//		//ee.LineWeightMethod = colorMethodType.byEntity;
			//	}

			m_edit.AutoFill = Sail.Watermark(Curve, Tree.SelectedTag as IGroup).ToList<object>();
			m_edit.ReadCurve(m_temp);
			m_edit.Label = Curve.Label;
			m_edit.Refresh();

			if (Tree.SelectedTag != Curve)
				Tree.SelectedTag = Curve;

			View.Select(Curve);
			View.Refresh();
		}
		void UpdateViewCurve(bool bEditor)
		{
			m_temp.ReFit();
			List<Entity> verts = m_temp.CreateEntities(true);
			if (m_tents == null || m_tents.Length != verts.Count)
				m_tents = View.AddRange(verts);
			else
			{
				if (verts != null && verts.Count > 1 && verts[0] != null && verts[1] != null)
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
	}
}
