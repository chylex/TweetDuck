using CefSharp;
using CefSharp.WinForms;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefSchemeHandlerFactory : ISchemeHandlerFactory {
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

		private readonly SchemeHandlerFactoryLogic<IRequest, IResourceHandler> logic;

		private CefSchemeHandlerFactory(ICustomSchemeHandler handler) {
			this.logic = new SchemeHandlerFactoryLogic<IRequest, IResourceHandler>(handler, CefRequestAdapter.Instance, CefResourceHandlerFactory.Instance);
		}

		IResourceHandler ISchemeHandlerFactory.Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			return logic.Create(request);
		}
	}
}
