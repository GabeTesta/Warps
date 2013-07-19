using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public class CurvePoint : IFitPoint
	{
		public CurvePoint() : this(0, null, 0) { }

		public CurvePoint(CurvePoint c)
			: this(c.S, c.m_curve, c.m_sEqu) { }

		public CurvePoint(MouldCurve curve, double sCurve)
			: this(0, curve, sCurve) { }

		public CurvePoint(double s, MouldCurve curve, double sCurve)
		{
			m_sPos = s;
			S_Equ = new Equation(sCurve);
			m_curve = curve;
		}

		public CurvePoint(double s, MouldCurve curve, Equation Sequ)
		{
			m_sPos = s;
			S_Equ = Sequ;
			m_curve = curve;
		}

		internal double m_sPos;
		internal MouldCurve m_curve;
		//internal double m_sCurve;

		Equation m_sEqu = new Equation();
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

		#region IFitPoint Members

		public virtual IFitPoint Clone()
		{
			return new CurvePoint(this);
		}

		/// <summary>
		/// Algo sets this guy so leave him alone
		/// </summary>
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

		/// <summary>
		/// The user input (users can only change this)
		/// </summary>
		public double SCurve
		{
			get { return S_Equ.Value; }
			set
			{
				if(S_Equ.IsNumber())
					S_Equ.Value = value;
				else
					throw new Exception(string.Format("Cannot set value [{0}] for non-numeric equation [{1}]", value, S_Equ.ToString()));
			}
		}

		public virtual Vect2 UV
		{
			get
			{
				Vect2 uv = new Vect2();
				if (m_curve != null)
					m_curve.uVal(SCurve, ref uv);//m_curve.uVal(m_sCurve, ref uv);
				
				return uv;
			}
			set
			{
				double dist = 0;
				if (m_curve != null)
				{
					double sCur = 0;
					m_curve.uClosest(ref sCur, ref value, ref dist, 1e-9);//m_curve.uClosest(ref m_sCurve, ref value, ref dist, 1e-9);
					SCurve = sCur;
				}
			}
		}

		public MouldCurve Curve
		{
			get { return m_curve; }
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

		public virtual TreeNode Node
		{
			get
			{
				TreeNode point = new TreeNode(string.Format("{0:0.0000} [{1}]", S, UV.ToString("0.0000")));
				point.ImageKey = this.GetType().Name;
				point.SelectedImageKey = this.GetType().Name;
				TreeNode tmp = new TreeNode(string.Format("S-Cur: {0:0.0000}", S_Equ.Value));
				tmp.ImageKey = "empty";
				tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				if (m_curve != null)
					tmp = new TreeNode(string.Format("Curve: {0}", m_curve.Label));
				else
					tmp = new TreeNode(string.Format("Curve: {0}", "empty"));
				tmp.ImageKey = "empty";
				tmp.SelectedImageKey = "empty";
				point.Nodes.Add(tmp);

				point.Tag = this;

				return point;
			}
		}

		//#region PointTypeSwitcher
		//public PointTypeSwitcher WriteEditor(PointTypeSwitcher edit)
		//{
		//	if (edit == null)
		//		edit = new PointTypeSwitcher();
		//	CurvePointEditor ce = edit.Edit as CurvePointEditor;
		//	if (ce == null)
		//		ce = new CurvePointEditor();
		//	ce.Tag = GetType();
		//	//ce.Label = GetType().Name;
		//	ce.Curves = PointTypeSwitcher.GetCurves();
		//	ce.Curve = m_curve;
		//	ce.CS = S_Equ;

		//	edit.SetEdit(ce);
		//	return edit;
		//}
		//public void ReadEditor(PointTypeSwitcher edit)
		//{
		//	CurvePointEditor ce = edit.Edit as CurvePointEditor;
		//	if (ce == null)
		//		throw new ArgumentException("Invalid Editor in CurvePoint");
		//	S_Equ = ce.CS;

		//	m_curve = ce.Curve == null ? m_curve : ce.Curve;
		//} 
		//#endregion

		public virtual Control WriteEditor(ref IFitEditor edit)
		{
			if (edit == null || !(edit is CurvePointEditor))
				edit = new CurvePointEditor();
			CurvePointEditor cdit = edit as CurvePointEditor;
			cdit.Tag = GetType();
			cdit.Curve = m_curve;
			cdit.CS = S_Equ;

			return cdit;
		}

		public virtual void ReadEditor(IFitEditor edit)
		{
			if (edit == null )
			throw new ArgumentNullException();
			if( !(edit is CurvePointEditor) )
				throw new ArgumentException("Type must be CurvePointEditor");
			CurvePointEditor cdit = edit as CurvePointEditor;
			m_curve = cdit.Curve;
			S_Equ = cdit.CS;
		}

		#endregion

		#region IRebuild Members

		public bool Affected(List<IRebuild> connected)
		{
			if (connected == null) 
				return false;
			if (connected.Contains(m_curve)) 
				return true;

			if (connected != null)
			{
				bool bupdate = false;
				connected.ForEach(element =>
				{
					if (element is MouldCurve)
					{
						if (S_Equ.EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower()))
							bupdate = true;
					}
					else if (element is Equation)
					{
						if (S_Equ.EquationText.ToLower().Contains((element as Equation).Label.ToLower()))
							bupdate = true;
					}
					else if (element is VariableGroup)
					{
						foreach (KeyValuePair<string, Equation> e in element as VariableGroup)
						{
							if (S_Equ.EquationText.ToLower().Contains(e.Key.ToLower()))
								bupdate = true;
						}
					}
				});
				return bupdate;
			}
			
			return false;
		}
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			parents.Add(Curve);

			//parents.Add(m_sEqu);
			m_sEqu.GetParents(s, parents);
		}


		public bool Update(Sail s) {

			bool ret = true;
			ret &= !double.IsNaN(S_Equ.Evaluate(s));			
			//ret &= U.Evaluate(s) != Double.NaN;
			//ret &= V.Evaluate(s) != Double.NaN;
			return ret;
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count != 3)
				return false;
			//[1] = "\t\tCurve: Luff"
			m_curve = sail.FindCurve(txt[1].Split(new char[]{':'})[1].Trim());
			txt[2] = txt[2].Trim('\t');

			string[] split = txt[2].Split(new char[] { ':' });

			S_Equ = new Equation(split[0], split[1]);


			return Update(sail);

			#region Old
			//string line;
			//double d;
			//foreach (string s in txt)
			//{
			//	line = s.TrimStart('\t');
			//	if (line.StartsWith("S-Cur: "))
			//	{
			//		if (double.TryParse(line.Substring(7), out d))
			//			m_sCurve = d;
			//	}
			//	else if (line.StartsWith("Curve: "))
			//	{
			//		string curlbl = line.Substring(7).Trim();
			//		m_curve = sail.FindCurve(curlbl);
			//	}

			//}

			//return true; 
			#endregion
		}
		public List<string> WriteScript()
		{
			List<string> script = new List<string>();
			//script.Add(string.Format("{0}: [{1}]", GetType().Name, UV.ToString("0.0000")));
			script.Add(GetType().Name);
			script.Add("\tCurve: " + m_curve.Label);
			script.Add("\t" + S_Equ.ToScriptString());
			return script;
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0}: {1:0.0000} [{2}]", GetType().Name, m_sPos, UV.ToString("0.0000"));
		}
		public bool ValidFitPoint
		{
			get
			{
				return m_curve != null;
			}
		}
	}
}
