using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Browser;
using TweetDuck.Browser.Adapters;
using TweetDuck.Browser.Data;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Handling.General;
using TweetDuck.Controls;
using TweetDuck.Management;
using TweetDuck.Utils;

namespace TweetDuck.Dialogs {
	sealed partial class FormGuide : Form, FormManager.IAppDialog {
		private const string GuideUrl = "https://tweetduck.chylex.com/guide/v2/";
		private const string GuidePathRegex = @"^guide(?:/v\d+)?(?:/(#.*))?";

		private static readonly ResourceLink DummyPage = new ResourceLink("http://td/dummy", ResourceHandlers.ForString(string.Empty));

		public static bool CheckGuideUrl(string url, out string hash) {
			if (!url.Contains("//tweetduck.chylex.com/guide")) {
				hash = null;
				return false;
			}

			string path = url.Substring(url.IndexOf("/guide") + 1);
			Match match = Regex.Match(path, GuidePathRegex);

			if (match.Success) {
				hash = match.Groups[1].Value;
				return true;
			}
			else {
				hash = null;
				return false;
			}
		}

		public static void Show(string hash = null) {
			string url = GuideUrl + (hash ?? string.Empty);
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

		private string nextUrl;

		private FormGuide(string url, FormBrowser owner) {
			InitializeComponent();

			Text = Program.BrandName + " Guide";
			Size = new Size(owner.Size.Width * 3 / 4, owner.Size.Height * 3 / 4);
			VisibleChanged += (sender, args) => this.MoveToCenter(owner);

			var resourceRequestHandler = new ResourceRequestHandlerBase();
			resourceRequestHandler.ResourceHandlers.Register(DummyPage);

			this.browser = new ChromiumWebBrowser(url) {
				MenuHandler = new ContextMenuGuide(),
				JsDialogHandler = new JavaScriptDialogHandler(),
				KeyboardHandler = new KeyboardHandlerBase(),
				LifeSpanHandler = new CustomLifeSpanHandler(),
				RequestHandler = new RequestHandlerBase(true),
				ResourceRequestHandlerFactory = resourceRequestHandler.SelfFactory
			};

			browser.LoadingStateChanged += browser_LoadingStateChanged;
			browser.FrameLoadEnd += browser_FrameLoadEnd;

			browser.BrowserSettings.BackgroundColor = (uint) BackColor.ToArgb();
			browser.Dock = DockStyle.None;
			browser.Location = ControlExtensions.InvisibleLocation;
			browser.SetupZoomEvents();

			Controls.Add(browser);
			Disposed += (sender, args) => browser.Dispose();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void Reload(string url) {
			nextUrl = url;
			browser.LoadingStateChanged += browser_LoadingStateChanged;
			browser.Dock = DockStyle.None;
			browser.Location = ControlExtensions.InvisibleLocation;
			browser.Load(DummyPage.Url);
		}

		private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			if (!e.IsLoading) {
				if (browser.Address == DummyPage.Url) {
					browser.Load(nextUrl);
				}
				else {
					this.InvokeAsyncSafe(() => {
						browser.Location = Point.Empty;
						browser.Dock = DockStyle.Fill;
					});

					browser.LoadingStateChanged -= browser_LoadingStateChanged;
				}
			}
		}

		private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			CefScriptExecutor.RunScript(e.Frame, "Array.prototype.forEach.call(document.getElementsByTagName('A'), ele => ele.addEventListener('click', e => { e.preventDefault(); window.open(ele.getAttribute('href')); }))", "gen:links");
		}
	}
}
