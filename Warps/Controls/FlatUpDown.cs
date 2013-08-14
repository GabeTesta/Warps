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
	public partial class FlatUpDown : UserControl
	{
		public FlatUpDown()
		{
			InitializeComponent();
			m_up.Text = m_down.Text = "";
			m_up.Click += button_Click;
			m_down.Click += button_Click;
			m_up.Paint += button_Paint;
			m_down.Paint += button_Paint;
			//m_up.Click += UpClick;
			//m_down.Click += DownClick;
		}
		Pen m_pen = new Pen(Color.Black, 1);
		void button_Paint(object sender, PaintEventArgs e)
		{
			Button b = sender as Button;
			//e.Graphics.DrawLine(m_pen, b.Width / 4, b.Height / 2, b.Width * 3 / 4, b.Height / 2);
			//if (b == m_up)
			//	e.Graphics.DrawLine(m_pen, b.Width / 2, b.Height / 5, b.Width / 2, b.Height * 4 / 5);

			int head = b == m_up ? m_up.Height / 3 : m_down.Height * 3 / 4;
			int tail = b == m_up ? m_up.Height * 3 / 4 : m_down.Height / 3;

			Point[] tri = new Point[4];
			tri[0] = new Point(b.Width / 2, head);
			tri[1] = new Point(b.Width / 4, tail);
			tri[2] = new Point(b.Width * 3 / 4, tail);
			tri[3] = new Point(b.Width / 2, head);

			//e.Graphics.FillPolygon(Brushes.Black, tri);
			e.Graphics.DrawPolygon(m_pen, tri);
			//e.Graphics.DrawLine(m_pen, tri[0], tri[1]);
			//e.Graphics.DrawLine(m_pen, tri[0], tri[2]);
			//e.Graphics.DrawLine(m_pen, tri[1], tri[2]);

			Rectangle r = b.ClientRectangle;
			r.Width -= 1;
			if( b != m_up )
				r.Height -= 1;
			//e.Graphics.DrawRectangle(m_pen, r);
			if (b == m_down)
				e.Graphics.DrawLine(m_pen, 0, 0, m_down.Width, 0);


		}

		void button_Click(object sender, EventArgs e)
		{
			if (sender == m_up)
				if (UpClick != null)
					UpClick(this, e);

			if (sender == m_down)
				if (DownClick != null)
					DownClick(this, e);
		}
		public event EventHandler UpClick;
		public event EventHandler DownClick;
		Button this[int i]
		{
			get { return i == 0 ? m_up : m_down; }
		}
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			m_up.Height = Height / 2;
			m_down.Height = Height / 2;
			m_down.Top = m_up.Bottom;
		}
	}
}
