namespace TweetLib.Browser.CEF.Interfaces {
	public interface IFileDialogCallbackAdapter<T> {
		void Continue(T callback, int selectedAcceptFilter, string[] filePaths);
		void Cancel(T callback);
		void Dispose(T callback);
	}
}
