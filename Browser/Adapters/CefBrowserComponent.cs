using System;
using System.IO;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser.Handling;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Browser.Base;
using TweetLib.Browser.Events;
using TweetLib.Browser.Interfaces;
using TweetLib.Utils.Static;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;

namespace TweetDuck.Browser.Adapters {
	abstract class CefBrowserComponent : IBrowserComponent {
		public bool Ready { get; private set; }

		public string Url => browser.Address;
		public string CacheFolder => BrowserCache.CacheFolder;

		public event EventHandler<BrowserLoadedEventArgs> BrowserLoaded;
		public event EventHandler<PageLoadEventArgs> PageLoadStart;
		public event EventHandler<PageLoadEventArgs> PageLoadEnd;

		private readonly ChromiumWebBrowser browser;

		protected CefBrowserComponent(ChromiumWebBrowser browser) {
			this.browser = browser;
			this.browser.JsDialogHandler = new JavaScriptDialogHandler();
			this.browser.LifeSpanHandler = new CustomLifeSpanHandler();
			this.browser.LoadingStateChanged += OnLoadingStateChanged;
			this.browser.LoadError += OnLoadError;
			this.browser.FrameLoadStart += OnFrameLoadStart;
			this.browser.FrameLoadEnd += OnFrameLoadEnd;
			this.browser.SetupZoomEvents();
		}

		void IBrowserComponent.Setup(BrowserSetup setup) {
			browser.MenuHandler = SetupContextMenu(setup.ContextMenuHandler);
			browser.ResourceRequestHandlerFactory = SetupResourceHandlerFactory(setup.ResourceRequestHandler);
		}

		protected abstract ContextMenuBase SetupContextMenu(IContextMenuHandler handler);

		protected abstract CefResourceHandlerFactory SetupResourceHandlerFactory(IResourceRequestHandler handler);

		private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			if (!e.IsLoading) {
				Ready = true;
				browser.LoadingStateChanged -= OnLoadingStateChanged;
				BrowserLoaded?.Invoke(this, new BrowserLoadedEventArgsImpl(browser));
				BrowserLoaded = null;
			}
		}

		private sealed class BrowserLoadedEventArgsImpl : BrowserLoadedEventArgs {
			private readonly IWebBrowser browser;

			public BrowserLoadedEventArgsImpl(IWebBrowser browser) {
				this.browser = browser;
			}

			public override void AddDictionaryWords(params string[] words) {
				foreach (string word in words) {
					browser.AddWordToDictionary(word);
				}
			}
		}

		private void OnLoadError(object sender, LoadErrorEventArgs e) {
			if (e.ErrorCode == CefErrorCode.Aborted) {
				return;
			}

			if (!e.FailedUrl.StartsWithOrdinal("td://resources/error/")) {
				string errorName = Enum.GetName(typeof(CefErrorCode), e.ErrorCode);
				string errorTitle = StringUtils.ConvertPascalCaseToScreamingSnakeCase(errorName ?? string.Empty);
				browser.Load("td://resources/error/error.html#" + Uri.EscapeDataString(errorTitle));
			}
		}

		private void OnFrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			if (e.Frame.IsMain) {
				PageLoadStart?.Invoke(this, new PageLoadEventArgs(e.Url));
			}
		}

		private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			if (e.Frame.IsMain) {
				PageLoadEnd?.Invoke(this, new PageLoadEventArgs(e.Url));
			}
		}

		public void AttachBridgeObject(string name, object bridge) {
			browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
			browser.JavascriptObjectRepository.Register(name, bridge, isAsync: true, BindingOptions.DefaultBinder);
		}

		public void RunScript(string identifier, string script) {
			using IFrame frame = browser.GetMainFrame();
			frame.ExecuteJavaScriptAsync(script, identifier, 1);
		}

		public void DownloadFile(string url, string path, Action onSuccess, Action<Exception> onError) {
			Cef.UIThreadTaskFactory.StartNew(() => {
				try {
					using IFrame frame = browser.GetMainFrame();
					var request = frame.CreateRequest(false);

					request.Method = "GET";
					request.Url = url;
					request.Flags = UrlRequestFlags.AllowStoredCredentials;
					request.SetReferrer(Url, ReferrerPolicy.Default);

					var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
					var client = new DownloadRequestClient(fileStream, onSuccess, onError);
					frame.CreateUrlRequest(request, client);
				} catch (Exception e) {
					onError?.Invoke(e);
				}
			});
		}
	}
}
