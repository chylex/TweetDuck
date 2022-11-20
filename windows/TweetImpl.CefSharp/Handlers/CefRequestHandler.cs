using System.Diagnostics;
using CefSharp;
using CefSharp.Handler;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Logic;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefRequestHandler : RequestHandler {
		public RequestHandlerLogic<IRequest> Logic { get; }

		private readonly AutoReloader? autoReloader;

		public CefRequestHandler(CefLifeSpanHandler lifeSpanHandler, bool autoReload) {
			this.Logic = new RequestHandlerLogic<IRequest>(CefRequestAdapter.Instance, lifeSpanHandler.Logic);
			this.autoReloader = autoReload ? new AutoReloader() : null;
		}

		protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
			return Logic.OnBeforeBrowse(request, userGesture);
		}

		protected override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) {
			return Logic.OnOpenUrlFromTab(targetUrl, userGesture, CefLifeSpanHandler.ConvertTargetDisposition(targetDisposition));
		}

		protected override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) {
			if (autoReloader?.RequestReload() == true) {
				browser.Reload();
			}
		}

		private sealed class AutoReloader {
			private readonly Stopwatch lastReload = Stopwatch.StartNew();
			private int rapidReloadCount;
			
			public bool RequestReload() {
				if (rapidReloadCount >= 2) {
					lastReload.Stop();
					return false;
				}
				
				if (lastReload.ElapsedMilliseconds < 5000) {
					++rapidReloadCount;
				}
				else {
					rapidReloadCount = 0;
				}
				
				lastReload.Restart();
				return true;
			}
		}
	}
}
