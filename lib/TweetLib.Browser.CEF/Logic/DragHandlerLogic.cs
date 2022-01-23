using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class DragHandlerLogic<TDragData, TRequest> {
		private readonly IScriptExecutor executor;
		private readonly RequestHandlerLogic<TRequest> requestHandlerLogic;
		private readonly IDragDataAdapter<TDragData> dragDataAdapter;

		public DragHandlerLogic(IScriptExecutor executor, RequestHandlerLogic<TRequest> requestHandlerLogic, IDragDataAdapter<TDragData> dragDataAdapter) {
			this.executor = executor;
			this.requestHandlerLogic = requestHandlerLogic;
			this.dragDataAdapter = dragDataAdapter;
		}

		private void TriggerDragStart(string type, string? data = null) {
			executor.RunFunction("window.TDGF_onGlobalDragStart && window.TDGF_onGlobalDragStart", type, data);
		}

		public bool OnDragEnter(TDragData dragData) {
			var link = dragDataAdapter.GetLink(dragData);
			requestHandlerLogic.BlockNextUserNavUrl = link;

			if (dragDataAdapter.IsLink(dragData)) {
				TriggerDragStart("link", link);
			}
			else if (dragDataAdapter.IsFragment(dragData)) {
				TriggerDragStart("text", dragDataAdapter.GetFragmentAsText(dragData).Trim());
			}
			else {
				TriggerDragStart("unknown");
			}

			return false;
		}
	}
}
