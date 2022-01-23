using System.Net;
using System.Text;

namespace TweetLib.Browser.CEF.Data {
	public sealed class ByteArrayResource {
		private const string DefaultMimeType = "text/html";
		private const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;
		private const string DefaultStatusText = "OK";

		internal byte[] Contents { get; }
		internal int Length { get; }
		internal string MimeType { get; }
		internal HttpStatusCode StatusCode { get; }
		internal string StatusText { get; }

		public ByteArrayResource(byte[] contents, string mimeType = DefaultMimeType, HttpStatusCode statusCode = DefaultStatusCode, string statusText = DefaultStatusText) {
			this.Contents = contents;
			this.Length = contents.Length;
			this.MimeType = mimeType;
			this.StatusCode = statusCode;
			this.StatusText = statusText;
		}

		public ByteArrayResource(string contents, Encoding encoding, string mimeType = DefaultMimeType, HttpStatusCode statusCode = DefaultStatusCode, string statusText = DefaultStatusText) : this(encoding.GetBytes(contents), mimeType, statusCode, statusText) {}
	}
}
