using CefSharp;
using CefSharp.Handler;
using TweetLib.Core;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Handling {
	sealed class CustomLifeSpanHandler : LifeSpanHandler {
		public static bool HandleLinkClick(WindowOpenDisposition targetDisposition, string targetUrl) {
			switch (targetDisposition) {
				case WindowOpenDisposition.NewBackgroundTab:
				case WindowOpenDisposition.NewForegroundTab:
				case WindowOpenDisposition.NewPopup when !TwitterUrls.IsAllowedPopupUrl(targetUrl):
				case WindowOpenDisposition.NewWindow:
					App.SystemHandler.OpenBrowser(targetUrl);
					return true;

				default:
					return false;
			}
		}

		protected override bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser) {
			newBrowser = null;
			return HandleLinkClick(targetDisposition, targetUrl);
		}

		protected override bool DoClose(IWebBrowser browserControl, IBrowser browser) {
			return false;
		}
	}
}
