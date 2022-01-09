using System;
using System.Threading.Tasks;
using Gtk;
using Lunixo.ChromiumGtk;
using TweetDuck.Application;
using TweetDuck.Browser.Base;
using TweetImpl.CefGlue.Component;
using TweetLib.Core;
using TweetLib.Core.Features;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using TweetDeckBrowserImpl = TweetLib.Core.Features.TweetDeck.TweetDeckBrowser;

namespace TweetDuck.Browser {
	sealed class TweetDeckBrowser : ISoundNotificationHandler, IDisposable {
		public WebView View { get; }

		public TweetDeckBrowser(Window window, PluginManager pluginManager) {
			this.View = CustomWebClient.CreateWebView(PopupHandler.Instance);

			var extraContext = new TweetDeckExtraContext();
			var browserComponent = new CefBrowserComponent(window, View, handler => new ContextMenuBrowser(handler, extraContext));
			var _ = new TweetDeckBrowserImpl(browserComponent, new TweetDeckInterfaceImpl(this), extraContext, this, pluginManager);

			View.LoadUrl(TwitterUrls.TweetDeck);
		}

		public void Dispose() {
			View.Dispose();
		}

		private sealed class TweetDeckInterfaceImpl : ITweetDeckInterface {
			private readonly TweetDeckBrowser owner;

			public TweetDeckInterfaceImpl(TweetDeckBrowser owner) {
				this.owner = owner;
			}

			void ICommonInterface.Alert(string type, string contents) {
				MessageType messageType = type switch {
					"error"   => MessageType.Error,
					"warning" => MessageType.Warning,
					"info"    => MessageType.Error,
					_         => MessageType.Other
				};

				MessageDialogs.Show(messageType, "TweetDuck Browser Message", contents);
			}

			void ICommonInterface.DisplayTooltip(string? text) {
				throw new NotSupportedException();
			}

			void ICommonInterface.FixClipboard() {}

			int ICommonInterface.GetIdleSeconds() {
				return 0;
			}

			void ICommonInterface.OnSoundNotification() {
				throw new NotSupportedException();
			}

			void ICommonInterface.PlayVideo(string videoUrl, string tweetUrl, string username, object callShowOverlay) {
				App.SystemHandler.OpenBrowser(videoUrl);
			}

			void ICommonInterface.ScreenshotTweet(string html, int width) {
				throw new NotSupportedException();
			}

			void ICommonInterface.ShowDesktopNotification(DesktopNotification notification) {
				throw new NotSupportedException();
			}

			void ICommonInterface.StopVideo() {
				throw new NotSupportedException();
			}

			void ITweetDeckInterface.OnIntroductionClosed(bool showGuide) {
				throw new NotSupportedException();
			}

			void ITweetDeckInterface.OpenContextMenu() {
				throw new NotSupportedException();
			}

			void ITweetDeckInterface.OpenProfileImport() {
				throw new NotSupportedException();
			}

			Task ICommonInterface.ExecuteCallback(object callback, params object[] parameters) {
				throw new NotSupportedException();
			}
		}

		void ISoundNotificationHandler.Unregister(string url) {}
		void ISoundNotificationHandler.Register(string url, string path) {}
	}
}
