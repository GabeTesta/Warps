using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Warps.Logger;

namespace Warps
{
	public class ColorMap: IEnumerable<string>
	{
		//static Color DEFAULT = Color.SlateGray;
		Dictionary<string, Color> m_colors = new Dictionary<string, Color>();
		public Color this[string lbl, Color def]
		{
			get
			{
				Color col = Color.Empty;
				if (!m_colors.ContainsKey(lbl))
				{
					//attempt to find color in ini file first
					if (HasIniFile)
						col = IniColor(lbl);
					//use default color if not supplied by ini
					if (col.IsEmpty)
						col = def.IsEmpty ? ColorMath.GetRandomColor() : def;

					m_colors.Add(lbl, col);
					logger.Instance.Log(string.Format("ColorMap: {0} added for type \"{1}\"", col, lbl));
				}
				return m_colors[lbl];
			}
		}
		public Color this[string lbl]
		{
			get
			{
				return this[lbl, Color.Empty];
			}
			set
			{
				//add if not alread found
				if (!m_colors.ContainsKey(lbl))
					m_colors.Add(lbl, value);
				else//set otherwise
					m_colors[lbl] = value;
			}
		}

		private Color IniColor(string lbl)
		{
			if (!HasIniFile)
				return Color.Empty;
			foreach (string s in m_lines)
				if (s.StartsWith(lbl, StringComparison.InvariantCultureIgnoreCase))
					return ReadLine(s);
			return Color.Empty;
		}
		private Color ReadLine(string line)
		{
			string[] splits = line.Split(new char[]{':'}, StringSplitOptions.RemoveEmptyEntries);
			if (splits.Length < 2)
				return Color.Empty;
			else
			{
				splits = splits[1].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				if (splits.Length < 3)
					return Color.Empty;

				int[] rgb = new int[3];
				for( int i =0; i< 3; i++ )
					int.TryParse(splits[i], out rgb[i]);

				return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
			}
		}

		string[] m_lines = null;
		string m_path;

		public string IniPath
		{
			get { return m_path; }
			set { m_path = value; }
		}
		public bool HasIniFile { get{return m_lines != null;} }
		public void ReadIniFile(string path)
		{
			try
			{
				if (path == null)
				{
					string[] colortexts = Utilities.OpenFileDlg("txt", "Open Color.txt file", Utilities.ExeDir);
					{
						List<string> lines = new List<string>();
						foreach (string s in colortexts)
						{
							lines.AddRange(File.ReadAllLines(path));
							path = s;
						}
						m_lines = lines.ToArray();
					}
				}
				else
				{
					m_lines = File.ReadAllLines(path);
					m_path = path;
					//fillDictionary();
				}
			}
			catch
			{ 
				m_lines = null;
				m_path = null;
			}
		}

		public bool WriteIniFile(string path)
		{
			if (path != null)
				m_path = path;
			if (m_path == null)
			{
				m_path = Utilities.SaveFileDialog("txt", "Save Color.txt file", Utilities.ExeDir);
				if (m_path == null)
					return false;
			}
			using (StreamWriter ini = new StreamWriter(m_path))
			{
				foreach (KeyValuePair<string, Color> col in m_colors)
					ini.WriteLine(String.Format("{0}: {1} {2} {3}", col.Key, col.Value.R, col.Value.G, col.Value.B));
			}
			return true;
		}

		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return m_colors.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_colors.Keys.GetEnumerator();
		}

		#endregion
	}
}
