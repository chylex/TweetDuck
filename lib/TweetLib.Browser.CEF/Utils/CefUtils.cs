using System.IO;
using System.Text.RegularExpressions;
using TweetLib.Utils.Collections;

namespace TweetLib.Browser.CEF.Utils {
	public static class CefUtils {
		public static string GetCacheFolder(string storagePath) {
			return Path.Combine(storagePath, "Cache");
		}

		public static CommandLineArgs ParseCommandLineArguments(string argumentString) {
			CommandLineArgs args = new CommandLineArgs();

			if (string.IsNullOrWhiteSpace(argumentString)) {
				return args;
			}

			foreach (Match match in Regex.Matches(argumentString, @"([^=\s]+(?:=(?:\S*""[^""]*?""\S*|\S*))?)")) {
				string matchValue = match.Value;

				int indexEquals = matchValue.IndexOf('=');
				string key, value;

				if (indexEquals == -1) {
					key = matchValue.TrimStart('-');
					value = "1";
				}
				else {
					key = matchValue.Substring(0, indexEquals).TrimStart('-');
					value = matchValue.Substring(indexEquals + 1).Trim('"');
				}

				if (key.Length != 0) {
					args.SetValue(key, value);
				}
			}

			return args;
		}
	}
}
