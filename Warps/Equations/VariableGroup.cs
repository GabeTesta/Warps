using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Controls;

namespace Warps
{
	public class VariableGroup: Dictionary<string, Equation>, IGroup 
	{
		public VariableGroup()
		{ }

		public VariableGroup(string label, Sail s)
		{
			Label = label;
			m_sail = s;
		}

		public  Equation this[int index]
		{
			get
			{
				return this[this.Keys.ElementAt(index)];
			}
		}
		public new Equation this[string label]
		{
			get
			{
				if (this.ContainsKey(label))
					return base[label];
				return null;
			}
			set
			{
				if (this.ContainsKey(value.Label))
					base[value.Label] = value;
			}
		}

		public Equation Add(Equation curve)
		{
			if (base.ContainsKey(curve.Label))
				base[curve.Label] = curve;
			else
				base.Add(curve.Label, curve);
	
			return base[curve.Label];
		}

		#region IRebuild Members

		string m_label;
		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		bool m_locked = false;

		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			return WriteNode(true);
		}
		private TreeNode WriteNode(bool bclear)
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Tag = this;
			//	m_node.Text = GetType().Name + ": " + Label;
			m_node.Text = Label;
			m_node.ToolTipText = GetToolTipData();
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			if (m_node.Nodes.Count != this.Count || bclear)
			{
				m_node.Nodes.Clear();
				foreach (KeyValuePair<string, Equation> entry in this)
					m_node.Nodes.Add(entry.Value.WriteNode());
			}
			return m_node;
		}
		private string GetToolTipData()
		{
			return String.Format("{0}\n#:{1}", GetType().Name, Count);
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get { return null; }
		}
		public devDept.Eyeshot.Entities.Entity[] CreateEntities()
		{
			return null;
		}

		public void GetConnected(List<IRebuild> connected)
		{
			foreach (KeyValuePair<string, Equation> entry in this)
				if (entry.Value.Affected(connected))
				{
					connected.Add(entry.Value);
					break;
				}
		}
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			foreach (KeyValuePair<string, Equation> entry in this)
				entry.Value.GetParents(s, parents);

			//Equations inside a VariableGroup are allowed to be dependent on other
			//Equations in the same VariableGroup
			List<IRebuild> removeMe = new List<IRebuild>();
			for (int i = 0; i < parents.Count; i++)
			{
				if (this.ContainsKey((parents[i] as Equation).Label))
					removeMe.Add(parents[i]);
			}
			removeMe.ForEach(irb => parents.Remove(irb));
		}
		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = false;
			foreach (KeyValuePair<string, Equation> entry in this)
				bupdate |= entry.Value.Affected(connected);

			return bupdate;
		}
		public bool Delete() { return false; }
		public bool Update(Sail s)
		{
			bool bupdate = true;
			foreach (KeyValuePair<string, Equation> entry in this)
				bupdate &= entry.Value.Update(s);

			return bupdate;
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt == null || txt.Count == 0)
				return false;
			string[] splits = txt[0].Split(':');
			Label = "";
			if (splits.Length > 0)//extract label
				Label = splits[1];
			if (splits.Length > 1)//incase label contains ":"
				for (int i = 2; i < splits.Length; i++)
					Label += ":" + splits[i];
			Label = Label.Trim();

			for (int nLine = 1; nLine < txt.Count; )
			{
				IList<string> lines = ScriptTools.Block(ref nLine, txt);
				//nLine += lines.Count - 1;

				object equ = null;
				splits = lines[0].Split(':');
				if (splits.Length > 0)
					equ = Utilities.CreateInstance(splits[0].Trim('\t'));
				if (equ != null && equ is Equation)
				{
					//(equ as Equation).sail = Sail;
					(equ as Equation).ReadScript(Sail, lines);
					Add(equ as Equation);
				}
			}

			//this.Rebuild(null);

			return true;
		}
		public List<string> WriteScript()
		{
			List<string> script = new List<string>(Count * 3);
			script.Add(GetType().Name + ": " + Label);
			foreach (KeyValuePair<string, Equation> entry in this)
			{
				IList<string> mcScript = entry.Value.WriteScript();
				foreach (string s in mcScript)
					script.Add("\t" + s);
			}
			return script;
		}

		#endregion

		internal List<string> ParametersToString(object tag)
		{
			//tag is the watermark 
			List<string> ret = new List<string>();
			foreach (KeyValuePair<string, Equation> entry in this)
				ret.Add(String.Format("{0}", entry.Key));
			return ret;
		}
		internal IEnumerable<KeyValuePair<string,Equation>> GetEquations(object tag)
		{
			int i = -1;
			if (tag is Equation)
			{
				Equation tagEq = tag as Equation;

				for (int j = 0; j < this.Count; j++)
					if (this.ElementAt(j).Key == tagEq.Label)
						i = j;
			}
			else if (tag is VariableGroup)
			{
				return this.ToList();
			}
			//IAttribute atr = Attributes.Find((IAttribute atrib) => { return attribute.Label == atrib.Label; });
			if (i >= 0)
				return this.Take(i).ToList();
			return this.ToList();
		}

		#region IGroup Members

		Sail m_sail;
		public Sail Sail
		{
			get
			{
				return m_sail;
			}
			set
			{
				m_sail = value;
			}
		}

		public IRebuild FindItem(string label)
		{
			//search by label
			return this[label];
		}

		public IRebuild FindItem(IRebuild item)
		{
			if (!(item is Equation))
				return null;

			if (this.ContainsKey((item as Equation).Label))
				return this[(item as Equation).Label];
			return null;
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			if (tag is Equation)
				for (int nEq = 0; nEq < this.Count; nEq++)
				{
					if (this[nEq].Equals(tag))
						return true;
					rets.Add(this[nEq]);
				}
			else if (this == tag)
				return true;
			else
				rets.AddRange(this.Values);
			return false;
		}

		#endregion

		/// <summary>
		/// need this guy for tree reordering
		/// </summary>
		/// <param name="equation"></param>
		/// <returns></returns>
		internal int IndexOf(Equation equation)
		{
			for (int i = 0; i < this.Keys.Count; i++)
				if (this.Keys.ElementAt(i) == equation.Label)
					return i;

			return -1;
		}

		/// <summary>
		/// need this guy for tree reordering
		/// </summary>
		/// <param name="insertIndex"></param>
		/// <param name="equation"></param>
		internal void Insert(int insertIndex, Equation equation)
		{
			List<KeyValuePair<string, Equation>> dicList = this.ToList();
			dicList.Insert(insertIndex, new KeyValuePair<string, Equation>(equation.Label, equation));
			this.Clear();
			dicList.ForEach(pair => this.Add(pair.Key, pair.Value));
		}
	}
}
