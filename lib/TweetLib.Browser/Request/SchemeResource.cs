using System.Net;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.Request {
	public abstract class SchemeResource {
		private SchemeResource() {}

		public abstract T Visit<T>(ISchemeResourceVisitor<T> visitor);

		public sealed class Status : SchemeResource {
			public HttpStatusCode Code { get; }
			public string Message { get; }

			public Status(HttpStatusCode code, string message) {
				Code = code;
				Message = message;
			}

			public override T Visit<T>(ISchemeResourceVisitor<T> visitor) {
				return visitor.Status(this);
			}
		}

		public sealed class File : SchemeResource {
			public byte[] Contents { get; }
			public string Extension { get; }

			public File(byte[] contents, string extension) {
				Contents = contents;
				Extension = extension;
			}

			public override T Visit<T>(ISchemeResourceVisitor<T> visitor) {
				return visitor.File(this);
			}
		}
	}
}
