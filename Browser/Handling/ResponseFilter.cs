using System;
using System.IO;
using CefSharp;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Browser.Handling {
	sealed class ResponseFilter : IResponseFilter {
		private enum State {
			Reading,
			Writing,
			Done
		}

		private readonly IResponseProcessor processor;
		private byte[] responseData;

		private State state;
		private int offset;

		public ResponseFilter(IResponseProcessor processor, int totalBytes) {
			this.processor = processor;
			this.responseData = new byte[totalBytes];
			this.state = State.Reading;
		}

		public bool InitFilter() {
			return true;
		}

		FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten) {
			int responseLength = responseData.Length;

			if (state == State.Reading) {
				int bytesToRead = Math.Min(responseLength - offset, (int) Math.Min(dataIn?.Length ?? 0, int.MaxValue));

				dataIn?.Read(responseData, offset, bytesToRead);
				offset += bytesToRead;

				dataInRead = bytesToRead;
				dataOutWritten = 0;

				if (offset >= responseLength) {
					responseData = processor.Process(responseData);
					state = State.Writing;
					offset = 0;
				}

				return FilterStatus.NeedMoreData;
			}
			else if (state == State.Writing) {
				int bytesToWrite = Math.Min(responseLength - offset, (int) Math.Min(dataOut.Length, int.MaxValue));

				if (bytesToWrite > 0) {
					dataOut.Write(responseData, offset, bytesToWrite);
					offset += bytesToWrite;
				}

				dataOutWritten = bytesToWrite;
				dataInRead = 0;

				if (offset < responseLength) {
					return FilterStatus.NeedMoreData;
				}
				else {
					state = State.Done;
					return FilterStatus.Done;
				}
			}
			else {
				throw new InvalidOperationException("This resource filter cannot be reused.");
			}
		}

		public void Dispose() {}
	}
}
