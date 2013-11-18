using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	public class Pair<T1, T2>
	{
		T1 m_a;
		T2 m_b;
		public Pair(T1 a, T2 b)
		{
			m_a = a;
			m_b = b;
		}
		public T1 A
		{
			get { return m_a; }
			set { m_a = value; }
		}
		public T2 B
		{
			get { return m_b; }
			set { m_b = value; }
		}
	}
}
