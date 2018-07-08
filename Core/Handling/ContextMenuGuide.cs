using CefSharp;
using TweetDuck.Core.Other.Analytics;

namespace TweetDuck.Core.Handling{
    sealed class ContextMenuGuide : ContextMenuBase{
        public ContextMenuGuide(AnalyticsFile.IProvider analytics) : base(analytics){}

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();
            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);
            AddDebugMenuItems(model);
        }
    }
}
