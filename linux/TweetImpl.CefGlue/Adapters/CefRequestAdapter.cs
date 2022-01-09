using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Request;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefRequestAdapter : IRequestAdapter<CefRequest> {
		public static CefRequestAdapter Instance { get; } = new ();

		private CefRequestAdapter() {}

		public ulong GetIdentifier(CefRequest request) {
			return request.Identifier;
		}

		public string GetUrl(CefRequest request) {
			return request.Url;
		}

		public void SetUrl(CefRequest request, string url) {
			request.Url = url;
		}

		public void SetMethod(CefRequest request, string method) {
			request.Method = method;
		}

		public bool IsTransitionForwardBack(CefRequest request) {
			return request.TransitionType.HasFlag(CefTransitionType.ForwardBackFlag);
		}

		public bool IsCspReport(CefRequest request) {
			return request.ResourceType == CefResourceType.CspReport;
		}

		public ResourceType GetResourceType(CefRequest request) {
			return request.ResourceType switch {
				CefResourceType.MainFrame  => ResourceType.MainFrame,
				CefResourceType.Script     => ResourceType.Script,
				CefResourceType.Stylesheet => ResourceType.Stylesheet,
				CefResourceType.Xhr        => ResourceType.Xhr,
				CefResourceType.Image      => ResourceType.Image,
				_                          => ResourceType.Unknown
			};
		}

		public void SetHeader(CefRequest request, string header, string value) {
			request.SetHeaderByName(header, value, overwrite: true);
		}

		public void SetReferrer(CefRequest request, string referrer) {
			request.SetReferrer(referrer, CefReferrerPolicy.Default);
		}

		public void SetAllowStoredCredentials(CefRequest request) {
			request.Options |= CefUrlRequestOptions.AllowStoredCredentials;
		}
	}
}
