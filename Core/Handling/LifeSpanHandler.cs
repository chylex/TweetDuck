using CefSharp;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    class LifeSpanHandler : ILifeSpanHandler{
        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser){
            newBrowser = null;

            switch(targetDisposition){
                case WindowOpenDisposition.SingletonTab: // TODO remove when CefSharp is updated to 57; enums don't line up in 55
                case WindowOpenDisposition.NewBackgroundTab:
                case WindowOpenDisposition.NewForegroundTab:
                case WindowOpenDisposition.NewPopup:
                // TODO case WindowOpenDisposition.NewWindow:
                    BrowserUtils.OpenExternalBrowser(targetUrl);
                    return true;

                default:
                    return false;
            }
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser){}

        public bool DoClose(IWebBrowser browserControl, IBrowser browser){
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser){}
    }
}
