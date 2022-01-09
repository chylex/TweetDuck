using System;
using System.IO;
using System.Runtime.InteropServices;
using TweetImpl.CefGlue.Utils;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers.Resource {
	sealed class DisableCspResourceHandler : CefResourceHandler {
		private readonly CefFrame frame;

		private Client? client;
		private int position;

		public DisableCspResourceHandler(CefFrame frame) {
			this.frame = frame;
		}

		protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback) {
			if (client != null) {
				throw new InvalidOperationException("DisableCspResourceHandler is not reusable!");
			}

			client = new Client(callback);
			CefRuntime.PostTask(CefThreadId.IO, new CefActionTask(() => frame.CreateUrlRequest(request, client)));
			handleRequest = false;
			return true;
		}

		protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string? redirectUrl) {
			redirectUrl = null;

			if (client is not { Response: {} result }) {
				responseLength = 0;
				return;
			}

			response.Url = result.Url;
			response.Charset = result.Charset;
			response.MimeType = result.MimeType;
			response.Status = result.Status;
			response.StatusText = result.StatusText;
			response.Error = result.Error;

			var headers = result.GetHeaderMap();
			headers.Remove("Content-Security-Policy");
			response.SetHeaderMap(headers);

			responseLength = client.Stream.Length;
		}

		protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback) {
			throw new NotSupportedException();
		}

		protected override bool Read(IntPtr dataOut, int bytesToRead, out int bytesRead, CefResourceReadCallback callback) {
			callback.Dispose();

			if (client == null) {
				bytesRead = 0;
				return false;
			}

			if (bytesToRead > 0) {
				int maxLength = (int) (client.Stream.Length - position);
				if (bytesToRead > maxLength) {
					bytesToRead = maxLength;
				}

				Marshal.Copy(client.Stream.GetBuffer(), position, dataOut, bytesToRead);
				position += bytesToRead;
			}

			bytesRead = bytesToRead;
			return bytesRead > 0;
		}

		protected override void Cancel() {}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			frame.Dispose();
			client?.Dispose();
		}

		private sealed class Client : CefUrlRequestClient, IDisposable {
			public MemoryStream Stream { get; }
			public CefResponse? Response { get; private set; }

			private readonly CefCallback openCallback;

			public Client(CefCallback openCallback) {
				this.openCallback = openCallback;
				this.Stream = new MemoryStream();
			}

			protected override void OnDownloadData(CefUrlRequest request, Stream data) {
				data.CopyTo(Stream);
			}

			protected override void OnRequestComplete(CefUrlRequest request) {
				if (request.RequestStatus == CefUrlRequestStatus.Success) {
					Response = request.GetResponse();
					openCallback.Continue();
				}
				else {
					openCallback.Cancel();
				}
			}

			protected override void OnDownloadProgress(CefUrlRequest request, long current, long total) {}
			protected override void OnUploadProgress(CefUrlRequest request, long current, long total) {}

			public void Dispose() {
				Stream.Dispose();
				Response?.Dispose();
				openCallback.Dispose();
			}
		}
	}
}
