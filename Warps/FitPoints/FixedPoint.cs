﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Curves;

namespace Warps
{
	class FixedPoint : IFitPoint
	{
		public FixedPoint() : this(0, 0) { }
		public FixedPoint(FixedPoint f) : this(f.U, f.V) { m_s = f.m_s; }

		public FixedPoint(Vect2 uv)	:this(0, uv){}
		public FixedPoint(double s, Vect2 uv) : this(s, uv[0], uv[1]) { }
		public FixedPoint(double u, double v):this(0,u,v){}
		public FixedPoint(double s, double u, double v)
		{
			m_s = s;
			U.Value = u;
			V.Value = v;
		}

		public FixedPoint(Equation ueq, Equation veq)
		{
			U.EquationText = ueq.EquationText;
			V.EquationText = veq.EquationText;
		}

		double m_s;

		#region IFitPoint Members

		public IFitPoint Clone()
		{
			return new FixedPoint(this);
		}

		public double S
		{
			get { return m_s; }
			set { m_s = value; }
		}

		public Vect2 UV
		{
			get { return new Vect2(U.Value, V.Value); }
			//set
			//{
			//	this[1] = value.u;
			//	this[2] = value.v;
			//}
		}

		Equation m_uEqu = new Equation("U Position");
		Equation m_vEqu = new Equation("V Position");

		public Equation U
		{
			get
			{
				return m_uEqu;
			}
			set
			{
				m_uEqu = value; 
			}
		}
		public Equation V
		{
			get
			{
				return m_vEqu;
			}
			set
			{
				m_vEqu = value;
			}
		}

		public string CurrentUV
		{
			get
			{
				return UV.ToString("0.0000");
			}
		}

		public double this[int i]
		{
			get 
			{
				switch (i)
				{
					case 0:
						return S;
					case 1:
						return U.Value;
					case 2:
						return V.Value;
					default:
						return 0;
				}
			}
			set
			{
				switch (i)
				{
					case 0:
						S = value;
						break;
					case 1:
						if (U.IsNumber)
							U.Value = value;
						break;
					case 2:
						if (V.IsNumber)
							V.Value = value;
						break;
				}
			}
		}

		public TreeNode Node
		{
			get
			{
				TreeNode point = new TreeNode(string.Format("{0:0.0000} [{1}]", S, UV.ToString("0.0000")));
				point.ImageKey = point.SelectedImageKey = this.GetType().Name;
				point.Tag = this;

				TreeNode tmp;
				//TreeNode tmp = new TreeNode(string.Format("S-Pos: {0:0.0000}", S));
				//tmp.ImageKey = tmp.SelectedImageKey = typeof(Equation).Name;
				//point.Nodes.Add(tmp);

				tmp = new TreeNode(string.Format("UV Position: {0}", CurrentUV));
				tmp.ImageKey = tmp.SelectedImageKey = typeof(Equation).Name;
				point.Nodes.Add(tmp);

				return point;
				//point.Nodes.Add(string.Format("S-Pos: {0:0.0000}", S), string.Format("S-Pos: {0:0.0000}", S), "empty");
				//point.Nodes.Add(string.Format("UVPos: {0}", UV.ToString("0.0000"), string.Format("UVPos: {0}", "UVPos: {0}", UV.ToString("0.0000"), string.Format("UVPos: {0}", "empty"))));


			}
			//set
			//{
			//	if (value != null)
			//	{
			//		double d;
			//		foreach (TreeNode tn in value.Nodes)
			//		{
			//			if (tn.Text.StartsWith("S: "))
			//			{
			//				if (double.TryParse(tn.Text.Substring(3), out d))
			//					S = d;
			//			}
			//			else if (tn.Text.StartsWith("UV: "))
			//				UV.FromString(tn.Text.Substring(4));
			//		}
			//	}
			//}
		}

		public Control WriteEditor(ref IFitEditor edit)
		{
			if (edit == null || !(edit is FixedPointEditor))
				edit = new FixedPointEditor();
			FixedPointEditor cdit = edit as FixedPointEditor;
			cdit.Tag = GetType();
			cdit.U = U;
			cdit.V = V;

			return cdit;
		}

		public void ReadEditor(IFitEditor edit)
		{
			if (edit == null)
				throw new ArgumentNullException();
			if (!(edit is FixedPointEditor))
				throw new ArgumentException("Type must be FixedPointEditor");

			FixedPointEditor pdit = edit as FixedPointEditor;
			U = pdit.U;
			V = pdit.V;
		}
	
		public bool Affected(List<IRebuild> connected)
		{
			if (connected != null)
			{
				bool bupdate = true;
				bupdate &= U.Affected(connected);
				bupdate &= V.Affected(connected);
				//connected.ForEach(element =>
				//{
				//	//if (element is MouldCurve)
				//	//{
				//	//	if (U.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower())
				//	//		|| V.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower()))
				//	//		bupdate = true;
				//	//}
				//	//else if (element is Equation)
				//	//{
				//	//	if (U.EquationText.ToLower().Contains((element as Equation).Label.ToLower())
				//	//		|| V.EquationText.ToLower().Contains((element as Equation).Label.ToLower()))
				//	//		bupdate = true;
				//	//}
				//	//else if (element is VariableGroup)
				//	//{
				//	//	foreach (KeyValuePair<string, Equation> e in element as VariableGroup)
				//	//	{
				//	//		if (U.EquationText.ToLower().Contains(e.Key.ToLower())
				//	//		|| V.EquationText.ToLower().Contains(e.Key.ToLower()))
				//	//			bupdate = true;
				//	//	}
				//	//}
				//});
				return bupdate;
			}

			return false;
		}
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			//parents.Add(m_uEqu);
			m_uEqu.GetParents(s, parents);

			//parents.Add(m_vEqu);
			m_vEqu.GetParents(s, parents);
		}

		public bool Update(MouldCurve cur)
		{
			bool ret = true;
			ret &= U.Update(cur.Sail);
			ret &= V.Update(cur.Sail);
			return ret;
		}
		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	if (txt.Count != 3)
		//		return false;
			
		//	txt[1] = txt[1].Trim('\t');
		//	txt[2] = txt[2].Trim('\t');
		//	string[] split = txt[1].Split(new char[] { ':' });
		//	U = new Equation(split[0],split[1]);
		//	split = txt[2].Split(new char[] { ':' });
		//	V = new Equation(split[0], split[1]);
		//	return Update(sail);
		//}

		//public List<string> WriteScript()
		//{
		//	List<string> script = new List<string>();
		//	//script.Add(string.Format("{0}: [{1}]", GetType().Name, UV.ToString("0.0000")));
		//	script.Add(GetType().Name);

		//	script.Add("\t" + U.ToScriptString());
		//	script.Add("\t" + V.ToScriptString());

		//	return script;
		//}

		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlNode node = NsXml.MakeNode(doc, GetType().Name);
			node.AppendChild(m_uEqu.WriteXScript(doc));
			node.AppendChild(m_vEqu.WriteXScript(doc));
			return node;
		}

		public void ReadXScript(Sail s, System.Xml.XmlNode node)
		{
			m_uEqu.ReadXScript(s, node.ChildNodes[0]);
			m_vEqu.ReadXScript(s, node.ChildNodes[1]);
		}

		#endregion


		public override string ToString()
		{
			return string.Format("{0}: {1:0.0000} [{2}]", GetType().Name, S, CurrentUV);
		}

		public bool ValidFitPoint
		{
			get
			{
				return true;
			}
		}

	}
}