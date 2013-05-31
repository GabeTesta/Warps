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

		List<Type> useMe = null;

		public AddGroup(List<Type> useThese)
		{
			InitializeComponent();
			useMe = new List<Type>(useThese);
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
			if (useMe == null)
			{
				List<Type> grouptypes = Utilities.GetAllOf(typeof(IGroup), false);
				m_type.Items.Clear();
				m_type.Items.AddRange(grouptypes.ToArray());
				if (grouptypes.Count > 0)
					m_type.SelectedIndex = 0;
			}
			else
			{
				m_type.Items.Clear();
				m_type.Items.AddRange(useMe.ToArray());
				if (useMe.Count > 0)
					m_type.SelectedIndex = 0;
			}
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
