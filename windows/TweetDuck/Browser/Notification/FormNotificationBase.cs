using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Browser.Base;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Utils;
using TweetImpl.CefSharp.Handlers;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Browser.Notification {
	abstract partial class FormNotificationBase : Form {
		protected static UserConfig Config => Program.Config.User;

		protected delegate NotificationBrowser CreateBrowserImplFunc(FormNotificationBase form, IBrowserComponent browserComponent);

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
			get {
				return base.Location;
			}

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

		protected readonly ChromiumWebBrowser browser;
		protected readonly CefBrowserComponent browserComponent;
		private readonly NotificationBrowser browserImpl;

		private readonly CefByteArrayResourceHandler resourceHandler = new CefByteArrayResourceHandler();

		private DesktopNotification currentNotification;
		private readonly HashSet<NotificationPauseReason> pauseReasons = new HashSet<NotificationPauseReason>();

		public string CurrentTweetUrl => currentNotification?.TweetUrl;
		public string CurrentQuoteUrl => currentNotification?.QuoteUrl;

		protected bool IsPaused => pauseReasons.Count > 0;
		protected internal bool IsCursorOverBrowser => browser.Bounds.Contains(PointToClient(Cursor.Position));

		public bool FreezeTimer { get; set; }
		public bool ContextMenuOpen { get; set; }

		protected FormNotificationBase(FormBrowser owner, CreateBrowserImplFunc createBrowserImpl) {
			InitializeComponent();

			this.owner = owner;
			this.owner.FormClosed += owner_FormClosed;

			this.browser = new ChromiumWebBrowser(NotificationBrowser.BlankURL);
			this.browserComponent = new CefBrowserComponent(browser, handler => new ContextMenuNotification(this, handler), autoReload: false);
			this.browserComponent.ResourceHandlerRegistry.RegisterStatic(NotificationBrowser.BlankURL, string.Empty);
			this.browserComponent.ResourceHandlerRegistry.RegisterDynamic(TwitterUrls.TweetDeck, resourceHandler);
			this.browserImpl = createBrowserImpl(this, browserComponent);

			this.browser.Dock = DockStyle.None;
			this.browser.ClientSize = ClientSize;

			Controls.Add(browser);

			Disposed += (sender, args) => {
				this.owner.FormClosed -= owner_FormClosed;
				this.browserImpl.Dispose();
				this.browser.Dispose();
			};

			DpiScale = this.GetDPIScale();

			// ReSharper disable once VirtualMemberCallInContructor
			UpdateTitle();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();
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
			browser.Load(NotificationBrowser.BlankURL);
			DisplayTooltip(null);

			Location = ControlExtensions.InvisibleLocation;
			currentNotification = null;
		}

		public virtual void FinishCurrentNotification() {}

		public virtual void PauseNotification(NotificationPauseReason reason) {
			if (pauseReasons.Add(reason) && IsNotificationVisible) {
				Location = ControlExtensions.InvisibleLocation;
			}
		}

		public virtual void ResumeNotification(NotificationPauseReason reason) {
			pauseReasons.Remove(reason);
		}

		protected virtual void LoadTweet(DesktopNotification tweet) {
			currentNotification = tweet;
			resourceHandler.SetResource(new ByteArrayResource(browserImpl.GetTweetHTML(tweet), Encoding.UTF8));

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
			if (currentNotification != null && owner.ShowTweetDetail(currentNotification.ColumnId, currentNotification.ChirpId, currentNotification.TweetUrl)) {
				FinishCurrentNotification();
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
