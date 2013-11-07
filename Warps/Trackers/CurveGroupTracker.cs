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

namespace Warps.Curves
{
	public class CurveGroupTracker : ITracker
	{
		public CurveGroupTracker(CurveGroup group)
		{
			m_group = group;
			m_edit = new CurveGroupEditor(m_group);
			m_edit.addCur.Click += OnAdd;
			m_edit.delCur.Click += delCur_Click;
			m_edit.importCurve.Click += importCurve_Click;
		}

		#region Members

		CurveTracker m_curveTracker = null;

		WarpFrame m_frame;
		CurveGroup m_group;
		CurveGroupEditor m_edit;
		CurveGroupEditor Edit
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
		Sail sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}

		#endregion

		#region ITracker

		public bool IsTracking { get { return false; } }

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_frame.EditorPanel = Edit;
				//EditMode = m_frame.EditMode;
				m_edit.AfterSelect += m_frame.OnSelectionChanging;

				ReselectView();
			}
		}

		public void Cancel()
		{
			View.DeSelectAllLayers();
			foreach (MouldCurve curve in m_group)
				View.DeSelect(curve);

			View.Refresh();

			if (m_curveTracker != null)
			{
				m_curveTracker.Cancel();
				m_curveTracker = null;
			}
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (Edit.Label != m_group.Label)
			{
				m_group.Label = Edit.Label;
				m_frame.Rebuild(m_group);
			}
			if (m_curveTracker != null)
			{
				//if tracker exists
				// add curve to group
				// call rebuild with the group
				// should update the tree and view

				//passing a null sender will skip the rebuild call
				m_curveTracker.OnBuild(null, e);//create the curve from the fitpoints
				m_group.Add(m_curveTracker.Curve);//add it to the group
				m_frame.Rebuild(m_group);//rebuild the objects and update the views
				m_curveTracker.Cancel();//destory the temporary curve tracker
				Edit.ReadGroup(m_group);//refresh the editor
				m_frame.EditorPanel = Edit;//restore the curve group editor that was displaced by the curve editor
				Edit.Refresh();
				ReselectView();
			}
		}

		public void OnPreview(object sender, EventArgs e)
		{
			if (m_curveTracker != null)
				m_curveTracker.OnPreview(sender, e);
		}

		[Obsolete]
		public void OnAdd(object sender, EventArgs e)
		{
			if (sail == null)
				return;

			List<Type> useThese = new List<Type>();

			useThese.Add(typeof(MouldCurve));
			useThese.Add(typeof(GuideComb));

			AddItemDialog dlg = new AddItemDialog(useThese);
			dlg.Text = "Add Curve";
			dlg.Name = "enter name";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IRebuild rbd = dlg.CreateIRebuild();
				MouldCurve cur = rbd as MouldCurve;
				m_group.Add(cur);
				m_group.Update(sail);
				m_frame.OnSelectionChanging(this, new EventArgs<IRebuild>(cur));
			}
		}
		[Obsolete]
		public void OnDelete(object sender, EventArgs e)
		{
			// HERE WE DELETE

			// Get clicked item (Curve)

			// List<IRebuild> ret = m_frame.Rebuild(Curve)
			// ret is invalid shit
			// call invalidate(ret)-> color themselves SHOW THEY ARE BROKEN
			// View entity keep previous state, color invalid byEntity, when fixed, byLayer

			for (int i = 0; i < m_group.Count; i++)
				View.Remove(m_group[i], true);

			m_group.Clear();

			m_frame.Rebuild(m_group);
			m_frame.Delete(m_group);
			//m_frame.Rebuild(null);

		}

		public void OnClick(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}
		public void OnDown(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}
		public void OnMove(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}
		public void OnUp(object sender, MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void ProcessSelection(object Tag)
		{

		}

		#endregion

		#region Import/Delete Curve

		void delCur_Click(object sender, EventArgs e)
		{
			if (Edit == null || Edit.SelectedCurve == null)
				return;
			MouldCurve cur = Edit.SelectedCurve;
			if (MessageBox.Show(string.Format("Are you sure you want to delete [{0}]", cur.Label), "Delete Curve?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			View.Remove(cur, true);
			m_group.Remove(cur);
			Edit.ReadGroup(m_group);
			m_frame.Delete(cur);
		}
		void importCurve_Click(object sender, EventArgs e)
		{
			Warps.Curves.CurveW4L dlg = new Curves.CurveW4L();
			if (dlg.ShowDialog(null) == System.Windows.Forms.DialogResult.OK)
			{
				MouldCurve cur = dlg.ParseScript();
				m_curveTracker = new CurveTracker(cur);
				m_curveTracker.Track(m_frame);
			}
		}

		#endregion

		private void ReselectView()
		{
			View.DeSelectAllLayers();
			View.SelectLayer(m_group);
			foreach (MouldCurve curve in m_group)
				View.Select(curve);
			View.Refresh();
		}
	}
}