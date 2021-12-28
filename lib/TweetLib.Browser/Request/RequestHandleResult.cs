using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.Request {
	public abstract class RequestHandleResult {
		private RequestHandleResult() {}

		public sealed class Redirect : RequestHandleResult {
			public string Url { get; }

			public Redirect(string url) {
				Url = url;
			}
		}

		public sealed class Process : RequestHandleResult {
			public IResponseProcessor Processor { get; }

			public Process(IResponseProcessor processor) {
				Processor = processor;
			}
		}

		public sealed class Cancel : RequestHandleResult {
			public static Cancel Instance { get; } = new ();

			private Cancel() {}
		}
	}
}
