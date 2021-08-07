using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Notification;
using TweetDuck.Controls;

namespace TweetDuck.Browser.Handling {
	sealed class KeyboardHandlerNotification : KeyboardHandlerBase {
		private readonly FormNotificationBase notification;

		public KeyboardHandlerNotification(FormNotificationBase notification) {
			this.notification = notification;
		}

		private void TriggerKeyboardShortcutAnalytics() {
			notification.InvokeAsyncSafe(notification.AnalyticsFile.NotificationKeyboardShortcuts.Trigger);
		}

		protected override bool HandleRawKey(IWebBrowser browserControl, Keys key, CefEventFlags modifiers) {
			if (base.HandleRawKey(browserControl, key, modifiers)) {
				return true;
			}

			switch (key) {
				case Keys.Enter:
					notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
					TriggerKeyboardShortcutAnalytics();
					return true;

				case Keys.Escape:
					notification.InvokeAsyncSafe(notification.HideNotification);
					TriggerKeyboardShortcutAnalytics();
					return true;

				case Keys.Space:
					notification.InvokeAsyncSafe(() => notification.FreezeTimer = !notification.FreezeTimer);
					TriggerKeyboardShortcutAnalytics();
					return true;

				default:
					return false;
			}
		}
	}
}
