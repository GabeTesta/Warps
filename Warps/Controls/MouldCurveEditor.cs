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
	public partial class MouldCurveEditor : UserControl
	{
		public MouldCurveEditor()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			//m_panel.Paint += m_panel_Paint;
		}

		public string Label
		{
			get { return m_label.Text; }
			set { m_label.Text = value; }
		}
		public double Length
		{
			set { m_length.Text = value.ToString("f4"); }
		}

		//public IFitEditor this[int i]
		//{
		//	get { return m_edits[i]; }
		//}

		CheckBox[] m_girths;
		ImageComboBox[] m_combos;
		IFitEditor[] m_edits;

		ControlCollection Panel
		{ get { return m_panel.Controls; } }

		ImageComboBox ImageBox(Type ptType)
		{
			ImageComboBox box = new ImageComboBox();
			box.Width = 38;
			box.DropDownWidth = 75;
			box.DropDownStyle = ComboBoxStyle.DropDownList;
			box.FlatStyle = FlatStyle.Flat;
			box.SelectionChangeCommitted += box_SelectionChangeCommitted;
			box.IntegralHeight = false;

			box.Items.Add(new DropDownImage("Point", Warps.Properties.Resources.fixedpt, typeof(FixedPoint)));
			box.Items.Add(new DropDownImage("Curve", Warps.Properties.Resources.curvept, typeof(CurvePoint)));
			box.Items.Add(new DropDownImage("Slide", Warps.Properties.Resources.slidept, typeof(SlidePoint)));

			foreach (DropDownImage img in box.Items)
			{
				if (img.Tag.Equals(ptType))
				{
					box.SelectedItem = img;
					break;
				}
			}
			return box;
		}
		CheckBox GirthCheck(bool bChecked)
		{
			CheckBox check = new CheckBox();
			check.Text = "";
			check.Width = 15;
			check.FlatStyle = FlatStyle.Flat;
			check.Checked = bChecked;
			return check;
		}

		public void ReadCurve(MouldCurve c)
		{
			if (c == null)
			{
				Panel.Clear();
				return;
			}
			//Label = c.Label;
			Length = c.Length;

			if (m_edits == null || c.FitPoints.Length != m_edits.Length)
			{
				Panel.Clear();
				m_girths = new CheckBox[c.FitPoints.Length - 1];
				m_combos = new ImageComboBox[c.FitPoints.Length];
				m_edits = new IFitEditor[c.FitPoints.Length];
			}
			Control ptBox = null;
			Control old = null;
			//get the list of available curves from the sail
			object[] autofill =  c.Sail.GetAutoFillData(c).ToArray();

			//create the point controls and add them to the panel
			for (int i = 0; i < c.FitPoints.Length; i++)
			{
				//create the type-speific point editor
				old = m_edits[i] as Control;
				ptBox = c[i].WriteEditor(ref m_edits[i]);
				m_edits[i].AutoFillData = autofill;
				//remove old control if new pointeditor
				if (old != ptBox)
					Panel.Remove(old);
				Panel.Add(ptBox);

				//create the type selection combobox
				if (m_combos[i] == null)
					m_combos[i] = ImageBox(m_edits[i].FitType);
				else
					SetCombo(m_combos[i], m_edits[i].FitType);
				Panel.Add(m_combos[i]);
				
				//create the segment checkboxes
				if (i < c.FitPoints.Length - 1)
				{
					old = m_girths[i];
					m_girths[i] = GirthCheck(c.IsGirth(i));
					if (old != m_girths[i])
						Panel.Remove(old);
					Panel.Add(m_girths[i]);
				}
			}
			//force the layout of the panel controls
			PerformLayout();
			m_panel.Invalidate();//invalidate the panel for next redraw
		}
		public void WriteCurve(MouldCurve c)
		{
			//c.Label = Label;

			bool[] girths = new bool[m_girths.Length];
			int i =0;
			foreach (CheckBox b in m_girths)
				girths[i++] = b.Checked;

			i = 0;
			IFitPoint[] points =  new IFitPoint[m_edits.Length];
			foreach (IFitEditor fe in m_edits)
			{
				points[i++] = fe.CreatePoint();
				points[i - 1].Update(c.Sail);
			}

			c.Fit(points, girths);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			if (m_edits == null)
				return;
			
			// distribute the point editors and checkboxes down the panel
			int top = 0, CHK = 0;
			Control ptBox = null;
			for (int i = 0; i < m_edits.Length; i++)
			{
				ptBox = m_edits[i] as Control;

				m_combos[i].Top = top;
				ptBox.Top = top;

				m_combos[i].Height = ptBox.Height = 21;

				// make a checkbox for internal segments
				if (i < m_edits.Length - 1)
				{
					m_girths[i].Top = m_combos[i].Bottom - (int)((double)m_girths[i].Height / 2.0);
					m_girths[i].Left = 0;
				}

				if (CHK == 0)//initialize checkbox offset
					CHK = m_girths[i].Right;

				m_combos[i].Left = CHK;
				ptBox.Left = m_combos[i].Right;
				ptBox.Width = m_panel.Width - ptBox.Left - 5;

				top = ptBox.Bottom;
			}
			m_panel.Height = top;
			this.Height = m_panel.Bottom;
			Invalidate();//redraw the panel
		}

		void box_SelectionChangeCommitted(object sender, EventArgs e)
		{
			ImageComboBox box = sender as ImageComboBox;
			if (box == null || !m_combos.Contains(sender))
				return;

			int nFit = 0;
			for (nFit = 0; nFit < m_combos.Length; nFit++)
				if (m_combos[nFit] == box)
					break;

			if (nFit == m_combos.Length)
				return;

			DropDownImage item = box.SelectedItem as DropDownImage;
			SetType(nFit, item.Tag as Type);
		}

		void SetCombo(ImageComboBox box, Type fpType)
		{
			//search the dropdowns for the type you want
			foreach (DropDownImage fitpt in box.Items)
				if (fitpt.Tag.Equals(fpType))//find the correct item
				{
					if (fitpt != box.SelectedItem)//dont reselect
						box.SelectedItem = fitpt;
					break;
				}

		}

		void SetType(int nFit, Type fitPointType)
		{
			//set the type selection in the combo box
			SetCombo(m_combos[nFit], fitPointType);

			//replace the old control with the correct one
			Control old = m_edits[nFit] as Control;
			//create a new one
			object fit = Utilities.CreateInstance(fitPointType);
			Vect2 uv = (old as IFitEditor).CreatePoint().UV;
			(fit as IFitPoint).UV = uv;//copy over the uv coords if possible
			Control ptBox = (fit as IFitPoint).WriteEditor(ref m_edits[nFit]);
			ptBox.Top = old.Top;
			ptBox.Left = old.Left;

			Panel.Remove(old);//remove the old control
			Panel.Add(ptBox);//add the new one
			PerformLayout();
		}

		//void m_panel_Paint(object sender, PaintEventArgs e)
		//{
		//	if (m_types == null)
		//		return;

		//	Point inp, mdp, lop;
		//	for (int i = 0; i < m_types.Length; i++)
		//	{
		//		inp = new Point(m_types[i].Left, (m_types[i].Top + m_types[i].Bottom) / 2);
		//		if (i < m_types.Length - 1)
		//		{
		//			mdp = new Point((m_girths[i].Left + m_girths[i].Right)/2, inp.Y);
		//			lop = new Point(mdp.X, m_girths[i].Top);
		//			e.Graphics.DrawLine(Pens.Black, mdp, lop);
		//		}
		//		else
		//			mdp = new Point((m_girths[i-1].Left + m_girths[i-1].Right) / 2, inp.Y);

		//		if (i >= 1)
		//		{
		//			lop = new Point(mdp.X, m_girths[i - 1].Bottom);
		//			e.Graphics.DrawLine(Pens.Black, mdp, lop);
		//		}
		//		e.Graphics.DrawLine(Pens.Black, inp, mdp);
		//	}
		//}
		//public void ReadCurve(MouldCurve c)
		//{
		//	//Label = c.Label;
		//	Length = c.Length;

		//	Panel.Clear();
		//	m_girths = new CheckBox[c.FitPoints.Length - 1];
		//	m_types = new ImageComboBox[c.FitPoints.Length];
		//	m_edits = new IFitEditor[c.FitPoints.Length];

		//	int top = 0, CHK = 0;
		//	Control ptBox = null;
		//	for( int i = 0; i< c.FitPoints.Length; i++)
		//	{
		//		ptBox = c[i].WriteEditor(ref m_edits[i]);

		//		m_types[i] = ImageBox(ptBox.Tag as Type);

		//		m_types[i].Top = top;
		//		ptBox.Top = top;

		//		if (i < c.FitPoints.Length - 1)
		//		{
		//			m_girths[i] = GirthCheck(c.IsGirth(i));
		//			m_girths[i].Top = m_types[i].Bottom - (int)((double)m_girths[i].Height / 2.0);
		//			m_girths[i].Left = 0;
		//			Panel.Add(m_girths[i]);
		//		}

		//		if (CHK == 0)//initialize checkbox offset
		//			CHK = m_girths[i].Right;

		//		m_types[i].Left = CHK;
		//		ptBox.Left = m_types[i].Right;

		//		top += m_types[i].Height;
		//		Panel.Add(m_types[i]);
		//		Panel.Add(ptBox);
		//	}
		//	m_panel.Invalidate();//redraw the panel
		//}
	}
}
