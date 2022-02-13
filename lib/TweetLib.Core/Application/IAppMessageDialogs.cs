namespace TweetLib.Core.Application {
	public interface IAppMessageDialogs {
		void Information(string caption, string text);
		void Error(string caption, string text);
	}
}
