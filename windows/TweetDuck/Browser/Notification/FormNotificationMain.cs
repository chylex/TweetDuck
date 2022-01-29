using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Handling;
using TweetDuck.Controls;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;

namespace TweetDuck.Browser.Notification {
	abstract partial class FormNotificationMain : FormNotificationBase, CustomKeyboardHandler.IBrowserKeyHandler {
		protected sealed class NotificationInterfaceImpl : INotificationInterface {
			public bool FreezeTimer {
				get => notification.FreezeTimer;
				set => notification.FreezeTimer = value;
			}

			public bool IsHovered => notification.IsCursorOverBrowser;

			private readonly FormNotificationBase notification;

			public NotificationInterfaceImpl(FormNotificationBase notification) {
				this.notification = notification;
			}

			public void DisplayTooltip(string text) {
				notification.InvokeAsyncSafe(() => notification.DisplayTooltip(text));
			}

			public void FinishCurrentNotification() {
				notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
			}

			public void ShowTweetDetail() {
				notification.InvokeAsyncSafe(notification.ShowTweetDetail);
			}
		}

		private static int FontSizeLevel {
			get => NotificationBrowser.FontSize switch {
				"largest"  => 4,
				"large"    => 3,
				"small"    => 1,
				"smallest" => 0,
				_          => 2
			};
		}

		private readonly int timerBarHeight;

		protected int timeLeft, totalTime;
		protected bool pausedDuringNotification;

		private readonly NativeMethods.HookProc mouseHookDelegate;
		private IntPtr mouseHook;
		private bool blockXButtonUp;

		private int currentOpacity;

		private bool? prevDisplayTimer;
		private int? prevFontSize;

		public virtual bool RequiresResize {
			get {
				return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Config.DisplayNotificationTimer || prevFontSize != FontSizeLevel;
			}

			set {
				if (value) {
					prevDisplayTimer = null;
					prevFontSize = null;
				}
				else {
					prevDisplayTimer = Config.DisplayNotificationTimer;
					prevFontSize = FontSizeLevel;
				}
			}
		}

		private int BaseClientWidth {
			get => Config.NotificationSize switch {
				DesktopNotification.Size.Custom => Config.CustomNotificationSize.Width,
				_                               => BrowserUtils.Scale(284, SizeScale * (1.0 + 0.05 * FontSizeLevel))
			};
		}

		private int BaseClientHeight {
			get => Config.NotificationSize switch {
				DesktopNotification.Size.Custom => Config.CustomNotificationSize.Height,
				_                               => BrowserUtils.Scale(122, SizeScale * (1.0 + 0.08 * FontSizeLevel))
			};
		}

		public Size BrowserSize => Config.DisplayNotificationTimer ? new Size(ClientSize.Width, ClientSize.Height - timerBarHeight) : ClientSize;

		protected FormNotificationMain(FormBrowser owner, CreateBrowserImplFunc createBrowserImpl) : base(owner, createBrowserImpl) {
			InitializeComponent();

			this.timerBarHeight = BrowserUtils.Scale(4, DpiScale);

			browser.KeyboardHandler = new CustomKeyboardHandler(this);

			browser.LoadingStateChanged += Browser_LoadingStateChanged;

			mouseHookDelegate = MouseHookProc;
			Disposed += (sender, args) => StopMouseHook(true);
		}

		private void SetOpacity(int opacity) {
			if (currentOpacity != opacity) {
				currentOpacity = opacity;
				Opacity = opacity / 100.0;
			}
		}

		// mouse wheel hook

		private void StartMouseHook() {
			if (mouseHook == IntPtr.Zero) {
				mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WM_MOUSE_LL, mouseHookDelegate, IntPtr.Zero, 0);
			}
		}

		private void StopMouseHook(bool force) {
			if (mouseHook != IntPtr.Zero && (force || !blockXButtonUp)) {
				NativeMethods.UnhookWindowsHookEx(mouseHook);
				mouseHook = IntPtr.Zero;
				blockXButtonUp = false;
			}
		}

		private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
			if (nCode == 0) {
				int eventType = wParam.ToInt32();

				if (eventType == NativeMethods.WM_MOUSEWHEEL && IsCursorOverBrowser) {
					int delta = BrowserUtils.Scale(NativeMethods.GetMouseHookData(lParam), Config.NotificationScrollSpeed * 0.01);

					if (Config.EnableSmoothScrolling) {
						browser.BrowserCore.ExecuteScriptAsync("window.TDGF_scrollSmoothly", (int) Math.Round(-delta / 0.6));
					}
					else {
						browser.SendMouseWheelEvent(0, 0, 0, delta, CefEventFlags.None);
					}

					return NativeMethods.HOOK_HANDLED;
				}
				else if (eventType == NativeMethods.WM_XBUTTONDOWN && DesktopBounds.Contains(Cursor.Position)) {
					int extraButton = NativeMethods.GetMouseHookData(lParam);

					if (extraButton == 2) { // forward button
						this.InvokeAsyncSafe(FinishCurrentNotification);
					}
					else if (extraButton == 1) { // back button
						this.InvokeAsyncSafe(Close);
					}

					blockXButtonUp = true;
					return NativeMethods.HOOK_HANDLED;
				}
				else if (eventType == NativeMethods.WM_XBUTTONUP && blockXButtonUp) {
					blockXButtonUp = false;

					if (!Visible) {
						StopMouseHook(false);
					}

					return NativeMethods.HOOK_HANDLED;
				}
			}

			return NativeMethods.CallNextHookEx(mouseHook, nCode, wParam, lParam);
		}

		// event handlers

		private void FormNotification_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				HideNotification();
				e.Cancel = true;
			}
		}

		private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e) {
			if (!e.IsLoading && browser.Address != NotificationBrowser.BlankURL) {
				this.InvokeSafe(() => {
					Visible = true; // ensures repaint before moving the window to a visible location
					timerDisplayDelay.Start();
				});
			}
		}

		private void timerDisplayDelay_Tick(object sender, EventArgs e) {
			OnNotificationReady();
			timerDisplayDelay.Stop();
		}

		private void timerHideProgress_Tick(object sender, EventArgs e) {
			bool isCursorInside = Bounds.Contains(Cursor.Position);

			if (isCursorInside) {
				StartMouseHook();
				SetOpacity(100);
			}
			else {
				StopMouseHook(false);
				SetOpacity(Config.NotificationWindowOpacity);
			}

			if (isCursorInside || FreezeTimer || ContextMenuOpen) {
				return;
			}

			timeLeft -= timerProgress.Interval;

			int value = BrowserUtils.Scale(progressBarTimer.Maximum + 25, (totalTime - timeLeft) / (double) totalTime);
			progressBarTimer.SetValueInstant(Config.NotificationTimerCountDown ? progressBarTimer.Maximum - value : value);

			if (timeLeft <= 0) {
				FinishCurrentNotification();
			}
		}

		// notification methods

		public virtual void ShowNotification(DesktopNotification notification) {
			LoadTweet(notification);
		}

		public override void HideNotification() {
			base.HideNotification();

			progressBarTimer.Value = Config.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;
			timerProgress.Stop();
			totalTime = 0;

			StopMouseHook(false);
		}

		public override void FinishCurrentNotification() {
			timerProgress.Stop();
		}

		public override void PauseNotification() {
			if (!IsPaused) {
				pausedDuringNotification = IsNotificationVisible;
				timerProgress.Stop();
				StopMouseHook(true);
			}

			base.PauseNotification();
		}

		public override void ResumeNotification() {
			bool wasPaused = IsPaused;
			base.ResumeNotification();

			if (wasPaused && !IsPaused && pausedDuringNotification) {
				OnNotificationReady();
			}
		}

		protected override void LoadTweet(DesktopNotification tweet) {
			timerProgress.Stop();
			totalTime = timeLeft = tweet.GetDisplayDuration(Config.NotificationDurationValue);
			progressBarTimer.Value = Config.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;

			base.LoadTweet(tweet);
		}

		protected override void SetNotificationSize(int width, int height) {
			if (Config.DisplayNotificationTimer) {
				ClientSize = new Size(width, height + timerBarHeight);
				progressBarTimer.Visible = true;
			}
			else {
				ClientSize = new Size(width, height);
				progressBarTimer.Visible = false;
			}

			browser.ClientSize = new Size(width, height);
		}

		protected void PrepareAndDisplayWindow() {
			if (RequiresResize) {
				RequiresResize = false;
				SetNotificationSize(BaseClientWidth, BaseClientHeight);
			}

			SetOpacity(IsCursorOverBrowser ? 100 : Config.NotificationWindowOpacity);
			MoveToVisibleLocation();
		}

		protected virtual void OnNotificationReady() {
			PrepareAndDisplayWindow();
			timerProgress.Start();
		}

		bool CustomKeyboardHandler.IBrowserKeyHandler.HandleBrowserKey(Keys key) {
			switch (key) {
				case Keys.Enter:
					this.InvokeAsyncSafe(FinishCurrentNotification);
					return true;

				case Keys.Escape:
					this.InvokeAsyncSafe(HideNotification);
					return true;

				case Keys.Space:
					this.InvokeAsyncSafe(() => FreezeTimer = !FreezeTimer);
					return true;

				default:
					return false;
			}
		}
	}
}
