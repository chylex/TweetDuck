using System.Windows.Forms;
using CefSharp;

namespace TweetDuck.Core.Handling{
    sealed class KeyboardHandlerBrowser : IKeyboardHandler{
        private readonly FormBrowser form;

        public KeyboardHandlerBrowser(FormBrowser form){
            this.form = form;
        }

        bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut){
            if (type == KeyType.RawKeyDown && (Keys)windowsKeyCode == Keys.Space){
                return form.ToggleVideoPause();
            }

            return false;
        }

        bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey){
            return false;
        }
    }
}
