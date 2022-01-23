using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class DownloadRequestClientLogic {
		public enum RequestStatus {
			Unknown,
			Success,
			Failed
		}

		private readonly FileStream fileStream;
		private readonly Action? onSuccess;
		private readonly Action<Exception>? onError;

		private bool hasFailed;

		public DownloadRequestClientLogic(FileStream fileStream, Action? onSuccess, Action<Exception>? onError) {
			this.fileStream = fileStream;
			this.onSuccess = onSuccess;
			this.onError = onError;
		}

		public bool GetAuthCredentials(IDisposable callback) {
			callback.Dispose();

			hasFailed = true;
			fileStream.Dispose();
			onError?.Invoke(new Exception("This URL requires authentication."));

			return false;
		}

		public void OnDownloadData(Stream data) {
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

		[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
		public void OnRequestComplete(RequestStatus status) {
			if (hasFailed) {
				return;
			}

			bool isEmpty = fileStream.Position == 0;
			fileStream.Dispose();

			switch (status) {
				case RequestStatus.Failed:
					onError?.Invoke(new Exception("Unknown error."));
					break;

				case RequestStatus.Success when isEmpty:
					onError?.Invoke(new Exception("File is empty."));
					return;

				case RequestStatus.Success:
					onSuccess?.Invoke();
					break;
			}
		}
	}
}
