﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps.Mixed
{
	[System.Diagnostics.DebuggerDisplay("{Label} Count={Count}", Name = "{Label}", Type = "{GetType()}")]
	public class MixedGroup : List<IRebuild>, IGroup
	{
		public MixedGroup() :this("", null){ }
		public MixedGroup(string label, Sail s) : this(label, s, null) { }
		public MixedGroup(string label, Sail s, string layer)
		{
			m_label = label;
			m_sail = s;
			m_layer = layer;
		}

		public void WriteBin(System.IO.BinaryWriter bin) { }

		string m_label;
		Sail m_sail;
		string m_layer;

		#region IGroup Members

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
			foreach (IRebuild r in this)
			{
				//if (label == r.Label)
				if( label.Equals(r.Label, StringComparison.InvariantCultureIgnoreCase))
					return r;
				if (r is IGroup)//handle nested groups
				{
					IRebuild ret = (r as IGroup).FindItem(label);
					if (ret != null)
						return ret;
				}
			}
			return null;
		}

		public bool ContainsItem(IRebuild obj)
		{
			bool bfound = false;
			this.ForEach(irb =>
			{
				if (obj == irb)
				{
					bfound = true;
					return;
				}
				if (irb is IGroup) 
					if ((irb as IGroup).ContainsItem(obj))
					{
						bfound = true;
						return;
					} 
			});
			return bfound;
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			if (tag == this)
				return true;

			foreach (IRebuild irb in this)
			{
				if (irb == tag)
					return true;
				if( !(irb is IGroup ) )
					rets.Add(irb);
				else
					if( (irb as IGroup).Watermark(tag, ref rets))//if a subgroup watermarks return true
						return true;
			}
			return false;

			//int i = -1;
			//i = this.IndexOf(tag);

			//if (i >= 0)
			//	rets.AddRange(this.Take(i));
			//else
			//	rets.AddRange(this);

			//return i != -1;//true if found		
		}

		public bool FindParent<T>(IRebuild item, out T parent) where T : class, IGroup
		{
			if (Contains(item))
			{
				parent = this as T;
				return parent != null;
			}
			foreach (IRebuild r in this)
				if (r is IGroup && (r as IGroup).FindParent(item, out parent))
					return true;

			parent = null;
			return false;
		}
		#endregion

		#region IRebuild Members

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

		public string Layer
		{
			get { return m_layer; }
			//get { return m_layer == null ? "Default" : m_layer; }
		}

		//public List<string> WriteScript()
		//{
		//	List<string> script = new List<string>();
		//	script.Add(ScriptTools.Label(GetType().Name, Label));
		//	this.ForEach(ir =>
		//	{
		//		IList<string> mcScript = ir.WriteScript();
		//		foreach (string s in mcScript)
		//			script.Add("\t" + s);
		//	});
		//	return script;
		//}

		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	Label = ScriptTools.ReadLabel(txt[0]);
		//	string[] splits;
		//	for (int nLine = 1; nLine < txt.Count; )
		//	{
		//		IList<string> lines = ScriptTools.Block(ref nLine, txt);
		//		//nLine += lines.Count - 1;

		//		IRebuild cur = null;
		//		splits = lines[0].Split(':');
		//		if (splits.Length > 0)
		//			cur = Utilities.CreateInstance<IRebuild>(splits[0].Trim('\t'));
		//		if (cur != null)
		//		{
		//			cur.ReadScript(sail, lines);
		//			Add(cur);
		//			sail.Add(this);//keep sail up to date for interdependent children
		//		}
		//	}
		//	return true;
		//}

		public bool Locked
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.Nodes.Clear();
			this.ForEach(r => m_node.Nodes.Add(r.WriteNode()));
			return m_node;
		}

		public List<devDept.Eyeshot.Entities.Entity> CreateEntities()
		{
			List<devDept.Eyeshot.Entities.Entity> ents = new List<devDept.Eyeshot.Entities.Entity>();
			this.ForEach(r => ents.AddRange(r.CreateEntities()));
			return ents;
		}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> ents = new List<devDept.Eyeshot.Labels.Label>();
				this.ForEach(r => ents.AddRange(r.EntityLabel));
				return ents;
			}
		}

		public void GetChildren(List<IRebuild> updated)
		{
			this.ForEach(r => r.GetChildren(updated));
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			this.ForEach(irb => irb.GetParents(s, parents));
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bAff = Count > 0;
			this.ForEach(irb => bAff &= irb.Affected(connected));
			return bAff;
		}

		public bool Update(Sail s)
		{
			bool success = true;
			this.ForEach(r => success &= r.Update(s));
			WriteNode();
			return success;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlNode node = NsXml.MakeNode(doc, this);
			foreach (IRebuild rbd in this)
				node.AppendChild(rbd.WriteXScript(doc));

			return node;
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			Label = NsXml.ReadLabel(node);
			m_sail = sail;
			//Sail.Add(this);//keep sail up to date for interdependent children
			foreach (System.Xml.XmlNode child in node)
			{
				IRebuild cur = Utilities.CreateInstance<IRebuild>(child.Name);
				if (cur != null)
				{
					Add(cur);
					cur.ReadXScript(sail, child);
				}
			}
		}

		#endregion

		#region TreeDragging Members

		public bool CanInsert(Type item)
		{
			return true;
		}

		public void Insert(IRebuild item, IRebuild target)
		{
			Utilities.Insert(this, item, target);
			//int nTar = IndexOf(target);
			//int nIrb = IndexOf(item);
			//if (nIrb >= 0)//item is already in this group: reorder
			//	Remove(item);
			//Insert(nTar, item);
		}

		#endregion

		#region Flattening Members

		public void FlatLayout(List<IRebuild> flat)
		{
			ForEach(cur => {
				flat.Add(cur);
				if (cur is IGroup)
					(cur as IGroup).FlatLayout(flat);
			});
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Label);
		}
	}
}