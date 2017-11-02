﻿using CefSharp;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;

namespace TweetDuck.Core.Handling {
    sealed class KeyboardHandlerNotification : IKeyboardHandler{
        private readonly FormNotificationBase notification;

        public KeyboardHandlerNotification(FormNotificationBase notification){
            this.notification = notification;
        }

        bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut){
            if (type == KeyType.RawKeyDown && !browser.FocusedFrame.Url.StartsWith("chrome-devtools://")){
                switch((Keys)windowsKeyCode){
                    case Keys.Enter:
                        notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
                        return true;

                    case Keys.Escape:
                        notification.InvokeAsyncSafe(notification.HideNotification);
                        return true;

                    case Keys.Space:
                        notification.InvokeAsyncSafe(() => notification.FreezeTimer = !notification.FreezeTimer);
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
