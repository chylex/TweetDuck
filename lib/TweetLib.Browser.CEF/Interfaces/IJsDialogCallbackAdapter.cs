namespace TweetLib.Browser.CEF.Interfaces {
	public interface IJsDialogCallbackAdapter<T> {
		void Continue(T callback, bool success, string? userInput = null);
		void Dispose(T callback);
	}
}
