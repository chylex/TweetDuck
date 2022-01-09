using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefFrameAdapter : IFrameAdapter<CefFrame> {
		public static CefFrameAdapter Instance { get; } = new ();

		private CefFrameAdapter() {}

		public bool IsValid(CefFrame frame) {
			return frame.IsValid;
		}

		public bool IsMain(CefFrame frame) {
			return frame.IsMain;
		}

		public void LoadUrl(CefFrame frame, string url) {
			frame.LoadUrl(url);
		}

		public void ExecuteJavaScriptAsync(CefFrame frame, string script, string identifier, int startLine = 1) {
			frame.ExecuteJavaScript(script, identifier, startLine);
		}
	}
}
