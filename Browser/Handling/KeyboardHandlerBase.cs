using System.Windows.Forms;
using CefSharp;
using TweetDuck.Utils;

namespace TweetDuck.Browser.Handling {
	class KeyboardHandlerBase : IKeyboardHandler {
		protected virtual bool HandleRawKey(IWebBrowser browserControl, Keys key, CefEventFlags modifiers) {
			if (modifiers == (CefEventFlags.ControlDown | CefEventFlags.ShiftDown) && key == Keys.I) {
				browserControl.OpenDevToolsCustom();
				return true;
			}

			return false;
		}

		bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut) {
			if (type == KeyType.RawKeyDown) {
				using var frame = browser.FocusedFrame;

				if (!frame.Url.StartsWith("devtools://")) {
					return HandleRawKey(browserControl, (Keys) windowsKeyCode, modifiers);
				}
			}

			return false;
		}

		bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey) {
			return false;
		}
	}
}
