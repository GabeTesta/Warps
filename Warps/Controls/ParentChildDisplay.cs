using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps.Controls
{
	public partial class ParentChildDisplay : UserControl
	{
		public static Pair<Form, ParentChildDisplay> Show(Control owner, IRebuild item, List<IRebuild> parents, List<IRebuild> children)
		{
			ParentChildDisplay pcd = new ParentChildDisplay();
			pcd.Item = item;
			pcd.Parents = parents;
			pcd.Children = children;

			Form f = new Form();
			f.Owner = owner.TopLevelControl as Form;
			f.Size = new Size(300, 400);
			f.StartPosition = FormStartPosition.CenterParent;
			f.Controls.Add(pcd);
			pcd.Dock = DockStyle.Fill;
			f.Show();
			return new Pair<Form, ParentChildDisplay>(f, pcd);
		}
		public ParentChildDisplay()
		{
			InitializeComponent();
		}
		IRebuild m_item = null;
		IRebuild Item
		{
			set
			{
				m_item = value;
				if (m_item != null)
					m_itemLabel.Text = m_item.ToString();//string.Format("{0} [{1}]", value.GetType().Name, value.Label);
				else
					m_itemLabel.Text = "";
			}
			get { return m_item; }
		}

		List<IRebuild> Parents
		{
			set
			{
				m_parentList.Items.Clear();
				if (value != null)
				{
					//value.ForEach(par => m_parentList.Items.Add(string.Format("{0} [{1}]", par.GetType().Name, par.Label)));
					m_parentList.Items.AddRange(value.ToArray());
				}
			}
		}
		List<IRebuild> Children
		{
			set
			{
				m_childList.Items.Clear();
				if (value != null)
				{
					value.ForEach(par => {
						if (par != Item)
							m_childList.Items.Add(par.ToString());
							//m_childList.Items.Add(string.Format("{0} [{1}]", par.GetType().Name, par.Label));
					});

					//m_childList.Items.AddRange(value.ToArray());
				}
				//if(Item != null )
				//	m_childList.Items.Remove(Item);
			}
		}
	}
}
