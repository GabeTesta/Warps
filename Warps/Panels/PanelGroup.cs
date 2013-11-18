using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using Warps.Curves;
using System.Xml;

namespace Warps.Panels
{
	[System.Diagnostics.DebuggerDisplay("{Label} {m_clothAlignment} [{Count}]", Name = "{Label}", Type = "{GetType()}")]
	public class PanelGroup : List<Panel>, IGroup
	{
		public PanelGroup() : this("", null, new Equation("", 1.0), ClothOrientations.FILLS) { }
		public PanelGroup(string label, Sail s) : this(label, s, new Equation("panelwidth", 1.0), ClothOrientations.FILLS) { }
		public PanelGroup(string label, Sail s, Equation width, ClothOrientations alignment)
		{
			Label = label;
			Sail = s;
			Bounds = new List<IMouldCurve>();
			Guides = new List<IMouldCurve>();
			PanelWidth = width;
			ClothAlignment = alignment;
		}

		#region Members

		//IGroup
		Sail m_sail;
		string m_label;

		Equation m_Width;
		List<IMouldCurve> m_bounds;
		List<IMouldCurve> m_guides;
		ClothOrientations m_clothAlignment = ClothOrientations.FILLS;


		public double Width
		{
			get { return m_Width.Value; }
			//set { m_Width = value; }
		}

		public Equation PanelWidth
		{
			get { return m_Width; }
			set { m_Width = value; m_Width.Label = "PanelWidth"; }
		}

		public List<IMouldCurve> Bounds
		{
			get { return m_bounds; }
			set { m_bounds = value; }
		}
		public List<IMouldCurve> Guides
		{
			get { return m_guides; }
			set { m_guides = value; }
		}
		public ClothOrientations ClothAlignment
		{
			get { return m_clothAlignment; }
			set { m_clothAlignment = value; }
		}
		public enum ClothOrientations
		{
			WARPS,
			FILLS,
			PERPS
		}

		#endregion

		public void WriteBin(System.IO.BinaryWriter bin) { }

		public bool IsEqual(PanelGroup temp)
		{
			if (temp == null)
				return false;
			if (Label != temp.Label)
				return false;
			if (m_Width != temp.m_Width)
				return false;
			if (ClothAlignment != temp.ClothAlignment)
				return false;

			if (Bounds.Count != temp.Bounds.Count)
				return false;
			for (int i = 0; i < Bounds.Count; i++)
				if (Bounds[i] != temp.Bounds[i])
					return false;

			if (Guides.Count != temp.Guides.Count)
				return false;
			for (int i = 0; i < Guides.Count; i++)
				if (Guides[i] != temp.Guides[i])
					return false;

			return true;
		}

		const int MAX_PANS = 50;

		//#region OLD
		//public int LayoutPanels(List<MouldCurve> boundaryCurves, List<MouldCurve> guides, ClothOrientations align, double width)
		//{
		//	//store values
		//	if (boundaryCurves != null)
		//		Bounds = boundaryCurves;
		//	if (guides != null)
		//		Guides = guides;
		//	ClothAlignment = align;

		//	if (Guides == null || Bounds == null)
		//		return -1;
		//	else if (Guides.Count == 0 || Bounds.Count == 0)
		//		return -1;

		//	//if (width.Result > 0)
		//	//m_Width = width;
		//	Clear();

		//	//get boundary intersection uv and s pos
		//	Vect2 sCor = new Vect2();
		//	Vect3[] xCorners = new Vect3[Bounds.Count];//xyz corner points
		//	Vect2[] uCorners = new Vect2[Bounds.Count];//uv corner points
		//	sCorners = new Vect2[Bounds.Count];//s pos of each corner for each curve 0:start, 1:end
		//	if (Bounds.Count > 0) sCorners[0] = new Vect2();//initialize starting scorner
		//	for (int nB = 0; nB < Bounds.Count; nB++)
		//	{
		//		int nF = nB == Bounds.Count - 1 ? 0 : nB + 1;//forward boundaries index
		//		uCorners[nB] = new Vect2();
		//		xCorners[nB] = new Vect3();
		//		if (!CurveTools.CrossPoint(Bounds[nB], Bounds[nF], ref uCorners[nB], ref xCorners[nB], ref sCor, 20))
		//			throw new Exception(string.Format("Panel Group [{0}] Seam [{1}] does not intersect Seam [{2}]", Label, Bounds[nB].Label, Bounds[nF].Label));

		//		if (nF > 0) sCorners[nF] = new Vect2();//initialize forward scorner
		//		sCorners[nB][1] = sCor[0];//this curve's endpos
		//		sCorners[nF][0] = sCor[1];//next curve's startpos
		//	}

		//	//get guide/boundary intersections
		//	List<GuideCross>[] xGuides = new List<GuideCross>[Guides.Count];
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3();
		//	//Vect3[] xGuides = new Vect3[Guides.Count];
		//	//Vect2[] uGuides = new Vect2[Guides.Count];//uv guide intersections
		//	//Vect2[] sGuides = new Vect2[Guides.Count];//s pos of each corner for each curve 0:start, 1:end

		//	//find intersections for each guidecurve
		//	for (int nG = 0; nG < Guides.Count; nG++)
		//	{
		//		xGuides[nG] = new List<GuideCross>(2);//assume 2 intersections
		//		for (int nB = 0; nB < Bounds.Count; nB++)
		//			if (CurveTools.CrossPoint(Guides[nG], Bounds[nB], ref uv, ref xyz, ref sCor, 20))
		//			{
		//				//int nC = sCorners[nB][0] > sCorners[nB][1] ? 1 : 0;//accomodate curve direction
		//				//ensure intersection is inside our bounds
		//				//if (sCorners[nB][nC] <= sCor[1] && sCor[1] <= sCorners[nB][1-nC])
		//				if (Utilities.IsBetween(sCorners[nB][0], sCor[1], sCorners[nB][1]))
		//				{
		//					bool bdupe = false;
		//					foreach (GuideCross cross in xGuides[nG])
		//						if (BLAS.IsEqual(cross.sPos, sCor[0]))
		//							bdupe = true;
		//					if (!bdupe)//dont add duplicate ins
		//						xGuides[nG].Add(new GuideCross(sCor[0], uv, xyz, Bounds[nB], sCor[1]));//store all intersections
		//				}
		//			}
		//		if (xGuides[nG].Count != 2) //guide doesnt cross bounds or hits more than 2
		//			throw new Exception(string.Format("Panel Group [{0}] Guide Curve [{1}] does not intersect 2 boundry warps", Label, Guides[nG].Label));
		//	}

		//	//determine back and fore seams using target width
		//	GuideCross[] panCorners = new GuideCross[4];//one for each panel corner
		//	MouldCurve[] seams = new MouldCurve[2];//2 primary seams
		//	MouldCurve[] edges = new MouldCurve[2];//2 sliding edge curves
		//	//construct initial backseam as a segment of the bounding curve
		//	switch (ClothAlignment)
		//	{
		//		case ClothOrientations.WARPS:
		//			//use bracketing bounds as initial edges
		//			edges[0] = Bounds[1];
		//			edges[1] = Bounds[Bounds.Count - 1];
		//			//use first boundary segment as initial seam
		//			//seams[0] = new MouldCurve(Label + Count.ToString("000"), Bounds[0], sCorners[0]);
		//			seams[0] = new MouldCurve(Label + Count.ToString("000"), Sail,
		//				new IFitPoint[]{ 
		//					new CurvePoint(edges[0], sCorners[1][0]), 
		//					new CurvePoint(edges[1], sCorners[Bounds.Count - 1][1])});
		//			break;
		//		case ClothOrientations.FILLS:
		//			//start on initial intersection seam
		//			seams[1] = xGuides[0][0].Seam;
		//			seams[0] = new MouldCurve(Label + Count.ToString("000"), xGuides[0][0].Seam, sCorners[m_bounds.IndexOf(xGuides[0][0].Seam)]);

		//			break;
		//		case ClothOrientations.PERPS:
		//			break;
		//	}
		//	int nNwt = 0;
		//	bool atSeam = false;
		//	Vect2 sPos = new Vect2();
		//	//	Vect3[] xEnd = new Vect3[2];
		//	int nGuide = 0;

		//	do
		//	{
		//		width = Width;//initialize guess width
		//		//iterate on width to lay second seam
		//		switch (ClothAlignment)
		//		{
		//			case ClothOrientations.WARPS:
		//				//get seam/bound interesctions
		//				//project up bounds to get target width at endpoints
		//				//span girth between endpoints

		//				//initialize seam as segment of next guidecurve
		//				//seams[1] = new MouldCurve(Label + (Count + 1).ToString("000"), Guides[nGuide], new Vect2(xGuides[nGuide][0].sPos, xGuides[nGuide][1].sPos));
		//				seams[1] = new MouldCurve(Label + (Count + 1).ToString("000"), Sail,
		//					new IFitPoint[]{ 
		//					xGuides[nGuide][0].GetSeamPoint(), 
		//					xGuides[nGuide][1].GetSeamPoint()});

		//				double wEnd = 0;
		//				for (nNwt = 0; nNwt < 50; nNwt++)
		//				{
		//					wEnd = 0;
		//					for (int nEnd = 0; nEnd < 2; nEnd++)
		//					{
		//						//get seam endpoints
		//						seams[0].xVal(nEnd, ref uv, ref xyz);
		//						panCorners[nEnd * 2] = new GuideCross((double)nEnd, uv, xyz, edges[0], 0);
		//						if (seams[0].FitPoints[nEnd * (seams[0].FitPoints.Length - 1)] is CurvePoint)
		//						{
		//							panCorners[nEnd * 2].Seam = (seams[0].FitPoints[nEnd * (seams[0].FitPoints.Length - 1)] as CurvePoint).Curve;
		//							panCorners[nEnd * 2].sSeam = (seams[0].FitPoints[nEnd * (seams[0].FitPoints.Length - 1)] as CurvePoint).SCurve;
		//						}

		//						seams[1].xVal(nEnd, ref uv, ref xyz);
		//						panCorners[nEnd * 2 + 1] = new GuideCross((double)nEnd, uv, xyz, edges[1], 0);
		//						if (seams[1].FitPoints[nEnd * (seams[1].FitPoints.Length - 1)] is CurvePoint)
		//						{
		//							panCorners[nEnd * 2 + 1].Seam = (seams[1].FitPoints[nEnd * (seams[1].FitPoints.Length - 1)] as CurvePoint).Curve;
		//							panCorners[nEnd * 2 + 1].sSeam = (seams[1].FitPoints[nEnd * (seams[1].FitPoints.Length - 1)] as CurvePoint).SCurve;
		//						}

		//						//get distance from crosses to endpoints
		//						wEnd += panCorners[nEnd * 2].xPos.Distance(panCorners[nEnd * 2 + 1].xPos);
		//					}
		//					//average to get "width"
		//					wEnd /= 2.0;
		//					//break if ontarget
		//					if (BLAS.IsEqual(wEnd, width))
		//						break; //seam is good

		//					//adjust using max % method
		//					wEnd = width / wEnd;//	get target/width %

		//					//interpolate new end positions using % target
		//					sPos[0] = BLAS.interpolate(wEnd, panCorners[1].sSeam, panCorners[0].sSeam);
		//					sPos[1] = BLAS.interpolate(wEnd, panCorners[3].sSeam, panCorners[2].sSeam);

		//					seams[1] = new MouldCurve(seams[1].Label, Sail, new IFitPoint[] { new CurvePoint(edges[0], sPos[0]), new CurvePoint(edges[1], sPos[1]) });

		//				}
		//				if (nNwt == 50)
		//					throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

		//				//check to see if we've stepped past the boundary cornerpoints
		//				int nEdg = Bounds.IndexOf(edges[0]);
		//				if (!Utilities.IsBetween(sCorners[nEdg][0], sPos[0], sCorners[nEdg][1]))
		//					atSeam = true;

		//				nEdg = Bounds.IndexOf(edges[1]);
		//				if (!Utilities.IsBetween(sCorners[nEdg][0], sPos[0], sCorners[nEdg][1]))
		//					atSeam = true;

		//				Add(new Panel(seams, null));

		//				break;
		//			case ClothOrientations.FILLS:
		//				//single guide
		//				//get guide/seam0 intersection
		//				if (!CurveTools.CrossPoint(Guides[0], seams[0], ref uv, ref xyz, ref sPos, 20))
		//					break;
		//				GuideCross start = new GuideCross(sPos[0], uv, xyz, seams[0], sPos[1]);
		//				//estimate delta-x/delta-s for width based guide-stepping
		//				double cord = xGuides[0][1].xPos.Distance(xGuides[0][0].xPos);
		//				double span = xGuides[0][1].sPos - xGuides[0][0].sPos;
		//				double dxds = cord / span;
		//				double ds = width / dxds;//guess delta s
		//				for (nNwt = 0; nNwt < 50; nNwt++)
		//				{
		//					//project up guide to target width
		//					Guides[0].xVal(start.sPos + ds, ref uv, ref xyz);

		//					//check "width"
		//					span = xyz.Distance(start.xPos);
		//					if (BLAS.is_equal(span, width, 1e-5))
		//						break;//success

		//					dxds = span / ds;//recalculate dxds using current step
		//					ds = width / dxds;//recalc ds using new gradient
		//				}
		//				if (nNwt == 50)
		//					throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

		//				double s = start.sPos + ds;
		//				if (!Utilities.IsBetween(xGuides[0][0].sPos, s, xGuides[0][1].sPos))
		//				{
		//					atSeam = true;
		//					break;
		//				}

		//				//span girth on curve normal to bounds intersections
		//				//CurvePoint startpt = new CurvePoint(Guides[0], s);
		//				//set the endpoint angle
		//				Vect3 xyzGuide = new Vect3();
		//				Vect3 dxnGuide = new Vect3();
		//				Guides[0].xVec(s, ref uv, ref xyzGuide, ref dxnGuide);//get the target point and tangent vector
		//				List<CurvePoint> ends = new List<CurvePoint>(2);
		//				for (int nB = 0; nB < Bounds.Count; nB++)
		//				{
		//					xyz.Set(xyzGuide);//initialize guess
		//					s = start.sPos + ds;
		//					if (!CurveTools.AnglePoint(Bounds[nB], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
		//						continue; //no intersection with this boundary curve

		//					//set the angle and ensure it's in our bracket
		//					if (Utilities.IsBetween(sCorners[nB][0], s, sCorners[nB][1]))
		//					{
		//						ends.Add(new CurvePoint(Bounds[nB], s));
		//						//make edge seams here using nB index and seams[0] fitpoints' indices

		//					}
		//				}
		//				if (ends.Count != 2)
		//					throw new Exception(string.Format("Panel Group [{0}] failed to set angle points for panel [{1}]", Label, Count));

		//				//fit the girth seam
		//				seams[1] = new MouldCurve(Label + (Count + 1).ToString("000"), Sail, ends.ToArray());
		//				Add(new Panel(seams, MakeEndSegments(seams)));
		//				//set seam as next starting seam
		//				//seams[0] = seams[1];
		//				break;
		//			case ClothOrientations.PERPS:
		//				//get guide cord
		//				//project up cord to target width
		//				//forward step girth to bounds intersections
		//				break;
		//		}

		//		//set seam as next starting seam
		//		seams[0] = seams[1];

		//		//construct end-cap poly curves
		//		//flatten and check actual width(panel.Width) vs input width(m_Width)
		//		//iterate target width to hit input width
		//	}
		//	while (!atSeam && Count < MAX_PANS);

		//	return atSeam ? Count : -Count;
		//}

		//#endregion

		#region Corners	

		int NumSegments
		{
			get { return Bounds.Count; }
		}
		bool InSegment(int nSeg, double s)
		{
			return Utilities.IsBetween(Corners[nSeg].sPos[1], s, Corners[(nSeg+1)%NumSegments].sPos[0]);
		}

		PanelCorner[] Corners;
		void SetCorners()
		{
			if (Bounds == null)
			{
				Corners = null;
				return;
			}

			//get boundary intersection uv and s pos
			Vect2 sCor = new Vect2();
			Corners = new PanelCorner[NumSegments];
			Corners[0] = new PanelCorner(Bounds[NumSegments - 1], Bounds[0]);
			for (int nB = 1; nB < NumSegments; nB++)
			{
				Corners[nB] = new PanelCorner(Bounds[nB-1], Bounds[nB]);
			}
		}

		//void SetCorners()
		//{
		//	if (Bounds == null)
		//	{
		//		Corners = null;
		//		//xCorners = null;//xyz corner points
		//		//uCorners = null;//uv corner points
		//		//sCorners = null;//s pos of each corner for each curve 0:start, 1:end
		//		return;
		//	}

		//	//get boundary intersection uv and s pos
		//	Vect2 sCor = new Vect2();
		//	Corners = new PanelCorner[Bounds.Count];
		//	if (Bounds.Count > 0) Corners[0] = new PanelCorner();//initialize starting corner
		//	//xCorners = new Vect3[Bounds.Count];//xyz corner points
		//	//uCorners = new Vect2[Bounds.Count];//uv corner points
		//	//sCorners = new Vect2[Bounds.Count];//s pos of each corner for each curve 0:start, 1:end
		//	//if (Bounds.Count > 0) sCorners[0] = new Vect2();//initialize starting scorner
		//	for (int nB = 0; nB < Bounds.Count; nB++)
		//	{
		//		//int nF = nB == Bounds.Count - 1 ? 0 : nB + 1;//forward boundaries index
		//		int nF = (nB+1) % Bounds.Count;//forward boundaries index
		//		if (nF > 0) Corners[nF] = new PanelCorner();//create forward corner

		//		Corners[nB].Seams[0] = Bounds[nB];
		//		Corners[nB].Seams[1] = Bounds[nF];
		//		//uCorners[nB] = new Vect2();
		//		//xCorners[nB] = new Vect3();
		//		if (!CurveTools.CrossPoint(Bounds[nB], Bounds[nF], ref Corners[nB].uPos, ref Corners[nB].xPos, ref Corners[nB].sPos, 20))
		//			throw new Exception(string.Format("Panel Group [{0}] Edge [{1}] does not intersect Edge [{2}]", Label, Bounds[nB].Label, Bounds[nF].Label));


		//		if (nF > 0) sCorners[nF] = new Vect2();//initialize forward scorner
		//		sCorners[nB][1] = sCor[0];//this curve's endpos
		//		sCorners[nF][0] = sCor[1];//next curve's startpos
		//	}
		//}
		//Vect3[] xCorners;// = new Vect3[Bounds.Count];//xyz corner points
		//Vect2[] uCorners;//= new Vect2[Bounds.Count];//uv corner points
		//Vect2[] sCorners;

		List<PanelCorner>[] GuideCross;
		void SetGuides()
		{
			if (Guides == null || Bounds == null)
			{
				GuideCross = null;
				return;
			}
			//get guide/boundary intersections
			GuideCross = new List<PanelCorner>[Guides.Count];
			Vect2 sCor = new Vect2();
			//find intersections for each guidecurve
			for (int nG = 0; nG < Guides.Count; nG++)
			{
				GuideCross[nG] = new List<PanelCorner>(2);//assume 2 intersections
				for (int nB = 0; nB < NumSegments; nB++)//check each boundary
					if (CurveTools.CrossPoint(Guides[nG], Bounds[nB], ref uv, ref xyz, ref sCor, 20))
					{
						//ensure intersection is inside our bounds
						if ( InSegment(nB, sCor[1]) )
						{
							bool bdupe = false;
							foreach (PanelCorner cross in GuideCross[nG])
								if (BLAS.IsEqual(cross.sPos[0], sCor[0]))
									bdupe = true;
							if (!bdupe)//dont add duplicate ins
								GuideCross[nG].Add(new PanelCorner(Guides[nG], Bounds[nB], sCor, uv, xyz));//store intersection
						}
					}
				if (GuideCross[nG].Count != 2) //guide doesnt cross bounds or hits more than 2
					throw new Exception(string.Format("Panel Group [{0}] Guide Curve [{1}] does not intersect exactly 2 boundry warps", Label, Guides[nG].Label));
			}
		}

		//void SetGuides()
		//{
		//	if (Guides == null || Bounds == null)
		//	{
		//		xGuides = null;
		//		return;
		//	}
		//	//get guide/boundary intersections
		//	xGuides = new List<GuideCross>[Guides.Count];
		//	//Vect2 uv = new Vect2();
		//	//Vect3 xyz = new Vect3();
		//	Vect2 sCor = new Vect2();
		//	//Vect3[] xGuides = new Vect3[Guides.Count];
		//	//Vect2[] uGuides = new Vect2[Guides.Count];//uv guide intersections
		//	//Vect2[] sGuides = new Vect2[Guides.Count];//s pos of each corner for each curve 0:start, 1:end

		//	//find intersections for each guidecurve
		//	for (int nG = 0; nG < Guides.Count; nG++)
		//	{
		//		xGuides[nG] = new List<GuideCross>(2);//assume 2 intersections
		//		for (int nB = 0; nB < Bounds.Count; nB++)//check each boundary
		//			if (CurveTools.CrossPoint(Guides[nG], Bounds[nB], ref uv, ref xyz, ref sCor, 20))
		//			{
		//				//int nC = sCorners[nB][0] > sCorners[nB][1] ? 1 : 0;//accomodate curve direction
		//				//ensure intersection is inside our bounds
		//				//if (sCorners[nB][nC] <= sCor[1] && sCor[1] <= sCorners[nB][1-nC])
		//				if (Utilities.IsBetween(sCorners[nB][0], sCor[1], sCorners[nB][1]))
		//				{
		//					bool bdupe = false;
		//					foreach (GuideCross cross in xGuides[nG])
		//						if (BLAS.IsEqual(cross.sPos, sCor[0]))
		//							bdupe = true;
		//					if (!bdupe)//dont add duplicate ins
		//						xGuides[nG].Add(new GuideCross(sCor[0], uv, xyz, Bounds[nB], sCor[1]));//store intersection
		//				}
		//			}
		//		if (xGuides[nG].Count != 2) //guide doesnt cross bounds or hits more than 2
		//			throw new Exception(string.Format("Panel Group [{0}] Guide Curve [{1}] does not intersect 2 boundry warps", Label, Guides[nG].Label));
		//	}

		//}
		//List<GuideCross>[] xGuides;

		#endregion	
		
		delegate Panel PanelSpreader(double width);

		public int LayoutPanels()
		{
			if (Guides == null || Bounds == null)
				return -1;
			else if (Guides.Count == 0 || Bounds.Count == 0)
				return -1;

			//remove existing panels
			Clear();

			//get corner point s,uv,xyz interesections
			SetCorners();

			//get guide/boundary intersections
			SetGuides();
			m_nGuide = 0; //start on first guide curve

			//determine back and fore seams using target width
			//iterate on width to lay second seam
			//set the appropriate spreader based on cloth alignment
			PanelSpreader Seamer = null;
			switch (ClothAlignment)
			{
				case ClothOrientations.WARPS:
					Seamer  = new PanelSpreader(Panel8Warps);
					break;
				case ClothOrientations.FILLS:
					Seamer = new PanelSpreader(Panel8Fills);
					break;
			}

			if (Seamer == null)
				throw new Exception(string.Format("Panel Group [{0}] has an invalid Cloth Alignment [{1}]", Label, ClothAlignment));

			double target;
			bool atSeam = false;
			Panel p = null;
			int nNwt;
			do
			{				
				target = Width;//initialize guess width

				for ( nNwt = 0; nNwt < 50; nNwt++)
				{
					//iterate on width to lay second seam
					//try
					//{
						
					p = Seamer(target);
						atSeam = p == null;
					//}
					//catch (Exception e) { MessageBox.Show(e.Message); atSeam = true; }


					//flatten and check actual width(panel.Width) vs input width(m_Width)
					if (atSeam || BLAS.IsEqual(p.Width, Width, 1e-3))//on target width
						break;

					//iterate target width to hit input width
					//target += (Width - p.Width);//calculate new width aim-off
					double del = (p.Width - Width) / Width;
					if (Math.Abs(del) > 0.5)
						del = 0.5 * Math.Sign(del);//half-width maximum increment
					target = target * (1 - del);

					if (target <= 0)//vanishing target width, probably flattening error
						break;
					//target = Width / p.Width;
				}
				if (!atSeam && nNwt == 50)
					MessageBox.Show(string.Format("Failed to hit target width for Panel [{0}]", p.Label));
				if (!atSeam)
				{
					Add(p);

					if (Count > 1)//set internal panels to match the previous panel
						p.AlignFlatPanels(this[Count - 2]);
					else//set the first panel aligned with it's seam and offset in x
					{
						Vect3 xyp = new Vect3();
						p.Seams[0].xVec(0, ref uv, ref xyz, ref xyp);
						//p.Seams[0].xVal(1, ref uv, ref xyp);
						p.Seams[0].xVal(1, ref uv, ref xyp);
						xyp -= xyz;
						p.AlignFlatPanels(new Point2D(xyz.x+Sail.Width, xyz.y), new Vect2(xyp.x, xyp.y));
						//p.AlignFlatPanels(new Point2D(Sail.Width, Sail.Height), new Vect2(-1, 0));
					}
				}

			}
			while (!atSeam && Count < MAX_PANS);

			return atSeam ? Count : -Count;
		}

		Vect2 uv = new Vect2();
		Vect3 xyz = new Vect3();
		int m_nGuide = 0;

		PanelCorner LoCorner(int nSeg) { return Corners[nSeg % NumSegments]; }
		PanelCorner HiCorner(int nSeg) { return Corners[(nSeg + 1) % NumSegments]; }

		int PrevSeg(int nSeg) { return nSeg == 0 ? NumSegments : nSeg - 1; }
		int NextSeg(int nSeg) { return (nSeg + 1) % NumSegments; }

		//Panel WarpsPanel(double width)
		//{
		//	//panel corner points, 0,1: aft seam, 2,3: for seam
		//	//	1----2
		//	//	|	|
		//	//	0----3

		//	PanelCorner[] panCorners = new PanelCorner[4];

		//	//initialize aft seam corners from previous panel/boundary
		//	if (Count == 0)//first panel, use boundary curves
		//	{
		//		//use boundary corners as initial panel corners
		//		panCorners[0] = LoCorner(0).Clone();
		//		panCorners[1] = HiCorner(0).Clone();

		//		//swap the low corner to get 0 as the seam and 1 as the edge
		//		panCorners[0].Swap();
		//	}
		//	else//use previous panel's endseam
		//	{
		//		panCorners[0] = this[Count - 1].Corners[3];
		//		panCorners[1] = this[Count - 1].Corners[2];
		//	}

		//	//initialize fore seam corners from guidecrosses
		//	if (m_nGuide < GuideCross.Length)
		//	{
		//		panCorners[2] = GuideCross[m_nGuide][0].Clone(); //all guidecrosses have guide as 0 and edge as 1
		//		panCorners[3] = GuideCross[m_nGuide][1].Clone();
		//	}
		//	else//use terminating boundary if past our last guidecurve
		//	{
		//		panCorners[2] = LoCorner(NumSegments - 2).Clone();
		//		panCorners[3] = HiCorner(NumSegments - 2).Clone();

		//		//swap the low corner to get 0 as the seam and 1 as the edge
		//		panCorners[2].Swap();
		//	}

		//	Vect2 sPos = new Vect2();
		//	double wEnd = 0;
		//	int nNwt;
		//	for (nNwt = 0; nNwt < 50; nNwt++)
		//	{
		//		wEnd = panCorners[0].Distance(panCorners[3]) + panCorners[1].Distance(panCorners[2]);
		//		//average end widths to get "width"
		//		wEnd /= 2.0;
		//		//break if ontarget
		//		if (BLAS.is_equal(wEnd, width, 1e-5))
		//			break; //seam is good

		//		//adjust using max % method
		//		wEnd = width / wEnd;//	get target/width %

		//		//interpolate new end positions using % target
		//		panCorners[2].SlidePos(1, BLAS.interpolate(wEnd, panCorners[2].sPos[1], panCorners[1].sPos[1]));
		//		panCorners[3].SlidePos(1, BLAS.interpolate(wEnd, panCorners[3].sPos[1], panCorners[0].sPos[1]));
		//		//check here for corner wrapping and handle appropriately

		//	}
		//	if (nNwt == 50)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

		//	//check for guide-stepover
		//	if (m_nGuide < GuideCross.Length && !Utilities.IsBetween(panCorners[1].sPos[1], panCorners[2].sPos[1], GuideCross[m_nGuide][0].sPos[1]))
		//		m_nGuide++;//increment guide index if we stepped over

		//	//check to see if we've stepped past the boundary cornerpoints
		//	if (!InSegment(Bounds.IndexOf(panCorners[2].Seams[1]), panCorners[2].sPos[1]))
		//		return null;//quit when we exit the boundaries
		//	if (!InSegment(Bounds.IndexOf(panCorners[3].Seams[1]), panCorners[3].sPos[1]))
		//		return null;//quit when we exit the boundaries

		//	return MakePanel(panCorners);
		//}
		//Panel FillsPanel(double width)
		//{
		//	//panel corner points, 0,1: aft seam, 2,3: for seam
		//	//	1----2
		//	//	|	|
		//	//	0----3

		//	PanelCorner[] panCorners = new PanelCorner[4]; 

		//	PanelCorner[] gCorners = new PanelCorner[2];//forward steppin guide/seam intersections
			
		//	//initialize aft seam corners from previous panel/boundary
		//	if (Count == 0)//first panel, use boundary curves
		//	{
		//		//single guide, should always be m_nGuide=0
		//		gCorners[0] = GuideCross[m_nGuide][0]; //start with guide/bounds 0 intersection

		//		int nSeg = Bounds.IndexOf(gCorners[0].Seams[1]);//find intersecting segment

		//		//use boundary corners as initial panel corners
		//		panCorners[1] = LoCorner(nSeg).Clone();
		//		panCorners[0] = HiCorner(nSeg).Clone();

		//		//swap the low corner to get 0 as the seam and 1 as the edge
		//		panCorners[1].Swap();

		//	}
		//	else//use previous panel's endseam
		//	{
		//		try
		//		{
		//			gCorners[0] = new PanelCorner(Guides[m_nGuide], this[Count - 1].Seams[1]);//find corner of previous seam and guidecurve
		//		}
		//		catch
		//		{ return null; }//stepped out of bounds, at seam

		//		panCorners[0] = this[Count - 1].Corners[3];
		//		panCorners[1] = this[Count - 1].Corners[2];
		//	}

		//	//estimate delta-x/delta-s for width based guide-stepping
		//	double cord = GuideCross[m_nGuide][1].Distance(GuideCross[m_nGuide][0]);
		//	double span = GuideCross[m_nGuide][1].sPos[0] - GuideCross[m_nGuide][0].sPos[0];
		//	double dxds = cord / span;
		//	double ds = width / dxds;//guess delta s, 10% maxstep
		//	gCorners[1] = new PanelCorner(Guides[m_nGuide], null, new Vect2(), new Vect2(), new Vect3());
		//	int nNwt;
		//	for (nNwt = 0; nNwt < 50; nNwt++)
		//	{
		//		//project up guide to target width
		//		gCorners[1].SlidePos(0, gCorners[0].sPos[0] + ds);
		//		//Guides[m_nGuide].xVal(gCorners[1].sPos[1], ref gCorners[1].uPos, ref gCorners[1].xPos);

		//		//check "width"
		//		span = gCorners[1].Distance(gCorners[0]);
		//		if (BLAS.is_equal(span, width, 1e-5))
		//			break;//success

		//		dxds = span / ds;//recalculate dxds using current step
		//		ds = width / dxds;//recalc ds using new gradient
		//	}
		//	if (nNwt == 50)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

		//	//if (!Utilities.IsBetween(xGuides[0][0].sPos, s, xGuides[0][1].sPos))//ensure the intersection is between the guide's intersections (always should be)
		//	//	return null;

		//	//span girth on curve normal to bounds intersections
		//	//CurvePoint startpt = new CurvePoint(Guides[0], s);
		//	//set the endpoint angle
		//	Vect3 xyzGuide = new Vect3();
		//	Vect3 dxnGuide = new Vect3();
		//	Guides[m_nGuide].xVec(gCorners[1].sPos[0], ref uv, ref xyzGuide, ref dxnGuide);//get the target point and tangent vector
		//	int nEnd = 2;
		//	double s;
		//	for (int nB = 0; nB < Bounds.Count; nB++)
		//	{
		//		//nSeg %= Bounds.Count;//start at initial segment to ensure panCorners are ordered correctly
		//		xyz.Set(xyzGuide);//initialize guess
		//		s = panCorners[1].sPos[0];
		//		if (!CurveTools.AnglePoint(Bounds[nB], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
		//			continue; //no intersection with this boundary curve

		//		//set the angle and ensure it's in our bracket
		//		//if (Utilities.IsBetween(sCorners[nB][0], s, sCorners[nB][1]))
		//		if (InSegment(nB, s) && nEnd < panCorners.Length)
		//		{
		//			panCorners[nEnd++] = new PanelCorner(null, Bounds[nB], new Vect2(-1, s), new Vect2(uv), new Vect3(xyz));
		//			//ends.Add(new CurvePoint(Bounds[nB], s));
		//			//make edge seams here using nB index and seams[0] fitpoints' indices

		//		}
		//	}
		//	if (nEnd < 4)
		//		return null;//outside the group: atSeam
		//	if (nEnd != 4)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to set angle points for panel [{1}]", Label, Count));

		//	//fit the girth seam
		//	//pan.Seams[1] = new MouldCurve(PanLabel(1), Sail, ends.ToArray());
		//	return MakePanel(panCorners);
		//}

		Panel Panel8Fills(double width)
		{
			//panel segments, 0: aft seam, 1: fore seam
			//
			/*
			 *			0	0
			 *		    /      \
			 *		    1      1
			 *		    |	 |
			 *		0   |	 |  1
			 *		    2	 2
			 *		    \	 /
			 *		     3	3
			 * 
			 */


			PanelCorner[,] panCorners = new PanelCorner[2,4];

			PanelCorner[] gCorners = new PanelCorner[2];//forward steppin guide/seam intersections
			int nSeg;
			//initialize aft seam corners from previous panel/boundary
			if (Count == 0)//first panel, use boundary curves
			{
				//single guide, should always be m_nGuide=0
				double min = 1e9;
				gCorners[0] = null;
				foreach( PanelCorner c in GuideCross[m_nGuide] )
					if (c.sPos[0] < min)
					{
						gCorners[0] = c;
						min = c.sPos[0];
					}
			
				//gCorners[0] = GuideCross[m_nGuide][0]; //start with guide/bounds 0 intersection
				if (gCorners[0] == null)
					return null;

				nSeg = Bounds.IndexOf(gCorners[0].Seams[1]);//find intersecting segment

				//use boundary corners as initial panel corners
				panCorners[0,1] = HiCorner(nSeg).Clone();
				panCorners[0,2] = LoCorner(nSeg).Clone();

				//swap the low corner to get 0 as the seam and 1 as the edge
				panCorners[0,2].Swap();

			}
			else//use previous panel's endseam
			{
				try
				{
					gCorners[0] = new PanelCorner(Guides[m_nGuide], this[Count - 1].Seams[1]);//find corner of previous seam and guidecurve
				}
				catch
				{ return null; }//stepped out of bounds, at seam

				nSeg = Bounds.IndexOf(this[0].Corners[0,1].Seams[0]);

				panCorners[0,1] = this[Count - 1].Corners[1,1];
				panCorners[0,2] = this[Count - 1].Corners[1,2];
			}

			//estimate delta-x/delta-s for width based guide-stepping
			double cord = GuideCross[m_nGuide][1].Distance(GuideCross[m_nGuide][0]);
			double span = GuideCross[m_nGuide][1].sPos[0] - GuideCross[m_nGuide][0].sPos[0];
			double dxds = Math.Abs( cord / span ); //always positive since we start on lower guidecross
			double ds = width / dxds;//guess delta s, 10% maxstep
			gCorners[1] = new PanelCorner(Guides[m_nGuide], null, null, null, null);
			int nNwt;
			for (nNwt = 0; nNwt < 50; nNwt++)
			{
				//project up guide to target width
				gCorners[1].SlidePos(0, gCorners[0].sPos[0] + ds);
				//Guides[m_nGuide].xVal(gCorners[1].sPos[1], ref gCorners[1].uPos, ref gCorners[1].xPos);

				//check "width"
				span = gCorners[1].Distance(gCorners[0]);
				if (BLAS.IsEqual(span, width, 1e-5))
					break;//success

				dxds = span / ds;//recalculate dxds using current step
				ds = width / dxds;//recalc ds using new gradient
			}
			if (nNwt == 50)
				throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

			//if (!Utilities.IsBetween(xGuides[0][0].sPos, s, xGuides[0][1].sPos))//ensure the intersection is between the guide's intersections (always should be)
			//	return null;

			//span girth on curve normal to bounds intersections
			//CurvePoint startpt = new CurvePoint(Guides[0], s);
			//set the endpoint angle
			Vect3 xyzGuide = new Vect3();
			Vect3 dxnGuide = new Vect3();
			Guides[m_nGuide].xVec(gCorners[1].sPos[0], ref uv, ref xyzGuide, ref dxnGuide);//get the target point and tangent vector
			double s;
			int nTop =0, nBot = 0;
			for (int nB = 0; nB < NumSegments; nB++)
			{
				//get top and bottom point segment indices
				nTop = (nB + nSeg+1) % NumSegments;//
				nBot = nSeg - nB;
				if (nBot < 0)
					nBot += NumSegments;

				//find top corner point
				xyz.Set(xyzGuide);//initialize guess
				s = panCorners[0,1].sPos[0];
				if (panCorners[1, 1] == null && CurveTools.AnglePoint(Bounds[nTop], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
				{
					//set the angle and ensure it's in our bracket
					if (InSegment(nTop, s))
					{
						//intersection with this boundary curve
						panCorners[1, 1] = new PanelCorner(null, Bounds[nTop], new Vect2(-1, s), new Vect2(uv), new Vect3(xyz));
						if (panCorners[1, 1] == panCorners[1, 2])
						{
							panCorners[1, 1] = null;//skip repeated corner
						}
						else if (panCorners[0, 1].Seams[0] == panCorners[0, 2].Seams[0]//triangle condition: shift corner in
							&&  panCorners[1, 1].Seams[1] == panCorners[0, 1].Seams[0])
						{
							panCorners[0, 1] = panCorners[1, 1].Clone();
							panCorners[1, 0] = panCorners[0, 2].Clone();
							panCorners[1, 0].Swap();

						}
						else if (panCorners[0, 1].Seams[1] == panCorners[1, 1].Seams[1])//same seam: cross corners
						{
							panCorners[0, 0] = panCorners[1, 1].Clone();
							panCorners[1, 0] = panCorners[0, 1].Clone();
						}
						else//different seams, wrapped corner
						{
							panCorners[0, 0] = LoCorner(nTop).Clone();
							panCorners[1, 0] = LoCorner(nTop).Clone();
							if (panCorners[0, 0].Seams[0] == panCorners[0, 1].Seams[1])
								panCorners[0, 0].Swap();
							if (panCorners[1, 0].Seams[0] == panCorners[1, 1].Seams[1])
								panCorners[1, 0].Swap();
							//panCorners[0, 0].Swap();
							//panCorners[1, 0].Swap();
						}
					}
				}
				
				//set the bottom corner
				if (panCorners[1,2] == null && CurveTools.AnglePoint(Bounds[nBot], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
				{
					//set the angle and ensure it's in our bracket
					if (InSegment(nBot, s))
					{
						//intersection with this boundary curve
						panCorners[1, 2] = new PanelCorner(null, Bounds[nBot], new Vect2(-1, s), new Vect2(uv), new Vect3(xyz));
						if (panCorners[1, 2] == panCorners[1, 1])
						{
							panCorners[1, 2] = null;//skip repeated corner
						}
						else if (panCorners[0, 1].Seams[0] == panCorners[0, 2].Seams[0]//triangle condition: shift corner in
							&&  panCorners[1, 2].Seams[1] == panCorners[0, 1].Seams[0])
						{
							panCorners[0, 2] = panCorners[1, 2].Clone();//shift boundary-corner to triangle
							panCorners[1, 3] = panCorners[0, 1].Clone();//set extension point to corner
							panCorners[1, 3].Swap();
							//panCorners[0, 3] = panCorners[1, 1].Clone();//set extensionpoint to seam end
						}
						else if (panCorners[0, 2].Seams[1] == panCorners[1, 2].Seams[1])//same seam cross corners
						{
							panCorners[0, 3] = panCorners[1, 2].Clone();
							panCorners[1, 3] = panCorners[0, 2].Clone();
						}
						else//different seams, wrapped corner
						{
							panCorners[0, 3] = HiCorner(nBot).Clone();
							panCorners[1, 3] = HiCorner(nBot).Clone();
							if(panCorners[0,3].Seams[0] == panCorners[0,2].Seams[1] )
							panCorners[0, 3].Swap();
							if(panCorners[1,3].Seams[0] == panCorners[1,2].Seams[1] )
							panCorners[1, 3].Swap();
						}
					}
				}
				if (panCorners[1, 1] != null && panCorners[1, 2] != null)//both corners found
				{
					//check for triangle and set lower seam corners
					if (panCorners[0, 1] == panCorners[1, 1])
						panCorners[0, 0] = panCorners[1, 2].Clone();
					if (panCorners[0, 2] == panCorners[1, 2])
						panCorners[0, 3] = panCorners[1, 1].Clone();//this needs to use the seam as it's curve in order to be drawn/interpreted correctly
					break;
				}
			}
			if (panCorners[1, 1] == null || panCorners[1, 2] == null )
				return null;
				//throw new Exception(string.Format("Panel Group [{0}] failed to set angle points for panel [{1}]", Label, Count));

			//fit the girth seam
			//pan.Seams[1] = new MouldCurve(PanLabel(1), Sail, ends.ToArray());
			return MakePanel(panCorners);
		}

		/// <summary>
		/// warps style panel group
		/// requires boundaries and at least 1 warp curve
		/// warp curves should span entire boundary
		/// </summary>
		/// <param name="target">the target panel width</param>
		/// <returns>the panel if successful, null if overstep</returns>
		Panel Panel8Warps(double target)
		{
			//panel segments, 0: aft seam, 1: fore seam
			//
			/*
			 *			0	0
			 *		    /      \
			 *		    1      1
			 *		    |	 |
			 *		0   |	 |  1
			 *		    2	 2
			 *		    \	 /
			 *		     3	3
			 * 
			 */


			PanelCorner[,] panCorners = new PanelCorner[2, 4];

			return null;
		}

		private Panel MakePanel(PanelCorner[,] panCorners)
		{
			//first create 2 seams from the corner curvepoints
			//MouldCurve s1 = new MouldCurve(PanLabel(), Sail, new IFitPoint[] { panCorners[0,1].GetSeamPoint(1), panCorners[0,2].GetSeamPoint(1) });
			////MouldCurve s2 = new MouldCurve(PanLabel(1), Sail, new IFitPoint[] { panCorners[1,1].GetSeamPoint(1), panCorners[1,2].GetSeamPoint(1) });
			//MouldCurve s2 = new MouldCurve(PanLabel(1), Sail, new IFitPoint[] { panCorners[1,1].GetSeamPoint(1), panCorners[1,2].GetSeamPoint(1) });

			//e2.Add(panCorners[1].Seams[1], panCorners[1].sPos[1], panCorners[2].sPos[1]);

			return new Panel(this, panCorners);
		}

		internal string PanLabel() { return PanLabel(0); }
		internal string PanLabel(int n) { return Label + (Count+n).ToString("000"); }
		//private Panel MakePanel(PanelCorner[] panCorners)
		//{
		//	if (panCorners[0].Seams[1] == panCorners[2].Seams[1] || panCorners[1].Seams[1] == panCorners[3].Seams[1] )
		//	{//swap if inital seg is reversed
		//		PanelCorner pc = panCorners[0];
		//		panCorners[0] = panCorners[1];
		//		panCorners[1] = pc;
		//	}
		//	//first create 2 seams from the corner curvepoints
		//	MouldCurve s1 = new MouldCurve(PanLabel(), Sail, new IFitPoint[] { panCorners[0].GetSeamPoint(1), panCorners[1].GetSeamPoint(1) });
		//	MouldCurve s2 = new MouldCurve(PanLabel(1), Sail, new IFitPoint[] { panCorners[2].GetSeamPoint(1), panCorners[3].GetSeamPoint(1) });
			
		//	//then create endcaps from corner edgeseams
		//	EndSeam e1 = MakeEdge(panCorners, 0);
		//	//if (panCorners[0].Seams[1] == panCorners[3].Seams[1])//same seam, single-segment
		//	//	e1.Add(panCorners[0].Seams[1], panCorners[0].sPos[1], panCorners[3].sPos[1]);
		//	//else//dual segment
		//	//{
		//	//	int nSeg = Bounds.IndexOf(panCorners[0].Seams[1]);
		//	//	e1.Add(panCorners[0].Seams[1], panCorners[0].sPos[1], HiCorner(nSeg).sPos[0]);
		//	//	e1.Add(panCorners[3].Seams[1], HiCorner(nSeg).sPos[1], panCorners[3].sPos[1]);
		//	//}

		//	EndSeam e2 = MakeEdge(panCorners, 1);
		//	//e2.Add(panCorners[1].Seams[1], panCorners[1].sPos[1], panCorners[2].sPos[1]);

		//	return new Panel(s1, s2, e1, e2, panCorners);
		//}
		//private EndSeam MakeEdge(PanelCorner[] panCorners, int nEnd)
		//{
		//	int n0 = nEnd;
		//	int n3 = nEnd == 0 ? 3 : 2;
		//	EndSeam e1 = new EndSeam();
		//	if (panCorners[n0].Seams[1] == panCorners[n3].Seams[1])//same seam, single-segment
		//		e1.Add(panCorners[n0].Seams[1], panCorners[n0].sPos[1], panCorners[n3].sPos[1]);
		//	else//dual segment
		//	{
		//		int nSeg = Bounds.IndexOf(panCorners[n0].Seams[1]);
		//		e1.Add(panCorners[n0].Seams[1], panCorners[n0].sPos[1], HiCorner(nSeg).sPos[0]);
		//		e1.Add(panCorners[n3].Seams[1], HiCorner(nSeg).sPos[1], panCorners[n3].sPos[1]);
		//	}
		//	return e1;
		//}

		//Panel WarpsPanel(double width)
		//{
		//	Panel pan = new Panel();
		//	MouldCurve[] edges = new MouldCurve[2];
		//	GuideCross[] panCorners = new GuideCross[Bounds.Count];//one for each panel corner

		//	if (Count == 0)//first panel, use boundary curves
		//	{
		//		//use bracketing bounds as initial edges
		//		edges[0] = Bounds[1];
		//		edges[1] = Bounds[Bounds.Count - 1];

		//		//use first boundary segment as aft seam
		//		//pan.Seams[0] = new MouldCurve(PanLabel(), Bounds[0], sCorners[0]);
		//		pan.Seams[0] = new MouldCurve(PanLabel(), Sail,
		//			new IFitPoint[]{ 
		//				Corners[0].GetSeamPoint(1),
		//				Corners[NumSegments-1].GetSeamPoint(0)});
		//				//new CurvePoint(edges[0], sCorners[1][0]), 
		//				//new CurvePoint(edges[1], sCorners[Bounds.Count - 1][1])});
		//	}
		//	else
		//	{
		//		pan.Seams[0] = this[Count - 1].Seams[1];
		//	}
		//	//initialize fore seam as girth between guidecrosses
		//	//seams[1] = new MouldCurve(Label + (Count + 1).ToString("000"), Guides[nGuide], new Vect2(xGuides[nGuide][0].sPos, xGuides[nGuide][1].sPos));
		//	pan.Seams[1] = new MouldCurve(PanLabel(1), Sail,
		//		new IFitPoint[]{ 
		//			GuideCross[m_nGuide][0].GetSeamPoint(1),
		//			GuideCross[m_nGuide][1].GetSeamPoint(1)});
		//		//xGuides[m_nGuide][0].GetSeamPoint(), 
		//		//xGuides[m_nGuide][1].GetSeamPoint()});
		//	Vect2 sPos = new Vect2();
		//	double wEnd = 0;
		//	int nNwt;
		//	for (nNwt = 0; nNwt < 50; nNwt++)
		//	{
		//		wEnd = 0;
		//		for (int nEnd = 0; nEnd < 2; nEnd++)
		//		{
		//			//get aft seam endpoint
		//			pan.Seams[0].xVal(nEnd, ref uv, ref xyz);
		//			panCorners[nEnd * 2] = new GuideCross((double)nEnd, uv, xyz, null, 0);
		//			if (pan.Seams[0].FitPoints[nEnd * (pan.Seams[0].FitPoints.Length - 1)] is CurvePoint)
		//			{
		//				edges[nEnd] = panCorners[nEnd * 2].Seam = (pan.Seams[0].FitPoints[nEnd * (pan.Seams[0].FitPoints.Length - 1)] as CurvePoint).Curve;
		//				panCorners[nEnd * 2].sSeam = (pan.Seams[0].FitPoints[nEnd * (pan.Seams[0].FitPoints.Length - 1)] as CurvePoint).SCurve;
		//			}
		//			//get fore seam endpoint
		//			pan.Seams[1].xVal(nEnd, ref uv, ref xyz);
		//			panCorners[nEnd * 2 + 1] = new GuideCross((double)nEnd, uv, xyz, null, 0);
		//			if (pan.Seams[1].FitPoints[nEnd * (pan.Seams[1].FitPoints.Length - 1)] is CurvePoint)
		//			{
		//				edges[nEnd] = panCorners[nEnd * 2 + 1].Seam = (pan.Seams[1].FitPoints[nEnd * (pan.Seams[1].FitPoints.Length - 1)] as CurvePoint).Curve;
		//				panCorners[nEnd * 2 + 1].sSeam = (pan.Seams[1].FitPoints[nEnd * (pan.Seams[1].FitPoints.Length - 1)] as CurvePoint).SCurve;
		//			}

		//			//get distance from aft to fore endpoints
		//			wEnd += panCorners[nEnd * 2].xPos.Distance(panCorners[nEnd * 2 + 1].xPos);
		//		}
		//		//average to get "width"
		//		wEnd /= 2.0;
		//		//break if ontarget
		//		if (BLAS.is_equal(wEnd, width, 1e-5))
		//			break; //seam is good

		//		//adjust using max % method
		//		wEnd = width / wEnd;//	get target/width %

		//		//interpolate new end positions using % target
		//		sPos[0] = BLAS.interpolate(wEnd, panCorners[1].sSeam, panCorners[0].sSeam);
		//		sPos[1] = BLAS.interpolate(wEnd, panCorners[3].sSeam, panCorners[2].sSeam);

		//		pan.Seams[1] = new MouldCurve(pan.Seams[1].Label, Sail, new IFitPoint[] { new CurvePoint(edges[0], sPos[0]), new CurvePoint(edges[1], sPos[1]) });

		//	}
		//	if (nNwt == 50)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));


		//	//check to see if we've stepped past the boundary cornerpoints
		//	int nEdg = Bounds.IndexOf(edges[0]);
		//	if (!Utilities.IsBetween(sCorners[nEdg][0], sPos[0], sCorners[nEdg][1]))
		//		return null;//null to end panelling

		//	nEdg = Bounds.IndexOf(edges[1]);
		//	if (!Utilities.IsBetween(sCorners[nEdg][0], sPos[0], sCorners[nEdg][1]))
		//		return null;//null too end panelling

		//	return pan;
		//}
		//Panel FillsPanel(double width)
		//{	
		//	//single guide
		//	Vect2 sPos = new Vect2();
		//	//get guide/seam0 intersection
		//	Panel pan = new Panel();

		//	if (Count == 0)//first panel, use boundary curves
		//	{
		//		////use bracketing bounds as initial edges
		//		//edges[0] = Bounds[1];
		//		//edges[1] = Bounds[Bounds.Count - 1];

		//		//use first boundary segment as aft seam
		//		//pan.Seams[0] = new MouldCurve(PanLabel(), Bounds[0], sCorners[0]);
		//		pan.Seams[0] = new MouldCurve(PanLabel(), Sail,
		//			new IFitPoint[]{ 
		//				new CurvePoint( Bounds[1], sCorners[1][0]), 
		//				new CurvePoint(Bounds[Bounds.Count - 1], sCorners[Bounds.Count - 1][1])});
		//	}
		//	else
		//	{
		//		pan.Seams[0] = this[Count - 1].Seams[1];
		//	} 
			
		//	if (!CurveTools.CrossPoint(Guides[0], pan.Seams[0], ref uv, ref xyz, ref sPos, 20))
		//		return null;

		//	GuideCross start = new GuideCross(sPos[0], uv, xyz, pan.Seams[0], sPos[1]);
		//	//estimate delta-x/delta-s for width based guide-stepping
		//	double cord = xGuides[0][1].xPos.Distance(xGuides[0][0].xPos);
		//	double span = xGuides[0][1].sPos - xGuides[0][0].sPos;
		//	double dxds = cord / span;
		//	double ds = width / dxds;//guess delta s
		//	int nNwt;
		//	for( nNwt = 0; nNwt < 50; nNwt++)
		//	{
		//		//project up guide to target width
		//		Guides[0].xVal(start.sPos + ds, ref uv, ref xyz);

		//		//check "width"
		//		span = xyz.Distance(start.xPos);
		//		if (BLAS.is_equal(span, width, 1e-5))
		//			break;//success

		//		dxds = span / ds;//recalculate dxds using current step
		//		ds = width / dxds;//recalc ds using new gradient
		//	}
		//	if (nNwt == 50)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

		//	double s = start.sPos + ds;

		//	//if (!Utilities.IsBetween(xGuides[0][0].sPos, s, xGuides[0][1].sPos))//ensure the intersection is between the guide's intersections (always should be)
		//	//	return null;

		//	//span girth on curve normal to bounds intersections
		//	//CurvePoint startpt = new CurvePoint(Guides[0], s);
		//	//set the endpoint angle
		//	Vect3 xyzGuide = new Vect3();
		//	Vect3 dxnGuide = new Vect3();
		//	Guides[0].xVec(s, ref uv, ref xyzGuide, ref dxnGuide);//get the target point and tangent vector
		//	List<CurvePoint> ends = new List<CurvePoint>(2);
		//	for (int nB = 0; nB < Bounds.Count; nB++)
		//	{
		//		xyz.Set(xyzGuide);//initialize guess
		//		s = start.sPos + ds;
		//		if (!CurveTools.AnglePoint(Bounds[nB], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
		//			continue; //no intersection with this boundary curve

		//		//set the angle and ensure it's in our bracket
		//		if (Utilities.IsBetween(sCorners[nB][0], s, sCorners[nB][1]))
		//		{
		//			ends.Add(new CurvePoint(Bounds[nB], s));
		//			//make edge seams here using nB index and seams[0] fitpoints' indices

		//		}
		//	}
		//	if (ends.Count < 2)
		//		return null;//outside the group: atSeam
		//	if (ends.Count != 2)
		//		throw new Exception(string.Format("Panel Group [{0}] failed to set angle points for panel [{1}]", Label, Count));

		//	//fit the girth seam
		//	pan.Seams[1] = new MouldCurve(PanLabel(1), Sail, ends.ToArray());
		//	return pan;
		//}


		//private List<SeamSegment>[] MakeEndSegments(MouldCurve[] seams)
		//{
		//	CurvePoint[] s = new CurvePoint[2];
		//	s[0] = seams[0].FitPoints[0] as CurvePoint;
		//	s[1] = seams[0].FitPoints.Last() as CurvePoint;
		//	CurvePoint[] e = new CurvePoint[2];
		//	e[0] = seams[1].FitPoints[0] as CurvePoint;
		//	e[1] = seams[1].FitPoints.Last() as CurvePoint;

		//	if (s[0] == null || s[1] == null || e[0] == null || e[1] == null)
		//		return null;


		//	//if (s[0].UV.Distance(e[1].UV) < s[0].UV.Distance(e[0].UV))//seams face opposing directions
		//	//{
		//	//	CurvePoint swap = e[1];
		//	//	e[1] = e[0];
		//	//	e[0] = swap;
		//	//}

		//	List<SeamSegment>[] ends = new List<SeamSegment>[2];
		//	for (int nEnd = 0; nEnd < 2; nEnd++)
		//	{
		//		ends[nEnd] = new List<SeamSegment>();
		//		if (s[nEnd].Curve == e[nEnd].Curve)//same curve, single-segment
		//		{
		//			ends[nEnd].Add(new SeamSegment(s[nEnd].Curve, new Vect2(s[nEnd].SCurve, e[nEnd].SCurve)));
		//		}
		//		else//different curves, multi-segment
		//		{
		//			int nB0 = m_bounds.IndexOf(s[nEnd].Curve);
		//			int nB1 = m_bounds.IndexOf(e[nEnd].Curve);
		//			//add start segment
		//			ends[nEnd].Add(new SeamSegment(m_bounds[nB0], new Vect2(sCorners[nB0][0], s[nEnd].SCurve)));
		//			for (int nB = nB0 + 1; nB < nB1; nB++)
		//			{
		//				//add internal segments
		//				ends[nEnd].Add(new SeamSegment(m_bounds[nB], sCorners[nB]));
		//			}
		//			//add end segment
		//			ends[nEnd].Add(new SeamSegment(m_bounds[nB1], new Vect2(sCorners[nB1][0], e[nEnd].SCurve)));
		//		}
		//	}
		//	return ends;
		//}


		#region IGroup Members

		public Sail Sail
		{
			get
			{
				return m_sail;
			}
			set
			{
				m_sail = value;
			}
		}

		public IRebuild FindItem(string label)
		{
			if (label == m_label)
				return this;

			foreach (Panel p in this)
			{
				if (p.Label == label)
					return p;
			}
			return null;
		}

		public bool ContainsItem(IRebuild item)
		{
			if (!(item is Panel))
				return false;

			return this.Contains(item as Panel);
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			int i = -1;
			if (tag == this)
				return true;
			if (tag is Panel)
				i = this.IndexOf(tag as Panel);

			if (i >= 0)
				rets.AddRange(this.Take(i));
			else
				rets.AddRange(this);

			return i != -1;//true if found
		}

		public bool FindParent<T>(IRebuild item, out T parent) where T : class, IGroup
		{
			if (ContainsItem(item))
			{
				parent = this as T;
				return parent != null;
			}
			parent = null;
			return false;
		}

		#endregion

		#region IRebuild Members

		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}
		public string Layer
		{
			get { return "Panels"; }
		}

		bool m_locked = false;
		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		//public List<string> WriteScript()
		//{
		//	List<string> script = new List<string>();
		//	script.Add(GetType().Name + ": " + Label);
		//	//script.Add("\tTargetDPI: ");
		//	script.Add("\t" + m_Width.ToScriptString());
		//	script.Add("\tGuides: ");
		//	foreach (MouldCurve w in m_guides)
		//		script.Add("\t\t" + w.Label);
		//	script.Add("\tBounds: ");
		//	foreach (MouldCurve w in m_bounds)
		//		script.Add("\t\t" + w.Label);

		//	script.Add("\tClothAlignment: " + ClothAlignment.ToString());

		//	return script;
		//}

		//public bool ReadScript(Sail sail, IList<string> txt)
		//{
		//	if (txt == null || txt.Count == 0)
		//		return false;
		//	Label = ScriptTools.ReadLabel(txt[0]);
		//	string[] splits;// = txt[0].Split(':');
		//	//Label = "";
		//	//if (splits.Length > 0)//extract label
		//	//	Label = splits[1];
		//	//if (splits.Length > 1)//incase label contains ":"
		//	//	for (int i = 2; i < splits.Length; i++)
		//	//		Label += ":" + splits[i];
		//	//Label = Label.Trim();

		//	for (int nLine = 1; nLine < txt.Count; )
		//	{
		//		IList<string> lines = ScriptTools.Block(ref nLine, txt);
		//		splits = lines[0].Split(':');
		//		if (splits.Length > 0)
		//		{
		//			if (splits[0].ToLower().Contains("bounds"))
		//			{
		//				for (int i = 1; i < lines.Count; i++)
		//					m_bounds.Add(sail.FindCurve(lines[i].Trim()));

		//			}
		//			else if (splits[0].ToLower().Contains("guides"))
		//			{
		//				for (int i = 1; i < lines.Count; i++)
		//					m_guides.Add(sail.FindCurve(lines[i].Trim()));

		//			}
		//			else if (splits[0].ToLower().Contains("clothalignment"))
		//				ClothAlignment = (ClothOrientations)Enum.Parse(typeof(ClothOrientations), splits[1].Trim());
		//			else if (splits[0].ToLower().Contains("panelwidth"))
		//				m_Width = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'));

		//		}
		//	}

		//	Update(sail);

		//	return true;
		//}

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);
			m_node.ToolTipText = GetToolTipData();
			m_node.Nodes.Clear();
			foreach (Panel p in this)
				m_node.Nodes.Add(p.WriteNode());
			return m_node;
		}

		private string GetToolTipData()
		{
			return GetType().Name;
		}

		public List<Entity> CreateEntities()
		{
			List<Entity> ents = new List<Entity>();
			foreach (Panel p in this)
				ents.AddRange(p.CreateEntities());

			ents.ForEach(ent => { ent.EntityData = this; });

			if (Corners != null)
			{
				List<devDept.Geometry.Point3D> pts = new List<devDept.Geometry.Point3D>(NumSegments);
				foreach (PanelCorner c in Corners)
				{
					pts.Add(Utilities.Vect3ToPoint3D(c.xPos));
				}
				PointCloud pc = new PointCloud(pts);
				pc.EntityData = this;
				pc.ColorMethod = colorMethodType.byLayer;
				pc.LineWeight = 10;
				pc.LineWeightMethod = colorMethodType.byEntity;
	
				ents.Add(pc);
			}
			return ents;
		}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get
			{
				if (Count == 0)
					return null;
				List<devDept.Eyeshot.Labels.Label> ret = new List<devDept.Eyeshot.Labels.Label>();
				this.ForEach(cur => { ret.AddRange(cur.EntityLabel); });
				return ret;
			}
		}

		public void GetChildren(List<IRebuild> updated)
		{
			if (updated != null && Affected(updated))
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			Guides.ForEach(g =>
			{
				if (g is IRebuild)
					parents.Add(g as IRebuild);
			});

			Bounds.ForEach(g =>
			{
				if (g is IRebuild)
					parents.Add(g as IRebuild);
			});
			//if (Guides != null)
			//	parents.AddRange(Guides);

			//if (Bounds.Count > 0)
			//	parents.AddRange(Bounds);

			//	parents.Add(TargetDenierEqu);
			PanelWidth.GetParents(s, parents);

		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = connected == null;
			if (!bupdate)
			{
				foreach (MouldCurve warp in Guides)
					bupdate |= connected.Contains(warp);
				foreach (MouldCurve warp in Bounds)
					bupdate |= connected.Contains(warp);
				bupdate |= PanelWidth == null ? false : PanelWidth.Affected(connected);
			}
			return bupdate;
		}

		public bool Update(Sail s)
		{
			bool ret = true;
			ret &= !double.IsNaN(PanelWidth.Evaluate(s));
			if (ret)
				ret &= (LayoutPanels() > 0);
			return ret;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		public XmlNode WriteXScript(XmlDocument doc)
		{
			XmlNode node = NsXml.MakeNode(doc, this);
			node.AppendChild(m_Width.WriteXScript(doc));
			NsXml.AddAttribute(node, "ClothAlignment", ClothAlignment.ToString());

			NsXml.AddAttribute(node, "Bounds", m_bounds);
			NsXml.AddAttribute(node, "Guides", m_guides);

			return node;
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			Label = NsXml.ReadLabel(node);
			m_sail = sail;
			m_Width.ReadXScript(sail, node.FirstChild);

			ClothAlignment = (ClothOrientations)Enum.Parse(typeof(ClothOrientations), NsXml.ReadString(node, "ClothAlignment"));

			string[] curs = NsXml.ReadStrings(node, "Bounds");
			foreach (string s in curs)
				m_bounds.Add(sail.FindCurve(s));

			curs = NsXml.ReadStrings(node, "Guides");
			foreach (string s in curs)
				m_guides.Add(sail.FindCurve(s));
			Update(sail);
		}

		#endregion

		#region TreeDragging Members

		public bool CanInsert(Type item)
		{
			return false;
		}

		public void Insert(IRebuild item, IRebuild target)
		{
			throw new NotImplementedException("Cannot Insert into PanelGroup");
			//int nTar = IndexOf(target as Panel);
			//int nIrb = IndexOf(item as Panel);
			//if (nIrb >= 0)//item is already in this group: reorder
			//	Remove(item);
			//Insert(nTar, item as Panel);
		}

		public bool Remove(IRebuild item)
		{
			throw new NotImplementedException("Cannot Remove from PanelGroup");
			//Remove(item as Panel);
		}

		#endregion

		#region Tree Flattening Members

		public void FlatLayout(List<IRebuild> flat)
		{
			//no sub-items to add
			//ForEach(cur => flat.Add(cur));
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Label);
		}
	}

	//struct GuideCross
	//{
	//	public GuideCross(double s, Vect2 u, Vect3 x, MouldCurve seam, double sMC)
	//	{
	//		sPos = s; 
	//		uPos = u == null ? new Vect2() : new Vect2(u); 
	//		xPos = x == null ? new Vect3() : new Vect3(x); 
	//		Seam = seam; 
	//		sSeam = sMC;
	//	}
	//	//public GuideCross()
	//	//{
	//	//	sPos = new double[2];//guide curve s-positions
	//	//	uPos = new Vect2[2];//cross uv
	//	//	xPos = new Vect3[2];//cross xyz
	//	//	Seams = new MouldCurve[2];//cross curves
	//	//	sSeams = new double[2];//cross curves' s-pos
	//	//}

	//	public double sPos;// = new double[2];//guide curve s-positions
	//	public Vect2 uPos;// = new Vect2[2];//cross uv
	//	public Vect3 xPos;// = new Vect3[2];//cross xyz
	//	public MouldCurve Seam;// = new MouldCurve[2];//cross curves
	//	public double sSeam;// = new double[2];//cross curves' s-pos

	//	public CurvePoint GetSeamPoint()
	//	{
	//		return new CurvePoint(Seam, sSeam);
	//	}
	//}
}
