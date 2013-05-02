﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Warps
{
	public interface IFitEditor
	{
		IFitPoint CreatePoint();

		event EventHandler<KeyEventArgs> ReturnPress;
		Type FitType { get; }
		object[] AutoFillData { get; set; }
		Sail sail { get; set; }
	}
}
