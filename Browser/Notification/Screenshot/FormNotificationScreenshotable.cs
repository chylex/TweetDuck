using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.DevTools.Page;
using TweetDuck.Browser.Adapters;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Utils;

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

				string script = FileUtils.ReadFileOrNull(Path.Combine(Program.ResourcesPath, "notification/screenshot/screenshot.js"));

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

		public Task<Image> TakeScreenshot(bool ignoreHeightError = false) {
			if (!ignoreHeightError) {
				if (height == 0) {
					FormMessage.Error("Screenshot Failed", "Could not detect screenshot size.", FormMessage.OK);
					return null;
				}
				else if (height > ClientSize.Height) {
					FormMessage.Error("Screenshot Failed", $"Screenshot is too large: {height}px > {ClientSize.Height}px", FormMessage.OK);
					return null;
				}
			}

			return Task.Run(TakeScreenshotImpl);
		}

		private async Task<Image> TakeScreenshotImpl() {
			if (this.height == 0) {
				return null;
			}

			Viewport viewport = new Viewport {
				Width = this.ClientSize.Width,
				Height = this.height,
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
