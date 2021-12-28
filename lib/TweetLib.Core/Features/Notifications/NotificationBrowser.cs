using System;
using TweetLib.Browser.Base;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Twitter;

namespace TweetLib.Core.Features.Notifications {
	public abstract partial class NotificationBrowser : BaseBrowser<NotificationBrowser> {
		public const string BlankURL = TwitterUrls.TweetDeck + "/?blank";

		internal static void SetNotificationLayout(string? fontSize, string? headLayout) {
			FontSize = fontSize;
			HeadLayout = headLayout;
		}

		public static string? FontSize { get; private set; }
		public static string? HeadLayout { get; private set; }

		private NotificationBrowser(IBrowserComponent browserComponent, Func<NotificationBrowser, BrowserSetup> setup) : base(browserComponent, setup) {}

		public abstract string GetTweetHTML(DesktopNotification notification);
	}
}
