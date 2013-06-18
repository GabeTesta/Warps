using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Warps
{
	public static class ScriptTools
	{
		public static string Label(string type, string label)
		{
			return type + ": [" + label + "]";
		}

		public static string ReadType(string line)
		{
			if (line == null)
				return null;
			int nc = line.IndexOf(':');
			if (nc == -1 || line.Length == 0)
				return null;
			return line.Substring(0, nc).Trim(' ', '\t', '\n', '\r');		
		}

		public static string ReadPath(string line)
		{
			if (line == null)
				return null;
			int nc = line.IndexOf('[')+1;
			int ne = line.IndexOf(']', nc);
			if (nc == -1 || ne == -1 || line.Length == 0)
				return null;
			return line.Substring(nc, ne - nc).Trim(' ', '\t', '\n', '\r');	
		}

		public static int ReadCount(string line)
		{
			if (line == null)
				return -1;
			int nc = line.IndexOf('<') + 1;
			int ne = line.IndexOf('>', nc);
			if (nc == -1 || ne == -1 || line.Length == 0)
				return -1;
			line = line.Substring(nc, ne - nc).Trim(' ', '\t', '\n', '\r');
			if (int.TryParse(line, out nc))
				return nc;
			return -1;
		}

		public static string ReadLabel(string line)
		{
			if (line == null)
				return null;
			int nc = line.IndexOf(':');
			if (nc == -1 || line.Length == 0)
				return null;
			return line.Substring(nc + 1).Trim(' ', '\t', '\n', '\r');		
		}

		public static int Depth(string line)
		{
			int nDepth = 0;
			if (line.StartsWith("\t"))
				while (line[nDepth++] == '\t' && nDepth < line.Length) ;
			if (nDepth == line.Length)
				return -1;
			return nDepth;
		}

		public static List<string> Block(ref string Line, StreamReader txt)
		{
			List<string> lines = new List<string>();
			int nDepth = ScriptTools.Depth(Line);
			string tabs = "";
			if (nDepth > 0)
				tabs = new string('\t', nDepth);//++nDepth?
			lines.Add(Line);

			while ((Line = txt.ReadLine()) != null)
			{
				if (!Line.StartsWith(tabs))
					break;
				lines.Add(Line);
			}
			//if (header == null)//premature EOF 
			//	return tabs == "" ? lines : null;
			return lines;
		}

		public static IList<string> Block(ref int nLine, IList<string> txt)
		{
			List<string> lines = new List<string>();
			int nDepth = Depth(txt[nLine]);
			string tabs = "";
			if (nDepth > 0)
				tabs = new string('\t', nDepth);//++nDepth?
			lines.Add(txt[nLine]);

			//int nstart = txt.IndexOf(line);
			for (nLine = nLine+1; nLine < txt.Count; nLine++)
			{
				if (!txt[nLine].StartsWith(tabs))
					break;
				lines.Add(txt[nLine]);
			}
			return lines;
		}

		public static void ModifyScriptToShowCopied(ref List<string> result)
		{
			for (int i = 0; i < result.Count; i++)
			{
				if (result[i].Contains("CurveGroup:") 
				|| result[i].Contains("MouldCurve:") 
				|| result[i].Contains("Label:") 
				|| result[i].Contains("GuideComb:")
				|| result[i].Contains("VariableGroup:"))
					result[i] = String.Format("{0}:{1}", result[i].Split(new char[] { ':' })[0], result[i].Split(new char[] { ':' })[1] + "_Copy");
				else if(result[i].Contains("Equation")){
					result[i + 1] = String.Format("{0}:{1}", result[i + 1].Split(new char[] { ':' })[0] + "_Copy", result[i + 1].Split(new char[] { ':' })[1]);
				}
			}
		}
	}
}
