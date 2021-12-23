using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Adapters;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Browser.Notification.Screenshot {
	sealed class FormNotificationScreenshotable : FormNotificationBase {
		protected override bool CanDragWindow => false;

		private readonly PluginManager plugins;
		private int height;

		public FormNotificationScreenshotable(Action callback, FormBrowser owner, PluginManager pluginManager, string html, int width) : base(owner, false) {
			this.plugins = pluginManager;

			int realWidth = BrowserUtils.Scale(width, DpiScale);

			browser.RegisterJsBridge("$TD_NotificationScreenshot", new ScreenshotBridge(this, SetScreenshotHeight, callback));

			browser.LoadingStateChanged += (sender, args) => {
				if (args.IsLoading) {
					return;
				}

				string script = Program.Resources.LoadSilent("notification/screenshot/screenshot.js");

				if (script == null) {
					this.InvokeAsyncSafe(callback);
					return;
				}

				using IFrame frame = args.Browser.MainFrame;
				CefScriptExecutor.RunScript(frame, script.Replace("{width}", realWidth.ToString()).Replace("1/*FRAMES*/", TweetScreenshotManager.WaitFrames.ToString()), "gen:screenshot");
			};

			SetNotificationSize(realWidth, 1024);
			LoadTweet(new DesktopNotification(string.Empty, string.Empty, string.Empty, html, 0, string.Empty, string.Empty));
		}

		protected override string GetTweetHTML(DesktopNotification tweet) {
			return tweet.GenerateHtml("td-screenshot", HeadLayout, Config.CustomNotificationCSS, plugins.NotificationInjections, Array.Empty<string>());
		}

		private void SetScreenshotHeight(int browserHeight) {
			this.height = BrowserUtils.Scale(browserHeight, SizeScale);
		}

		public bool TakeScreenshot(bool ignoreHeightError = false) {
			if (!ignoreHeightError) {
				if (height == 0) {
					FormMessage.Error("Screenshot Failed", "Could not detect screenshot size.", FormMessage.OK);
					return false;
				}
				else if (height > ClientSize.Height) {
					FormMessage.Error("Screenshot Failed", $"Screenshot is too large: {height}px > {ClientSize.Height}px", FormMessage.OK);
					return false;
				}
			}

			if (!WindowsUtils.IsAeroEnabled) {
				MoveToVisibleLocation(); // TODO make this look nicer I guess
			}

			IntPtr context = NativeMethods.GetDC(this.Handle);

			if (context == IntPtr.Zero) {
				FormMessage.Error("Screenshot Failed", "Could not retrieve a graphics context handle for the notification window to take the screenshot.", FormMessage.OK);
				return false;
			}
			else {
				using Bitmap bmp = new Bitmap(ClientSize.Width, Math.Max(1, height), PixelFormat.Format32bppRgb);

				try {
					NativeMethods.RenderSourceIntoBitmap(context, bmp);
				} finally {
					NativeMethods.ReleaseDC(this.Handle, context);
				}

				Clipboard.SetImage(bmp);
				return true;
			}
		}
	}
}
