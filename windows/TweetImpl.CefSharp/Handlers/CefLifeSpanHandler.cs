using CefSharp;
using CefSharp.Handler;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.CEF.Logic;
using static TweetLib.Browser.CEF.Logic.LifeSpanHandlerLogic.TargetDisposition;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefLifeSpanHandler : LifeSpanHandler {
		public LifeSpanHandlerLogic Logic { get; }

		public CefLifeSpanHandler(IPopupHandler popupHandler) {
			this.Logic = new LifeSpanHandlerLogic(popupHandler);
		}

		protected override bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser) {
			newBrowser = null;
			return Logic.OnBeforePopup(targetUrl, ConvertTargetDisposition(targetDisposition));
		}

		protected override bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			return Logic.DoClose();
		}

		public static LifeSpanHandlerLogic.TargetDisposition ConvertTargetDisposition(WindowOpenDisposition targetDisposition) {
			return targetDisposition switch {
				WindowOpenDisposition.NewBackgroundTab => NewBackgroundTab,
				WindowOpenDisposition.NewForegroundTab => NewForegroundTab,
				WindowOpenDisposition.NewPopup         => NewPopup,
				WindowOpenDisposition.NewWindow        => NewWindow,
				_                                      => Other
			};
		}
	}
}
