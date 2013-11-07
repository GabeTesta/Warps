using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Geometry;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Warps
{
	public delegate Point3D Transformer(Point3D pt);

	public static class Utilities
	{
		#region Exe Properties

		/// <summary>
		/// get the execution directory
		/// </summary>
		public static string ExeDir
		{
			get
			{
				return
					System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			}
		}

		/// <summary>
		/// get the current version of Warps
		/// </summary>
		public static string CurVersion
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		#endregion

		#region Settings

		static VAkos.Xmlconfig m_ConfigFile;
		public static void OpenConfigFile(string path)
		{
			m_ConfigFile = new VAkos.Xmlconfig(Path.Combine(ExeDir, path), true);
		}
		public static void CloseSettingsFile()
		{
			m_ConfigFile.Commit();
		}
		public static VAkos.ConfigSetting Settings
		{
			get { return Utilities.m_ConfigFile.Settings; }
		}

		#endregion

		#region Limits

		/// <summary>
		/// ensures a value is between limits
		/// </summary>
		/// <param name="low">the minimum inclusive limit</param>
		/// <param name="val">in: the value to check, out: the limited value</param>
		/// <param name="high">the maximium inclusive limit</param>
		public static void LimitRange(int low, ref int val, int high)
		{
			if (val < low) val = low;
			if (high < val) val = high;
		}
		/// <summary>
		/// ensures a value is between limits
		/// </summary>
		/// <param name="low">the minimum inclusive limit</param>
		/// <param name="val">the value to check</param>
		/// <param name="high">the maximium inclusive limit</param>
		public static void LimitRange(double low, ref double val, double high)
		{
			if (val < low) val = low;
			if (high < val) val = high;
		}
		/// <summary>
		/// ensures a value is between limits
		/// </summary>
		/// <param name="low">the minimum inclusive limit</param>
		/// <param name="val">the value to check</param>
		/// <param name="high">the maximium inclusive limit</param>
		/// <returns>the limited value</returns>
		public static double LimitRange(double low, double val, double high)
		{
			if (val < low) return low;
			if (high < val) return high;
			return val;
		}


		/// <summary>
		/// test if  a value is between two others
		/// </summary>
		/// <param name="a">one limit</param>
		/// <param name="val">the value</param>
		/// <param name="b">the other limit</param>
		/// <returns>true if inside(inclusive), false otherwise</returns>
		public static bool IsBetween(double a, double val, double b)
		{
			return a > b ? (b <= val && val <= a) : (a <= val && val <= b);
		}

		#endregion

		#region Conversions

		public static double DegToRad(double deg) { return deg * Math.PI / 180.0; }
		public static double RadToDeg(double rad) { return rad * 180.0 / Math.PI; }

		public static Point3D DoubleToPoint3D(double[] xyz)
		{
			return new Point3D(xyz);
		}
		public static void DoubleToPoint3D(ref Point3D pnt, double[] xyz)
		{
			pnt.X = xyz[0];
			pnt.Y = xyz[1];
			pnt.Z = xyz[2];
		}
		public static void Vect3ToPoint3D(ref Point3D pnt, Vect3 xyz)
		{
			pnt.X = xyz.x;
			pnt.Y = xyz.y;
			pnt.Z = xyz.z;
		}
		public static Point3D Vect3ToPoint3D(Vect3 xyz)
		{
			return new Point3D(xyz.Array);
		}
		public static PointRGB Vect3ToPointRGB(Vect3 xyz, System.Drawing.Color c)
		{
			return new PointRGB(xyz.x, xyz.y, xyz.z, c);
		}

		#endregion

		#region CreateInstance
		public static T CreateInstance<T>() where T : class { return CreateInstance<T>(typeof(T), new object[]{}); }
		public static T CreateInstance<T>(Type t, params object[] parameters) where T : class
		{
			List<Type> types = new List<Type>();
			if (parameters != null)
				foreach (object param in parameters)
					types.Add(param.GetType());
			ConstructorInfo ctor = t.GetConstructor(types.ToArray());
			if (ctor != null)
				return ctor.Invoke(parameters) as T;
			return null;
		}

		//public static object CreateInstance(Type t, params object[] parameters)
		//{
		//	List<Type> types = new List<Type>();
		//	if (parameters != null)
		//		foreach (object param in parameters)
		//			types.Add(param.GetType());
		//	ConstructorInfo ctor = t.GetConstructor(types.ToArray());
		//	if (ctor != null)
		//		return ctor.Invoke(parameters);
		//	return null;
		//}

		public static T CreateInstance<T>(string stype) where T : class
		{
			return CreateInstance<T>(stype, null);
		}
		//public static object CreateInstance(string stype)
		//{
		//	return CreateInstance(stype, null);
		//}

		public static T CreateInstance<T>(string stype, params object[] parameters) where T : class
		{
			bool bShort = !stype.Contains(".");
			//if (!stype.Contains("."))
			//{
			//	stype = "Warps." + stype;
			//}
			object o = null;
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in asms)
			{
				if (bShort)
				{
					try
					{
						Type[] typs = asm.GetTypes();
						foreach (Type t in typs)
							if (t.Name == stype)
								return CreateInstance<T>(t, parameters);
					}
					catch (Exception e) { Logleton.TheLog.Log(e.Message, Logleton.LogPriority.Debug); }
				}
				else
				{
					//try
					//{
					if (parameters != null)
						o = asm.CreateInstance(stype, false, BindingFlags.CreateInstance, null, parameters, null, null);

					else
						o = asm.CreateInstance(stype, true);
					//o = asm.CreateInstance(stype, false, BindingFlags.CreateInstance, null, new object[] { }, null, null);
					if (o != null)
						return o as T;
				}
				//}
				//catch (Exception e)
				//{
				//	Log("Creating Instance of type:" + xn.Name + " failed\n" + e.Message, LogPriority.Error);
				//}
			}
			return null;
		}

		#endregion

		#region FindTypes

		public static List<Type> GetAllOf(Type baseType, bool bInclude)
		{
			List<Type> atrs = new List<Type>();
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			IEnumerable<Type> ret;
			foreach (Assembly asm in asms)
			{
				ret = FindDerivedTypes(asm, baseType, bInclude);
				if (ret != null)
					atrs.AddRange(ret);
			}
			return atrs;
		}
		public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType, bool bInclude)
		{
			IEnumerable<Type> ret = null;

			try
			{
				ret = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && (bInclude || t != baseType) && !t.IsInterface);//don't include interfaces since we cannot create them
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Logleton.TheLog.LogErrorException(ex);
			}

			return ret;
		}

		#endregion

		#region Clipboard Serialization

		/// <summary>
		/// This function is required so we can push objects onto the clipboard
		/// </summary>
		/// <param name="objectToSerialize"></param>
		/// <returns></returns>
		public static string Serialize(object objectToSerialize)
		{
			string serialString = null;
			using (System.IO.MemoryStream ms1 = new System.IO.MemoryStream())
			{
				BinaryFormatter b = new BinaryFormatter();
				b.Serialize(ms1, objectToSerialize);
				byte[] arrayByte = ms1.ToArray();
				serialString = Convert.ToBase64String(arrayByte);
			}
			return serialString;
		}

		/// <summary>
		/// This function is required so we can pop objects off the clipboard
		/// </summary>
		/// <param name="serializationString"></param>
		/// <returns></returns>
		public static object DeSerialize(string serializationString)
		{
			object deserialObject = null;
			byte[] arrayByte = Convert.FromBase64String(serializationString);
			using (System.IO.MemoryStream ms1 = new System.IO.MemoryStream(arrayByte))
			{
				BinaryFormatter b = new BinaryFormatter();
				deserialObject = b.Deserialize(ms1);
			}
			return deserialObject;
		}

		public static Type GetClipboardObjType()
		{
			if (Clipboard.ContainsData(typeof(Curves.CurveGroup).Name))
				return typeof(Curves.CurveGroup);
			else if (Clipboard.ContainsData(typeof(Curves.MouldCurve).Name))
				return typeof(Curves.MouldCurve);
			else if (Clipboard.ContainsData(typeof(Curves.GuideComb).Name))
				return typeof(Curves.GuideComb);
			else if (Clipboard.ContainsData(typeof(Equation).Name))
				return typeof(Equation);
			else if (Clipboard.ContainsData(typeof(VariableGroup).Name))
				return typeof(VariableGroup);
			return null;

		}


		#endregion

		#region File Dialogs

		public static string[] OpenFileDlg(string extension, string title, string initialdir)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (title != null && title.Length > 0)
				ofd.Title = title;
			if (extension != null && extension.Length > 0)
			{
				ofd.Filter = string.Format("{0} files (*.{0})|*.{0}|All files (*.*)|*.*", extension.Trim('.'));
				ofd.AddExtension = true;
				ofd.FilterIndex = 0;
			}
			ofd.Multiselect = true;
			if (initialdir != null && initialdir.Length > 0)
				ofd.InitialDirectory = initialdir;

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return ofd.FileNames;
			return null;
		}
		public static string SaveFileDialog(string extension, string title, string initialdir)
		{
			SaveFileDialog ofd = new SaveFileDialog();
			if (title != null && title.Length > 0)
				ofd.Title = title;
			if (extension != null && extension.Length > 0)
			{
				ofd.Filter = string.Format("{0} files (*.{0})|*.{0}|All files (*.*)|*.*", extension.Trim('.'));
				ofd.AddExtension = true;
				ofd.FilterIndex = 0;
			}
			if (initialdir != null && initialdir.Length > 0)
				ofd.InitialDirectory = initialdir;

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return ofd.FileName;
			return null;
		}

		#endregion

		#region C++ Serialization

		public static string ReadCString(System.IO.BinaryReader bin)
		{
			uint len = bin.ReadUInt32();
			if (len == 0)
				return null;
			return new string(bin.ReadChars((int)len)).Trim();
		}
		public static void WriteCString(System.IO.BinaryWriter bin, string str)
		{
			if (str == null)
			{
				bin.Write((UInt32)0);
				return;
			}
			bin.Write((UInt32)(str.Length));
			bin.Write(str.ToCharArray());
		}

		#endregion

		#region Windows Functions

		public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
		{
			if (!destination.Exists)
			{
				destination.Create();
			}

			List<string> failed = new List<string>();
			// Copy all files.
			if (!source.Exists)
				return;

			FileInfo[] files = source.GetFiles();
			if (files != null)
				foreach (FileInfo file in files)
				{
					//try
					//{
					file.CopyTo(Path.Combine(destination.FullName,
						file.Name));
					//}
					//catch (Exception e)
					//{
					//Log(e.Message, LogPriority.Error);
					//}
				}

			// Process subdirectories.
			DirectoryInfo[] dirs = source.GetDirectories();
			if (dirs != null)
				foreach (DirectoryInfo dir in dirs)
				{
					// Get destination directory.
					string destinationDir = Path.Combine(destination.FullName, dir.Name);

					// Call CopyDirectory() recursively.
					CopyDirectory(dir, new DirectoryInfo(destinationDir));
				}
		}

		public static void HandleProcess(string path, EventHandler callback)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = path;
			//if( arguments != null )
			//     p.StartInfo.Arguments = arguments;// "-x";//default to x projection
			//p.StartInfo.UseShellExecute = false;
			//#if DEBUG
			//               p.StartInfo.Verb = "Edit";
			//#else
			//p.StartInfo.Verb = "Open";
			//#endif
			if (callback != null)
			{
				p.EnableRaisingEvents = true;
				p.Exited += callback;
			}
			try
			{
				p.Start();
			}
			catch (Exception e)
			{
				Logleton.TheLog.LogErrorException(e);
				System.Windows.Forms.MessageBox.Show("File: [" + path + "]\n" + e.Message, "Failed to launch file");
			}
		}

		#endregion

		public static void Insert<T>(List<T> group, IRebuild item, IRebuild target) where T : class
		{
			int nTar = group.IndexOf(target as T);
			int nIrb = group.IndexOf(item as T);
			if (nIrb >= 0)//item is already in this group: reorder
				group.Remove(item as T);
			if (!Utilities.IsBetween(0, nTar, group.Count))
				nTar = 0;//insert at head to avoid breaking sequence group.Count;
			group.Insert(nTar, item as T);
		}

		public static System.Drawing.Font Font { get { return new System.Drawing.Font("Consolas", 11F); } }
	}
}
