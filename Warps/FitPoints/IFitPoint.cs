﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Warps.Curves;

namespace Warps
{
	public interface IFitPoint
	{
		//bool ReadScript(Sail sail, IList<string> txt);
		//List<string> WriteScript();

		XmlNode WriteXScript(XmlDocument doc);
		void ReadXScript(Sail s, XmlNode node);

		double S { get; set; }
		Vect2 UV { get;  }

		IFitPoint Clone();

		/// <summary>
		/// Returns the value of this point
		/// </summary>
		/// <param name="i">0 for S, 1 for U, 2 for V</param>
		/// <returns>The requested parameter's value</returns>
		double this[int i] { get; set; }

		/// <summary>
		/// Gets the FitPoint's TreeNode representation
		/// </summary>
		TreeNode Node { get;  }
		
		Control WriteEditor(ref IFitEditor edit);
		void ReadEditor(IFitEditor edit);

		void GetParents(Sail s, List<IRebuild> parents);
		bool Affected(List<IRebuild> connected);
		bool Update(MouldCurve cur);
		bool ValidFitPoint { get; }
	}
}