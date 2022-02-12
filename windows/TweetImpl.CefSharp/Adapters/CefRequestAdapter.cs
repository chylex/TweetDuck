using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
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
			return request.ResourceType == ResourceType.CspReport;
		}

		public TweetLib.Browser.Request.ResourceType GetResourceType(IRequest request) {
			return request.ResourceType switch {
				ResourceType.MainFrame  => TweetLib.Browser.Request.ResourceType.MainFrame,
				ResourceType.Script     => TweetLib.Browser.Request.ResourceType.Script,
				ResourceType.Stylesheet => TweetLib.Browser.Request.ResourceType.Stylesheet,
				ResourceType.Xhr        => TweetLib.Browser.Request.ResourceType.Xhr,
				ResourceType.Image      => TweetLib.Browser.Request.ResourceType.Image,
				_                       => TweetLib.Browser.Request.ResourceType.Unknown
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
