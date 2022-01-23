using System;
using System.IO;
using CefSharp;
using TweetLib.Browser.CEF.Logic;
using static TweetLib.Browser.CEF.Logic.DownloadRequestClientLogic.RequestStatus;

namespace TweetDuck.Browser.Base {
	sealed class CefDownloadRequestClient : UrlRequestClient {
		private readonly DownloadRequestClientLogic logic;

		public CefDownloadRequestClient(FileStream fileStream, Action onSuccess, Action<Exception> onError) {
			this.logic = new DownloadRequestClientLogic(fileStream, onSuccess, onError);
		}

		protected override bool GetAuthCredentials(bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
			return logic.GetAuthCredentials(callback);
		}

		protected override void OnDownloadData(IUrlRequest request, Stream data) {
			logic.OnDownloadData(data);
		}

		protected override void OnRequestComplete(IUrlRequest request) {
			logic.OnRequestComplete(request.RequestStatus switch {
				UrlRequestStatus.Success => Success,
				UrlRequestStatus.Failed  => Failed,
				_                        => Unknown
			});
		}
	}
}
