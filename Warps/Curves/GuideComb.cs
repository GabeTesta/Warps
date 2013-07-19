using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
namespace Warps
{
	/// <summary>
	/// A MouldCurve with an additional Comb spline that allows for independant comb control
	/// Used to control yarn spacing
	/// </summary>
	public class GuideComb : MouldCurve
	{
		public GuideComb() { Label = "none"; uSplines = new Vect2[] { new Vect2(0, 0) }; }

		/// <summary>
		/// creates a new guidecomb optionally fit to the specified points and combheights
		/// </summary>
		/// <param name="label">the name of the curve</param>
		/// <param name="sail">the sail the curve is on</param>
		/// <param name="fits">optional array of fitpoints, geodesic if 2, otherwise spline</param>
		/// <param name="combs">optonal array of comb heights, minimum 5</param>
		public GuideComb(string label, Sail sail, IFitPoint[] fits, Vect2[] combs)
			:base(label, sail, fits)
		{
			if (fits != null)
				Fit(fits);
			if (combs != null)
				FitComb(combs);
		}

		/// <summary>
		/// fits the 1-D comb spline to a set of (s-pos, height) pairs
		/// </summary>
		/// <param name="combs">the array of points to fit to, minimum 5</param>
		//public void FitComb(Vect2[] combs)
		//{
		//	double[] s = new double[combs.Length];
		//	double[][] h = new double[][]{ new double[combs.Length] };
		//	for (int i = 0; i < combs.Length; i++)
		//	{
		//		s[i] = combs[i][0];
		//		h[0][i] = combs[i][1];
		//	}
		//	m_sComb = s;
		//	Comb.Fit(s, h);
		//}
		public void FitComb(Vect2[] combs)
		{
			if (combs != null)
				CombPnts = combs;
			else
				combs = CombPnts;

			List<double[]> x = new List<double[]>();
			List<double> s = new List<double>();

			for (int i = 0; i < combs.Length; i++)
			{
				s.Add(combs[i][0] );
				x.Add(new double[] { combs[i][1] });
			}
			m_sComb = s.ToArray();
			Comb.Fit(s, x);
		}

		public double[] SComb
		{
			get { return m_sComb; }
		}

		List<Vect2> m_combPnts = new List<Vect2>();

		public Vect2[] CombPnts
		{
			get { return m_combPnts.ToArray(); }
			set { m_combPnts = value.ToList(); }
		}

		double[] m_sComb = null;
		RBF.RBFSpline m_comb = new RBF.RBFSpline();	
		//BSpline m_comb = new BSpline(1); //1D comb spline for setting density distribution
		public static int COMBMAX = 4;

		internal RBF.RBFSpline Comb
		{
			get { return m_comb; }
			set { m_comb = value; }
		}

		public void hVal(double s, ref Vect2 uv, ref double h)
		{
			uVal(s, ref uv);
			double[] p = new double[1];
			Comb.BsVal(s, ref p);
			h = p[0];
		}

		public void xVal(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 xyzComb)
		{
			Vect3 dx = new Vect3(), xnor = new Vect3();
			//get the xyz and normal vector
			double h =0;
			//get the comb height and uv pos
			hVal(s, ref uv, ref h);
			//get the surface normal and xyz
			Surface.xNor(uv, ref xyz, ref xnor);

			//scale the normal to the comb height
			xnor.Magnitude = h;
			//sum for the comb point
			xyzComb = xyz + xnor;
		}

		public override List<Entity> CreateEntities(bool bFitPoints, double TolAngle, out double[] sPos)
		{
			List<Entity> e = base.CreateEntities(bFitPoints, TolAngle, out sPos);
			if (sPos == null)
				return e;
			
			e.AddRange(CreateCombEntity(sPos, false));
			Vect2 u = new Vect2();
			Vect3 xyz = new Vect3(), xup = new Vect3();
			foreach (double s in SComb)
			{
				e.Add(CreateNormal(s, ref u, ref xyz, ref xup));
			}

			return e;
		}
		LinearPath CreateNormal(double s, ref Vect2 uv, ref Vect3 xyz, ref Vect3 xup)
		{
			LinearPath nor = new LinearPath(2);
			xVal(s, ref uv, ref xyz, ref xup);
			nor.Vertices[0] = Utilities.Vect3ToPoint3D(xyz);
			Vect3 xnor = xup - xyz;
			if (xnor.Magnitude > COMBMAX)//enforce maxmimum combheight
			{
				xnor.Magnitude = COMBMAX;
				xup = xyz + xnor;//ensure you passt he values back to the caller
			}
			nor.Vertices[1] = Utilities.Vect3ToPoint3D(xup);
			nor.EntityData = this;
			///nor.GroupIndex = 0;
			return nor;
		}
		public List<Entity> CreateCombEntity(double[] sPos, bool bNorms)
		{
			//create normal offset comb lines
			List<Entity> normals = new List<Entity>();
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Vect3 xup = new Vect3();
			List<Point3D> pts = new List<Point3D>(sPos.Length*2);
			List<Point3D> pup = new List<Point3D>(sPos.Length);
			for (int i = 0; i < sPos.Length; i++)
			{
				LinearPath nor = CreateNormal(sPos[i], ref uv, ref xyz, ref xup);
				//xVal(sPos[i], ref uv, ref xyz, ref xup);
				//pts.Add(Utilities.Vect3ToPoint3D(xyz));
				//Vect3 xnor = xup - xyz;
				//if (xnor.Magnitude > COMBMAX)//enforce maxmimum combheight
				//{
				//	xnor.Magnitude = COMBMAX;
				//	xup = xyz + xnor;
				//}
				pts.Add(Utilities.Vect3ToPoint3D(xyz));
				pup.Add(Utilities.Vect3ToPoint3D(xup));
				if (bNorms)
					normals.Add(nor);
			}
			pts.AddRange(pup);

			Mesh m = SurfaceTools.GetMesh(pts.ToArray(), 2);
			//m.GroupIndex = 0;
			m.EntityData = this;
			normals.Add(m);
			return normals;
		}

		public override Point3D GetLabelPoint3D(double s)
		{
			Vect2 u = new Vect2();
			Vect3 x = new Vect3();
			Vect3 c = new Vect3();

			xVal(s, ref u, ref x, ref c);
			c -= x; //get normal vector
			c.Magnitude /= 2;//half height
			return Utilities.Vect3ToPoint3D(x + c);
		}

		public override bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt == null || txt.Count == 0)
				return false;

			List<IFitPoint> fits = new List<IFitPoint>();
			string[] splits = txt[0].Split(':');
			Label = "";
			if (splits.Length > 0)//extract label
				Label = splits[1];
			if (splits.Length > 1)//incase label contains ":"
				for (int i = 2; i < splits.Length; i++)
					Label += ":" + splits[i];
			Label = Label.Trim();

			for (int nLine = 1; nLine < txt.Count; )
			{
				IList<string> lines = ScriptTools.Block(ref nLine, txt);
				//nLine += lines.Count;

				object cur = null;
				splits = lines[0].Split(':');
				if (splits.Length > 0)
					cur = Utilities.CreateInstance(splits[0].Trim('\t'));
				if (cur != null && cur is IFitPoint)
				{
					(cur as IFitPoint).ReadScript(Sail, lines);
					fits.Add(cur as IFitPoint);
				}
				else if (cur != null && cur is Vect2)
				{
					m_combPnts.Add(ParseVect2Lines(lines));
				}
			}
			FitPoints = fits.ToArray();

			FitComb(m_combPnts.ToArray());

			if (AllFitPointsValid())
				ReFit();

			return true;
		}

		private Vect2 ParseVect2Lines(IList<string> lines)
		{
			Vect2 ret = new Vect2();
			
			string[] split = lines.Last().Trim().Split(new char[] { ':' });

			ret.FromString(split.Last());

			return ret;
		}

		public override List<string> WriteScript()
		{

			List<string> script = base.WriteScript();

			//script.Add(GetType().Name + ": " + Label);
			//foreach (IFitPoint fp in FitPoints)
			//{
			//     foreach (string s in fp.WriteScript())
			//          script.Add("\t" + s);
			//}
			foreach (Vect2 v in CombPnts)
			{
				script.Add("\t" + v.GetType().Name);
				script.Add(string.Format("\t\tCombVal: {0},{1}", v[0], v[1]));
			}
			

			return script;
		}
	}
}
