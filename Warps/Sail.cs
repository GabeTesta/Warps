using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Warps.Yarns;

namespace Warps
{
	public class Sail
	{
		SurfaceType m_type = SurfaceType.COMBO;

		#region Members

		string m_path;
		ISurface m_mould;
		List<IGroup> m_layout;

		#endregion

		public string FilePath
		{
			get { return m_path; }
		}
		public ISurface Mould
		{
			get { return m_mould; }
			set { m_mould = value; }
		}
		public List<IGroup> Layout
		{
			get { return m_layout; }
			set { m_layout = value; }
		}

		public List<IGroup> FullLayout
		{
			get
			{
				List<IGroup> full = new List<IGroup>();
				if (Mould.Groups != null)
					full.AddRange(Mould.Groups);
				full.AddRange(Layout);
				return full;
			}
		}

		public void ReadFile(string path)
		{
			switch (Path.GetExtension(path).ToLower())
			{
				case ".wrp":
					ReadScriptFile(path);
					m_path = path;
					break;
				default:
					m_layout = new List<IGroup>();//default empty layout
					CreateMould(path);//read the file

					//#if DEBUG
					//					CreateOuterCurves();
					//					CreateInnerCurves();
					//#endif
					m_path = path;
					break;
			}
		}

		void CreateMould(string path)
		{
			CreateMould(null, path);
		}
		void CreateMould(string type, string path)
		{
			if (path == null || path.Length == 0)
			{
				string[] paths = WarpFrame.OpenFileDlg(1);
				if (paths.Length > 0)
					path = paths[0];

				if (path == null || path.Length == 0)
					return;
			}
			if (type == null)
			{
				switch (m_type)
				{
					case SurfaceType.COMBO:
						m_mould = new ComboMould(this, path);
						//(m_mould as ComboMould).ReadCofFile(this, path);
						return;
					case SurfaceType.COF:
						m_mould = new CofMould(this, path);
						//(m_mould as CofMould).ReadCofFile(this, path);
						return;
					case SurfaceType.RBF:
						m_mould = new RBFMould(this, path);
						//(m_mould as RBFMould).ReadCofFile(this, path);
						return;
				}
			}

			if (type.Contains(':'))
				type = type.Split(':')[0];

			m_mould = Utilities.CreateInstance(type, new object[] { this, path }) as ISurface;


		}
		//ISurface CreateMould(string type, string path)
		//{
		//	if (type == null)
		//	{
		//		switch (m_type)
		//		{
		//			case SurfaceType.COMBO:
		//				return new ComboMould(this, path);
		//			case SurfaceType.COF:
		//				return new CofMould(this, path);
		//			case SurfaceType.RBF:
		//				return new RBFMould(this, path);
		//		}
		//	}

		//	if (type.Contains(':'))
		//		type = type.Split(':')[0];

		//	return Utilities.CreateInstance(type, new object[] { this, path }) as ISurface;
		//}

		public event UpdateUI updateStatus;

		void ReadScriptFile(string path)
		{
			using (StreamReader txt = new StreamReader(path))
			{
				string line = txt.ReadLine();
				int cnt = ScriptTools.ReadCount(line);
				if (cnt >= 0)
				{
					if (updateStatus != null)
						updateStatus(-cnt, path);
				}
				line = txt.ReadLine();
	
				CreateMould(ScriptTools.ReadType(line), ScriptTools.ReadPath(line));
				
	

				//read layout
				m_layout = new List<IGroup>();
				line = txt.ReadLine();
				string[] splits;
				while (line != null)
				{
					List<string> lines = new List<string>(ScriptTools.Block(ref line, txt));

					object grp = null;
					splits = lines[0].Split(':');
					if (splits.Length > 0)
						grp = Utilities.CreateInstance(splits[0].Trim('\t'));
					if (grp != null && grp is IGroup)
					{
						(grp as IGroup).Sail = this;
						m_layout.Add(grp as IGroup);

						if (updateStatus != null)
							updateStatus(Layout.Count, "Loading " + lines[0]);
						(grp as IGroup).ReadScript(this, lines);
					}
				}
			}
		}
		public void WriteScriptFile(string path)
		{
			using (StreamWriter txt = new StreamWriter(path))
			{
				//write label, path and layout count
				txt.WriteLine(string.Format("{0} [{1}] <{2}>", Mould.ToString(), FilePath, Layout.Count));
				foreach (string s in Mould.WriteScript())
				{
					txt.WriteLine("\t" + s);
				}
				//txt.WriteLine("\t" + Mould.Script);
				foreach (IGroup grp in Layout)
					foreach (string s in grp.WriteScript())
					{
						txt.WriteLine("\t" + s);
					}
			}
		}
		public override string ToString()
		{
			return Mould.ToString();
		}

		//public List<MouldCurve> GetCurves(object tag)
		//{
		//	bool found = false;
		//	List<MouldCurve> curves = new List<MouldCurve>();
		//	for (int i = 0; i < Layout.Count; i++)
		//	{
		//		if (Layout[i] is CurveGroup)
		//		{
		//			CurveGroup grp = Layout[i] as CurveGroup;
		//			IEnumerable<MouldCurve> mcs = grp.GetCurves(tag);
		//			curves.AddRange(mcs);
		//			if (mcs.Count() != grp.Count)
		//				break;

		//		}
		//		else if (Layout[i] is VariableGroup)
		//		{
		//			VariableGroup grp = Layout[i] as VariableGroup;
		//			found = grp == tag as VariableGroup;
		//			IEnumerable<KeyValuePair<string, Equation>> mcs = grp.GetEquations(tag);
		//			if (mcs.Count() != grp.Count)
		//				break;
		//			if (found)
		//				break;
		//		}
		//		else if (Layout[i] is YarnGroup)
		//		{
		//			YarnGroup grp = Layout[i] as YarnGroup;
		//			found = grp == tag as YarnGroup;
		//			if (found)
		//				break;
		//		}
		//	}
		//	return curves;
		//}

		//public List<KeyValuePair<string, Equation>> GetEquations(object tag)
		//{
		//	bool found = false;
		//	List<KeyValuePair<string, Equation>> equations = new List<KeyValuePair<string, Equation>>();
		//	for (int i = 0; i < Layout.Count; i++)
		//	{
		//		if (Layout[i] is VariableGroup)
		//		{
		//			//continue;
		//			VariableGroup grp = Layout[i] as VariableGroup;
		//			found = grp == tag as VariableGroup;
		//			IEnumerable<KeyValuePair<string, Equation>> mcs = grp.GetEquations(tag);
		//			equations.AddRange(mcs);
		//			if (mcs.Count() != grp.Count)
		//				break;
		//			if (found)
		//				break;
		//		}
		//		else if (Layout[i] is CurveGroup)
		//		{
		//			CurveGroup grp = Layout[i] as CurveGroup;
		//			found = grp == tag as CurveGroup;
		//			IEnumerable<MouldCurve> mcs = grp.GetCurves(tag);
		//			if (mcs.Count() != grp.Count)
		//				break;
		//			if (found)
		//				break;
		//		}
		//		else if (Layout[i] is YarnGroup)
		//		{
		//			YarnGroup grp = Layout[i] as YarnGroup;
		//			found = grp == tag as YarnGroup;
		//			//IEnumerable<KeyValuePair<string, Equation>> mcs = grp.GetEquations(tag);
		//			//if (mcs.Count() != grp.Count)
		//			//	break;
		//			if (found)
		//				break;
		//		}
		//	}
		//	return equations;
		//}

		//public List<MouldCurve> GetAllCurves()
		//{
		//	List<MouldCurve> curves = new List<MouldCurve>();
		//	for (int i = 0; i < Layout.Count; i++)
		//	{
		//		if (Layout[i] is CurveGroup)
		//		{
		//			CurveGroup grp = Layout[i] as CurveGroup;
		//			if (grp == null)
		//				continue;
		//			curves.AddRange(grp.GetAllCurves());
		//		}
		//	}
		//	return curves;
		//}

		//public MouldCurve FindCurve(string name, object tag)
		//{
		//	List<MouldCurve> curves = GetCurves(tag);

		//	return curves.Find(curve => curve.Label == name);
		//}

		public List<IRebuild> Rebuild(IRebuild tag)
		{
			List<IRebuild> updated = null;
			if (tag != null)
			{
				updated = new List<IRebuild>();
				updated.Add(tag);
			}

			foreach (IRebuild item in Layout)
			{
				item.Update(this);// (updated);
			}
			if (tag == null)
				WriteNode();
			return updated;
		}
		public List<IRebuild> GetConnected(IRebuild tag)
		{
			List<IRebuild> connected = null;
			if (tag != null)
			{
				connected = new List<IRebuild>();
				connected.Add(tag);
			}
			foreach (IRebuild item in Layout)
			{
				item.GetConnected(connected);
			}

			return connected;
		}

		TreeNode m_node;
		public TreeNode WriteNode()
		{
			if (m_node == null)
				m_node = new System.Windows.Forms.TreeNode();
			m_node.Text = m_path;
			m_node.Tag = this;
			m_node.ImageKey = m_node.SelectedImageKey = 	m_node.ToolTipText = GetType().Name;
			m_node.Nodes.Clear();
			m_node.Nodes.Add(Mould.WriteNode());
			foreach (IGroup g in Layout)
				m_node.Nodes.Add(g.WriteNode());
			return m_node;
		}
		//public TreeNode WriteNode()
		//{
		//	if (m_node == null)
		//		m_node = new System.Windows.Forms.TreeNode();
		//	m_node.Text = string.Format("{0}: {1}", GetType().Name, m_path);
		//	m_node.Tag = this;
		//	m_node.ImageKey = GetType().Name;
		//	m_node.SelectedImageKey = GetType().Name;
		//	m_node.Nodes.Clear();
		//	var tn = m_node.Nodes.Add(string.Format("{0}: {1}", Mould.GetType().Name, Mould.Label));
		//	tn.ImageKey = GetType().Name;
		//	tn.SelectedImageKey = GetType().Name;
		//	foreach (IGroup g in Layout)
		//		m_node.Nodes.Add(g.WriteNode());
		//	return m_node;
		//}

		internal void Add(IGroup grp)
		{
			if (grp == null)
				throw new ArgumentNullException("grp");
			IGroup found = FindGroup(grp.Label);
			if (found != null)
				return;
			//{
			//	grp.Label += "_Copy";
			//}
			//throw new Exception("Group [" + grp.Label + "] already exists");
			//else
			//{
			grp.Sail = this;

			Layout.Add(grp);
			//Rebuild(null);
			//}

		}

		//internal List<object> GetAutoFillData(object tag)
		//{
		//	List<object> autoComplete = new List<object>();
		//	List<MouldCurve> curves = GetCurves(tag);
		//	curves.ForEach(cur => { autoComplete.Add(cur); });
		//	List<KeyValuePair<string, Equation>> availableEqs = GetEquations(tag);
		//	availableEqs.ForEach(eq => { autoComplete.Add(eq.Value.Label); });
		//	return autoComplete;
		//}

		//internal IGroup GetParentGroup(object tag, out MouldCurve refCurve)
		//{
		//	refCurve = null;
		//	for (int i = Layout.Count - 1; i >= 0; i--)
		//	{
		//		if (Layout[i] is CurveGroup)
		//		{
		//			if ((Layout[i] as CurveGroup).ContainsObject(tag, out refCurve))
		//				return Layout[i];

		//		}
		//		else if (Layout[i] is VariableGroup)
		//		{
		//			if ((Layout[i] as VariableGroup).ContainsKey(tag.ToString()))
		//				return Layout[i];
		//		}
		//	}
		//	return null;
		//}

		//internal IGroup GetParentGroup(object tag)
		//{
		//	for (int i = Layout.Count - 1; i >= 0; i--)
		//	{
		//		if (Layout[i] is CurveGroup)
		//		{
		//			MouldCurve tmp = null;
		//			if ((Layout[i] as CurveGroup).ContainsObject(tag, out tmp))
		//			{
		//				return Layout[i];
		//			}
		//		}
		//		else if (Layout[i] is VariableGroup)
		//		{
		//			if ((Layout[i] as VariableGroup).ContainsKey(tag.ToString()))
		//				return Layout[i];
		//		}
		//	}
		//	return null;
		//}

		internal void Remove(IRebuild tag)
		{
			if (tag is IGroup)
			{
				IGroup g = FindGroup(tag.Label);
				Layout.Remove(g);
			}
		}

		#region Watermark

		/// <summary>
		/// Searches the layout for the specified item.
		/// </summary>
		/// <param name="lbl">the item to find, must be unique name</param>
		/// <returns>the cooresponding IRebuild object, null if failed</returns>
		public IRebuild FindItem(string lbl)
		{
			if (lbl == null || lbl.Length == 0)
				return null;
			IRebuild item = null;
			//loop through groups in reverse
			for (int i = Layout.Count - 1; i >= 0; i--)
			{
				//check group label
				if (Layout[i].Label.Equals(lbl))
					return Layout[i];//return if match

				//search group for item
				item = Layout[i].FindItem(lbl);
				if (item != null)
					return item;//return if match
			}
			//check mould items (if any)
			if (Mould != null && Mould.Groups != null)
				for (int i = 0; i < Mould.Groups.Count; i++)
				{
					//check group label
					if (Mould.Groups[i].Label.Equals(lbl))
						return Mould.Groups[i];//return if match

					//search group for item
					item = Mould.Groups[i].FindItem(lbl);
					if (item != null)
						return item;//return if match
				}
			return null;//null on failure
		}
		public MouldCurve FindCurve(string curve)
		{
			return FindItem(curve) as MouldCurve;

			//for (int i = Layout.Count - 1; i >= 0; i--)
			//{
			//	if (Layout[i] is CurveGroup)
			//	{
			//		MouldCurve cur = (Layout[i] as CurveGroup).Find((MouldCurve m) => { return m.Label.ToLower() == curve.ToLower(); });
			//		if (cur != null)
			//			return cur;
			//	}
			//}
			//return null;
		}
		public Equation FindEquation(string variable)
		{
			return FindItem(variable) as Equation;
		}
		public IGroup FindGroup(string group)
		{
			return FindItem(group) as IGroup;
		}

		/// <summary>
		/// Finds the parent group of the specified tag
		/// </summary>
		/// <param name="tag">the item to find the parent of</param>
		/// <returns>the containing IGroup, null if not found</returns>
		public IGroup FindGroup(IRebuild tag)
		{
			if (tag == null)
				return null;
			List<IRebuild> rets = new List<IRebuild>();
			for (int i = Layout.Count - 1; i >= 0; i--)
			{
				if (Layout[i].Watermark(tag, ref rets))
					return Layout[i];
			}
			if (Mould != null && Mould.Groups != null)
				for (int i = 0; i < Mould.Groups.Count; i++)
				{
					//check group label
					if (Mould.Groups[i].Watermark(tag, ref rets))
						return Mould.Groups[i];//return if match
				}

			return null;
		}

		/// <summary>
		/// Returns all IRebuild items that are above the tag
		/// </summary>
		/// <param name="tag">the item being used as a watermark, can be null</param>
		/// <returns>a List of IRebuild items</returns>
		public List<IRebuild> Watermark(IRebuild tag)
		{
			List<IRebuild> rets = new List<IRebuild>();

			//watermark mould first (if any)
			if (Mould != null && Mould.Groups != null)
				for (int i = 0; i < Mould.Groups.Count; i++)
				{
					if (Mould.Groups[i].Watermark(tag, ref rets))
						break;//break on finding tag
				}

			//watermark layout
			for (int i = 0; i < Layout.Count; i++)
			{
				if (Layout[i].Watermark(tag, ref rets))
					break;//break on finding tag
			}

			return rets;
		}

		/// <summary>
		/// Returns all IRebuild items that are above the tag
		/// </summary>
		/// <param name="tag">the item being used as a watermark, can be null</param>
		/// <param name="Parent">the IGroup the item belongs too (REQUIRED IF YOU ARE ADDING A CURVE)</param>
		/// <returns></returns>
		public List<IRebuild> Watermark(IRebuild tag, IGroup Parent)
		{
			List<IRebuild> rets = new List<IRebuild>();

			//watermark mould first (if any)
			if (Mould != null && Mould.Groups != null)
				for (int i = 0; i < Mould.Groups.Count; i++)
				{
					if (Mould.Groups[i].Watermark(tag, ref rets))
						break;//break on finding tag
				}

			int index = -1;

			if (Parent != null)
				index = Layout.IndexOf(Parent); // if the Parent is not null, then don't allow the watermark to go below this group.

			//watermark layout
			for (int i = 0; i < Layout.Count; i++)
			{
				if (index > -1)
				{
					if (i <= index)
						if (Layout[i].Watermark(tag, ref rets))
							break;//break on finding tag

				}
				else if (Layout[i].Watermark(tag, ref rets))
					break;//break on finding tag

			}

			return rets;
		}
		public List<Equation> WatermarkEqs(IRebuild tag)
		{
			List<Equation> eqs = new List<Equation>();
			foreach (IRebuild rb in Watermark(tag))
			{
				if (rb is Equation)
					eqs.Add(rb as Equation);
			}
			return eqs;
		}
		public List<MouldCurve> WatermarkCur(IRebuild tag)
		{
			List<MouldCurve> eqs = new List<MouldCurve>();
			foreach (IRebuild rb in Watermark(tag))
			{
				if (rb is MouldCurve)
					eqs.Add(rb as MouldCurve);
			}
			return eqs;
		}

		#endregion


		#region Default Geometry Shit

		void CreateSpokes()
		{
			const int NGIR = 5;
			const int NANG = 5;
			Vect2[] uv = new Vect2[2];
			CurveGroup group;
			//for (int nLu = 0; nLu < 1; nLu++)
			for (int nGir = 0; nGir < NGIR; nGir++)
			{
				group = new CurveGroup(string.Format("Spokes[{0}]", nGir), this);
				//group = new CurveGroup(string.Format("Spokes[{0}][{1}]", nLu, nGir), S);

				uv[0] = new Vect2(-0.3, BLAS.interpolant(nGir, NGIR));
				//uv[1-nLu] = new Vect2(1,0);

				for (int nAng = 0; nAng < NANG; nAng++)
				{
					uv[1] = new Vect2(1.3, BLAS.interpolant(nAng, NANG));

					MouldCurve g = new MouldCurve(string.Format(group.Label + "[{0}]", nAng), this, null);
					g.Fit(uv[0], uv[1]);
					group.Add(g);
				}
				Add(group);
			}

		}

		public IGroup CreateOuterCurves()
		{
			if (FindGroup("Outer") != null)
				return FindGroup("Outer");

			CurveGroup outer = new CurveGroup("Outer", this);
			//int nLayer = View.AddLayer("Outer", Color.MistyRose, true);
			FixedPoint[] corners = new FixedPoint[4];
			corners[0] = new FixedPoint(0, 0);
			corners[1] = new FixedPoint(0, 1);
			corners[2] = new FixedPoint(1, 1);
			corners[3] = new FixedPoint(1, 0);
			FixedPoint middle = new FixedPoint(0.5, 0.5);

			FixedPoint[] lu = new FixedPoint[] { new FixedPoint(corners[0]), new FixedPoint(corners[1]) };
			FixedPoint[] hd = new FixedPoint[] { new FixedPoint(corners[1]), new FixedPoint(corners[2]) };
			FixedPoint[] le = new FixedPoint[] { new FixedPoint(corners[3]), new FixedPoint(corners[2]) };
			FixedPoint[] ft = new FixedPoint[] { new FixedPoint(corners[0]), new FixedPoint(corners[3]) };


			outer.Add(new MouldCurve("Luff", this, lu));
			outer.Add(new MouldCurve("Leech", this, le));
			outer.Add(new MouldCurve("Head", this, hd));
			outer.Add(new MouldCurve("Foot", this, ft));

			Add(outer);

			return outer;
			//LinearPath[] paths = new LinearPath[4];
			//Entity[] es;
			//for (int i = 0; i < 4; i++)
			//{
			//	Color c = Color.FromArgb(255 * (i % 2), 122 * (i % 3), 85 * i);
			//	//	LinearPath p = new LinearPath(GetPathPoints(outer[i], 100));
			//	//p.LayerIndex = nLayer;
			//	es = outer[i].CreateEntity(false);
			//	foreach (Entity e in es)
			//	{
			//		e.LayerIndex = nLayer;
			//		View.Add(e);
			//	}
			//	//m_viewleft.Entities.Add(new PointCloud(GetPoints(outer[i].xFits), 5f, Color.Tomato));

			//}
			//s.Layout.Add(outer);

			////SurfaceCurve gir = null;
			//for (int i = 0; i < 2; i++)
			//{
			//	CurveGroup girths = new CurveGroup(i == 0 ? "Girths" : "Virths", s);
			//	nLayer = View.AddLayer(girths.Label, i == 0 ? Color.SteelBlue : Color.IndianRed, i == 0);
			//	for (double dg = 0.0; dg < 1; dg += 0.1)
			//	{
			//		IFitPoint[] gir = new IFitPoint[2];
			//		gir[1] = new CurvePoint(1, outer[2 * i + 1], dg);

			//		//gir[1] = new FixedPoint(.3, .3);
			//		if (i == 0)
			//			gir[0] = new SlidePoint(0, outer[2 * i], dg);
			//		else
			//			gir[0] = new CurvePoint(0, outer[2 * i], dg);

			//		Geodesic girth = new Geodesic(String.Format("{0}ir-{1:##0}%", i == 0 ? "G" : "V", dg * 100), s, gir);
			//		if (girth.xGeos != null)
			//		{
			//			//gir = girth;
			//			//m_viewleft.Entities.Add(new LinearPath(GetPathPoints(girth, 100), Color.DarkGoldenrod));
			//			//LinearPath p = new LinearPath(GetPathPoints(girth, 100));
			//			//p.LayerIndex = nLayer;
			//			es = girth.CreateEntity(false);
			//			foreach (Entity e in es)
			//			{
			//				e.LayerIndex = nLayer;
			//				View.Add(e);
			//			}
			//			//m_viewleft.Entities.Add(new PointCloud(GetPoints(girth.xGeos), 5f, Color.Tomato));
			//			girths.Add(girth);
			//		}
			//	}
			//	s.Layout.Add(girths);
			//}

			//CurveGroup mid = new CurveGroup("Mids", s);
			//nLayer = View.AddLayer("Mids", Color.GreenYellow, true);
			//SurfaceCurve x = new SurfaceCurve("CtoH", s, new IFitPoint[] { corners[1], middle, new CurvePoint(1, outer["Leech"], 0.0) });
			//mid.Add(x);
			//es = x.CreateEntity(false);
			//foreach (Entity e in es)
			//{
			//	e.LayerIndex = nLayer;
			//	View.Add(e);
			//}

			//s.Layout.Add(mid);

		}

		IGroup[] CreateInnerGirths()
		{
			CurveGroup outer = FindGroup("Outer") as CurveGroup;
			if (outer == null)
				outer = CreateOuterCurves() as CurveGroup;

			IGroup[] girvir = new IGroup[2];
			//SurfaceCurve gir = null;
			for (int i = 0; i < 2; i++)
			{
				CurveGroup girths = new CurveGroup(i == 0 ? "Girths" : "Virths", this);
				for (double dg = 0.0; dg < 1; dg += 0.1)
				{
					IFitPoint[] gir = new IFitPoint[2];
					gir[1] = new CurvePoint(1, outer[2 * i + 1], dg);

					//gir[1] = new FixedPoint(.3, .3);
					if (i == 0)
						gir[0] = new SlidePoint(0, outer[2 * i], dg);
					else
						gir[0] = new CurvePoint(0, outer[2 * i], dg);

					MouldCurve girth = new MouldCurve(String.Format("{0}ir-{1:##0}%", i == 0 ? "G" : "V", dg * 100), this, null);
					girth.Fit(gir);
					girths.Add(girth);
				}
				Add(girths);
				girvir[i] = girths;
			}
			return girvir;
		}

		IGroup[] CreateInnerCurves()
		{
			CurveGroup outer = FindGroup("Outer") as CurveGroup;
			if (outer == null)
				outer = CreateOuterCurves() as CurveGroup;

			IGroup[] rets = new IGroup[2];
			//SurfaceCurve gir = null;
			for (int i = 0; i < 2; i++)
			{
				CurveGroup curves = new CurveGroup(i == 0 ? "Horiz" : "Verts", this);
				for (double dg = 0.0; dg < 1; dg += 0.1)
				{
					IFitPoint[] gir = new IFitPoint[2];
					gir[1] = new CurvePoint(1, outer[2 * i + 1], dg);

					//gir[1] = new FixedPoint(.3, .3);

					gir[0] = new CurvePoint(0, outer[2 * i], dg);

					MouldCurve curve = new MouldCurve(String.Format("{0}ec-{1:##0}%", i == 0 ? "S" : "V", dg * 100), this, null);
					curve.Fit(gir);
					curves.Add(curve);
				}
				Add(curves);
				rets[i] = curves;
			}
			return rets;
		}

		#endregion


	}
}
