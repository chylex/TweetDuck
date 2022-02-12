using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Utils;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;

namespace TweetDuck.Browser.Notification {
	sealed partial class FormNotificationTweet : FormNotificationMain {
		private static NotificationBrowser CreateBrowserImpl(IBrowserComponent browserComponent, INotificationInterface notificationInterface, ITweetDeckInterface tweetDeckInterface, PluginManager pluginManager) {
			return new NotificationBrowser.Tweet(browserComponent, notificationInterface, tweetDeckInterface, pluginManager);
		}

		private const int NonIntrusiveIdleLimit = 30;
		private const int TrimMinimum = 32;

		protected override Point PrimaryLocation => hasTemporarilyMoved && IsNotificationVisible ? Location : base.PrimaryLocation;
		private bool IsCursorOverNotificationArea => new Rectangle(PrimaryLocation, Size).Contains(Cursor.Position);

		protected override bool CanDragWindow {
			get {
				if (ModifierKeys.HasFlag(Keys.Alt)) {
					hasTemporarilyMoved = true;
					return true;
				}
				else {
					return false;
				}
			}
		}

		private readonly Queue<DesktopNotification> tweetQueue = new Queue<DesktopNotification>(4);
		private bool needsTrim;
		private bool hasTemporarilyMoved;

		public FormNotificationTweet(FormBrowser owner, ITweetDeckInterface tweetDeckInterface, PluginManager pluginManager) : base(owner, (form, browserComponent) => CreateBrowserImpl(browserComponent, new NotificationInterfaceImpl(form), tweetDeckInterface, pluginManager)) {
			InitializeComponent();

			Config.MuteToggled += Config_MuteToggled;
			Disposed += (sender, args) => Config.MuteToggled -= Config_MuteToggled;

			if (Config.MuteNotifications) {
				PauseNotification(NotificationPauseReason.UserConfiguration);
			}
		}

		protected override void WndProc(ref Message m) {
			if (m.Msg == 0x00A7) { // WM_NCMBUTTONDOWN
				int hitTest = m.WParam.ToInt32();

				if (hitTest == 2 || hitTest == 20) { // HTCAPTION, HTCLOSE
					hasTemporarilyMoved = false;
					MoveToVisibleLocation();
					return;
				}
			}

			base.WndProc(ref m);
		}

		// event handlers

		private void Config_MuteToggled(object sender, EventArgs e) {
			if (Config.MuteNotifications) {
				PauseNotification(NotificationPauseReason.UserConfiguration);
			}
			else {
				ResumeNotification(NotificationPauseReason.UserConfiguration);
			}
		}

		private void timerCursorCheck_Tick(object sender, EventArgs e) {
			if (!IsCursorOverNotificationArea) {
				ResumeNotification(NotificationPauseReason.CursorOverNotificationArea);
				timerCursorCheck.Stop();
			}
		}

		private void timerIdlePauseCheck_Tick(object sender, EventArgs e) {
			if (NativeMethods.GetIdleSeconds() < Config.NotificationIdlePauseSeconds) {
				ResumeNotification(NotificationPauseReason.SystemIdle);
				timerIdlePauseCheck.Stop();
			}
		}

		// notification methods

		public override void ShowNotification(DesktopNotification notification) {
			tweetQueue.Enqueue(notification);

			if (!IsPaused) {
				UpdateTitle();

				if (totalTime == 0) {
					LoadNextNotification();
				}
			}

			needsTrim |= tweetQueue.Count >= TrimMinimum;
		}

		public override void HideNotification() {
			base.HideNotification();
			tweetQueue.Clear();

			if (needsTrim) {
				tweetQueue.TrimExcess();
				needsTrim = false;
			}

			hasTemporarilyMoved = false;
		}

		public override void FinishCurrentNotification() {
			if (tweetQueue.Count > 0) {
				LoadNextNotification();
			}
			else {
				HideNotification();
			}
		}

		public override void ResumeNotification(NotificationPauseReason reason) {
			bool wasPaused = IsPaused;
			base.ResumeNotification(reason);

			if (wasPaused && !IsPaused && !pausedDuringNotification && tweetQueue.Count > 0) {
				LoadNextNotification();
			}
		}

		private void LoadNextNotification() {
			if (!IsNotificationVisible) {
				if (Config.NotificationNonIntrusiveMode && IsCursorOverNotificationArea && NativeMethods.GetIdleSeconds() < NonIntrusiveIdleLimit) {
					if (!timerCursorCheck.Enabled) {
						PauseNotification(NotificationPauseReason.CursorOverNotificationArea);
						timerCursorCheck.Start();
					}

					return;
				}
				else if (Config.NotificationIdlePauseSeconds > 0 && NativeMethods.GetIdleSeconds() >= Config.NotificationIdlePauseSeconds) {
					if (!timerIdlePauseCheck.Enabled) {
						PauseNotification(NotificationPauseReason.SystemIdle);
						timerIdlePauseCheck.Start();
					}

					return;
				}
			}

			LoadTweet(tweetQueue.Dequeue());
		}

		protected override void UpdateTitle() {
			base.UpdateTitle();

			if (tweetQueue.Count > 0) {
				Text = Text + " (" + tweetQueue.Count + " more left)";
			}
		}

		protected override void OnNotificationReady() {
			UpdateTitle();
			base.OnNotificationReady();
		}
	}
}
