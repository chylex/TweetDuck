using CefSharp;
using TweetLib.Browser.CEF.Interfaces;
using ResourceType = TweetLib.Browser.Request.ResourceType;

namespace TweetDuck.Browser.Base {
	sealed class CefRequestAdapter : IRequestAdapter<IRequest> {
		public static CefRequestAdapter Instance { get; } = new CefRequestAdapter();

		private CefRequestAdapter() {}

		public ulong GetIdentifier(IRequest request) {
			return request.Identifier;
		}

		public string GetUrl(IRequest request) {
			return request.Url;
		}

		public void SetUrl(IRequest request, string url) {
			request.Url = url;
		}

		public void SetMethod(IRequest request, string method) {
			request.Method = method;
		}

		public bool IsTransitionForwardBack(IRequest request) {
			return request.TransitionType.HasFlag(TransitionType.ForwardBack);
		}

		public bool IsCspReport(IRequest request) {
			return request.ResourceType == CefSharp.ResourceType.CspReport;
		}

		public ResourceType GetResourceType(IRequest request) {
			return request.ResourceType switch {
				CefSharp.ResourceType.MainFrame  => ResourceType.MainFrame,
				CefSharp.ResourceType.Script     => ResourceType.Script,
				CefSharp.ResourceType.Stylesheet => ResourceType.Stylesheet,
				CefSharp.ResourceType.Xhr        => ResourceType.Xhr,
				CefSharp.ResourceType.Image      => ResourceType.Image,
				_                                => ResourceType.Unknown
			};
		}

		public void SetHeader(IRequest request, string header, string value) {
			request.SetHeaderByName(header, value, overwrite: true);
		}

		public void SetReferrer(IRequest request, string referrer) {
			request.SetReferrer(referrer, ReferrerPolicy.Default);
		}

		public void SetAllowStoredCredentials(IRequest request) {
			request.Flags |= UrlRequestFlags.AllowStoredCredentials;
		}
	}
}
