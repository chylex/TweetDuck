using System;
using System.IO;
using System.Linq;
using System.Text;
using TweetLib.Utils.Static;

namespace TweetLib.Utils.IO {
	public sealed class CombinedFileStream : IDisposable {
		private const char KeySeparator = '|';
		private const int MaxBufferSize = 4096;

		private static string ValidateIdentifier(string identifier) {
			if (identifier.IndexOf(KeySeparator) != -1) {
				throw new ArgumentException("Identifier must not contain the '|' character!");
			}

			return identifier;
		}

		private static string JoinIdentifier(string[] identifier) {
			return string.Join(KeySeparator.ToString(), identifier.Select(ValidateIdentifier));
		}

		private static void ReadExactly(Stream stream, byte[] buffer) {
			var length = buffer.Length;
			var read = stream.Read(buffer, 0, length);
			
			if (read != length) {
				throw new IOException("Read fewer bytes than requested: " + read + " < " + length);
			}
		}

		private readonly Stream stream;

		public CombinedFileStream(Stream stream) {
			this.stream = stream;
		}

		public void WriteFile(string[] identifier, string path) {
			using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			WriteStreamImpl(JoinIdentifier(identifier), fileStream);
		}

		public void WriteFile(string identifier, string path) {
			using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			WriteStreamImpl(ValidateIdentifier(identifier), fileStream);
		}

		public void WriteString(string[] identifier, string contents) {
			using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			WriteStreamImpl(JoinIdentifier(identifier), memoryStream);
		}

		public void WriteString(string identifier, string contents) {
			using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			WriteStreamImpl(ValidateIdentifier(identifier), memoryStream);
		}

		private void WriteStreamImpl(string identifier, Stream sourceStream) {
			byte[] name = Encoding.UTF8.GetBytes(identifier);

			if (name.Length > 255) {
				throw new ArgumentOutOfRangeException("Identifier cannot be 256 or more characters long: " + identifier);
			}

			int index = 0;
			int left = (int) sourceStream.Length;

			var contents = new byte[left];

			while (left > 0) {
				int read = sourceStream.Read(contents, index, Math.Min(left, MaxBufferSize));
				index += read;
				left -= read;
			}

			stream.WriteByte((byte) name.Length);
			stream.Write(name, 0, name.Length);
			stream.Write(BitConverter.GetBytes(contents.Length), 0, 4);
			stream.Write(contents, 0, contents.Length);
		}

		public Entry? ReadFile() {
			int nameLength = stream.ReadByte();

			if (nameLength == -1) {
				return null;
			}

			byte[] name = new byte[nameLength];
			ReadExactly(stream, name);

			byte[] contentLength = new byte[4];
			ReadExactly(stream, contentLength);

			byte[] contents = new byte[BitConverter.ToInt32(contentLength, 0)];
			ReadExactly(stream, contents);

			return new Entry(Encoding.UTF8.GetString(name), contents);
		}

		public string? SkipFile() {
			int nameLength = stream.ReadByte();

			if (nameLength == -1) {
				return null;
			}

			byte[] name = new byte[nameLength];
			ReadExactly(stream, name);

			byte[] contentLength = new byte[4];
			ReadExactly(stream, contentLength);

			stream.Position += BitConverter.ToInt32(contentLength, 0);

			string keyName = Encoding.UTF8.GetString(name);
			return StringUtils.ExtractBefore(keyName, KeySeparator);
		}

		public void Flush() {
			stream.Flush();
		}

		void IDisposable.Dispose() {
			stream.Dispose();
		}

		public sealed class Entry {
			private string Identifier { get; }

			public string KeyName => StringUtils.ExtractBefore(Identifier, KeySeparator);

			public string[] KeyValue {
				get {
					int index = Identifier.IndexOf(KeySeparator);
					return index == -1 ? StringUtils.EmptyArray : Identifier.Substring(index + 1).Split(KeySeparator);
				}
			}

			public byte[] Contents => (byte[]) contents.Clone();

			private readonly byte[] contents;

			internal Entry(string identifier, byte[] contents) {
				this.Identifier = identifier;
				this.contents = contents;
			}

			public void WriteToFile(string path) {
				File.WriteAllBytes(path, contents);
			}

			public void WriteToFile(string path, bool createDirectory) {
				if (createDirectory) {
					FileUtils.CreateDirectoryForFile(path);
				}

				File.WriteAllBytes(path, contents);
			}
		}
	}
}
