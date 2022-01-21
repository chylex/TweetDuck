using System;
using TweetLib.Browser.Base;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Events;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Resources;
using TweetLib.Core.Systems.Updates;
using TweetLib.Utils.Static;
using Version = TweetDuck.Version;

namespace TweetLib.Core.Features.TweetDeck {
	public sealed class TweetDeckBrowser : BaseBrowser<TweetDeckBrowser> {
		private const string NamespaceTweetDeck = "tweetdeck";
		private const string BackgroundColorOverride = "setTimeout(function f(){let h=document.head;if(!h){setTimeout(f,5);return;}let e=document.createElement('style');e.innerHTML='body,body::before{background:#1c6399!important;margin:0}';h.appendChild(e);},1)";

		public TweetDeckFunctions Functions { get; }
		public FileDownloadManager FileDownloadManager => new (browserComponent);

		private readonly ISoundNotificationHandler soundNotificationHandler;
		private readonly PluginManager pluginManager;

		private bool isBrowserReady;
		private bool ignoreUpdateCheckError;
		private string? prevSoundNotificationPath = null;

		public TweetDeckBrowser(IBrowserComponent browserComponent, ITweetDeckInterface tweetDeckInterface, TweetDeckExtraContext extraContext, ISoundNotificationHandler soundNotificationHandler, PluginManager pluginManager, UpdateChecker? updateChecker = null) : base(browserComponent, CreateSetupObject) {
			this.Functions = new TweetDeckFunctions(this.browserComponent);

			this.soundNotificationHandler = soundNotificationHandler;

			this.pluginManager = pluginManager;
			this.pluginManager.Register(PluginEnvironment.Browser, this.browserComponent);
			this.pluginManager.Reloaded += pluginManager_Reloaded;
			this.pluginManager.Executed += pluginManager_Executed;
			this.pluginManager.Reload();

			this.browserComponent.BrowserLoaded += browserComponent_BrowserLoaded;
			this.browserComponent.PageLoadStart += browserComponent_PageLoadStart;
			this.browserComponent.PageLoadEnd += browserComponent_PageLoadEnd;

			this.browserComponent.AttachBridgeObject("$TD", new TweetDeckBridgeObject(tweetDeckInterface, this, extraContext));

			if (updateChecker != null) {
				updateChecker.CheckFinished += updateChecker_CheckFinished;
				this.browserComponent.AttachBridgeObject("$TDU", updateChecker.InteractionManager.BridgeObject);
			}

			App.UserConfiguration.MuteToggled += UserConfiguration_GeneralEventHandler;
			App.UserConfiguration.OptionsDialogClosed += UserConfiguration_GeneralEventHandler;
			App.UserConfiguration.SoundNotificationChanged += UserConfiguration_SoundNotificationChanged;
		}

		public override void Dispose() {
			base.Dispose();

			this.browserComponent.BrowserLoaded -= browserComponent_BrowserLoaded;
			this.browserComponent.PageLoadStart -= browserComponent_PageLoadStart;
			this.browserComponent.PageLoadEnd -= browserComponent_PageLoadEnd;

			App.UserConfiguration.MuteToggled -= UserConfiguration_GeneralEventHandler;
			App.UserConfiguration.OptionsDialogClosed -= UserConfiguration_GeneralEventHandler;
			App.UserConfiguration.SoundNotificationChanged -= UserConfiguration_SoundNotificationChanged;
		}

		private void browserComponent_BrowserLoaded(object sender, BrowserLoadedEventArgs e) {
			e.AddDictionaryWords("tweetdeck", "TweetDeck", "tweetduck", "TweetDuck", "TD");
			isBrowserReady = true;
		}

		private void browserComponent_PageLoadStart(object sender, PageLoadEventArgs e) {
			string url = e.Url;

			if (TwitterUrls.IsTweetDeck(url) || (TwitterUrls.IsTwitter(url) && !TwitterUrls.IsTwitterLogin2Factor(url))) {
				browserComponent.RunScript("gen:backgroundcolor", BackgroundColorOverride);
			}
		}

		private void browserComponent_PageLoadEnd(object sender, PageLoadEventArgs e) {
			string url = e.Url;

			if (TwitterUrls.IsTweetDeck(url)) {
				NotificationBrowser.SetNotificationLayout(null, null);

				UpdatePropertyObject();
				browserComponent.RunBootstrap(NamespaceTweetDeck);
				pluginManager.Execute(PluginEnvironment.Browser, browserComponent);

				if (App.UserConfiguration.FirstRun) {
					browserComponent.RunBootstrap("introduction");
				}
			}
			else if (TwitterUrls.IsTwitter(url)) {
				browserComponent.RunBootstrap("login");
			}

			browserComponent.RunBootstrap("update");
		}

		private void pluginManager_Reloaded(object sender, PluginErrorEventArgs e) {
			if (e.HasErrors) {
				App.MessageDialogs.Error("Error Loading Plugins", "The following plugins will not be available until the issues are resolved:\n\n" + string.Join("\n\n", e.Errors));
			}

			if (isBrowserReady) {
				ReloadToTweetDeck();
			}
		}

		private void pluginManager_Executed(object sender, PluginErrorEventArgs e) {
			if (e.HasErrors) {
				App.MessageDialogs.Error("Error Executing Plugins", "Failed to execute the following plugins:\n\n" + string.Join("\n\n", e.Errors));
			}
		}

		private void updateChecker_CheckFinished(object sender, UpdateCheckEventArgs e) {
			var updateChecker = (UpdateChecker) sender;

			e.Result.Handle(update => {
				string tag = update.VersionTag;

				if (tag != Version.Tag && tag != App.UserConfiguration.DismissedUpdate) {
					update.BeginSilentDownload();
					Functions.ShowUpdateNotification(tag, update.ReleaseNotes);
				}
				else {
					updateChecker.StartTimer();
				}
			}, ex => {
				if (!ignoreUpdateCheckError) {
					App.ErrorHandler.HandleException("Update Check Error", "An error occurred while checking for updates.", true, ex);
					updateChecker.StartTimer();
				}
			});

			ignoreUpdateCheckError = true;
		}

		private void UserConfiguration_GeneralEventHandler(object sender, EventArgs e) {
			UpdatePropertyObject();
		}

		private void UserConfiguration_SoundNotificationChanged(object? sender, EventArgs e) {
			const string soundUrl = "https://ton.twimg.com/tduck/updatesnd";

			bool hasCustomSound = App.UserConfiguration.IsCustomSoundNotificationSet;
			string newNotificationPath = App.UserConfiguration.NotificationSoundPath;

			if (prevSoundNotificationPath != newNotificationPath) {
				prevSoundNotificationPath = newNotificationPath;
				soundNotificationHandler.Unregister(soundUrl);

				if (hasCustomSound) {
					soundNotificationHandler.Register(soundUrl, newNotificationPath);
				}
			}

			Functions.SetSoundNotificationData(hasCustomSound, App.UserConfiguration.NotificationSoundVolume);
		}

		internal void OnModulesLoaded(string moduleNamespace) {
			if (moduleNamespace == NamespaceTweetDeck) {
				Functions.ReinjectCustomCSS(App.UserConfiguration.CustomBrowserCSS);
				UserConfiguration_SoundNotificationChanged(null, EventArgs.Empty);
			}
		}

		private void UpdatePropertyObject() {
			browserComponent.RunScript("gen:propertyobj", PropertyObjectScript.Generate(App.UserConfiguration, PropertyObjectScript.Environment.Browser));
		}

		public void ReloadToTweetDeck() {
			ignoreUpdateCheckError = false;
			browserComponent.RunScript("gen:reload", $"if(window.TDGF_reload)window.TDGF_reload();else window.location.href='{TwitterUrls.TweetDeck}'");
		}

		private static BrowserSetup CreateSetupObject(TweetDeckBrowser browser) {
			return BaseBrowser.CreateSetupObject(browser.browserComponent, new BrowserSetup {
				ContextMenuHandler = new ContextMenu(browser),
				ResourceRequestHandler = new ResourceRequestHandler()
			});
		}

		private sealed class ContextMenu : BaseContextMenu {
			private readonly TweetDeckBrowser owner;

			public ContextMenu(TweetDeckBrowser owner) : base(owner.browserComponent) {
				this.owner = owner;
			}

			public override void Show(IContextMenuBuilder menu, Context context) {
				if (context.Selection is { Editable: true } ) {
					menu.AddAction("Apply ROT13", owner.Functions.ApplyROT13);
					menu.AddSeparator();
				}

				base.Show(menu, context);

				if (context.Selection == null && context.Tweet is {} tweet) {
					AddOpenAction(menu, "Open tweet in browser", tweet.Url);
					AddCopyAction(menu, "Copy tweet address", tweet.Url);
					menu.AddAction("Screenshot tweet to clipboard", () => owner.Functions.TriggerTweetScreenshot(tweet.ColumnId, tweet.ChirpId));
					menu.AddSeparator();

					if (!string.IsNullOrEmpty(tweet.QuoteUrl)) {
						AddOpenAction(menu, "Open quoted tweet in browser", tweet.QuoteUrl!);
						AddCopyAction(menu, "Copy quoted tweet address", tweet.QuoteUrl!);
						menu.AddSeparator();
					}
				}
			}

			protected override void AddSearchSelectionItems(IContextMenuBuilder menu, string selectedText) {
				base.AddSearchSelectionItems(menu, selectedText);

				if (TwitterUrls.IsTweetDeck(owner.browserComponent.Url)) {
					menu.AddAction("Search in a column", () => {
						owner.Functions.AddSearchColumn(selectedText);
						DeselectAll();
					});
				}
			}
		}

		private sealed class ResourceRequestHandler : BaseResourceRequestHandler {
			private const string UrlLoadingSpinner = "/backgrounds/spinner_blue";
			private const string UrlVendorResource = "/dist/vendor";
			private const string UrlVersionCheck = "/web/dist/version.json";

			public override RequestHandleResult? Handle(string url, ResourceType resourceType) {
				switch (resourceType) {
					case ResourceType.MainFrame when url.EndsWithOrdinal("://twitter.com/"):
						return new RequestHandleResult.Redirect(TwitterUrls.TweetDeck); // redirect plain twitter.com requests, fixes bugs with login 2FA

					case ResourceType.Image when url.Contains(UrlLoadingSpinner):
						return new RequestHandleResult.Redirect("td://resources/images/spinner.apng");

					case ResourceType.Script when url.Contains(UrlVendorResource):
						return new RequestHandleResult.Process(VendorScriptProcessor.Instance);

					case ResourceType.Script when url.Contains("analytics."):
						return RequestHandleResult.Cancel.Instance;

					case ResourceType.Xhr when url.Contains(UrlVersionCheck):
						return RequestHandleResult.Cancel.Instance;

					case ResourceType.Xhr when url.Contains("://api.twitter.com/") && url.Contains("include_entities=1") && !url.Contains("&include_ext_has_nft_avatar=1"):
						return new RequestHandleResult.Redirect(url.Replace("include_entities=1", "include_entities=1&include_ext_has_nft_avatar=1"));

					default:
						return base.Handle(url, resourceType);
				}
			}
		}
	}
}
