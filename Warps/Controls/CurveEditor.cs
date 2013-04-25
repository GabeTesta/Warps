using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Controls;

namespace Warps
{
	public partial class CurveEditor : UserControl
	{
		public CurveEditor()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			//m_flow.BackColor = Color.SeaGreen;
		}

		public CurveEditor(MouldCurve curve)
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			m_curve = curve;
			//m_flow.BackColor = Color.SeaGreen;
		}

		bool dataChanged = false;

		MouldCurve m_curve = null;

		internal MouldCurve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
		}
		public string Label
		{
			get { return m_lblBox.Text; }
			set { m_lblBox.Text = value != null ? value : ""; }
		}
		public double Length
		{
			set { m_length.Text = string.Format("Length: {0:g5}", value); m_length.Invalidate(); }
		}
		public IFitPoint[] FitPoints
		{
			set
			{
				m_flow.SuspendLayout();
				//foreach(Control c in m_flow.Controls )
				//	if (c is IFitEditor)
				//		(c as IFitEditor).ReturnPress -= CurveEditor_ReturnPress;
				m_flow.Controls.Clear();
				foreach (IFitPoint fp in value)
				{
					Add(fp);
				}

				m_flow.ResumeLayout();
			}
		}
		public int Count
		{
			get { return m_flow.Controls.Count; }
			set
			{
				if (value > Count)
				{
					while (value > Count)
						Add(new FixedPoint());
				}
				else if (value < Count)
				{
					while (value < Count )
						m_flow.Controls.RemoveAt(value);
				}
			}
		}

		void Add(IFitPoint fp)
		{
			PointTypeSwitcher pt = fp.WriteEditor(null);

			pt.Size = new System.Drawing.Size(406, 28);
			//pt.Curves = AvailableCurves;
			m_flow.Controls.Add(pt);
			pt.panel1.Size = new System.Drawing.Size(305, 25);
			//m_flow.Controls[m_flow.Controls.Count - 1].Size = new System.Drawing.Size(410, 30);
			pt.MouseDown += m_flow_MouseDown;
			pt.MouseMove += m_flow_MouseMove;
			pt.MouseUp += m_flow_MouseUp;
			pt.ReturnPress += CurveEditor_ReturnPress;
		}

		void CurveEditor_ReturnPress(object sender, KeyEventArgs e)
		{
			int index = -1;
			if (sender is IFitEditor)
				index = m_flow.Controls.IndexOf(sender as Control);
			else if (sender is EquationBoxAuto)
				index = m_flow.Controls.IndexOf((sender as EquationBoxAuto).Parent);
			if (ReturnPress != null)
				ReturnPress(sender, new EventArgs<int>(index));
		}
		public event EventHandler<EventArgs<int>> ReturnPress;

		public PointTypeSwitcher this[int index]
		{
			get { return m_flow.Controls[index] as PointTypeSwitcher; }
		}

		PointTypeSwitcher m_drag;
		Color m_bak;

		bool m_highLighted = false;

		public bool HighLighted
		{
			get { return m_highLighted; }
			set { m_highLighted = value; }
		}

		private void m_flow_MouseDown(object sender, MouseEventArgs e)
		{
			PointTypeSwitcher c = MouseOver(sender, e);
			if (c != null)
			{
				m_drag = c;
				m_bak = m_drag.BackColor;
				ReColor(m_drag);
			}
		}

		private void m_flow_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_drag == null || e.Button != System.Windows.Forms.MouseButtons.Left)
				return;
			PointTypeSwitcher c = MouseOver(sender, e);
			ReColor(c);
		}

		private void m_flow_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_drag != null)
			{
				PointTypeSwitcher c = MouseOver(sender, e);
				if (c != null)
				{
					int n = m_flow.Controls.IndexOf(m_drag);
					int i = m_flow.Controls.IndexOf(c);
					if (i >= 0)
						m_flow.Controls.SetChildIndex(m_drag, i);
				}
				ReColor(null);
				m_drag = null;
			}
		}
		PointTypeSwitcher MouseOver(object sender, MouseEventArgs e)
		{
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return null;
			PointTypeSwitcher c = null;
			if (sender is FlowLayoutPanel)
				c = m_flow.GetChildAtPoint(e.Location) as PointTypeSwitcher;
			else if (sender is PointTypeSwitcher)
			{
				Point p = e.Location;
				p = (sender as PointTypeSwitcher).PointToScreen(p);
				p = m_flow.PointToClient(p);
				c = m_flow.GetChildAtPoint(p) as PointTypeSwitcher;
			}
			return c;
		}
		private void ReColor(PointTypeSwitcher mouseover)
		{
			for (int i = 0; i < Count; i++)
				this[i].SetColor = m_bak;
			if (mouseover != null)
			{
				mouseover.SetColor = Color.MediumSeaGreen;
				m_drag.SetColor = Color.LightSteelBlue;

				//int ndrag = m_flow.Controls.IndexOf(m_drag);
				//int ntarg = m_flow.Controls.IndexOf(mouseover);
				//if (ndrag > 0 && ntarg > 0)
				//{
				//	for (int i = 0; i < Count; i++)
				//	{
				//		int nd = i;
				//		if(i > ndrag)
				//			nd = i+1;
				//		this[i].Index = nd
				//	}
				//}
			}
		}

		private void m_add_Click(object sender, EventArgs e)
		{
			Add(new FixedPoint());
		}

		private void button1_Click(object sender, EventArgs e)
		{
			List<PointTypeSwitcher> removeMe = new List<PointTypeSwitcher>();
			for (int i = 0; i < Count; i++)
				if (this[i].Selected)
					removeMe.Add(this[i]);

			if (removeMe.Count > 0)
			{
				if(MessageBox.Show("Delete selected items?", "Warps", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					removeMe.ForEach(pts => m_flow.Controls.Remove(pts));
			}
			
		}

		//public event EventHandler Commit;
		//private void CurveEditor_KeyUp(object sender, KeyEventArgs e)
		//{
		//	if (e.KeyCode == Keys.Enter)
		//	{
		//		if (Commit != null)
		//			Commit(this, new EventArgs());
		//	}
		//}

		//private void CurveEditor_KeyDown(object sender, KeyEventArgs e)
		//{
		//	int i = 0;
		//}

		public bool Editing
		{
			get { return dataChanged; }
			set { dataChanged = value; }
		}
	}
}
