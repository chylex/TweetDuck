using CefSharp;
using TweetDuck.Controls;
using TweetDuck.Utils;

namespace TweetDuck.Browser.Handling.General{
    sealed class LifeSpanHandler : ILifeSpanHandler{
        private static bool IsPopupAllowed(string url){
            return url.StartsWith("https://twitter.com/teams/authorize?");
        }

        public static bool HandleLinkClick(IWebBrowser browserControl, WindowOpenDisposition targetDisposition, string targetUrl){
            switch(targetDisposition){
                case WindowOpenDisposition.NewBackgroundTab:
                case WindowOpenDisposition.NewForegroundTab:
                case WindowOpenDisposition.NewPopup when !IsPopupAllowed(targetUrl):
                case WindowOpenDisposition.NewWindow:
                    browserControl.AsControl().InvokeAsyncSafe(() => BrowserUtils.OpenExternalBrowser(targetUrl));
                    return true;

                default:
                    return false;
            }
        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser){
            newBrowser = null;
            return HandleLinkClick(browserControl, targetDisposition, targetUrl);
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser){}

        public bool DoClose(IWebBrowser browserControl, IBrowser browser){
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser){}
    }
}
