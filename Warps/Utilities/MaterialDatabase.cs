using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warps
{
	public class MaterialDatabase 
	{
		public MaterialDatabase(string path)
		{
			//ParseFile(path);
			ParseYarns(path);
		}

		public List<double> this[string mat]
		{
			get { return m_materials[mat]; }
		}
		public double this[string mat, int index]
		{
			get { return m_materials[mat][index]; }
		}
		public List<string> Materials
		{
			get { return m_materials.Keys.ToList(); }
		}

		string m_label;

		public string Label
		{
			get { return m_label; }
			set { m_label = value; }
		}
		Dictionary<string, List<double>> m_materials = new Dictionary<string, List<double>>();

		public Dictionary<string, List<double>> Mats
		{
			get { return m_materials; }
		}

		private void ParseYarns(string path)
		{
			if( path == null || path.Length == 0)
				throw new FileNotFoundException("No Material DB specified");

			FileInfo file = new FileInfo(path);
			if (!file.Exists)
				throw new FileNotFoundException("Failed to find Material DB", file.FullName);

			//create the node
			Mats.Clear();
			//store the path
			Label = file.FullName;

			string line;
			using (StreamReader sr = new StreamReader(path) )
			{
				line = sr.ReadLine();
				ReadYarnTable(sr, ref line);
			}
		}
		private void ReadYarnTable(StreamReader sr, ref string line)
		{
			while (line != null && !line.StartsWith("Yarn_Table"))
				line = sr.ReadLine();

			if (line == null)
				return;
			double d;
			string[] splits;
			string[] header = line.Split(',');
			List<double> beam;
			while ((line = sr.ReadLine()) != null)
			{
				splits = line.Split(',');
				if (splits.Length == 0 || splits[0].Length == 0)//blank line
					continue;//skip over blanks
				if (splits[0].Length > 8)//all mat-labels must be 8chars. if more then its a header line
					return;//return on finding a new header

				beam = new List<double>();
				//convert to numbers and insert
				for (int i = 1; i < 5; i++)//EIs
					if (double.TryParse(splits[i].Trim(), out d))
						beam.Add(d);
				Mats[splits[0]] = beam;
				//Mats.Add(splits[0], beam);
			}
		}


		//private void ParseFile(string path)
		//{
		//	if( path == null || path.Length == 0)
		//		throw new FileNotFoundException("No Material DB specified");

		//	FileInfo file = new FileInfo(path);
		//	if (!file.Exists)
		//		throw new FileNotFoundException("Failed to find Material DB", file.FullName);

		//	//create the node
		//	Mats.Clear();
		//	//store the path
		//	Label = file.FullName;

		//	string line;
		//	using (StreamReader sr = new StreamReader(file.OpenRead()))
		//	{
		//		line = sr.ReadLine();
		//		while (line != null && !line.StartsWith("Material_Table", StringComparison.InvariantCultureIgnoreCase))
		//			line = sr.ReadLine();

		//		ReadTable(sr, ref line, "Material_Table", 9);
		//		ReadBattenTable(sr, ref line);
		//		ReadTable(sr, ref line, "Beam_Table", 4);
		//		ReadTable(sr, ref line, "Spreader_Table", 6);
		//		ReadTable(sr, ref line, "Ball_Table", 8);
		//		ReadTable(sr, ref line, "Hinge_Table", 8);
		//		ReadTable(sr, ref line, "Sheet_Table", 4);
		//		ReadWireTable(sr, ref line);
		//	}
		//}
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
		//	{	splits = line.Split(',');
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

		//private void ReadTable(StreamReader sr, ref string line, string deflabel, int numEIs)
		//{
		//	if (line == null)
		//		return;
		//	//List<string[]> batts = new List<string[]>();
		//	double d;
		//	NsNode beam;
		//	string[] splits;
		//	string[] header = line.Split(',');
		//	NsNode beams = FindAddNode(header[0].Length == 0 ? deflabel : header[0]);

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
		//		for (int i = 1; i < numEIs; i++)//EIs
		//			if( double.TryParse(splits[i].Trim(), out d) )
		//				beam.Add(new DoubleAttribute(beam, header[i].Trim(), d));
		//	}
		//}

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

	}
}
