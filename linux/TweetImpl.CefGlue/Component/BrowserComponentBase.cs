using Gtk;
using Lunixo.ChromiumGtk.Core;
using TweetImpl.CefGlue.Adapters;
using TweetImpl.CefGlue.Handlers;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.Base;
using TweetLib.Browser.CEF.Component;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Component {
	public abstract class BrowserComponentBase : BrowserComponent<CefFrame, CefRequest> {
		public delegate ContextMenuHandler CreateContextMenu(IContextMenuHandler? handler);

		private ResourceHandlerRegistry<CefResourceHandler> ResourceHandlerRegistry { get; } = new (CefResourceHandlerFactory.Instance);
		private BridgeObjectRegistry BridgeObjectRegistry { get; } = new ();

		private readonly Window window;
		private readonly CustomWebClient client;
		private readonly CreateContextMenu createContextMenu;
		private readonly bool autoReload;

		protected BrowserComponentBase(Window window, WebBrowser wrapper, CreateContextMenu createContextMenu, bool autoReload) : base(new CefBrowserAdapter(wrapper), CefAdapter.Instance, CefFrameAdapter.Instance, CefRequestAdapter.Instance) {
			this.window = window;
			this.client = (CustomWebClient) wrapper.Client;
			this.client.LoadHandler = new LoadHandler(this);
			this.createContextMenu = createContextMenu;
			this.autoReload = autoReload;
		}

		public override void Setup(BrowserSetup setup) {
			var resourceRequestHandlerFactory = new ResourceRequestHandlerFactory(setup.ResourceRequestHandler, ResourceHandlerRegistry, BridgeObjectRegistry);
			var requestHandler = new RequestHandler(client.LifeSpanHandler, resourceRequestHandlerFactory, autoReload);

			client.ContextMenuHandler = createContextMenu(setup.ContextMenuHandler);
			client.DialogHandler = new DialogHandler(window);
			client.DragHandler = new DragHandler(requestHandler, this);
			client.JSDialogHandler = new JsDialogHandler(window);
			client.RequestHandler = requestHandler;
		}

		public override void AttachBridgeObject(string name, object bridge) {
			BridgeObjectRegistry.Attach(name, bridge);
		}

		private sealed class LoadHandler : CefLoadHandler {
			private readonly BrowserComponentBase component;

			public LoadHandler(BrowserComponentBase component) {
				this.component = component;
			}

			protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward) {
				component.OnLoadingStateChanged(isLoading);
			}

			protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType) {
				component.OnFrameLoadStart(frame.Url, frame);
			}

			protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode) {
				component.BridgeObjectRegistry.RunScripts(component);
				component.OnFrameLoadEnd(frame.Url, frame);
			}

			protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl) {
				component.OnLoadError(failedUrl, errorCode, CefErrorCodeAdapter.Instance);
			}
		}
	}
}
