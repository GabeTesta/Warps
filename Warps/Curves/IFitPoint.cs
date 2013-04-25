using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Warps
{
	public interface IFitPoint
	{
		bool ReadScript(Sail sail, IList<string> txt);
		List<string> WriteScript();

		double S { get; set; }
		Vect2 UV { get; set; }

		Equation S_Equ { get; set; }
		Equation U { get; set; }
		Equation V { get; set; }

		IFitPoint Clone();
		/// <summary>
		/// Returns the value of this point
		/// </summary>
		/// <param name="i">0 for S, 1 for U, 2 for V</param>
		/// <returns>The requested parameter's value</returns>
		double this[int i] { get; set; }

		//IList<string> Script { get; set; }

		/// <summary>
		/// Gets the FitPoint's TreeNode representation or sets the FitPoint's values from a TreeNode
		/// </summary>
		TreeNode Node { get; set; }

		bool Affected(List<IRebuild> connected);

		bool Delete();

		bool Update(Sail s);

		PointTypeSwitcher WriteEditor(PointTypeSwitcher edit);
		void ReadEditor(PointTypeSwitcher edit);
		bool ValidFitPoint { get; set; }
	}
}
