using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefDragHandler : IDragHandler {
		private readonly DragHandlerLogic<IDragData, IRequest> logic;

		public CefDragHandler(CefRequestHandler requestHandler, IScriptExecutor executor) {
			this.logic = new DragHandlerLogic<IDragData, IRequest>(executor, requestHandler.Logic, CefDragDataAdapter.Instance);
		}

		public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask) {
			return logic.OnDragEnter(dragData);
		}

		public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IList<DraggableRegion> regions) {}
	}
}
