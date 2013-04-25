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
		//bool Rebuild(List<IRebuild> parents);
		
		bool ReadScript(Sail sail, IList<string> txt);
		List<string> WriteScript();
		/// <summary>
		/// get a list of GetConnect objects
		/// </summary>
		/// <param name="updated">object to search from</param>
		/// <returns>List of connected objects</returns>
		void GetConnected(List<IRebuild> updated);

		bool Affected(List<IRebuild> connected);

		bool Delete();

		bool Update(Sail s);

		string Label { get; set; } 

		TreeNode WriteNode();

		Entity[] CreateEntities();
		devDept.Eyeshot.Labels.Label[] EntityLabel { get; }
	}
}
