namespace TweetLib.Browser.CEF.Interfaces {
	public interface IFrameAdapter<T> {
		bool IsValid(T frame);
		bool IsMain(T frame);
		void LoadUrl(T frame, string url);
		void ExecuteJavaScriptAsync(T frame, string script, string identifier, int startLine = 1);
	}
}
