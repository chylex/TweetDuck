using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class RequestHandlerLogic<TRequest> {
		internal string BlockNextUserNavUrl { get; set; } = string.Empty;

		private readonly IRequestAdapter<TRequest> requestAdapter;
		private readonly LifeSpanHandlerLogic lifeSpanHandlerLogic;

		public RequestHandlerLogic(IRequestAdapter<TRequest> requestAdapter, LifeSpanHandlerLogic lifeSpanHandlerLogic) {
			this.requestAdapter = requestAdapter;
			this.lifeSpanHandlerLogic = lifeSpanHandlerLogic;
		}

		private bool ShouldBlockNav(string url) {
			bool block = url == BlockNextUserNavUrl;
			BlockNextUserNavUrl = string.Empty;
			return block;
		}

		public bool OnBeforeBrowse(TRequest request, bool userGesture) {
			return requestAdapter.IsTransitionForwardBack(request) || (userGesture && ShouldBlockNav(requestAdapter.GetUrl(request)));
		}

		public bool OnOpenUrlFromTab(string targetUrl, bool userGesture, LifeSpanHandlerLogic.TargetDisposition targetDisposition) {
			return (userGesture && ShouldBlockNav(targetUrl)) || lifeSpanHandlerLogic.OnBeforePopup(targetUrl, targetDisposition);
		}
	}
}
