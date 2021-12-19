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
	}
}
