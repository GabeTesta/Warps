using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;

namespace Warps
{	
	public interface IGroup: IRebuild
	{ 
		//string Label { get; set; }
		Sail Sail { get; set; }
		//TreeNode WriteNode();
		//Control Editor { get; set; }
		//Entity[] CreateEntities();
		//devDept.Eyeshot.Labels.Label[] EntityLabel { get; }
	}
}
