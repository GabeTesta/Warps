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
	public static class Utilities
	{
		public static string ExeDir
		{
			get
			{
				return
					System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			}
		}

		public static void LimitRange(int low, ref int val, int high)
		{
			if (val < low) val = low;
			if (high < val) val = high;
		}
		public static void LimitRange(double low, ref double val, double high)
		{
			if (val < low) val = low;
			if (high < val) val = high;
		}
		public static double LimitRange(double low, double val, double high)
		{
			if (val < low) return low;
			if (high < val) return high;
			return val;
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

		public static object CreateInstance(Type t, params object[] parameters)
		{
			List<Type> types = new List<Type>();
			foreach (object param in parameters)
				types.Add(param.GetType());
			ConstructorInfo ctor = t.GetConstructor(types.ToArray());
			if (ctor != null)
				return ctor.Invoke(parameters);
			return null;
		}
		public static object CreateInstance(string stype)
		{
			return CreateInstance(stype, null);
		}
		public static object CreateInstance(string stype, params object[] paramteters)
		{
			if (!stype.Contains("."))
				stype = "Warps." + stype;
			object o = null;
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in asms)
			{
				//try
				//{
				if( paramteters != null )
					o = asm.CreateInstance(stype, false, BindingFlags.CreateInstance, null,paramteters, null, null);

				else
					o = asm.CreateInstance(stype, false);
				//o = asm.CreateInstance(stype, false, BindingFlags.CreateInstance, null, new object[] { }, null, null);
				if (o != null)
					return o;
				//}
				//catch (Exception e)
				//{
				//	Log("Creating Instance of type:" + xn.Name + " failed\n" + e.Message, LogPriority.Error);
				//}
			}
			return null;
		}

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
				ret = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t != baseType);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Warps.Logger.logger.Instance.LogErrorException(ex); 
			}

			return ret;
		}

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
				Warps.Logger.logger.Instance.LogErrorException(e); 
				System.Windows.Forms.MessageBox.Show("File: [" + path + "]\n" + e.Message, "Failed to launch file");
			}
		}

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

		public static string[] OpenFileDlg(string extension, string title, string initialdir)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if(title != null && title.Length > 0 )
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

		public static Type GetClipboardObjType()
		{
			if (Clipboard.ContainsData(typeof(CurveGroup).Name))
				return typeof(CurveGroup);
			else if (Clipboard.ContainsData(typeof(MouldCurve).Name))
				return typeof(MouldCurve);
			else if (Clipboard.ContainsData(typeof(SurfaceCurve).Name))
				return typeof(SurfaceCurve);
			else if (Clipboard.ContainsData(typeof(Geodesic).Name))
				return typeof(Geodesic);
			else if (Clipboard.ContainsData(typeof(GuideComb).Name))
				return typeof(GuideComb);
			else if (Clipboard.ContainsData(typeof(Equation).Name))
				return typeof(Equation);
			else if (Clipboard.ContainsData(typeof(VariableGroup).Name))
				return typeof(VariableGroup);
			return null;

		}

	}

}
