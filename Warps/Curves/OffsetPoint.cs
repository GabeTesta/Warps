using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	class OffsetPoint: IFitPoint
	{
		public OffsetPoint(OffsetPoint cloneme) : this(cloneme.m_sCurve, cloneme.m_curve, cloneme.m_xOffset) {  }
		public OffsetPoint(double sCurve, MouldCurve curve, double offset)
		{
			m_sCurve = sCurve;
			m_curve = curve;
			m_xOffset = offset;
			Update(null);
		}
		double m_sCurve;
		double m_xOffset;
		MouldCurve m_curve;

		#region IFitPoint Members

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		double m_sPos;
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

		Vect2 m_uv = new Vect2();
		public Vect2 UV
		{
			get
			{
				return m_uv;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IFitPoint Clone()
		{
			return new OffsetPoint(this);
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
					case 1:
						//U.Value = value;
						break;
					case 2:
						//V.Value = value;
						break;
				}
			}
		}

		
		public TreeNode Node
		{
			get
			{
				TreeNode point = new TreeNode(string.Format("{0:0.0000} [{1}]", S, UV.ToString("0.0000")));
				point.ImageKey = point.SelectedImageKey = GetType().Name;
				TreeNode tmp = new TreeNode(string.Format("S-Cur: {0:0.0000}", m_sCurve));
				tmp.ImageKey = tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				if (m_curve != null)
					tmp = new TreeNode(string.Format("Curve: {0}", m_curve.Label));
				else
					tmp = new TreeNode(string.Format("Curve: {0}", "empty"));

				tmp.ImageKey = tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				tmp = new TreeNode(string.Format("Offset: {0:f4}", m_xOffset));
				tmp.ImageKey = tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				point.Tag = this;
				return point;
			}
		}

		public Control WriteEditor(ref IFitEditor edit)
		{
			throw new NotImplementedException();
		}

		public void ReadEditor(IFitEditor edit)
		{
			throw new NotImplementedException();
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			if( !parents.Contains(m_curve) )
				parents.Add(m_curve);
		}

		public bool Affected(List<IRebuild> connected)
		{
			return connected.Contains(m_curve);
		}

		public bool Update(Sail s)
		{
			int nNwt;
			Vect3 x = new Vect3(), xn = new Vect3();
			Vect2 un = new Vect2();
			//get unit inplane normal in u-coords
			m_curve.uNor(m_sCurve, ref m_uv, ref un);
			for( nNwt = 0; nNwt < 25; nNwt++ )
			{
				//curve point and normal offset in x-coords
				m_curve.xVal(m_uv, ref x);
				m_curve.xVal(un + m_uv, ref xn);

				//x-offset from unit normal
				double dn = xn.Distance(x);
				if (BLAS.IsEqual(dn, m_xOffset, 1e-6))
					break;
				//scale normal to match target offset
				dn = m_xOffset / dn;
				un.Magnitude *= dn;
			}
			//offset uv coords using scaled normal
			m_uv += un;

			return nNwt < 25;
		}

		public bool ValidFitPoint
		{
			get { return m_curve != null;  }
		}

		#endregion
	}
}
