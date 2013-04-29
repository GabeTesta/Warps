using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Warps
{
	public class RigLine: List<Vect3>
	{
		public RigLine(BinaryReader bin)
		{
			ReadBin(bin);
		}
		//public RigLine(int nVerts)
		//{
		//	Capacity = nVerts;
		//}
		//public RigLine(string label, string layer)
		//{
		//	Label = label;
		//	Layer = layer;
		//}
		public string Label, Layer;
		public void ReadBin(BinaryReader bin)
		{
			Label = Utilities.ReadCString(bin);
			Layer = Utilities.ReadCString(bin);
			int nPnt = bin.ReadInt32();
			Vect3 v;
			Capacity = nPnt;
			for (int nP = 0; nP < nPnt; nP++)
			{
				v = new Vect3();
				for (int ix = 0; ix < 3; ix++)
					v[ix] = bin.ReadDouble();
				Add(v);
			}
		}

		internal devDept.Eyeshot.Entities.Entity CreateEntities()
		{
			devDept.Eyeshot.Entities.LinearPath path  = new devDept.Eyeshot.Entities.LinearPath(Count);
			for (int i = 0; i < Count; i++)
				path.Vertices[i] = new devDept.Geometry.Point3D(this[i][0], this[i][1], this[i][2]);
			return path;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} [{2}]", Label, Layer, Count);
		}
	}
}
