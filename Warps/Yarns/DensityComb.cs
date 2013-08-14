using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warps;
using Warps.Yarns;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
namespace Warps
{
	[System.Diagnostics.DebuggerDisplay("{Label} {DPI}", Name = "{Label}", Type = "{GetType()}")]
	public class DensityComb : GuideComb
	{
		public DensityComb(YarnGroup group, double s)
			:base(group.Label + s.ToString("0.000"), group.Sail, null, null)
		{
			m_Group = group;
			m_sPos = s;
			FitCurve();
		}
		public double DPI = 0;

		private double FitCurve()
		{
			if (m_Group == null || m_Group.Count < 5)
				return 0;


			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3(), xprev = new Vect3();

			List<IFitPoint> fit = new List<IFitPoint>(m_Group.Count);
			//get target point on starting yarn
			//yar0.xVal(m_sPos, ref uv, ref xyz);
			double s1 = m_sPos, d = 0;
			//fit.Add(new FixedPoint(uv));
			double length = 0, h=0;
			Vect2 v;
			List<Vect2> combs = new List<Vect2>(m_Group.Count);
			for (int i = 0; i < m_Group.Count; i++)
			{
				m_Group[i].xVal(s1, ref uv, ref xprev);
				fit.Add(new FixedPoint(uv[0], uv[1]));
				//find closest point on yar1 to the target point on yar0;
				if (i < m_Group.Count - 1)
				{
					xyz.Set(xprev);
					//find the spacing to the next yarn
					if (!CurveTools.xClosest(m_Group[i + 1], ref s1, ref uv, ref xyz, ref d, 1e-6, true))
						Logger.logger.Instance.Log("xClosest failed in DensityComb");
						//throw new Exception("xClosest failed in DensityComb");
					h = xyz.Distance(xprev);
					//store the spacing and x-distance
					v = new Vect2();
					v[0] = length;
					v[1] = m_Group.InverseSpacing(h);
					combs.Add(v);

					length += h;//accumulate length
				}
			}

			Length = length;
			//convert x-distance to s-spacing
			for (int i = 0; i < combs.Count; i++)
				fit[i][0] = combs[i][0] /= length;
			combs.Add(new Vect2(1, m_Group.InverseSpacing(h)));//add endpoint with spacing to previous curve
			//fit to the intersection points
			SurfaceCurve.SimpleFit(this, fit.ToArray());
			FitComb(combs.ToArray());
			return DPI = m_Group.YarnDenier * .0254 * (m_Group.Count - 1) / length;
		}
		YarnGroup m_Group;
		double m_sPos;

		public override void Fit(IFitPoint[] points)
		{
			FitCurve();
		}

		public override List<devDept.Eyeshot.Entities.Entity> CreateEntities(bool bFitPoints, double TolAngle, out List<double> sPos)
		{
			List<Entity> e = base.CreateEntities(bFitPoints, TolAngle, out sPos);
			if (sPos == null || e.Count == 0)
				return e;
			LinearPath curve = e[0] as LinearPath;
			if( curve == null )
				return e;
			//remove default comb
			e.Clear();
			//e.RemoveRange((e.Count - sPos.Length - 1), sPos.Length + 1);

			//add original curve
			e.Add(curve);

			//add comb entities
			e.AddRange(CreateCombEntity(SComb, true));

			return e;
		}
		
	}
}
