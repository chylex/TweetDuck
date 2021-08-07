using CefSharp;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Handling {
	sealed class RequestHandlerBrowser : RequestHandlerBase {
		public string BlockNextUserNavUrl { get; set; }

		public RequestHandlerBrowser() : base(true) {}

		protected override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
			if (userGesture && request.TransitionType == TransitionType.LinkClicked) {
				bool block = request.Url == BlockNextUserNavUrl;
				BlockNextUserNavUrl = string.Empty;
				return block;
			}
			else if (request.TransitionType.HasFlag(TransitionType.ForwardBack) && TwitterUrls.IsTweetDeck(frame.Url)) {
				return true;
			}

			return base.OnBeforeBrowse(browserControl, browser, frame, request, userGesture, isRedirect);
		}
	}
}
