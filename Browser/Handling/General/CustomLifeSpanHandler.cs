using System;
using CefSharp;
using CefSharp.Handler;
using TweetDuck.Controls;
using TweetDuck.Utils;

namespace TweetDuck.Browser.Handling.General {
	sealed class CustomLifeSpanHandler : LifeSpanHandler {
		private static bool IsPopupAllowed(string url) {
			return url.StartsWith("https://twitter.com/teams/authorize?", StringComparison.Ordinal) ||
			       url.StartsWith("https://accounts.google.com/", StringComparison.Ordinal) ||
			       url.StartsWith("https://appleid.apple.com/", StringComparison.Ordinal);
		}

		public static bool HandleLinkClick(IWebBrowser browserControl, WindowOpenDisposition targetDisposition, string targetUrl) {
			switch (targetDisposition) {
				case WindowOpenDisposition.NewBackgroundTab:
				case WindowOpenDisposition.NewForegroundTab:
				case WindowOpenDisposition.NewPopup when !IsPopupAllowed(targetUrl):
				case WindowOpenDisposition.NewWindow:
					browserControl.AsControl().InvokeAsyncSafe(() => BrowserUtils.OpenExternalBrowser(targetUrl));
					return true;

				default:
					return false;
			}
		}

		protected override bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser) {
			newBrowser = null;
			return HandleLinkClick(browserControl, targetDisposition, targetUrl);
		}

		protected override bool DoClose(IWebBrowser browserControl, IBrowser browser) {
			return false;
		}
	}
}
