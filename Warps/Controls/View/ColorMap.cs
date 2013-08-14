using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Logger;

namespace Warps
{
	public class ColorMap: IEnumerable<string>
	{
		//static Color DEFAULT = Color.SlateGray;
		Dictionary<string, Color> m_colors = new Dictionary<string, Color>();

		/// <summary>
		/// Find the color for the specified label and returns it. Will create a new entry in the color map if the lbl does not already exist.
		/// </summary>
		/// <param name="lbl">The color object's label</param>
		/// <param name="def">The Default color to return if not defined, can be null</param>
		/// <returns>The Ini/Map color, the default color, or a new Random color</returns>
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
						col = def == null || def.IsEmpty ? ColorMath.GetRandomColor() : def;

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

		/// <summary>
		/// Searches the ini lines for the specified color
		/// </summary>
		/// <param name="lbl">The color to search for</param>
		/// <returns>The ini color or Color.Empty if not found</returns>
		private Color IniColor(string lbl)
		{
			if (!HasIniFile)
				return Color.Empty;
			foreach (string s in m_lines)
			{
				if (s.Length == 0)
					continue;
				string[] txt = s.Split(':');
				if (txt != null && txt.Length == 2 && txt[0].Equals(lbl, StringComparison.InvariantCultureIgnoreCase))
					return ReadLine(txt[1]);
			}
			return Color.Empty;
		}


		/// <summary>
		/// Parses a line of text and returns the color
		/// </summary>
		/// <param name="line">The space-seperated RGB color string, e.g., "125 50 255"</param>
		/// <returns>A new color object from the passed RGB</returns>
		private Color ReadLine(string line)
		{
			//string[] splits = line.Split(new char[]{':'}, StringSplitOptions.RemoveEmptyEntries);
			//if (splits.Length < 2)
			//	return Color.Empty;
			//else
			//{
				string[] splits = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				if (splits.Length < 3)
					return Color.Empty;

				int[] rgb = new int[3];
				for( int i =0; i< 3; i++ )
					int.TryParse(splits[i], out rgb[i]);

				return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
		//	}
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
							lines.AddRange(File.ReadAllLines(s));
							path = s;
						}
						m_lines = lines.ToArray();
						foreach (string s in this)
							this[s] = IniColor(s);//refresh the local color with the ini values
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


		/// <summary>
		/// Stores the current color values to the specified file
		/// </summary>
		/// <param name="path">The file to write. If null a SaveFileDialog will prompt the user</param>
		/// <returns>true if successful, false otherwise</returns>
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
