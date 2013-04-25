using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace Warps
{
	public delegate void YarnSpreader();

	public class YarnGroup : List<YarnCurve>, IGroup
	{
		// default constructor required for Utilities.CreateInstance()
		public YarnGroup() { m_guide = new GuideComb(); m_Warps = new List<MouldCurve>(); sPos = new List<double>() { 0.25, 0.5, 0.75 }; }

		public YarnGroup(string label, Sail s, double yarnDenier)
		{
			Label = label;
			Sail = s;
			YarnDenier = new Equation("yarnDenier", yarnDenier.ToString(), s);// = yarnDenier; // individual yarn denier (input)	
		}

		string m_label;
		Sail m_sail;

		Equation m_targetDenierEqu = new Equation("td", 0);

		public Equation TargetDenier
		{
			get { return m_targetDenierEqu; }
			set { m_targetDenierEqu = value; m_targetDenierEqu.Label = "td"; }
		}

		Equation m_yarnDenierEqu = new Equation("yd", 0);

		public Equation YarnDenier
		{
			get { return m_yarnDenierEqu; }
			set { m_yarnDenierEqu = value; m_yarnDenierEqu.Label = "yd"; }
		}

		double m_Scale = 1;

		List<MouldCurve> m_Warps;

		public List<MouldCurve> Warps
		{
			get { return m_Warps; }
			set { m_Warps = value; }
		}

		GuideComb m_guide;

		public GuideComb Guide
		{
			get { return m_guide; }
			set { m_guide = value; }
		}

		DensityComb[] m_Combs;// = new DensityComb[spos.Length];

		List<double> m_densityCurveLocations = new List<double>();

		public List<double> sPos
		{
			get { return m_densityCurveLocations; }
			set { m_densityCurveLocations = value; }
		}

		//public int GenerateYarns(List<MouldCurve> warps, GuideComb guide)
		//{
		//	if (warps == null)
		//		warps = m_Warps;
		//	if (guide == null)
		//		guide = m_Guide;
		//	if (warps == null || warps.Count == 0 || guide == null)
		//		return -1;

		//	//delete existing yarns
		//	this.Clear();
		//	m_Warps = warps;
		//	m_Guide = guide;

		//	const int CNT = 100;
		//	//generate 100 yarns using the prescribed warps
		//	for( int i=0; i < CNT; i++ )
		//	{
		//		double p = BLAS.interpolant(i, CNT);
		//		Add(new YarnCurve(p, m_Warps[0], m_Warps[1]));
		//	}

		//	return Count;
		//}

		#region GenerateYarns

		internal double EvalSpacing(double sGuide)
		{
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			double h = 0;
			m_guide.hVal(sGuide, ref uv, ref h);
			h = Math.Max(h, 1e-5);//h must be positive;
			return Math.Max(0.0001, m_Scale * YarnDenier.Result * 0.0254 / TargetDenier.Result / h);//enforce 1mm minimum spacing
		}
		internal double InverseSpacing(double h)
		{
			return Math.Max(0.0001, m_Scale * YarnDenier.Result * 0.0254 / TargetDenier.Result / h);//enforce 1mm minimum spacing
		}

		/// <summary>
		/// creates yarns between the given warps with spacing controlled by a guide curve and target dpi
		/// </summary>
		/// <param name="warps">a list of warps to spread</param>
		/// <param name="guide">the guide comb that controls the spacing, this must intersect every warp</param>
		/// <returns>the number of yarns created, -1 if incorrect parameters, -Count if unconverged</returns>
		public int LayoutYarns(List<MouldCurve> warps, GuideComb guide, double targetDen)
		{
			return LayoutYarns(warps, guide, targetDen, SpreadYarnsGlobally);
		}

		public int LayoutYarns(List<MouldCurve> warps, GuideComb guide, double targetDen, YarnSpreader SpreadYar)
		{
			if (warps == null)
				warps = m_Warps;
			if (guide == null)
				guide = m_guide;
			if (targetDen == 0)
			{
				if (TargetDenier == null)
					TargetDenier = new Equation("td", targetDen.ToString(), Sail);
			}
			else
			{
				if (TargetDenier == null)
					TargetDenier = new Equation("td", targetDen.ToString(), Sail);

				TargetDenier.EquationText = targetDen.ToString();
			}

			TargetDenier.Evaluate();
			YarnDenier.Evaluate();
			if (warps == null || warps.Count == 0 || guide == null || TargetDenier.Result == 0)
				return -1;

			//store new warps etc
			m_Warps = warps;
			m_guide = guide;
			//TargetDenier = targetDen;
			//double[] spos = new double[] { .25, .5, .75 };
			int nNwt;
			for (nNwt = 0; nNwt < 100; nNwt++)
			{
				SpreadYar();

				double dpi = CheckDpi(sPos.ToArray());
				if (BLAS.is_equal(dpi, TargetDenier.Result, 200))
					break;
				m_Scale *= dpi / TargetDenier.Result;
			}

			return nNwt < 100 ? Count : -Count;
		}

		private double CheckDpi(double[] spos)
		{
			m_Combs = new DensityComb[spos.Length];
			int i = 0;
			double d = 0;
			foreach (double s in spos)
			{
				m_Combs[i] = new DensityComb(this, s);
				d += m_Combs[i].DPI;
				i++;
			}
			d /= m_Combs.Length;
			return d;
		}

		public void SpreadYarnsAcrossWarps()
		{
			Clear();//clear existing yarns
			//vars
			Vect2 u = new Vect2(), uYar = new Vect2();
			Vect3 x = new Vect3(), xYar = new Vect3();
			Vect2 s = new Vect2(); // intersection positions 0:m_Guide, 1:this[nYar] 
			YarnCurve cur = null;
			double hTarget;
			double hYar;
			double sYar;

			Vect3 dxu = new Vect3(), dxv = new Vect3();
			Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2() };
			Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3() };

			//determine warp pPos crossovers

			//add initial yarn on starting warp
			//int nYar = 0;
			Add(new YarnCurve(0, m_Warps[0], m_Warps[1]));
			for (int nWrp = 1; nWrp < m_Warps.Count; nWrp++)
			{
				while (this.Last().m_p < 1)
				{
					//intersection of previous yarn and guide curve
					m_guide.CrossPoint(this.Last(), ref u, ref x, ref s, 20);
					//determine target spacing from dpi equation
					hTarget = EvalSpacing(s[0]);
					//create a yarn with an initial spacing
					cur = new YarnCurve(this.Last().m_p + .05, m_Warps[nWrp - 1], m_Warps[nWrp]);
					Add(cur);
					//xYar.Set(x);
					sYar = s[1];
					hYar = 0;

					for (int nNwt = 0; nNwt < 100; nNwt++)
					{
						xYar.Set(x);
						bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-3, true);

						if (Math.Abs(hTarget - hYar) < 1e-3)
						{
							break;
						}
						double dhdp = hYar / (this[Count - 1].m_p - this[Count - 2].m_p);
						double dp = hTarget - hYar;
						dp /= dhdp;

						if (dp > .1) dp *= .1 / dp;

						//increment the curve's p value
						cur.m_p += dp;
					}
				}
				//overstepped yarn
				if (nWrp < m_Warps.Count - 1)//shift yarn into next warp bracket
				{
					cur.m_p = 0;//start on the next warp
					cur.m_Warps[0] = m_Warps[nWrp];
					cur.m_Warps[1] = m_Warps[nWrp + 1];
				}
				else //land on last warp
				{
					//this.Remove(cur);
					cur.m_p = 1;
				}
			}
		}

		public void SmoothYarnsAcrossWarps()
		{
			Clear();//clear existing yarns
			//vars
			Vect2 u = new Vect2(), uYar = new Vect2();
			Vect3 x = new Vect3(), xYar = new Vect3();
			Vect2 s = new Vect2(); // intersection positions 0:m_Guide, 1:this[nYar] 
			YarnCurve cur = null;
			double hTarget;
			double hYar;
			double sYar;

			//Vect3 dxu = new Vect3(), dxv = new Vect3();

			//yarn points for warp stepping
			Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2(), new Vect2() };
			Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3(), new Vect3() };
			//warp points for warp stepping
			Vect2[] uWars = new Vect2[] { new Vect2(), new Vect2(), new Vect2() };
			Vect3[] xWars = new Vect3[] { new Vect3(), new Vect3(), new Vect3() };

			//determine warp pPos crossovers

			//add initial yarn on starting warp
			//int nYar = 0;
			Add(new YarnCurve(0, m_Warps[0], m_Warps[1]));
			int nWrp = 1;
			//for (int nWrp = 1; nWrp < m_Warps.Count; nWrp++)
			//{
			while (this.Last().m_p < 1)
			{
				//intersection of previous yarn and guide curve
				m_guide.CrossPoint(this.Last(), ref u, ref x, ref s, 20);
				//determine target spacing from dpi equation
				hTarget = EvalSpacing(s[0]);
				//create a yarn with an initial spacing
				cur = new YarnCurve(this.Last().m_p + .05, m_Warps[nWrp - 1], m_Warps[nWrp]);
				Add(cur);
				//xYar.Set(x);
				sYar = s[1];
				hYar = 0;

				for (int nNwt = 0; nNwt < 100; nNwt++)
				{
					xYar.Set(x);
					bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-3, true);
					if (!res)
					{
						cur.xVal(sYar, ref uYar, ref xYar);
						hYar = xYar.Distance(x);
					}
					if (Math.Abs(hTarget - hYar) < 1e-3)
					{
						cur.m_h = hYar;//record height
						break;
					}
					if (cur.m_p > 1 || cur.m_Warps[0] != this[Count - 2].m_Warps[0])//overstepped or warp-mismatch
					{
						//overstepped warp: use 3 warp stepping
						if (nWrp < m_Warps.Count - 1)
						{
							//set bottom  middle and top warp values
							m_Warps[nWrp - 1].xVal(s[1], ref uWars[0], ref xWars[0]);
							m_Warps[nWrp].xVal(s[1], ref uWars[1], ref xWars[1]);
							m_Warps[nWrp + 1].xVal(s[1], ref uWars[2], ref xWars[2]);
							//set previous and current yarn values
							this[Count - 2].xVal(s[1], ref uYars[0], ref xYars[0]);
							this[Count - 1].xVal(s[1], ref uYars[1], ref xYars[1]);
							//calculate total P span across 3 warps
							double AB = uWars[1].Distance(uYars[0]);
							double DelP = (AB + uWars[1].Distance(uYars[1])) / (uWars[0].Distance(uWars[1]) + uWars[1].Distance(uWars[2]));
							//determine target total P span
							double pTar = DelP * hTarget / hYar;
							//target u distance
							double uTar = pTar * (uWars[0].Distance(uWars[1]) + uWars[1].Distance(uWars[2]));
							if (uTar < AB)//target point is in lower bracket
							{
								//delta p from previous yarn
								double dp = uTar / uWars[0].Distance(uWars[1]);//percentage of warp-span
								cur.m_p = this[Count - 2].m_p + dp;
								//set lower-bracket warps
								cur.m_Warps[0] = m_Warps[nWrp - 1];
								cur.m_Warps[1] = m_Warps[nWrp];

							}
							else//target point is in upper bracket
							{
								uTar -= AB; //subtract off lower-bracket u-span contribution
								double dp = uTar / uWars[1].Distance(uWars[2]);//percentage of warp-span
								//set upper-bracket p-value
								cur.m_p = dp;
								//set upper-bracket warps
								cur.m_Warps[0] = m_Warps[nWrp];
								cur.m_Warps[1] = m_Warps[nWrp + 1];
							}
						}
						else //land on last warp
						{
							//either move this yarn down, or slide the previous one up
							double d1 = cur.m_p - 1;
							double d2 = 1 - this[Count - 2].m_p;
							if (d1 > d2) //farther from the warp than the last yarn
							{
								this.Remove(cur);
								this[Count - 2].m_p = 1;
							}
							else //closer than the last cur
								cur.m_p = 1;
							break;
						}
					}
					else
					{
						double dhdp = hYar / (this[Count - 1].m_p - this[Count - 2].m_p);
						double dp = hTarget - hYar;
						dp /= dhdp;

						if (dp > .1) dp *= .1 / dp;

						//increment the curve's p value
						cur.m_p += dp;
					}
				}
				nWrp = m_Warps.IndexOf(cur.m_Warps[1]);//ensure warp index is correct with bracket shifting
			}

			////overstepped yarn
			//if (nWrp < m_Warps.Count - 1)//shift yarn into next warp bracket
			//{
			//	cur.m_p = 0;//start on the next warp
			//	cur.m_Warps[0] = m_Warps[nWrp];
			//	cur.m_Warps[1] = m_Warps[nWrp + 1];
			//}
			//else //land on last warp
			//{
			//	//this.Remove(cur);
			//	cur.m_p = 1;
			//}
		}

		public double GlobalToBracket(double pG, List<double> pWarps, ref int nWrp)
		{
			if (pG == 1)//end condition
			{
				nWrp = pWarps.Count - 1;
				return 1;
			}
			nWrp = pWarps.Count;
			for (nWrp = 1; nWrp < pWarps.Count; nWrp++)
			{
				if (pWarps[nWrp - 1] <= pG && pG < pWarps[nWrp])//found bracket
					break;
			}
			if (nWrp < pWarps.Count)//successful find
			{
				double pB = (pG - pWarps[nWrp - 1]) / (pWarps[nWrp] - pWarps[nWrp - 1]);//convert to bracket parameter
				return pB;
			}
			return -1;
		}
		public double BracketToGlobal(YarnCurve cur, List<double> pWarps)
		{
			int nWrp = m_Warps.IndexOf(cur.m_Warps[1]);
			return BracketToGlobal(cur.m_p, pWarps, nWrp);

		}
		public double BracketToGlobal(double pB, List<double> pWarps, int nWrp)
		{
			return pB * (pWarps[nWrp] - pWarps[nWrp - 1]) + pWarps[nWrp - 1];

		}
		public void SpreadYarnsGlobally()
		{
			Clear();//clear existing yarns
			//vars
			Vect2 u = new Vect2(), uYar = new Vect2();
			Vect3 x = new Vect3(), xYar = new Vect3();
			Vect2 s = new Vect2(); // intersection positions 0:m_Guide, 1:this[nYar] 
			YarnCurve cur = null;
			double hTarget;
			double hYar;
			double sYar;

			//Vect3 dxu = new Vect3(), dxv = new Vect3();

			//yarn points for warp stepping
			Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2() };
			Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3() };
			//warp points for warp stepping
			Vect2[] uWars = new Vect2[] { new Vect2(), new Vect2(), new Vect2() };
			Vect3[] xWars = new Vect3[] { new Vect3(), new Vect3(), new Vect3() };

			//determine warp pPos crossovers from guide/warp0 starting point
			m_guide.CrossPoint(m_Warps[0], ref u, ref x, ref s);
			List<double> pWarps = new List<double>(m_Warps.Count);
			pWarps.Add(0);
			int i = 0;
			foreach (IMouldCurve wrp in m_Warps)
			{
				wrp.uVal(s[1], ref uYars[0]);
				if (i != 0)
					pWarps.Add(uYars[0].Distance(uYars[1]) + pWarps[i - 1]);
				uYars[1].Set(uYars[0]);
				i++;
			}
			for (i = 0; i < pWarps.Count; i++)
				pWarps[i] /= pWarps.Last();

			//add initial yarn on starting warp
			//int nYar = 0;
			int nWrp = 1;
			double dHdP = 0, dP;
			double P = 0, p = 0;
			Add(new YarnCurve(p, m_Warps[0], m_Warps[1]));
			while (P < 1)
			{
				//intersection of previous yarn and guide curve
				m_guide.CrossPoint(this.Last(), ref u, ref x, ref s, 20);
				//determine target spacing from dpi equation
				hTarget = EvalSpacing(s[0]);
				dP = .1;
				if (dHdP != 0)//attempt better starting guess
					dP = hTarget / dHdP;
				dP = Math.Min(dP, .1); //enforce maxmimum inital step

				P += dP;
				P = Math.Min(1, P);
				p = GlobalToBracket(P, pWarps, ref nWrp);

				//create a yarn with an initial spacing
				cur = new YarnCurve(p, m_Warps[nWrp - 1], m_Warps[nWrp]);
				Add(cur);
				//xYar.Set(x);
				sYar = s[1];
				hYar = 0;

				for (int nNwt = 0; nNwt < 100; nNwt++)
				{
					xYar.Set(x);
					bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-5, true);
					if (!res)//default to p-distance if failed
					{
						cur.xVal(sYar, ref uYar, ref xYar);
						hYar = xYar.Distance(x);
					}

					if (Math.Abs(hTarget - hYar) < 1e-5)
					{
						cur.m_h = hYar;//record height
						break;
					}
					else
					{
						//calc global-P derivative
						dHdP = hYar / (BracketToGlobal(this[Count - 1], pWarps) - BracketToGlobal(this[Count - 2], pWarps));
						dP = hTarget - hYar;
						dP /= dHdP;
						// determine delta P and enforce max step
						if (dP > .1) dP *= .1 / dP;

						//increment the current P value
						P += dP;
						//ensure inbounds
						P = Math.Min(1, P);
						//set the yarn's bracket p value and warps
						cur.m_p = GlobalToBracket(P, pWarps, ref nWrp);
						cur.m_Warps[0] = m_Warps[nWrp - 1];
						cur.m_Warps[1] = m_Warps[nWrp];
					}
				}
			}
		}

		#region Old Yarn Spreading Methods
		//public void SpreadYarns()
		//{
		//	Clear();//clear existing yarns
		//	//vars
		//	Vect2 u = new Vect2(), uYar = new Vect2();
		//	Vect3 x = new Vect3(), xYar = new Vect3();
		//	Vect2 s = new Vect2(); // intersection positions 0:m_Guide, 1:this[nYar] 
		//	YarnCurve cur;
		//	double hTarget;
		//	double hYar;
		//	double sYar;

		//	Vect3 dxu = new Vect3(), dxv = new Vect3();
		//	Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2() };
		//	Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3() };

		//	//add initial yarn on starting warp
		//	//int nYar = 0;
		//	Add(new YarnCurve(0, m_Warps[0], m_Warps[1]));
		//	while (this.Last().m_p < 1)
		//	{
		//		//intersection of previous yarn and guide curve
		//		m_guide.CrossPoint(this.Last(), ref u, ref x, ref s, 10);
		//		//determine target spacing from dpi equation
		//		hTarget = EvalSpacing(s[0]);
		//		//create a yarn with an initial spacing
		//		cur = new YarnCurve(this.Last().m_p + .05, m_Warps[0], m_Warps[1]);
		//		Add(cur);
		//		//xYar.Set(x);
		//		sYar = s[1];
		//		hYar = 0;

		//		for (int nNwt = 0; nNwt < 100; nNwt++)
		//		{

		//			//for (int i = 0; i < 2; i++)
		//			//	this[Count-(i+1)].xVal(s[1], ref uYars[i], ref xYars[i]);

		//			//Sail.Mould.xVec(u, ref x, ref dxu, ref dxv);

		//			//Vect2 du = uYars[0] - uYars[1];
		//			//Vect3 dxp = new Vect3();
		//			//for (int i = 0; i < 3; i++)
		//			//{
		//			//	dxp[i] = dxu[i] * du[0] + dxv[i] * du[1];
		//			//}
		//			xYar.Set(x);
		//			bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-3, true);

		//			if (Math.Abs(hTarget - hYar) < 1e-3)
		//			{
		//				break;
		//			}
		//			//Vect3 H = xYar - x;
		//			double dhdp = hYar / (this[Count - 1].m_p - this[Count - 2].m_p);

		//			double dp = hTarget - hYar;
		//			dp /= dhdp;
		//			if (dp > .1) dp *= .1 / dp;
		//			//increment the curve's p value
		//			cur.m_p += dp;
		//		}
		//	}
		//}

		//public void SpreadYarnsAlongGuide()
		//{
		//	Clear();//clear existing yarns
		//	//vars
		//	Vect2 u = new Vect2(), uYar = new Vect2();
		//	Vect3 x = new Vect3(), xYar = new Vect3();
		//	Vect2 s = new Vect2(); // intersection positions 0:m_Guide, 1:this[nYar] 
		//	YarnCurve cur = null;
		//	double hTarget;
		//	double hYar;
		//	double sYar;

		//	Vect3 dxu = new Vect3(), dxv = new Vect3();
		//	Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2() };
		//	Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3() };

		//	//add initial yarn on starting warp
		//	//int nYar = 0;
		//	Add(new YarnCurve(0, m_Warps[0], m_Warps[1]));
		//	for (int nWrp = 1; nWrp < m_Warps.Count; nWrp++)
		//	{
		//		while (this.Last().m_p < 1)
		//		{
		//			//intersection of previous yarn and guide curve
		//			m_guide.CrossPoint(this.Last(), ref u, ref x, ref s, 20);
		//			//determine target spacing from dpi equation
		//			hTarget = EvalSpacing(s[0]);
		//			//create a yarn with an initial spacing
		//			cur = new YarnCurve(this.Last().m_p + .05, m_Warps[nWrp - 1], m_Warps[nWrp]);
		//			Add(cur);
		//			//xYar.Set(x);
		//			sYar = s[1];
		//			hYar = 0;

		//			for (int nNwt = 0; nNwt < 100; nNwt++)
		//			{
		//				xYar.Set(x);
		//				//bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-3, true);
		//				bool res = m_guide.CrossPoint(cur, ref uYar, ref xYar, ref s, 10);
		//				hYar = (xYar - x).Magnitude;
		//				if (Math.Abs(hTarget - hYar) < 1e-3)
		//				{
		//					break;
		//				}
		//				double dhdp = hYar / (this[Count - 1].m_p - this[Count - 2].m_p);
		//				double dp = hTarget - hYar;
		//				dp /= dhdp;

		//				if (dp > .1) dp *= .1 / dp;

		//				//increment the curve's p value
		//				cur.m_p += dp;
		//			}
		//		}
		//		//overstepped yarn
		//		if (nWrp < m_Warps.Count - 1)//shift yarn into next warp bracket
		//		{
		//			cur.m_p = 0;//start on the next warp
		//			cur.m_Warps[0] = m_Warps[nWrp];
		//			cur.m_Warps[1] = m_Warps[nWrp + 1];
		//		}
		//		else //land on last warp
		//		{
		//			//this.Remove(cur);
		//			cur.m_p = 1;
		//		}
		//	}
		//}

		#endregion

		#endregion

		#region IGroup Members

		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

		public Sail Sail
		{
			get { return m_sail; }
			set { m_sail = value; }
		}

		TreeNode m_node;

		public TreeNode WriteNode()
		{
			return WriteNode(true);
		}

		private TreeNode WriteNode(bool bclear)
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Tag = this;
			m_node.Text = GetType().Name + ": " + Label;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			if (m_node.Nodes.Count != this.Count || bclear)
			{
				m_node.Nodes.Clear();
				TreeNode yarnNode = new TreeNode("Yarns: " + Count.ToString());
				yarnNode.ImageKey = "Result";
				yarnNode.SelectedImageKey = "Result";
				m_node.Nodes.Add(yarnNode);
				TreeNode guide = new TreeNode("Guide: " + m_guide.Label);
				guide.ImageKey = m_guide.GetType().Name;
				guide.SelectedImageKey = m_guide.GetType().Name;
				m_node.Nodes.Add(guide);
				foreach (Vect2 u in m_guide.uSplines)
					guide.Nodes.Add(u.ToString());
				TreeNode wrps = new TreeNode("Warps");
				wrps.ImageKey = "Warps";
				wrps.SelectedImageKey = "Warps";
				m_node.Nodes.Add(wrps);
				foreach (MouldCurve wrp in m_Warps)
					wrps.Nodes.Add(wrp.ToString());
			}
			return m_node;
		}

		public System.Windows.Forms.Control Editor
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Entity[] CreateEntities()
		{
			List<Entity> yarns = new List<Entity>(Count);
			Vect3[] pnts;

			foreach (YarnCurve yarn in this)
			{
				pnts = yarn.GetPathPoints(100);
				yarns.Add(new LinearPath(ConvertPoints(pnts)));
				yarns.Last().EntityData = this;
			}

			if (m_Combs != null)
			{
				foreach (DensityComb comb in m_Combs)
				{
					List<Entity> es = comb.CreateEntities().ToList();
					foreach (Entity e in es)
						e.EntityData = this;
					yarns.AddRange(es);
				}
			}

			return yarns.ToArray();
		}

		/// <summary>
		/// converts all Vect3 to Point3D
		/// </summary>
		/// <param name="pts">the array to convert</param>
		/// <returns>a new Point3D array</returns>
		public static Point3D[] ConvertPoints(Vect3[] pts)
		{
			return Array.ConvertAll<Vect3, Point3D>(pts, Utilities.Vect3ToPoint3D);
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				return new devDept.Eyeshot.Labels.Label[]{ new devDept.Eyeshot.Labels.OutlinedText(m_Warps[0].GetLabelPoint3D(.66), Label,
					new Font("Helvectiva", 8.0f), Color.White, Color.Black, ContentAlignment.MiddleCenter)};
			}
		}

		#endregion

		#region IRebuild Members

		public bool Rebuild(List<IRebuild> parents)
		{
			bool bupdate = Affected(parents);

			if (bupdate && parents != null)
				parents.Add(this);

			if (bupdate)
				LayoutYarns(null, null, 0);

			return bupdate;
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = connected == null;
			if (!bupdate)
			{
				bupdate |= connected.Contains(m_guide);
				foreach (MouldCurve warp in m_Warps)
					bupdate |= connected.Contains(warp);
				bupdate |= TargetDenier == null ? false : TargetDenier.Affected(connected);
				bupdate |= YarnDenier == null ? false : YarnDenier.Affected(connected);
			}
			return bupdate;
		}

		public bool Update(Sail s)
		{
			LayoutYarns(null, null, 0);
			return true;
		}

		public bool Delete() { return false; }

		public void GetConnected(List<IRebuild> connected)
		{
			if (Affected(connected) && connected != null)
				connected.Add(this);
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			if (txt == null || txt.Count == 0)
				return false;
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
				//nLine += lines.Count - 1;
				splits = lines[0].Split(':');
				if (splits.Length > 0)
				{
					if (splits[0].ToLower().Contains("td"))
						m_targetDenierEqu = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'), sail);
					else if (splits[0].ToLower().Contains("yd"))
						m_yarnDenierEqu = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'), sail);

					else if (splits[0].ToLower().Contains("scale"))
						m_Scale = Convert.ToDouble(splits[1]);
					else if (splits[0].ToLower().Contains("guide"))
						m_guide = sail.FindCurve(splits[1].Trim()) as GuideComb;
					else if (splits[0].ToLower().Contains("warps"))
					{
						for (int i = 1; i < lines.Count; i++)
							m_Warps.Add(sail.FindCurve(lines[i].Trim()));

					}
					else if (splits[0].ToLower().Contains("spos"))
					{
						m_densityCurveLocations.Clear();
						string[] dat = splits[1].Split(new char[] { ',' });
						foreach (string s in dat)
						{
							if (s == " ") continue;
							m_densityCurveLocations.Add(Convert.ToDouble(s));
						}
					}
				}
			}

			Update(sail);

			return true;
		}

		public List<string> WriteScript()
		{
			List<string> script = new List<string>();
			script.Add(GetType().Name + ": " + Label);
			//script.Add("\tTargetDPI: ");
			script.Add("\t" + TargetDenier.ToString());
			script.Add("\t" + YarnDenier.ToString());
			script.Add("\tScale: " + m_Scale);
			script.Add("\tGuide: " + m_guide.Label);
			script.Add("\tWarps: ");
			foreach (MouldCurve w in m_Warps)
				script.Add("\t\t" + w.Label);
			string s = "\tsPos: ";
			foreach (double v in sPos)
				s += v.ToString() + ", ";
			script.Add(s);

			return script;
		}

		#endregion

	}
}
