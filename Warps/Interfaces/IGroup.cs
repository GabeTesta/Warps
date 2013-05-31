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
		Sail Sail { get; set; }

		IRebuild FindItem(string label);
		IRebuild FindItem(IRebuild obj);

		int Count { get; }

		bool Watermark(IRebuild tag, ref List<IRebuild> rets);
	}
}
