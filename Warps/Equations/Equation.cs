using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;
using System.Windows.Forms;

namespace Warps
{
	public class Equation: IRebuild
	{
		public Equation() { m_label = "empty"; m_text = "0"; Value = 0; }

		public Equation(string label, string equationText)
		{
			m_label = label;
			m_text = equationText;
			IsNumber();
		}

		public Equation(string label, double value)
		{
			m_label = label;
			m_text = value.ToString();
			SetValue(value);
		}

		string m_text = null;
		string m_label = null;
		double m_result = double.NaN;

		System.Windows.Forms.TreeNode m_node = null;

		public bool IsNumber()
		{
			return double.TryParse(EquationText, out m_result);
		}

		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

		public string EquationText
		{
			get { return m_text; }
			set { m_text = value; }
		}

		private double Value
		{
			set { m_text = value.ToString(); IsNumber(); }
		}

		/// <summary>
		/// Attempt to set the EquationText and Result to value (if equation uses an expression, this will return false)
		/// </summary>
		/// <param name="val">double to attempt to set</param>
		/// <returns>true if equation is a number and can be set this way, false if equation is expression</returns>
		public bool SetValue(double val)
		{
			if (IsNumber())
			{
				Value = val;
				return true;
			}
			return false;
		}

		public double Result
		{
			get { return m_result; }
		}

		public double Evaluate(Sail s)
		{
			if (IsNumber())
				return Result;

			if (s == null) 
				return double.NaN;
			if (EquationEvaluator.Evaluate(this, s, out m_result))
				return Result;

			return double.NaN;
		}

		public devDept.Eyeshot.Entities.Entity[] CreateEntities()
		{
			return null;
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				return null;
			}
		}

		public bool Update(Sail s) { return !double.IsNaN(Evaluate(s)); }

		/// <summary>
		/// this only returns false currently.  
		/// </summary>
		/// <returns></returns>
		public bool Delete() { return false; }

		public void GetConnected(List<IRebuild> connected)
		{
			bool bupdate = false;
			//if (connected == null)
			//{
			//	Evaluate();
			//	return bupdate;
			//}
			connected.ForEach(element =>
			{
				if (element is MouldCurve)
				{
					if (EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower()))
						bupdate = true;
				}
				else if (element is Equation)
				{
					if (EquationText.ToLower().Contains((element as Equation).Label.ToLower()))
						bupdate = true;
				}
				else if (element is VariableGroup)
				{
					foreach (KeyValuePair<string, Equation> equ in element as VariableGroup)
						if (EquationText.ToLower().Contains(equ.Key.ToLower()))
							bupdate = true;
				}
			});
			if (bupdate)
			{
				connected.Add(this);
			}
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = false;

			//if (connected == null)
			//{
			//	Evaluate();
			//	return bupdate;
			//}

			if (connected == null)
				return false;

			connected.ForEach(element =>
			{
				if (element is MouldCurve)
				{
					if (EquationText.ToLower().Contains((element as MouldCurve).Label.ToLower()))
						bupdate = true;
				}
				else if (element is Equation)
				{
					if (EquationText.ToLower().Contains((element as Equation).Label.ToLower()))
						bupdate = true;
				}
				else if (element is VariableGroup)
				{
					foreach (KeyValuePair<string, Equation> equ in element as VariableGroup)
						if (EquationText.ToLower().Contains(equ.Key.ToLower()))
							bupdate = true;
				}
			});

			return bupdate;
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt.Count != 2)
				return false;

			txt[1] = txt[1].Trim('\t');
			Label = txt[1].Split(new char[] { ':' })[0];
			EquationText = txt[1].Split(new char[] { ':' })[1];
			
			Evaluate(sail);
			return true;
		}

		public List<string> WriteScript(string depth)
		{
			List<string> script = new List<string>();
			script.Add(depth + GetType().Name);
			script.Add(depth + "\t" + ToString());
			return script;
		}
		public List<string> WriteScript()
		{
			return WriteScript("");
		}
		public TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new TreeNode(string.Format("{0}", Label));

			m_node.ImageKey = "Equation";
			m_node.SelectedImageKey = "Equation";
			m_node.Tag = this;
			m_node.Name = Label;
			TreeNode tmp1 = new TreeNode(string.Format("Text: {0}", EquationText));
			tmp1.ImageKey = "EquationText";
			tmp1.SelectedImageKey = "EquationText";
			TreeNode tmp2 = new TreeNode(string.Format("Value: {0}", Result));
			tmp2.ImageKey = "Result";
			tmp2.SelectedImageKey = "Result";
			m_node.Nodes.Clear();
			m_node.Nodes.Add(tmp1);
			m_node.Nodes.Add(tmp2);
			
			return m_node;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Label, EquationText);
		}

		//public List<MouldCurve> ExtractReferencedCurves()
		//{
		//	if (sail == null)
		//		return null;

		//	return EquationEvaluator.ExtractCurves(this, sail);
		//}

		Warps.Controls.VariableEditor GetEditor()
		{
			Warps.Controls.VariableEditor eq = new Warps.Controls.VariableEditor(Label, EquationText);
			//eq.AutoFillData = sail.GetAutoFillData(this).ToArray();
			//eq.sail = sail;
			return eq;
		}

		public Warps.Controls.VariableEditor WriteEditor(Warps.Controls.VariableEditor edit)
		{
			if (edit == null)
				edit = new Warps.Controls.VariableEditor(Label, EquationText);
			Warps.Controls.VariableEditor ee = edit as Warps.Controls.VariableEditor;
			if (ee == null)
				ee = new Warps.Controls.VariableEditor(Label, EquationText);
			//ee.sail = sail;
			//if(sail!=null)
			//	ee.AutoFillData = sail.GetAutoFillData(this).ToArray();
			ee.Tag = GetType();
			//ee.Label = GetType().Name;
			return edit;
		}

		public void ReadEditor(Warps.Controls.VariableEditor edit)
		{
			if (edit == null)
				throw new ArgumentException("Invalid Editor in CurvePoint");
			Label = edit.Label;
			EquationText = edit.EquationText;
			Evaluate(edit.sail);
		}

		internal void SetResult(double result)
		{
			m_result = result;
		}
	}

	/// <summary>
	/// Statis Equation Solver Class.  It solves equations...
	/// </summary>
	public static class EquationEvaluator
	{
		private static List<string> KeyWords = new List<string>()
		{
			"length"
		};

		public static bool Evaluate(Equation labelToEvaluate, Sail sail, out double result)
		{
			return Evaluate(labelToEvaluate, sail, out result, false);
		}

		public static bool Evaluate(Equation labelToEvaluate, Sail sail, out double result, bool showBox)
		{
			if (labelToEvaluate == null)
			{
				result = 0;
				return false;
			}
			bool worked = false;

			Expression ex = new Expression(labelToEvaluate.EquationText, EvaluateOptions.IgnoreCase);

			List<KeyValuePair<string, Equation>> availableEqs = sail.GetEquations(labelToEvaluate);

			availableEqs.ForEach(eq => ex.Parameters[eq.Key] = eq.Value.Result);
			//Set up a custom delegate so NCalc will ask you for a parameter's value
			//   when it first comes across a variable

			ex.EvaluateParameter += delegate(string name, ParameterArgs args)
			{
				if (name.ToLower().Contains("length"))
					args.Result = EvaluateToDouble(name, sail);
			};

			try
			{
				result = (double)ex.Evaluate();
				labelToEvaluate.SetResult(result);
				worked = true;
			}
			catch(Exception exx)
			{
				worked = double.TryParse(labelToEvaluate.EquationText, out result);
				labelToEvaluate.SetResult(result);
				Warps.Logger.logger.Instance.LogErrorException(exx);
			}
			finally
			{
				if (!worked)
				{
					if(showBox)
						System.Windows.Forms.MessageBox.Show("Error parsing equation");
					result = double.NaN;
				}
			}

			return worked;
		}

		public static List<MouldCurve> ExtractCurves(Equation Equ, Sail sail)
		{
			List<MouldCurve> ret = new List<MouldCurve>();

			double result = 0;

			Expression ex = null;

			List<KeyValuePair<string, Equation>> availableEqs = sail.GetEquations(Equ);

			availableEqs.ForEach(eq => {
				if (Equ.EquationText.Contains(eq.Key))
				{
					ex = new Expression(eq.Value.EquationText, EvaluateOptions.IgnoreCase);
					ex.EvaluateParameter += delegate(string name, ParameterArgs args)
					{
						if (name.ToLower().Contains("length"))
						{
							args.Result = EvaluateToDouble(name, sail, ref ret);
						}
					};
					try
					{
						result = (double)ex.Evaluate();
					}
					catch
					{
						System.Windows.Forms.MessageBox.Show("Error parsing equation");
						result = double.NaN;
					}

					ex.Parameters[eq.Key] = eq.Value.Result;
				}
			});

			ex = new Expression(Equ.EquationText, EvaluateOptions.IgnoreCase);
			ex.EvaluateParameter += delegate(string name, ParameterArgs args)
			{
				if (name.ToLower().Contains("length"))
				{
					args.Result = EvaluateToDouble(name, sail, ref ret);
				}
			};

			try
			{
				result = (double)ex.Evaluate();
			}
			catch
			{
				//System.Windows.Forms.MessageBox.Show("Error parsing equation");
				result = double.NaN;
			}

			return ret;
		}

		private static double EvaluateToDouble(string entry, Sail sail)
		{
			if (sail == null)
				return Double.NaN;

			string oldEntry = entry;

			entry = entry.ToLower();

			double ret = 0;

			for (int i = 0; i < KeyWords.Count; i++)
			{
				if (entry.Contains(KeyWords[i]))
				{
					string curveName = entry.Replace(KeyWords[i], "");
					curveName = curveName.Replace("(", "");
					curveName = curveName.Replace(")", "");
					curveName = curveName.Replace("\"", "");
					curveName = curveName.Trim();
					ret = EvaluteKeyWordOnCurve(KeyWords[i], curveName, sail);
				}
			}

			return ret;
		}

		private static double EvaluateToDouble(string entry, Sail sail, ref List<MouldCurve> curves)
		{
			if (sail == null)
				return Double.NaN;

			string oldEntry = entry;

			entry = entry.ToLower();

			double ret = 0;

			for (int i = 0; i < KeyWords.Count; i++)
			{
				if (entry.Contains(KeyWords[i]))
				{
					string curveName = entry.Replace(KeyWords[i], "");
					curveName = curveName.Replace("(", "");
					curveName = curveName.Replace(")", "");
					curveName = curveName.Replace("\"", "");
					curveName = curveName.Trim();
					ret = EvaluteKeyWordOnCurve(KeyWords[i], curveName, sail, ref curves);
				}
			}

			return ret;
		}

		private static double EvaluteKeyWordOnCurve(string KeyWord, string curveName, Sail sail)
		{
			switch (KeyWord)
			{
				case "length":

					MouldCurve curve = sail.FindCurve(curveName);
					if (curve == null)
						return double.NaN;

					return curve.Length;

				default:

					return double.NaN;
			}
		}

		private static double EvaluteKeyWordOnCurve(string KeyWord, string curveName, Sail sail, ref List<MouldCurve> curves)
		{
			switch (KeyWord)
			{
				case "length":

					MouldCurve curve = sail.FindCurve(curveName);
					
					if (curve == null)
						return double.NaN;

					if (!curves.Contains(curve))
						curves.Add(curve);

					return curve.Length;

				default:

					return double.NaN;
			}
		}
	}
}
