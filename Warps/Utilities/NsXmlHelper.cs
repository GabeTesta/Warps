using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

	public static class NsXml
	{
		public static XmlDocument MakeDoc(string root)
		{
			if (root == null || root == "")
				root = "xml";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(String.Format("<{0}></{0}>",root));
			AddAttribute(doc.DocumentElement, "Version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			return doc;
		}
		public static XmlDocument LoadDoc(string xmlpath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(xmlpath);
			}
			catch (Exception e) { Logleton.TheLog.Log(e.Message, Logleton.LogPriority.Debug); return null; }
			return doc;
		}

		public static XmlElement MakeNode(XmlDocument doc, Warps.IRebuild item) { return MakeNode(doc, item.GetType().Name, item.Label); }
		public static XmlElement MakeNode(XmlDocument doc, string type) { return MakeNode(doc, type, null); }
		public static XmlElement MakeNode(XmlDocument doc, string type, string label)
		{
			XmlElement xn;
			// create an element
			xn = doc.CreateElement(type);
			// add the label
			if (label != null)
			{
				XmlAttribute xa;
				xa = doc.CreateAttribute("Label");
				xa.Value = label;
				xn.Attributes.Append(xa);
			}
			return xn;
		}
		
		public static XmlAttribute AddAttribute(XmlNode node, string name, string value)
		{
			//name = name.Replace(' ', '_');
			XmlAttribute atr = node.OwnerDocument.CreateAttribute(name);
			atr.Value = value;
			node.Attributes.Append(atr);
			return atr;
		}
		//public static XmlAttribute AddAttribute(XmlNode node, string name, object value)
		//{
		//	return AddAttribute(node, name, value.ToString());
		//}
		public static XmlAttribute AddAttribute(XmlNode node, string name, params string[] values)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in values)
				sb.Append(s + ",");
			return AddAttribute(node, name, sb.ToString());
		}
		public static XmlAttribute AddAttribute<T>(XmlNode node, string name, List<T> values)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T s in values)
				sb.Append(s.ToString() + ",");
			return AddAttribute(node, name, sb.ToString());
		}
		public static XmlAttribute AddAttribute<T>(XmlNode node, string name, T[] values)
		{
			return AddAttribute<T>(node, name, values.ToList());
		}

		public static string ReadLabel(XmlNode node)
		{
			return ReadString(node, "Label");
		}
		public static string ReadString(XmlNode node, string attributeName)
		{
			return node.Attributes[attributeName].Value;
		}
		public static string[] ReadStrings(XmlNode node, string attributeName)
		{
			return node.Attributes[attributeName].Value.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
		}
		public static double ReadDouble(XmlNode node, string attributeName)
		{
			double d = 0;
			if (double.TryParse(node.Attributes[attributeName].Value, out d))
				return d;
			else return Double.NaN;
		}
		public static int ReadInt(XmlNode node, string attributeName)
		{
			int d = 0;
			if (int.TryParse(node.Attributes[attributeName].Value, out d))
				return d;
			else return int.MaxValue;
		}

		//public static List<T> ReadArray<T>(XmlNode node, string attributeName) 
		//{
		//	List<T> list = new List<T>();
		//	string[] items = ReadStrings(node, attributeName);
		//	foreach (string s in items)
		//		list.Add(s as T);
		//	return list;
		//}
}