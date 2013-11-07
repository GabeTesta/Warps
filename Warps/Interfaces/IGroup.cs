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

		/// <summary>
		/// finds an item by label
		/// </summary>
		/// <param name="label">the name of the item to find</param>
		/// <returns>the item or null</returns>
		IRebuild FindItem(string label);
		/// <summary>
		/// returns true if the item is a child(directly or nested) of this group
		/// </summary>
		/// <param name="obj">the item to search for</param>
		/// <returns>true if found</returns>
		bool ContainsItem(IRebuild obj);

		/// <summary>
		/// attempts to find the items parent group
		/// </summary>
		/// <typeparam name="T">The type of IGroup to search for</typeparam>
		/// <param name="item">the item to find the parent of</param>
		/// <param name="parent">the found parent group, null if none</param>
		/// <returns>true if succesful, false if no parent found</returns>
		bool FindParent<T>(IRebuild item, out T parent) where T : class, IGroup;

		/// <summary>
		/// the number of direct children, does not count nested children
		/// </summary>
		int Count { get; }

		/// <summary>
		/// appends this group's flattened layout to the list
		/// </summary>
		/// <param name="flat">the list to append to</param>
		void FlatLayout(List<IRebuild> flat);

		/// <summary>
		/// searches for the tag and adds all items up to the tagged one
		/// </summary>
		/// <param name="tag">the item to watermark too</param>
		/// <param name="rets">the collection of items above the watermark</param>
		/// <returns>true if tag found ,false otherwise</returns>
		bool Watermark(IRebuild tag, ref List<IRebuild> rets);

		/// <summary>
		/// Checks if a specified IRebuild can be added to this group type
		/// </summary>
		/// <param name="item">The type of IRebuild to check</param>
		/// <returns>True if acceptable, false if invalid type</returns>
		bool CanInsert(Type item);
		/// <summary>
		/// Inserts the specified IRebuild into the group at the desired index. If the item already exists in this group, reorders it to the new index position
		/// </summary>
		/// <param name="item">The IRebuild Item to insert, should be checked with CanInsert first</param>
		/// <param name="target">The item to insert above, null for appending to the end</param>
		void Insert(IRebuild item, IRebuild target);
		/// <summary>
		/// Removes the specified item from this group
		/// </summary>
		/// <param name="item">the item to remove</param>
		bool Remove(IRebuild item);

		/// <summary>
		/// writes this group to the membrain.bin file 
		/// </summary>
		/// <param name="bin">the file to write to</param>
		void WriteBin(System.IO.BinaryWriter bin);
	}
}
