using System.IO;
using CefSharp;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using static TweetLib.Browser.CEF.Logic.DownloadRequestClientLogic.RequestStatus;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefDownloadRequestClient : UrlRequestClient {
		private readonly DownloadRequestClientLogic logic;

		public CefDownloadRequestClient(DownloadCallbacks callbacks) {
			this.logic = new DownloadRequestClientLogic(callbacks);
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
