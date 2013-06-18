using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;

namespace Warps
{
	public class Panel : IRebuild
	{
		public Panel(MouldCurve[] seams, List<SeamSegment>[] ends)
		{
			Locked = true;
			m_seams = seams.Clone() as MouldCurve[];
			m_sends = ends.Clone() as List<SeamSegment>[];
		}

		MouldCurve[] m_seams;//primary seams, should be 2 

		List<SeamSegment>[] m_sends = new List<SeamSegment>[2];//end seams, can be poly-lines of mouldcurves

		#region IRebuild Members

		public string Label
		{
			get
			{
				return m_seams != null && m_seams.Length > 1 && m_seams[1] != null ? m_seams[1].Label : "";
			}
			set
			{
				if (m_seams != null && m_seams.Length > 1 && m_seams[1] != null)
					m_seams[1].Label = value;
			}
		}

		bool m_locked = false;
		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		TreeNode m_node = null;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new TreeNode(Label);
			else
				m_node.Nodes.Clear();
			m_node.ForeColor = Locked ? System.Drawing.Color.Gray : System.Drawing.Color.Black;
			m_node.Tag = this;
			m_node.Text = Label;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.ToolTipText = GetToolTipData();
			if (m_seams != null && m_seams.Length > 1 && m_seams[0] != null && m_seams[1] != null)
			{
				m_node.Nodes.Add(m_seams[0].Label);
				m_node.Nodes.Add(m_seams[1].Label);
			}
			return m_node;
		}

		private string GetToolTipData()
		{
			return GetType().Name;
		}

		public devDept.Eyeshot.Entities.Entity[] CreateEntities()
		{
			if (m_seams != null && m_seams.Length > 1 && m_seams[0] != null && m_seams[1] != null)
			{
				List<Entity> ee = new List<Entity>(m_seams[0].CreateEntities());
				ee.AddRange(m_seams[1].CreateEntities());

				double s;
				Vect2 uv = new Vect2();
				Vect3 xyz = new Vect3();
				if (m_sends != null)
				{
					for (int i = 0; i < m_sends.Length; i++)
					{
						int nVert = 0;
						LinearPath line = new LinearPath(20 * m_sends[i].Count);
						foreach (SeamSegment seg in m_sends[i])
						{
							for (int j = 0; j < 20; j++)
							{
								s = BLAS.interpolate(j, 20, seg.m_sLimits[0], seg.m_sLimits[1]);
								seg.m_seam.xVal(s, ref uv, ref xyz);
								line.Vertices[nVert++] = Utilities.Vect3ToPoint3D(xyz);
							}
						}
						ee.Add(line);
					}

				}

				foreach (Entity e in ee)
					e.EntityData = this;

				return ee.ToArray();
			}
			return null;
			
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> labels = new List<devDept.Eyeshot.Labels.Label>();
				for (int i = 0; i < 2; i++)
				{
					labels.AddRange(m_seams[i].EntityLabel);
					foreach (SeamSegment s in m_sends[i])
						labels.AddRange(s.m_seam.EntityLabel);
				}
				return labels.ToArray();
			}
			//get { return m_seams != null && m_seams.Length > 1 && m_seams[1] != null ? m_seams[1].EntityLabel: null; }
		}

		public void GetConnected(List<IRebuild> updated)
		{
			if (Affected(updated) && updated != null)
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			foreach (MouldCurve seam in m_seams)
			{
				parents.Add(seam);
			}
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bcon = false;

			foreach (MouldCurve seam in m_seams)
			{
				bcon |= seam.Affected(connected);
			}
			return bcon;
		}

		public bool Update(Sail s)
		{
			bool bcon = false;

			foreach (MouldCurve seam in m_seams)
				bcon |= seam.Update(s);
			return bcon;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion

	}
	public class SeamSegment
	{
		public SeamSegment(MouldCurve seam, Vect2 sLimits)
		{
			m_seam = seam;
			m_sLimits = new Vect2(sLimits);
		}
		public MouldCurve m_seam;
		public Vect2 m_sLimits;
	}
}
