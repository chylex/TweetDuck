using System.Reflection;
using Lunixo.ChromiumGtk;
using Lunixo.ChromiumGtk.Core;
using TweetImpl.CefGlue.Handlers;
using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Component {
	public sealed class CustomWebClient : WebClient {
		public static WebView CreateWebView(IPopupHandler popupHandler) {
			WebView view = new WebView();
			WebBrowser browser = view.Browser;
			typeof(WebBrowser).GetField("<Client>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(browser, new CustomWebClient(browser, popupHandler));
			return view;
		}

		internal LifeSpanHandler LifeSpanHandler { get; }

		private CustomWebClient(WebBrowser core, IPopupHandler popupHandler) : base(core) {
			LifeSpanHandler = new LifeSpanHandler(core, popupHandler);
		}

		protected override CefLifeSpanHandler GetLifeSpanHandler() {
			return LifeSpanHandler;
		}
	}
}
