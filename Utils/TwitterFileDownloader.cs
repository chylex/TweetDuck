using System;
using System.Net;
using System.Threading.Tasks;
using CefSharp;
using TweetDuck.Management;
using TweetLib.Browser.Interfaces;
using TweetLib.Utils.Static;
using Cookie = CefSharp.Cookie;

namespace TweetDuck.Utils {
	sealed class TwitterFileDownloader : IFileDownloader {
		public static TwitterFileDownloader Instance { get; } = new TwitterFileDownloader();

		private TwitterFileDownloader() {}

		public string CacheFolder => BrowserCache.CacheFolder;

		public void DownloadFile(string url, string path, Action onSuccess, Action<Exception> onFailure) {
			const string authCookieName = "auth_token";

			using ICookieManager cookies = Cef.GetGlobalCookieManager();

			cookies.VisitUrlCookiesAsync(url, true).ContinueWith(task => {
				string cookieStr = null;

				if (task.Status == TaskStatus.RanToCompletion) {
					Cookie found = task.Result?.Find(cookie => cookie.Name == authCookieName); // the list may be null

					if (found != null) {
						cookieStr = $"{found.Name}={found.Value}";
					}
				}

				WebClient client = WebUtils.NewClient(BrowserUtils.UserAgentChrome);
				client.Headers[HttpRequestHeader.Cookie] = cookieStr;
				client.DownloadFileCompleted += WebUtils.FileDownloadCallback(path, onSuccess, onFailure);
				client.DownloadFileAsync(new Uri(url), path);
			});
		}
	}
}
