using System;
using System.Globalization;
using System.Text;

namespace TweetLib.Browser.Interfaces {
	public interface IScriptExecutor {
		void RunScript(string identifier, string script);
	}
	
	public static class ScriptExecutorExtensions {
		public static void RunFunction(this IScriptExecutor executor, string name, params object?[] args) {
			executor.RunScript("about:blank", GenerateJavaScriptFunctionCall(name, args));
		}
		
		private static string GenerateJavaScriptFunctionCall(string name, object?[] args) {
			var builder = new StringBuilder();
			builder.Append(name);
			builder.Append('(');

			for (var index = 0; index < args.Length; index++) {
				var obj = args[index];

				switch (obj) {
					case null:
						builder.Append("null");
						break;

					case bool b:
						builder.Append(b.ToString().ToLowerInvariant());
						break;

					case sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal:
						builder.Append(Convert.ToString(obj, CultureInfo.InvariantCulture));
						break;

					default:
						var str = obj.ToString() ?? string.Empty;
						var escaped = str.Replace("\\", "\\\\")
						                 .Replace("'", "\\'")
						                 .Replace("\t", "\\t")
						                 .Replace("\r", "\\r")
						                 .Replace("\n", "\\n");

						builder.Append('\'');
						builder.Append(escaped);
						builder.Append('\'');
						break;
				}

				if (index < args.Length - 1) {
					builder.Append(',');
				}
			}

			return builder.Append(");").ToString();
		}
	}
}
