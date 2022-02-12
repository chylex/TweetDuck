using CefSharp.WinForms;
using TweetDuck.Utils;
using TweetImpl.CefSharp.Component;
using TweetLib.Browser.Base;
using TweetLib.Browser.CEF.Utils;
using TweetLib.Core;

namespace TweetDuck.Browser.Base {
	sealed class CefBrowserComponent : BrowserComponentBase {
		private static readonly CreateContextMenu DefaultContextMenuFactory = handler => new ContextMenuBase(handler);

		public override string CacheFolder => CefUtils.GetCacheFolder(App.StoragePath);
		
		public CefBrowserComponent(ChromiumWebBrowser browser, CreateContextMenu createContextMenu = null, bool autoReload = true) : base(browser, createContextMenu ?? DefaultContextMenuFactory, PopupHandler.Instance, autoReload) {
			browser.SetupZoomEvents();	
		}

		public override void Setup(BrowserSetup setup) {
			base.Setup(setup);
			browser.JsDialogHandler = new CustomJsDialogHandler();
		}
	}
}
