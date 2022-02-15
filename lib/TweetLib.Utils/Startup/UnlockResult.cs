using System;

namespace TweetLib.Utils.Startup {
	public class UnlockResult {
		private readonly string name;

		private UnlockResult(string name) {
			this.name = name;
		}

		public override string ToString() {
			return name;
		}

		public static UnlockResult Success { get; } = new ("Success");

		public sealed class Fail : UnlockResult {
			public Exception Exception { get; }

			internal Fail(Exception exception) : base("Fail") {
				this.Exception = exception;
			}
		}
	}
}
