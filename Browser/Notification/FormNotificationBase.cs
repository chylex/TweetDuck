using System.Drawing;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Browser.Data;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Handling.General;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Notification {
	abstract partial class FormNotificationBase : Form {
		public static readonly ResourceLink AppLogo = new ResourceLink("https://ton.twimg.com/tduck/avatar", ResourceHandlers.ForBytes(Properties.Resources.avatar, "image/png"));

		protected const string BlankURL = TwitterUrls.TweetDeck + "/?blank";

		public static string FontSize = null;
		public static string HeadLayout = null;

		protected static UserConfig Config => Program.Config.User;

		protected static int FontSizeLevel {
			get => FontSize switch {
				"largest"  => 4,
				"large"    => 3,
				"small"    => 1,
				"smallest" => 0,
				_          => 2
			};
		}

		protected virtual Point PrimaryLocation {
			get {
				Screen screen;

				if (Config.NotificationDisplay > 0 && Config.NotificationDisplay <= Screen.AllScreens.Length) {
					screen = Screen.AllScreens[Config.NotificationDisplay - 1];
				}
				else {
					screen = Screen.FromControl(owner);
				}

				int edgeDist = Config.NotificationEdgeDistance;

				switch (Config.NotificationPosition) {
					case DesktopNotification.Position.TopLeft:
						return new Point(screen.WorkingArea.X + edgeDist, screen.WorkingArea.Y + edgeDist);

					case DesktopNotification.Position.TopRight:
						return new Point(screen.WorkingArea.X + screen.WorkingArea.Width - edgeDist - Width, screen.WorkingArea.Y + edgeDist);

					case DesktopNotification.Position.BottomLeft:
						return new Point(screen.WorkingArea.X + edgeDist, screen.WorkingArea.Y + screen.WorkingArea.Height - edgeDist - Height);

					case DesktopNotification.Position.BottomRight:
						return new Point(screen.WorkingArea.X + screen.WorkingArea.Width - edgeDist - Width, screen.WorkingArea.Y + screen.WorkingArea.Height - edgeDist - Height);

					case DesktopNotification.Position.Custom:
						if (!Config.IsCustomNotificationPositionSet) {
							Config.CustomNotificationPosition = new Point(screen.WorkingArea.X + screen.WorkingArea.Width - edgeDist - Width, screen.WorkingArea.Y + edgeDist);
							Config.Save();
						}

						return Config.CustomNotificationPosition;
				}

				return Location;
			}
		}

		protected bool IsNotificationVisible => Location != ControlExtensions.InvisibleLocation;
		protected virtual bool CanDragWindow => true;

		public new Point Location {
			get { return base.Location; }

			set {
				Visible = (base.Location = value) != ControlExtensions.InvisibleLocation;
				FormBorderStyle = NotificationBorderStyle;
			}
		}

		protected virtual FormBorderStyle NotificationBorderStyle {
			get {
				if (WindowsUtils.ShouldAvoidToolWindow && Visible) { // Visible = workaround for alt+tab
					return FormBorderStyle.FixedSingle;
				}
				else {
					return FormBorderStyle.FixedToolWindow;
				}
			}
		}

		protected override bool ShowWithoutActivation => true;

		protected float DpiScale { get; }
		protected double SizeScale => DpiScale * Config.ZoomLevel / 100.0;

		private readonly FormBrowser owner;

		#pragma warning disable IDE0069 // Disposable fields should be disposed
		protected readonly ChromiumWebBrowser browser;
		#pragma warning restore IDE0069 // Disposable fields should be disposed

		private readonly ResourceHandlerNotification resourceHandler = new ResourceHandlerNotification();

		private DesktopNotification currentNotification;
		private int pauseCounter;

		public string CurrentTweetUrl => currentNotification?.TweetUrl;
		public string CurrentQuoteUrl => currentNotification?.QuoteUrl;

		public bool CanViewDetail => currentNotification != null && !string.IsNullOrEmpty(currentNotification.ColumnId) && !string.IsNullOrEmpty(currentNotification.ChirpId);

		protected bool IsPaused => pauseCounter > 0;
		protected bool IsCursorOverBrowser => browser.Bounds.Contains(PointToClient(Cursor.Position));

		public bool FreezeTimer { get; set; }
		public bool ContextMenuOpen { get; set; }

		protected FormNotificationBase(FormBrowser owner, bool enableContextMenu) {
			InitializeComponent();

			this.owner = owner;
			this.owner.FormClosed += owner_FormClosed;

			var resourceRequestHandler = new ResourceRequestHandlerBase();
			var resourceHandlers = resourceRequestHandler.ResourceHandlers;

			resourceHandlers.Register(BlankURL, ResourceHandlers.ForString(string.Empty));
			resourceHandlers.Register(TwitterUrls.TweetDeck, () => this.resourceHandler);
			resourceHandlers.Register(AppLogo);

			this.browser = new ChromiumWebBrowser(BlankURL) {
				MenuHandler = new ContextMenuNotification(this, enableContextMenu),
				JsDialogHandler = new JavaScriptDialogHandler(),
				LifeSpanHandler = new CustomLifeSpanHandler(),
				RequestHandler = new RequestHandlerBase(false),
				ResourceRequestHandlerFactory = resourceRequestHandler.SelfFactory
			};

			this.browser.Dock = DockStyle.None;
			this.browser.ClientSize = ClientSize;
			this.browser.SetupZoomEvents();

			Controls.Add(browser);

			Disposed += (sender, args) => {
				this.owner.FormClosed -= owner_FormClosed;
				this.browser.Dispose();
			};

			DpiScale = this.GetDPIScale();

			// ReSharper disable once VirtualMemberCallInContructor
			UpdateTitle();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();
				resourceHandler.Dispose();
			}

			base.Dispose(disposing);
		}

		protected override void WndProc(ref Message m) {
			if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanDragWindow) { // WM_SYSCOMMAND, SC_MOVE
				return;
			}

			base.WndProc(ref m);
		}

		// event handlers

		private void owner_FormClosed(object sender, FormClosedEventArgs e) {
			Close();
		}

		// notification methods

		public virtual void HideNotification() {
			browser.Load(BlankURL);
			DisplayTooltip(null);

			Location = ControlExtensions.InvisibleLocation;
			currentNotification = null;
		}

		public virtual void FinishCurrentNotification() {}

		public virtual void PauseNotification() {
			if (pauseCounter++ == 0 && IsNotificationVisible) {
				Location = ControlExtensions.InvisibleLocation;
			}
		}

		public virtual void ResumeNotification() {
			if (pauseCounter > 0) {
				--pauseCounter;
			}
		}

		protected abstract string GetTweetHTML(DesktopNotification tweet);

		protected virtual void LoadTweet(DesktopNotification tweet) {
			currentNotification = tweet;
			resourceHandler.SetHTML(GetTweetHTML(tweet));

			browser.Load(TwitterUrls.TweetDeck);
			DisplayTooltip(null);
		}

		protected virtual void SetNotificationSize(int width, int height) {
			browser.ClientSize = ClientSize = new Size(BrowserUtils.Scale(width, SizeScale), BrowserUtils.Scale(height, SizeScale));
		}

		protected virtual void UpdateTitle() {
			string title = currentNotification?.ColumnTitle;
			Text = string.IsNullOrEmpty(title) || !Config.DisplayNotificationColumn ? Program.BrandName : $"{Program.BrandName} - {title}";
		}

		public void ShowTweetDetail() {
			if (currentNotification != null) {
				owner.ShowTweetDetail(currentNotification.ColumnId, currentNotification.ChirpId, currentNotification.TweetUrl);
			}
		}

		public void MoveToVisibleLocation() {
			bool needsReactivating = Location == ControlExtensions.InvisibleLocation;
			Location = PrimaryLocation;

			if (needsReactivating) {
				NativeMethods.SetFormPos(this, NativeMethods.HWND_TOPMOST, NativeMethods.SWP_NOACTIVATE);
			}
		}

		public void DisplayTooltip(string text) {
			if (string.IsNullOrEmpty(text)) {
				toolTip.Hide(this);
			}
			else {
				Point position = PointToClient(Cursor.Position);
				position.Offset(20, 5);
				toolTip.Show(text, this, position);
			}
		}
	}
}
