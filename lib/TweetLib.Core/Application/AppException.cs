using System;

namespace TweetLib.Core.Application {
	public sealed class AppException : Exception {
		public string Title { get; }

		internal AppException(string title) {
			this.Title = title;
		}

		internal AppException(string title, string message) : base(message) {
			this.Title = title;
		}
	}
}
