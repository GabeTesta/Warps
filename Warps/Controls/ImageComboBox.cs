using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Warps
{
	class ImageComboBox : ComboBox
	{
		public ImageComboBox()
		{
			DrawMode = DrawMode.OwnerDrawFixed;
			DropDownStyle = ComboBoxStyle.DropDownList;
		}

		// Draws the items into the ColorSelector object
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();
			e.DrawFocusRectangle();

			if (e.Index >= 0)
			{
				DropDownImage item = (DropDownImage)Items[e.Index];
				// Draw the colored 16 x 16 square

				Rectangle bounds = e.Bounds;//get the item bounds

				//if (bounds.Width > bounds.Height)//make it square
					bounds.Width = bounds.Height;
				//else if (bounds.Height > bounds.Width)
				//	bounds.Height = bounds.Width;

				e.Graphics.DrawImage(item.Image, bounds);
				// Draw the label
				e.Graphics.DrawString(item.Label, e.Font, new
					   SolidBrush(e.ForeColor), bounds.Right, e.Bounds.Top + 2);

			}
			base.OnDrawItem(e);
		}
	}

	public class DropDownImage
	{
		public string Label
		{
			get { return value; }
			set { this.value = value; }
		}
		private string value;

		public Image Image
		{
			get { return img; }
			set { img = value; }
		}
		private Image img;

		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}
		private object tag;

		public DropDownImage(string label, Image image, object type)
		{
			value = label;
			img = image;
			tag = type;
		}
		//public DropDownItem(string val)
		//{
		//	value = val;
		//	this.img = new Bitmap(16, 16);
		//	Graphics g = Graphics.FromImage(img);
		//	Brush b = new SolidBrush(Color.FromName(val));
		//	g.DrawRectangle(Pens.White, 0, 0, img.Width, img.Height);
		//	g.FillRectangle(b, 1, 1, img.Width - 1, img.Height - 1);
		//}

		public override string ToString()
		{
			return value;
		}
	}
}
