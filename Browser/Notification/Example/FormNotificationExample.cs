using System;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Controls;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Utils;

namespace TweetDuck.Browser.Notification.Example {
	sealed class FormNotificationExample : FormNotificationMain {
		public override bool RequiresResize => true;
		protected override bool CanDragWindow => Config.NotificationPosition == DesktopNotification.Position.Custom;

		protected override FormBorderStyle NotificationBorderStyle {
			get {
				if (Config.NotificationSize == DesktopNotification.Size.Custom) {
					switch (base.NotificationBorderStyle) {
						case FormBorderStyle.FixedSingle: return FormBorderStyle.Sizable;
						case FormBorderStyle.FixedToolWindow: return FormBorderStyle.SizableToolWindow;
					}
				}

				return base.NotificationBorderStyle;
			}
		}

		protected override string BodyClasses => base.BodyClasses + " td-example";

		public event EventHandler Ready;

		private readonly DesktopNotification exampleNotification;

		public FormNotificationExample(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, false) {
			browser.LoadingStateChanged += browser_LoadingStateChanged;

			string exampleTweetHTML = FileUtils.ReadFileOrNull(Path.Combine(Program.ResourcesPath, "notification/example/example.html"))?.Replace("{avatar}", AppLogo.Url) ?? string.Empty;

			#if DEBUG
			exampleTweetHTML = exampleTweetHTML.Replace("</p>", @"</p><div style='margin-top:256px'>Scrollbar test padding...</div>");
			#endif

			exampleNotification = new DesktopNotification(string.Empty, string.Empty, "Home", exampleTweetHTML, 176, string.Empty, string.Empty);
		}

		private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			if (!e.IsLoading) {
				Ready?.Invoke(this, EventArgs.Empty);
				browser.LoadingStateChanged -= browser_LoadingStateChanged;
			}
		}

		public override void HideNotification() {
			Location = ControlExtensions.InvisibleLocation;
		}

		public override void FinishCurrentNotification() {}

		public void ShowExampleNotification(bool reset) {
			if (reset) {
				LoadTweet(exampleNotification);
			}
			else {
				PrepareAndDisplayWindow();
			}

			UpdateTitle();
		}
	}
}
