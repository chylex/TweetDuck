using System.Windows.Forms;
using CefSharp;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Utils;

namespace TweetDuck.Browser.Handling{
    class KeyboardHandlerBase : IKeyboardHandler{
        protected virtual bool HandleRawKey(IWebBrowser browserControl, IBrowser browser, Keys key, CefEventFlags modifiers){
            if (modifiers == (CefEventFlags.ControlDown | CefEventFlags.ShiftDown) && key == Keys.I){
                if (BrowserUtils.HasDevTools){
                    browserControl.OpenDevToolsCustom();
                }
                else{
                    browserControl.AsControl().InvokeSafe(() => {
                        string extraMessage;
                        
                        if (Program.IsPortable){
                            extraMessage = "Please download the portable installer, select the folder with your current installation of TweetDuck Portable, and tick 'Install dev tools' during the installation process.";
                        }
                        else{
                            extraMessage = "Please download the installer, and tick 'Install dev tools' during the installation process. The installer will automatically find and update your current installation of TweetDuck.";
                        }

                        FormMessage.Information("Dev Tools", "You do not have dev tools installed. " + extraMessage, FormMessage.OK);
                    });
                }

                return true;
            }

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
