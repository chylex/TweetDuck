using System;
using TweetLib.Utils.Collections;

namespace TweetDuck.Configuration {
	static class Arguments {
		// public args
		public const string ArgDataFolder = "-datafolder";
		public const string ArgLogging = "-log";
		public const string ArgIgnoreGDPR = "-nogdpr";
		public const string ArgHttpVideo = "-httpvideo";
		public const string ArgFreeze = "-freeze";
		public const string ArgHideDeprecation = "-hidedeprecation";

		// internal args
		public const string ArgRestart = "-restart";
		public const string ArgUpdated = "-updated";

		// class data and methods
		private static readonly CommandLineArgs Current = CommandLineArgs.FromStringArray('-', Environment.GetCommandLineArgs());

		public static bool HasFlag(string flag) {
			return Current.HasFlag(flag);
		}

		public static string? GetValue(string key) {
			return Current.GetValue(key);
		}

		public static CommandLineArgs GetCurrentClean() {
			CommandLineArgs args = Current.Clone();
			args.RemoveFlag(ArgRestart);
			args.RemoveFlag(ArgUpdated);
			return args;
		}

		private static CommandLineArgs GetCurrentForInstaller() {
			CommandLineArgs args = GetCurrentClean();
			args.AddFlag(ArgUpdated);
			return args;
		}

		public static string GetCurrentForInstallerCmd() {
			return GetCurrentForInstaller().ToString().Replace("\"", "::");
		}
	}
}
