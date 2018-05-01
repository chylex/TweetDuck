using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;

namespace TweetDuck.Core.Handling{
    sealed class DragHandlerBrowser : IDragHandler{
        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask){
            void TriggerDragStart(string type, string data = null){
                browserControl.ExecuteScriptAsync("window.TDGF_onGlobalDragStart", type, data);
            }

            if (dragData.IsLink){
                TriggerDragStart("link", dragData.LinkUrl);
            }
            else if (dragData.IsFragment){
                TriggerDragStart("text", dragData.FragmentText.Trim());
            }
            else{
                TriggerDragStart("unknown");
            }

            return false;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions){}
    }
}
