using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept;
using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;


namespace Warps
{
	public class DensityMesh
	{
		double m_radius = .5;//default 1/2 meter circle

		public double Radius
		{
			get { return m_radius; }
			set { m_radius = value; }
		}
		Vect3[,] m_mesh;
		double[,] m_dpi;
		int Rows { get { return m_mesh.GetLength(0); } }
		int Cols { get { return m_mesh.GetLength(1); } }

		void Allocate(int nRow, int nCol)
		{
			m_mesh = new Vect3[nRow, nCol];
			m_dpi = new double[nRow, nCol];

		}
		public void MeshSail(Sail sail)
		{
			List<Yarns.YarnGroup> yarns = new List<Yarns.YarnGroup>();
			sail.Layout.ForEach(grp => { if (grp is Yarns.YarnGroup) yarns.Add(grp as Yarns.YarnGroup); });

			Allocate(30, 20);
			for (int nRow = 0; nRow < Rows; nRow++)
			{
				Parallel.For(0, Cols, nCol =>
				//for (int nCol = 0; nCol < Cols; nCol++)
				{

					double s = 0.5, h = 0;
					Vect2 uv = new Vect2();
					Vect3 xyz = new Vect3();

					uv[0] = BLAS.interpolant(nRow, Rows);
					uv[1] = BLAS.interpolant(nCol, Cols);
					m_mesh[nRow, nCol] = new Vect3();
					sail.Mould.xVal(uv, ref m_mesh[nRow, nCol]);
					yarns.ForEach(grp =>
					{
						grp.ForEach(yar =>
							{
								xyz.Set(m_mesh[nRow, nCol]);
								if (Curves.CurveTools.xClosest(yar, ref s, ref uv, ref xyz, ref h, 1e-5, true) && h < Radius)
									m_dpi[nRow, nCol] += Radius-h;//accumulate yarns in area
							});
					});
				});
			}
		}
		public List<Entity> CreateEntities()
		{
			return new List<Entity>() { SurfaceTools.GetMesh(m_mesh, m_dpi) };
		}
	}
}
