using System;
using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefAdapter : ICefAdapter {
		public static CefAdapter Instance { get; } = new ();

		private CefAdapter() {}

		public void RunOnUiThread(Action action) {
			Cef.UIThreadTaskFactory.StartNew(action);
		}
	}
}
