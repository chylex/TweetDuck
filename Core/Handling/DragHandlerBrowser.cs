using System.Collections.Generic;
using CefSharp;

namespace TweetDuck.Core.Handling{
    sealed class DragHandlerBrowser : IDragHandler{
        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask){
            if (dragData.IsLink){
                browserControl.ExecuteScriptAsync("window.TDGF_onGlobalDragStart", "link", dragData.LinkUrl);
            }
            else{
                browserControl.ExecuteScriptAsync("window.TDGF_onGlobalDragStart", "unknown");
            }

            return false;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions){}
    }
}
