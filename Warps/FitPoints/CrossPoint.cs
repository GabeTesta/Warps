using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	class CrossPoint: IFitPoint
	{
		public CrossPoint() { }
		public CrossPoint(string c1, string c2)
		{
			m_Curves[0] = WarpFrame.CurrentSail.FindCurve(c1);
			m_Curves[1] = WarpFrame.CurrentSail.FindCurve(c2);
		}
		public CrossPoint(IMouldCurve c1, IMouldCurve c2)
		{
			m_Curves[0] = c1;
			m_Curves[1] = c2;
		}
		public CrossPoint(CrossPoint cross)
		{
			m_Curves[0] = cross.m_Curves[0];
			m_Curves[1] = cross.m_Curves[1];
		}
		internal double m_sPos;
		internal IMouldCurve[] m_Curves = new IMouldCurve[2];
		#region IFitPoint Members

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count != 3)
				return false;
			//[1] = "\t\tCurve1: Luff"
			m_Curves[0] = sail.FindCurve(txt[1].Split(new char[] { ':' })[1].Trim());
			m_Curves[1] = sail.FindCurve(txt[2].Split(new char[] { ':' })[1].Trim());
			if (m_Curves[0] == null || m_Curves[1] == null)
				throw new ArgumentException(string.Format("Cannot find CrossPoint curve: {0}",
					m_Curves[0] == null ? 
					txt[1].Split(new char[] { ':' })[1].Trim() : 
					txt[2].Split(new char[] { ':' })[1].Trim()));

			return Update(sail);
		}

		public List<string> WriteScript()
		{
			List<string> script = new List<string>();
			script.Add(GetType().Name);
			script.Add("\tCurve1: " + m_Curves[0].Label);
			script.Add("\tCurve2: " + m_Curves[1].Label);
			return script;
		}

		public double S
		{
			get
			{
				return m_sPos;
			}
			set
			{
				m_sPos = value;
			}
		}

		Vect2 m_uv;
		public Vect2 UV
		{
			get
			{
				if (m_uv == null)
				{
					if( Update(null))
						return m_uv;
					return null;
				}
				else
					return m_uv;
			}
			set
			{
				//throw new NotImplementedException();
			}
		}

		public IFitPoint Clone()
		{
			return new CrossPoint(this);
		}

		public double this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:
						return m_sPos;
					case 1:
						return UV[0];
					case 2:
						return UV[1];
					default:
						return 0;
				}
			}
			set
			{
				switch (i)
				{
					case 0:
						m_sPos = value;
						break;
					//case 1:
					//	//U.Value = value;
					//	break;
					//case 2:
					//	//V.Value = value;
					//	break;
				}
			}
		}

		public System.Windows.Forms.TreeNode Node
		{
			get
			{
				TreeNode point = new TreeNode(string.Format("{0:0.0000} [{1}]", S, UV.ToString("0.0000")));
				point.ImageKey = point.SelectedImageKey = GetType().Name;


				if (ValidFitPoint)
				{
					TreeNode tmp = new TreeNode(string.Format("Curves: {0}, {1}", m_Curves[0], m_Curves[1]));
					tmp.ImageKey = tmp.SelectedImageKey = typeof(MouldCurve).Name;
					point.Nodes.Add(tmp);
				}
				point.Tag = this;
				return point;
			}
		}

		public Control WriteEditor(ref IFitEditor edit)
		{
			if (edit == null || !(edit is CrossPointEditor))
				edit = new CrossPointEditor();
			CrossPointEditor cdit = edit as CrossPointEditor;
			cdit.Tag = GetType();
			cdit.Curve1 = m_Curves[0];
			cdit.Curve2 = m_Curves[1];
			return cdit;
		}

		public void ReadEditor(IFitEditor edit)
		{
			if (edit == null)
				throw new ArgumentNullException();
			if (!(edit is CrossPointEditor))
				throw new ArgumentException("Type must be CrossPointEditor");
			CrossPointEditor cdit = edit as CrossPointEditor;
			m_Curves[0] = cdit.Curve1;
			m_Curves[1] = cdit.Curve2;
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			if (m_Curves[0] is IRebuild)
				parents.Add(m_Curves[0] as IRebuild);

			if (m_Curves[1] is IRebuild)
				parents.Add(m_Curves[1] as IRebuild);
			//parents.AddRange(m_Curves);
		}

		public bool Affected(List<IRebuild> connected)
		{
			if (connected == null)
				return false;
			return connected.Contains(m_Curves[0] as IRebuild) || connected.Contains(m_Curves[1] as IRebuild);
		}
		public bool Update(Sail s)
		{
			Vect2 ss = new Vect2();
			Vect3 xyz = new Vect3();
			if( m_uv == null) 
				m_uv = new Vect2();
			return CurveTools.CrossPoint(m_Curves[0], m_Curves[1], ref m_uv, ref xyz, ref ss, 30);
		}

		public bool ValidFitPoint
		{
			get { return m_Curves != null && m_Curves.Length == 2 && m_Curves[0] != null && m_Curves[1] != null; }
		}

		#endregion
	}
}
