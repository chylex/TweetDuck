namespace TweetLib.Browser.CEF.Interfaces {
	public interface IResponseAdapter<T> {
		void SetCharset(T response, string charset);
		void SetMimeType(T response, string mimeType);
		void SetStatus(T response, int statusCode, string statusText);
		void SetHeader(T response, string header, string value);
		string? GetHeader(T response, string header);
	}
}
