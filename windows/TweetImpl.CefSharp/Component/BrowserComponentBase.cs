using CefSharp;
using CefSharp.WinForms;
using TweetImpl.CefSharp.Adapters;
using TweetImpl.CefSharp.Handlers;
using TweetLib.Browser.Base;
using TweetLib.Browser.CEF.Component;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetImpl.CefSharp.Component {
	public abstract class BrowserComponentBase : BrowserComponent<IFrame, IRequest> {
		public delegate CefContextMenuHandler CreateContextMenu(IContextMenuHandler? handler);

		public ResourceHandlerRegistry<IResourceHandler> ResourceHandlerRegistry { get; } = new (CefResourceHandlerFactory.Instance);

		private readonly ChromiumWebBrowser browser;
		private readonly CreateContextMenu createContextMenu;
		private readonly IJsDialogOpener jsDialogOpener;
		private readonly IPopupHandler popupHandler;
		private readonly bool autoReload;

		protected BrowserComponentBase(ChromiumWebBrowser browser, CreateContextMenu createContextMenu, IJsDialogOpener jsDialogOpener, IPopupHandler popupHandler, bool autoReload) : base(new CefBrowserAdapter(browser), CefAdapter.Instance, CefFrameAdapter.Instance, CefRequestAdapter.Instance) {
			this.browser = browser;
			this.browser.LoadingStateChanged += OnLoadingStateChanged;
			this.browser.LoadError += OnLoadError;
			this.browser.FrameLoadStart += OnFrameLoadStart;
			this.browser.FrameLoadEnd += OnFrameLoadEnd;
			this.createContextMenu = createContextMenu;
			this.jsDialogOpener = jsDialogOpener;
			this.popupHandler = popupHandler;
			this.autoReload = autoReload;
		}

		public override void Setup(BrowserSetup setup) {
			var lifeSpanHandler = new CefLifeSpanHandler(popupHandler);
			var requestHandler = new CefRequestHandler(lifeSpanHandler, autoReload);

			browser.DialogHandler = new CefFileDialogHandler();
			browser.DragHandler = new CefDragHandler(requestHandler, this);
			browser.LifeSpanHandler = lifeSpanHandler;
			browser.JsDialogHandler = new CefJsDialogHandler(jsDialogOpener);
			browser.MenuHandler = createContextMenu(setup.ContextMenuHandler);
			browser.RequestHandler = requestHandler;
			browser.ResourceRequestHandlerFactory = new CefResourceRequestHandlerFactory(setup.ResourceRequestHandler, ResourceHandlerRegistry);
		}

		public override void AttachBridgeObject(string name, object bridge) {
			browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
			browser.JavascriptObjectRepository.Register(name, bridge);
		}

		private void OnLoadingStateChanged(object? sender, LoadingStateChangedEventArgs e) {
			base.OnLoadingStateChanged(e.IsLoading);
		}

		private void OnLoadError(object? sender, LoadErrorEventArgs e) {
			base.OnLoadError(e.FailedUrl, e.ErrorCode, CefErrorCodeAdapter.Instance);
		}

		private void OnFrameLoadStart(object? sender, FrameLoadStartEventArgs e) {
			base.OnFrameLoadStart(e.Url, e.Frame);
		}

		private void OnFrameLoadEnd(object? sender, FrameLoadEndEventArgs e) {
			base.OnFrameLoadEnd(e.Url, e.Frame);
		}
	}
}
