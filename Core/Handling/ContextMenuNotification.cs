using CefSharp;

namespace TweetDck.Core.Handling{
    class ContextMenuNotification : ContextMenuBase{
        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();

            base.OnBeforeContextMenu(browserControl,browser,frame,parameters,model);
            RemoveSeparatorIfLast(model);
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            return false;
        }
    }
}
