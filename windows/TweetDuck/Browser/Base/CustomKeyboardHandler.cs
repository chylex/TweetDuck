using System.Windows.Forms;
using CefSharp;
using TweetDuck.Utils;
using TweetLib.Utils.Static;

namespace TweetDuck.Browser.Base {
	sealed class CustomKeyboardHandler : IKeyboardHandler {
		private readonly IBrowserKeyHandler? handler;

		public CustomKeyboardHandler(IBrowserKeyHandler? handler) {
			this.handler = handler;
		}

		bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut) {
			if (type != KeyType.RawKeyDown) {
				return false;
			}

			using (var frame = browser.FocusedFrame) {
				if (frame.Url.StartsWithOrdinal("devtools://")) {
					return false;
				}
			}

			Keys key = (Keys) windowsKeyCode;

			if (modifiers == (CefEventFlags.ControlDown | CefEventFlags.ShiftDown) && key == Keys.I) {
				browserControl.OpenDevToolsCustom();
				return true;
			}

			return handler != null && handler.HandleBrowserKey(key);
		}

		bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey) {
			return false;
		}

		public interface IBrowserKeyHandler {
			bool HandleBrowserKey(Keys key);
		}
	}
}
