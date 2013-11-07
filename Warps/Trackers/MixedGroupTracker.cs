using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warps.Controls;

namespace Warps.Mixed
{
	class MixedGroupTracker: ITracker
	{
		public MixedGroupTracker(MixedGroup group)
		{
			m_group = group;
			m_edit = new MixedGroupEditor();
		}
		#region Members

		MixedGroup m_group;
		MixedGroupEditor m_edit;

		WarpFrame m_frame;
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

		#region ITracker Members

		public bool IsTracking
		{
			get { return false; }
		}


		public void Track(WarpFrame frame)
		{
			m_frame = frame;

			if (m_frame != null && m_group != null)
			{
				m_frame.EditorPanel = m_edit;
				//EditMode = m_frame.EditMode;
				m_edit.AfterSelect += m_frame.OnSelectionChanging;

				m_edit.ReadGroup(m_group);
				ReselectView();
			}
		}

		public void Cancel()
		{
			View.DeSelectAllLayers();
			View.Refresh();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			m_edit.WriteGroup(m_group);
			m_frame.Rebuild(m_group);
		}

		public void OnPreview(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//throw new NotImplementedException();
		}

		[Obsolete]
		public void OnDelete(object sender, EventArgs e)
		{
			for (int i = 0; i < m_group.Count; i++)
				View.Remove(m_group[i], true);

			m_group.Clear();

			m_frame.Rebuild(m_group);
			m_frame.Delete(m_group);
		}

		//public void OnAdd(object sender, EventArgs e)
		//{
		//	if (sail == null)
		//		return;

		//	List<Type> useThese = Utilities.GetAllOf(typeof(IRebuild), false);

		//	AddItemDialog dlg = new AddItemDialog(useThese);
		//	dlg.Text = "Add Item";
		//	dlg.Name = "enter name";
		//	if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		//	{
		//		IRebuild cur = dlg.CreateIRebuild();
		//		if (cur != null)
		//		{
		//			m_group.Add(cur);
		//			m_frame.OnSelectionChanging(this, new EventArgs<IRebuild>(cur));
		//		}
		//	}
		//}

		public void ProcessSelection(object Tag)
		{
			//throw new NotImplementedException();
		}

		#endregion

		private void ReselectView()
		{
			View.DeSelectAllLayers();
			View.SelectLayer(m_group);
			foreach (IRebuild curve in m_group)
				View.Select(curve);
			View.Refresh();
		}
	}
}
