using Gtk;
using Lunixo.ChromiumGtk;
using TweetImpl.CefGlue.Component;
using TweetLib.Browser.CEF.Utils;
using TweetLib.Core;

namespace TweetDuck.Browser.Base {
	sealed class CefBrowserComponent : BrowserComponentBase {
		public override string CacheFolder => CefUtils.GetCacheFolder(App.StoragePath);

		public CefBrowserComponent(Window window, WebView view, CreateContextMenu createContextMenu, bool autoReload = true) : base(window, view.Browser, createContextMenu, autoReload) {}
	}
}
