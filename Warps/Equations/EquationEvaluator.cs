using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;
using System.Windows.Forms;
using Warps.Curves;

namespace Warps
{
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
			return Evaluate(labelToEvaluate, sail.Watermark(labelToEvaluate), out result, false);
		}

		public static bool Evaluate(Equation labelToEvaluate, List<IRebuild> watermark, out double result)
		{
			return Evaluate(labelToEvaluate, watermark, out result, false);
		}

		public static bool Evaluate(Equation equation, List<IRebuild> watermark, out double result, bool showBox)
		{
			if (equation == null)
			{
				result = 0;
				return false;
			}

			bool worked = false;

			Expression ex = new Expression(equation.EquationText, EvaluateOptions.IgnoreCase);

			//Set up a custom delegate so NCalc will ask you for a parameter's value when it first comes across a variable
			IEnumerable<Equation> avails = watermark.OfType<Equation>();
			foreach (Equation eq in avails)
				ex.Parameters[eq.Label] = eq.Value;

			//set the function point to 
			ex.EvaluateFunction += delegate(string FunctionName, FunctionArgs args)
			{
				args.Result = EvaluateToDouble(args.Parameters[0].ParsedExpression.ToString(), FunctionName.ToLower(), watermark);
			};

			try
			{
				result = Convert.ToDouble(ex.Evaluate());
				equation.m_result = result;
				worked = true;
			}
			catch (Exception exx)
			{
				worked = double.TryParse(equation.EquationText, out result);
				equation.m_result = result;
				Logleton.TheLog.LogErrorException(exx);
			}
			finally
			{
				if (!worked)
				{
					if (showBox)
						System.Windows.Forms.MessageBox.Show("Error parsing equation");
					equation.m_result = double.NaN;
				}
			}

			return worked;
		}

		private static double EvaluateToDouble(string entry, string function, List<IRebuild> watermarks)
		{
			//if (sail == null)
			//	return Double.NaN;

			string oldEntry = entry;

			//entry = entry.ToLower();

			double ret = 0;

			for (int i = 0; i < KeyWords.Count; i++)
			{
				if (function.Contains(KeyWords[i]))
				{
					entry = entry.Replace("[", "");
					string curveName = entry.Replace("]", "");
					ret = EvaluteKeyWordOnCurve(KeyWords[i], curveName, watermarks);
				}
			}

			return ret;
		}

		private static double EvaluteKeyWordOnCurve(string KeyWord, string curveName, List<IRebuild> watermarks)
		{
			switch (KeyWord)
			{
				case "length":
					IRebuild irb = watermarks.Find(item => item.Label.Equals(curveName, StringComparison.InvariantCultureIgnoreCase));
					if (irb != null)
					{
						IMouldCurve curve = irb as IMouldCurve;
						if (curve != null)
							return curve.Length;
					}
					return double.NaN;

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

		public static List<string> ListParameters(Equation Equ)
		{
			List<string> param = new List<string>();
			Expression ex = new Expression(Equ.EquationText);

			ex.EvaluateFunction += delegate(string name, FunctionArgs args)
			{
				if (name == "Length")
					param.Add(args.Parameters[0].ToString());
				args.Result = 1;
			};

			ex.EvaluateParameter += delegate(string name, ParameterArgs args)
			{
				param.Add(name);
				args.Result = 1;
			};
			if (ex.HasErrors())
				MessageBox.Show(ex.Error);
			ex.Evaluate();
			return param;
		}

		public static List<MouldCurve> ExtractCurves(string eq, Sail sail)
		{
			List<MouldCurve> param = new List<MouldCurve>();
			Expression ex = new Expression(eq);

			ex.EvaluateFunction += delegate(string name, FunctionArgs args)
			{
				if (name == "Length")
					param.Add(sail.FindCurve(args.Parameters[0].ToString()));
				args.Result = 1;
			};

			ex.EvaluateParameter += delegate(string name, ParameterArgs args)
			{
				param.AddRange(ExtractCurves(name, sail));
				args.Result = 1;
			};

			ex.Evaluate();
			return param;
		}
	}
}
