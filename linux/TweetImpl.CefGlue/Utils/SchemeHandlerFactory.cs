using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TweetImpl.CefGlue.Adapters;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Utils {
	public sealed class SchemeHandlerFactory {
		private static readonly Dictionary<string, SchemeHandlerFactory> Factories = new ();

		internal static CefResourceHandler? TryGetHandler(CefRequest request) {
			if (Uri.TryCreate(request.Url, UriKind.Absolute, out Uri? uri) && Factories.TryGetValue(uri.Scheme, out var factory)) {
				return factory.Create(request);
			}
			else {
				return null;
			}
		}

		[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
		public static void Register(CefSchemeRegistrar registrar, ICustomSchemeHandler handler) {
			string protocol = handler.Protocol;
			Factories.Add(protocol, new SchemeHandlerFactory(handler));
			registrar.AddCustomScheme(protocol, CefSchemeOptions.Secure | CefSchemeOptions.CorsEnabled | CefSchemeOptions.CspBypassing);
		}

		private readonly SchemeHandlerFactoryLogic<CefRequest, CefResourceHandler> logic;

		private SchemeHandlerFactory(ICustomSchemeHandler handler) {
			this.logic = new SchemeHandlerFactoryLogic<CefRequest, CefResourceHandler>(handler, CefRequestAdapter.Instance, CefResourceHandlerFactory.Instance);
		}

		private CefResourceHandler? Create(CefRequest request) {
			return logic.Create(request);
		}
	}
}
