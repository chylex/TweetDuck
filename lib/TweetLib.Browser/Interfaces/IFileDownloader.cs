using System;

namespace TweetLib.Browser.Interfaces {
	public interface IFileDownloader {
		string CacheFolder { get; }
		void DownloadFile(string url, string path, Action? onSuccess, Action<Exception>? onError);
	}
}
