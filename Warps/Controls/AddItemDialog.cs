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
	public partial class AddItemDialog : Form
	{
		public AddItemDialog()
		{
			InitializeComponent();
			m_type.DisplayMember = "Name";
			m_list.MultiSelect = false;
		}

		List<Type> useMe = null;

		public AddItemDialog(List<Type> availableTypes)
			:this()
		{
			useMe = new List<Type>(availableTypes);
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
			get { return m_list.SelectedItems != null && m_list.SelectedItems.Count > 0 ? m_list.SelectedItems[0].Tag as Type : null; }
		}		
		//public Type Type
		//{
		//	get { return m_type.SelectedItem != null ? m_type.SelectedItem as Type : null; }
		//	set{ if(m_type.Items.Contains(value) ) m_type.SelectedItem = value; }
		//}
		void SortItems()
		{
			if( useMe == null )
				useMe = Utilities.GetAllOf(typeof(IGroup), false);
			useMe.Sort((a, b) =>
			{
				if (a.Name.Contains("Group"))
				{
					if (b.Name.Contains("Group"))
						return a.Name.CompareTo(b.Name);
					else
						return 1;// groups are "greater" than non-groups
				}
				else
				{
					if (b.Name.Contains("Group"))
						return -1;// groups are "greater" than non-groups
					else
						return a.Name.CompareTo(b.Name);
				}
			});
		}
		void PopulateCombo()
		{
			SortItems();
			//m_type.DataSource = useMe;
			m_list.View = View.SmallIcon;
			m_list.SmallImageList = WarpFrame.Images;
			useMe.ForEach(typ =>
			{
				ListViewItem item = m_list.Items.Add(typ.FullName, typ.Name, typ.Name);
				item.Tag = typ;
			});
		}

		public IRebuild CreateIRebuild()
		{
			if (Type == null)
				return null;
			IRebuild grp = Utilities.CreateInstance(Type) as IRebuild;
			if (grp != null)
				grp.Label = Label;
			return grp;
		}
	}
}
