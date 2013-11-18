using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBF
{
	public interface ICenter
	{
		double radius(double[] p);
		double w { get; set; }
		double this[int i] { get; set; }
		double[] ToArray();
	}

	public class Center3d : ICenter
	{
		public Center3d(double x, double y, double w)
		{
			m_pt = new double[] { x, y };
			m_weight = w;
		}
		public Center3d(double x, double y)
			: this(x, y, 0)
		{ }
		Center3d()
			: this(double.MinValue, double.MinValue, double.MinValue)
		{ }

		Center3d(double[] pt, double weight)
		{
			pt.CopyTo(m_pt, 0);
			m_weight = weight;
		}
		double[] m_pt = new double[2];
		double m_weight;

		#region ICenter Members

		public double radius(double[] p)
		{
			return Math.Sqrt(Math.Pow(p[0] - this[0], 2) + Math.Pow(p[1] - this[1], 2));
		}

		public double w
		{
			get { return m_weight; }
			set { m_weight = value; }
		}

		public double this[int i]
		{
			get
			{
				return m_pt[i];
			}
			set
			{
				m_pt[i] = value;
			}
		}

		public double[] ToArray()
		{
			return new double[3] { m_pt[0], m_pt[1], m_weight };
		}

		#endregion

		public override string ToString()
		{
			return string.Format("({0}, {1}) {2}", m_pt[0], m_pt[1], m_weight);
		}
	}
	public class Center2d : ICenter
	{
		public Center2d(double x, double w)
		{
			m_pt = new double[] { x, w };
		}
		public Center2d(double x)
			: this(x, 0)
		{ }
		Center2d(double[] pt)
			:this(pt[0], pt[1])
		{ }

		double[] m_pt = new double[2];

		#region ICenter Members

		public double radius(double[] p)
		{
			return Math.Abs(p[0] - this[0]);
		}

		public double w
		{
			get { return m_pt[1]; }
			set { m_pt[1] = value; }
		}

		public double this[int i]
		{
			get
			{
				return m_pt[i];
			}
			set
			{
				m_pt[i] = value;
			}
		}

		public double[] ToArray()
		{
			return new double[2] { m_pt[0], m_pt[1] };
		}

		#endregion

		public override string ToString()
		{
			return string.Format("({0}) {1}", m_pt[0], m_pt[1]);
		}
	}

	//public class CenterArray 
	//{
	//	CenterArrayNode() : base("Centers", null) { }
	//	public CenterArrayNode(NsNode parent, System.Xml.XmlNode xml)
	//		: base(parent, xml)
	//	{
	//		//Parent = parent;
	//		////Label = "Centers";
	//		//if(!FromXml(xml))
	//		//     throw new AttributeXmlFormatException(null, xml, "Failed to read xml");
	//	}
	//	public CenterArrayNode(NsNode parent)
	//		: base("Centers", parent)
	//	{ }
	//	public CenterArrayNode(NsNode parent, List<ICenter<double>> centerdata)
	//		: base("Centers", parent)
	//	{
	//		if (centerdata != null)
	//			foreach (ICenter<double> center in centerdata)
	//				Add(center);
	//	}

	//	public List<IAttribute> Centers
	//	{
	//		get { return Attributes; }
	//		set
	//		{
	//			PauseUpdating();
	//			Attributes.Clear();
	//		 value.ForEach(delegate(IAttribute atr)
	//		 {
	//			Add(atr);
	//		 });
	//			//foreach (IAttribute atr in value)
	//			//	Add(atr);
	//			ResumeUpdating(false);
	//		}
	//	}

	//	public void ClearCenters()
	//	{
	//		Attributes.Clear();
	//	}

	//	public new ICenter<double> this[int i]
	//	{
	//		get { return Attributes[i] as ICenter<double>; }
	//		set { Attributes[i] = value; }
	//	}

	//   public List<double[]> CentersAsList
	//   {
	//	  get
	//	  {
	//		 List<double[]> ret = new List<double[]>(Centers.Count);
	//		 Centers.ForEach(delegate(IAttribute center)
	//		 {
	//			ret.Add((double[])center.Value);
	//		 });
	//		 //foreach (IAttribute center in Centers)
	//		 //    ret.Add((double[])center.Value);

	//		 return ret;
	//	  }
	//   }
	//}
}
