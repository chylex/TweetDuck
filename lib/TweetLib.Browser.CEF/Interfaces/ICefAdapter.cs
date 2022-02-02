using System;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface ICefAdapter {
		void RunOnUiThread(Action action);
	}
}
