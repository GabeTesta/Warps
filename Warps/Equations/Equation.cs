﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public class Equation: IRebuild
	{
		public Equation() { m_label = ""; m_text = null; Value = 0; }
		public Equation(string equationText) : this(null, equationText) { }
		public Equation(double value)
		{
			m_label = "";
			Value = value;
		}
		public Equation(string label, string equationText)
		{
			m_label = label == null ? equationText : label;
			EquationText = equationText;
		}

		public Equation(string label, double value)
		{
			m_label = label;
			//m_text = value.ToString();
			Value = value;
		}

		string m_text = null;
		string m_label = null;
		internal double m_result = double.NaN;

		System.Windows.Forms.TreeNode m_node = null;

		public bool IsNumber()
		{
			return m_text == null;
		}

		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}
		public string Layer
		{
			get { return "EQ"; }
		}

		public string EquationText
		{
			get { return m_text == null ? m_result.ToString("f4"): m_text; }
			set 
			{
				m_text = value;
				if (double.TryParse(m_text, out m_result))
					m_text = null;//numeric text entry
			}
		}

		public double Value
		{
			get { return m_result; }
			set { m_text = null; m_result = value; }
		}

		///// <summary>
		///// Attempt to set the EquationText and Result to value (if equation uses an expression, this will return false)
		///// </summary>
		///// <param name="val">double to attempt to set</param>
		///// <returns>true if equation is a number and can be set this way, false if equation is expression</returns>
		//public bool SetValue(double val)
		//{
		//	if (IsNumber())
		//	{
		//		Value = val;
		//		return true;
		//	}
		//	return false;
		//}


		public double Evaluate(Sail s)
		{
			if (IsNumber())
				return Value;

			if (s == null) 
				return double.NaN;
			if (EquationEvaluator.Evaluate(this, s, out m_result))
				return Value;

			return double.NaN;
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities()
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
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			List<string> labels = EquationEvaluator.ListParameters(this);
			if (labels.Count > 0)
			{
				IRebuild rbld;
				foreach (string lbl in labels)
				{
					rbld = s.FindItem(lbl);
					if (rbld != null)
						parents.Add(rbld);
				}
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

		bool m_locked = false;
		public bool Locked { get { return m_locked; } set { m_locked = value; } }

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
			script.Add(depth + "\t" + ToScriptString());
			return script;
		}
		public List<string> WriteScript()
		{
			return WriteScript("");
		}
		public TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new TreeNode();
			m_node.Text = Label;
			m_node.ImageKey = m_node.SelectedImageKey = "Equation";
			m_node.Tag = this;
			m_node.ToolTipText = this.ToScriptString() + "=" + Value;
			m_node.Name = Label;

			TreeNode tmp1 = new TreeNode(string.Format("Text: {0}", EquationText));
			tmp1.ImageKey = tmp1.SelectedImageKey = "EquationText";

			TreeNode tmp2 = new TreeNode(string.Format("Result: {0}", Value));
			tmp2.ImageKey = tmp2.SelectedImageKey = "Result";
			m_node.Nodes.Clear();
			m_node.Nodes.Add(tmp1);
			m_node.Nodes.Add(tmp2);
			
			return m_node;
		}

		public override string ToString()
		{
			return Label;
		}

		public string ToScriptString()
		{
			return string.Format("{0}:{1}", Label, EquationText);
		}

		Warps.Controls.VariableEditor GetEditor()
		{
			Warps.Controls.VariableEditor eq = new Warps.Controls.VariableEditor(Label, this);
			//eq.AutoFillData = sail.GetAutoFillData(this).ToArray();
			//eq.sail = sail;
			return eq;
		}

		public Warps.Controls.VariableEditor WriteEditor(Warps.Controls.VariableEditor edit)
		{
			if (edit == null)
				edit = new Warps.Controls.VariableEditor(Label, this);
			Warps.Controls.VariableEditor ee = edit as Warps.Controls.VariableEditor;
			if (ee == null)
				ee = new Warps.Controls.VariableEditor(Label, this);
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

	}
}
