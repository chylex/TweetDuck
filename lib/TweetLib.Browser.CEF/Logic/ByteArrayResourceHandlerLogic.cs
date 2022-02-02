using System;
using System.Diagnostics.CodeAnalysis;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public abstract class ByteArrayResourceHandlerLogic {
		public delegate void WriteToOut<T>(T dataOut, byte[] dataIn, int position, int length);
	}

	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public sealed class ByteArrayResourceHandlerLogic<TResponse> : ByteArrayResourceHandlerLogic {
		private readonly ByteArrayResource resource;
		private readonly IResponseAdapter<TResponse> responseAdapter;

		private int position;

		public ByteArrayResourceHandlerLogic(ByteArrayResource resource, IResponseAdapter<TResponse> responseAdapter) {
			this.resource = resource;
			this.responseAdapter = responseAdapter;
		}

		public bool Open(out bool handleRequest) {
			position = 0;
			handleRequest = true;
			return true;
		}

		public void GetResponseHeaders(TResponse response, out long responseLength, out string? redirectUrl) {
			responseLength = resource.Length;
			redirectUrl = null;

			responseAdapter.SetMimeType(response, resource.MimeType);
			responseAdapter.SetStatus(response, (int) resource.StatusCode, resource.StatusText);
			responseAdapter.SetCharset(response, "utf-8");
			responseAdapter.SetHeader(response, "Access-Control-Allow-Origin", "*");
		}

		public bool Skip(long bytesToSkip, out long bytesSkipped, IDisposable callback) {
			callback.Dispose();

			position = (int) (position + bytesToSkip);
			bytesSkipped = bytesToSkip;
			return true;
		}

		public bool Read<T>(WriteToOut<T> write, T dataOut, int bytesToRead, out int bytesRead, IDisposable callback) {
			callback.Dispose();

			if (bytesToRead > 0) {
				write(dataOut, resource.Contents, position, bytesToRead);
				position += bytesToRead;
			}

			bytesRead = bytesToRead;
			return bytesRead > 0;
		}

		public bool Read<T>(WriteToOut<T> write, T dataOut, out int bytesRead, IDisposable callback) {
			return Read(write, dataOut, resource.Length - position, out bytesRead, callback);
		}
	}
}
