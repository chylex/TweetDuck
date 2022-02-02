using System;
using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefAdapter : ICefAdapter {
		public static CefAdapter Instance { get; } = new CefAdapter();

		private CefAdapter() {}

		public void RunOnUiThread(Action action) {
			Cef.UIThreadTaskFactory.StartNew(action);
		}
	}
}
