using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using TweetLib.Browser.CEF.Data;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class DownloadRequestClientLogic {
		public enum RequestStatus {
			Unknown,
			Success,
			Failed
		}

		private readonly DownloadCallbacks callbacks;
		private bool hasFailed;

		public DownloadRequestClientLogic(DownloadCallbacks callbacks) {
			this.callbacks = callbacks;
		}

		public bool GetAuthCredentials(IDisposable callback) {
			callback.Dispose();

			hasFailed = true;
			callbacks.OnError(new Exception("This URL requires authentication."));

			return false;
		}

		public void OnDownloadData(Stream data) {
			if (hasFailed) {
				return;
			}

			try {
				callbacks.OnData(data);
			} catch (Exception e) {
				callbacks.OnError(e);
				hasFailed = true;
			}
		}

		[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
		public void OnRequestComplete(RequestStatus status) {
			if (hasFailed) {
				return;
			}

			switch (status) {
				case RequestStatus.Success when callbacks.HasData:
					callbacks.OnSuccess();
					break;

				case RequestStatus.Success:
					callbacks.OnError(new Exception("File is empty."));
					break;

				default:
					callbacks.OnError(new Exception("Unknown error."));
					break;
			}
		}
	}
}
