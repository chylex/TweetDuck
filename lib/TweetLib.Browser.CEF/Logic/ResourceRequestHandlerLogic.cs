using System;
using System.Collections.Generic;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class ResourceRequestHandlerLogic<TRequest, TResponse, TResourceHandler> where TResourceHandler : class {
		private readonly IRequestAdapter<TRequest> requestAdapter;
		private readonly IResponseAdapter<TResponse> responseAdapter;
		private readonly ResourceHandlerRegistry<TResourceHandler> resourceHandlerRegistry;
		private readonly IResourceRequestHandler? resourceRequestHandler;

		private readonly Dictionary<ulong, IResponseProcessor> responseProcessors = new ();

		public ResourceRequestHandlerLogic(IRequestAdapter<TRequest> requestAdapter, IResponseAdapter<TResponse> responseAdapter, ResourceHandlerRegistry<TResourceHandler> resourceHandlerRegistry, IResourceRequestHandler? resourceRequestHandler) {
			this.requestAdapter = requestAdapter;
			this.responseAdapter = responseAdapter;
			this.resourceHandlerRegistry = resourceHandlerRegistry;
			this.resourceRequestHandler = resourceRequestHandler;
		}

		public bool OnBeforeResourceLoad(TRequest request, IDisposable callback) {
			if (requestAdapter.IsCspReport(request)) {
				callback.Dispose();
				return false;
			}

			if (resourceRequestHandler != null) {
				var result = resourceRequestHandler.Handle(requestAdapter.GetUrl(request), requestAdapter.GetResourceType(request));

				switch (result) {
					case RequestHandleResult.Redirect redirect:
						requestAdapter.SetUrl(request, redirect.Url);
						break;

					case RequestHandleResult.Process process:
						requestAdapter.SetHeader(request, "Accept-Encoding", "identity");
						responseProcessors[requestAdapter.GetIdentifier(request)] = process.Processor;
						break;

					case RequestHandleResult.Cancel:
						callback.Dispose();
						return false;
				}
			}

			return true;
		}

		public TResourceHandler? GetResourceHandler(TRequest request) {
			return resourceHandlerRegistry.GetHandler(requestAdapter.GetUrl(request));
		}

		public ResponseFilterLogic? GetResourceResponseFilter(TRequest request, TResponse response) {
			if (responseProcessors.TryGetValue(requestAdapter.GetIdentifier(request), out var processor) && int.TryParse(responseAdapter.GetHeader(response, "Content-Length"), out int totalBytes)) {
				return new ResponseFilterLogic(processor, totalBytes);
			}

			return null;
		}

		public void OnResourceLoadComplete(TRequest request) {
			responseProcessors.Remove(requestAdapter.GetIdentifier(request));
		}
	}
}
