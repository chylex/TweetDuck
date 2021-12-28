using System.Drawing;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Browser;
using TweetDuck.Browser.Adapters;
using TweetDuck.Browser.Handling;
using TweetDuck.Controls;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features;

namespace TweetDuck.Dialogs {
	sealed partial class FormGuide : Form, FormManager.IAppDialog {
		private const string GuideUrl = "td://guide/index.html";

		public static void Show(string hash = null) {
			string url = GuideUrl + (string.IsNullOrEmpty(hash) ? string.Empty : "#" + hash);
			FormGuide guide = FormManager.TryFind<FormGuide>();

			if (guide == null) {
				FormBrowser owner = FormManager.TryFind<FormBrowser>();

				if (owner != null) {
					new FormGuide(url, owner).Show(owner);
				}
			}
			else {
				guide.Reload(url);
				guide.Activate();
			}
		}

		#pragma warning disable IDE0069 // Disposable fields should be disposed
		private readonly ChromiumWebBrowser browser;
		#pragma warning restore IDE0069 // Disposable fields should be disposed

		private FormGuide(string url, Form owner) {
			InitializeComponent();

			Text = Program.BrandName + " Guide";
			Size = new Size(owner.Size.Width * 3 / 4, owner.Size.Height * 3 / 4);
			VisibleChanged += (sender, args) => this.MoveToCenter(owner);

			browser = new ChromiumWebBrowser(url) {
				KeyboardHandler = new CustomKeyboardHandler(null),
				RequestHandler = new RequestHandlerBase(true)
			};

			browser.BrowserSettings.BackgroundColor = (uint) BackColor.ToArgb();

			var browserComponent = new ComponentImpl(browser);
			var browserImpl = new BaseBrowser(browserComponent);

			BrowserUtils.SetupDockOnLoad(browserComponent, browser);

			Controls.Add(browser);

			Disposed += (sender, args) => {
				browserImpl.Dispose();
				browser.Dispose();
			};
		}

		private sealed class ComponentImpl : CefBrowserComponent {
			public ComponentImpl(ChromiumWebBrowser browser) : base(browser) {}

			protected override ContextMenuBase SetupContextMenu(IContextMenuHandler handler) {
				return new ContextMenuGuide(handler);
			}

			protected override CefResourceHandlerFactory SetupResourceHandlerFactory(IResourceRequestHandler handler) {
				return new CefResourceHandlerFactory(handler, null);
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void Reload(string url) {
			browser.Load(url);
		}
	}
}
