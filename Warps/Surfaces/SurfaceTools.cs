﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using Warps.Curves;

namespace Warps
{
	public static class SurfaceTools
	{

		public static int MESHU = 50;
		public static int MESHV = 50;
		public static double[,] MESHLIMITS = new double[,] { { -0.2, 1.2 }, { -0.2, 1.2 } };

		//static public void GetFittingMesh(ISurface s, double vMid, out List<Vect2> uv, out List<Vect3> xyz)
		//{
		//	FixedPoint[] fits = new FixedPoint[2];
		//	fits[0] = new FixedPoint(0, vMid);
		//	fits[1] = new FixedPoint(1, vMid);
		//	SurfaceCurve midcur = new SurfaceCurve("MidCur", s, fits);


		//}

		/// <summary>
		/// calculate the gaussian curvature for a given surface at a given point
		/// </summary>
		/// <param name="s">the surface</param>
		/// <param name="uv">the point to calculate at</param>
		/// <param name="xyz">outputs the xyz location</param>
		/// <param name="k">outputs the gaussian curvature</param>
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

		/// <summary>
		/// gets the closest point on the surface to a target xyz point
		/// </summary>
		/// <param name="s">the surface</param>
		/// <param name="uv">ouputs the closest point in uv-coords</param>
		/// <param name="xyzTarget">input the target point, outputs the closest point on the surface</param>
		/// <param name="dist">the distance from the point to the target</param>
		/// <param name="tol">the acceptable tolerance between the target-vector and the surface normal</param>
		/// <returns>true if succesfully found a point, false if off the surface</returns>
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

		//public static bool xAngle(ISurface s, double rad, double dist, Vect2 uvCenter, ref Vect2 uvEnd, double tol)
		//{
		//	Vect3 xCenter = new Vect3();
		//	Vect3 dxuCenter = new Vect3();
		//	Vect3 dxvCenter = new Vect3();
		//	s.xVec(uvCenter, ref xCenter, ref dxuCenter, ref dxvCenter);

		//	Vect2 uv = new Vect2();
		//	Vect3 x = new Vect3();
		//	Vect3 dxu = new Vect3(), dxv = new Vect3();
		//	Vect3 ddxu = new Vect3(), ddxv = new Vect3(), dduv = new Vect3();

		//	Vect3 h = new Vect3();
		//	Vect2 c = new Vect2();
		//	Vect2 res = new Vect2();
		//	Vect2 a = new Vect2(), b = new Vect2();
		//	double det, r;
		//	Vect2 d = new Vect2();

		//	rad = Math.Cos(rad) * dxnTarget.Magnitude;

		//	int loop = 0, max_loops = 150;
		//	while (loop++ < max_loops)
		//	{
		//		s.xCvt(uv, ref x, ref dxu, ref dxv, ref ddxu, ref ddxv, ref dduv);

		//		h = x - xCenter;
		//		dist = h.Magnitude;

		//		c[0] = h.Dot(dxu) - rad * dist; ;// error
		//		c[1] = h.Dot(dxv) - rad * dist; ;// error

		//		if (Math.Abs(c[0]) < tol && Math.Abs(c[1]) < tol) // error is less than the tolerance
		//		{
		//			xyzTarget.Set(x);// return point to caller
		//			return true;
		//		}

		//		a[0] = dxu.Norm + h.Dot(ddxu);
		//		a[1] = b[0] = dxu.Dot(dxv) + h.Dot(dduv);
		//		b[1] = dxv.Norm + h.Dot(ddxv);

		//		//a[0] = BLAS.dot(dxu, dxu) + BLAS.dot(h, ddxu);
		//		//a[1] = BLAS.dot(dxu, dxv) + BLAS.dot(h, dduv);
		//		//b[0] = a[1];
		//		//b[1] = BLAS.dot(dxv, dxv) + BLAS.dot(h, ddxv);

		//		det = a.Cross(b);
		//		//det = BLAS.cross2d(a, b);

		//		d[0] = c.Cross(b) / det;
		//		d[1] = a.Cross(c) / det;
		//		//d[0] = BLAS.cross2d(c, b) / det;
		//		//d[1] = BLAS.cross2d(a, c) / det;

		//		c[0] = 0.01 > Math.Abs(d[0]) ? 1 : 0.01 / Math.Abs(d[0]);
		//		c[1] = 0.01 > Math.Abs(d[1]) ? 1 : 0.01 / Math.Abs(d[1]);
		//		//enforce maximum increment
		//		r = Math.Min(c[0], c[1]);

		//		//increment uv by scaled residuals
		//		//uv = BLAS.subtract(uv, BLAS.scale(d, r));
		//		uv = uv - d * r;
		//		//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
		//	}
		//	//s = s0;
		//	return false;
		//}

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
			if (uvLim == null)
				uvLim = new double[,] { { 0, 1 }, { 0, 1 } };
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
		static public PointRGB[] GetMeshGaussianPoints(ISurface s, int ROWS, int COLS, double[,] uvLim, out double[] cGauss)
		{
			if (uvLim == null)
				uvLim = new double[,] { { 0, 1 }, { 0, 1 } };
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			List<Vect3> xyzs = new List<Vect3>(ROWS * COLS);
			PointRGB[] meshpts = new PointRGB[ROWS * COLS];
			cGauss = new double[ROWS * COLS];
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
					s.xRad(uv, ref xyz, ref cGauss[i]);
					//copy to temp array
					xyzs.Add(new Vect3(xyz));
					//meshpts[i].X = xyz[0];
					//meshpts[i].Y = xyz[1];
					//meshpts[i].Z = xyz[2];
					//track max/min for color scale
					kMax = Math.Max(kMax, cGauss[i]);
					kMin = Math.Min(kMin, cGauss[i]);
				}
			}
			double ave, q1, q3, stddev = BLAS.StandardDeviation(cGauss, out ave, out q3, out q1);
			Color c;
			for (i = 0; i < meshpts.Length; i++)
			{
				c = ColorMath.GetScaleColor(ave + 2 * stddev, ave - 2 * stddev, cGauss[i]);
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
		/// creates a mesh from the given surface
		/// </summary>
		/// <param name="surf">the surface to mesh</param>
		/// <param name="uvLims">the u and v limits, if null defaults to [0,1]</param>
		/// <param name="bGauss">true for gaussian colormap, false for solid-color</param>
		/// <returns>a new mesh object</returns>
		public static Mesh GetMesh(ISurface surf, double[,] uvLims, bool bGauss)
		{
			if (uvLims != null)//extension surface, ignore OUTER
				return SurfaceTools.GetUVMesh(surf, uvLims, bGauss);

			//find OUTER
			MouldCurve luff = WarpFrame.CurrentSail.FindCurve("Luff");
			MouldCurve leech = WarpFrame.CurrentSail.FindCurve("Leech");
			MouldCurve head = WarpFrame.CurrentSail.FindCurve("Head");
			MouldCurve foot = WarpFrame.CurrentSail.FindCurve("Foot");

			//make trimmed mesh
			if (luff != null && leech != null && foot != null && head != null)
				return SurfaceTools.GetMesh(surf, luff, head, leech, foot);
			else
				return SurfaceTools.GetUVMesh(surf, uvLims, bGauss);
		}
		static Mesh GetMesh(ISurface surf, IMouldCurve luff, IMouldCurve head, MouldCurve leech, IMouldCurve foot)
		{


			Vect2 st = new Vect2();
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			int FITEDGES = 50;
			List<double[]> sts = new List<double[]>(FITEDGES * 4);
			List<double[]> uvs = new List<double[]>(FITEDGES * 4);


			for (int i = 0; i < FITEDGES; i++)//run along foot and head
			{
				st[0] = BLAS.interpolant(i, FITEDGES);
				st[1] = 0; //foot
				foot.uVal(st[0], ref uv);
				sts.Add(st.ToArray());
				uvs.Add(uv.ToArray());

				st[1] = 1; //head
				head.uVal(st[0], ref uv);
				sts.Add(st.ToArray());
				uvs.Add(uv.ToArray());
			}

			List<double> kinks = leech.GetKinkPositions();
			int nSeg = FITEDGES / (kinks.Count - 1) + 1;

			//for (int j = 1; j < FITEDGES - 1; j++)//only do internals for lu/le to avoid duplicate corners
			//{
			for (int seg = 0; seg < kinks.Count - 1; seg++)//only do internals for lu/le to avoid duplicate corners
			{
				for (int j = 1; j <= nSeg; j++)
				{
					st[1] = BLAS.interpolate(j, nSeg, kinks[seg + 1], kinks[seg]);

					if (BLAS.IsEqual(st[1], 1, 1e-9))
						break;//avoid duplicate corners

					st[0] = 0; //luff
					luff.uVal(st[1], ref uv);
					sts.Add(st.ToArray());
					uvs.Add(uv.ToArray());

					st[0] = 1; //leech
					leech.uVal(st[1], ref uv);
					sts.Add(st.ToArray());
					uvs.Add(uv.ToArray());
				}
			}

			RBF.RBFNetwork ntw = new RBF.RBFNetwork();
			ntw.Fit(sts, uvs);

			//calc new segment count for meshing
			nSeg = MESHV / (kinks.Count - 1) + 1;

			Vect3[,] mesh = new Vect3[MESHU, nSeg * (kinks.Count - 1)];
			//populate internal nodes
			for (int i = 0; i < MESHU; i++)
			{
				st[0] = BLAS.interpolant(i, MESHU);
				for (int seg = 0; seg < kinks.Count - 1; seg++)
				{
					for (int j = 0; j < nSeg; j++)
					{
						st[1] = BLAS.interpolate(j, nSeg + 1, kinks[seg + 1], kinks[seg]);
						ntw.Value(st.m_vec, ref uv.m_vec);
						mesh[i, j + seg * nSeg] = new Vect3();
						surf.xVal(uv, ref mesh[i, j + seg * nSeg]);
					}
				}
				//do headpoint
				st[1] = 1;
				ntw.Value(st.m_vec, ref uv.m_vec);
				mesh[i, mesh.GetLength(1) - 1] = new Vect3();
				surf.xVal(uv, ref mesh[i, mesh.GetLength(1) - 1]);
			}

			return GetMesh(mesh);
		}

		/// <summary>
		/// constructs a mesh representation of a surface using MESHROWS and MESHCOLS for resoultion
		/// </summary>
		/// <param name="s">the surface to mesh</param>
		/// <param name="uvLims">(optional)the uv limits to mesh, uvLim[0,x] = uLim, uvLim[1,x] = vLim</param>
		/// <param name="bGauss">true for gaussian colormap, false for by-layer coloring</param>
		/// <returns>a mesh of either MulticolorPlain or ColorPlain type</returns>
		static Mesh GetUVMesh(ISurface s, double[,] uvLims, bool bGauss)
		{
			int rows = MESHU, cols = MESHV;
			//Mesh mesh = new Mesh(meshNatureType.RichPlain);
			double[] cGauss = null;
			Mesh mesh = new Mesh(bGauss ? meshNatureType.MulticolorSmooth : meshNatureType.ColorSmooth);
			mesh.ColorMethod = bGauss ? colorMethodType.byEntity : colorMethodType.byLayer;
			mesh.Vertices = bGauss ? SurfaceTools.GetMeshGaussianPoints(s, rows, cols, uvLims, out cGauss) : SurfaceTools.GetExtensionPoints(s, rows, cols, uvLims);
			//record the color vals for displaying colorscale
			s.ColorValues = cGauss;

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

		public static Mesh GetMesh(Vect3[,] verts)
		{
			return GetMesh(verts, null, false);
		}
		public static Mesh GetMesh(Vect3[,] verts, double[,] colors)
		{
			return GetMesh(verts, colors, false);
		}
		public static Mesh GetMesh(Vect3[,] verts, double[,] colors, bool bLog)
		{
			double ave = 0, q1 = 0, q3 = 0, stddev = 0;
			if (colors != null)
			{
				stddev = BLAS.StandardDeviation(colors, out ave, out q3, out q1);
				if (bLog)
				{
					q3 = Math.Log(q3);
					q1 = Math.Log(q1);
				}
				//q1 = ave - 2 * stddev;
				//q3 = ave + 2 * stddev;
			}
			List<Point3D> pnts = new List<Point3D>(verts.Length);
			for (int i = 0; i < verts.GetLength(0); i++)
			{
				for (int j = 0; j < verts.GetLength(1); j++)
				{
					if (colors != null)
						pnts.Add(Utilities.Vect3ToPointRGB(verts[i, j], ColorMath.GetScaleColor(q3, q1, bLog ? Math.Log(colors[i, j]) : colors[i, j])));
					else
						pnts.Add(Utilities.Vect3ToPoint3D(verts[i, j]));
				}
			}
			return GetMesh(pnts.ToArray(), verts.GetLength(0));

		}


		public static Mesh GetMesh(List<double[]> verts, int rows)
		{
			List<Point3D> pnts = verts.ConvertAll<Point3D>(new Converter<double[], Point3D>(Utilities.DoubleToPoint3D));
			return GetMesh(pnts.ToArray(), rows);

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
			bool colors = verts[0] is PointRGB;
			int cols = verts.Length / rows;
			Mesh mesh = new Mesh(colors ? meshNatureType.MulticolorSmooth : meshNatureType.ColorSmooth);
			mesh.ColorMethod = colors ? colorMethodType.byEntity : colorMethodType.byLayer;
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
			mesh.NormalAveragingMode = meshNormalAveragingType.Averaged;

			return mesh;

		}

		/// <summary>
		/// updates the vertices of the target mesh with the vertices from the source mesh
		/// </summary>
		/// <param name="target">the mesh to copy to</param>
		/// <param name="source">the mesh to copy from</param>
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

		internal static Entity GetPointCloud(Vect3[,] verts)
		{
			List<Point3D> pnts = new List<Point3D>(verts.Length);
			for (int i = 0; i < verts.GetLength(0); i++)
			{
				for (int j = 0; j < verts.GetLength(1); j++)
					pnts.Add(Utilities.Vect3ToPoint3D(verts[i, j]));
			}
			PointCloud cloud = new PointCloud(pnts);
			cloud.LineWeight = 4;
			cloud.LineWeightMethod = colorMethodType.byEntity;
			return cloud;
		}
		internal static Entity GetPointCloud(Vect3[] verts)
		{
			List<Point3D> pnts = new List<Point3D>(verts.Length);
			for (int i = 0; i < verts.Length; i++)
			{
				pnts.Add(Utilities.Vect3ToPoint3D(verts[i]));
			}
			PointCloud cloud = new PointCloud(pnts);
			cloud.LineWeight = 10;
			cloud.LineWeightMethod = colorMethodType.byEntity;
			return cloud;
		}
		internal static Entity GetMesh(Point2D[,] strip)
		{
			List<Point3D> pnts = new List<Point3D>(strip.Length);
			for (int i = 0; i < strip.GetLength(0); i++)
			{
				for (int j = 0; j < strip.GetLength(1); j++)
					pnts.Add(new Point3D(strip[i, j].X, strip[i, j].Y));
			}
			return GetMesh(pnts.ToArray(), strip.GetLength(0));
		}

		internal static Entity GetMesh(Vect2[,] uMesh)
		{
			int nI = uMesh.GetLength(0), nJ = uMesh.GetLength(1);
			Vect3[,] verts = new Vect3[nI, nJ];
			//Parallel.For(0, nI, i =>
			for (int i = 0; i < nI; i++)
			{
				for (int j = 0; j < nJ; j++)
				{
					verts[i, j] = uMesh[i, j].ToVect3();
				}
			}
			//);
			return GetMesh(verts);
		}
	}

}