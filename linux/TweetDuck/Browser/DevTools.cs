using System;
using Gtk;
using Lunixo.ChromiumGtk.Interop;
using TweetDuck.Utils;
using Xilium.CefGlue;

namespace TweetDuck.Browser {
	sealed class DevTools : IDisposable {
		public event EventHandler? Disposed;

		private readonly CefBrowserHost host;
		private readonly Window window;
		private bool isShown;
		private int initialX, initialY;

		public DevTools(CefBrowserHost host) {
			this.host = host;
			this.window = WindowManager.CreateWindow("Dev Tools");
			this.window.SizeAllocated += WindowOnSizeAllocated;
			this.window.Destroyed += (_, _) => Dispose();
		}

		private void WindowOnSizeAllocated(object o, SizeAllocatedArgs args) {
			if (args.Allocation.Width != 200) {
				isShown = true;
				window.SizeAllocated -= WindowOnSizeAllocated;
				Show(initialX, initialY);
			}
		}

		public void Show(int x, int y) {
			if (!isShown) {
				initialX = x;
				initialY = y;
				window.ShowAll();
				return;
			}

			var xid = InteropLinux.gdk_x11_window_get_xid(InteropLinux.gtk_widget_get_window(window.Handle));
			var windowInfo = CefWindowInfo.Create();
			windowInfo.SetAsChild(xid, new CefRectangle(0, 0, window.AllocatedWidth, window.AllocatedHeight));
			host.ShowDevTools(windowInfo, new DummyClient(), new CefBrowserSettings(), new CefPoint(x, y));
			window.Present();
		}

		public void Dispose() {
			host.CloseDevTools();
			host.Dispose();
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		private sealed class DummyClient : CefClient {}
	}
}
