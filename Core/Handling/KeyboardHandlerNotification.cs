using CefSharp;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other.Analytics;

namespace TweetDuck.Core.Handling {
    sealed class KeyboardHandlerNotification : IKeyboardHandler{
        private readonly FormNotificationBase notification;

        public KeyboardHandlerNotification(FormNotificationBase notification){
            this.notification = notification;
        }

        private void TriggerKeyboardShortcutAnalytics(){
            notification.InvokeAsyncSafe(() => notification.TriggerAnalyticsEvent(AnalyticsFile.Event.NotificationKeyboardShortcut));
        }

        bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut){
            if (type == KeyType.RawKeyDown && !browser.FocusedFrame.Url.StartsWith("chrome-devtools://")){
                switch((Keys)windowsKeyCode){
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
                }
            }

            return false;
        }

        bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey){
            return false;
        }
    }
}
