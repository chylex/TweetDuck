using System.Collections.Generic;
using System.IO;
using CefSharp;
using TweetDuck.Utils;
using TweetLib.Core.Browser;

namespace TweetDuck.Browser.Adapters {
	sealed class CefScriptExecutor : IScriptExecutor {
		private readonly IWebBrowser browser;

		public CefScriptExecutor(IWebBrowser browser) {
			this.browser = browser;
		}

		public void RunFunction(string name, params object[] args) {
			browser.ExecuteJsAsync(name, args);
		}

		public void RunScript(string identifier, string script) {
			using IFrame frame = browser.GetMainFrame();
			RunScript(frame, script, identifier);
		}

		public bool RunFile(string file) {
			using IFrame frame = browser.GetMainFrame();
			return RunFile(frame, file);
		}

		public void RunBootstrap(string moduleNamespace) {
			using IFrame frame = browser.GetMainFrame();
			RunBootstrap(frame, moduleNamespace);
		}

		// Helpers

		public static void RunScript(IFrame frame, string script, string identifier) {
			if (script != null) {
				frame.ExecuteJavaScriptAsync(script, identifier, 1);
			}
		}

		public static bool RunFile(IFrame frame, string file) {
			string script = Program.Resources.Load(file);
			RunScript(frame, script, "root:" + Path.GetFileNameWithoutExtension(file));
			return script != null;
		}

		public static void RunBootstrap(IFrame frame, string moduleNamespace) {
			string script = GetBootstrapScript(moduleNamespace, includeStylesheets: true);

			if (script != null) {
				RunScript(frame, script, "bootstrap");
			}
		}

		public static string GetBootstrapScript(string moduleNamespace, bool includeStylesheets) {
			string script = Program.Resources.Load("bootstrap.js");

			if (script == null) {
				return null;
			}

			string path = Path.Combine(Program.ResourcesPath, moduleNamespace);
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
	}
}
