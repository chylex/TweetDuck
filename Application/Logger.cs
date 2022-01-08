using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using TweetLib.Core;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class Logger : IAppLogger {
		private string LogFilePath => Path.Combine(App.StoragePath, logFileName);

		private readonly string logFileName;

		public Logger(string logFileName) {
			this.logFileName = logFileName;
		}

		bool IAppLogger.Debug(string message) {
			#if DEBUG
			return Log("DEBUG", message);
			#else
			return Configuration.Arguments.HasFlag(Configuration.Arguments.ArgLogging) && Log("DEBUG", message);
			#endif
		}

		bool IAppLogger.Info(string message) {
			return Log("INFO", message);
		}

		bool IAppLogger.Error(string message) {
			return Log("ERROR", message);
		}

		bool IAppLogger.OpenLogFile() {
			try {
				using (Process.Start(LogFilePath)) {}
			} catch (Exception) {
				return false;
			}

			return true;
		}

		private bool Log(string level, string message) {
			#if DEBUG
			Debug.WriteLine("[" + level + "] " + message);
			#endif

			string logFilePath = LogFilePath;

			StringBuilder build = new StringBuilder();

			if (!File.Exists(logFilePath)) {
				build.Append("Please, report all issues to: https://github.com/chylex/TweetDuck/issues\r\n\r\n");
			}

			build.Append("[").Append(DateTime.Now.ToString("G", Lib.Culture)).Append("]\r\n");
			build.Append(message).Append("\r\n\r\n");

			try {
				File.AppendAllText(logFilePath, build.ToString(), Encoding.UTF8);
				return true;
			} catch {
				return false;
			}
		}
	}
}
