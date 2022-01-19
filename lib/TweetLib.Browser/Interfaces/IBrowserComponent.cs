using System;
using TweetLib.Browser.Base;
using TweetLib.Browser.Events;

namespace TweetLib.Browser.Interfaces {
	public interface IBrowserComponent : IScriptExecutor {
		string Url { get; }
		string CacheFolder { get; }

		event EventHandler<BrowserLoadedEventArgs> BrowserLoaded;
		event EventHandler<PageLoadEventArgs> PageLoadStart;
		event EventHandler<PageLoadEventArgs> PageLoadEnd;

		void Setup(BrowserSetup setup);
		void AttachBridgeObject(string name, object bridge);
		void DownloadFile(string url, string path, Action? onSuccess, Action<Exception>? onError);
	}
}
