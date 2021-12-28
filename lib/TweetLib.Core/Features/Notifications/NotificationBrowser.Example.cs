using TweetLib.Browser.Base;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Plugins;

namespace TweetLib.Core.Features.Notifications {
	public abstract partial class NotificationBrowser {
		public sealed class Example : Tweet {
			protected override string BodyClasses => base.BodyClasses + " td-example";

			public Example(IBrowserComponent browserComponent, INotificationInterface notificationInterface, ICommonInterface commonInterface, PluginManager pluginManager) : base(browserComponent, notificationInterface, commonInterface, pluginManager, CreateSetupObject) {}

			private static BrowserSetup CreateSetupObject(NotificationBrowser browser) {
				return BaseBrowser.CreateSetupObject(browser.browserComponent);
			}
		}
	}
}
