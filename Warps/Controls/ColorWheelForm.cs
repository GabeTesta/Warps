using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot;

namespace Warps.Controls
{
	public partial class ColorWheelForm : Form
	{
		public ColorWheelForm()
		{
			InitializeComponent();
		}

		public ColorWheelForm(SingleViewportLayout view)
		{
			InitializeComponent();
			Text = view.Name;
			m_view = view;
			this.DoubleBuffered = true;
			colorWheel1.SetColor(view.Background.TopColor);
		}

		public ColorWheelForm(Layer layer)
		{
			InitializeComponent();
			Text = layer.Name;
			m_layer = layer;
			DoubleBuffered = true;
			colorWheel1.SetColor(layer.Color);
		}

		SingleViewportLayout m_view = null;

		Layer m_layer = null;

		private void colorWheel1_HueChanged(object sender, EventArgs e)
		{
			if (m_view != null)
			{
				m_view.Background.TopColor = colorWheel1.RGBColor;
				m_view.Refresh();
			}
			else if (m_layer != null)
			{
				m_layer.Color = colorWheel1.RGBColor;
				
			}
			this.Invalidate();
		}

		private void colorWheel1_SLChanged(object sender, EventArgs e)
		{
			if (m_view != null)
			{
				m_view.Background.TopColor = colorWheel1.RGBColor;
				m_view.Refresh();
			}
			else if (m_layer != null)
			{
				m_layer.Color = colorWheel1.RGBColor;
			}
			this.Invalidate();

		}

		private void ColorWheelForm_Paint(object sender, PaintEventArgs e)
		{
			//e.Graphics.Clear(colorWheel1.RGBColor);
		}
	}
}
