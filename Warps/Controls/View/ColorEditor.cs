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
	public delegate void ColorChangedHandler(object sender, EventArgs<string[], Color> e);
	public partial class ColorEditor : UserControl
	{
		public static KeyValuePair<Form, ColorEditor> Show(ColorMap colors)
		{
			ColorEditor edit = new ColorEditor(colors);
			if (colors.Count() > 0)
			{
				edit.m_list.SelectedIndices.Clear();
				edit.m_list.SelectedIndices.Add(0);
			}
			Form f = new Form();
			f.Text = "Colors";
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			int pad = f.Size.Width - f.ClientRectangle.Width;
			int ypad = f.Size.Height - f.ClientRectangle.Height;
			f.Width = edit.Width + pad;
			f.Height = edit.Height + ypad;

			f.Controls.Add(edit);
			edit.Dock = DockStyle.Fill;

			f.Show();
			return new KeyValuePair<Form, ColorEditor>(f, edit);
		}
		public ColorEditor(ColorMap colors)
		{
			InitializeComponent();
			Colors = colors;
		}

		Color WheelColor
		{
			get { return Wheel.RGBColor; }
			set { Wheel.SetColor(value); }
		}

		ColorWheel Wheel
		{
			get { return colorWheel1; }
		}
		Color TextColor
		{
			get
			{
				int[] rgb = new int[3];
				int.TryParse(m_rTxt.Text, out rgb[0]);
				int.TryParse(m_gTxt.Text, out  rgb[1]);
				int.TryParse(m_bTxt.Text, out  rgb[2]);
				return Color.FromArgb( rgb[0], rgb[1], rgb[2]);
			}
			set
			{
				R = value.R;
				G = value.G;
				B = value.B;
			}
		}
		int R { set { m_rTxt.Text = value.ToString(); } }
		int G { set { m_gTxt.Text = value.ToString(); } }
		int B { set { m_bTxt.Text = value.ToString(); } }

		ColorMap m_map;
		public ColorMap Colors
		{
			get { return m_map; }
			set
			{
				m_map = value;
				RefreshListView();
			}
		}

		Color ListColor
		{
			get
			{
				if (SelectedItem != null)
					return SelectedItem.BackColor;
				return Color.Empty;
			}
			set
			{
				if (m_list.SelectedItems == null)
					return;
				foreach (ListViewItem lvi in m_list.SelectedItems)
				{
					lvi.BackColor = value;
					lvi.ForeColor = ColorMath.TextColor(value);
				}
				//if (SelectedItem != null)
				//{
				//	SelectedItem.BackColor = value;
				//	SelectedItem.ForeColor = ColorMath.TextColor(value);
				//}
			}
		}
		string ListString
		{
			get
			{
				if (SelectedItem != null)
					return SelectedItem.Text;
				return null;
			}
		}
		List<string> ListStrings
		{
			get
			{
				if( m_list.SelectedItems == null ) 
					return null;
				List<string> s = new List<string>(m_list.SelectedItems.Count);
				foreach (ListViewItem lvi in m_list.SelectedItems)
				{
					s.Add(lvi.Text);
				}
				return s;
			}
		}
		ListViewItem SelectedItem
		{
			get
			{
				if (m_list.SelectedItems != null && m_list.SelectedItems.Count > 0)
					return m_list.SelectedItems[0];
				else
					return null;
			}
		}
		void RefreshListView()
		{
			//m_list.FullRowSelect = true;
			if (m_map == null)
				m_list.Items.Clear();
			foreach (string s in m_map)
			{
				ListViewItem lvi = new ListViewItem(s);
				lvi.BackColor = m_map[s];
				lvi.ForeColor = ColorMath.TextColor(m_map[s]);
				m_list.Items.Add(lvi);
			}
		}

		private void m_list_SelectedIndexChanged(object sender, EventArgs e)
		{
			WheelColor = ListColor;
			TextColor = ListColor;
			Refresh();
			RaiseColorChanged();
		}

		private void colorWheel1_HueChanged(object sender, EventArgs e)
		{
			TextColor = WheelColor;
			ListColor = WheelColor;
			Refresh();
			RaiseColorChanged();

		}

		void RaiseColorChanged()
		{
			Colors[ListString] = WheelColor;
			if (ColorChanged != null)
				ColorChanged(this, new EventArgs<string[], Color>(ListStrings.ToArray(), WheelColor));
		}

		public event ColorChangedHandler ColorChanged;
	}
}
