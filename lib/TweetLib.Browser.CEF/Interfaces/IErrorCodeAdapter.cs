namespace TweetLib.Browser.CEF.Interfaces {
	public interface IErrorCodeAdapter<T> {
		bool IsAborted(T errorCode);
		string? GetName(T errorCode);
	}
}
