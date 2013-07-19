﻿using System;
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
	public partial class CrossPointEditor : UserControl, IFitEditor
	{
		public CrossPointEditor()
		{
			InitializeComponent();
		}
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			int wid = Width / 2 - (2 * Padding.Horizontal);
			m_curve1.Top = m_curve2.Top = 0;
			m_curve1.Left = 0;
			m_curve1.Width = wid;
			m_curve2.Left = wid + Padding.Horizontal;
			m_curve2.Width = wid;
			this.Height = m_curve1.Height;
		}

		internal MouldCurve Curve1
		{
			get{ return m_curve1.SelectedItem as MouldCurve; }
			set
			{
				if (value != null && !m_curve1.Items.Contains(value))
					m_curve1.Items.Add(value);
				m_curve1.SelectedItem = value;
			}
		}
		internal MouldCurve Curve2
		{
			get{ return m_curve2.SelectedItem as MouldCurve; }
			set
			{
				if (value != null && !m_curve2.Items.Contains(value))
					m_curve2.Items.Add(value);
				m_curve2.SelectedItem = value;
			}
		}


		#region IFitEditor Members

		public IFitPoint CreatePoint()
		{
			return new CrossPoint(Curve1, Curve2);
		}

		public Type FitType
		{
			get { return typeof(CrossPoint); }
		}

		public List<object> AutoFillData
		{
			set
			{
				if (value == null)
					return;

				MouldCurve c1 = Curve1;//backup current curve
				MouldCurve c2 = Curve2;//backup current curve
				m_curve1.Items.Clear();
				m_curve2.Items.Clear();
				foreach (object o in value)
				{
					if (o is MouldCurve)
					{
						if (!m_curve1.Items.Contains(o))
							m_curve1.Items.Add(o);
						if (!m_curve2.Items.Contains(o))
							m_curve2.Items.Add(o);
					}
				}
				Curve1 = c1;//select currecnt curve
				Curve2 = c2;//select currecnt curve
			}
		}

		public string W4LText
		{
			get
			{
				string type = FitType.Name.ToString();
				type = type.ToUpper().Substring(0, 5);
				string lbl1 = Curve1.Label.Length > 5 ? Curve1.Label.Substring(0, 5) : Curve1.Label;
				string lbl2 = Curve2.Label.Length > 5 ? Curve2.Label.Substring(0, 5) : Curve2.Label;

				return String.Format("CROSS [{0,5};{1,5}]", lbl1, lbl2);
			}
		}

		#endregion

		private void m_curve2_Validating(object sender, CancelEventArgs e)
		{
			ComboBox curve = sender as ComboBox;
			if (curve == null) return;

			if (curve.SelectedItem != null)
				return;//valid selection already

			//search curve list for specified curve
			foreach (Object o in curve.Items)
				if (o.ToString() == curve.Text)
				{
					curve.SelectedItem = o;
					return;
				}

			//prompt user on fail
			MessageBox.Show("Please select a valid curve");
			curve.Focus();
		}
	}
}
