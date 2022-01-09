using TweetImpl.CefGlue.Adapters;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class DragHandler : CefDragHandler {
		private readonly DragHandlerLogic<CefDragData, CefRequest> logic;

		public DragHandler(RequestHandler requestHandler, IScriptExecutor executor) {
			this.logic = new DragHandlerLogic<CefDragData, CefRequest>(executor, requestHandler.Logic, CefDragDataAdapter.Instance);
		}

		protected override bool OnDragEnter(CefBrowser browser, CefDragData dragData, CefDragOperationsMask mask) {
			return logic.OnDragEnter(dragData);
		}

		protected override void OnDraggableRegionsChanged(CefBrowser browser, CefFrame frame, CefDraggableRegion[] regions) {}
	}
}
