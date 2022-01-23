using System;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class SchemeHandlerFactoryLogic<TRequest, TResourceHandler> where TResourceHandler : class {
		private readonly ICustomSchemeHandler handler;
		private readonly IRequestAdapter<TRequest> requestAdapter;
		private readonly ISchemeResourceVisitor<TResourceHandler> resourceVisitor;

		public SchemeHandlerFactoryLogic(ICustomSchemeHandler handler, IRequestAdapter<TRequest> requestAdapter, IResourceHandlerFactory<TResourceHandler> resourceHandlerFactory) {
			this.handler = handler;
			this.requestAdapter = requestAdapter;
			this.resourceVisitor = new SchemeResourceVisitor<TResourceHandler>(resourceHandlerFactory);
		}

		public TResourceHandler? Create(TRequest request) {
			return Uri.TryCreate(requestAdapter.GetUrl(request), UriKind.Absolute, out var uri) ? handler.Resolve(uri)?.Visit(resourceVisitor) : null;
		}
	}
}
