using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Drawing;

namespace Warps
{
	enum SurfaceType
	{
		COF = 0,
		RBF,
		COMBO
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


		List<string> WriteScript();
		bool ReadScript(Sail sail, IList<string> txt);

		List<devDept.Eyeshot.Entities.Entity> CreateEntities(double[,] uvLims, bool bGauss);

		System.Windows.Forms.TreeNode WriteNode();
	}

	public static class SurfaceTools
	{

		public static int MESHU = 60;
		public static int MESHV = 90;
		public static double[,] MESHLIMITS = new double[,] { { -0.2, 1.2 }, { -0.2, 1.2 } };

		//static public void GetFittingMesh(ISurface s, double vMid, out List<Vect2> uv, out List<Vect3> xyz)
		//{
		//	FixedPoint[] fits = new FixedPoint[2];
		//	fits[0] = new FixedPoint(0, vMid);
		//	fits[1] = new FixedPoint(1, vMid);
		//	SurfaceCurve midcur = new SurfaceCurve("MidCur", s, fits);


		//}

		public static void xRad(ISurface s, Vect2 uv, ref Vect3 xyz, ref double k)
		{
			Vect3 dxu = new Vect3(), dxv = new Vect3(), ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3(), xnor = new Vect3();
			s.xCvt(uv, ref xyz, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);
			s.xNor(uv, ref xyz, ref xnor);

			//calculate first fundamental form
			double E = dxu.Norm;
			double F = dxu.Dot(dxv);
			double G = dxv.Norm;
			//double E = BLAS.dot(dxu, dxu);
			//double F = BLAS.dot(dxu, dxv);
			//double G = BLAS.dot(dxv, dxv);
			double detI = E * G - F * F;
			//calculate second fundamental form
			double e = xnor.Dot(ddxu);
			double f = xnor.Dot(dduv);
			double g = xnor.Dot(ddxv);
			//double e = BLAS.dot(ddxu, xnor);
			//double f = BLAS.dot(dduv, xnor);
			//double g = BLAS.dot(ddxv, xnor);
			double detII = e * g - f * f;

			k = detII / detI;
		}


		public static bool xClosest(ISurface s, ref Vect2 uv, ref Vect3 xyzTarget, ref double dist, double tol)
		{
			Vect3 x = new Vect3(xyzTarget);
			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();

			Vect3 h = new Vect3();
			Vect2 c = new Vect2();
			Vect2 res = new Vect2();
			Vect2 a = new Vect2(), b = new Vect2();
			double det, r;
			Vect2 d = new Vect2();

			int loop = 0, max_loops = 150;
			while (loop++ < max_loops)
			{
				s.xCvt(uv, ref x, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

				h = x - xyzTarget;
				//h = BLAS.subtract(x, xyzTarget);
				dist = h.Magnitude;

				//e[0] = s;
				c[0] = h.Dot(dxu);// BLAS.dot(h, dxu); // error, dot product is 0 at pi/2
				c[1] = h.Dot(dxv);// BLAS.dot(h, dxv); // error, dot product is 0 at pi/2

				if (Math.Abs(c[0]) < tol && Math.Abs(c[1]) < tol) // error is less than the tolerance
				{
					xyzTarget.Set(x);// return point to caller
					return true;
				}

				a[0] = dxu.Norm + h.Dot(ddxu);
				a[1] = b[0] = dxu.Dot(dxv) + h.Dot(dduv);
				b[1] = dxv.Norm + h.Dot(ddxv);

				//a[0] = BLAS.dot(dxu, dxu) + BLAS.dot(h, ddxu);
				//a[1] = BLAS.dot(dxu, dxv) + BLAS.dot(h, dduv);
				//b[0] = a[1];
				//b[1] = BLAS.dot(dxv, dxv) + BLAS.dot(h, ddxv);

				det = a.Cross(b);
				//det = BLAS.cross2d(a, b);

				d[0] = c.Cross(b) / det;
				d[1] = a.Cross(c) / det;
				//d[0] = BLAS.cross2d(c, b) / det;
				//d[1] = BLAS.cross2d(a, c) / det;

				c[0] = 0.01 > Math.Abs(d[0]) ? 1 : 0.01 / Math.Abs(d[0]);
				c[1] = 0.01 > Math.Abs(d[1]) ? 1 : 0.01 / Math.Abs(d[1]);
				//enforce maximum increment
				r = Math.Min(c[0], c[1]);

				//increment uv by scaled residuals
				//uv = BLAS.subtract(uv, BLAS.scale(d, r));
				uv = uv - d * r;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}
			//s = s0;
			return false;
		}


		//public static Mesh GetMesh(ISurface s, int rows, int cols, bool bGauss)
		//{
		//	//Mesh mesh = new Mesh(meshNatureType.RichPlain);
		//	meshNatureType meshtype = bGauss ? meshNatureType.MulticolorPlain : meshNatureType.ColorPlain;
		//	Mesh mesh = new Mesh(meshtype);
		//	mesh.ColorMethod = bGauss ? colorMethodType.byEntity : colorMethodType.byLayer;
		//	mesh.Vertices = bGauss ? SurfaceTools.GetMeshGaussianPoints(s, rows, cols, MESHLIMITS) : SurfaceTools.GetMeshPoints(s, rows, cols);

		//	mesh.RegenMode = regenType.RegenAndCompile;
		//	mesh.Triangles = new IndexTriangle[(rows - 1) * (cols - 1) * 2];
		//	int count = 0;
		//	for (int j = 0; j < (rows - 1); j++)
		//	{
		//		for (int i = 0; i < (cols - 1); i++)
		//		{

		//			mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
		//														   i + j * cols + 1,
		//														   i + (j + 1) * cols + 1);
		//			mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
		//														   i + (j + 1) * cols + 1,
		//														   i + (j + 1) * cols);
		//		}
		//	}

		//	mesh.ComputeEdges();
		//	//mesh.ComputeNormals();
		//	mesh.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;
		//	mesh.EntityData = s;
		//	mesh.Selectable = false;
		//	return mesh;
		//}
		//static public Point3D[] GetMeshPoints(ISurface s, int ROWS, int COLS)
		//{
		//	double[,] uvLim = new double[2, 2];
		//	uvLim[0, 0] = uvLim[1, 0] = 0;
		//	uvLim[0, 1] = uvLim[1, 1] = 1;
		//	return GetExtensionPoints(s, ROWS, COLS, uvLim);


		//	//double[] uv = new double[2], xyz = new double[3];
		//	//Point3D[] d = new Point3D[ROWS * COLS];
		//	//int i = 0;
		//	//for (int iU = 0; iU < ROWS; iU++)
		//	//{
		//	//	uv[0] = (double)iU / (double)(ROWS - 1);
		//	//	for (int iV = 0; iV < COLS; iV++, i++)
		//	//	{
		//	//		uv[1] = (double)iV / (double)(COLS - 1);
		//	//		xVal(uv, ref xyz);
		//	//		d[i].X = xyz[0];
		//	//		d[i].Y = xyz[1];
		//	//		d[i].Z = xyz[2];
		//	//	}
		//	//}
		//	//return d;
		//}
		/// <summary>
		/// Creates a regular grid of 3dpoints from the Surface on the specified uv-interval 
		/// </summary>
		/// <param name="s">the surface to mesh</param>
		/// <param name="ROWS">the number of constant-u sections</param>
		/// <param name="COLS">the number of constant-v sections</param>
		/// <param name="uvLim">(optional)the uv limits to mesh, uvLim[0,x] = uLim, uvLim[1,x] = vLim</param>
		/// <returns>the grid of points for meshing</returns>
		static public Point3D[] GetExtensionPoints(ISurface s, int ROWS, int COLS, double[,] uvLim)
		{
		//	double[,] uvLim = new double[2, 2];
		//	uvLim[0, 0] = uvLim[1, 0] = 0;
		//	uvLim[0, 1] = uvLim[1, 1] = 1;
			if( uvLim == null )
				uvLim = new double[,]{ { 0, 1 }, {0, 1} };
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			Point3D[] d = new Point3D[ROWS * COLS];
			int i = 0;
			for (int iU = 0; iU < ROWS; iU++)
			{
				uv[0] = BLAS.interpolate((double)iU / (double)(ROWS - 1), uvLim[0, 1], uvLim[0, 0]);
				for (int iV = 0; iV < COLS; iV++, i++)
				{
					uv[1] = BLAS.interpolate((double)iV / (double)(COLS - 1), uvLim[1, 1], uvLim[1, 0]);
					s.xVal(uv, ref xyz);
					d[i] = new Point3D();
					Utilities.Vect3ToPoint3D(ref d[i], xyz);
				}
			}
			return d;
		}
		/// <summary>
		/// Creates a regular grid of 3dpoints from the Surface on the specified uv-interval and colors them by gaussian
		/// </summary>
		/// <param name="s">the surface to mesh</param>
		/// <param name="ROWS">the number of constant-u sections</param>
		/// <param name="COLS">the number of constant-v sections</param>
		/// <param name="uvLim">(optional)the uv limits to mesh, uvLim[0,x] = uLim, uvLim[1,x] = vLim</param>
		/// <returns>the grid of points for meshing</returns>
		static public PointRGB[] GetMeshGaussianPoints(ISurface s, int ROWS, int COLS, double[,] uvLim)
		{
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			List<Vect3> xyzs = new List<Vect3>(ROWS * COLS);
			PointRGB[] meshpts = new PointRGB[ROWS * COLS];
			double[] gauss = new double[ROWS * COLS];
			double kMax = -1e9, kMin = 1e9;
			int i = 0;
			for (int iU = 0; iU < ROWS; iU++)
			{
				uv[0] = BLAS.interpolate((double)iU / (double)(ROWS - 1), uvLim[0, 1], uvLim[0, 0]);
				//uv[0] = (double)iU / (double)(ROWS - 1);
				for (int iV = 0; iV < COLS; iV++, i++)
				{
					uv[1] = BLAS.interpolate((double)iV / (double)(COLS - 1), uvLim[1, 1], uvLim[1, 0]);
					//uv[1] = (double)iV / (double)(COLS - 1);
					s.xRad(uv, ref xyz, ref gauss[i]);
					//copy to temp array
					xyzs.Add(new Vect3(xyz));
					//meshpts[i].X = xyz[0];
					//meshpts[i].Y = xyz[1];
					//meshpts[i].Z = xyz[2];
					//track max/min for color scale
					kMax = Math.Max(kMax, gauss[i]);
					kMin = Math.Min(kMin, gauss[i]);
				}
			}
			double ave, q1, q3, stddev = BLAS.StandardDeviation(gauss, out ave, out q3, out q1);
			Color c;
			for (i = 0; i < meshpts.Length; i++)
			{
				c = ColorMath.GetScaleColor(ave + 2 * stddev, ave - 2 * stddev, gauss[i]);
				meshpts[i] = new PointRGB(xyzs[i][0], xyzs[i][1], xyzs[i][2], c);
			}

			return meshpts;

		}
		//static public Point3D[] GetMeshGaussianPoints(ISurface s, int ROWS, int COLS, out double[] gauss)
		//{
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3();
		//	gauss = new double[ROWS * COLS];
		//	Point3D[] d = new Point3D[ROWS * COLS];
		//	int i = 0;
		//	for (int iU = 0; iU < ROWS; iU++)
		//	{
		//		uv[0] = (double)iU / (double)(ROWS - 1);
		//		for (int iV = 0; iV < COLS; iV++, i++)
		//		{
		//			uv[1] = (double)iV / (double)(COLS - 1);
		//			s.xRad(uv, ref xyz, ref gauss[i]);
		//			Utilities.Vect3ToPoint3D(ref d[i], xyz);
		//		}
		//	}
		//	return d;
		//}

		/// <summary>
		/// constructs a mesh representation of a surface using MESHROWS and MESHCOLS for resoultion
		/// </summary>
		/// <param name="s">the surface to mesh</param>
		/// <param name="uvLims">(optional)the uv limits to mesh, uvLim[0,x] = uLim, uvLim[1,x] = vLim</param>
		/// <param name="bGauss">true for gaussian colormap, false for by-layer coloring</param>
		/// <returns>a mesh of either MulticolorPlain or ColorPlain type</returns>
		public static Mesh GetMesh(ISurface s, double[,] uvLims, bool bGauss)
		{
			int rows = MESHU, cols = MESHV;
			//Mesh mesh = new Mesh(meshNatureType.RichPlain);
			meshNatureType meshtype = bGauss ? meshNatureType.MulticolorSmooth : meshNatureType.ColorSmooth;
			Mesh mesh = new Mesh(meshtype);
			mesh.ColorMethod = bGauss ? colorMethodType.byEntity : colorMethodType.byLayer;
			mesh.Vertices = bGauss ? SurfaceTools.GetMeshGaussianPoints(s, rows, cols, uvLims) : SurfaceTools.GetExtensionPoints(s, rows, cols, uvLims);

			mesh.RegenMode = regenType.RegenAndCompile;
			mesh.Triangles = new SmoothTriangle[(rows - 1) * (cols - 1) * 2];
			int count = 0;
			for (int j = 0; j < (rows - 1); j++)
			{
				for (int i = 0; i < (cols - 1); i++)
				{

					mesh.Triangles[count++] = new SmoothTriangle(i + j * cols,
																   i + j * cols + 1,
																   i + (j + 1) * cols + 1);
					mesh.Triangles[count++] = new SmoothTriangle(i + j * cols,
																   i + (j + 1) * cols + 1,
																   i + (j + 1) * cols);
				}
			}

			mesh.ComputeEdges();
			//mesh.ComputeNormals();
			mesh.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;
			mesh.EntityData = s;
			mesh.Selectable = false;
			return mesh;
		}

		/// <summary>
		/// constructs a mesh from the passed array of 3d points
		/// points must be in column-major format
		/// </summary>
		/// <param name="verts">a grid of 3d points to mesh</param>
		/// <param name="rows">the number of rows in the grid</param>
		/// <returns>a ColorPlain mesh object colored by layer</returns>
		public static Mesh GetMesh(Point3D[] verts, int rows)
		{
			int cols = verts.Length / rows;
			Mesh mesh = new Mesh(meshNatureType.ColorSmooth);
			mesh.Vertices = verts;
			mesh.Triangles = new SmoothTriangle[(rows - 1) * (cols - 1) * 2];
			int count = 0;
			for (int j = 0; j < (rows - 1); j++)
			{
				for (int i = 0; i < (cols - 1); i++)
				{

					mesh.Triangles[count++] = new SmoothTriangle(i + j * cols,
																   i + j * cols + 1,
																   i + (j + 1) * cols + 1);
					mesh.Triangles[count++] = new SmoothTriangle(i + j * cols,
																   i + (j + 1) * cols + 1,
																   i + (j + 1) * cols);
				}
			}

			mesh.RegenMode = regenType.RegenAndCompile;
			mesh.ComputeEdges();
			//mesh.ComputeNormals();
			mesh.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;

			return mesh;

		}

		public static void UpdateMesh(Mesh target, Mesh source)
		{
			target.Vertices = source.Vertices;
			target.Triangles = source.Triangles.ToArray();
			//int count = 0;
			//for (int j = 0; j < (rows - 1); j++)
			//{
			//	for (int i = 0; i < (cols - 1); i++)
			//	{

			//		mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
			//													   i + j * cols + 1,
			//													   i + (j + 1) * cols + 1);
			//		mesh.Triangles[count++] = new IndexTriangle(i + j * cols,
			//													   i + (j + 1) * cols + 1,
			//													   i + (j + 1) * cols);
			//	}
			//}

			target.RegenMode = regenType.RegenAndCompile;
			target.ComputeEdges();
			//mesh.ComputeNormals();
			target.NormalAveragingMode = meshNormalAveragingType.AveragedByAngle;

		}
	}
}
