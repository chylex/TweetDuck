using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Core.Resources {
	public static class ResourceUtils {
		public static string? ReadFileOrNull(string relativePath) {
			string path = Path.Combine(App.ResourcesPath, relativePath);

			try {
				return File.ReadAllText(path, Encoding.UTF8);
			} catch (Exception e) {
				App.Logger.Error("Error reading file: " + path);
				App.Logger.Error(e.ToString());
				return null;
			}
		}

		internal static string? GetBootstrapScript(string moduleNamespace, bool includeStylesheets) {
			string? script = ReadFileOrNull("bootstrap.js");

			if (script == null) {
				return null;
			}

			string path = Path.Combine(App.ResourcesPath, moduleNamespace);
			var files = new DirectoryInfo(path).GetFiles();

			var moduleNames = new List<string>();
			var stylesheetNames = new List<string>();

			foreach (var file in files) {
				var ext = Path.GetExtension(file.Name);

				var targetList = ext switch {
					".js"  => moduleNames,
					".css" => includeStylesheets ? stylesheetNames : null,
					_      => null
				};

				targetList?.Add(Path.GetFileNameWithoutExtension(file.Name));
			}

			script = script.Replace("{{namespace}}", moduleNamespace);
			script = script.Replace("{{modules}}", string.Join("|", moduleNames));
			script = script.Replace("{{stylesheets}}", string.Join("|", stylesheetNames));

			return script;
		}

		internal static void RunBootstrap(this IScriptExecutor executor, string moduleNamespace) {
			var script = GetBootstrapScript(moduleNamespace, includeStylesheets: true);
			if (script != null) {
				executor.RunScript("bootstrap", script);
			}
		}
	}
}
