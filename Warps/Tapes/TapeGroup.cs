using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warps;
using Warps.Curves;
using devDept.Eyeshot.Entities;
using Warps.Yarns;

namespace Warps.Tapes
{
	public class TapeGroup : List<Tape>, IGroup
	{
		public TapeGroup()
		{
		}
		public TapeGroup(string label, List<MouldCurve> warps, GuideSurface uvDens, double pLen, double chainTol, double angTol)
		{
			m_label = label;
			m_warps = new List<MouldCurve>(warps);
			m_densitymap = uvDens;
			m_pixlen = pLen;
			m_chainTol = chainTol;
			m_angleTol = angTol;
		}

		public TapeGroup(TapeGroup copy)
		{
			m_label = copy.Label;
			m_sail = copy.Sail;
			Fit(copy);
		}

		public bool IsEqual(TapeGroup g)
		{
			if (g == null)
				return false;

			return g.m_angleTol == this.m_angleTol
				&& g.m_bStagger == this.m_bStagger
				&& g.m_chainTol == this.m_chainTol
				&& g.m_densitymap == this.m_densitymap
				&& g.m_label == this.m_label
				&& g.m_pixlen == this.m_pixlen
				&& g.m_warps == this.m_warps;
		}

		public void WriteBin(System.IO.BinaryWriter bin) { }

		void LayoutPixelsSync(Sail s)
		{
			Clear();
			//create base comb to generate evenly spaced yarns
			List<IFitPoint> combpts = new List<IFitPoint>(Warps.Count);
			Warps.ForEach(w => combpts.Add(new CurvePoint(w, 0.5)));
			//GuideComb comb = new GuideComb("PixelComb", s, combpts.ToArray(), new Vect2[]{ new Vect2(0,1), new Vect2(1,1)});

			//create yarn group for pixelation
			YarnGroup baseyarns = new YarnGroup("Pixels", s, 1);
			if (baseyarns.LayoutEvenYarns(Warps, 1000) < 0)//create 500 base yarns to start with
				throw new Exception("Failed to generate base yarns for " + Label);

			int i = 0;
			//pixels and active/inactive
			//List<SegmentCurve>[] threadpix = new List<SegmentCurve>[baseyarns.Count];
			List<List<SegmentCurve>> pixels = new List<List<SegmentCurve>>(baseyarns.Count);
			//DateTime t = DateTime.Now;
			//Parallel.ForEach(baseyarns, yarn =>
			//{
			//	i = baseyarns.IndexOf(yarn);
			//	threadpix[i] = PixelateYarn(yarn);
			//});
			//List<List<SegmentCurve>> pixels = threadpix.ToList();
			//TimeSpan mtd =	t - DateTime.Now;
			//pixels.Clear();
			//t = DateTime.Now;
			baseyarns.ForEach(yarn => pixels.Add(PixelateYarn(yarn)));
			//TimeSpan std = t - DateTime.Now;
			//double ratio = (double)mtd.Ticks / (double)std.Ticks;

			int nMaxPix = 0;
			//find maximum pixel count
			//Parallel.ForEach(pixels, pix => nMaxPix = Math.Max(nMaxPix, pix.Count));

			pixels.ForEach(pYar => nMaxPix = Math.Max(nMaxPix, pYar.Count));
			bool[,] active = new bool[pixels.Count, nMaxPix];//array of active pixels

			//int nYar = 0;
			//starting with first pixel bracket check neighbors for dpi	
			for (i = 0; i < nMaxPix; i++)
			{
				DeactivatePixelBracket(i, pixels, active);
			}

			//Parallel.For(0, nMaxPix, nBrak => DeactivatePixelBracket(nBrak, pixels, active));

			//chain pixels together
			List<SegmentCurve> chains = new List<SegmentCurve>();
			ChainCurve chain;
			Vect3 xBase = new Vect3(), xTest = new Vect3();
			Vect2 uBase = new Vect2(), uTest = new Vect2();
			int nYar = 0;
			int nPix = 0;
			bool bfound;
			double pPixel, pAve;
			m_totalLength = 0;//reset total tape length
			for (nPix = 0; nPix < nMaxPix; nPix++)//loop through each pixelbracket
				for (nYar = 0; nYar < pixels.Count; nYar++)//sidestep to find starting pixels
				{
					if (!active[nYar, nPix]) continue;//sidestep until first active pixel

					chain = new ChainCurve();//initialize chain with starting pix
					chain.Add(pixels[nYar][nPix]);
					active[nYar, nPix] = false;//deactivate chained sections
					pPixel = baseyarns.BracketToGlobal(baseyarns[nYar]);//get the base p-value
					bfound = true;//start true for each pixel

					for (int j = 1; j < (nMaxPix - nPix); j++)//chain along the yarns
					{
						if (!bfound) break; //quit if previous bracket had no chain
						bfound = false; //start search unfound
						for (i = 0; i < pixels.Count; i++)//sidestep through bracket to find possible chain segment
						{
							if (!active[i, nPix + j] || pixels[i].Count < (nPix + j)) continue;//skip inative pixels

							pixels[i][nPix + j].xVal(0, ref uTest, ref xTest);//get the test point's u and x
							baseyarns.xVal(pPixel, pixels[i][nPix + j].Min, ref uBase, ref xBase);//slide the base point's u and x up to the test points s-pos

							//check distance
							if (xBase.Distance(xTest) < m_chainTol)
							//if (Math.Abs(pPixel - baseyarns.BracketToGlobal(baseyarns[i])) < m_chainTol)
							{
								chain.Add(pixels[i][nPix + j]);	//add chain
								active[i, nPix + j] = false; //deactivate chained sections
								bfound = true;
								break;//step to next bracket
							}
						}
					}
					//calculate averaged p-value
					pAve = 0;
					chain.Curves.ForEach(yarnseg => pAve += baseyarns.BracketToGlobal(yarnseg.Curve as YarnCurve));
					pAve /= chain.Curves.Count;
					//span new yarn with ave-p
					//create new segment curve using extreme s-limits
					//tape this curve
					m_totalLength += TapeCurve(s, new SegmentCurve(baseyarns.MakeYarn(pAve), chain.Curves[0].Min, chain.Curves[chain.Curves.Count - 1].Max), nPix);
					//TapeCurve(new SegmentCurve(baseyarns[nYar], chain.Curves[0].Min, chain.Curves[chain.Curves.Count - 1].Max), nPix);
				}
		}

		void LayoutPixels(Sail s)
		{
			Clear();
			if (Warps == null || DensityMap == null)
				return;
			//create base comb to generate evenly spaced yarns
			//List<IFitPoint> combpts = new List<IFitPoint>(Warps.Count);
			//Warps.ForEach(w => combpts.Add(new CurvePoint(w, 0.5)));
			//GuideComb comb = new GuideComb("PixelComb", s, combpts.ToArray(), new Vect2[]{ new Vect2(0,1), new Vect2(1,1)});

			//create yarn group for pixelation
			YarnGroup baseyarns = new YarnGroup("Pixels", s, 1);
			if (baseyarns.LayoutEvenYarns(Warps, 500) < 0)//create 500 base yarns to start with
				throw new Exception("Failed to generate base yarns for " + Label);

			int i = 0;
			//pixels and active/inactive
			List<SegmentCurve>[] threadpix = new List<SegmentCurve>[baseyarns.Count];
			//List<List<SegmentCurve>> pixels = new List<List<SegmentCurve>>(baseyarns.Count);
			//DateTime t = DateTime.Now;
			Parallel.ForEach(baseyarns, yarn =>
				{
					i = baseyarns.IndexOf(yarn);
					threadpix[i] = PixelateYarn(yarn);
				});
			List<List<SegmentCurve>> pixels = threadpix.ToList();
			//TimeSpan mtd =	t - DateTime.Now;
			//pixels.Clear();
			//t = DateTime.Now;
			//baseyarns.ForEach(yarn => pixels.Add(PixelateYarn(yarn)));
			//TimeSpan std = t - DateTime.Now;
			//double ratio = (double)mtd.Ticks / (double)std.Ticks;
			
			int nMaxPix = 0;
			//find maximum pixel count
			Parallel.ForEach(pixels, pix => nMaxPix = Math.Max(nMaxPix, pix.Count));
			
			//pixels.ForEach(pYar => nMaxPix = Math.Max(nMaxPix, pYar.Count));
			bool[,] active = new bool[pixels.Count, nMaxPix];//array of active pixels

			//int nYar = 0;
			//starting with first pixel bracket check neighbors for dpi	
			//for (i = 0; i < nMaxPix; i++)
			//{
			//	DeactivatePixelBracket(i, pixels, active);
			//}

			Parallel.For(0, nMaxPix, nBrak => DeactivatePixelBracket(nBrak, pixels, active));

			//chain pixels together
			List<SegmentCurve> chains = new List<SegmentCurve>();
			ChainCurve chain;
			Vect3 xBase = new Vect3(), xTest = new Vect3();
			Vect2 uBase = new Vect2(), uTest = new Vect2();

			int nYar = 0;
			int nPix = 0;
			bool bfound;
			double pPixel, pAve, pDel;
			m_totalLength = 0;//reset total tape length
			for(nPix = 0; nPix < nMaxPix; nPix++)//loop through each pixelbracket
				for (nYar = 0; nYar < pixels.Count; nYar++)//sidestep to find starting pixels
				{
					if (!active[nYar, nPix]) continue;//sidestep until first active pixel

					chain = new ChainCurve();//initialize chain with starting pix
					chain.Add(pixels[nYar][nPix]);
					active[nYar, nPix] = false;//deactivate chained sections
					pPixel = baseyarns.BracketToGlobal(baseyarns[nYar]);//get the base p-value
					bfound = true;//start true for each pixel

					for (int j = 1; j < (nMaxPix - nPix); j++)//chain along the yarns
					{
						if (!bfound) break; //quit if previous bracket had no chain
						bfound = false; //start search unfound
						for (i = 0; i < pixels.Count; i++)//sidestep through bracket to find possible chain segment
						{
							if (!active[i, nPix + j] || pixels[i].Count <= (nPix + j)) continue;//skip inative pixels

							pixels[i][nPix + j].xVal(0.5, ref uTest, ref xTest);//get the test point's u and x
							baseyarns.xVal(pPixel, pixels[i][nPix + j].Mid, ref uBase, ref xBase);//slide the base point's u and x up to the test points s-pos

							//check distance
							pDel = xBase.Distance(xTest);

							//check distance
							//pDel = Math.Abs(pPixel - baseyarns.BracketToGlobal(baseyarns[i]));

							if (pDel <= m_chainTol)
							{
								////check next point to ensure closest
								//if (i < pixels.Count - 2)
								//{
								//	pAve = Math.Abs(pPixel - baseyarns.BracketToGlobal(baseyarns[i + 1]));
								//	while (i < pixels.Count - 2 && pAve < pDel )
								//	{
								//		i++;//step forward one if it's closer
								//		if (!active[i, nPix + j] || pixels[i].Count <= (nPix + j)) continue;//skip inative pixels

								//		pDel = pAve;
								//		pAve = Math.Abs(pPixel - baseyarns.BracketToGlobal(baseyarns[i + 1]));
								//	}
								//}
								chain.Add(pixels[i][nPix + j]);	//add chain
								active[i, nPix + j] = false; //deactivate chained sections to avoid repeats
								bfound = true;
								break;//step to next bracket
							}
						}
					}
					//calculate averaged p-value
					pAve = 0;
					chain.Curves.ForEach(yarnseg => pAve += baseyarns.BracketToGlobal(yarnseg.Curve as YarnCurve));
					pAve /= chain.Curves.Count;
					//span new yarn with ave-p
					//create new segment curve using extreme s-limits
					//tape this curve
					m_totalLength += TapeCurve(s, new SegmentCurve(baseyarns.MakeYarn(pAve), chain.Curves[0].Min, chain.Curves[chain.Curves.Count - 1].Max), nPix);
					//TapeCurve(new SegmentCurve(baseyarns[nYar], chain.Curves[0].Min, chain.Curves[chain.Curves.Count - 1].Max), nPix);
				}
		}

		private List<SegmentCurve> PixelateYarn(YarnCurve yarn)
		{
			List<SegmentCurve> pixs = new List<SegmentCurve>();
			//Vect3[] pts = yarn.GetPathPoints(100);

			////get the yarn length
			int i;
			//double len = 0;
			//for (i = 1; i < pts.Length; i++)
			//	len += pts[i].Distance(pts[i - 1]);

			double sPix = PixelLength / yarn.Length;//determine pixel spacing in s
			for (i = 1; (double)i * sPix < 1.0; i++)
				pixs.Add(new SegmentCurve(yarn, (i - 1) * sPix, i * sPix));//create pixel segments
			pixs.Add(new SegmentCurve(yarn, (i - 1) * sPix, 1));//final segment

			return pixs;
		}

		private void DeactivatePixelBracket(int i, List<List<SegmentCurve>> pixels, bool[,] active)
		{
			double[] densWid = new double[3];

			Vect3 pixMid = new Vect3();
			Vect3 nexMid = new Vect3();
			Vect2 uvMid = new Vect2();
			int nYar = 0;//start on first pixel
			bool bStagger = i % 2 == 0 && m_bStagger;
			while (pixels[nYar].Count <= i && nYar < pixels.Count)
				nYar++;
			//if (nYar >= pixels.Count)
			//	continue;
			if (nYar < pixels.Count)
			{
				active[nYar, i] = !bStagger;//first pixel is active
				pixels[nYar][i].xVal(0.5, ref uvMid, ref pixMid);//first pixel reference point
				densWid[0] = uvMid[0]; densWid[1] = uvMid[1];
				m_densitymap.Surf.Value(ref densWid);//get the target density at this point
				densWid[2] = 1.0 / densWid[2];
				if (bStagger) densWid[2] /= 2;//half step the first step if staggering
				for (int j = nYar + 1; j < pixels.Count; j++)
				{
					if (i >= pixels[j].Count)
						continue;
					pixels[j][i].xVal(0.5, ref uvMid, ref nexMid);
					if (pixMid.Distance(nexMid) > densWid[2])
					{
						active[j, i] = true;//activate pixel
						pixMid.Set(nexMid);//update refernce pixel
						densWid[0] = uvMid[0]; densWid[1] = uvMid[1];
						m_densitymap.Surf.Value(ref densWid);//update the target density at this point
						densWid[2] = 1.0 / densWid[2];
					}
					else
						active[j, i] = false;//deactivate pixel
				}
			}
		}

		double TapeCurve(Sail sail, IMouldCurve curve, int nPix)
		{

			double INC = 0.01;
			double sStep = INC;//10% step
			double sStart = 0;
			double alpha;
			double len = 0;

			Vect2 uv0 = new Vect2();
			Vect2 uv1 = new Vect2();

			Vect3 x0 = new Vect3();
			Vect3 dx0 = new Vect3();
			Vect3 x1 = new Vect3();
			Vect3 dx1 = new Vect3();
			Vect3 xNor = new Vect3();

			for (int nNwt = 0; nNwt < 100; nNwt++)
			{

				curve.xVec(sStart, ref uv0, ref x0, ref dx0);
				curve.xVec( Math.Min(1.0,sStart+sStep), ref uv1, ref x1, ref dx1);

				sail.Mould.xNor(uv0, ref x0, ref xNor);

				alpha = (dx0.Cross(dx1)).Dot(xNor);
				alpha /= (dx0.Magnitude * dx1.Magnitude * xNor.Magnitude);
				Utilities.LimitRange(-1, ref alpha, 1);
				alpha = Math.Asin(alpha);//inplane angle change

				if (  BLAS.IsGreaterEqual(Math.Abs(alpha) , m_angleTol) || BLAS.IsGreaterEqual(sStart + sStep, 1))//angle tolerance exceeded, cut tape
				{
					MouldCurve cl = new MouldCurve("Centerline " + Count.ToString(), Sail, uv0, uv1);
					Add(new Tape(cl, nPix));
					len += cl.Length;
					if (BLAS.IsGreaterEqual(sStart + sStep, 1))
						break;//stop at end
					sStart += sStep;//set new starting point
					sStep = INC;//reset stepsize
				}
				else
					sStep += INC;//increment step

			}
			return len;
			//MouldCurve gir = new MouldCurve("Gir" + Count.ToString(), Sail, uv0, uv1);
			//Add(new Tape(gir, nPix));
		}

		#region Members

		string m_label;
		Sail m_sail;

		List<MouldCurve> m_warps = new List<MouldCurve>();
		GuideSurface m_densitymap;

		double m_pixlen;
		double m_angleTol = 3 * Math.PI / 180.0; //3 degree tolerance by default
		double m_chainTol = 0.05;

		double m_totalLength = 0;

		bool m_bStagger = false;

		public bool Stagger
		{
			get { return m_bStagger; }
			set { m_bStagger = value; }
		}

		public List<MouldCurve> Warps { get { return m_warps; } }
		public GuideSurface DensityMap { get { return m_densitymap; } set { m_densitymap = value; } }

		public double PixelLength
		{
			get { return m_pixlen; }
			set { m_pixlen = value; }
		}
		public double ChainTolerance
		{
			get { return m_chainTol; }
			set { m_chainTol = value; }
		}
		public double AngleTolerance
		{
			get { return m_angleTol; }
			set { m_angleTol = value; }
		}

		public double TotalLength
		{
			get { return m_totalLength; }
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
			//nothing to search for in a yarn group
			return null;
		}

		public bool ContainsItem(IRebuild obj)
		{
			//nothing to search for in a tape group
			return false;
		}

		public bool Watermark(IRebuild tag, ref List<IRebuild> rets)
		{
			if( object.ReferenceEquals(tag , this) )
				return true;
			//no IRebuilds in a yarn group either (except combs which we ignore)
			return false;
		}

		public bool FindParent<T>(IRebuild item, out T parent) where T : class, IGroup
		{
			if (ContainsItem(item))
			{
				parent = this as T;
				return true;
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
			get { return "Tapes"; }
		}

		public List<string> WriteScript()
		{
			return new List<string>() { ScriptTools.Label(GetType().Name, Label) };
		}

		public bool ReadScript(Sail sail, IList<string> txt)
		{
			throw new NotImplementedException();
		}

		public bool Locked
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal TreeNode m_node;
		public TreeNode WriteNode()
		{
			TabTree.MakeNode(this, ref m_node);

			m_node.Nodes.Clear();

			if (m_warps != null)
			{
				TreeNode wrps = m_node.Nodes.Add("Warps");
				wrps.Tag = this;
				wrps.ImageKey = wrps.SelectedImageKey = "Warps";
				TreeNode wrpnode;
				foreach (MouldCurve wrp in m_warps)
				{
					//clone wardp node
					wrpnode = wrp.WriteNode().Clone() as TreeNode;
					wrpnode.Nodes.Clear();//remove its children
					wrps.Nodes.Add(wrpnode);
					//wrpnode = wrps.Nodes.Add(wrp.WriteNode().Text);
					//wrpnode.ImageKey = wrpnode.SelectedImageKey = wrp.GetType().Name;
				}
			}

			if (DensityMap != null)
			{
				//TreeNode dens = m_node.Nodes.Add("Density Map");
				//dens.ImageKey = dens.SelectedImageKey = "Surface";

				m_node.Nodes.Add(DensityMap.WriteNode().Clone() as TreeNode);
			}

			TreeNode tapes = m_node.Nodes.Add("Tapes: " + Count.ToString());
			tapes.ImageKey = tapes.SelectedImageKey = "Result";
			tapes.Tag = this;
			for (int i = 0; i < Count; i++)
				tapes.Nodes.Add(String.Format(Label + "[{0}] {1}",
					i.ToString(Count > 99 ? "000" : Count > 9 ? "00" : "0"), //index
					this[i].ToString()));//yarn string


			return m_node;
		}

		public List<Entity> CreateEntities()
		{
			List<Entity> ents = new List<Entity>(Count);
			this.ForEach(t =>
			{
				ents.AddRange(t.CreateEntities());
				ents.Last().EntityData = this;
			});
			//ents.Add(CreateDensitySurf());
			return ents;
		}

		//public Entity CreateDensitySurf()
		//{
		//	int[] MESH = new int[] { 25, 30 };
		//	double SCALE = 1;
		//	Vect2 uv = new Vect2();
		//	Vect3 xyz = new Vect3();
		//	Vect3 nor = new Vect3();
		//	double[] rbf = new double[3];
		//	Vect3[,] mesh = new Vect3[MESH[0], MESH[1]];
		//	double[,] color = new double[MESH[0], MESH[1]];
		//	for (int i = 0; i < MESH[0]; i++)
		//	{
		//		uv[0] = rbf[0] = BLAS.interpolant(i, MESH[0]);
		//		for (int j = 0; j < MESH[1]; j++)
		//		{
		//			uv[1] = rbf[1] = BLAS.interpolant(j, MESH[1]);
		//			m_densitymap.Surf.Value(ref rbf);
		//			Sail.Mould.xNor(uv, ref xyz, ref nor);
		//			mesh[i, j] = xyz + (nor * rbf[2] * SCALE);
		//			color[i,j] = rbf[2];
		//		}
		//	}
		//	Mesh m = SurfaceTools.GetMesh(mesh,color);
		//	m.EntityData = this;
		//	return m;
		//}

		public List<devDept.Eyeshot.Labels.Label> EntityLabel
		{
			get { return new List<devDept.Eyeshot.Labels.Label>(); }
		}

		public void GetChildren(List<IRebuild> updated)
		{
			if (Affected(updated) && updated != null)
				updated.Add(this);
		}

		public void GetParents(Sail s, List<IRebuild> parents)
		{
			//throw new NotImplementedException();
			foreach (var w in Warps)
			{
				parents.Add(w);
				w.GetParents(s, parents);
			}
			parents.Add(DensityMap);
			DensityMap.GetParents(s, parents);	
		}

		public bool Affected(List<IRebuild> connected)
		{
			bool bupdate = connected == null;
			if (!bupdate)
			{
				foreach (MouldCurve warp in m_warps)
				{
					bupdate |= connected.Contains(warp);
					foreach (IRebuild irb in connected)
					{
						if (irb is IGroup)
						{
							bupdate |= (irb as IGroup).ContainsItem(warp);
						}
					}
				}
				if (DensityMap != null)
					bupdate |= connected.Contains(DensityMap);
			}
			return bupdate;
		}

		public bool Update(Sail s)
		{
			//DateTime std = DateTime.Now;
			//LayoutPixelsSync(s);
			//double tic = DateTime.Now.Ticks - std.Ticks;
			//std = DateTime.Now;
			if (Sail == null)
				Sail = s;
			LayoutPixels(s);
			//double t2 = DateTime.Now.Ticks - std.Ticks;
			//double ratio = t2 / tic;
			//MessageBox.Show(string.Format("Threaded: [{0}]\nSingled: [{1}]\nRatio: [{2}]", t2, tic, ratio));
			return Count > 0;
		}

		public bool Delete()
		{
			throw new NotImplementedException();
		}

		#endregion

		internal void Fit(TapeGroup temp)
		{
			m_warps = temp.Warps;
			m_densitymap = temp.m_densitymap;
			m_angleTol = temp.m_angleTol;
			m_chainTol = temp.m_chainTol;
			m_pixlen = temp.m_pixlen;
			m_totalLength = temp.m_totalLength;
			//copy tapes
			Clear();
			this.AddRange(temp);
			//udpate treenode
			WriteNode();
		}

		#region IRebuild Members


		public System.Xml.XmlNode WriteXScript(System.Xml.XmlDocument doc)
		{
			return NsXml.MakeNode(doc, this);
		}

		public void ReadXScript(Sail sail, System.Xml.XmlNode node)
		{
			Label = NsXml.ReadLabel(node);
			m_sail = sail;
		}

		#endregion

		#region TreeDragging Members

		public bool CanInsert(Type item)
		{
			return false;
		}

		public void Insert(IRebuild item, IRebuild target)
		{
			throw new NotImplementedException("Cannot Insert into TapeGroup");
			//int nTar = IndexOf(target as MouldCurve);
			//int nIrb = IndexOf(item as MouldCurve);
			//if (nIrb >= 0)//item is already in this group: reorder
			//	Remove(item);
			//Insert(nTar, item as MouldCurve);
		}

		public bool Remove(IRebuild item)
		{
			throw new NotImplementedException("Cannot Remove from TapeGroup");
			//Remove(item as MouldCurve);
		}

		#endregion


		#region Flattening Members

		public void FlatLayout(List<IRebuild> flat)
		{
			//ForEach(cur => flat.Add(cur));
		}

		#endregion
	}
}
