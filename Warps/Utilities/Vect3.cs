using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Warps
{
	[Serializable()]
	public class Vect3
	{
		#region Indexer

		public double this[int i]
		{
			get
			{
				Debug.Assert(i < 3 && i >= 0);
				return m_vec[i];
			}
			set
			{
				Debug.Assert(i < 3 && i >= 0);
				m_vec[i] = value;
			}
		}
		public double x
		{
			get { return this[0]; }
			set { this[0] = value; }
		}
		public double y
		{
			get { return this[1]; }
			set { this[1] = value; }
		}
		public double z
		{
			get { return this[2]; }
			set { this[2] = value; }
		}
		public double Length
		{ get { Debug.Assert(m_vec.Length == 3); return 3; } }
		internal double[] m_vec = new double[3];

		#endregion

		#region Constructors

		public Vect3(Vect3 vec)
			: this(vec[0], vec[1], vec[2]) { }
		public Vect3(double x, double y, double z)
		{
			this[0] = x;
			this[1] = y;
			this[2] = z;
		}
		public Vect3(IList<double> vec)
		{
			Set(vec);
		}
		public Vect3()
		{
			Zero();
		}

		#endregion

		#region Initialzers

		public void Set(Vect3 vec)
		{
			Set(vec.m_vec);
		}
		public void Set(IList<double> vec)
		{
			Debug.Assert(vec.Count >= 3);
			this[0] = vec[0];
			this[1] = vec[1];
			this[2] = vec[2];
		}
		public void Zero()
		{
			this[0] = this[1] = this[2] = 0;
		}
		public void Scale(double d)
		{
			this[0] *= d;
			this[1] *= d;
			this[2] *= d;
		}

		#endregion

		#region Operators

		//additon/subtraction
		public static Vect3 operator +(Vect3 a, Vect3 b)
		{
			Debug.Assert(a.Length == b.Length);
			return new Vect3(a[0] + b[0], a[1] + b[1], a[2] + b[2]);
		}
		public static Vect3 operator -(Vect3 a, Vect3 b)
		{
			Debug.Assert(a.Length == b.Length);
			return new Vect3(a[0] - b[0], a[1] - b[1], a[2] - b[2]);
		}
		//scaling
		public static Vect3 operator *(Vect3 a, double d)
		{
			Debug.Assert(a.Length == 3);
			return new Vect3(a[0] * d, a[1] * d, a[2] * d);
		}
		public static Vect3 operator /(Vect3 a, double d)
		{
			Debug.Assert(a.Length == 3);
			return new Vect3(a[0] / d, a[1] / d, a[2] / d);
		}

		//equalities
		readonly static double TOL = 1e-7;
		public static bool operator ==(Vect3 a, Vect3 b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(Vect3 a, Vect3 b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is Vect3))
				return false;
			Vect3 b = obj as Vect3;
			Debug.Assert(this.Length == b.Length);
			return BLAS.is_equal(this[0], b[0], TOL)
				&& BLAS.is_equal(this[1], b[1], TOL)
				&& BLAS.is_equal(this[2], b[2], TOL);
		}
		public override int GetHashCode()
		{
			int x = (int)Math.Round(this[0] / TOL);//round to tolerance used by isequal
			int y = (int)Math.Round(this[1] / TOL);
			int z = (int)Math.Round(this[2] / TOL);
			return x ^ y ^ z;
		}

		//conversions
		//public static explicit operator double[](Vect3 v)
		//{
		//	return v.m_vec;
		//}

		public double[] ToArray()
		{
			return new double[] { this[0], this[1], this[2] };
		}
		public double[] Array
		{
			get { return m_vec; }
		}
		#endregion

		#region Magnitude
		/// <summary>
		/// Get or Set the Magnitude of the vector
		/// </summary>
		public double Magnitude
		{
			get { return Math.Sqrt(Norm); }
			set { Scale(value / Magnitude); }
		}
		/// <summary>
		/// Get the Euclidian Norm (Magnitude Squared)
		/// </summary>
		public double Norm
		{
			get { return Dot(this); }
		}
		/// <summary>
		/// Sets the Magnitude to 1
		/// </summary>
		public void Unitize()
		{
			Magnitude = 1;
		}
		#endregion

		#region Distance
		/// <summary>
		/// Find the distance between this vector and another
		/// </summary>
		/// <param name="v">the target vector</param>
		/// <returns>the distance between these two vectors</returns>
		public double Distance(Vect3 v)
		{
			return Distance(this, v);
		}
		/// <summary>
		/// Find the distance between 2 vectors
		/// </summary>
		/// <param name="a">the starting vector</param>
		/// <param name="b">the target vector</param>
		/// <returns>the distance between them</returns>
		public static double Distance(Vect3 a, Vect3 b)
		{
			return (a - b).Magnitude;
		}
		#endregion

		#region Vector Algebra
		/// <summary>
		/// Dot this vector with another
		/// </summary>
		/// <param name="v">the vector to dot with</param>
		/// <returns>this dot v</returns>
		public double Dot(Vect3 v)
		{
			return Dot(this, v);
		}
		/// <summary>
		/// Cross this vector into another
		/// </summary>
		/// <param name="v">the vector to cross with</param>
		/// <returns>this x v</returns>
		public Vect3 Cross(Vect3 v)
		{
			return Cross(this, v);
		}
		/// <summary>
		/// Dot product of two vectors
		/// </summary>
		/// <param name="a">first vector</param>
		/// <param name="b">second vector</param>
		/// <returns>a dot b</returns>
		public static double Dot(Vect3 a, Vect3 b)
		{
			Debug.Assert(a.Length == b.Length);
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}
		/// <summary>
		/// Cross product of two vectors
		/// </summary>
		/// <param name="a">left hand vector</param>
		/// <param name="b">right hand vector</param>
		/// <returns>a x b</returns>
		public static Vect3 Cross(Vect3 a, Vect3 b)
		{
			Debug.Assert(a.Length == b.Length);
			return new Vect3(
				a[1] * b[2] - a[2] * b[1], 
				a[2] * b[0] - a[0] * b[2], 
				a[0] * b[1] - a[1] * b[0]);
		}

		/// <summary>
		/// Rotate this vector about an axis by a specified angle
		/// </summary>
		/// <param name="axis">the axis of rotation</param>
		/// <param name="rad">the angle in radians to rotate</param>
		/// <returns>a new rotated vector</returns>
		public Vect3 Rotate(Vect3 axis, double rad)
		{
			return Rotate(this, axis, rad);
		}
		/// <summary>
		/// Rotate a vector about an axis by a specified angle
		/// </summary>
		/// <param name="v">the vector to rotate</param>
		/// <param name="a">the axis of rotation</param>
		/// <param name="rad">the angle in radians to rotate</param>
		/// <returns>a new rotated vector</returns>
		public static Vect3 Rotate(Vect3 v, Vect3 a, double rad)
		{
			Debug.Assert(a.Length == v.Length);
			//http://inside.mines.edu/~gmurray/ArbitraryAxisRotation/
			//rot = a(a*v)(1-cos) + v*cos + (aXv)sin
			double dot = v.Dot(a);
			double cos = Math.Cos(rad);
			double sin = Math.Sin(rad);
			return new Vect3(
				a.x * dot * (1 - cos) + v.x * cos + (v.z * a.y - v.y * a.z) * sin,
				a.y * dot * (1 - cos) + v.y * cos + (v.x * a.z - v.z * a.x) * sin,
				a.z * dot * (1 - cos) + v.z * cos + (v.y * a.x - v.x * a.y) * sin);
		}
		#endregion

		#region ToString
		public override string ToString()
		{
			return ToString(false);
		}
		public string ToString(bool brackets)
		{
			Debug.Assert(m_vec.Length == 3);
			if (brackets)
				return String.Format("<{0}, {1}, {2}>", this[0], this[1], this[2]);
			else
				return String.Format("{0}, {1}, {2}", this[0], this[1], this[2]);
		}
		public string ToString(string frmt)
		{
			return String.Format("{0}, {1}, {2}", this[0].ToString(frmt), this[1].ToString(frmt), this[2].ToString(frmt));
		}

		#endregion
	}
}
