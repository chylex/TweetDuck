using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefFrameAdapter : IFrameAdapter<IFrame> {
		public static CefFrameAdapter Instance { get; } = new CefFrameAdapter();

		private CefFrameAdapter() {}

		public bool IsValid(IFrame frame) {
			return frame.IsValid;
		}

		public bool IsMain(IFrame frame) {
			return frame.IsMain;
		}

		public void LoadUrl(IFrame frame, string url) {
			frame.LoadUrl(url);
		}

		public void ExecuteJavaScriptAsync(IFrame frame, string script, string identifier, int startLine = 1) {
			frame.ExecuteJavaScriptAsync(script, identifier, startLine);
		}
	}
}
