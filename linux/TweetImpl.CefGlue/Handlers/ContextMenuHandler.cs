using TweetImpl.CefGlue.Adapters;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	public abstract class ContextMenuHandler : CefContextMenuHandler {
		private readonly ContextMenuLogic<CefMenuModel> logic;

		protected ContextMenuHandler(IContextMenuHandler? handler) {
			this.logic = new ContextMenuLogic<CefMenuModel>(handler, CefMenuModelAdapter.Instance);
		}

		protected abstract Context CreateContext(CefContextMenuParams parameters);

		protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model) {
			logic.OnBeforeContextMenu(model, CreateContext(state));
		}

		protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags) {
			return logic.OnContextMenuCommand(commandId);
		}

		protected override void OnContextMenuDismissed(CefBrowser browser, CefFrame frame) {
			logic.OnContextMenuDismissed();
		}

		protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback) {
			return logic.RunContextMenu();
		}
	}
}
