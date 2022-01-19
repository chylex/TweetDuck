using System;
using System.IO;
using CefSharp;

namespace TweetDuck.Browser.Handling {
	sealed class DownloadRequestClient : UrlRequestClient {
		private readonly FileStream fileStream;
		private readonly Action onSuccess;
		private readonly Action<Exception> onError;

		private bool hasFailed;

		public DownloadRequestClient(FileStream fileStream, Action onSuccess, Action<Exception> onError) {
			this.fileStream = fileStream;
			this.onSuccess = onSuccess;
			this.onError = onError;
		}

		protected override bool GetAuthCredentials(bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
			onError?.Invoke(new Exception("This URL requires authentication."));
			fileStream.Dispose();
			hasFailed = true;
			return false;
		}

		protected override void OnDownloadData(IUrlRequest request, Stream data) {
			if (hasFailed) {
				return;
			}

			try {
				data.CopyTo(fileStream);
			} catch (Exception e) {
				fileStream.Dispose();
				onError?.Invoke(e);
				hasFailed = true;
			}
		}

		protected override void OnRequestComplete(IUrlRequest request) {
			if (hasFailed) {
				return;
			}

			bool isEmpty = fileStream.Position == 0;
			fileStream.Dispose();

			var status = request.RequestStatus;
			if (status == UrlRequestStatus.Failed) {
				onError?.Invoke(new Exception("Unknown error."));
			}
			else if (status == UrlRequestStatus.Success) {
				if (isEmpty) {
					onError?.Invoke(new Exception("File is empty."));
					return;
				}

				onSuccess?.Invoke();
			}
		}
	}
}
