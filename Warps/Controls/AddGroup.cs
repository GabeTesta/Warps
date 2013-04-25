using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class AddGroup : Form
	{
		public AddGroup()
		{
			InitializeComponent();
		}
		private void AddGroup_Load(object sender, EventArgs e)
		{
			PopulateCombo();
		}
		public string Label
		{
			get { return m_name.Text; }
			set { m_name.Text = value; }
		}
		public Type Type
		{
			get { return m_type.SelectedItem != null ? m_type.SelectedItem as Type : null; }
			set{ if(m_type.Items.Contains(value) ) m_type.SelectedItem = value; }
		}
		void PopulateCombo()
		{
			List<Type> grouptypes = Utilities.GetAllOf(typeof(IGroup),false);
			//if (grouptypes.Contains(typeof(IGroup)))
			//grouptypes.Remove(typeof(IGroup));
			m_type.Items.Clear();
			m_type.Items.AddRange(grouptypes.ToArray());
			if (grouptypes.Count > 0)
				m_type.SelectedIndex = 0;
		}
		public IGroup CreateGroup()
		{
			if (Type == null)
				return null;
			IGroup grp = Utilities.CreateInstance(Type) as IGroup;
			if (grp != null)
				grp.Label = Label;
			return grp;
		}
	}
}
