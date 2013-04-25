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
	public partial class AddNewCurveControl : UserControl
	{
		public AddNewCurveControl()
		{
			InitializeComponent();
			Normal = m_addBtn.BackColor;
		}

		Color OverAdd = Color.Lime;
		Color OverDelete = Color.Pink;
		Color Normal;		
		private void button_MouseEnter(object sender, EventArgs e)
		{
			if ((sender as Button) == m_addBtn)
				(sender as Button).BackColor = OverAdd;
			else if((sender as Button) == m_delBtn)
				(sender as Button).BackColor = OverDelete;
		}

		private void button_Leave(object sender, EventArgs e)
		{
			(sender as Button).BackColor = Normal;
		}
		
	}
}
