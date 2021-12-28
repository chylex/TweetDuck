using System;

namespace TweetLib.Browser.Events {
	public sealed class PageLoadEventArgs : EventArgs {
		public string Url { get; }

		public PageLoadEventArgs(string url) {
			Url = url;
		}
	}
}
