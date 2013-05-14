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
	public class YarnGroup : List<YarnCurve>, IGroup
	{
		public YarnGroup() : this("", null, 0) { }

		public YarnGroup(string label, Sail s, double yarnDenier)
		{
			Label = label;
			Sail = s;
			m_yarnDenier = new Equation("yarndenier", yarnDenier); // individual yarn denier (input)
		}

		public YarnGroup(string label, Sail s, Equation yarnDenier, Equation targetDPI)
		{
			Label = label;
			Sail = s;
			m_yarnDenier = yarnDenier; // individual yarn denier (input)
			m_targetDenier = targetDPI;
		}

		#region Members

		//IGroup
		string m_label;
		Sail m_sail;

		//Yarn Materials
		//double m_targetDenier = 0;
		//double m_yarnDenier = 0;

		Equation m_yarnDenier = new Equation("yarndenier", 0.0);
		Equation m_targetDenier = new Equation("targetdpi", 0.0);

		public Equation YarnDenierEqu
		{
			get { return m_yarnDenier; }
			set { m_yarnDenier = value; m_yarnDenier.Label = "yarndenier"; }
		}	
		public Equation TargetDenierEqu
		{
			get { return m_targetDenier; }
			set { m_targetDenier = value; m_targetDenier.Label = "targetdpi"; }
		}
		public double AchievedDpi = 0;

		//Fitting Values
		Ending m_end = Ending.ToWarp;
		double m_Scale = 1;
		List<double> m_CombPos = new List<double>();

		//Geometry Curves
		List<MouldCurve> m_Warps = new List<MouldCurve>();
		GuideComb m_guide;
		DensityComb[] m_Combs;// = new DensityComb[spos.Length]; 

		#endregion

		#region Fitting Parameters


		public List<MouldCurve> Warps
		{
			get { return m_Warps; }
			set { m_Warps = value; }
		}
		public GuideComb Guide
		{
			get { return m_guide; }
			set { m_guide = value; }
		}
		public double TargetDpi
		{
			get { return m_targetDenier.Result; }
			set { m_targetDenier.SetValue(value); }
		}
		public double YarnDenier
		{
			get { return m_yarnDenier.Result; }
			set { m_yarnDenier.SetValue(value); }
		}
		public List<double> DensityPos
		{
			get { return m_CombPos; }
			set { m_CombPos = value; }
		}
		public Ending EndCondition
		{
			get { return m_end; }
			set { m_end = value; }
		}
		public enum Ending
		{
			InWarp,
			ToWarp,
			OnWarp,
			Evens,
			Odds,
			Quads
		}

		#endregion

		#region LayoutYarns
		public event EventHandler<EventArgs<YarnGroup>> YarnsUpdated;

		/// <summary>
		/// return the target yarn spacing as determined by the guide curve
		/// </summary>
		/// <param name="sGuide">the s-positon on the guide curve to eval at</param>
		/// <returns>the target yarn spacing in meters</returns>
		internal double EvalSpacing(double sGuide)
		{
			Vect2 uv =new Vect2();
			Vect3 xyz = new Vect3();
			double h = 0;
			m_guide.hVal(sGuide, ref uv, ref h);
			h = Math.Max(h, 1e-5);//h must be positive;
			return Math.Max(0.0001, m_Scale * YarnDenier * 0.0254 / TargetDpi / h);//enforce 1mm minimum spacing
		}
		/// <summary>
		/// return the target yarn spacing as determined by the guide curve
		/// </summary>
		/// <param name="h">the yarn spacing to calculate the comb height from</param>
		/// <returns>the comb height</returns>
		internal double InverseSpacing(double h)
		{
			return Math.Max(0.0001, m_Scale * YarnDenier * 0.0254 / TargetDpi / h);//enforce minimum height 
		}

		/// <summary>
		/// Spreads yarns using the current fitting parameters
		/// </summary>
		/// <returns>The number of yarns created -1 if failed</returns>
		public int LayoutYarns()
		{
			return LayoutYarns(null, null, 0);
		}

		/// <summary>
		/// creates yarns between the given warps with spacing controlled by a guide curve and target dpi
		/// </summary>
		/// <param name="warps">a list of warps to spread</param>
		/// <param name="guide">the guide comb that controls the spacing, this must intersect every warp</param>
		/// <param name="targetDpi">the target dpi to match</param>
		/// <returns>the number of yarns created, -1 if incorrect parameters, -Count if unconverged</returns>
		public int LayoutYarns(List<MouldCurve> warps, GuideComb guide, double targetDpi)
		{
			//spread yarns to achieve target dpi
			int res = SpreadTargetDpi(warps, guide, targetDpi, SpreadYarnsGlobally);

			//once we have hit our target dpi, satisfy end condition
			if (res > 0)
				switch (m_end)
				{
					case Ending.ToWarp://end when overstepped P
						break;
					case Ending.InWarp://remove overstepping yarn
						YarnCurve last = this.Last();
						if (last.m_p > 1 - 1e-5)
						{
							Remove(last);
							res = Count;
						}
						break;
					case Ending.OnWarp://land on last warp exactly
						res = SpreadTargetCount(null, null, -1, SpreadYarnsGlobally);
						break;
					case Ending.Evens:
						if (Count % 2 != 0)
						{
							int nCnt = Count;
							nCnt += AchievedDpi > TargetDpi ? -1 : 1;//add or subtract a yarn depending on dpi
							//respread yarns to new target count
							res = SpreadTargetCount(null, null, nCnt, SpreadYarnsGlobally);
						}
						break;
					case Ending.Odds:
						if (Count % 2 != 1)
						{
							int nCnt = Count;
							nCnt += AchievedDpi > TargetDpi ? -1 : 1;//add or subtract a yarn depending on dpi
							//respread yarns to new target count
							res = SpreadTargetCount(null, null, nCnt, SpreadYarnsGlobally);
						}
						break;
					case Ending.Quads:
						int mod = Count % 4 ;
						if (mod != 0)
						{
							int nCnt = Count;
							switch (mod)
							{
								case 1:
									nCnt -= 1;//remove extra yarn
									break;
								case 2:
									nCnt += AchievedDpi > TargetDpi ? -2 : 2;//add or remove 2 yarns depending on dpi
									break;
								case 3:
									nCnt += 1;//add extra yarn
									break;
							}
							//respread yarns to new target count
							res = SpreadTargetCount(null, null, nCnt, SpreadYarnsGlobally);
						}
						break;
				}

			//calculate the final dpi
			if (res > 0)
				AchievedDpi = CheckDpi(DensityPos);
			else
				m_Combs = null;

			return res;
		}

		/// <summary>
		/// creates yarns between the given warps with spacing controlled by a guide curve and target dpi
		/// </summary>
		/// <param name="warps">a list of warps to spread</param>
		/// <param name="guide">the guide comb that controls the spacing, this must intersect every warp</param>
		/// <param name="targetDpi">the target dpi to match</param>
		/// <param name="SpreadYar">the yarn spreading function to use</param>
		/// <returns>the number of yarns created, -1 if incorrect parameters, -Count if unconverged</returns>
		int SpreadTargetDpi(List<MouldCurve> warps, GuideComb guide, double targetDpi, YarnSpreader SpreadYar)
		{
			//store new warps etc
			if (warps != null && Warps != warps)
				Warps = warps;
			if (guide != null && m_guide != guide)
				m_guide = guide;
			if (targetDpi > 0 && TargetDpi != targetDpi)
				TargetDpi = targetDpi;

			if (Warps == null || Warps.Count == 0 || m_guide == null || TargetDpi == 0)
				return -1;

			int nNwt;
			int MAXNWT = 100;
			for (nNwt = 0; nNwt < MAXNWT; nNwt++)
			{
				//spread yarns, no target count
				SpreadYar(-1);
				if (Count == MAXYAR) //ended early, increase scale and try again
				{
					m_Scale = 1 / this.Last().m_p;
					continue;
				}
				AchievedDpi = CheckDpi(DensityPos);
				if( YarnsUpdated != null )
					YarnsUpdated(this, new EventArgs<YarnGroup>(this));

				if (BLAS.is_equal(AchievedDpi, TargetDpi, 200))
					break;
				m_Scale *= AchievedDpi / TargetDpi;
			}

			return nNwt < MAXNWT ? Count : -Count;
		}
		/// <summary>
		/// returns the groups average dpi using density combs at the specified positions
		/// defaults to the guide and warp[0] intersection or 0.5 if that fails
		/// </summary>
		/// <param name="spos">a list of density curves s-positons on the starting warp</param>
		/// <returns>the average dpi of the group</returns>
		double CheckDpi(List<double> spos)
		{
			if (spos == null)
				spos = new List<double>();
			//default to 1 density curve at the guide X warp[0] point if none specified
			if( spos.Count == 0)
			{
				Vect2 u = new Vect2();
				Vect3 x = new Vect3();
				Vect2 s = new Vect2();
				//get starting warp cross point as singular dpi curve
				if (!m_guide.CrossPoint(m_Warps[0], ref u, ref x, ref s))
					s[1] = 0.5;//default midpoint on cross fail
				spos.Add(s[1]);
			}

			//create combs for inspection
			m_Combs = new DensityComb[spos.Count];
			int i =0;
			double d = 0;
			foreach( double s in spos )
			{
				m_Combs[i] = new DensityComb(this, s);
				//generate combs and accumulate dpi
				d += m_Combs[i].DPI;
				i++;
			}
			d /= m_Combs.Length;//return average dpi
			return d;
		}

		/// <summary>
		/// Spreads yarns to land on the final warp exactly using the current number of yarns as a target
		/// </summary>
		/// <returns>The number of yarns if successful. Negative if failed.</returns>
		int SpreadTargetCount(List<MouldCurve> warps, GuideComb guide, int nTarget, YarnSpreader SpreadYar)
		{
			//store new warps etc
			if (warps != null && Warps != warps)
				Warps = warps;
			if (guide != null && m_guide != guide)
				m_guide = guide;
			if (nTarget <= 0)
				nTarget = Count;

			if (Warps == null || Warps.Count == 0 || m_guide == null || nTarget <= 0)
				return -1;

			YarnCurve last = null;
			int nNwt;
			for (nNwt = 0; nNwt < 100; nNwt++)
			{
				//generate the desired number of yarns
				SpreadYar(nTarget);

				last = this.Last();
				if (last == null)
					break;

				if (YarnsUpdated != null)
					YarnsUpdated(this, new EventArgs<YarnGroup>(this));

				if (BLAS.is_equal(last.m_p, 1, 1e-4) && Count == nTarget)//check to see if last yarn hits warp and that the target number of yarns exist
				{
					//last.m_p = 1;//enssure towarp
					break;
				}
				if (Count == nTarget)
					m_Scale /= (1.0 + (last.m_p - 1.0) * .6);//adjust scale to land last yarn on warp, go 80% to target at a time
				else
					m_Scale /= (double)nTarget / (double)Count;//adjust to get the correct number of yarns

			}

			return nNwt < 100 ? Count : -Count;
		}

		#region SpreadYar

		const int MAXYAR = 1000;
		delegate void YarnSpreader(int nTargetYar);

		/// <summary>
		/// Spreads yarns across any number of warps using a global-P algorithm
		/// </summary>
		public void SpreadYarnsGlobally(int nTarget)
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
			CurveTools.CrossPoint(m_guide, m_Warps[0], ref u, ref x, ref s, 15);
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

			//check target count condition
			if (nTarget <= 0)
				nTarget = MAXYAR;

			//add initial yarn on starting warp
			//int nYar = 0;
			int nWrp = 1;
			double dHdP = 0, dP;
			double P = 0, p = 0;
			Add(new YarnCurve(p, m_Warps[0], m_Warps[1]));
			while (P < 1 && Count < MAXYAR && Count < nTarget)
			{
				//intersection of previous yarn and guide curve
				CurveTools.CrossPoint(m_guide, this.Last(), ref u, ref x, ref s, 20);
				//determine target spacing from dpi equation
				hTarget = EvalSpacing(s[0]);
				dP = .1;
				if (dHdP != 0 && !double.IsInfinity(dHdP))//attempt better starting guess using previous yarn's dHdP
					dP = hTarget / dHdP;
				dP = Math.Min(dP, .1); //enforce maxmimum inital step

				P += dP;
				P = Math.Min(1, P);
				p = GlobalToBracket(P, pWarps, ref nWrp);

				if (Count > 2 && BLAS.is_equal(p, this.Last().m_p, 1e-7)) //zero length space
					break;

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
						if(nNwt < 5 )
							//ensure inbounds initially
							P = Math.Min(1.3, P);

						//set the yarn's bracket p value and warps
						cur.m_p = GlobalToBracket(P, pWarps, ref nWrp);
						cur.m_Warps[0] = m_Warps[nWrp - 1];
						cur.m_Warps[1] = m_Warps[nWrp];
					}
				}
				//if (YarnsUpdated != null)
				//	YarnsUpdated(this, new EventArgs<YarnGroup>(this));
			}
		}
		public double GlobalToBracket(double pG, List<double> pWarps, ref int nWrp)
		{
			if (pG >= 1)//end condition
			{
				nWrp = pWarps.Count - 1;
			}
			else//find warp bracket
			{
				nWrp = pWarps.Count;
				for (nWrp = 1; nWrp < pWarps.Count; nWrp++)
				{
					if (pWarps[nWrp - 1] <= pG && pG < pWarps[nWrp])//found bracket
						break;
				}
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

		/// <summary>
		/// Spreads yarns across any number of warps using warp-stepping and bracket-p algorithm
		/// SpreadYarnsGlobally is preferred 
		/// </summary>
		public void SpreadYarnsOverWarps(int nTarget)
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

			if (nTarget <= 0)
				nTarget = MAXYAR;

			//add initial yarn on starting warp
			//int nYar = 0;
			Add(new YarnCurve(0, m_Warps[0], m_Warps[1]));
			int nWrp = 1;
			//for (int nWrp = 1; nWrp < m_Warps.Count; nWrp++)
			//{
			while (this.Last().m_p < 1 && Count < MAXYAR && Count < nTarget)
			{
				//intersection of previous yarn and guide curve
				CurveTools.CrossPoint(m_guide, this.Last(), ref u, ref x, ref s, 20);
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
		
		//void SpreadYarnsByCount(int nTarget)//became the same as SpreadGlobally
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

		//	//yarn points for warp stepping
		//	Vect2[] uYars = new Vect2[] { new Vect2(), new Vect2() };
		//	Vect3[] xYars = new Vect3[] { new Vect3(), new Vect3() };
		//	//warp points for warp stepping
		//	Vect2[] uWars = new Vect2[] { new Vect2(), new Vect2(), new Vect2() };
		//	Vect3[] xWars = new Vect3[] { new Vect3(), new Vect3(), new Vect3() };

		//	//determine warp pPos crossovers from guide/warp0 starting point
		//	CurveTools.CrossPoint(m_guide, m_Warps[0], ref u, ref x, ref s, 15);
		//	List<double> pWarps = new List<double>(m_Warps.Count);
		//	pWarps.Add(0);
		//	int i = 0;
		//	foreach (IMouldCurve wrp in m_Warps)
		//	{
		//		wrp.uVal(s[1], ref uYars[0]);
		//		if (i != 0)
		//			pWarps.Add(uYars[0].Distance(uYars[1]) + pWarps[i - 1]);
		//		uYars[1].Set(uYars[0]);
		//		i++;
		//	}
		//	for (i = 0; i < pWarps.Count; i++)
		//		pWarps[i] /= pWarps.Last();

		//	//add initial yarn on starting warp
		//	//int nYar = 0;
		//	int nWrp = 1;
		//	double dHdP = 0, dP;
		//	double P = 0, p = 0; //P: global paramter, p: bracket parameter
		//	Add(new YarnCurve(p, m_Warps[0], m_Warps[1]));
		//	while (P < 1 && Count < MAXYAR && Count < nTarget)
		//	{
		//		//intersection of previous yarn and guide curve
		//		CurveTools.CrossPoint(m_guide, this.Last(), ref u, ref x, ref s, 20);
		//		//determine target spacing from dpi equation
		//		hTarget = EvalSpacing(s[0]);
		//		dP = .1;
		//		if (dHdP != 0)//attempt better starting guess using previous yarn's dHdP
		//			dP = hTarget / dHdP;
		//		dP = Math.Min(dP, .1); //enforce maxmimum inital step

		//		P += dP;
		//		//	P = Math.Min(1, P);
		//		p = GlobalToBracket(P, pWarps, ref nWrp);

		//		//create a yarn with an initial spacing
		//		cur = new YarnCurve(p, m_Warps[nWrp - 1], m_Warps[nWrp]);
		//		Add(cur);
		//		//xYar.Set(x);
		//		sYar = s[1];
		//		hYar = 0;

		//		for (int nNwt = 0; nNwt < 100; nNwt++)
		//		{
		//			xYar.Set(x);
		//			bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-5, true);
		//			if (!res)//default to p-distance if failed
		//			{
		//				cur.xVal(sYar, ref uYar, ref xYar);
		//				hYar = xYar.Distance(x);
		//			}

		//			if (Math.Abs(hTarget - hYar) < 1e-5)
		//			{
		//				cur.m_h = hYar;//record height
		//				break;
		//			}
		//			else
		//			{
		//				//calc global-P derivative
		//				dHdP = hYar / (BracketToGlobal(this[Count - 1], pWarps) - BracketToGlobal(this[Count - 2], pWarps));
		//				dP = hTarget - hYar;
		//				dP /= dHdP;
		//				// determine delta P and enforce max step
		//				if (dP > .1) dP *= .1 / dP;

		//				//increment the current P value
		//				P += dP;
		//				//allow out of bounds

		//				//set the yarn's bracket p value and warps
		//				cur.m_p = GlobalToBracket(P, pWarps, ref nWrp);
		//				cur.m_Warps[0] = m_Warps[nWrp - 1];
		//				cur.m_Warps[1] = m_Warps[nWrp];
		//			}
		//		}
		//		//if (YarnsUpdated != null)
		//		//	YarnsUpdated(this, new EventArgs<YarnGroup>(this));
		//	}
		//}


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
		//public void SpreadYarnsAcrossWarps()
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

		//	//determine warp pPos crossovers

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
		//				bool res = CurveTools.xClosest(cur, ref sYar, ref uYar, ref xYar, ref hYar, 1e-3, true);

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

		////Spreads yarns to hit the required count
		#endregion

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

		//private TreeNode WriteNode(bool bclear)
		//{
		//	if (m_node == null)
		//		m_node = new System.Windows.Forms.TreeNode();
		//	m_node.Tag = this;
		//	m_node.Text = GetType().Name + ": " + Label;
		//	m_node.ImageKey = GetType().Name;
		//	m_node.SelectedImageKey = GetType().Name;

		//	if (bclear)
		//	{
		//		m_node.Nodes.Clear();

		//		//write the warp cuves
		//		TreeNode wrps = new TreeNode("Warps");
		//		wrps.ImageKey = "Warps";
		//		wrps.SelectedImageKey = "Warps";
		//		m_node.Nodes.Add(wrps);
		//		foreach (MouldCurve wrp in m_Warps)
		//			wrps.Nodes.Add(wrp.ToString());

		//		//write the guide curve
		//		TreeNode guide = new TreeNode("Guide: " + m_guide.Label);
		//		guide.ImageKey = m_guide.GetType().Name;
		//		guide.SelectedImageKey = m_guide.GetType().Name;
		//		m_node.Nodes.Add(m_guide.ToString());

		//		//write the yarns
		//		TreeNode yarnNode = new TreeNode("Yarns: " + Count.ToString());
		//		yarnNode.ImageKey = "Result";
		//		yarnNode.SelectedImageKey = "Result";
		//		m_node.Nodes.Add(yarnNode);
		//		foreach (YarnCurve yar in this)
		//			yarnNode.Nodes.Add(yar.ToString());

	
		//	}
		//	return m_node;
		//}

		private string GetToolTipData()
		{
			return String.Format(
				"{0}\nTargetDPI:{1}\nAchievedDPI:{2}\n#:{3}",
				GetType().Name, TargetDpi, AchievedDpi.ToString("#0.00"), Count
			);
		}

		private TreeNode WriteNode(bool bclear)
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Tag = this;
			//m_node.Text = GetType().Name + ": " + Label;
			m_node.Text = Label;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.ToolTipText = GetToolTipData();
			if (bclear)
			{
				m_node.Nodes.Clear();

				TreeNode yarnNode = new TreeNode("Yarns: " + Count.ToString());
				yarnNode.ImageKey = "Result";
				yarnNode.SelectedImageKey = "Result";
				m_node.Nodes.Add(yarnNode);
				for (int i = 0; i < Count; i++)
					yarnNode.Nodes.Add(String.Format(Label + "[{0}] {1} {2} {3} {4}", 
						i.ToString(Count > 99 ? "000" : Count > 9 ? "00" : "0"), //index
						this[i].m_p.ToString("0.0000"), //p value
						m_Warps[0].Label, m_Warps[1].Label, //warp labels
						this[i].m_h.ToString("f6"))); // yarn spacing

				if (m_guide != null)
				{
					TreeNode guide = new TreeNode(m_guide.WriteNode().Text);
					guide.ImageKey = m_guide.GetType().Name;
					guide.SelectedImageKey = m_guide.GetType().Name;
					m_node.Nodes.Add(guide);
					Vect2 uv = new Vect2(); double h = 0;
					if(m_guide.SComb != null )
					foreach (double s in m_guide.SComb)
					{
						m_guide.hVal(s, ref uv, ref h);
						guide.Nodes.Add(string.Format("{0} [{1}] {2}", s.ToString("0.000"), uv.ToString("0.000"), h.ToString("0.0000")));
					}
				}
				if (m_Warps != null)
				{
					TreeNode wrps = new TreeNode("Warps");
					wrps.ImageKey = "Warps";
					wrps.SelectedImageKey = "Warps";
					m_node.Nodes.Add(wrps);
					foreach (MouldCurve wrp in m_Warps)
						wrps.Nodes.Add(wrp.WriteNode().Text);
				}

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
			if( m_Combs != null )
				foreach (DensityComb comb in m_Combs)
				{
					List<Entity> es = comb.CreateEntities().ToList();
					foreach (Entity e in es)
						e.EntityData = this;
					yarns.AddRange(es);
				}

			return yarns.ToArray();
		}

		public Entity[] CreateOnlyYarnEntities()
		{
			List<Entity> yarns = new List<Entity>(Count);
			Vect3[] pnts;

			foreach (YarnCurve yarn in this)
			{
				pnts = yarn.GetPathPoints(100);
				yarns.Add(new LinearPath(ConvertPoints(pnts)));
				yarns.Last().EntityData = this;
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


		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = connected == null;
			if (!bupdate)
			{
				bupdate |= connected.Contains(m_guide);
				foreach (MouldCurve warp in m_Warps)
					bupdate |= connected.Contains(warp);
				bupdate |= TargetDenierEqu == null ? false : TargetDenierEqu.Affected(connected);
				bupdate |= YarnDenierEqu == null ? false : YarnDenierEqu.Affected(connected);
			}
			return bupdate;
		}		
		public bool Update(Sail s)
		{
			bool ret = true;
			ret &= !double.IsNaN(YarnDenierEqu.Evaluate(s));
			ret &= !double.IsNaN(TargetDenierEqu.Evaluate(s));
			if (ret)
				ret &= LayoutYarns() > 0;
			return ret;
		}
		public bool Delete() { return false; }
		public void GetConnected(List<IRebuild> connected)
		{
			if (Affected(connected) && connected != null)
			{
				connected.Add(this);
			}
		}
		public void GetParents(Sail s, List<IRebuild> parents)
		{
			parents.Add(Guide);
			parents.AddRange(Warps);

		//	parents.Add(TargetDenierEqu);
			TargetDenierEqu.GetParents(s, parents);

		//	parents.Add(YarnDenierEqu);
			YarnDenierEqu.GetParents(s, parents);
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
					if (splits[0].ToLower().Contains("targetdpi"))
						m_targetDenier = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'));
					else if (splits[0].ToLower().Contains("yarndenier"))
						m_yarnDenier = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'));
					else if(splits[0].ToLower().Contains("ending"))
						EndCondition = (Ending)Enum.Parse(typeof(Ending), splits[1].Trim()); 
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
						DensityPos.Clear();
						string[] dat = splits[1].Split(new char[] { ',' });
						foreach (string s in dat)
						{
							if (s == " ") continue;
							DensityPos.Add(Convert.ToDouble(s));
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
			script.Add("\t" + m_targetDenier.ToString());
			script.Add("\t" + m_yarnDenier.ToString());
			script.Add("\tScale: " + m_Scale);
			script.Add("\tGuide: " + m_guide.Label);
			script.Add("\tWarps: ");
			foreach (MouldCurve w in m_Warps)
				script.Add("\t\t" + w.Label);
			string s = "\tsPos: ";
			foreach (double v in DensityPos)
				s += v.ToString() + ", ";
			script.Add(s);

			script.Add("\tEnding: " + EndCondition.ToString());

			return script;
		}

		#endregion

	}
}
