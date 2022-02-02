using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser.Handling;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Browser.Base;
using TweetLib.Browser.CEF.Component;
using TweetLib.Browser.CEF.Data;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetDuck.Browser.Base {
	sealed class CefBrowserComponent : BrowserComponent<IFrame, IRequest> {
		public delegate ContextMenuBase CreateContextMenu(IContextMenuHandler handler);

		private static readonly CreateContextMenu DefaultContextMenuFactory = handler => new ContextMenuBase(handler);

		public override string CacheFolder => BrowserCache.CacheFolder;

		public ResourceHandlerRegistry<IResourceHandler> ResourceHandlerRegistry { get; } = new ResourceHandlerRegistry<IResourceHandler>(CefResourceHandlerFactory.Instance);

		private readonly ChromiumWebBrowser browser;
		private readonly bool autoReload;

		private CreateContextMenu createContextMenu;

		public CefBrowserComponent(ChromiumWebBrowser browser, CreateContextMenu createContextMenu = null, bool autoReload = true) : base(new CefBrowserAdapter(browser), CefAdapter.Instance, CefFrameAdapter.Instance, CefRequestAdapter.Instance) {
			this.browser = browser;
			this.browser.LoadingStateChanged += OnLoadingStateChanged;
			this.browser.LoadError += OnLoadError;
			this.browser.FrameLoadStart += OnFrameLoadStart;
			this.browser.FrameLoadEnd += OnFrameLoadEnd;
			this.browser.SetupZoomEvents();
			this.createContextMenu = createContextMenu ?? DefaultContextMenuFactory;
			this.autoReload = autoReload;
		}

		public override void Setup(BrowserSetup setup) {
			var lifeSpanHandler = new CefLifeSpanHandler(PopupHandler.Instance);
			var requestHandler = new CefRequestHandler(lifeSpanHandler, autoReload);

			browser.DragHandler = new CefDragHandler(requestHandler, this);
			browser.JsDialogHandler = new CustomJsDialogHandler();
			browser.LifeSpanHandler = lifeSpanHandler;
			browser.MenuHandler = createContextMenu(setup.ContextMenuHandler);
			browser.RequestHandler = requestHandler;
			browser.ResourceRequestHandlerFactory = new CefResourceRequestHandlerFactory(setup.ResourceRequestHandler, ResourceHandlerRegistry);

			createContextMenu = null;
		}

		public override void AttachBridgeObject(string name, object bridge) {
			browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
			browser.JavascriptObjectRepository.Register(name, bridge, isAsync: true, BindingOptions.DefaultBinder);
		}

		private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			base.OnLoadingStateChanged(e.IsLoading);
		}

		private void OnLoadError(object sender, LoadErrorEventArgs e) {
			base.OnLoadError(e.FailedUrl, e.ErrorCode, CefErrorCodeAdapter.Instance);
		}

		private void OnFrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			base.OnFrameLoadStart(e.Url, e.Frame);
		}

		private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			base.OnFrameLoadEnd(e.Url, e.Frame);
		}
	}
}
