namespace TweetLib.Browser.CEF.Interfaces {
	public interface IDragDataAdapter<T> {
		bool IsLink(T data);
		string GetLink(T data);

		bool IsFragment(T data);
		string GetFragmentAsText(T data);
	}
}
