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

namespace Warps.Curves
{
	public partial class MouldCurveEditor : UserControl
	{
		public MouldCurveEditor()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			//m_panel.Paint += m_panel_Paint;
		}

		#region Members

		public string Label
		{
			get { return m_label.Text; }
			set { m_label.Text = value; }
		}
		public double Length
		{
			set { m_length.Text = value.ToString("f4"); }
		}
		public List<object> AutoFill
		{
			get { return m_autoFill; }
			set { m_autoFill = value; }
		}
		ControlCollection Panel
		{ get { return m_panel.Controls; } }

		internal List<CheckBox> m_girths;
		internal List<ImageComboBox> m_combos;
		internal IFitEditor[] m_edits;
		internal List<FlatUpDown> m_updowns;

		List<object> m_autoFill;

		#endregion		

		#region Curve IO

		public void ReadCurve(MouldCurve c)
		{
			if (c == null || c.FitPoints == null)
			{
				Panel.Clear();
				Length = 0;
				return;
			}
			//Label = c.Label;
			Length = c.Length;
			SuspendLayout();
	
			if (m_edits == null || c.FitPoints.Length != m_edits.Length)
			{
				Panel.Clear();
				m_girths = new List<CheckBox>(new CheckBox[c.FitPoints.Length - 1]);
				m_combos = new List<ImageComboBox>(new ImageComboBox[c.FitPoints.Length]);
				m_edits = new IFitEditor[c.FitPoints.Length];
				m_updowns = new List<FlatUpDown>(new FlatUpDown[c.FitPoints.Length]);
			}
			Control ptBox = null;
			Control old = null;
			////get the list of available curves from the sail
			//object[] autofill =  c.Sail.GetAutoFillData(c).ToArray();
			//create the point controls and add them to the panel
			for (int i = 0; i < c.FitPoints.Length; i++)
			{
				//create the type-speific point editor
				old = m_edits[i] as Control;
				ptBox = c.FitPoints[i].WriteEditor(ref m_edits[i]);
				if (AutoFill != null)
					m_edits[i].AutoFillData = AutoFill;
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

				//create the updowns
				if (m_updowns[i] == null)
					m_updowns[i] = UpDown();
				Panel.Add(m_updowns[i]);

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
			ResumeLayout(true);
			PerformLayout();
			m_panel.Invalidate();//invalidate the panel for next redraw
		}

		public void WriteCurve(MouldCurve c)
		{
			//c.Label = Label;
			if (m_edits != null)
			{
				bool[] girths = new bool[m_girths.Count];
				int i = 0;
				foreach (CheckBox b in m_girths)
					girths[i++] = b.Checked;

				i = 0;
				IFitPoint[] points = new IFitPoint[m_edits.Length];
				foreach (IFitEditor fe in m_edits)
				{
					points[i++] = CurveTools.CreatePoint(fe);// fe.CreatePoint();
					//points[i - 1].Update(c); //update now happens when refitting curve
				}

				c.Fit(points, girths);
			}
			else
				c.UnFit();
		}

		#endregion

		#region Factories

		ImageComboBox ImageBox(Type ptType)
		{
			ImageComboBox box = new ImageComboBox();
			box.Width = 38;
			box.DropDownWidth = 75;
			box.DropDownStyle = ComboBoxStyle.DropDownList;
			box.FlatStyle = FlatStyle.Flat;
			box.SelectionChangeCommitted += box_SelectionChangeCommitted;
			box.IntegralHeight = false;
			box.TabStop = false;

			box.Items.Add(new DropDownImage("Point", Warps.Properties.Resources.FitFixed, typeof(FixedPoint)));
			box.Items.Add(new DropDownImage("Curve", Warps.Properties.Resources.FitCurve, typeof(CurvePoint)));
			box.Items.Add(new DropDownImage("Slide", Warps.Properties.Resources.FitSlide, typeof(SlidePoint)));
			box.Items.Add(new DropDownImage("Cross", Warps.Properties.Resources.FitCross, typeof(CrossPoint)));
			box.Items.Add(new DropDownImage("Offset", Warps.Properties.Resources.FitOffset, typeof(OffsetPoint)));
			box.Items.Add(new DropDownImage("Angle", Warps.Properties.Resources.FitAngle, typeof(AnglePoint)));

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
			check.TabStop = false;
			return check;
		}
		FlatUpDown UpDown()
		{
			FlatUpDown ud = new FlatUpDown();
			ud.UpClick += ud_UpClick;
			ud.DownClick += ud_DownClick;
			ud.TabStop = false;
			return ud;
		}
		
		#endregion

		#region UpDowns

		void ud_DownClick(object sender, EventArgs e)
		{
			if (!(sender is FlatUpDown))
				return;
			int nFit = m_updowns.IndexOf(sender as FlatUpDown);
			if (nFit >= 0 && nFit < m_updowns.Count - 1)
				Swap(nFit, nFit + 1);

		}

		void ud_UpClick(object sender, EventArgs e)
		{
			if (!(sender is FlatUpDown))
				return;
			int nFit = m_updowns.IndexOf(sender as FlatUpDown);
			if (nFit > 0 && nFit < m_updowns.Count)
				Swap(nFit, nFit - 1);
		}

		void Swap(int n1, int n2)
		{
			//swap combo's and editors
			ImageComboBox box;
			box = m_combos[n1];
			m_combos[n1] = m_combos[n2];
			m_combos[n2] = box;

			IFitEditor edit;
			edit = m_edits[n1];
			m_edits[n1] = m_edits[n2];
			m_edits[n2] = edit;
			PerformLayout();

			//FlatUpDown ud;
			//ud = m_updowns[n1];
			//m_updowns[n1] = m_updowns[n2];
			//m_updowns[n2] = ud;
		}

		#endregion

		#region Combos

		void box_SelectionChangeCommitted(object sender, EventArgs e)
		{
			ImageComboBox box = sender as ImageComboBox;
			if (box == null || !m_combos.Contains(sender))
				return;

			int nFit = 0;
			for (nFit = 0; nFit < m_combos.Count; nFit++)
				if (m_combos[nFit] == box)
					break;

			if (nFit == m_combos.Count)
				return;

			DropDownImage item = box.SelectedItem as DropDownImage;
			SetPointType(nFit, item.Tag as Type);
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

		void SetPointType(int nFit, Type fitPointType)
		{
			//set the type selection in the combo box
			SetCombo(m_combos[nFit], fitPointType);

			//replace the old control with the correct one
			Control old = m_edits[nFit] as Control;
			//create a new one
			IFitPoint fit = Utilities.CreateInstance<IFitPoint>(fitPointType);
			Vect2 uv = CurveTools.CreatePoint(old as IFitEditor).UV;
			//Vect2 uv = (old as IFitEditor).CreatePoint().UV;
			//fit.UV = uv;//copy over the uv coords if possible
			Control ptBox = (fit as IFitPoint).WriteEditor(ref m_edits[nFit]);
			m_edits[nFit].AutoFillData = AutoFill;
			//m_edits[nFit]
			//m_edits[nFit].ReturnPress += ReturnPress;

			ptBox.Top = old.Top;
			ptBox.Left = old.Left;

			Panel.Remove(old);//remove the old control
			Panel.Add(ptBox);//add the new one
			PerformLayout();
		}
		
		#endregion

		#region Popup Menu

		int m_popuped = -1;
		private void m_Popup_Opened(object sender, EventArgs e)
		{
			m_popuped = GetIndexUnderMouse();
			m_insert.Enabled = m_delete.Enabled = m_popuped >= 0;
		}

		private int GetIndexUnderMouse()
		{
			Point mouse = m_panel.PointToClient(System.Windows.Forms.Cursor.Position);
			//Control c = m_panel.GetChildAtPoint(mouse);
			//if (c == null)
			//{
			return m_updowns.FindIndex(ud => Utilities.IsBetween(ud.Top, mouse.Y, ud.Bottom));
				//foreach (FlatUpDown ud in m_updowns)
				//	if (Utilities.IsBetween(ud.Top, mouse.Y, ud.Bottom))
				//		return m_updowns.IndexOf(ud);
			//}
			//int nFit = -1;
			//if (c is IFitEditor)
			//{
			//	for (nFit = 0; nFit < m_edits.Length; nFit++)
			//		if (m_edits[nFit] == c)
			//			break;
			//	if (nFit == m_edits.Length) nFit = -1;
			//}
			//else if (c is ImageComboBox)
			//{
			//	m_combos.IndexOf(c as ImageComboBox);
			//}
			//else if (c is FlatUpDown)
			//{
			//	nFit = m_updowns.IndexOf(c as FlatUpDown);
			//}
			//return nFit;
		}

		private void m_add_Click(object sender, EventArgs e)
		{
			if (m_edits == null)
			{
				m_girths = new List<CheckBox>();
				m_combos = new List<ImageComboBox>();
				m_edits = new IFitEditor[0];
				m_updowns = new List<FlatUpDown>();
			}
			//new array of edits
			IFitEditor[] edits = new IFitEditor[m_edits.Length + 1];
			m_edits.CopyTo(edits, 0);//copy existing
			//append new fixedpoint
			new FixedPoint().WriteEditor(ref edits[m_edits.Length]);
			if (AutoFill != null)//set autofills
				edits[m_edits.Length].AutoFillData = AutoFill;
			//add combo/girth/updown
			m_combos.Add(ImageBox(typeof(FixedPoint)));
			m_updowns.Add(UpDown());
			if (edits.Length > 1)
			{
				m_girths.Add(GirthCheck(false));
				//add controls to panel
				Panel.Add(m_girths.Last());
			}
			Panel.Add(m_combos.Last());
			Panel.Add(edits[m_edits.Length] as Control);
			Panel.Add(m_updowns.Last());

			m_edits = edits;//save new array
			PerformLayout();//redistribute
			m_panel.Invalidate();
		}

		private void m_insert_Click(object sender, EventArgs e)
		{
			if (m_popuped < 0)
				return;
			//new array of edits
			List<IFitEditor> edits = new List<IFitEditor>(m_edits);
			//append new fixedpoint
			IFitEditor edit = null;
			new FixedPoint().WriteEditor(ref edit);
			if (AutoFill != null)//set autofills
				edit.AutoFillData = AutoFill;

			//add combo/girth/updown
			edits.Insert(m_popuped, edit);
			m_combos.Insert(m_popuped, ImageBox(typeof(FixedPoint)));
			m_girths.Insert(m_popuped, GirthCheck(false));
			m_updowns.Insert(m_popuped, UpDown());

			//add controls to panel
			Panel.Add(m_girths[m_popuped]);
			Panel.Add(m_combos[m_popuped]);
			Panel.Add(edit as Control);
			Panel.Add(m_updowns[m_popuped]);

			m_edits = edits.ToArray();//save new array
			PerformLayout();//redistribute
			m_panel.Invalidate();
		}

		private void m_delete_Click(object sender, EventArgs e)
		{
			if (m_popuped < 0)
				return;
			//new array of edits
			List<IFitEditor> edits = new List<IFitEditor>(m_edits);

			//remove controls from panel
			int nGir = Math.Min(m_popuped, m_girths.Count-1);//account for girth count
			Panel.Remove(m_girths[nGir]);
			Panel.Remove(m_combos[m_popuped]);
			Panel.Remove(m_edits[m_popuped] as Control);
			Panel.Remove(m_updowns[m_popuped]);

			//remove selected point
			edits.RemoveAt(m_popuped);
			m_combos.RemoveAt(m_popuped);
			m_girths.RemoveAt(nGir);
			m_updowns.RemoveAt(m_popuped);

			m_edits = edits.ToArray();//save new array
			PerformLayout();//redistribute
			m_panel.Invalidate();
		}
		
		#endregion

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			if (m_edits == null)
				return;
			m_panel.SuspendLayout();
			//m_panel.BackColor = colls[nCol++%colls.Length];
			//BackColor = colls[nCol++ % colls.Length]; 
			// distribute the point editors and checkboxes down the panel
			int top = 0, CHK = 0;
			Control ptBox = null;
			for (int i = 0; i < m_edits.Length; i++)
			{
				ptBox = m_edits[i] as Control;
				if (ptBox.ContextMenuStrip != m_Popup)
					ptBox.ContextMenuStrip = m_Popup;

				ptBox.TabIndex = i;
				ptBox.TabStop = true;

				//set height/widths
				ptBox.Height = 21;
				m_combos[i].Height = m_updowns[i].Height = ptBox.Height;
				m_updowns[i].Width = 20;

				if (m_combos[i].ContextMenuStrip != m_Popup)
					m_combos[i].ContextMenuStrip = m_Popup;

				m_combos[i].Top = top;
				// make a checkbox for internal segments
				if (i < m_edits.Length - 1)
				{
					if (m_girths.Count > i)
					{
						m_girths[i].Top = m_combos[i].Bottom - (int)((double)m_girths[i].Height / 2.0);
						m_girths[i].Left = 0;
					}
				}

				if (CHK == 0 && m_girths.Count > 0)//initialize checkbox offset
					CHK = m_girths[i].Right;

				m_combos[i].Left = CHK;
				//m_combos[i].Top = top;
				//m_combos[i].Left = CHK;
				ptBox.Location = new Point(m_combos[i].Right, top);
				//ptBox.Top = top;
				//ptBox.Left = m_combos[i].Right;
				ptBox.Width = m_panel.Width - ptBox.Left - m_updowns[i].Width - 17;//offset for vscroll bar so hscroll doesnt show

				m_updowns[i].Location = new Point(ptBox.Right, top);

				top = ptBox.Bottom + 1;
			}
			m_panel.ResumeLayout(false);
			//m_panel.Height = top;
			//this.Height = m_panel.Bottom;
			//Invalidate();//redraw the panel
		}

		private void m_w4lBtn_Click(object sender, EventArgs e)
		{
			Warps.Curves.CurveW4L importer = new Curves.CurveW4L();

			if (importer.ShowDialog(this) == DialogResult.OK)
			{
				//import here
				MouldCurve importCurve = importer.ParseScript();
				if (importCurve == null)
					MessageBox.Show("invalid fitpoints");
				else
				{
					Label = importCurve.Label;
					ReadCurve(importCurve);
					Refresh();
				}
			}
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
		//			mdp = new Point((m_girths[i].Left + m_girths[i].Right) / 2, inp.Y);
		//			lop = new Point(mdp.X, m_girths[i].Top);
		//			e.Graphics.DrawLine(Pens.Black, mdp, lop);
		//		}
		//		else
		//			mdp = new Point((m_girths[i - 1].Left + m_girths[i - 1].Right) / 2, inp.Y);

		//		if (i >= 1)
		//		{
		//			lop = new Point(mdp.X, m_girths[i - 1].Bottom);
		//			e.Graphics.DrawLine(Pens.Black, mdp, lop);
		//		}
		//		e.Graphics.DrawLine(Pens.Black, inp, mdp);
		//	}
		//}
	}
}
