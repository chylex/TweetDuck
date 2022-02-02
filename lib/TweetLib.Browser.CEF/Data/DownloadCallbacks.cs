using System;
using System.IO;

namespace TweetLib.Browser.CEF.Data {
	public sealed class DownloadCallbacks {
		internal bool HasData { get; private set; }

		private readonly FileStream fileStream;
		private readonly Action? onSuccess;
		private readonly Action<Exception>? onError;

		internal DownloadCallbacks(FileStream fileStream, Action? onSuccess, Action<Exception>? onError) {
			this.fileStream = fileStream;
			this.onSuccess = onSuccess;
			this.onError = onError;
		}

		internal void OnData(Stream data) {
			data.CopyTo(fileStream);
			HasData |= fileStream.Position > 0;
		}

		internal void OnSuccess() {
			fileStream.Dispose();
			onSuccess?.Invoke();
		}

		internal void OnError(Exception e) {
			fileStream.Dispose();
			onError?.Invoke(e);
		}
	}
}
