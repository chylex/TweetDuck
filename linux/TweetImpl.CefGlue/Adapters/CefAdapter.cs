using System;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefAdapter : ICefAdapter {
		public static CefAdapter Instance { get; } = new ();

		private CefAdapter() {}

		public void RunOnUiThread(Action action) {
			CefRuntime.PostTask(CefThreadId.UI, new CefActionTask(action));
		}
	}
}
