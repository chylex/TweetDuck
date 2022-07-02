using System;
using System.IO;
using CefSharp;
using CefSharp.Callback;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;

namespace TweetImpl.CefSharp.Handlers {
	public sealed class CefByteArrayResourceHandler : IResourceHandler {
		private static readonly ByteArrayResourceHandlerLogic.WriteToOut<Stream> WriteToOut = static delegate (Stream dataOut, byte[] dataIn, int position, int length) {
			dataOut.Write(dataIn, position, length);
		};

		private static ByteArrayResourceHandlerLogic<IResponse> CreateLogic(ByteArrayResource resource) {
			return new ByteArrayResourceHandlerLogic<IResponse>(resource, CefResponseAdapter.Instance);
		}

		private ByteArrayResourceHandlerLogic<IResponse> logic;

		public CefByteArrayResourceHandler() : this(new ByteArrayResource(Array.Empty<byte>())) {}

		internal CefByteArrayResourceHandler(ByteArrayResource resource) {
			this.logic = CreateLogic(resource);
		}

		public void SetResource(ByteArrayResource resource) {
			this.logic = CreateLogic(resource);
		}

		bool IResourceHandler.Open(IRequest request, out bool handleRequest, ICallback callback) {
			return logic.Open(out handleRequest);
		}

		void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string? redirectUrl) {
			logic.GetResponseHeaders(response, out responseLength, out redirectUrl);
		}

		bool IResourceHandler.Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback) {
			return logic.Skip(bytesToSkip, out bytesSkipped, callback);
		}

		bool IResourceHandler.Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback) {
			return logic.Read(WriteToOut, dataOut, dataOut.Length, out bytesRead, callback);
		}

		bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback) {
			throw new NotSupportedException();
		}

		bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback) {
			throw new NotSupportedException();
		}

		void IResourceHandler.Cancel() {}
		void IDisposable.Dispose() {}
	}
}
