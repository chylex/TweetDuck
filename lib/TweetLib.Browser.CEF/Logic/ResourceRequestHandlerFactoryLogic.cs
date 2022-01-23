using System.Diagnostics.CodeAnalysis;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class ResourceRequestHandlerFactoryLogic<TResourceRequestHandler, TResourceHandler, TRequest> where TResourceHandler : class {
		private readonly IRequestAdapter<TRequest> requestAdapter;
		private readonly TResourceRequestHandler handler;
		private readonly ResourceHandlerRegistry<TResourceHandler> registry;

		public ResourceRequestHandlerFactoryLogic(IRequestAdapter<TRequest> requestAdapter, TResourceRequestHandler handler, ResourceHandlerRegistry<TResourceHandler> registry) {
			this.handler = handler;
			this.registry = registry;
			this.requestAdapter = requestAdapter;
		}

		[SuppressMessage("ReSharper", "RedundantAssignment")]
		public TResourceRequestHandler GetResourceRequestHandler(TRequest request, ref bool disableDefaultHandling) {
			disableDefaultHandling = registry.HasHandler(requestAdapter.GetUrl(request));
			return handler;
		}
	}
}
