using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using devDept.Eyeshot.Entities;

namespace Warps
{
	[System.Diagnostics.DebuggerDisplay("{Label} {m_clothAlignment} Count={Count}", Name = "{Label}", Type = "{GetType()}")]
	public class PanelGroup : List<Panel>, IGroup
	{
		public PanelGroup() : this("", null, new Equation("", 1.0), ClothOrientations.FILLS) { }
		public PanelGroup(string label, Sail s) : this(label, s, new Equation("panelwidth", 1.0), ClothOrientations.FILLS) { }
		public PanelGroup(string label, Sail s, Equation width, ClothOrientations alignment)
		{
			Label = label;
			Sail = s;
			Bounds = new List<MouldCurve>();
			Guides = new List<MouldCurve>();
			PanelWidth = width;
			ClothAlignment = alignment;
		}

		public int LayoutPanels(List<MouldCurve> boundaryCurves, List<MouldCurve> guides, ClothOrientations align, double width)
		{
			//store values
			if (boundaryCurves != null)
				Bounds = boundaryCurves;
			if (guides != null)
				Guides = guides;

			if (Guides == null || Bounds == null)
				return -1;
			else if (Guides.Count == 0 || Bounds.Count == 0)
				return -1;

			//if (width.Result > 0)
			//m_Width = width;
			ClothAlignment = align;
			Clear();

			//get boundary intersection uv and s pos
			Vect2 sCor = new Vect2();
			Vect3[] xCorners = new Vect3[Bounds.Count];//xyz corner points
			Vect2[] uCorners = new Vect2[Bounds.Count];//uv corner points
			sCorners = new Vect2[Bounds.Count];//s pos of each corner for each curve 0:start, 1:end
			if (Bounds.Count > 0) sCorners[0] = new Vect2();//initialize starting scorner
			for (int nB = 0; nB < Bounds.Count; nB++)
			{
				int nF = nB == Bounds.Count - 1 ? 0 : nB + 1;//forward boundaries index
				uCorners[nB] = new Vect2();
				xCorners[nB] = new Vect3();
				if (!CurveTools.CrossPoint(Bounds[nB], Bounds[nF], ref uCorners[nB], ref xCorners[nB], ref sCor, 20))
					throw new Exception(string.Format("Panel Group [{0}] Seam [{1}] does not intersect Seam [{2}]", Label, Bounds[nB].Label, Bounds[nF].Label));

				if (nF > 0) sCorners[nF] = new Vect2();//initialize forward scorner
				sCorners[nB][1] = sCor[0];//this curve's endpos
				sCorners[nF][0] = sCor[1];//next curve's startpos
			}

			List<GuideCross>[] xGuides = new List<GuideCross>[Guides.Count];
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			//Vect3[] xGuides = new Vect3[Guides.Count];
			//Vect2[] uGuides = new Vect2[Guides.Count];//uv guide intersections
			//Vect2[] sGuides = new Vect2[Guides.Count];//s pos of each corner for each curve 0:start, 1:end
			//get guide/boundary intersections
			for (int nG = 0; nG < Guides.Count; nG++)
			{
				xGuides[nG] = new List<GuideCross>(2);//assume 2 intersections
				for (int nB = 0; nB < Bounds.Count; nB++)
					if (CurveTools.CrossPoint(Guides[nG], Bounds[nB], ref uv, ref xyz, ref sCor, 20))
					{
						//int nC = sCorners[nB][0] > sCorners[nB][1] ? 1 : 0;//accomodate curve direction
						//ensure intersection is inside our bounds
						//if (sCorners[nB][nC] <= sCor[1] && sCor[1] <= sCorners[nB][1-nC])
						if (Utilities.IsBetween(sCorners[nB][0], sCor[1], sCorners[nB][1]))
						{
							bool bdupe = false;
							foreach (GuideCross cross in xGuides[nG])
								if (BLAS.is_equal(cross.sPos, sCor[0]))
									bdupe = true;
							if (!bdupe)//dont add duplicate ins
								xGuides[nG].Add(new GuideCross(sCor[0], uv, xyz, Bounds[nB], sCor[1]));//store all intersections
						}
					}
				if (xGuides[nG].Count != 2) //guide doesnt cross bounds
					throw new Exception(string.Format("Panel Group [{0}] Guide Curve [{1}] does not intersect 2 boundry warps", Label, Guides[nG].Label));
			}

			//use first seam as initial seam
			MouldCurve[] seams = new MouldCurve[2];
			//construct initial backseam as a segment of the bounding curve
			seams[1] = xGuides[0][0].Seam;//start on initial intersection seam
			seams[0] = new MouldCurve(Label + Count.ToString("000"), xGuides[0][0].Seam, sCorners[m_bounds.IndexOf(seams[1])]);
			bool atSeam = false;
			Vect2 sPos = new Vect2();
			do
			{
				width = Width;//initialize guess width
				//iterate on width to lay second seam
				switch (ClothAlignment)
				{
					case ClothOrientations.WARPS:
						//get seam/bound interesctions
						//project up bounds to get target width at endpoints
						//span girth between endpoints
						break;
					case ClothOrientations.FILLS:
						//single guide
						//get guide/seam0 intersection
						if (!CurveTools.CrossPoint(Guides[0], seams[0], ref uv, ref xyz, ref sPos, 20))
							break;
						GuideCross start = new GuideCross(sPos[0], uv, xyz, seams[0], sPos[1]);
						//estimate delta-x/delta-s for width based guide-stepping
						double cord = xGuides[0][1].xPos.Distance(xGuides[0][0].xPos);
						double span = xGuides[0][1].sPos - xGuides[0][0].sPos;
						double dxds = cord / span;
						double ds = width / dxds;//guess delta s
						int nNwt = 0;
						for (nNwt = 0; nNwt < 50; nNwt++)
						{
							//project up guide to target width
							Guides[0].xVal(start.sPos + ds, ref uv, ref xyz);

							//check "width"
							span = xyz.Distance(start.xPos);
							if (BLAS.is_equal(span, width, 1e-5))
								break;//success

							dxds = span / ds;//recalculate dxds using current step
							ds = width / dxds;//recalc ds using new gradient
						}
						if (nNwt == 50)
							throw new Exception(string.Format("Panel Group [{0}] failed to project seam for panel [{1}]", Label, Count));

						double s = start.sPos + ds;
						if (!Utilities.IsBetween(xGuides[0][0].sPos, s, xGuides[0][1].sPos))
						{
							atSeam = true;
							break;
						}

						//span girth on curve normal to bounds intersections
						//CurvePoint startpt = new CurvePoint(Guides[0], s);
						//set the endpoint angle
						Vect3 xyzGuide = new Vect3();
						Vect3 dxnGuide = new Vect3();
						Guides[0].xVec(s, ref uv, ref xyzGuide, ref dxnGuide);//get the target point and tangent vector
						List<CurvePoint> ends = new List<CurvePoint>(2);
						for (int nB = 0; nB < Bounds.Count; nB++)
						{
							xyz.Set(xyzGuide);//initialize guess
							s = start.sPos + ds;
							if (!CurveTools.AnglePoint(Bounds[nB], ref s, ref uv, ref xyz, dxnGuide, Math.PI / 2.0, true))
								continue; //no intersection with this boundary curve

							//AnglePoint endpt = new AnglePoint(Bounds[nB], Math.PI / 2.0);
							//AnglePoint.SetAnglePoint(Sail.Mould, startpt, endpt);
							//set the angle and ensure it's in our bracket
							if (Utilities.IsBetween(sCorners[nB][0], s, sCorners[nB][1]))
							{
								ends.Add(new CurvePoint(Bounds[nB], s));
							}
						}
						if (ends.Count != 2)
							throw new Exception(string.Format("Panel Group [{0}] failed to set angle points for panel [{1}]", Label, Count));

						//fit the girth seam
						seams[1] = new MouldCurve(Label + (Count + 1).ToString("000"), Sail, ends.ToArray());
						Add(new Panel(seams, MakeEndSegments(seams)));
						//set seam as next starting seam
						seams[0] = seams[1];
						break;
					case ClothOrientations.PERPS:
						//get guide cord
						//project up cord to target width
						//forward step girth to bounds intersections
						break;
				}

				//construct end-cap poly curves
				//flatten and check actual width(panel.Width) vs input width(m_Width)
				//iterate target width to hit input width
			}
			while (!atSeam);

			return atSeam ? Count : -Count;
		}

		private List<SeamSegment>[] MakeEndSegments(MouldCurve[] seams)
		{
			CurvePoint[] s = new CurvePoint[2];
			s[0] = seams[0].FitPoints[0] as CurvePoint;
			s[1] = seams[0].FitPoints.Last() as CurvePoint;
			CurvePoint[] e = new CurvePoint[2];
			e[0] = seams[1].FitPoints[0] as CurvePoint;
			e[1] = seams[1].FitPoints.Last() as CurvePoint;

			if (s[0] == null || s[1] == null || e[0] == null || e[1] == null)
				return null;


			//if (s[0].UV.Distance(e[1].UV) < s[0].UV.Distance(e[0].UV))//seams face opposing directions
			//{
			//	CurvePoint swap = e[1];
			//	e[1] = e[0];
			//	e[0] = swap;
			//}

			List<SeamSegment>[] ends = new List<SeamSegment>[2];
			for (int nEnd = 0; nEnd < 2; nEnd++)
			{
				ends[nEnd] = new List<SeamSegment>();
				if (s[nEnd].Curve == e[nEnd].Curve)//same curve, single-segment
				{
					ends[nEnd].Add(new SeamSegment(s[nEnd].Curve, new Vect2(s[nEnd].SCurve, e[nEnd].SCurve)));
				}
				else//different curves, multi-segment
				{
					int nB0 = m_bounds.IndexOf(s[nEnd].Curve);
					int nB1 = m_bounds.IndexOf(e[nEnd].Curve);
					//add start segment
					ends[nEnd].Add(new SeamSegment(m_bounds[nB0], new Vect2(sCorners[nB0][0], s[nEnd].SCurve)));
					for (int nB = nB0 + 1; nB < nB1; nB++)
					{
						//add internal segments
						ends[nEnd].Add(new SeamSegment(m_bounds[nB], sCorners[nB]));
					}
					//add end segment
					ends[nEnd].Add(new SeamSegment(m_bounds[nB1], new Vect2(sCorners[nB1][0], e[nEnd].SCurve)));
				}
			}
			return ends;
		}

		#region Members

		//IGroup
		Sail m_sail;
		string m_label;

		Equation m_Width;
		List<MouldCurve> m_bounds;
		List<MouldCurve> m_guides;
		ClothOrientations m_clothAlignment = ClothOrientations.FILLS;

		Vect2[] sCorners;

		public double Width
		{
			get { return m_Width.Result; }
			//set { m_Width = value; }
		}

		public Equation PanelWidth
		{
			get { return m_Width; }
			set { m_Width = value; m_Width.Label = "PanelWidth"; }
		}

		public List<MouldCurve> Bounds
		{
			get { return m_bounds; }
			set { m_bounds = value; }
		}
		public List<MouldCurve> Guides
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

		public IRebuild FindItem(IRebuild item)
		{
			if (!(item is Panel))
				return null;

			return this.Find(pan => (item as Panel) == pan);
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			int i = -1;
			if (tag is Panel)
				i = this.IndexOf(tag as Panel);
			if (tag == this)
				return true;

			if (i >= 0)
				rets.AddRange(this.Take(i));
			else
				rets.AddRange(this);

			return i != -1;//true if found
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

		bool m_locked = false;
		public bool Locked { get { return m_locked; } set { m_locked = value; } }

		public List<string> WriteScript()
		{
			List<string> script = new List<string>();
			script.Add(GetType().Name + ": " + Label);
			//script.Add("\tTargetDPI: ");
			script.Add("\t" + m_Width.ToScriptString());
			script.Add("\tGuides: ");
			foreach (MouldCurve w in m_guides)
				script.Add("\t\t" + w.Label);
			script.Add("\tBounds: ");
			foreach (MouldCurve w in m_bounds)
				script.Add("\t\t" + w.Label);

			script.Add("\tClothAlignment: " + ClothAlignment.ToString());

			return script;
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
				splits = lines[0].Split(':');
				if (splits.Length > 0)
				{
					if (splits[0].ToLower().Contains("bounds"))
					{
						for (int i = 1; i < lines.Count; i++)
							m_bounds.Add(sail.FindCurve(lines[i].Trim()));

					}
					else if (splits[0].ToLower().Contains("guides"))
					{
						for (int i = 1; i < lines.Count; i++)
							m_guides.Add(sail.FindCurve(lines[i].Trim()));

					}
					else if (splits[0].ToLower().Contains("clothalignment"))
						ClothAlignment = (ClothOrientations)Enum.Parse(typeof(ClothOrientations), splits[1].Trim());
					else if (splits[0].ToLower().Contains("panelwidth"))
						m_Width = new Equation(lines[0].Split(new char[] { ':' })[0].Trim('\t'), lines[0].Split(new char[] { ':' })[1].Trim('\t'));

				}
			}

			Update(sail);

			return true;
		}

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new TreeNode(Label);
			m_node.Text = Label;
			m_node.ImageKey = GetType().Name;
			m_node.SelectedImageKey = GetType().Name;
			m_node.ToolTipText = GetToolTipData();
			m_node.Tag = this;
			m_node.Nodes.Clear();
			foreach (Panel p in this)
				m_node.Nodes.Add(p.WriteNode());
			return m_node;
		}

		private string GetToolTipData()
		{
			return GetType().Name;
		}

		public Entity[] CreateEntities()
		{
			List<Entity> ents = new List<Entity>();
			foreach (Panel p in this)
				ents.AddRange(p.CreateEntities());
			ents.ForEach(ent => { ent.EntityData = this; });
			return ents.ToArray();
		}

		public devDept.Eyeshot.Labels.Label[] EntityLabel
		{
			get
			{
				if (Count == 0)
					return null;
				List<devDept.Eyeshot.Labels.Label> ret = new List<devDept.Eyeshot.Labels.Label>();
				this.ForEach(cur => { ret.AddRange(cur.EntityLabel); });
				return ret.ToArray();
			}
		}

		public void GetConnected(List<IRebuild> updated)
		{
			if (updated != null && Affected(updated))
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			if (Guides != null)
				parents.AddRange(Guides);

			if (Bounds.Count > 0)
				parents.AddRange(Bounds);

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
				ret &= (LayoutPanels(null, null, ClothAlignment, Width) > 0);
			return ret;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}
	}

	struct GuideCross
	{
		public GuideCross(double s, Vect2 u, Vect3 x, MouldCurve seam, double sMC)
		{
			sPos = s; uPos = new Vect2(u); xPos = new Vect3(x); Seam = seam; sSeam = sMC;
		}
		//public GuideCross()
		//{
		//	sPos = new double[2];//guide curve s-positions
		//	uPos = new Vect2[2];//cross uv
		//	xPos = new Vect3[2];//cross xyz
		//	Seams = new MouldCurve[2];//cross curves
		//	sSeams = new double[2];//cross curves' s-pos
		//}

		public double sPos;// = new double[2];//guide curve s-positions
		public Vect2 uPos;// = new Vect2[2];//cross uv
		public Vect3 xPos;// = new Vect3[2];//cross xyz
		public MouldCurve Seam;// = new MouldCurve[2];//cross curves
		public double sSeam;// = new double[2];//cross curves' s-pos
	}
}
