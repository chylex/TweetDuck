using System;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IBrowserWrapper<TFrame> where TFrame : IDisposable {
		string Url { get; }
		TFrame MainFrame { get; }

		void AddWordToDictionary(string word);
	}
}
