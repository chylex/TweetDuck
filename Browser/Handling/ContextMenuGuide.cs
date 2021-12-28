using CefSharp;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetDuck.Browser.Handling {
	sealed class ContextMenuGuide : ContextMenuBase {
		public ContextMenuGuide(IContextMenuHandler handler) : base(handler) {}

		public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);
			AddDebugMenuItems(model);
		}
	}
}
