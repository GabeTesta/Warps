using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;

namespace Warps
{
	public interface IRebuild
	{
		string Label { get; set; } 
		
		List<string> WriteScript();
		bool ReadScript(Sail sail, IList<string> txt);

		bool Draggable { get; set; }

		TreeNode WriteNode();
		Entity[] CreateEntities();
		devDept.Eyeshot.Labels.Label[] EntityLabel { get; }

		/// <summary>
		/// get a list of GetConnect objects
		/// </summary>
		/// <param name="updated">object to search from</param>
		/// <returns>List of connected objects</returns>
		void GetConnected(List<IRebuild> updated);
		void GetParents(Sail s, List<IRebuild> parents);
		bool Affected(List<IRebuild> connected);
		bool Update(Sail s);
		bool Delete();
	}
}
