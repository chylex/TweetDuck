using System;
using TweetLib.Browser.Base;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Events;
using TweetLib.Browser.Interfaces;
using TweetLib.Utils.Static;

namespace TweetLib.Browser.CEF.Component {
	public abstract class BrowserComponent<TFrame> : IBrowserComponent where TFrame : IDisposable {
		public bool Ready { get; private set; }

		public string Url => browser.Url;
		public abstract string CacheFolder { get; }

		public event EventHandler<BrowserLoadedEventArgs>? BrowserLoaded;
		public event EventHandler<PageLoadEventArgs>? PageLoadStart;
		public event EventHandler<PageLoadEventArgs>? PageLoadEnd;

		private readonly IBrowserWrapper<TFrame> browser;
		private readonly IFrameAdapter<TFrame> frameAdapter;

		protected BrowserComponent(IBrowserWrapper<TFrame> browser, IFrameAdapter<TFrame> frameAdapter) {
			this.browser = browser;
			this.frameAdapter = frameAdapter;
		}

		public abstract void Setup(BrowserSetup setup);
		public abstract void AttachBridgeObject(string name, object bridge);
		public abstract void DownloadFile(string url, string path, Action? onSuccess, Action<Exception>? onError);

		private sealed class BrowserLoadedEventArgsImpl : BrowserLoadedEventArgs {
			private readonly IBrowserWrapper<TFrame> browser;

			public BrowserLoadedEventArgsImpl(IBrowserWrapper<TFrame> browser) {
				this.browser = browser;
			}

			public override void AddDictionaryWords(params string[] words) {
				foreach (string word in words) {
					browser.AddWordToDictionary(word);
				}
			}
		}

		protected void OnLoadingStateChanged(bool isLoading) {
			if (!isLoading && !Ready) {
				Ready = true;
				BrowserLoaded?.Invoke(this, new BrowserLoadedEventArgsImpl(browser));
				BrowserLoaded = null;
			}
		}

		protected void OnLoadError<T>(string failedUrl, T errorCode, IErrorCodeAdapter<T> errorCodeAdapter) {
			if (errorCodeAdapter.IsAborted(errorCode)) {
				return;
			}

			if (!failedUrl.StartsWithOrdinal("td://resources/error/")) {
				using TFrame frame = browser.MainFrame;

				if (frameAdapter.IsValid(frame)) {
					string? errorName = errorCodeAdapter.GetName(errorCode);
					string errorTitle = StringUtils.ConvertPascalCaseToScreamingSnakeCase(errorName ?? string.Empty);
					frameAdapter.LoadUrl(frame, "td://resources/error/error.html#" + Uri.EscapeDataString(errorTitle));
				}
			}
		}

		protected void OnFrameLoadStart(string url, TFrame frame) {
			if (frameAdapter.IsMain(frame)) {
				PageLoadStart?.Invoke(this, new PageLoadEventArgs(url));
			}
		}

		protected void OnFrameLoadEnd(string url, TFrame frame) {
			if (frameAdapter.IsMain(frame)) {
				PageLoadEnd?.Invoke(this, new PageLoadEventArgs(url));
			}
		}

		public void RunScript(string identifier, string script) {
			using TFrame frame = browser.MainFrame;
			frameAdapter.ExecuteJavaScriptAsync(frame, script, identifier, 1);
		}
	}
}
