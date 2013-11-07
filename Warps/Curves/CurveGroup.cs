using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System.Windows.Forms;
using System.Xml;

namespace Warps.Curves
{
	//[Serializable()]
	[System.Diagnostics.DebuggerDisplay("{Label} Count={Count}", Name = "{Label}", Type = "{GetType()}")]
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
			m_layer = "Spiral Curves";
			m_label = Utilities.ReadCString(bin);
//#if DEBUG
//			m_layer = "Mould";
//#endif
			m_locked = true;
			int iC = bin.ReadInt32();
			for (int nC = 0; nC < iC; nC++)
				Add(new MouldCurve(bin, sail));
		}
		public CurveGroup(System.IO.BinaryReader bin)
			: this(bin, WarpFrame.CurrentSail) { }

		public void WriteBin(System.IO.BinaryWriter bin)
		{
			Utilities.WriteCString(bin, GetType().ToString());
			Utilities.WriteCString(bin, m_label);
			bin.Write((Int32)Count);
			ForEach(mc => mc.WriteBin(bin));
		}

		public MouldCurve this[string label]
		{
			get
			{
				return this.Find((MouldCurve c) =>
				{
					return c.Label.Equals(label, StringComparison.InvariantCultureIgnoreCase);
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

		#region IRebuild Members

		string m_label;
		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}
		public string Layer
		{
			get { return m_layer != null ? m_layer : "Curves"; }
		}
		string m_layer;

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			return WriteNode(true);
		}
		private TreeNode WriteNode(bool bclear)
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.ToolTipText = GetToolTipData();
			if (m_node.Nodes.Count != this.Count || bclear)
			{
				//m_node.BeginEdit();
				m_node.Nodes.Clear();
				foreach (MouldCurve c in this)
					m_node.Nodes.Add(c.WriteNode());
			//	m_node.EndEdit(false);
			}
			return m_node;
		}
		private string GetToolTipData()
		{
			if(Locked)
				return String.Format("{0}\n#:{1}\n{2}", GetType().Name, Count, "Locked");
			return String.Format("{0}\n#:{1}", GetType().Name, Count);
		}

		//CurveGroupEditor m_editor;
		//public System.Windows.Forms.Control Editor
		//{
		//	get
		//	{
		//		UpdateEditor();
		//		return m_editor;
		//	}
		//	set
		//	{
		//		m_editor = value as CurveGroupEditor;
		//		if (value is CurveGroupEditor)
		//			ReadEditor();
		//	}
		//}
		//public void UpdateEditor()
		//{
		//	if (m_editor == null)
		//		m_editor = new CurveGroupEditor();
		//	m_editor.Tag = this;
		//	m_editor.Label = Label;
		//	m_editor.Count = this.Count;
		//	int i = 0;
		//	foreach (MouldCurve c in this)
		//		m_editor[i++] = c;
		//}
		//public void ReadEditor()
		//{
		//	if (this.Count != m_editor.Count)
		//		this.Clear();
		//	for (int i = 0; i < m_editor.Count; i++)
		//	{
		//		Add(m_editor[i]);
		//	}
		//}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				var ret = GetCurveLabels();
				if (ret != null)
					return ret;
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
		public List<Entity> CreateEntities()
		{
			List<Entity> ret = new List<Entity>();

			this.ForEach(curve =>
				{
					List<Entity> ents = curve.CreateEntities().ToList();
					if (ents != null)
						ret.AddRange(ents);
				});


			return ret;
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = false;
			this.ForEach((MouldCurve c) => { bupdate |= c.Affected(connected); });
			return bupdate;
		}
		public bool Update(Sail s) { return true; }
		public bool Delete() { return false; }
		public void GetChildren(List<IRebuild> connected)
		{
			this.ForEach((MouldCurve c) =>
			{
				if (c.Affected(connected))
					connected.Add(c);
			});
		}
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			foreach (MouldCurve mc in this)
				mc.GetParents(s, parents);

			//curves inside a curvegroup are allowed to be dependent on other
			//curves in the same curvegroup
			List<IRebuild> removeMe = new List<IRebuild>();
			for (int i = 0; i < parents.Count; i++)
			{
				if (this.Contains(parents[i] as MouldCurve))
					removeMe.Add(parents[i]);
			}
			removeMe.ForEach(irb => parents.Remove(irb));
		}

		//[Obsolete]
		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	if (txt == null || txt.Count == 0)
		//		return false;
		//	Label = ScriptTools.ReadLabel(txt[0]);
		//	string[] splits;// = txt[0].Split(':');
		//	sail.Add(this);
		//	//Label = "";
		//	//if (splits.Length > 0)//extract label
		//	//	Label = splits[1];
		//	//if (splits.Length > 1)//incase label contains ":"
		//	//	for (int i = 2; i < splits.Length; i++)
		//	//		Label += ":" + splits[i];
		//	//Label = Label.Trim();

		//	for (int nLine = 1; nLine < txt.Count; )
		//	{
		//		IList<string> lines = ScriptTools.Block(ref nLine, txt);
		//		//nLine += lines.Count - 1;

		//		MouldCurve cur = null;
		//		splits = lines[0].Split(':');
		//		if (splits.Length > 0)
		//			cur = Utilities.CreateInstance<MouldCurve>(splits[0].Trim('\t'));
		//		if (cur != null )
		//		{
		//			cur.Sail = sail;
		//			cur.ReadScript(sail, lines);
		//			Add(cur);
		//		}
		//	}

		//	//this.Rebuild(null);

		//	return true;
		//}
		//[Obsolete]
		//public List<string> WriteScript()
		//{
		//	List<string> script = new List<string>(Count * 3);
		//	script.Add(ScriptTools.Label(GetType().Name,Label));
		//	foreach (MouldCurve mc in this)
		//	{
		//		IList<string> mcScript = mc.WriteScript();
		//		foreach (string s in mcScript)
		//			script.Add("\t" + s);
		//	}
		//	return script;
		//}

		bool m_locked = false;

		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		#endregion

		//internal IEnumerable<MouldCurve> GetCurves(object tag)
		//{
		//	int i = -1;
		//	if (tag is MouldCurve) i = this.IndexOf(tag as MouldCurve);
		//	//IAttribute atr = Attributes.Find((IAttribute atrib) => { return attribute.Label == atrib.Label; });
		//	else if (tag is IFitPoint)
		//	{
		//		foreach (MouldCurve mc in this)
		//			if (mc.FitPoints.Contains(tag))
		//			{
		//				i = this.IndexOf(mc);
		//				break;
		//			}
		//	}
		//	if (i >= 0)
		//		return this.Take(i);
		//	return this;
		//}
		//public bool ContainsObject(object tag, out MouldCurve refCurv)
		//{
		//	refCurv = null;
		//	if (this.Contains(tag))
		//		return true;

		//	if (tag is IFitPoint)
		//	{
		//		foreach (MouldCurve cur in this)
		//		{
		//			if (cur.Contains(tag as IFitPoint))
		//			{
		//				refCurv = cur;
		//				return true;
		//			}
		//		}
		//	}

		//	return false;
		//}
		//internal IEnumerable<MouldCurve> GetAllCurves()
		//{
		//	List<MouldCurve> ret = new List<MouldCurve>();
		//	foreach (MouldCurve mc in this)
		//		ret.Add(mc);

		//	return ret;
		//}

		#region IGroup Members

		Sail m_sail;
		public Sail Sail
		{
			get { return m_sail; }
			set { m_sail = value; }
		}

		public IRebuild FindItem(string label)
		{
			//search by label
			return this[label];
		}

		public bool ContainsItem(IRebuild item)
		{
			if (!(item is MouldCurve))
				return false;

			return this.Contains(item as MouldCurve);
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			int i = -1;
			if (tag == this)
				return true;

			if (tag is MouldCurve) 
				i = this.IndexOf(tag as MouldCurve);

			if (i >= 0)
				rets.AddRange(this.Take(i));
			else
				rets.AddRange(this);
			
			return i != -1;//true if found
		}

		public bool FindParent<T>(IRebuild item, out T parent) where T : class, IGroup
		{
			if (ContainsItem(item))
			{
				parent = this as T;
				return true;
			}
			parent = null;
			return false;
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}

		#region IRebuild Members

		public XmlNode WriteXScript(XmlDocument doc)
		{
			XmlElement node = NsXml.MakeNode(doc, this);
			this.ForEach(mc=> node.AppendChild(mc.WriteXScript(doc)));
			return node;
		}

		public void ReadXScript(Sail sail, XmlNode node)
		{
			Label = NsXml.ReadLabel(node);
			m_sail = sail;
			foreach (XmlNode child in node.ChildNodes)
			{
				MouldCurve cur = Utilities.CreateInstance<MouldCurve>(child.Name);
				if (cur != null)
				{
					//(cur as MouldCurve).Sail = sail;
					Add(cur);
					cur.ReadXScript(sail, child);
				}
			}
		}

		#endregion

		#region TreeDragging Members

		public bool CanInsert(Type item)
		{
			return Utilities.GetAllOf(typeof(MouldCurve), true).Contains(item);
		}
		public void Insert(IRebuild item, IRebuild target)
		{
			Utilities.Insert(this, item, target);
			//int nTar = IndexOf(target as MouldCurve);
			//int nIrb = IndexOf(item as MouldCurve);
			//if (nIrb >= 0)//item is already in this group: reorder
			//	Remove(item);
			//Utilities.LimitRange(0, ref nTar, Count);
			//Insert(nTar, item as MouldCurve);
		}

		public bool Remove(IRebuild item)
		{
			return base.Remove(item as MouldCurve);
		}

		#endregion

		#region Flattening Members

		public void FlatLayout(List<IRebuild> flat)
		{
			ForEach(cur => flat.Add(cur));
		}

		#endregion
	}
}