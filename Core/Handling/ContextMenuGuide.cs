using CefSharp;

namespace TweetDuck.Core.Handling{
    sealed class ContextMenuGuide : ContextMenuBase{
        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();
            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            if (HasDevTools){
                AddSeparator(model);
                AddDebugMenuItems(model);
            }
        }
    }
}
