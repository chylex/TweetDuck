using System;
using System.Runtime.InteropServices;
using TweetImpl.CefGlue.Adapters;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers.Resource {
	sealed class ByteArrayResourceHandler : CefResourceHandler {
		private static readonly ByteArrayResourceHandlerLogic.WriteToOut<IntPtr> WriteToOut = static delegate (IntPtr dataOut, byte[] dataIn, int position, int length) {
			Marshal.Copy(dataIn, position, dataOut, length);
		};

		private ByteArrayResourceHandlerLogic<CefResponse>? logic;

		public ByteArrayResourceHandler() {
			SetResource(new ByteArrayResource(Array.Empty<byte>()));
		}

		public ByteArrayResourceHandler(ByteArrayResource resource) {
			SetResource(resource);
		}

		private void SetResource(ByteArrayResource resource) {
			this.logic = new ByteArrayResourceHandlerLogic<CefResponse>(resource, CefResponseAdapter.Instance);
		}

		protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback) {
			return logic!.Open(out handleRequest);
		}

		protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string? redirectUrl) {
			logic!.GetResponseHeaders(response, out responseLength, out redirectUrl);
		}

		protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback) {
			return logic!.Skip(bytesToSkip, out bytesSkipped, callback);
		}

		protected override bool Read(IntPtr dataOut, int bytesToRead, out int bytesRead, CefResourceReadCallback callback) {
			return logic!.Read(WriteToOut, dataOut, Math.Min(bytesToRead, logic.RemainingBytes), out bytesRead, callback);
		}

		protected override void Cancel() {}
	}
}
