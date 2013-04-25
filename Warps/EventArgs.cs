using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warps
{
	public class EventArgs<T> : EventArgs
	{
		private T t;
		public EventArgs()
		{ }
		public EventArgs(T t)
		{
			this.t = t;
		}

		public T Value
		{
			get { return t; }
			set { t = value; }
		}
	}
	public class EventArgs<T, P> : EventArgs
	{
		private readonly T m_t;
		private readonly P m_p;
		public EventArgs(T t, P p)
		{
			m_t = t;
			m_p = p;
		}
		public T ValueT { get { return m_t; } }
		public P ValueP { get { return m_p; } }
	}
}
