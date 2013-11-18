using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;


	[Serializable()]
	public class Vect2
	{
		#region Indexer

		public double this[int i]
		{
			get
			{
				Debug.Assert(i < 2 && i >= 0);
				return m_vec[i];
			}
			set
			{
				Debug.Assert(i < 2 && i >= 0);
				m_vec[i] = value;
			}
		}
		public double u
		{
			get { return this[0]; }
			set { this[0] = value; }
		}
		public double v
		{
			get { return this[1]; }
			set { this[1] = value; }
		}
		public double Length
		{ get { Debug.Assert(m_vec.Length == 2); return 2; } }
		public double[] m_vec = new double[2];

		#endregion

		#region Constructors

		public Vect2(Vect2 vec)
			: this(vec[0], vec[1]) { }
		public Vect2(double u, double v)
		{
			this[0] = u;
			this[1] = v;
		}
		public Vect2(IList<double> vec)
		{
			Set(vec);
		}
		public Vect2(PointF pnt)
			: this(pnt.X, pnt.Y) { }
		public Vect2()
		{
			Zero();
		}
		public Vect2(string xy)
		{
			FromString(xy);
		}
		public Vect2(double radAngle) : this(Math.Cos(radAngle), Math.Sin(radAngle)) { }
		public Vect3 ToVect3() { return new Vect3(u, v, 0); }

		#endregion

		#region Initialzers

		public void Set(Vect2 vec)
		{
			Set(vec.m_vec);
		}
		public void Set(Vect3 vec3)
		{
			this[0] = vec3[0];
			this[1] = vec3[1];
		}
		public void Set(IList<double> vec)
		{
			Debug.Assert(vec.Count >= 2);
			this[0] = vec[0];
			this[1] = vec[1];
		}
		public void Set(double u, double v)
		{
			this[0] = u;
			this[1] = v;
		}
		public void Zero()
		{
			this[0] = this[1] = 0;
		}
		public void Scale(double d)
		{
			this[0] *= d;
			this[1] *= d;
		}

		#endregion

		#region Operators

		//additon/subtraction
		public static Vect2 operator +(Vect2 a, Vect2 b)
		{
			Debug.Assert(a.Length == b.Length);
			return new Vect2(a[0] + b[0], a[1] + b[1]);
		}
		public static Vect2 operator -(Vect2 a, Vect2 b)
		{
			Debug.Assert(a.Length == b.Length);
			return new Vect2(a[0] - b[0], a[1] - b[1]);
		}
		//scaling
		public static Vect2 operator *(Vect2 a, double d)
		{
			Debug.Assert(a.Length == 2);
			return new Vect2(a[0] * d, a[1] * d);
		}
		public static Vect2 operator /(Vect2 a, double d)
		{
			Debug.Assert(a.Length == 2);
			return new Vect2(a[0] / d, a[1] / d);
		}

		//equalities
		readonly static double TOL = 1e-7;
		public static bool operator ==(Vect2 a, Vect2 b)
		{
			if( System.Object.ReferenceEquals(a, null) )
				return System.Object.ReferenceEquals(b, null);//a and b are null, thus ==
			return a.Equals(b);
		}
		public static bool operator !=(Vect2 a, Vect2 b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is Vect2))
				return false;
			Vect2 b = obj as Vect2;
			Debug.Assert(this.Length == b.Length);
			return BLAS.IsEqual(this[0], b[0], TOL) && BLAS.IsEqual(this[1], b[1], TOL);
		}
		public override int GetHashCode()
		{
			int u = (int)Math.Round(this[0] / TOL);//round to tolerance used by isequal
			int v = (int)Math.Round(this[1] / TOL);
			return u ^ v;
		}

		//conversions
		public static explicit operator double[](Vect2 v)
		{
			return v.m_vec;
		}
		public double[] ToArray()
		{
			return new double[] { this[0], this[1] };
		}
		//public double[] Array
		//{
		//	get { return m_vec; }
		//}
		#endregion

		#region Magnitude
		/// <summary>
		/// Get or Set the Magnitude of the vector
		/// </summary>
		public double Magnitude
		{
			get { return Math.Sqrt(Norm); }
			set { if(Magnitude != 0 ) Scale(value / Magnitude); }
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
		public double Distance(Vect2 v)
		{
			return Distance(this, v);
		}
		/// <summary>
		/// Find the distance between 2 vectors
		/// </summary>
		/// <param name="a">the starting vector</param>
		/// <param name="b">the target vector</param>
		/// <returns>the distance between them</returns>
		public static double Distance(Vect2 a, Vect2 b)
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
		public double Dot(Vect2 v)
		{
			return Dot(this, v);
		}
		/// <summary>
		/// Cross this vector into another
		/// </summary>
		/// <param name="v">the vector to cross with</param>
		/// <returns>this x v</returns>
		public double Cross(Vect2 v)
		{
			return Cross(this, v);
		}
		/// <summary>
		/// Dot product of two vectors
		/// </summary>
		/// <param name="a">first vector</param>
		/// <param name="b">second vector</param>
		/// <returns>a dot b</returns>
		public static double Dot(Vect2 a, Vect2 b)
		{
			Debug.Assert(a.Length == b.Length);
			return a[0] * b[0] + a[1] * b[1];
		}
		/// <summary>
		/// Cross product of two vectors
		/// </summary>
		/// <param name="a">left hand vector</param>
		/// <param name="b">right hand vector</param>
		/// <returns>a x b</returns>
		public static double Cross(Vect2 a, Vect2 b)
		{
			Debug.Assert(a.Length == b.Length);
			return a[0] * b[1] - a[1] * b[0];
		}

		public Vect2 Rotate(double rad)
		{
			return Rotate(this, rad);
		}
		public static Vect2 Rotate(Vect2 v, double rad)
		{
			Vect2 rot0 = new Vect2(Math.Cos(rad), -Math.Sign(rad)),
				rot1 = new Vect2(Math.Sign(rad), Math.Cos(rad));

			return new Vect2(v.Dot(rot0), v.Dot(rot1));
		}

		public Vect2 Normal(){ return new Vect2(-v, u); }

		/// <summary>
		/// Calculate the angle between two vectors
		/// </summary>
		/// <param name="B">the vector to determine the angle between</param>
		/// <returns>The angle between this and B in radians</returns>
		public double AngleTo(Vect2 B)
		{
			return Math.Acos(this.Dot(B) / (Magnitude * B.Magnitude));
		}
		#endregion

		#region ToString
		public override string ToString()
		{
			return ToString(false, null);
		}
		public string ToString(bool brackets, string frmt)
		{
			if (frmt == null)
				frmt = "g";
			Debug.Assert(m_vec.Length == 2);
			if (brackets)
				return String.Format("<{0}, {1}>", this[0].ToString(frmt), this[1].ToString(frmt));
			else
				return String.Format("{0}, {1}", this[0].ToString(frmt), this[1].ToString(frmt));
		}
		public string ToString(string frmt)
		{
			return String.Format("{0}, {1}", this[0].ToString(frmt), this[1].ToString(frmt));
		}
		public bool FromString(string str)
		{
			bool bsuccess = false;
			string[] splits = str.Split('<', '>', ',');
			if (splits.Length == 2)
			{
				bsuccess = true;
				for (int i = 0; i < 2; i++)
				{
					bsuccess &= double.TryParse(splits[i], out m_vec[i]);
				}
			}
			return bsuccess;
		}
		#endregion

		#region BinFile

		public void WriteBin(System.IO.BinaryWriter bin)
		{
			bin.Write(u);
			bin.Write(v);
		}

		#endregion
	}

