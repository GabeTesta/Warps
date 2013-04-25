using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	class FixedPoint : IFitPoint
	{
		public FixedPoint() : this(0, 0) { }
		public FixedPoint(FixedPoint f)
			: this(f[0], f[1], f[2]) 
		{
			U = f.U;
			V = f.V;
		}
		public FixedPoint(double u, double v)
		{ 
			U = new Equation("u", u.ToString(), null); 
			V = new Equation("v", v.ToString(), null); 
		}

		public FixedPoint(Vect2 uv)
		{
			S_Equ = new Equation("s", "-1", null);
			U = new Equation("u", uv[0].ToString(), null);
			V = new Equation("v", uv[1].ToString(), null); 
		}

		public FixedPoint(double s, double u, double v)
		{
			S_Equ = new Equation("s", s.ToString(), null);
			U = new Equation("u", u.ToString(), null);
			V = new Equation("v", v.ToString(), null); 
		}
		public FixedPoint(double s, Vect2 uv)
		{
			m_s = s;
			m_uv = uv;
		}

		public FixedPoint(Equation ueq, Equation veq)
		{
			U = ueq;
			V = veq;
		}

		double m_s;
		Vect2 m_uv;

		#region IFitPoint Members

		public IFitPoint Clone()
		{
			return new FixedPoint(this);
		}

		public double S
		{
			get { return S_Equ.Result; }
			set { S_Equ.Value = value; }
		}

		public Vect2 UV
		{
			get { return new Vect2(U.Result, V.Result); }
			set { }
			//set { m_uv = value; }
		}

		Equation m_sEqu = new Equation();
		Equation m_uEqu = new Equation();
		Equation m_vEqu = new Equation();

		public Equation S_Equ
		{
			get
			{
				return m_sEqu;
			}
			set
			{
				m_sEqu = value; m_sEqu.Label = "S";
			}
		}

		public Equation U
		{
			get
			{
				return m_uEqu;
			}
			set
			{
				m_uEqu = value; m_uEqu.Label = "U";
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
				m_vEqu = value; m_vEqu.Label = "V";
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
						return S_Equ.Result;
					case 1:
						return U.Result;
					case 2:
						return V.Result;
					default:
						return 0;
				}
			}
			set
			{
				switch (i)
				{
					case 0:
						if(S_Equ.IsNumber())
							S_Equ.Value = value;
						break;
					case 1:
						if (U.IsNumber())
							U.Value = value;
						break;
					case 2:
						if (V.IsNumber())
							V.Value = value;
						break;
				}
			}
		}

		TreeNode IFitPoint.Node
		{
			get
			{
				TreeNode point = new TreeNode(ToString());
				point.ImageKey = this.GetType().Name;
				point.SelectedImageKey = this.GetType().Name;
				TreeNode tmp = new TreeNode(string.Format("S-Pos: {0:0.0000}", S));
				tmp.ImageKey = "empty";
				tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				tmp = new TreeNode(string.Format("UVPos: {0}", CurrentUV));
				tmp.ImageKey = "empty";
				tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				point.Tag = this;

				return point;
				//point.Nodes.Add(string.Format("S-Pos: {0:0.0000}", S), string.Format("S-Pos: {0:0.0000}", S), "empty");
				//point.Nodes.Add(string.Format("UVPos: {0}", UV.ToString("0.0000"), string.Format("UVPos: {0}", "UVPos: {0}", UV.ToString("0.0000"), string.Format("UVPos: {0}", "empty"))));


			}
			set
			{
				if (value != null)
				{
					double d;
					foreach (TreeNode tn in value.Nodes)
					{
						if (tn.Text.StartsWith("S: "))
						{
							if (double.TryParse(tn.Text.Substring(3), out d))
								S = d;
						}
						else if (tn.Text.StartsWith("UV: "))
							UV.FromString(tn.Text.Substring(4));
					}
				}
			}
		}

		public PointTypeSwitcher WriteEditor(PointTypeSwitcher edit)
		{
			if (edit == null)
				edit = new PointTypeSwitcher();
			FixedPointEditor fp = edit.Edit as FixedPointEditor;
			//fp.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			if (fp == null)
				fp = new FixedPointEditor();
			fp.Tag = GetType();
			fp.Label = GetType().Name;
			fp.U = U;
			fp.V = V;
			edit.SetEdit(fp);
			return edit;
		}

		public void ReadEditor(PointTypeSwitcher edit)
		{
			FixedPointEditor fp = edit.Edit as FixedPointEditor;
			if (fp == null)
				return;
			U = fp.U;
			V = fp.V;
		}

		public Control Editor
		{
			get
			{
				FixedPointEditor fp = new FixedPointEditor();
				fp.Label = GetType().Name;
				fp.U = U;
				fp.V = V;
				return fp;
			}
			set
			{
				FixedPointEditor fp = value as FixedPointEditor;
				if (fp == null)
					return;
				U = fp.U;
				V = fp.V;
			}
		}

		#endregion

		#region IRebuild Members

		public bool Affected(List<IRebuild> connected)
		{
			if (connected != null)
			{
				bool bupdate = false;
				connected.ForEach(element =>
				{
					if (element is MouldCurve)
					{
						if (S_Equ.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower())
							|| U.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower())
							|| V.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower()))
							bupdate = true;
					}
					else if (element is Equation)
					{
						if (S_Equ.EquationText.ToLower().Contains((element as Equation).Label.ToLower())
							|| U.EquationText.ToLower().Contains((element as Equation).Label.ToLower())
							|| V.EquationText.ToLower().Contains((element as Equation).Label.ToLower()))
							bupdate = true;
					}
					else if (element is VariableGroup)
					{
						foreach (KeyValuePair<string, Equation> e in element as VariableGroup)
						{
							if (S_Equ.EquationText.ToLower().Contains(e.Key.ToLower())
							|| U.EquationText.ToLower().Contains(e.Key.ToLower())
							|| V.EquationText.ToLower().Contains(e.Key.ToLower()))
								bupdate = true;
						}
					}
				});
				return bupdate;
			}

			return false;
		}
		public bool Delete() { return false; }

		public bool Update(Sail s)
		{
			bool ret = true;
			ret &= S_Equ.Evaluate(s) != Double.NaN;
			ret &= U.Evaluate(s) != Double.NaN;
			ret &= V.Evaluate(s) != Double.NaN;
			return ret;
		}
		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count != 3)
				return false;
			
			txt[1] = txt[1].Trim('\t');
			txt[2] = txt[2].Trim('\t');
			string[] split = txt[1].Split(new char[] { ':' });
			U = new Equation(split[0],split[1]);
			split = txt[2].Split(new char[] { ':' });
			V = new Equation(split[0], split[1]);
			return Update(sail);
		}

		public List<string> WriteScript()
		{
			List<string> script = new List<string>();
			//script.Add(string.Format("{0}: [{1}]", GetType().Name, UV.ToString("0.0000")));
			script.Add(GetType().Name);

			script.Add("\t" + U.ToString());
			script.Add("\t" + V.ToString());

			return script;
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
			set { }
		}
	
	}
}
