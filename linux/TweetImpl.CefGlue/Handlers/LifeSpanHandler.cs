using System.Reflection;
using Lunixo.ChromiumGtk.Core;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;
using static TweetLib.Browser.CEF.Logic.LifeSpanHandlerLogic.TargetDisposition;

namespace TweetImpl.CefGlue.Handlers {
	sealed class LifeSpanHandler : CefLifeSpanHandler {
		public LifeSpanHandlerLogic Logic { get; }

		private readonly WebBrowser core;

		public LifeSpanHandler(WebBrowser core, IPopupHandler popupHandler) {
			this.core = core;
			this.Logic = new LifeSpanHandlerLogic(popupHandler);
		}

		protected override void OnAfterCreated(CefBrowser browser) {
			core.GetType().GetMethod("OnCreated", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(core, new object[] { browser });
		}

		protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings browserSettings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess) {
			return Logic.OnBeforePopup(targetUrl, ConvertTargetDisposition(targetDisposition));
		}

		protected override bool DoClose(CefBrowser browser) {
			return Logic.DoClose();
		}

		public static LifeSpanHandlerLogic.TargetDisposition ConvertTargetDisposition(CefWindowOpenDisposition targetDisposition) {
			return targetDisposition switch {
				CefWindowOpenDisposition.NewBackgroundTab => NewBackgroundTab,
				CefWindowOpenDisposition.NewForegroundTab => NewForegroundTab,
				CefWindowOpenDisposition.NewPopup         => NewPopup,
				CefWindowOpenDisposition.NewWindow        => NewWindow,
				_                                         => Other
			};
		}
	}
}
