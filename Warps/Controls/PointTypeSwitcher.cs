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
	public partial class PointTypeSwitcher : UserControl
	{
		static List<MouldCurve> m_gCurves = new List<MouldCurve>();
		static List<object> m_autofillobjs = new List<object>();
		static Sail m_sail = null;

		internal static void SetCurves(List<MouldCurve> mcs)
		{
			m_gCurves.Clear();
			m_gCurves.AddRange(mcs);
		}
		internal static void SetAutofill(List<object> mcs)
		{
			m_autofillobjs.Clear();
			m_autofillobjs.AddRange(mcs);
		}
		internal static void SetSail(Sail s)
		{
			m_sail = s;
		}
		internal static IEnumerable<MouldCurve> GetCurves()
		{
			return m_gCurves;
		}

		public PointTypeSwitcher()
		{
			InitializeComponent();
		}

		public bool Selected
		{
			get { return m_selectedCheckBox.Checked; }
		}

		private void backBut_Click(object sender, EventArgs e)
		{
			SwitchEditors(-1);
		}

		private void forwardBut_Click(object sender, EventArgs e)
		{
			SwitchEditors(+1);
		}

		private void SwitchEditors(int increment)
		{
			IFitPoint pnt = null;
			List<Type> types = Utilities.GetAllOf(typeof(IFitPoint), false);
			int m_value = types.IndexOf(Edit.FitType);
			m_value += increment;
			if (m_value < 0)
				m_value += types.Count;
			m_value %= (types.Count);

			pnt = (IFitPoint)Utilities.CreateInstance(types[m_value]);// create a new instance of the given IFitPoint

			pnt.WriteEditor(this);
		}

		public Color SetColor
		{
			get { return BackColor; }
			set { BackColor = value; panel1.Controls[0].BackColor = value; }
		}

		public void ToggleImage(bool show)
		{
			//if (show)
			//{
			//	pointTypeImageBox.Visible = true;
			//	pointTypeImageBox.BringToFront();
			//}
			//else
			//{
			//	//pointTypeImageBox.Image = Warps.Properties.Resources.empty;
			//	pointTypeImageBox.Visible = false;
			//	pointTypeImageBox.BackColor = Color.Transparent;
			//	pointTypeImageBox.SendToBack();
			//}
		}

		public IFitEditor Edit
		{
			set
			{

				panel1.Controls.Clear();
				panel1.Size = new System.Drawing.Size(305, 25);
				//panel1.Location = new Point(28, 0);
				panel1.Controls.Add(value as Control);
				//panel1.Controls[0].Dock = DockStyle.Fill;
				value.ReturnPress -= c_ReturnPress;
				value.ReturnPress += c_ReturnPress;
				value.sail = m_sail;
				if (m_autofillobjs != null && m_autofillobjs.Count > 0)
					value.AutoFillData = m_autofillobjs.ToArray();

			}
			get
			{
				return panel1.Controls.Count > 0 ? panel1.Controls[0] as IFitEditor : null;
			}
		}

		#region IFitEditor Members

		public event EventHandler<KeyEventArgs> ReturnPress;

		void c_ReturnPress(object sender, KeyEventArgs e)
		{
			if (ReturnPress != null)
				ReturnPress(sender, e);
		}

		public Type FitType
		{
			get { return Edit == null ? null : Edit.FitType; }
		}

		#endregion

		internal void SetEdit(IFitEditor type)
		{
			Edit = type;
		}

		private void PointTypeSwitcher_MouseEnter(object sender, EventArgs e)
		{
			dragImage.Visible = true;
			dragImage.BringToFront();
		}

		private void PointTypeSwitcher_MouseLeave(object sender, EventArgs e)
		{
			dragImage.SendToBack();
			dragImage.Visible = false;
		}

		private void panel1_Enter(object sender, EventArgs e)
		{
			dragImage.Visible = true;
			dragImage.BringToFront();
		}

		private void dragImage_MouseHover(object sender, EventArgs e)
		{
			dragImage.Visible = true;
			dragImage.BringToFront();
		}

		private void PointTypeSwitcher_Enter(object sender, EventArgs e)
		{
		}

	}
}
