using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser;
using TweetDuck.Configuration;
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetLib.Core;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Utils {
	static class BrowserUtils {
		public static string UserAgentVanilla => Program.BrandName + " " + System.Windows.Forms.Application.ProductVersion;
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

			if (!Config.EnableTouchAdjustment) {
				args["disable-touch-adjustment"] = "1";
			}

			if (!Config.EnableColorProfileDetection) {
				args["force-color-profile"] = "srgb";
			}

			args["disable-component-update"] = "1";
			args["disable-pdf-extension"] = "1";
			args["disable-plugins-discovery"] = "1";
			args["enable-system-flash"] = "0";

			if (args.TryGetValue("js-flags", out string jsFlags)) {
				args["js-flags"] = "--expose-gc " + jsFlags;
			}
			else {
				args["js-flags"] = "--expose-gc";
			}
		}

		public static void SetupCustomScheme(this CefSettings settings, string name, ISchemeHandlerFactory factory) {
			settings.RegisterScheme(new CefCustomScheme {
				SchemeName = name,
				IsStandard = false,
				IsSecure = true,
				IsCorsEnabled = true,
				IsCSPBypassing = true,
				SchemeHandlerFactory = factory
			});
		}

		public static ChromiumWebBrowser AsControl(this IWebBrowser browserControl) {
			return (ChromiumWebBrowser) browserControl;
		}

		public static void SetupZoomEvents(this ChromiumWebBrowser browser) {
			static void SetZoomLevel(IBrowserHost host, int percentage) {
				host.SetZoomLevel(Math.Log(percentage / 100.0, 1.2));
			}

			void UpdateZoomLevel(object sender, EventArgs args) {
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

		public static void RegisterJsBridge(this IWebBrowser browserControl, string name, object bridge) {
			browserControl.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
			browserControl.JavascriptObjectRepository.Register(name, bridge, isAsync: true, BindingOptions.DefaultBinder);
		}

		public static void ExecuteJsAsync(this IWebBrowser browserControl, string scriptOrMethodName, params object[] args) {
			if (args.Length == 0) {
				browserControl.BrowserCore.ExecuteScriptAsync(scriptOrMethodName);
			}
			else {
				browserControl.BrowserCore.ExecuteScriptAsync(scriptOrMethodName, args);
			}
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

		public static void OpenExternalBrowser(string url) {
			if (string.IsNullOrWhiteSpace(url)) {
				return;
			}

			switch (TwitterUrls.Check(url)) {
				case TwitterUrls.UrlType.Fine:
					string browserPath = Config.BrowserPath;

					if (browserPath == null || !File.Exists(browserPath)) {
						App.SystemHandler.OpenAssociatedProgram(url);
					}
					else {
						string quotedUrl = '"' + url + '"';
						string browserArgs = Config.BrowserPathArgs == null ? quotedUrl : Config.BrowserPathArgs + ' ' + quotedUrl;

						try {
							using (Process.Start(browserPath, browserArgs)) {}
						} catch (Exception e) {
							App.ErrorHandler.HandleException("Error Opening Browser", "Could not open the browser.", true, e);
						}
					}

					break;

				case TwitterUrls.UrlType.Tracking:
					if (Config.IgnoreTrackingUrlWarning) {
						goto case TwitterUrls.UrlType.Fine;
					}

					using (FormMessage form = new FormMessage("Blocked URL", "TweetDuck has blocked a tracking url due to privacy concerns. Do you want to visit it anyway?\n" + url, MessageBoxIcon.Warning)) {
						form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel | ControlType.Focused);
						form.AddButton(FormMessage.Yes, DialogResult.Yes, ControlType.Accept);
						form.AddButton("Always Visit", DialogResult.Ignore);

						DialogResult result = form.ShowDialog();

						if (result == DialogResult.Ignore) {
							Config.IgnoreTrackingUrlWarning = true;
							Config.Save();
						}

						if (result == DialogResult.Ignore || result == DialogResult.Yes) {
							goto case TwitterUrls.UrlType.Fine;
						}
					}

					break;

				case TwitterUrls.UrlType.Invalid:
					FormMessage.Warning("Blocked URL", "A potentially malicious or invalid URL was blocked from opening:\n" + url, FormMessage.OK);
					break;
			}
		}

		public static void OpenExternalSearch(string query) {
			if (string.IsNullOrWhiteSpace(query)) {
				return;
			}

			string searchUrl = Config.SearchEngineUrl;

			if (string.IsNullOrEmpty(searchUrl)) {
				if (FormMessage.Question("Search Options", "You have not configured a default search engine yet, would you like to do it now?", FormMessage.Yes, FormMessage.No)) {
					bool wereSettingsOpen = FormManager.TryFind<FormSettings>() != null;

					FormManager.TryFind<FormBrowser>()?.OpenSettings();

					if (wereSettingsOpen) {
						return;
					}

					FormSettings settings = FormManager.TryFind<FormSettings>();

					if (settings == null) {
						return;
					}

					settings.FormClosed += (sender, args) => {
						if (args.CloseReason == CloseReason.UserClosing && Config.SearchEngineUrl != searchUrl) {
							OpenExternalSearch(query);
						}
					};
				}
			}
			else {
				OpenExternalBrowser(searchUrl + Uri.EscapeUriString(query));
			}
		}

		public static int Scale(int baseValue, double scaleFactor) {
			return (int) Math.Round(baseValue * scaleFactor);
		}
	}
}
