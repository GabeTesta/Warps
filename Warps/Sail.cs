﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using devDept.Eyeshot;
using devDept.Geometry;
using Warps.Curves;
using Warps.Yarns;
using Warps.Tapes;

namespace Warps
{
	public class Sail
	{

		internal SurfaceType m_type = SurfaceType.COMBO;

		#region Members

		string m_path;
		ISurface m_mould;
		List<IRebuild> m_layout = new List<IRebuild>();


		#endregion

		public string FilePath
		{
			get { return m_path; }
		}
		public string Label { get { return Path.GetFileNameWithoutExtension(FilePath); } }
		public ISurface Mould
		{
			get { return m_mould; }
			set { m_mould = value; }
		}
		public List<IRebuild> Layout
		{
			get { return m_layout; }
			set { m_layout = value; }
		}

		public List<IRebuild> FullLayout
		{
			get
			{
				List<IRebuild> full = new List<IRebuild>();
				if (Mould.Groups != null)
					full.AddRange(Mould.Groups);
				full.AddRange(Layout);
				return full;
			}
		}

		public List<IRebuild> FlatLayout()
		{
			List<IRebuild> flat = new List<IRebuild>();
			if (Mould.Groups != null)
				Mould.Groups.ForEach(grp =>
				{
					flat.Add(grp);
					grp.FlatLayout(flat);
				});
			Layout.ForEach(irb =>
			{
				flat.Add(irb);
				if (irb is IGroup)
					(irb as IGroup).FlatLayout(flat);

			});
			return flat;
		}
		public bool ReadFile(string path)
		{
			m_path = path;
			switch (Path.GetExtension(path).ToLower())
			{
				case ".wrp":
				case ".xml":
					return ReadXScript(path);
				case ".obj":
					return ReadOBJFile(path);
				case ".wrn":
				case ".bin":
					return ReadBinFile(path);
				default:
					return CreateMould(path);//read the file
			}
		}

		private bool ReadOBJFile(string path)
		{
			m_type = SurfaceType.OBJ;
			CreateMould(m_path);
			return Mould != null;
		}

		public void WriteBinFile(string binpath)
		{
			using (BinaryWriter bin = new BinaryWriter(File.Open(binpath, FileMode.OpenOrCreate, FileAccess.Write)))
			{
				Utilities.WriteCString(bin, Mould.Label);
				//bin.Write((Int32)Layout.Count);
				//int test = Layout.Count(g => g.GetType() == typeof(CurveGroup) || g.GetType() == typeof(YarnGroup));
				List<IRebuild> flat = FlatLayout();

				//bin.Write((Int32)Layout.Count(g => g.GetType() == typeof(CurveGroup) || g.GetType() == typeof(YarnGroup) || g.GetType() == typeof(TapeGroup)));
				bin.Write((Int32)flat.Count(g => g.GetType() == typeof(CurveGroup) || g.GetType() == typeof(YarnGroup) || g.GetType() == typeof(TapeGroup)));
				flat.ForEach(l =>
				{
					if (l is CurveGroup || l is YarnGroup || l is TapeGroup) 
						(l as IGroup).WriteBin(bin);
				});
			}
		}
		public void Save3DLFile(string tdipath)
		{
			Task.Factory.StartNew(() =>
			{
				Logleton.TheLog.Log("saving {0} to {1}...", Path.GetFileName(tdipath), Path.GetDirectoryName(tdipath));
				//DateTime now = DateTime.Now;
				using (StreamWriter sw = new StreamWriter(tdipath))
				{
					//write header
					//OUS102439-001, Fat26, EnergySolution ITA14313, Main ORCi, 10850 dpi, 3Dl 680, Capitani/NSI,//3DLayOut_Release 1.1.0.171
					string w1, w2, u, v, x, y, z, a, b, c;
					Vect2 uv = new Vect2();
					Vect3 xyz = new Vect3();
					Vect3 dx = new Vect3();
					Vect3 xnor = new Vect3();
					sw.WriteLine("{0} //Warps v{1}", ToString(), Utilities.CurVersion);
					foreach (YarnGroup yar in Layout.FindAll(grp => grp is YarnGroup))
					{
						List<List<double>> sPos;
						List<List<Point3D>> ents = yar.CreateYarnPaths(out sPos);

						for (int i = 0; i < ents.Count; i++)
						{
							/*
 9xxxxxxx-03, JV ORCi 38, Light Medium, 12600, xxxx, CW//3DLayOut_Release 1.1.0.227
   MD_UP  1.0000   FT_OT  0.0000  spacing  0.1106    80 offsets on yarn #1
 0.00000 0.56000    0 -1.592914  9.488185  0.016586 -0.876260  0.330783 -0.876260  0.000000
 0.00770 0.55505    1 -1.612374  9.417699  0.033659 -0.866415  0.326699 -0.866415  0.000000
 0.01541 0.55010    2 -1.631196  9.347072  0.050821 -0.855913  0.322363 -0.855913  0.000000
 0.02313 0.54514    3 -1.649370  9.276309  0.068057 -0.844782  0.317788 -0.844782  0.000000
							 * */

							w1 = yar[i].m_Warps[1].Label;
							w1 = w1.Length > 8 ? w1.Substring(0, 8) : w1.Length < 8 ? w1.PadLeft(8) : w1;

							w2 = yar[i].m_Warps[0].Label;
							w2 = w2.Length > 8 ? w2.Substring(0, 8) : w2.Length < 8 ? w2.PadLeft(8) : w2;

							a = "  1.0000";
							b = "  0.0000";
							c = (ents[i].Count - 1).ToString().PadLeft(8);

							sw.WriteLine("{0}{1}{2}{3}  spacing{4}{5} offsets on yarn #{6}",
								w1, a, w2, b, b, c, i);
							


							//sw.WriteLine("{0,-8}{1,-8:f4}{2,-8}{3,-8:f4}{4,-8}{5,-8:f4}{6,-8:n0} offsets on yarn #{7}"
							//		, yar[i].m_Warps[1].Label, 1.00, yar[i].m_Warps[0].Label.ToString(),0, "spacing", 0, ents[i].Count - 1, i);

							for (int j = 0; j < ents[i].Count; j++)
							{
								//get yarn position
								yar[i].xNor(sPos[i][j], ref uv, ref xyz, ref dx, ref xnor);
/*
 0.00000 0.56000    0 -1.592914  9.488185  0.016586 -0.876260  0.330783 -0.876260  0.000000
 0.00770 0.55505    1 -1.612374  9.417699  0.033659 -0.866415  0.326699 -0.866415  0.000000
 0.01541 0.55010    2 -1.631196  9.347072  0.050821 -0.855913  0.322363 -0.855913  0.000000
 0.02313 0.54514    3 -1.649370  9.276309  0.068057 -0.844782  0.317788 -0.844782  0.000000
*/
								u = uv[0].ToString("f5").PadLeft(8);
								v = uv[1].ToString("f5").PadLeft(8);

								w1 = j.ToString().PadLeft(5);

								x = ents[i][j].X.ToString("f5").PadLeft(10);
								y = ents[i][j].Y.ToString("f5").PadLeft(10);
								z = ents[i][j].Z.ToString("f5").PadLeft(10);

								a = xnor[0].ToString("f5").PadLeft(10);
								b = xnor[1].ToString("f5").PadLeft(10);
								c = xnor[2].ToString("f5").PadLeft(10);

								w2 = "   0.000000";

								sw.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
									u, v, w1,
									x, y, z,
									a, b, c,
									w2);

								//sw.WriteLine(" {0:#0.00000} {1:#0.00000}    {2}  {3:#0.000000}  {4:#0.000000}  {5:#0.000000}  {6:#0.000000}  {7:#0.000000}  {8:#0.000000}  {9:#0.000000}",
								//sw.WriteLine("{0,-8}{1,-8}{2,-5}{3,-10}{4,-10}{5,-10}{6,-10}{7,-10}{8,-10}{9,-10}",
								//	uv[0].ToString("f4"), uv[1].ToString("f4"), j.ToString()
								//	, ents[i][j].X.ToString("f5"), ents[i][j].Y.ToString("f5"), ents[i][j].Z.ToString("f5")
								//	, 0, 0, 0
								//	, 0);
								//0.98647-0.00093    0  3.726569 -0.020588  0.007790  0.155863  0.003351  0.987773  0.000000
							}
						}
					}
				}
				Utilities.HandleProcess(tdipath,null);
				//TimeSpan span = (DateTime.Now - now);
				//MessageBox.Show(string.Format("{0}\n{1}", span.TotalMilliseconds, span.TotalSeconds));
			});
		}
		public bool ReadBinFile(string binpath)
		{
			if (!File.Exists(binpath))
				return false;
			m_path = binpath;
			using (BinaryReader bin = new BinaryReader(File.Open(binpath, FileMode.Open, FileAccess.Read)))
			{
				string label = Utilities.ReadCString(bin);
				CreateMould(label);
				int nGrps = bin.ReadInt32();
				while (nGrps-- > 0)
				{
					IGroup grp = MakeGroup(bin);
					if (grp != null)
						Add(grp);
				}
			}
			return Mould != null;

			//Rebuild();
		}

		private IGroup MakeGroup(BinaryReader bin)
		{
			string type = Utilities.ReadCString(bin);
			var obj = Utilities.CreateInstance<IGroup>(type, bin);
			return obj;
		}

		bool CreateMould(string path)
		{
			return CreateMould(null, path);
		}
		bool CreateMould(string type, string path)
		{
			if (path == null || path.Length == 0 || !File.Exists(path))
			{
				string[] paths = WarpFrame.OpenFileDlg(1);
				if (paths != null && paths.Length > 0)
					path = paths[0];

				if (path == null || path.Length == 0)
					return false;
			}
			if (type == null)
			{
				switch (m_type)
				{
					case SurfaceType.COMBO:
						m_mould = new ComboMould(this, path);
						//(m_mould as ComboMould).ReadCofFile(this, path);
						break;
					case SurfaceType.COF:
						m_mould = new CofMould(this, path);
						//(m_mould as CofMould).ReadCofFile(this, path);
						break;
					case SurfaceType.RBF:
						m_mould = new RBFMould(this, path);
						//(m_mould as RBFMould).ReadCofFile(this, path);
						break;
					case SurfaceType.OBJ:
						m_mould = new Warps.Surfaces.RBFOBJ(path);
						break;
				}
			}
			else
			{
				if (type.Contains(':'))
					type = type.Split(':')[0];

				m_mould = Utilities.CreateInstance<ISurface>(type, new object[] { this, path });
			}
			if( m_mould != null )
				SetBox();//create sail bounding box

			return Mould != null;
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

		//[Obsolete]
		//void ReadScriptFile(string path)
		//{
		//	using (StreamReader txt = new StreamReader(path))
		//	{
		//		string line = txt.ReadLine();
		//		int cnt = ScriptTools.ReadCount(line);
		//		if (cnt >= 0)
		//		{
		//			if (updateStatus != null)
		//				updateStatus(-cnt, path);
		//		}
		//		line = txt.ReadLine();
	
		//		CreateMould(ScriptTools.ReadType(line), ScriptTools.ReadPath(line));

		//		//read layout
		//		line = txt.ReadLine();
		//		string[] splits;
		//		while (line != null)
		//		{
		//			List<string> lines = new List<string>(ScriptTools.Block(ref line, txt));

		//			IRebuild grp = null;
		//			splits = lines[0].Split(':');
		//			if (splits.Length > 0)
		//				grp = Utilities.CreateInstance<IRebuild>(splits[0].Trim('\t'));
		//			if (grp != null)
		//			{
		//				if( grp is IGroup)
		//					(grp as IGroup).Sail = this;
		//				m_layout.Add(grp);

		//				if (updateStatus != null)
		//					updateStatus(Layout.Count, "Loading " + lines[0]);
		//				grp.ReadScript(this, lines);
		//			}
		//		}
		//	}
		//}

		//[Obsolete]
		//public void WriteScriptFile(string path)
		//{
		//	using (StreamWriter txt = new StreamWriter(path))
		//	{
		//		//write label, path and layout count
		//		txt.WriteLine(string.Format("{0} [{1}] <{2}>", Mould.ToString(), FilePath, Layout.Count));
		//		foreach (string s in Mould.WriteScript())
		//		{
		//			txt.WriteLine("\t" + s);
		//		}
		//		//txt.WriteLine("\t" + Mould.Script);
		//		foreach (IRebuild grp in Layout)
		//			foreach (string s in grp.WriteScript())
		//			{
		//				txt.WriteLine("\t" + s);
		//			}
		//	}
		//}

		public XmlDocument WriteXScript(string path)
		{
			XmlDocument doc = NsXml.MakeDoc("Warps");
			XmlNode sail = doc.DocumentElement.AppendChild(NsXml.MakeNode(doc, GetType().Name, Label));
			//NsXml.AddAttribute(mould, "Path", path);
			NsXml.AddAttribute(sail, "Count", Layout.Count.ToString());
			XmlNode mould = sail.AppendChild(NsXml.MakeNode(doc, Mould.GetType().Name, Mould.Label));
			//Mould.Groups.ForEach(grp => mould.AppendChild(grp.WriteXScript(doc)));
			
			Layout.ForEach(grp => sail.AppendChild(grp.WriteXScript(doc)));
			if (path != null)
			{
				doc.Save(path);
				m_path = path;//store the path
			}
			return doc;
		}
		public bool ReadXScript(string path)
		{
			XmlDocument doc = NsXml.LoadDoc(path);
			if (doc == null)
			{
			//	ReadScriptFile(path);//try to read it as a script file
				return false;
			}
			XmlNode sail = doc.DocumentElement.FirstChild;

			if (sail.HasChildNodes && updateStatus != null)
				updateStatus(-sail.ChildNodes.Count, path);

			//construct the mould
			CreateMould(sail.FirstChild.Name, NsXml.ReadLabel(sail.FirstChild));

			//read the layout items
			IRebuild item;
			for (int nGrp = 1; nGrp < sail.ChildNodes.Count; nGrp++)
			{
				item = Utilities.CreateInstance<IRebuild>(sail.ChildNodes[nGrp].Name);
				if (item != null)
				{
					//if (item is IGroup)
					//	(item as IGroup).Sail = this;
					m_layout.Add(item);

					if (updateStatus != null)
						updateStatus(Layout.Count, "Loading " + NsXml.ReadString(sail.ChildNodes[nGrp], "Label"));

					item.ReadXScript(this, sail.ChildNodes[nGrp]);
				}
			}
			return Mould != null;
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

		public void Rebuild()
		{
			foreach (IRebuild item in Layout)
			{
				item.Update(this);
			}
			WriteNode();
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
				item.GetChildren(connected);
			}

			return connected;
		}
		public List<IRebuild> GetConnected(List<IRebuild> tags)
		{
			List<IRebuild> connected = null;
			if (tags != null)
			{
				connected = new List<IRebuild>(tags);
			}
			foreach (IRebuild item in Layout)
			{
				item.GetChildren(connected);
			}

			return connected;
		}
		TreeNode m_node;
		public TreeNode WriteNode() { return WriteNode(true); }
		public TreeNode WriteNode(bool bClear)
		{
			if (m_node == null)
			{
				m_node = new System.Windows.Forms.TreeNode();
				bClear = true;//if new node force full write
			}
			m_node.Text = Path.GetFileName(m_path);
			m_node.Tag = this;
			m_node.ImageKey = m_node.SelectedImageKey = 	m_node.ToolTipText = GetType().Name;
			if (bClear)
			{
				m_node.Nodes.Clear();
				m_node.Nodes.Add(Mould.WriteNode());
				foreach (IRebuild g in Layout)
					m_node.Nodes.Add(g.WriteNode());
			}
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

		public IRebuild Add(IRebuild item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var found = FindItem(item.Label);
			if (found != null)
				return found;

			Layout.Add(item);
			return null;
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
				if (Layout[i].Label != null && Layout[i].Label.Equals(lbl))
					return Layout[i];//return if match

				//search group for item
				if (Layout[i] is IGroup)
				{
					item = (Layout[i] as IGroup).FindItem(lbl);
					if (item != null)
						return item;//return if match
				}
			}
			//check mould items (if any)
			if (Mould != null && Mould.Groups != null)
				for (int i = Mould.Groups.Count - 1; i >= 0; i--)
				{
					//check group label
					if (Mould.Groups[i].Label != null && Mould.Groups[i].Label.Equals(lbl))
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
		/// Finds the parent curve of a given fitpoint
		/// </summary>
		/// <param name="fp">the fitpoint to search for</param>
		/// <returns>The parent curve if successful, null otherwise</returns>
		public MouldCurve FindCurve(IFitPoint fp)
		{
			foreach (IRebuild cur in FlatLayout().Where(irb => irb is MouldCurve))
			{
				if ((cur as MouldCurve).FitPoints.Contains(fp))
					return cur as MouldCurve;
			}
			return null;
		}

		/// <summary>
		/// Finds the parent group of the specified tag
		/// </summary>
		/// <param name="item">the item to find the parent of</param>
		/// <returns>the containing IGroup, null if not found</returns>
		public T FindParent<T>(IRebuild item)where T: class, IGroup
		{
			T parent;
			FindParent(item, out parent);
			return parent;
		}
		/// <summary>
		/// Finds the parent group of the specified item
		/// </summary>
		/// <param name="tag">the item to find the parent of</param>
		/// <param name="parent">outputs the parent group of the specified item, null if not found</param>
		/// <returns>true if successful</returns>
		public bool FindParent<T>(IRebuild item, out T parent) where T: class, IGroup
		{
			//check the mould groups	
			if (Mould != null && Mould.Groups != null)
				foreach( IGroup r in Mould.Groups)
					if ( r.FindParent(item, out parent))
						return true;
			//check the warps groups
			foreach( IRebuild r in Layout)
				if (r is IGroup && (r as IGroup).FindParent(item, out parent))
					return true;

			parent = null;
			return false;
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
				if (Layout[i] == tag)
					break;
				if( Layout[i] is IGroup)
				{
				if( (Layout[i] as IGroup).Watermark(tag, ref rets))
					break;//break on finding tag
				}
				else rets.Add(Layout[i]);
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
				if (Layout[i] == tag || Layout[i] == Parent)
					break;
				if( Layout[i] is IGroup )
					if ((Layout[i] as IGroup).Watermark(tag, ref rets))
						break;//break on finding tag


				//if (index > -1)
				//{
				//	if (i <= index)
				//		if (Layout[i].Watermark(tag, ref rets))
				//			break;//break on finding tag

				//}
				//else if (Layout[i].Watermark(tag, ref rets))
				//	break;//break on finding tag

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
			return Watermark(tag).FindAll(g => g is MouldCurve).ConvertAll(g => g as MouldCurve);
			//List<MouldCurve> eqs = new List<MouldCurve>();
			//foreach (IRebuild rb in Watermark(tag))
			//{
			//	if (rb is MouldCurve)
			//		eqs.Add(rb as MouldCurve);
			//}
			//return eqs;
		}

		#endregion

		#region Default Geometry

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

					MouldCurve g = new MouldCurve(string.Format(group.Label + "[{0}]", nAng), this, uv[0], uv[1]);
					group.Add(g);
				}
				Add(group);
			}

		}

		public IGroup CreateOuterCurves()
		{
			//if (FindGroup("Outer") != null)
			//	return FindGroup("Outer");

			CurveGroup outer = new CurveGroup("Outerr", this);
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

			foreach(MouldCurve mc in outer)
			{
				mc.Girth(0, false);
				mc.Update(this);
			}
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

					MouldCurve girth = new MouldCurve(String.Format("{0}ir-{1:##0}%", i == 0 ? "G" : "V", dg * 100), this, gir);
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

					MouldCurve curve = new MouldCurve(String.Format("{0}ec-{1:##0}%", i == 0 ? "S" : "V", dg * 100), this, gir);
					curves.Add(curve);
				}
				Add(curves);
				rets[i] = curves;
			}
			return rets;
		}

		public IGroup CreateBaxCurves()
		{
			CurveGroup batts = FindGroup("Batts") as CurveGroup;
			CurveGroup baxxs = new CurveGroup("Bax", this);
			MouldCurve Luff = FindCurve("Luff");
			MouldCurve Leech = FindCurve("Leech");
			Vect2 uv1 = new Vect2(), uv2 = new Vect2(), sPos = new Vect2();
			Vect3 xyz = new Vect3();

			foreach (MouldCurve bat in batts)
			{
				List<IFitPoint> threept = new List<IFitPoint>();
				threept.Add(new CrossPoint(Leech, bat));
				threept.Add(new CurvePoint(bat, 1));
				if( !CurveTools.CrossPoint(Luff,bat,ref uv1, ref xyz, ref sPos) )//shortbatten, do extension
					threept.Add(new SlidePoint(Luff, uv1[1]));

				baxxs.Add(new MouldCurve(bat.Label.Replace("Bat", "Bax"), this, threept.ToArray()));

				//if (CurveTools.CrossPoint(Luff, bat, ref uv1, ref xyz, ref sPos, 20, false))
				//	if (CurveTools.CrossPoint(Leech, bat, ref uv2, ref xyz, ref sPos, 20, false))
				//		baxxs.Add(new MouldCurve(bat.Label.Replace("Bat", "Bax"), this, uv1, uv2));
			}
			Add(baxxs);
			return baxxs;
		}

		#endregion

		#region Bounding Box
		/// <summary>
		/// Mould bounding box limits in xyz
		/// 0: x min/max, 1: y min/max, 2: z min/max
		/// </summary>
		public Vect2[] m_box = new Vect2[3];
		public void SetBox()
		{
			for (int i =0; i < m_box.Length; i++)
				m_box[i] = new Vect2(1e9,-1e9);
			Vect2 uv = new Vect2();
			Vect3 xyz = new Vect3();
			int nU = 10, nV = 10;
			for (int iu = 0; iu < nU; iu++)
			{
				uv[0] = BLAS.interpolant(iu, nU);
				for (int iv = 0; iv < nV; iv++)
				{
					uv[1] = BLAS.interpolant(iv, nV);
					Mould.xVal(uv, ref xyz);

					for (int ix = 0; ix < 3; ix++)
					{
						m_box[ix][0] = Math.Min(m_box[ix][0], xyz[ix]);
						m_box[ix][1] = Math.Max(m_box[ix][1], xyz[ix]);
					}
				}
			}
		}
		/// <summary>
		/// X Span
		/// </summary>
		public double Width
		{
			get
			{
				return m_box[0][1] - m_box[0][0];
			}
		}
		/// <summary>
		/// Y Span
		/// </summary>
		public double Height
		{
			get
			{
				return m_box[1][1] - m_box[1][0];
			}
		}
		/// <summary>
		/// Z Span
		/// </summary>
		public double Depth
		{
			get
			{
				return m_box[2][1] - m_box[2][0];
			}
		}
 
		#endregion	
	
		internal void Insert(IRebuild item, IRebuild target)
		{
			int nTar = Layout.IndexOf(target);
			int nIrb = Layout.IndexOf(item);
			if (nIrb >= 0)//item is already in this group: reorder
				Layout.Remove(item);
			if (!Utilities.IsBetween(0, nTar, Layout.Count))
				nTar = Layout.Count;//insert at end
			Layout.Insert(nTar, item);
		}

		static string[] exts = new string[] { ".wrp", ".xml", ".obj", ".wrn", ".bin", ".spi", ".cof" };
		internal static bool ValidExtension(string ext)
		{
			ext = ext.ToLower();
			return exts.Contains(ext);
		}
	}
}
