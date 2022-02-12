using CefSharp;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Contexts;

namespace TweetImpl.CefSharp.Handlers {
	public abstract class CefContextMenuHandler : IContextMenuHandler {
		private readonly ContextMenuLogic<IMenuModel> logic;

		protected CefContextMenuHandler(TweetLib.Browser.Interfaces.IContextMenuHandler handler) {
			this.logic = new ContextMenuLogic<IMenuModel>(handler, CefMenuModelAdapter.Instance);
		}

		protected abstract Context CreateContext(IContextMenuParams parameters);

		public virtual void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			logic.OnBeforeContextMenu(model, CreateContext(parameters));
		}

		public virtual bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {
			return logic.OnContextMenuCommand((int) commandId);
		}

		public virtual void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) {
			logic.OnContextMenuDismissed();
		}

		public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) {
			return logic.RunContextMenu();
		}
	}
}
