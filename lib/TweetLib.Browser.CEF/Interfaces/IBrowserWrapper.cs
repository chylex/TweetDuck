using System;
using TweetLib.Browser.CEF.Data;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IBrowserWrapper<TFrame, TRequest> where TFrame : IDisposable {
		string Url { get; }
		TFrame MainFrame { get; }

		void AddWordToDictionary(string word);
		TRequest CreateGetRequest();
		void RequestDownload(TFrame frame, TRequest request, DownloadCallbacks callbacks);
	}
}
