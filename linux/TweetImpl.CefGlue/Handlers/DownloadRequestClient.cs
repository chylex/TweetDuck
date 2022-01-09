using System.IO;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;
using static TweetLib.Browser.CEF.Logic.DownloadRequestClientLogic.RequestStatus;

namespace TweetImpl.CefGlue.Handlers {
	sealed class DownloadRequestClient : CefUrlRequestClient {
		private readonly DownloadRequestClientLogic logic;

		public DownloadRequestClient(DownloadCallbacks callbacks) {
			this.logic = new DownloadRequestClientLogic(callbacks);
		}

		protected override bool GetAuthCredentials(bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback) {
			return logic.GetAuthCredentials(callback);
		}

		protected override void OnDownloadData(CefUrlRequest request, Stream data) {
			logic.OnDownloadData(data);
		}

		protected override void OnRequestComplete(CefUrlRequest request) {
			logic.OnRequestComplete(request.RequestStatus switch {
				CefUrlRequestStatus.Success => Success,
				CefUrlRequestStatus.Failed  => Failed,
				_                           => Unknown
			});
		}

		protected override void OnDownloadProgress(CefUrlRequest request, long current, long total) {}
		protected override void OnUploadProgress(CefUrlRequest request, long current, long total) {}
	}
}
