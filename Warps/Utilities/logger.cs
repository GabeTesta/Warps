using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;

namespace Warps.Logger
{
	public enum LogPriority
	{
		Message,
		Warning,
		Error,
		Debug
	};

	/// <summary>
	/// ThreadSafe Singleton Logger Implementation
	/// </summary>
	public class logger
	{
		private static volatile logger instance;
		private static object syncRoot = new Object();

		private logger() { }

		public static logger Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new logger();
							m_tsEntriesI = Queue.Synchronized(m_EntriesInput);
							//logger.Instance.Cleanup(20);
							//logger.Instance.Path = Directory.GetCurrentDirectory() + "\\Logs\\" + now;
						}
					}
				}

				return instance;
			}
		}

		public bool Close
		{
			set { m_quit = value; }
		}

		/// <summary>
		/// create a log in a Log folder in the executing directory
		/// </summary>
		/// <param name="fileName">name of the log file (timeStamp will be appended)</param>
		public void CreateLogLocal(string fileName){
			string now = string.Format( fileName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.log", DateTime.Now);
			logger.Instance.Cleanup(Directory.GetCurrentDirectory() + "\\Logs\\", System.IO.Path.GetFileNameWithoutExtension(fileName), 20);
			logger.Instance.Path = Directory.GetCurrentDirectory() + "\\Logs\\" + now;
		}

		/// <summary>
		/// create a log in a Log folder located in the directory parsed from fullFileName
		/// </summary>
		/// <param name="fileName">FULL name of the log file (timeStamp will be appended)</param>
		public void CreateLogAt(string fullFileName)
		{
			string now = string.Format(System.IO.Path.GetFileName(fullFileName) + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.log", DateTime.Now);
			logger.Instance.Cleanup(System.IO.Path.GetFullPath(fullFileName) + "\\Logs\\", System.IO.Path.GetFileNameWithoutExtension(fullFileName), 20);
			logger.Instance.Path = System.IO.Path.GetFullPath(fullFileName) + "\\Logs\\" + now;
		}

		/// <summary>
		/// We don't want to have a million log files, so we are going to have a rolling count of the last intput size logs
		/// </summary>
		public void Cleanup(string fullDirectoryPath, string fileName, int size)
		{
			string dir = fullDirectoryPath;

			if (!Directory.Exists(dir))
				return;

			DirectoryInfo DI = new DirectoryInfo(dir);

			FileInfo[] files = DI.GetFiles();

			int logfileCount = 0;

			foreach (FileInfo f in files)
			{
				if (f.FullName.ToLower().Contains(fileName.ToLower()) 
					&& System.IO.Path.GetExtension(f.FullName) == ".log" 
					&& !f.FullName.ToLower().Contains("copy"))
					logfileCount++;
			}

			if (logfileCount + 1 > size)
			{
				// Now read the creation time for each file
				DateTime[] creationTimes = new DateTime[files.Length];
				for (int i = 0; i < files.Length; i++)
					creationTimes[i] = files[i].CreationTime;

				// sort it
				Array.Sort(creationTimes, files);
				Array.Reverse(creationTimes);

				if (creationTimes.Length + 1 < size)
					return;

				List<FileInfo> toBdeleted = new List<FileInfo>();

				for (int i = size - 1; i < creationTimes.Length; i++)
					for (int j = 0; j < files.Length; j++)
						if (files[j].CreationTime == creationTimes[i])
							toBdeleted.Add(files[j]);

				for (int i = 0; i < toBdeleted.Count; i++)
					File.Delete(toBdeleted[i].FullName);

			}
		}

		public void Log(string message, LogPriority p)
		{
			m_mutex.WaitOne();
			m_tsEntriesI.Enqueue(new Entry(message, p));
			m_mutex.ReleaseMutex();
		}

		public void LogErrorException(Exception ex)
		{
			List<string> data = new List<string>();
			data.Add(" | Exception: ");
			data.Add(" | Message: " + ex.Message);
			if(ex.InnerException != null)
				data.Add(" | InnerException.Message: " + ex.InnerException.Message);
			data.Add(" | Source: " + ex.Source);
			data.Add(" | StackTrace: " + ex.StackTrace);
			data.Add(" | TargetSite: " + ex.TargetSite);
			//string[] data = new string[]{
			//		"Exception: ",
			//		"Message: " + ex.Message,
			//		"InnerException.Message: " + ex.InnerException.Message,
			//		"Source: " + ex.Source, 
			//		"StackTrace: " + ex.StackTrace,
			//		"TargetSite: " + ex.TargetSite
			//	};

			string message = string.Join("\n", data.ToArray());

			m_mutex.WaitOne();
			m_tsEntriesI.Enqueue(new Entry(message, LogPriority.Error));
			m_mutex.ReleaseMutex();
		}

		public void Log(string message)
		{
			Log(message, LogPriority.Debug);
		}

		#region ILogger Members

		public void OnLog(object sender, EventArgs<string, LogPriority> e)
		{
			Log(e.ValueT, e.ValueP);
		}
		public void OnClear(object sender, EventArgs<string> e)
		{
			m_mutex.WaitOne();
			m_tsEntriesI.Clear();
			m_mutex.ReleaseMutex();
		}

		public string[] Messages
		{
			get { return null; }
		}

		public Entry[] Entries
		{
			get
			{
				Queue tmp = new Queue(m_tsEntriesI);
				List<Entry> ret = new List<Entry>(tmp.Count);

				while (tmp.Count > 0)
					ret.Add(tmp.Dequeue() as Entry);

				return ret.ToArray();
			}
		}

		public string Path
		{
			get { return m_path; }
			set
			{
				m_path = value;
				m_quit = false;
				if (m_first)
				{
					if (File.Exists(m_path))
						File.Delete(m_path);
					m_first = false;
				}

				if (m_path != null)
				{
					Thread filewriter = new Thread(() =>
					{
						if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
							Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

						m_txt = TextWriter.Synchronized(new StreamWriter(Path, true));

						Console.WriteLine("logger starting...");

						while (!m_quit)
						{
							m_mutex.WaitOne();
							if(m_tsEntriesI.Count > 0)
								Console.WriteLine("logger writing {0} entries to log...", m_tsEntriesI.Count);

							while (m_tsEntriesI.Count > 0)
							{
								Entry tmp = m_tsEntriesI.Dequeue() as Entry;
								m_txt.WriteLine(tmp.ToString());
							}

							m_mutex.ReleaseMutex();
							m_txt.Flush();
							Thread.Sleep(5000);
							//Console.WriteLine("logger running...");

						}

						m_txt.Close();
						Console.WriteLine("logger stopped");
						FileInfo f = new FileInfo(Path);
						if (f.Length == 0)
							File.Delete(Path);
					});


					filewriter.IsBackground = true; // THIS IS IMPORTANT.  Without this, this thread will run indefinitely even after the main program has closed
					filewriter.Start();
				}

			}
		}

		/// <summary>
		/// Manually stop the output file thread.  Once stopped, it can be restarted by setting a new Path
		/// </summary>
		public bool Stop
		{
			get { return m_quit; }
			set { m_quit = value; }
		}

		public int Count
		{
			get { return m_tsEntriesI.Count; }
		}

		TextWriter m_txt;

		static Queue m_EntriesInput = new Queue();
		static Queue m_tsEntriesI = new Queue();

		Mutex m_mutex = new Mutex(false);

		bool m_quit = false;

		bool m_first = true;

		string m_path;

		public event EventHandler LogChanged;

		public int Errors
		{
			get
			{
				int cnt = 0;
				foreach (Entry e in m_tsEntriesI)
				{
					if (e.Priority == LogPriority.Error)
						cnt++;
				}
				return cnt;
			}
		}

		#endregion
	}

	public class Entry
	{
		public Entry(string msg, LogPriority priority)
		{
			m_msg = msg;
			m_time = DateTime.Now;
			m_priority = priority;
		}

		private readonly string m_msg;
		private DateTime m_time;
		private LogPriority m_priority;

		public override string ToString()
		{
			return string.Format("[{0}] {1}: {2}", m_time.ToLongTimeString(), m_priority.ToString(), m_msg);
		}
		public LogPriority Priority
		{
			get { return m_priority; }
		}
		public string Message
		{
			get { return m_msg; }
		}
		public DateTime Time
		{
			get { return m_time; }
		}
	}
}

