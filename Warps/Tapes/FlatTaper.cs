using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warps.Panels;

namespace Warps.Tapes
{
	public static class FlatTaper
	{
		public static List<FlatSegment> TapeFlatPanel(Panel pan, Vect2 dir, Vect2 start, double dens, double tapeWidth)
		{
			return TapeFlatContour(MakeBoundarySegments(pan), dir, start, dens, tapeWidth);
		}
		public static List<FlatSegment> MakeBoundarySegments(Panel pan)
		{
			List<FlatSegment> edges = new List<FlatSegment>(2 * (pan.TMESH[0] + pan.TMESH[1]));
			FlatSegment seg;
			//get bottom edge
			for (int nSeg = 1; nSeg < pan.m_strips[0].GetLength(1); nSeg++)
			{
				seg = new FlatSegment(
						pan.m_uMesh[0, nSeg-1], //uv0
						pan.m_uMesh[0, nSeg], // uv1
						new Vect2(pan.m_strips[0][0, nSeg - 1].ToArray()), //x0
						new Vect2(pan.m_strips[0][0, nSeg].ToArray())); //x1
				edges.Add(seg);
			}

			//get all strip sides
			int nStrip;
			for( nStrip = 0; nStrip < pan.m_strips.Length; nStrip++ )
				for (int nSide = 0; nSide < pan.m_strips[nStrip].GetLength(1); nSide += (pan.m_strips[nStrip].GetLength(1)-1))
				{
					seg = new FlatSegment(
						pan.m_uMesh[nStrip, nSide], //uv0
						pan.m_uMesh[nStrip+1, nSide], // uv1
						new Vect2(pan.m_strips[nStrip][0, nSide].ToArray()), //x0
						new Vect2(pan.m_strips[nStrip][1, nSide].ToArray())); //x1
					edges.Add(seg);
				}


			//get top edge
			for (int nSeg = 1; nSeg < pan.m_strips[0].GetLength(1); nSeg++)
			{
				seg = new FlatSegment(
						pan.m_uMesh[nStrip, nSeg - 1], //uv0
						pan.m_uMesh[nStrip, nSeg], // uv1
						new Vect2(pan.m_strips[nStrip-1][1, nSeg - 1].ToArray()), //x0
						new Vect2(pan.m_strips[nStrip-1][1, nSeg].ToArray())); //x1
				edges.Add(seg);
			}
			return edges;
		}
		public static List<FlatSegment> TapeFlatContour(List<FlatSegment> edges, Vect2 dir, Vect2 start, double dens, double tapeWidth)
		{
			List<FlatSegment> tapes = new List<FlatSegment>();

			Vect2 nor = dir.Normal();// dir.Rotate(Math.PI / 2.0);//rotate 90 for normal
			nor.Magnitude = tapeWidth / dens;//set normal step distance based on target density

			Vect2 stop;// = start + dir;//project "end" point for segment intersection
			Vect2 end = new Vect2();
			FlatSegment tape;
			double p;
			int nEnd;
			int nTries = 0;
			while (true)
			{
				stop = start + dir;
				tape = new FlatSegment();
				nEnd = 0;
				foreach(FlatSegment edge in edges)
				//edges.ForEach(edge =>
				{
					if (nEnd > 1)
						break;//break once we have found both endpoints

					if (edge.Intersection(start, stop, ref end))
					{
						if (nEnd == 1 && tape[0, true] == end)
							continue;//skip duplicate intersections
						//store endpoint xy
						tape[nEnd, true] = end;

						p = edge.m_xStart.Distance(end) / edge.m_xStart.Distance(edge.m_xStop);//interpolation parameter
						System.Diagnostics.Debug.Assert(Utilities.IsBetween(0, p, 1));
						//interpolate endpoint uv
						tape[nEnd, false] = new Vect2(
							BLAS.interpolate(p, edge[1, false][0], edge[0, false][0]), //interpolate u
							BLAS.interpolate(p, edge[1, false][1], edge[0, false][1]) //interpolate v
							);
						nEnd++;
					}
				}
				//);
				if (nEnd > 1)//both ends found, valid tape
				{
					tapes.Add(tape);
					nTries = 21;//once a valid tape has been created, dont allow any more failed attempts
				}
				else if (nTries > 20)//give 20 tries to walk onto the sail
					return tapes;//return once we walk outside our contour
				else
					nTries++;//count number of failed attempts

				start += nor;//step starting point in normal direction
			}
		}
	}

	public class FlatSegment
	{
		//segment xy and uv points
		public Vect2 m_uStart, m_uStop, m_xStart, m_xStop;

		//line segment xy slope/determinant components
		double A2, B2, C2;

		public FlatSegment() { }
		public FlatSegment(Vect2 uStart, Vect2 uStop, Vect2 xStart, Vect2 xStop)
		{
			m_uStart = uStart; m_uStop = uStop; m_xStart = xStart; m_xStop = xStop;

			// Get A,B,C of second line - points : ps2 to pe2
			A2 = m_xStop[1] - m_xStart[1];
			B2 = m_xStart[0] - m_xStop[0];
			C2 = A2 * m_xStart[0] + B2 * m_xStart[1];
		}

		public Vect2 this[int nEnd, bool nXY]
		{
			get
			{
				switch(nEnd)
				{
					case 0:
						if(nXY)
							return m_xStart;
						else return m_uStart;
					case 1:
						if(nXY)
							return m_xStop;
						else return m_uStop;
				}
				return null;
			}			
			set
			{
				switch(nEnd)
				{
					case 0:
						if(nXY)
							m_xStart = value;
						else m_uStart = value;
						break;
					case 1:
						if(nXY)
							m_xStop = value;
						else m_uStop = value;
						break;
				}
			}
		}


		public bool Intersection(Vect2 start, Vect2 stop, ref Vect2 isect)
		{
			// Get A,B,C of first line - points : ps1 to pe1
			double A1 = stop[1] - start[1];
			double B1 = start[0] - stop[0];
			double C1 = A1 * start[0] + B1 * start[1];

			// Get delta and check if the lines are parallel
			double delta = A1 * B2 - A2 * B1;
			if (delta == 0)
				return false;
				//throw new System.Exception("Lines are parallel");

			// now return the Vector2 intersection point
			isect = new Vect2(
				(B2 * C1 - B1 * C2) / delta,
				(A1 * C2 - A2 * C1) / delta
				);
			//ensure is on segment
			return Utilities.IsBetween(m_xStart[0], isect[0], m_xStop[0]) && Utilities.IsBetween(m_xStart[1], isect[1], m_xStop[1]);
		}
	}

	public static class RosetteTaper
	{
		/// <summary>
		/// calculates a tape rosette
		/// </summary>
		/// <param name="numTapes">the number of tapes in the rosette</param>
		/// <param name="uvCenter">the rosette centerpoint</param>
		/// <param name="angleLimits">the max and min angle to rosette</param>
		/// <param name="xRadii">a list of radii to project the tapes out to</param>
		/// <returns>a list of endpoint uv-value for each tape</returns>
		public static List<Vect2> TapeRosette(ISurface surf, int numTapes, Vect2 uvCenter, Vect2 angleLimits, List<double> xRadii)
		{
			List<Vect2> tapes = new List<Vect2>(numTapes);
			double inAngle = angleLimits[1] - angleLimits[0];//the total included angle
			double incRad = inAngle / (numTapes - 1);//the angle between tapes
			int nRadii = xRadii.Count;
			double rad = angleLimits[0];
			Vect2 uvEnd;
			//find endpoints for each angle and xradius
			for (int nTape = 0; nTape < numTapes; nTape++, rad +=incRad)
			{
				uvEnd = new Vect2();
				//if (SurfaceTools.xAngle(surf, rad, xRadii[nTape % nRadii], uvCenter, ref uvEnd, 1e-6))
				//	tapes.Add(uvEnd);
			}
			return tapes;
		}
	}
}
