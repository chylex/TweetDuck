using System.Windows.Forms;
using CefSharp;

namespace TweetDuck.Core.Handling{
    class KeyboardHandlerBase : IKeyboardHandler{
        protected virtual bool HandleRawKey(IWebBrowser browserControl, IBrowser browser, Keys key, CefEventFlags modifiers){
            return false;
        }

        bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut){
            if (type == KeyType.RawKeyDown && !browser.FocusedFrame.Url.StartsWith("chrome-devtools://")){
                return HandleRawKey(browserControl, browser, (Keys)windowsKeyCode, modifiers);
            }

            return false;
        }

        bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey){
            return false;
        }
    }
}
