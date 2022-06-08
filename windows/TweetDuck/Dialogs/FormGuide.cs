using System.Drawing;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Browser;
using TweetDuck.Browser.Base;
using TweetDuck.Controls;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Core.Features;

namespace TweetDuck.Dialogs {
	sealed partial class FormGuide : Form, FormManager.IAppDialog {
		private const string GuideUrl = "td://guide/index.html";

		public static void Show(string? hash = null) {
			string url = GuideUrl + (string.IsNullOrEmpty(hash) ? string.Empty : "#" + hash);
			FormGuide? guide = FormManager.TryFind<FormGuide>();

			if (guide == null) {
				FormBrowser? owner = FormManager.TryFind<FormBrowser>();

				if (owner != null) {
					new FormGuide(url, owner).Show(owner);
				}
			}
			else {
				guide.Reload(url);
				guide.Activate();
			}
		}

		private readonly ChromiumWebBrowser browser;

		private FormGuide(string url, Form owner) {
			InitializeComponent();

			Text = Program.BrandName + " Guide";
			Size = new Size(owner.Size.Width * 3 / 4, owner.Size.Height * 3 / 4);
			VisibleChanged += (sender, args) => this.MoveToCenter(owner);

			browser = new ChromiumWebBrowser(url) {
				KeyboardHandler = new CustomKeyboardHandler(null)
			};

			browser.BrowserSettings.BackgroundColor = (uint) BackColor.ToArgb();

			var browserComponent = new CefBrowserComponent(browser);
			var browserImpl = new BaseBrowser(browserComponent);

			BrowserUtils.SetupDockOnLoad(browserComponent, browser);

			Controls.Add(browser);

			Disposed += (sender, args) => {
				browserImpl.Dispose();
				browser.Dispose();
			};
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
