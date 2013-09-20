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

namespace Warps.Yarns
{
	class YarnGroupTracker: ITracker
	{
		public YarnGroupTracker(YarnGroup group)
		{
			m_group = group;
			m_edit = new YarnGroupEditor(m_group);// m_group.Editor;
		}

		#region Members

		WarpFrame m_frame;
		YarnGroup m_group;
		YarnGroupEditor m_edit;
		YarnGroup m_temp;
		Entity[][] m_tents;

		YarnGroup Group
		{
			get { return m_group; }
			set { m_group = value; }
		}
		YarnGroupEditor Edit
		{
			get { return m_edit; }
		}
		Sail Sail
		{
			get { return m_frame != null ? m_frame.ActiveSail : null; }
		}

		DualView View
		{
			get { return m_frame != null ? m_frame.View : null; }
		}
		TabTree Tree
		{
			get { return m_frame != null ? m_frame.Tree : null; }
		} 
		#endregion

		public bool IsTracking { get { return Edit.IsWarp || Edit.IsGuide || !Group.IsEqual(m_temp); } }

		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_frame.EditorPanel = Edit;		
			}

			if (Tree != null)
			{
				Tree.AttachTracker(this);
			}
			if (View != null)
			{
				View.AttachTracker(this);
				Edit.View = View;
			}

			if (Tree != null)
				Tree.AttachTracker(this);

			SelectGroup(m_group);
		}
		private void SelectGroup(YarnGroup yarns)
		{
			if (yarns == null)
				return;
			if (m_temp != null)
				View.Remove(m_temp, false);

			m_group = yarns;
			//copy the tape group
			m_temp = new YarnGroup(yarns);
			//add temporary entites to view
			m_tents = View.AddRange(m_temp.CreateEntities(true));

			//foreach (Entity[] ents in m_tents)
			//	foreach (Entity ee in ents)
			//	{
			//		ee.Color = Color.FromArgb(100, Color.LightSkyBlue);
			//		ee.ColorMethod = colorMethodType.byEntity;
			//		if (ee.LineWeight == 1) ee.LineWeight = 2.0f;
			//		ee.LineWeightMethod = colorMethodType.byEntity;
			//	}

			//m_edit.AutoFill = Sail.Watermark(tapes).ToList<object>();
			m_edit.ReadGroup(m_temp);
			//m_edit.Label = Curve.Label;
			m_edit.Refresh();

			if (Tree.SelectedTag != m_group)
				Tree.SelectedTag = m_group;

			m_group.Warps.ForEach(wrp => View.Select(wrp));
			View.Select(m_group);
			View.Refresh();
		}
		public void Cancel()
		{
			if (m_frame != null)
				m_frame.EditorPanel = null;

			if (Tree != null)
				Tree.DetachTracker(this);

			if (View != null)
			{
				View.SetTrackerSelectionMode(null);
				View.DetachTracker(this);
				View.Remove(m_temp, false);
				View.DeSelect(m_group);
				m_group.Warps.ForEach(wrp => View.DeSelect(wrp));
				if( m_group.Guide != null )
					View.DeSelect(m_group.Guide);
				View.StopSelect();
				View.Refresh();
			}
		}

		public void OnBuild(object sender, EventArgs e)
		{
			Edit.IsWarp = false;
			Edit.IsGuide = false;

			OnPreview(null, null);
			Edit.WriteGroup(m_temp);
			Group.Fit(m_temp);
			//Group.Label = Edit.Label;
			//Group.Warps = Edit.SelectedWarps;
			//Group.YarnDenierEqu = Edit.YarnDenierEqu;
			//Group.TargetDenierEqu = Edit.TargetDPIEqu;
			//Group.Guide = Edit.Guide;
			//Group.DensityPos = Edit.DensityPos;
			//Group.EndCondition = Edit.Ending;
			//Group.YarnMaterial = Edit.YarnMaterial;
			if (sender != null)
				m_frame.Rebuild(Group);//returns false if AutoBuild is off

			Edit.AchievedDPI = Group.AchievedDpi;
			Edit.AchievedYarnCount = Group.Count;
			Edit.DensityPos = Group.DensityPos;
			Edit.Refresh();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			if (m_temp == null)
				m_temp = new YarnGroup(m_group);
			Edit.WriteGroup(m_temp);
			UpdatePreview(true);
		}
		void UpdatePreview(bool bEditor)
		{
			m_temp.Update(Sail);
			List<Entity> verts = m_temp.CreateEntities(true);
			//	Parallel.ForEach(verts, e => { e.Color = Color.FromArgb(100,Color.LightSkyBlue); e.ColorMethod = colorMethodType.byEntity; });
			if (verts != null)
			{
				View.RemoveRange(m_tents);
				m_tents = View.AddRange(verts);
			}
			//for (int nEnt = 0; nEnt < m_tents.GetLength(0); nEnt++ )
			//	for (int i = 0; i < 2; i++)
			//		m_tents[nEnt][i].Vertices = verts[nEnt].Vertices;//copy vertices to both views

			View.Regen();
			View.Refresh();
			if (bEditor)
			{
				Edit.ReadGroup(m_temp);	//update editor's data with results
				m_edit.Update();
			}
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		public void OnDelete(object sender, EventArgs e)
		{
			View.Remove(Group, true);
			//CurveGroup g = Curve.Group as CurveGroup;
			//if (yarGroup != null)
			//{
			//g.Remove(yarGroup);		
			//}
			m_frame.Delete(Group);
			//if (yarGroup != null)
			//	Tree.SelectedTag = yarGroup;
		}

		public void OnAdd(object sender, EventArgs e) { }

		public void OnPaste(object sender, EventArgs e)
		{

		}

		public void ProcessSelection(object Tag)
		{
			if (Tag == null)
				return;

			if (Tag is GuideComb)
			{
				if( Edit.AddGuide(Tag as GuideComb) )
					View.Select(Tag as GuideComb);
			}
			else if (Tag is MouldCurve)
			{
				if (Edit.AddRemoveWarp(Tag as MouldCurve))
					View.Select(Tag as MouldCurve);
				else
					View.DeSelect(Tag as MouldCurve);
			}
			View.Refresh();
		}
	}
}
