using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.DevTools.Page;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Utils;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Resources;

namespace TweetDuck.Browser.Notification.Screenshot {
	sealed class FormNotificationScreenshotable : FormNotificationBase {
		private static NotificationBrowser CreateBrowserImpl( IBrowserComponent browserComponent, PluginManager pluginManager) {
			return new NotificationBrowser.Screenshot(browserComponent, pluginManager.NotificationInjections);
		}

		protected override bool CanDragWindow => false;

		private int height;

		public FormNotificationScreenshotable(Action callback, FormBrowser owner, PluginManager pluginManager, string html, int width) : base(owner, (_, browserComponent) => CreateBrowserImpl(browserComponent, pluginManager)) {
			int realWidth = BrowserUtils.Scale(width, DpiScale);

			browserComponent.AttachBridgeObject("$TD_NotificationScreenshot", new ScreenshotBridge(this, SetScreenshotHeight, callback));

			browserComponent.BrowserLoaded += (sender, args) => {
				string? script = ResourceUtils.ReadFileOrNull("notification/screenshot/screenshot.js");

				if (script == null) {
					this.InvokeAsyncSafe(callback);
					return;
				}

				string substituted = script.Replace("{width}", realWidth.ToString()).Replace("1/*FRAMES*/", TweetScreenshotManager.WaitFrames.ToString());
				browserComponent.RunScript("gen:screenshot", substituted);
			};

			SetNotificationSize(realWidth, 1024);
			LoadTweet(new DesktopNotification(string.Empty, string.Empty, string.Empty, html, 0, string.Empty, string.Empty));
		}

		private void SetScreenshotHeight(int browserHeight) {
			this.height = BrowserUtils.Scale(browserHeight, SizeScale);
		}

		public Task<Image>? TakeScreenshot() {
			if (height == 0) {
				FormMessage.Error("Screenshot Failed", "Could not detect screenshot size.", FormMessage.OK);
				return null;
			}
			else if (height > ClientSize.Height) {
				FormMessage.Error("Screenshot Failed", $"Screenshot is too large: {height}px > {ClientSize.Height}px", FormMessage.OK);
				return null;
			}

			return Task.Run(TakeScreenshotImpl);
		}

		private async Task<Image> TakeScreenshotImpl() {
			if (height == 0) {
				throw new InvalidOperationException("Screenshot height must not be zero!");
			}

			Viewport viewport = new Viewport {
				Width = ClientSize.Width,
				Height = height,
				Scale = 1
			};

			byte[] data;
			using (var devToolsClient = browser.GetDevToolsClient()) {
				data = (await devToolsClient.Page.CaptureScreenshotAsync(CaptureScreenshotFormat.Png, clip: viewport)).Data;
			}

			return Image.FromStream(new MemoryStream(data));
		}
	}
}
