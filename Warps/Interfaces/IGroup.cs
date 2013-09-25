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
		bool ContainsItem(IRebuild obj);

		int Count { get; }

		bool Watermark(IRebuild tag, ref List<IRebuild> rets);

		void WriteBin(System.IO.BinaryWriter bin);

		/// <summary>
		/// Checks if a specified IRebuild can be added to this group type
		/// </summary>
		/// <param name="item">The type of IRebuild to check</param>
		/// <returns>True if acceptable, false if invalid type</returns>
		bool CanInsert(IRebuild item);
		/// <summary>
		/// Inserts the specified IRebuild into the group at the desired index. If the item already exists in this group, reorders it to the new index position
		/// </summary>
		/// <param name="item">The IRebuild Item to insert, should be checked with CanInsert first</param>
		/// <param name="target">The item to insert above, null for appending to the end</param>
		//// <param name="target">The Index to insert at, -1 to append to the end</param>
		void Insert(IRebuild item, IRebuild target);
		/// <summary>
		/// Removes the specified item from this group
		/// </summary>
		/// <param name="item">the item to remove</param>
		bool Remove(IRebuild item);

		void FlatLayout(List<IRebuild> flat);
	}
}
