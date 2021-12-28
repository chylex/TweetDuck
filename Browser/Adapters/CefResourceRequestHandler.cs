using System.Collections.Generic;
using CefSharp;
using CefSharp.Handler;
using TweetDuck.Browser.Handling;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;
using ResourceType = TweetLib.Browser.Request.ResourceType;

namespace TweetDuck.Browser.Adapters {
	sealed class CefResourceRequestHandler : ResourceRequestHandler {
		private readonly CefResourceHandlerRegistry resourceHandlerRegistry;
		private readonly IResourceRequestHandler resourceRequestHandler;
		private readonly Dictionary<ulong, IResponseProcessor> responseProcessors = new Dictionary<ulong, IResponseProcessor>();

		public CefResourceRequestHandler(CefResourceHandlerRegistry resourceHandlerRegistry, IResourceRequestHandler resourceRequestHandler) {
			this.resourceHandlerRegistry = resourceHandlerRegistry;
			this.resourceRequestHandler = resourceRequestHandler;
		}

		protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback) {
			if (request.ResourceType == CefSharp.ResourceType.CspReport) {
				callback.Dispose();
				return CefReturnValue.Cancel;
			}

			if (resourceRequestHandler != null) {
				var result = resourceRequestHandler.Handle(request.Url, TranslateResourceType(request.ResourceType));

				switch (result) {
					case RequestHandleResult.Redirect redirect:
						request.Url = redirect.Url;
						break;

					case RequestHandleResult.Process process:
						request.SetHeaderByName("Accept-Encoding", "identity", overwrite: true);
						responseProcessors[request.Identifier] = process.Processor;
						break;

					case RequestHandleResult.Cancel _:
						callback.Dispose();
						return CefReturnValue.Cancel;
				}
			}

			return base.OnBeforeResourceLoad(chromiumWebBrowser, browser, frame, request, callback);
		}

		protected override IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request) {
			return resourceHandlerRegistry?.GetHandler(request.Url);
		}

		protected override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
			if (responseProcessors.TryGetValue(request.Identifier, out var processor) && int.TryParse(response.Headers["Content-Length"], out int totalBytes)) {
				return new ResponseFilter(processor, totalBytes);
			}

			return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
		}

		protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) {
			responseProcessors.Remove(request.Identifier);
			base.OnResourceLoadComplete(chromiumWebBrowser, browser, frame, request, response, status, receivedContentLength);
		}

		private static ResourceType TranslateResourceType(CefSharp.ResourceType resourceType) {
			return resourceType switch {
				CefSharp.ResourceType.MainFrame  => ResourceType.MainFrame,
				CefSharp.ResourceType.Script     => ResourceType.Script,
				CefSharp.ResourceType.Stylesheet => ResourceType.Stylesheet,
				CefSharp.ResourceType.Xhr        => ResourceType.Xhr,
				CefSharp.ResourceType.Image      => ResourceType.Image,
				_                                => ResourceType.Unknown
			};
		}
	}
}
