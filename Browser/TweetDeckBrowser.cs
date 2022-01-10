using System;
using System.Drawing;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser.Adapters;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Notification;
using TweetDuck.Configuration;
using TweetDuck.Utils;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Updates;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;
using TweetDeckBrowserImpl = TweetLib.Core.Features.TweetDeck.TweetDeckBrowser;

namespace TweetDuck.Browser {
	sealed class TweetDeckBrowser : IDisposable {
		public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);

		public bool Ready => browserComponent.Ready;

		public bool Enabled {
			get => browser.Enabled;
			set => browser.Enabled = value;
		}

		public bool IsTweetDeckWebsite {
			get {
				if (!Ready) {
					return false;
				}

				using IFrame frame = browser.GetBrowser().MainFrame;
				return TwitterUrls.IsTweetDeck(frame.Url);
			}
		}

		public TweetDeckFunctions Functions => browserImpl.Functions;

		private readonly CefBrowserComponent browserComponent;
		private readonly TweetDeckBrowserImpl browserImpl;
		private readonly ChromiumWebBrowser browser;

		public TweetDeckBrowser(FormBrowser owner, PluginManager pluginManager, ITweetDeckInterface tweetDeckInterface, UpdateChecker updateChecker) {
			RequestHandlerBrowser requestHandler = new RequestHandlerBrowser();

			this.browser = new ChromiumWebBrowser(TwitterUrls.TweetDeck) {
				DialogHandler = new FileDialogHandler(),
				DragHandler = new DragHandlerBrowser(requestHandler),
				KeyboardHandler = new CustomKeyboardHandler(owner),
				RequestHandler = requestHandler
			};

			// ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
			this.browser.BrowserSettings.BackgroundColor = (uint) BackgroundColor.ToArgb();

			var extraContext = new TweetDeckExtraContext();
			var resourceHandlerRegistry = new CefResourceHandlerRegistry();
			var soundNotificationHandler = new SoundNotification(resourceHandlerRegistry);

			this.browserComponent = new ComponentImpl(browser, owner, extraContext, resourceHandlerRegistry);
			this.browserImpl = new TweetDeckBrowserImpl(browserComponent, tweetDeckInterface, extraContext, soundNotificationHandler, pluginManager, updateChecker);

			if (Arguments.HasFlag(Arguments.ArgIgnoreGDPR)) {
				browserComponent.PageLoadEnd += (sender, args) => {
					if (TwitterUrls.IsTweetDeck(args.Url)) {
						browserComponent.RunScript("gen:gdpr", "TD.storage.Account.prototype.requiresConsent = function() { return false; }");
					}
				};
			}

			owner.Controls.Add(browser);
		}

		private sealed class ComponentImpl : CefBrowserComponent {
			private readonly FormBrowser owner;
			private readonly TweetDeckExtraContext extraContext;
			private readonly CefResourceHandlerRegistry registry;

			public ComponentImpl(ChromiumWebBrowser browser, FormBrowser owner, TweetDeckExtraContext extraContext, CefResourceHandlerRegistry registry) : base(browser) {
				this.owner = owner;
				this.extraContext = extraContext;
				this.registry = registry;
			}

			protected override ContextMenuBase SetupContextMenu(IContextMenuHandler handler) {
				return new ContextMenuBrowser(owner, handler, extraContext);
			}

			protected override CefResourceHandlerFactory SetupResourceHandlerFactory(IResourceRequestHandler handler) {
				return new CefResourceHandlerFactory(handler, registry);
			}
		}

		public void PrepareSize(Size size) {
			if (!Ready) {
				browser.Size = size;
			}
		}

		public void Dispose() {
			browserImpl.Dispose();
			browser.Dispose();
		}

		public void Focus() {
			browser.Focus();
		}

		public void OpenDevTools() {
			browser.OpenDevToolsCustom();
		}

		public void ReloadToTweetDeck() {
			browserImpl.ReloadToTweetDeck();
		}

		public void SaveVideo(string url, string username) {
			browserImpl.FileDownloadManager.SaveVideo(url, username);
		}

		public void HideVideoOverlay(bool focus) {
			if (focus) {
				browser.GetBrowser().GetHost().SendFocusEvent(true);
			}

			browserComponent.RunScript("gen:hidevideo", "$('#td-video-player-overlay').remove()");
		}
	}
}
