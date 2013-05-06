using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System.Windows.Forms;

namespace Warps
{
	//[Serializable()]
	public class CurveGroup : List<MouldCurve>, IGroup
	{
		public CurveGroup()
		{ }

		public CurveGroup(string label, Sail sail)
		{
			m_label = label;
			m_sail = sail;
		}

		public CurveGroup(System.IO.BinaryReader bin, Sail sail)
		{
			m_sail = sail;
			m_label = Utilities.ReadCString(bin);
			int iC = bin.ReadInt32();
			for (int nC = 0; nC < iC; nC++)
				Add(new MouldCurve(bin, sail));
		}

		//List<MouldCurve> m_curves = new List<MouldCurve>();
		string m_label;
		Sail m_sail;

		public Sail Sail
		{
			get { return m_sail; }
			set { m_sail = value; }
		}

		public MouldCurve this[string label]
		{
			get
			{
				return this.Find((MouldCurve c) =>
				{
					return c.Label == label;
				});
			}
		}

		public new MouldCurve Add(MouldCurve curve)
		{
			if (this.Contains(curve))
				return null;

			base.Add(curve);
			return curve;
		}

		#region IGroup Members

		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

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
			//m_node.Text = GetType().Name + ": " + Label;
			m_node.Text = Label;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			if (m_node.Nodes.Count != this.Count || bclear)
			{
				m_node.Nodes.Clear();
				foreach (MouldCurve c in this)
					m_node.Nodes.Add(c.WriteNode());
			}
			return m_node;
		}

		CurveGroupEditor m_editor;

		public System.Windows.Forms.Control Editor
		{
			get
			{
				UpdateEditor();
				return m_editor;
			}
			set
			{
				m_editor = value as CurveGroupEditor;
				if (value is CurveGroupEditor)
					ReadEditor();
			}
		}
		public void UpdateEditor()
		{
			if (m_editor == null)
				m_editor = new CurveGroupEditor();
			m_editor.Tag = this;
			m_editor.Label = Label;
			m_editor.Count = this.Count;
			int i = 0;
			foreach (MouldCurve c in this)
				m_editor[i++] = c;
		}
		public void ReadEditor()
		{
			if (this.Count != m_editor.Count)
				this.Clear();
			for (int i = 0; i < m_editor.Count; i++)
			{
				Add(m_editor[i]);
			}
		}

		/// <summary>
		/// loops over every object in its group array and draw the data if there is any
		/// </summary>
		/// <returns></returns>
		public Entity[] CreateEntities()
		{
			List<Entity> ret = new List<Entity>();

			this.ForEach(curve =>
				{
					List<Entity> ents = curve.CreateEntities().ToList();
					if (ents != null)
						ret.AddRange(ents);
				});


			return ret.ToArray();
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				var ret = GetCurveLabels();
				if (ret != null)
					return ret.ToArray();
				else
					return null;
			}
		}

		private List<devDept.Eyeshot.Labels.Label> GetCurveLabels()
		{
			List<devDept.Eyeshot.Labels.Label> ret = new List<devDept.Eyeshot.Labels.Label>();

			this.ForEach(cur => { ret.AddRange(cur.EntityLabel); });

			return ret;
		}

		/// <summary>
		/// this method searches for curves and fitpoints
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool ContainsObject(object tag, out MouldCurve refCurv)
		{
			refCurv = null;
			if (this.Contains(tag))
				return true;

			if (tag is IFitPoint)
			{
				foreach (MouldCurve cur in this)
				{
					if (cur.Contains(tag as IFitPoint))
					{
						refCurv = cur;
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		internal IEnumerable<MouldCurve> GetCurves(object tag)
		{
			int i = -1;
			if (tag is MouldCurve) i = this.IndexOf(tag as MouldCurve);
			//IAttribute atr = Attributes.Find((IAttribute atrib) => { return attribute.Label == atrib.Label; });
			else if (tag is IFitPoint)
			{
				foreach (MouldCurve mc in this)
					if (mc.FitPoints.Contains(tag))
					{
						i = this.IndexOf(mc);
						break;
					}
			}
			if (i >= 0)
				return this.Take(i);
			return this;
		}

		internal IEnumerable<MouldCurve> GetAllCurves()
		{
			List<MouldCurve> ret = new List<MouldCurve>();
			foreach (MouldCurve mc in this)
				ret.Add(mc);

			return ret;
		}

		#region IRebuild Members

		//public bool Rebuild(List<IRebuild> updated)
		//{
		//	bool bupdate = updated == null;
		//	this.ForEach((MouldCurve c) => { bupdate |= c.Rebuild(updated); });
		//	return bupdate;
		//}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = false;
			this.ForEach((MouldCurve c) => { bupdate |= c.Affected(connected); });
			return bupdate; 
		}
		public bool Update() { return true; }
		public bool Update(Sail s) { return true; }
		public bool Delete() { return false; }
		public void GetConnected(List<IRebuild> connected)
		{
			this.ForEach((MouldCurve c) => 
			{ 
				if (c.Affected(connected))
					connected.Add(c); 
			});
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

				object cur = null;
				splits = lines[0].Split(':');
				if (splits.Length > 0)
					cur = Utilities.CreateInstance(splits[0].Trim('\t'));
				if (cur != null && cur is MouldCurve)
				{
					(cur as MouldCurve).Sail = Sail;
					(cur as MouldCurve).ReadScript(Sail, lines);
					Add(cur as MouldCurve);
					Sail.Add(this);
				}
			}

			//this.Rebuild(null);

			return true;
		}

		public List<string> WriteScript()
		{
			List<string> script = new List<string>(Count * 3);
			script.Add(GetType().Name + ": " + Label);
			foreach (MouldCurve mc in this)
			{
				IList<string> mcScript = mc.WriteScript();
				foreach (string s in mcScript)
					script.Add("\t" + s);
			}
			return script;
		}

		#endregion

	}
}
