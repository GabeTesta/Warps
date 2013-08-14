using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	public class MixedGroup : List<IRebuild>, IGroup
	{
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
				if (label == r.Label)
					return r;
			return null;
		}

		public IRebuild FindItem(IRebuild obj)
		{
			throw new NotImplementedException();
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			if (tag == this)
				return true;

			int i = -1;
			i = this.IndexOf(tag);

			if (i >= 0)
				rets.AddRange(this.Take(i));
			else
				rets.AddRange(this);

			return i != -1;//true if found		
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
			get { return m_layer == null ? "Default" : m_layer; }
		}

		public List<string> WriteScript()
		{
			throw new NotImplementedException();
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		public bool Locked
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		System.Windows.Forms.TreeNode m_node;
		public System.Windows.Forms.TreeNode WriteNode()
		{
			if (m_node == null) m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = Label;
			m_node.Tag = this;
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

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				List<devDept.Eyeshot.Labels.Label> ents = new List<devDept.Eyeshot.Labels.Label>();
				this.ForEach(r => ents.AddRange(r.EntityLabel));
				return ents.ToArray();
			}
		}

		public void GetConnected(List<IRebuild> updated)
		{
			this.ForEach(r => r.GetConnected(updated));
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			throw new NotImplementedException();
		}

		public bool Affected(List<IRebuild> connected)
		{
			throw new NotImplementedException();
		}

		public bool Update(Sail s)
		{
			bool success = true;
			this.ForEach(r => success &= r.Update(s));
			return success;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
