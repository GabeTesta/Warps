using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;
using System.Xml;

namespace Warps
{
	public interface IRebuild
	{
		string Label { get; set; }
		string Layer { get; }

		//[Obsolete]
		//List<string> WriteScript();
		//[Obsolete]
		//bool ReadScript(Sail sail, IList<string> txt);

		XmlNode WriteXScript(XmlDocument doc);
		void ReadXScript(Sail sail, XmlNode node);

		/// <summary>
		/// if an IRebuild is locked, it cannot be editted/deleted/added to etc.
		/// it is always in it's same state and can't be modified
		/// </summary>
		bool Locked { get; set; }

		TreeNode WriteNode();
		List<Entity> CreateEntities();
		List<devDept.Eyeshot.Labels.Label> EntityLabel { get; }

		/// <summary>
		/// checks the updated list and adds itself if any of it's parents are in the list
		/// </summary>
		/// <param name="updated">the list of all connected nodes</param>
		/// <returns>List of connected objects</returns>
		void GetChildren(List<IRebuild> updated);
		void GetParents(Sail s, List<IRebuild> parents);
		bool Affected(List<IRebuild> connected);
		bool Update(Sail s);
		bool Delete();
	}
}
