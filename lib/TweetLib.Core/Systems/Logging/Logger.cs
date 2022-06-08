using System;
using System.IO;
using System.Text;

namespace TweetLib.Core.Systems.Logging {
	public sealed class Logger {
		public string LogFilePath { get; }

		private readonly bool debug;

		internal Logger(string logPath, bool debug) {
			this.LogFilePath = logPath;
			this.debug = debug;
			#if DEBUG
			this.debug = true;
			#endif
		}

		public bool Debug(string message) {
			return debug && Log("DEBUG", message);
		}

		public bool Info(string message) {
			return Log("INFO", message);
		}

		public bool Warn(string message) {
			return Log("WARN", message);
		}

		public bool Error(string message) {
			return Log("ERROR", message);
		}

		private bool Log(string level, string message) {
			#if DEBUG
			System.Diagnostics.Debug.WriteLine("[" + level + "] " + message);
			#endif

			StringBuilder build = new StringBuilder();

			if (!File.Exists(LogFilePath)) {
				build.Append("Please, report all issues to: ").Append(Lib.IssueTrackerUrl).Append("\r\n\r\n");
			}

			build.Append('[').Append(DateTime.Now.ToString("G", Lib.Culture)).Append("] ").Append(level).Append("\r\n");
			build.Append(message).Append("\r\n\r\n");

			try {
				File.AppendAllText(LogFilePath, build.ToString(), Encoding.UTF8);
				return true;
			} catch {
				return false;
			}
		}
	}
}
