﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace TweetLib.Communication.Pipe {
	public abstract class DuplexPipe : IDisposable {
		private const string Separator = "\x1F";

		public static Server CreateServer() {
			return new Server();
		}

		public static Client CreateClient(string token) {
			int space = token.IndexOf(' ');
			return new Client(token[..space], token[(space + 1)..]);
		}

		private readonly PipeStream pipeIn;
		private readonly PipeStream pipeOut;
		private readonly StreamWriter writerStream;

		public event EventHandler<PipeReadEventArgs>? DataIn;

		private DuplexPipe(PipeStream pipeIn, PipeStream pipeOut) {
			this.pipeIn = pipeIn;
			this.pipeOut = pipeOut;
			this.writerStream = new StreamWriter(this.pipeOut);

			new Thread(ReaderThread) { IsBackground = true }.Start();
		}

		private void ReaderThread() {
			using StreamReader read = new StreamReader(pipeIn);

			try {
				while (read.ReadLine() is {} data) {
					DataIn?.Invoke(this, new PipeReadEventArgs(data));
				}
			} catch (ObjectDisposedException) {
				// expected
			}
		}

		public void Write(string key) {
			writerStream.WriteLine(key);
			writerStream.Flush();
		}

		public void Write(string key, string data) {
			writerStream.WriteLine(string.Concat(key, Separator, data));
			writerStream.Flush();
		}

		public void Dispose() {
			pipeIn.Dispose();
			writerStream.Dispose();
		}

		public sealed class Server : DuplexPipe {
			private AnonymousPipeServerStream ServerPipeIn => (AnonymousPipeServerStream) pipeIn;
			private AnonymousPipeServerStream ServerPipeOut => (AnonymousPipeServerStream) pipeOut;

			internal Server() : base(new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable), new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable)) {}

			public string GenerateToken() {
				return ServerPipeIn.GetClientHandleAsString() + " " + ServerPipeOut.GetClientHandleAsString();
			}

			public void DisposeToken() {
				ServerPipeIn.DisposeLocalCopyOfClientHandle();
				ServerPipeOut.DisposeLocalCopyOfClientHandle();
			}
		}

		public sealed class Client : DuplexPipe {
			internal Client(string handleOut, string handleIn) : base(new AnonymousPipeClientStream(PipeDirection.In, handleIn), new AnonymousPipeClientStream(PipeDirection.Out, handleOut)) {}
		}

		public sealed class PipeReadEventArgs : EventArgs {
			public string Key { get; }
			public string Data { get; }

			internal PipeReadEventArgs(string line) {
				int separatorIndex = line.IndexOf(Separator, StringComparison.Ordinal);

				if (separatorIndex == -1) {
					Key = line;
					Data = string.Empty;
				}
				else {
					Key = line[..separatorIndex];
					Data = line[(separatorIndex + 1)..];
				}
			}
		}
	}
}
