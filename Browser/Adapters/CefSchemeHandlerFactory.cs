using System;
using CefSharp;
using CefSharp.WinForms;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Browser.Adapters {
	internal sealed class CefSchemeHandlerFactory : ISchemeHandlerFactory {
		public static void Register(CefSettings settings, ICustomSchemeHandler handler) {
			settings.RegisterScheme(new CefCustomScheme {
				SchemeName = handler.Protocol,
				IsStandard = false,
				IsSecure = true,
				IsCorsEnabled = true,
				IsCSPBypassing = true,
				SchemeHandlerFactory = new CefSchemeHandlerFactory(handler)
			});
		}

		private readonly ICustomSchemeHandler handler;

		private CefSchemeHandlerFactory(ICustomSchemeHandler handler) {
			this.handler = handler;
		}

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			return Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) ? handler.Resolve(uri)?.Visit(CefSchemeResourceVisitor.Instance) : null;
		}
	}
}
