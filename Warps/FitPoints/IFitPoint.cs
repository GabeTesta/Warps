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

		IFitPoint Clone();

		/// <summary>
		/// Returns the value of this point
		/// </summary>
		/// <param name="i">0 for S, 1 for U, 2 for V</param>
		/// <returns>The requested parameter's value</returns>
		double this[int i] { get; set; }

		/// <summary>
		/// Gets the FitPoint's TreeNode representation or sets the FitPoint's values from a TreeNode
		/// </summary>
		TreeNode Node { get;  }
		
		Control WriteEditor(ref IFitEditor edit);
		void ReadEditor(IFitEditor edit);

		void GetParents(Sail s, List<IRebuild> parents);
		bool Affected(List<IRebuild> connected);
		bool Update(Sail s);
		bool ValidFitPoint { get; }

	}
}
