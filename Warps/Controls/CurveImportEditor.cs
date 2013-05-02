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
	
	public partial class CurveImportEditor : UserControl
	{
		public CurveImportEditor(CurveMaker tracker)
		{
			InitializeComponent();
			m_import.Click += tracker.OnImport;
		}
		public string Script
		{
			get { return m_scriptBox.Text; }
			set { m_scriptBox.Text = value; }
		}
		public MouldCurve Curve
		{
			set
			{
				if (value != null)
				{
					m_curveEdit.Label = value.Label;
					m_curveEdit.Length = value.Length;
					m_curveEdit.ReadCurve(value);
					//m_curveEdit.FitPoints = value.FitPoints;
				}
			}
		}
	}
}
