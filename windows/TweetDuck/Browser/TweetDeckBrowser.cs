using System;
using System.Drawing;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser.Base;
using TweetDuck.Browser.Notification;
using TweetDuck.Configuration;
using TweetDuck.Utils;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Updates;
using TweetDeckBrowserImpl = TweetLib.Core.Features.TweetDeck.TweetDeckBrowser;

namespace TweetDuck.Browser {
	sealed class TweetDeckBrowser : IDisposable {
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
			this.browser = new ChromiumWebBrowser(TwitterUrls.TweetDeck) {
				KeyboardHandler = new CustomKeyboardHandler(owner)
			};

			// ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
			this.browser.BrowserSettings.BackgroundColor = (uint) TweetDeckBrowserImpl.BackgroundColor.ToArgb();

			var extraContext = new TweetDeckExtraContext();

			this.browserComponent = new CefBrowserComponent(browser, handler => new ContextMenuBrowser(owner, handler, extraContext));
			this.browserImpl = new TweetDeckBrowserImpl(browserComponent, tweetDeckInterface, extraContext, new SoundNotification(browserComponent.ResourceHandlerRegistry), pluginManager, updateChecker);

			if (Arguments.HasFlag(Arguments.ArgIgnoreGDPR)) {
				browserComponent.PageLoadEnd += (_, args) => {
					if (TwitterUrls.IsTweetDeck(args.Url)) {
						browserComponent.RunScript("gen:gdpr", "TD.storage.Account.prototype.requiresConsent = function() { return false; }");
					}
				};
			}

			owner.Controls.Add(browser);
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
