using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warps
{
	public class MaterialDatabase 
	{
		public static class TableTypes
		{
			public static string Materials = "Material_Table";
			public static string Beams = "Beam_Table";
			public static string Spreaders = "Spreader_Table";
			public static string Balls = "Ball_Table";
			public static string Hinges = "Hinge_Table";
			public static string Sheets = "Sheet_Table";
			public static string Yarns = "Yarn_Table";
			public static string Tapes = "Tape_Table";
		}

		public MaterialDatabase(string path)
		{
			ParseFile(path);
			//ParseYarns(path);
		}

		/// <summary>
		/// returns the dictionary of materials for a given table
		/// </summary>
		/// <param name="table">the desired material's TableType</param>
		/// <returns>the dictionary of materials, null if not found</returns>
		public Dictionary<string, List<double>> this[string table]
		{
			get { return m_materials.ContainsKey(table) ? m_materials[table] : null; }
		}
		/// <summary>
		/// returns the material properties of a given material
		/// </summary>
		/// <param name="table">the desired material's TableType</param>
		/// <param name="mat">the desired material's label</param>
		/// <returns>the list of values</returns>
		public List<double> this[string table, string mat]
		{
			get { return this[table] != null && this[table].ContainsKey(mat) ? m_materials[table][mat] : null; }
		}
		/// <summary>
		/// returns the material property of a given material
		/// </summary>
		/// <param name="table">the desired material's TableType</param>
		/// <param name="mat">the desired material's label</param>
		/// <param name="index">the desired property's index</param>
		/// <returns>the value of the material's property</returns>
		public double this[string table, string mat, int index]
		{
			get { return this[table,mat] != null && this[table,mat].Count > index ? this[table,mat][index] : double.NaN; }
		}

		/// <summary>
		/// The list of available tables, a subset of the TableTypes options
		/// </summary>
		public List<string> Tables
		{
			get { return m_materials.Keys.ToList(); }
		}
		/// <summary>
		/// the list of materials of a given table
		/// </summary>
		/// <param name="table">the TableType to list</param>
		/// <returns>a list of material labels, null if the TableType doesnt exist</returns>
		public List<string> Materials(string table)
		{
			return m_materials[table].Keys.ToList();
		}

		Dictionary<string, Dictionary<string, List<double>>> m_materials = new  Dictionary<string, Dictionary<string, List<double>>>();
		public Dictionary<string,Dictionary<string, List<double>>> Mats
		{
			get { return m_materials; }
		}

		string m_label;
		public string Label
		{
			get { return m_label; }
			//set { m_label = value; }
		}

		//private void ParseYarns(string path)
		//{
		//	if (path == null || path.Length == 0)
		//	{
		//		path = Path.Combine(Utilities.ExeDir, "Materials.csv");
		//		//throw new FileNotFoundException("No Material DB specified");
		//	}

		//	FileInfo file = new FileInfo(path);
		//	if (!file.Exists)
		//		throw new FileNotFoundException("Failed to find Material DB", file.FullName);

		//	//create the node
		//	Mats.Clear();
		//	//store the path
		//	Label = file.FullName;

		//	string line;
		//	using (StreamReader sr = new StreamReader(path) )
		//	{
		//		line = sr.ReadLine();
		//		ReadYarnTable(sr, ref line);
		//	}
		//}
		//private void ReadYarnTable(StreamReader sr, ref string line)
		//{
		//	while (line != null && !line.StartsWith("Yarn_Table"))
		//		line = sr.ReadLine();

		//	if (line == null)
		//		return;

		//	Mats[TableTypes.Yarns] = new Dictionary<string, List<double>>();
		//	double d;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	List<double> beam;
		//	while ((line = sr.ReadLine()) != null)
		//	{
		//		splits = line.Split(',');
		//		if (splits.Length == 0 || splits[0].Length == 0)//blank line
		//			continue;//skip over blanks
		//		if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
		//			return;//return on finding a new header

		//		beam = new List<double>();
		//		//convert to numbers and insert
		//		for (int i = 1; i < 5; i++)//EIs
		//			if (double.TryParse(splits[i].Trim(), out d))
		//				beam.Add(d);
		//		Mats[TableTypes.Yarns][splits[0]] = beam;
		//		//Mats.Add(splits[0], beam);
		//	}
		//}

		private void ParseFile(string path)
		{
			if (path == null || path.Length == 0)
			{
				path = Path.Combine(Utilities.ExeDir, "Materials.csv");
				//throw new FileNotFoundException("No Material DB specified");
			}

			FileInfo file = new FileInfo(path);
			if (!file.Exists)
				throw new FileNotFoundException("Failed to find Material DB", file.FullName);

			//create the node
			Mats.Clear();
			//store the path
			m_label = file.FullName;
			
			string line;
			try
			{
				using (StreamReader sr = new StreamReader(file.OpenRead()))
				{
					line = sr.ReadLine();
					//while ((line = sr.ReadLine()) != null)
					while (line != null)
					{
						if (line.StartsWith(TableTypes.Materials, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Materials, 9);
						else if (line.StartsWith(TableTypes.Beams, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Beams, 4);
						else if (line.StartsWith(TableTypes.Spreaders, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Spreaders, 6);
						else if (line.StartsWith(TableTypes.Balls, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Balls, 8);
						else if (line.StartsWith(TableTypes.Hinges, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Hinges, 8);
						else if (line.StartsWith(TableTypes.Sheets, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Sheets, 4);
						else if (line.StartsWith(TableTypes.Yarns, StringComparison.InvariantCultureIgnoreCase))
							ReadTable(sr, ref line, TableTypes.Yarns, 2);
						else if (line.StartsWith(TableTypes.Tapes, StringComparison.InvariantCultureIgnoreCase))
							ReadTapes(sr, ref line);
						else
							line = sr.ReadLine();
					}
				}
			}
			catch (Exception e)
			{
				Logleton.TheLog.Log(e.Message, Logleton.LogPriority.Error);
			}
		}

		private void ReadTable(StreamReader sr, ref string line, string TableType, int numEIs)
		{
			if (line == null)
				return;
			//List<string[]> batts = new List<string[]>();
			Mats[TableType] = new Dictionary<string, List<double>>();
			double d;
			string[] splits;
			string[] header = line.Split(',');
			List<double> beam;
			numEIs++;//offset to account for label column
			while ((line = sr.ReadLine()) != null)
			{
				splits = line.Split(',');
				if (splits.Length == 0 || splits[0].Length == 0)//blank line
					continue;//skip over blanks
				if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
					return;//return on finding a new header

				beam = new List<double>(numEIs);
				//convert to numbers and insert
				for (int i = 1; i < numEIs; i++)//EIs
					if (double.TryParse(splits[i].Trim(), out d))
						beam.Add(d);
				Mats[TableType][splits[0]] = beam;
				//Mats.Add(splits[0], beam);
			}
		}
		private void ReadTapes(StreamReader sr, ref string line)
		{
			if (line == null)
				return;
			//List<string[]> batts = new List<string[]>();
			Mats[TableTypes.Tapes] = new Dictionary<string, List<double>>();
			double d;
			string[] splits;
			string[] header = line.Split(',');
			List<double> beam;
			while ((line = sr.ReadLine()) != null)
			{
				splits = line.Split(',');
				if (splits.Length == 0 || splits[0].Length == 0)//blank line
					return;//return on finding a blank

				beam = new List<double>();
				//convert to numbers and insert
				for (int i = 1; i < 11; i++)//EIs
					if (double.TryParse(splits[i].Trim(), out d))
						beam.Add(d);
				Mats[TableTypes.Tapes][splits[0]] = beam;
				//Mats.Add(splits[0], beam);
			}


		}
		//private void ReadWireTable(StreamReader sr, ref string line)
		//{
		//	if (line == null)
		//		return;
		//	//List<string[]> batts = new List<string[]>();
		//	double d;
		//	NsNode wire;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	NsNode wires = FindAddNode(header[0].Length == 0 ? "Wires" : header[0]);
		//	NsNode wiretype = wires.FindAddNode("Wire");//default initial wiretype
		//	while ((line = sr.ReadLine()) != null)
		//	{
		//		splits = line.Split(',');
		//		if (splits.Length == 0 || splits[0].Length == 0)//blank line
		//			continue;//skip over blanks
		//		if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
		//		{
		//			if (splits[0].Contains("_Table"))
		//				return;//new table, exit out
		//			//new wiretype add subnode
		//			wiretype = wires.FindAddNode(splits[0]);
		//			continue;
		//		}


		//		//create a subnode for the beam
		//		wire = wiretype.FindAddNode(splits[0]);

		//		//convert to numbers and insert
		//		for (int i = 1; i < 5; i++)//EIs
		//			if (double.TryParse(splits[i].Trim(), out d))
		//				wire.Add(new DoubleAttribute(wire, header[i].Trim(), d));
		//	}
		//}

		public System.Windows.Forms.TreeNode WriteNode()
		{
			System.Windows.Forms.TreeNode tnTable, tnMat, tnRoot = new System.Windows.Forms.TreeNode(Path.GetFileName(Label));
			tnRoot.Nodes.Add(Label);
			foreach (KeyValuePair<string, Dictionary<string, List<double>>> table in m_materials)
			{
				tnTable = tnRoot.Nodes.Add(table.Key);
				foreach (KeyValuePair<string, List<double>> mat in table.Value)
				{
					tnMat = tnTable.Nodes.Add(mat.Key);
					mat.Value.ForEach(d => tnMat.Nodes.Add(d.ToString("g")));
				}
			}
			return tnRoot;
		}

		//Dictionary<string, Dictionary<string, double[]>> m_battens = new Dictionary<string, Dictionary<string, double[]>>();

		//private void ReadBattenTable(StreamReader sr, ref string line)
		//{
		//	if (line == null)
		//		return;
		//	//List<string[]> batts = new List<string[]>();
		//	double d;
		//	//NsNode batt;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	//NsNode batts = FindAddNode(header[0].Length == 0 ? "Battens" : header[0]);

		//	while ((line = sr.ReadLine()) != null)
		//	{
		//		splits = line.Split(',');
		//		if (splits.Length == 0 || splits[0].Length == 0)//blank line
		//			continue;//skip over blanks
		//		if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
		//			return;//return on finding a new header

		//		//create a subnode for the batten
		//		batt = batts.FindAddNode(splits[0]);//straight batten (single-line) uses main batten node

		//		//taper'd batten (multi-line) subnode
		//		if (splits.Length >= 5 && splits[4].Length > 0)
		//			batt = batt.Add(splits[4]);

		//		//convert to numbers and insert
		//		for (int i = 1; i < 4; i++)//EIs
		//			if (double.TryParse(splits[i].Trim(), out d))
		//				batt.Add(new DoubleAttribute(batt, header[i].Trim(), d));
		//	}

		//}

		//private void ReadMaterialTable(StreamReader sr, ref string line)
		//{
		//	NsNode mat;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	NsNode mats = FindAddNode(header[0].Length == 0 ? "Materials" : header[0]);
		//	while ((line = sr.ReadLine()) != null)
		//	{
		//		splits = line.Split(',');
		//		if (splits.Length == 0 || splits[0].Length == 0)//blank line
		//			continue;

		//		if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
		//			return;

		//		//add mat node
		//		mat = mats.FindAddNode(splits[0]);

		//		//add properties to material node
		//		for (int i = 1; i < 9; i++)//EIs
		//			mat.Add(new DoubleAttribute(mat, header[i].Trim(), splits[i].Trim()));
		//	}
		//}

		//private void ReadBeamTable(StreamReader sr, ref string line)
		//{
		//	//List<string[]> batts = new List<string[]>();
		//	NsNode beam;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	NsNode beams = FindAddNode(header[0].Length == 0 ? "Beams" : header[0]);

		//	while ((line = sr.ReadLine()) != null)
		//	{
		//		splits = line.Split(',');
		//		if (splits.Length == 0 || splits[0].Length == 0)//blank line
		//			continue;//skip over blanks
		//		if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
		//			return;//return on finding a new header

		//		//create a subnode for the beam
		//		beam = beams.FindAddNode(splits[0]);

		//		//convert to numbers and insert
		//		for (int i = 1; i < 4; i++)//EIs
		//			beam.Add(new DoubleAttribute(beam, header[i].Trim(), splits[i].Trim()));
		//	}
		//}

	}
}
