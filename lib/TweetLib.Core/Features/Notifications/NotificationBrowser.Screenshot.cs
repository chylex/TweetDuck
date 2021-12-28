using System;
using System.Collections.Generic;
using TweetLib.Browser.Base;
using TweetLib.Browser.Interfaces;
using TweetLib.Utils.Data;

namespace TweetLib.Core.Features.Notifications {
	public abstract partial class NotificationBrowser {
		public sealed class Screenshot : NotificationBrowser {
			private readonly IEnumerable<InjectedString> notificationInjections;

			public Screenshot(IBrowserComponent browserComponent, IEnumerable<InjectedString> notificationInjections) : base(browserComponent, CreateSetupObject) {
				this.notificationInjections = notificationInjections;
			}

			public override string GetTweetHTML(DesktopNotification notification) {
				return notification.GenerateHtml("td-screenshot", HeadLayout, App.UserConfiguration.CustomNotificationCSS, notificationInjections, Array.Empty<string>());
			}

			private static BrowserSetup CreateSetupObject(NotificationBrowser browser) {
				return BaseBrowser.CreateSetupObject(browser.browserComponent);
			}
		}
	}
}
