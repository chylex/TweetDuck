using System;

namespace TweetLib.Core.Application {
	public interface IAppErrorHandler {
		bool Log(string text);
		void HandleException(string caption, string message, bool canIgnore, Exception e);
	}
}
