﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps.Controls;

namespace Warps
{
	class EquationBox : AutoCompleteTextBox
	{
		//static public List<string> m_eqs = new List<string>();
		public EquationBox()
		{
			this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Size = new System.Drawing.Size(80, 20);
			this.TextAlign = HorizontalAlignment.Right;
			this.AcceptsTab = false;
			this.MinimumSize = new System.Drawing.Size(0, 0);
		}

		public Sail sail = null;

		public EquationBox(object[] autofills)
		{
			this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Size = new System.Drawing.Size(80, 20);
			this.TextAlign = HorizontalAlignment.Right;
			this.Values = autofills;
		}
		
		public double Value
		{
			get
			{
				double u;
				if (Text != null && sail != null)
				{
					double result = 0;

					if (EquationEvaluator.Evaluate(new Equation(null, Text), sail, out result))
						return result;
				}
				else if (double.TryParse(Text, out u))
					return u;
				return 0;
			}
			set
			{
				Text = value.ToString("f4");
			}
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.Up)
			//{
			//	Value += 0.0001;
			//	e = new KeyEventArgs(Keys.Enter);
			//}
			//else if (e.KeyCode == Keys.Down)
			//{
			//	Value -= 0.0001;
			//	e = new KeyEventArgs(Keys.Enter);
			//}
			//else if (e.KeyCode != Keys.Enter)
			base.OnKeyDown(e);
			if (ReturnPress != null && e.KeyCode == Keys.Enter)
				ReturnPress(this, e);
		}
		public event KeyEventHandler ReturnPress;


		public bool IsNumber()
		{
			double outtie;
			return double.TryParse(Text, out outtie);
		}
	}
}
