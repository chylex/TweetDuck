using System;
using System.IO;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class ResponseFilterLogic {
		public enum FilterStatus {
			NeedMoreData,
			Done
		}

		private enum State {
			Reading,
			Writing,
			Done
		}

		private readonly IResponseProcessor processor;
		private byte[] responseData;

		private State state;
		private int offset;

		internal ResponseFilterLogic(IResponseProcessor processor, int totalBytes) {
			this.processor = processor;
			this.responseData = new byte[totalBytes];
			this.state = State.Reading;
		}

		public FilterStatus Filter(Stream? dataIn, out long dataInRead, Stream dataOut, long dataOutLength, out long dataOutWritten) {
			int responseLength = responseData.Length;

			if (state == State.Reading) {
				int bytesToRead = Math.Min(responseLength - offset, (int) Math.Min(dataIn?.Length ?? 0, int.MaxValue));
				int bytesRead = dataIn?.Read(responseData, offset, bytesToRead) ?? 0;

				offset += bytesRead;
				dataInRead = bytesRead;
				dataOutWritten = 0;

				if (offset >= responseLength) {
					responseData = processor.Process(responseData);
					state = State.Writing;
					offset = 0;
				}

				return FilterStatus.NeedMoreData;
			}
			else if (state == State.Writing) {
				int bytesToWrite = Math.Min(responseLength - offset, (int) Math.Min(dataOutLength, int.MaxValue));

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
				throw new InvalidOperationException("This resource filter cannot be reused!");
			}
		}
	}
}
