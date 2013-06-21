using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps;
using Warps.Controls;

namespace Warps.Curves
{
	public partial class CurveW4L : UserControl
	{
		public static DialogResult ShowDialog(MouldCurveEditor edit)
		{
			Form f = new Form();
			f.SuspendLayout();
			f.StartPosition = FormStartPosition.Manual;
			f.Size = new Size(520, 150);
			f.Location = edit.PointToScreen(edit.Location);
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			f.Owner = edit.ParentForm;

			CurveW4L importer = new CurveW4L(edit);
			importer.Dock = DockStyle.Fill;
			f.Controls.Add(importer);
			f.AcceptButton = importer.m_import;
			f.CancelButton = importer.m_copy;
			f.ResumeLayout(true);
			return f.ShowDialog();
		}
		public CurveW4L(MouldCurveEditor edit)
		{
			InitializeComponent();
			m_import.Visible = false;//no import for now
			m_edit = edit;
			SetText();
		}

		private void SetText()
		{
			if (m_edit == null)
				return;

			StringBuilder script = new StringBuilder();
			if (m_edit.m_girths[0].Checked && m_edit.m_edits.Length <= 3)
				script.Append("GIRTH");
			else
				script.Append("CURVE");

			string lbl = m_edit.Label.Length > 5 ? m_edit.Label.Substring(0, 5) : m_edit.Label;
			script.AppendFormat(" [{0,5}] starting ", lbl);

			for (int nFit = 0; nFit < m_edit.m_edits.Length; nFit++)
			{
				script.Append(m_edit.m_edits[nFit].W4LText);

				if (nFit < m_edit.m_edits.Length - 2)//internal points
				{
					if (nFit % 2 == 1)//line feed every other point
					{
						script.AppendLine();
						script.Append("             ");
					}
					script.Append("  through ");
				}
				else if (nFit == m_edit.m_edits.Length - 2)//final point
					script.Append(" stopping ");

			}
			textBox1.Text = script.ToString();
		}
		MouldCurveEditor m_edit;
		private void textBox1_Enter(object sender, EventArgs e)
		{
			if (Clipboard.ContainsText())
			{
				string txt = Clipboard.GetText(TextDataFormat.Text);
				if ((txt.StartsWith("GIRTH") || txt.StartsWith("CURVE")) && txt.Length > 13)
				{
					if (MessageBox.Show(string.Format("Copy {0} from clipboard?", txt.Substring(0, 13)), "Import Curve", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						textBox1.Text = txt;
				}
			}
		}

		private void m_import_Click(object sender, EventArgs e)
		{

		}

		private void m_copy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(textBox1.Text, TextDataFormat.Text);
		}

		#region CurveMaker

		//private void CreateCurve(string script)
		//{
		//	List<IFitPoint> fits = new List<IFitPoint>();
		//	int nS = script.IndexOf("tarting");
		//	nS = script.IndexOf(' ', nS);
		//	fits.Add(ReadFitPoint(script, ++nS));
		//	while ((nS = script.IndexOf("hrough", nS)) > 0)
		//	{
		//		nS += 6;
		//		while (script[++nS] == ' ') ;//skip spaces
		//		fits.Add(ReadFitPoint(script, nS));
		//	}
		//	nS = script.IndexOf("topping");
		//	nS = script.IndexOf(' ', nS);
		//	fits.Add(ReadFitPoint(script, ++nS));
		//	if (Curve == null)
		//		Curve = new MouldCurve(script.Substring(7, 5), m_sail, fits.ToArray());
		//	else
		//	{
		//		Curve.Label = script.Substring(7, 5);
		//		Curve.Fit(fits.ToArray());
		//	}
		//}

		//private void CreateGirth(string script)
		//{
		//	IFitPoint[] fits = new IFitPoint[2];
		//	int nS = script.IndexOf("tarting");
		//	nS = script.IndexOf(' ', nS);
		//	fits[0] = ReadFitPoint(script, ++nS);
		//	nS = script.IndexOf("topping");
		//	nS = script.IndexOf(' ', nS);
		//	fits[1] = ReadFitPoint(script, ++nS);
		//	if (Curve == null)
		//		Curve = new MouldCurve(script.Substring(7, 5), m_sail, fits);
		//	else
		//	{
		//		Curve.Label = script.Substring(7, 5);
		//		Curve.Fit(fits.ToArray());
		//	}
		//}

		//IFitPoint ReadFitPoint(string script, int nType)
		//{
		//	int nE, nS = nType + 5;
		//	double u, v;
		//	string curve;
		//	MouldCurve cur;
		//	string type = script.Substring(nType, 5);
		//	switch (type)
		//	{
		//		case "POINT":
		//			nS = script.IndexOf('[', nS);
		//			nE = script.IndexOf(';', ++nS);
		//			curve = script.Substring(nS, nE - nS);
		//			double.TryParse(curve, out u);
		//			nS = script.IndexOf(']', ++nE);
		//			curve = script.Substring(nE, nS - nE);
		//			double.TryParse(curve, out v);
		//			return new FixedPoint(u, v);
		//		case "CURVE":
		//			nS = script.IndexOf('[', nS);
		//			nE = script.IndexOf(';', ++nS);
		//			curve = script.Substring(nS, nE - nS).Trim();
		//			cur = m_sail.FindCurve(curve);
		//			nS = script.IndexOf(']', ++nE);
		//			curve = script.Substring(nE, nS - nE);
		//			double.TryParse(curve, out v);
		//			return new CurvePoint(cur, v);
		//		case "SLIDE":
		//			nS = script.IndexOf('[', nS);
		//			nE = script.IndexOf(';', ++nS);
		//			curve = script.Substring(nS, nE - nS);
		//			cur = m_sail.FindCurve(curve);
		//			nS = script.IndexOf(']', ++nE);
		//			double.TryParse(script.Substring(nE, nS - nE), out v);
		//			return new SlidePoint(cur, v);
		//	}
		//	return null;
		//}

		#endregion
	}
}
