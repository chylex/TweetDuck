using System;
using TweetLib.Browser.Base;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Events;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Resources;

namespace TweetLib.Core.Features.Notifications {
	public abstract partial class NotificationBrowser {
		public class Tweet : NotificationBrowser {
			protected virtual string BodyClasses => notificationInterface.IsHovered ? "td-notification td-hover" : "td-notification";

			private readonly INotificationInterface notificationInterface;
			private readonly PluginManager pluginManager;

			protected Tweet(IBrowserComponent browserComponent, INotificationInterface notificationInterface, ICommonInterface commonInterface, PluginManager pluginManager, Func<NotificationBrowser, BrowserSetup> setup) : base(browserComponent, setup) {
				this.browserComponent.PageLoadEnd += BrowserComponentOnPageLoadEnd;
				this.browserComponent.AttachBridgeObject("$TD", new NotificationBridgeObject(notificationInterface, commonInterface));

				this.notificationInterface = notificationInterface;
				this.pluginManager = pluginManager;
				this.pluginManager.Register(PluginEnvironment.Notification, browserComponent);
			}

			public Tweet(IBrowserComponent browserComponent, INotificationInterface notificationInterface, ICommonInterface commonInterface, PluginManager pluginManager) : this(browserComponent, notificationInterface, commonInterface, pluginManager, browser => CreateSetupObject(browser, notificationInterface)) {}

			public override string GetTweetHTML(DesktopNotification notification) {
				return notification.GenerateHtml(BodyClasses, HeadLayout, App.UserConfiguration.CustomNotificationCSS, pluginManager.NotificationInjections, new string[] {
					PropertyObjectScript.Generate(App.UserConfiguration, PropertyObjectScript.Environment.Notification),
					ResourceUtils.GetBootstrapScript("notification", includeStylesheets: false) ?? string.Empty
				});
			}

			private static BrowserSetup CreateSetupObject(NotificationBrowser browser, INotificationInterface notificationInterface) {
				return BaseBrowser.CreateSetupObject(browser.browserComponent, new BrowserSetup {
					ContextMenuHandler = new ContextMenu(browser, notificationInterface)
				});
			}

			private sealed class ContextMenu : BaseContextMenu {
				private readonly INotificationInterface notificationInterface;

				public ContextMenu(NotificationBrowser browser, INotificationInterface notificationInterface) : base(browser.browserComponent) {
					this.notificationInterface = notificationInterface;
				}

				public override void Show(IContextMenuBuilder menu, Context context) {
					base.Show(menu, context);

					menu.AddAction("View detail", notificationInterface.ShowTweetDetail);
					menu.AddAction("Skip tweet", notificationInterface.FinishCurrentNotification);
					menu.AddActionWithCheck("Freeze", notificationInterface.FreezeTimer, () => notificationInterface.FreezeTimer = !notificationInterface.FreezeTimer);
					menu.AddSeparator();

					if (context.Notification is {} notification) {
						AddCopyAction(menu, "Copy tweet address", notification.TweetUrl);

						if (!string.IsNullOrEmpty(notification.QuoteUrl)) {
							AddCopyAction(menu, "Copy quoted tweet address", notification.QuoteUrl!);
						}

						menu.AddSeparator();
					}
				}
			}

			public override void Dispose() {
				base.Dispose();
				this.browserComponent.PageLoadEnd -= BrowserComponentOnPageLoadEnd;
			}

			private void BrowserComponentOnPageLoadEnd(object sender, PageLoadEventArgs e) {
				string url = e.Url;

				if (TwitterUrls.IsTweetDeck(url) && url != BlankURL) {
					pluginManager.Execute(PluginEnvironment.Notification, browserComponent);
				}
			}
		}
	}
}
