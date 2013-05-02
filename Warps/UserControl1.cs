using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	public partial class UserControl1 : UserControl
	{
		public UserControl1()
		{
			InitializeComponent();
			Label.Text = "Lu_Up";
			Length.Text = "15.234";
		}

		private void UserControl1_Load(object sender, EventArgs e)
		{
			Random r = new Random();
			foreach (Control c in Controls)
				if (c is ImageComboBox)
				{
					//(c as ImageComboBox).Items.Add(new DropDownImage("Fixed Point", Warps.Properties.Resources.fixedpt));
					//(c as ImageComboBox).Items.Add(new DropDownImage("Curve Point", Warps.Properties.Resources.curvept));
					//(c as ImageComboBox).Items.Add(new DropDownImage("Slide Point", Warps.Properties.Resources.slidept));

				}
				else if (c is TextBox)
					c.Text = r.NextDouble().ToString("f4");
		}
	}
}
