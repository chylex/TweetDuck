using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Utils {
	static class BrowserUtils {
		public static string UserAgentChrome => "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/" + Cef.ChromiumVersion + " Safari/537.36";

		private static UserConfig Config => Program.Config.User;
		private static SystemConfig SysConfig => Program.Config.System;

		public static void SetupCefArgs(IDictionary<string, string> args) {
			if (!SysConfig.HardwareAcceleration) {
				args["disable-gpu"] = "1";
				args["disable-gpu-compositing"] = "1";
			}

			if (!Config.EnableSmoothScrolling) {
				args["disable-smooth-scrolling"] = "1";
			}

			if (!SysConfig.EnableTouchAdjustment) {
				args["disable-touch-adjustment"] = "1";
			}

			if (!SysConfig.EnableColorProfileDetection) {
				args["force-color-profile"] = "srgb";
			}

			args["disable-component-update"] = "1";
			args["disable-pdf-extension"] = "1";
			args["disable-plugins-discovery"] = "1";
			args["enable-system-flash"] = "0";

			if (args.TryGetValue("js-flags", out var jsFlags)) {
				args["js-flags"] = "--expose-gc " + jsFlags;
			}
			else {
				args["js-flags"] = "--expose-gc";
			}
		}

		public static void SetupDockOnLoad(IBrowserComponent browserComponent, ChromiumWebBrowser browser) {
			browser.Dock = DockStyle.None;
			browser.Location = ControlExtensions.InvisibleLocation;

			browserComponent.BrowserLoaded += (sender, args) => {
				browser.InvokeAsyncSafe(() => {
					browser.Location = Point.Empty;
					browser.Dock = DockStyle.Fill;
				});
			};
		}

		public static void SetupZoomEvents(this ChromiumWebBrowser browser) {
			static void SetZoomLevel(IBrowserHost host, int percentage) {
				host.SetZoomLevel(Math.Log(percentage / 100.0, 1.2));
			}

			void UpdateZoomLevel(object? sender, EventArgs args) {
				SetZoomLevel(browser.GetBrowserHost(), Config.ZoomLevel);
			}

			Config.ZoomLevelChanged += UpdateZoomLevel;
			browser.Disposed += (sender, args) => Config.ZoomLevelChanged -= UpdateZoomLevel;

			browser.FrameLoadStart += (sender, args) => {
				if (args.Frame.IsMain && Config.ZoomLevel != 100) {
					SetZoomLevel(args.Browser.GetHost(), Config.ZoomLevel);
				}
			};
		}

		public static void OpenDevToolsCustom(this IWebBrowser browser, Point? inspectPoint = null) {
			var info = new WindowInfo();
			info.SetAsPopup(IntPtr.Zero, "Dev Tools");

			if (Config.DevToolsWindowOnTop) {
				info.ExStyle |= 0x00000008; // WS_EX_TOPMOST
			}

			Point p = inspectPoint ?? Point.Empty;
			browser.GetBrowserHost().ShowDevTools(info, p.X, p.Y);
		}

		public static int Scale(int baseValue, double scaleFactor) {
			return (int) Math.Round(baseValue * scaleFactor);
		}
	}
}
