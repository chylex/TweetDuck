namespace TweetLib.Core.Application {
	public interface IAppLogger {
		bool Debug(string message);
		bool Info(string message);
		bool Error(string message);
		bool OpenLogFile();
	}
}
