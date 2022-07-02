using System;
using System.Windows.Forms;
using TweetDuck.Browser.Notification;
using TweetDuck.Controls;
using TweetLib.Core.Features.Notifications;

namespace TweetDuck.Dialogs.Settings {
	sealed partial class TabSettingsNotifications : FormSettings.BaseTab {
		private static readonly int[] IdlePauseSeconds = { 0, 30, 60, 120, 300 };

		private readonly FormNotificationExample notification;

		public TabSettingsNotifications(FormNotificationExample notification) {
			InitializeComponent();

			this.notification = notification;

			this.notification.Ready += (_, _) => {
				this.InvokeAsyncSafe(() => {
					this.notification.ShowExampleNotification(true);
					this.notification.Move += notification_Move;
					this.notification.ResizeEnd += notification_ResizeEnd;
				});
			};

			this.notification.Show();

			Disposed += (_, _) => this.notification.Dispose();

			// general

			toolTip.SetToolTip(checkColumnName, "Shows column name each notification originated\r\nfrom in the notification window title.");
			toolTip.SetToolTip(checkMediaPreviews, "Shows image and video thumbnails in the notification window.");
			toolTip.SetToolTip(checkSkipOnLinkClick, "Skips current notification when a link\r\ninside the notification is clicked.");
			toolTip.SetToolTip(checkNonIntrusive, "When not idle and the cursor is within the notification window area,\r\nit will be delayed until the cursor moves away to prevent accidental clicks.");
			toolTip.SetToolTip(comboBoxIdlePause, "Pauses new notifications after going idle for a set amount of time.");

			checkColumnName.Checked = Config.DisplayNotificationColumn;
			checkMediaPreviews.Checked = Config.NotificationMediaPreviews;
			checkSkipOnLinkClick.Checked = Config.NotificationSkipOnLinkClick;
			checkNonIntrusive.Checked = Config.NotificationNonIntrusiveMode;

			comboBoxIdlePause.Items.Add("Disabled");
			comboBoxIdlePause.Items.Add("30 seconds");
			comboBoxIdlePause.Items.Add("1 minute");
			comboBoxIdlePause.Items.Add("2 minutes");
			comboBoxIdlePause.Items.Add("5 minutes");
			comboBoxIdlePause.SelectedIndex = Math.Max(0, Array.FindIndex(IdlePauseSeconds, static val => val == Config.NotificationIdlePauseSeconds));

			trackBarOpacity.SetValueSafe(Config.NotificationWindowOpacity);
			labelOpacityValue.Text = Config.NotificationWindowOpacity + "%";

			// timer

			toolTip.SetToolTip(checkTimerCountDown, "The notification timer counts down instead of up.");
			toolTip.SetToolTip(labelDurationValue, "Milliseconds per character.");
			toolTip.SetToolTip(trackBarDuration, toolTip.GetToolTip(labelDurationValue));

			checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
			checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
			checkTimerCountDown.Checked = Config.NotificationTimerCountDown;

			trackBarDuration.SetValueSafe(Config.NotificationDurationValue);
			labelDurationValue.Text = Config.NotificationDurationValue + " ms/c";

			// location

			toolTip.SetToolTip(radioLocCustom, "Drag the example notification window to the desired location.");

			switch (Config.NotificationPosition) {
				case DesktopNotification.Position.TopLeft:
					radioLocTL.Checked = true;
					break;
				case DesktopNotification.Position.TopRight:
					radioLocTR.Checked = true;
					break;
				case DesktopNotification.Position.BottomLeft:
					radioLocBL.Checked = true;
					break;
				case DesktopNotification.Position.BottomRight:
					radioLocBR.Checked = true;
					break;
				case DesktopNotification.Position.Custom:
					radioLocCustom.Checked = true;
					break;
			}

			comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = !radioLocCustom.Checked;
			comboBoxDisplay.Items.Add("(Same as TweetDuck)");

			foreach (Screen screen in Screen.AllScreens) {
				comboBoxDisplay.Items.Add($"{screen.DeviceName.TrimStart('\\', '.')} ({screen.Bounds.Width}x{screen.Bounds.Height})");
			}

			comboBoxDisplay.SelectedIndex = Math.Min(comboBoxDisplay.Items.Count - 1, Config.NotificationDisplay);

			trackBarEdgeDistance.SetValueSafe(Config.NotificationEdgeDistance);
			labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value + " px";

			// size

			toolTip.SetToolTip(radioSizeAuto, "Notification size is based on the font size and browser zoom level.");
			toolTip.SetToolTip(radioSizeCustom, "Resize the example notification window to the desired size.");

			switch (Config.NotificationSize) {
				case DesktopNotification.Size.Auto:
					radioSizeAuto.Checked = true;
					break;
				case DesktopNotification.Size.Custom:
					radioSizeCustom.Checked = true;
					break;
			}

			trackBarScrollSpeed.SetValueSafe(Config.NotificationScrollSpeed);
			labelScrollSpeedValue.Text = trackBarScrollSpeed.Value + "%";
		}

		public override void OnReady() {
			checkColumnName.CheckedChanged += checkColumnName_CheckedChanged;
			checkMediaPreviews.CheckedChanged += checkMediaPreviews_CheckedChanged;
			checkSkipOnLinkClick.CheckedChanged += checkSkipOnLinkClick_CheckedChanged;
			checkNonIntrusive.CheckedChanged += checkNonIntrusive_CheckedChanged;
			comboBoxIdlePause.SelectedValueChanged += comboBoxIdlePause_SelectedValueChanged;
			trackBarOpacity.ValueChanged += trackBarOpacity_ValueChanged;

			checkNotificationTimer.CheckedChanged += checkNotificationTimer_CheckedChanged;
			checkTimerCountDown.CheckedChanged += checkTimerCountDown_CheckedChanged;
			trackBarDuration.ValueChanged += trackBarDuration_ValueChanged;
			btnDurationShort.Click += btnDurationShort_Click;
			btnDurationMedium.Click += btnDurationMedium_Click;
			btnDurationLong.Click += btnDurationLong_Click;

			radioLocTL.CheckedChanged += radioLoc_CheckedChanged;
			radioLocTR.CheckedChanged += radioLoc_CheckedChanged;
			radioLocBL.CheckedChanged += radioLoc_CheckedChanged;
			radioLocBR.CheckedChanged += radioLoc_CheckedChanged;
			radioLocCustom.Click += radioLocCustom_Click;
			comboBoxDisplay.SelectedValueChanged += comboBoxDisplay_SelectedValueChanged;
			trackBarEdgeDistance.ValueChanged += trackBarEdgeDistance_ValueChanged;

			radioSizeAuto.CheckedChanged += radioSize_CheckedChanged;
			radioSizeCustom.Click += radioSizeCustom_Click;
			trackBarScrollSpeed.ValueChanged += trackBarScrollSpeed_ValueChanged;
		}

		private void TabSettingsNotifications_ParentChanged(object? sender, EventArgs e) {
			if (Parent == null) {
				notification.HideNotification();
			}
			else {
				notification.ShowExampleNotification(true);
			}
		}

		private void notification_Move(object? sender, EventArgs e) {
			if (radioLocCustom.Checked && notification.Location != ControlExtensions.InvisibleLocation) {
				Config.CustomNotificationPosition = notification.Location;
			}
		}

		private void notification_ResizeEnd(object? sender, EventArgs e) {
			if (radioSizeCustom.Checked) {
				Config.CustomNotificationSize = notification.BrowserSize;
				notification.ShowExampleNotification(false);
			}
		}

		#region General

		private void checkColumnName_CheckedChanged(object? sender, EventArgs e) {
			Config.DisplayNotificationColumn = checkColumnName.Checked;
			notification.ShowExampleNotification(false);
		}

		private void checkMediaPreviews_CheckedChanged(object? sender, EventArgs e) {
			Config.NotificationMediaPreviews = checkMediaPreviews.Checked;
		}

		private void checkSkipOnLinkClick_CheckedChanged(object? sender, EventArgs e) {
			Config.NotificationSkipOnLinkClick = checkSkipOnLinkClick.Checked;
		}

		private void checkNonIntrusive_CheckedChanged(object? sender, EventArgs e) {
			Config.NotificationNonIntrusiveMode = checkNonIntrusive.Checked;
		}

		private void comboBoxIdlePause_SelectedValueChanged(object? sender, EventArgs e) {
			Config.NotificationIdlePauseSeconds = IdlePauseSeconds[comboBoxIdlePause.SelectedIndex];
		}

		private void trackBarOpacity_ValueChanged(object? sender, EventArgs e) {
			if (trackBarOpacity.AlignValueToTick()) {
				Config.NotificationWindowOpacity = trackBarOpacity.Value;
				labelOpacityValue.Text = Config.NotificationWindowOpacity + "%";
			}
		}

		#endregion

		#region Timer

		private void checkNotificationTimer_CheckedChanged(object? sender, EventArgs e) {
			Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
			checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
			notification.ShowExampleNotification(true);
		}

		private void checkTimerCountDown_CheckedChanged(object? sender, EventArgs e) {
			Config.NotificationTimerCountDown = checkTimerCountDown.Checked;
			notification.ShowExampleNotification(true);
		}

		private void trackBarDuration_ValueChanged(object? sender, EventArgs e) {
			durationUpdateTimer.Stop();
			durationUpdateTimer.Start();

			Config.NotificationDurationValue = trackBarDuration.Value;
			labelDurationValue.Text = Config.NotificationDurationValue + " ms/c";
		}

		private void btnDurationShort_Click(object? sender, EventArgs e) {
			trackBarDuration.Value = 15;
		}

		private void btnDurationMedium_Click(object? sender, EventArgs e) {
			trackBarDuration.Value = 25;
		}

		private void btnDurationLong_Click(object? sender, EventArgs e) {
			trackBarDuration.Value = 35;
		}

		private void durationUpdateTimer_Tick(object? sender, EventArgs e) {
			notification.ShowExampleNotification(true);
			durationUpdateTimer.Stop();
		}

		#endregion

		#region Location

		private void radioLoc_CheckedChanged(object? sender, EventArgs e) {
			if (radioLocTL.Checked) {
				Config.NotificationPosition = DesktopNotification.Position.TopLeft;
			}
			else if (radioLocTR.Checked) {
				Config.NotificationPosition = DesktopNotification.Position.TopRight;
			}
			else if (radioLocBL.Checked) {
				Config.NotificationPosition = DesktopNotification.Position.BottomLeft;
			}
			else if (radioLocBR.Checked) {
				Config.NotificationPosition = DesktopNotification.Position.BottomRight;
			}

			comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = true;
			notification.ShowExampleNotification(false);
		}

		private void radioLocCustom_Click(object? sender, EventArgs e) {
			if (!Config.IsCustomNotificationPositionSet) {
				Config.CustomNotificationPosition = notification.Location;
			}

			Config.NotificationPosition = DesktopNotification.Position.Custom;

			comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = false;
			notification.ShowExampleNotification(false);

			if (notification.IsFullyOutsideView() && FormMessage.Question("Notification is Outside View", "The notification seems to be outside of view, would you like to reset its position?", FormMessage.Yes, FormMessage.No)) {
				Config.NotificationPosition = DesktopNotification.Position.TopRight;
				notification.MoveToVisibleLocation();

				Config.CustomNotificationPosition = notification.Location;

				Config.NotificationPosition = DesktopNotification.Position.Custom;
				notification.MoveToVisibleLocation();
			}
		}

		private void comboBoxDisplay_SelectedValueChanged(object? sender, EventArgs e) {
			Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
			notification.ShowExampleNotification(false);
		}

		private void trackBarEdgeDistance_ValueChanged(object? sender, EventArgs e) {
			labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value + " px";
			Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
			notification.ShowExampleNotification(false);
		}

		#endregion

		#region Size

		private void radioSize_CheckedChanged(object? sender, EventArgs e) {
			if (radioSizeAuto.Checked) {
				Config.NotificationSize = DesktopNotification.Size.Auto;
			}

			notification.ShowExampleNotification(false);
		}

		private void radioSizeCustom_Click(object? sender, EventArgs e) {
			if (!Config.IsCustomNotificationSizeSet) {
				Config.CustomNotificationSize = notification.BrowserSize;
			}

			Config.NotificationSize = DesktopNotification.Size.Custom;
			notification.ShowExampleNotification(false);
		}

		private void trackBarScrollSpeed_ValueChanged(object? sender, EventArgs e) {
			if (trackBarScrollSpeed.AlignValueToTick()) {
				labelScrollSpeedValue.Text = trackBarScrollSpeed.Value + "%";
				Config.NotificationScrollSpeed = trackBarScrollSpeed.Value;
			}
		}

		#endregion
	}
}
