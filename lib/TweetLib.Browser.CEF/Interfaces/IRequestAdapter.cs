using TweetLib.Browser.Request;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IRequestAdapter<T> {
		ulong GetIdentifier(T request);

		string GetUrl(T request);

		void SetUrl(T request, string url);

		void SetMethod(T request, string method);

		bool IsTransitionForwardBack(T request);

		bool IsCspReport(T request);

		ResourceType GetResourceType(T request);

		void SetHeader(T request, string header, string value);

		void SetReferrer(T request, string referrer);

		void SetAllowStoredCredentials(T request);
	}
}
