using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser.Adapters;
using TweetDuck.Browser.Bridge;
using TweetDuck.Browser.Data;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Handling.General;
using TweetDuck.Browser.Notification;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Plugins;
using TweetDuck.Utils;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Utils;

namespace TweetDuck.Browser {
	sealed class TweetDeckBrowser : IDisposable {
		private static UserConfig Config => Program.Config.User;

		private const string ErrorUrl = "http://td/error";
		private const string NamespaceFeatures = "features";

		public bool Ready { get; private set; }

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

		private readonly ChromiumWebBrowser browser;
		private readonly ResourceHandlers resourceHandlers;

		private string prevSoundNotificationPath = null;

		public TweetDeckBrowser(FormBrowser owner, PluginManager plugins, TweetDeckBridge tdBridge, UpdateBridge updateBridge) {
			var resourceRequestHandler = new ResourceRequestHandlerBrowser();
			resourceHandlers = resourceRequestHandler.ResourceHandlers;

			resourceHandlers.Register(FormNotificationBase.AppLogo);
			resourceHandlers.Register(TwitterUtils.LoadingSpinner);

			RequestHandlerBrowser requestHandler = new RequestHandlerBrowser();

			this.browser = new ChromiumWebBrowser(TwitterUrls.TweetDeck) {
				DialogHandler = new FileDialogHandler(),
				DragHandler = new DragHandlerBrowser(requestHandler),
				MenuHandler = new ContextMenuBrowser(owner),
				JsDialogHandler = new JavaScriptDialogHandler(),
				KeyboardHandler = new KeyboardHandlerBrowser(owner),
				LifeSpanHandler = new CustomLifeSpanHandler(),
				RequestHandler = requestHandler,
				ResourceRequestHandlerFactory = resourceRequestHandler.SelfFactory
			};

			this.browser.LoadingStateChanged += browser_LoadingStateChanged;
			this.browser.FrameLoadStart += browser_FrameLoadStart;
			this.browser.FrameLoadEnd += browser_FrameLoadEnd;
			this.browser.LoadError += browser_LoadError;

			this.browser.RegisterJsBridge("$TD", tdBridge);
			this.browser.RegisterJsBridge("$TDU", updateBridge);

			// ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
			this.browser.BrowserSettings.BackgroundColor = (uint) TwitterUtils.BackgroundColor.ToArgb();
			this.browser.Dock = DockStyle.None;
			this.browser.Location = ControlExtensions.InvisibleLocation;
			this.browser.SetupZoomEvents();

			owner.Controls.Add(browser);
			plugins.Register(PluginEnvironment.Browser, new PluginDispatcher(browser, TwitterUrls.IsTweetDeck));

			Config.MuteToggled += Config_MuteToggled;
			Config.SoundNotificationChanged += Config_SoundNotificationInfoChanged;
		}

		// setup and management

		public void PrepareSize(Size size) {
			if (!Ready) {
				browser.Size = size;
			}
		}

		private void OnBrowserReady() {
			if (!Ready) {
				browser.Location = Point.Empty;
				browser.Dock = DockStyle.Fill;
				Ready = true;
			}
		}

		public void Focus() {
			browser.Focus();
		}

		public void Dispose() {
			Config.MuteToggled -= Config_MuteToggled;
			Config.SoundNotificationChanged -= Config_SoundNotificationInfoChanged;

			browser.Dispose();
		}

		// event handlers

		private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			if (!e.IsLoading) {
				foreach (string word in TwitterUtils.DictionaryWords) {
					browser.AddWordToDictionary(word);
				}

				browser.BeginInvoke(new Action(OnBrowserReady));
				browser.LoadingStateChanged -= browser_LoadingStateChanged;
			}
		}

		private void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			IFrame frame = e.Frame;

			if (frame.IsMain) {
				string url = frame.Url;

				if (TwitterUrls.IsTwitter(url)) {
					CefScriptExecutor.RunFile(frame, "twitter.js");
				}

				if (!TwitterUrls.IsTwitterLogin2Factor(url)) {
					frame.ExecuteJavaScriptAsync(TwitterUtils.BackgroundColorOverride);
				}
			}
		}

		private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			IFrame frame = e.Frame;
			string url = frame.Url;

			if (frame.IsMain) {
				if (TwitterUrls.IsTweetDeck(url)) {
					UpdateProperties();
					CefScriptExecutor.RunBootstrap(frame, NamespaceFeatures, "tweetdeck.css");

					TweetDeckBridge.ResetStaticProperties();

					if (Arguments.HasFlag(Arguments.ArgIgnoreGDPR)) {
						CefScriptExecutor.RunScript(frame, "TD.storage.Account.prototype.requiresConsent = function(){ return false; }", "gen:gdpr");
					}

					if (Config.FirstRun) {
						CefScriptExecutor.RunFile(frame, "introduction.js");
					}
				}

				CefScriptExecutor.RunFile(frame, "update.js");
			}

			if (url == ErrorUrl) {
				resourceHandlers.Unregister(ErrorUrl);
			}
		}

		private void browser_LoadError(object sender, LoadErrorEventArgs e) {
			if (e.ErrorCode == CefErrorCode.Aborted) {
				return;
			}

			if (!e.FailedUrl.StartsWith("http://td/", StringComparison.Ordinal)) {
				string errorPage = Program.Resources.LoadSilent("pages/error.html");

				if (errorPage != null) {
					string errorName = Enum.GetName(typeof(CefErrorCode), e.ErrorCode);
					string errorTitle = StringUtils.ConvertPascalCaseToScreamingSnakeCase(errorName ?? string.Empty);

					resourceHandlers.Register(ErrorUrl, ResourceHandlers.ForString(errorPage.Replace("{err}", errorTitle)));
					browser.Load(ErrorUrl);
				}
			}
		}

		private void Config_MuteToggled(object sender, EventArgs e) {
			UpdateProperties();
		}

		private void Config_SoundNotificationInfoChanged(object sender, EventArgs e) {
			const string soundUrl = "https://ton.twimg.com/tduck/updatesnd";

			bool hasCustomSound = Config.IsCustomSoundNotificationSet;
			string newNotificationPath = Config.NotificationSoundPath;

			if (prevSoundNotificationPath != newNotificationPath) {
				prevSoundNotificationPath = newNotificationPath;

				if (hasCustomSound) {
					resourceHandlers.Register(soundUrl, SoundNotification.CreateFileHandler(newNotificationPath));
				}
				else {
					resourceHandlers.Unregister(soundUrl);
				}
			}

			browser.ExecuteJsAsync("TDGF_setSoundNotificationData", hasCustomSound, Config.NotificationSoundVolume);
		}

		// external handling

		public void HideVideoOverlay(bool focus) {
			if (focus) {
				browser.GetBrowser().GetHost().SendFocusEvent(true);
			}

			browser.ExecuteJsAsync("$('#td-video-player-overlay').remove()");
		}

		// javascript calls

		public void ReloadToTweetDeck() {
			browser.ExecuteJsAsync($"if(window.TDGF_reload)window.TDGF_reload();else window.location.href='{TwitterUrls.TweetDeck}'");
		}

		public void OnModulesLoaded(string moduleNamespace) {
			if (moduleNamespace == NamespaceFeatures) {
				ReinjectCustomCSS(Config.CustomBrowserCSS);
				Config_SoundNotificationInfoChanged(null, EventArgs.Empty);
			}
		}

		public void UpdateProperties() {
			browser.ExecuteJsAsync(PropertyBridge.GenerateScript(PropertyBridge.Environment.Browser));
		}

		public void ReinjectCustomCSS(string css) {
			browser.ExecuteJsAsync("TDGF_reinjectCustomCSS", css?.Replace(Environment.NewLine, " ") ?? string.Empty);
		}

		public void OnMouseClickExtra(IntPtr param) {
			browser.ExecuteJsAsync("TDGF_onMouseClickExtra", (param.ToInt32() >> 16) & 0xFFFF);
		}

		public void ShowTweetDetail(string columnId, string chirpId, string fallbackUrl) {
			browser.ExecuteJsAsync("TDGF_showTweetDetail", columnId, chirpId, fallbackUrl);
		}

		public void AddSearchColumn(string query) {
			browser.ExecuteJsAsync("TDGF_performSearch", query);
		}

		public void TriggerTweetScreenshot() {
			browser.ExecuteJsAsync("TDGF_triggerScreenshot()");
		}

		public void ReloadColumns() {
			browser.ExecuteJsAsync("TDGF_reloadColumns()");
		}

		public void PlaySoundNotification() {
			browser.ExecuteJsAsync("TDGF_playSoundNotification()");
		}

		public void ApplyROT13() {
			browser.ExecuteJsAsync("TDGF_applyROT13()");
		}

		public void ShowUpdateNotification(string versionTag, string releaseNotes) {
			browser.ExecuteJsAsync("TDUF_displayNotification", versionTag, Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(releaseNotes)));
		}

		public void OpenDevTools() {
			browser.OpenDevToolsCustom();
		}
	}
}
