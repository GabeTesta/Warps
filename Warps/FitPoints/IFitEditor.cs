using System;
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

		//event KeyEventHandler ReturnPress;
		Type FitType { get; }
		List<object> AutoFillData { set; }

		string W4LText { get; }
	}
}
