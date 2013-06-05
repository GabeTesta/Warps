using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;

namespace Warps
{
	public class ComboMould : ISurface
	{
		public ComboMould() { }
		public ComboMould(Sail sail, string cofpath)
		{
			ReadCofFile(sail, cofpath);
		}

		string m_label;

		CofMould m_mould;
		public CofMould Mould
		{
			get { return m_mould; }
		}

		RBFMould m_extension;
		public RBFMould Extension
		{
			get { return m_extension; }
		}

		public void ReadCofFile(Sail sail, string cofpath)
		{
			m_mould = new CofMould(sail, cofpath);
			m_extension = new RBFMould(m_mould);
			m_label = "Combo " + Mould.Label;
		}

		public List<IGroup> Groups
		{
			get { return m_mould != null ? m_mould.Groups : null; }
		}

		#region IsOutside

		bool IsOutside(Vect2 uv)
		{
			return uv.m_vec.Min() < 0.0 || uv.m_vec.Max() > 1.0;
		}
		bool IsOutside(double u)
		{
			return u < 0.0 || u > 1.0;
		}

		#endregion

		#region ISurface Members

		public void xVal(Vect2 uv, ref Vect3 xyz)
		{
			if (IsOutside(uv))
				Extension.xVal(uv, ref xyz);
			else
				Mould.xVal(uv, ref xyz);
		}

		public void xVec(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv)
		{
			if (IsOutside(uv))
				Extension.xVec(uv, ref xyz, ref dxu, ref dxv);
			else
				Mould.xVec(uv, ref xyz, ref dxu, ref dxv);
		}

		public void xCvt(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv, ref Vect3 ddxu, ref Vect3 ddxv, ref Vect3 dduv)
		{
			if (IsOutside(uv))
				Extension.xCvt(uv, ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);
			else
				Mould.xCvt(uv, ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);
		}

		public void xNor(Vect2 uv, ref Vect3 xyz, ref Vect3 xnor)
		{
			if (IsOutside(uv))
				Extension.xNor(uv, ref xyz, ref xnor);
			else
				Mould.xNor(uv, ref xyz, ref xnor);
		}

		public void xRad(Vect2 uv, ref Vect3 xyz, ref double k)
		{
			SurfaceTools.xRad(this, uv, ref xyz, ref k);
		}

		public bool xClosest(ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol)
		{
			return SurfaceTools.xClosest(this, ref uv, ref xyzTarget, ref dist, tol);
		}

		public List<Entity> CreateEntities(double[,] uvLims, bool bGauss)
		{
			List<Entity> ents = new List<Entity>();
			ents.Add(SurfaceTools.GetMesh(this, uvLims, bGauss));
			return ents;
			//m_entities.Add(SurfaceTools.GetMesh(this, true));
			//m_entities.Last().LayerIndex 
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count == 0)
				return false;
			string line = ScriptTools.ReadLabel(txt[0]);
			if (line != null)
			{
				ReadCofFile(sail, line);
				return true;
			}
			return false;
		}
		public List<string> WriteScript()
		{
			List<string> s = new List<string>();
			s.Add(ScriptTools.Label(GetType().Name, Mould.CofPath));
			return s;
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			//m_node.Text = ScriptTools.Label(GetType().Name, Label);
			m_node.Text = Label;
			m_node.Tag = this;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.Nodes.Clear();
			m_node.Nodes.Add(Mould.WriteNode());
			m_node.Nodes.Add(Extension.WriteNode());
			return m_node;
		}

		public string Label
		{
			get { return m_label; }
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}

	}
}
