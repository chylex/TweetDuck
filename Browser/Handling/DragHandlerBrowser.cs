using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;
using TweetDuck.Utils;

namespace TweetDuck.Browser.Handling {
	sealed class DragHandlerBrowser : IDragHandler {
		private readonly RequestHandlerBrowser requestHandler;

		public DragHandlerBrowser(RequestHandlerBrowser requestHandler) {
			this.requestHandler = requestHandler;
		}

		public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask) {
			void TriggerDragStart(string type, string data = null) {
				browserControl.ExecuteJsAsync("window.TDGF_onGlobalDragStart", type, data);
			}

			requestHandler.BlockNextUserNavUrl = dragData.LinkUrl; // empty if not a link

			if (dragData.IsLink) {
				TriggerDragStart("link", dragData.LinkUrl);
			}
			else if (dragData.IsFragment) {
				TriggerDragStart("text", dragData.FragmentText.Trim());
			}
			else {
				TriggerDragStart("unknown");
			}

			return false;
		}

		public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IList<DraggableRegion> regions) {}
	}
}
