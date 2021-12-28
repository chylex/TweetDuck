using System;

namespace TweetLib.Browser.Events {
	public abstract class BrowserLoadedEventArgs : EventArgs {
		public abstract void AddDictionaryWords(params string[] word);
	}
}
