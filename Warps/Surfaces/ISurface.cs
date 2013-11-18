using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warps
{
	enum SurfaceType
	{
		COF = 0,
		RBF,
		COMBO,
		OBJ
	};

	public interface ISurface
	{
		string Label { get; }

		List<IGroup> Groups { get; }

		void xVal(Vect2 uv, ref Vect3 xyz);
		void xVec(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv);
		void xCvt(Vect2 uv, ref Vect3 xyz, ref Vect3 dxu, ref Vect3 dxv, ref Vect3 ddxu, ref Vect3 ddxv, ref Vect3 dduv);
		void xNor(Vect2 uv, ref Vect3 xyz, ref Vect3 xnor);
		void xRad(Vect2 uv, ref Vect3 xyz, ref double k);

		bool xClosest(ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol);

		//List<string> WriteScript();
		//bool ReadScript(Sail sail, IList<string> txt);

		//XmlNode WriteXScript(XmlDocument doc);
		//void ReadXScript(Sail sail, XmlNode node);

		List<devDept.Eyeshot.Entities.Entity> CreateEntities(double[,] uvLims, bool bGauss);
		double[] ColorValues { get; set; }

		System.Windows.Forms.TreeNode WriteNode();
	}

}
