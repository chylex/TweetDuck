using System;
using System.Globalization;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;

namespace TweetDuck.Core.Other.Settings{
    partial class TabSettingsNotifications : BaseTabSettings{
        private static readonly int[] IdlePauseSeconds = { 0, 30, 60, 120, 300 };

        private readonly FormNotificationMain notification;

        public TabSettingsNotifications(FormNotificationMain notification){
            InitializeComponent();

            this.notification = notification;
            this.notification.CanMoveWindow = () => radioLocCustom.Checked;

            this.notification.Move += (sender, args) => {
                if (radioLocCustom.Checked && this.notification.Location != ControlExtensions.InvisibleLocation){
                    Config.CustomNotificationPosition = this.notification.Location;
                }
            };
            
            this.notification.Initialized += (sender, args) => {
                this.InvokeAsyncSafe(() => this.notification.ShowNotificationForSettings(true));
            };

            this.notification.Activated += notification_Activated;
            this.notification.Show();

            switch(Config.NotificationPosition){
                case TweetNotification.Position.TopLeft: radioLocTL.Checked = true; break;
                case TweetNotification.Position.TopRight: radioLocTR.Checked = true; break;
                case TweetNotification.Position.BottomLeft: radioLocBL.Checked = true; break;
                case TweetNotification.Position.BottomRight: radioLocBR.Checked = true; break;
                case TweetNotification.Position.Custom: radioLocCustom.Checked = true; break;
            }

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = !radioLocCustom.Checked;
            
            toolTip.SetToolTip(trackBarDuration, toolTip.GetToolTip(labelDurationValue));
            trackBarDuration.SetValueSafe(Config.NotificationDurationValue);
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";

            comboBoxIdlePause.Items.Add("Disabled");
            comboBoxIdlePause.Items.Add("30 seconds");
            comboBoxIdlePause.Items.Add("1 minute");
            comboBoxIdlePause.Items.Add("2 minutes");
            comboBoxIdlePause.Items.Add("5 minutes");
            comboBoxIdlePause.SelectedIndex = Math.Max(0, Array.FindIndex(IdlePauseSeconds, val => val == Config.NotificationIdlePauseSeconds));

            comboBoxDisplay.Items.Add("(Same As "+Program.BrandName+")");

            foreach(Screen screen in Screen.AllScreens){
                comboBoxDisplay.Items.Add(screen.DeviceName.TrimStart('\\', '.')+" ("+screen.Bounds.Width+"x"+screen.Bounds.Height+")");
            }

            comboBoxDisplay.SelectedIndex = Math.Min(comboBoxDisplay.Items.Count-1, Config.NotificationDisplay);
            
            checkColumnName.Checked = Config.DisplayNotificationColumn;
            checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            checkTimerCountDown.Checked = Config.NotificationTimerCountDown;
            checkSkipOnLinkClick.Checked = Config.NotificationSkipOnLinkClick;
            checkNonIntrusive.Checked = Config.NotificationNonIntrusiveMode;

            trackBarScrollSpeed.SetValueSafe(Config.NotificationScrollSpeed);
            labelScrollSpeedValue.Text = trackBarScrollSpeed.Value.ToString(CultureInfo.InvariantCulture)+"%";

            trackBarEdgeDistance.SetValueSafe(Config.NotificationEdgeDistance);
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value.ToString(CultureInfo.InvariantCulture)+" px";

            Disposed += (sender, args) => this.notification.Dispose();
        }

        public override void OnReady(){
            radioLocTL.CheckedChanged += radioLoc_CheckedChanged;
            radioLocTR.CheckedChanged += radioLoc_CheckedChanged;
            radioLocBL.CheckedChanged += radioLoc_CheckedChanged;
            radioLocBR.CheckedChanged += radioLoc_CheckedChanged;
            radioLocCustom.Click += radioLocCustom_Click;

            trackBarDuration.ValueChanged += trackBarDuration_ValueChanged;
            btnDurationShort.Click += btnDurationShort_Click;
            btnDurationMedium.Click += btnDurationMedium_Click;
            btnDurationLong.Click += btnDurationLong_Click;

            checkColumnName.CheckedChanged += checkColumnName_CheckedChanged;
            checkNotificationTimer.CheckedChanged += checkNotificationTimer_CheckedChanged;
            checkTimerCountDown.CheckedChanged += checkTimerCountDown_CheckedChanged;
            checkSkipOnLinkClick.CheckedChanged += checkSkipOnLinkClick_CheckedChanged;
            checkNonIntrusive.CheckedChanged += checkNonIntrusive_CheckedChanged;

            comboBoxIdlePause.SelectedValueChanged += comboBoxIdlePause_SelectedValueChanged;
            trackBarScrollSpeed.ValueChanged += trackBarScrollSpeed_ValueChanged;

            comboBoxDisplay.SelectedValueChanged += comboBoxDisplay_SelectedValueChanged;
            trackBarEdgeDistance.ValueChanged += trackBarEdgeDistance_ValueChanged;
        }

        private void TabSettingsNotifications_ParentChanged(object sender, EventArgs e){
            if (Parent == null){
                notification.HideNotification(false);
            }
            else{
                notification.ShowNotificationForSettings(true);
            }
        }

        private void notification_Activated(object sender, EventArgs e){
            notification.Hide();
            notification.Activated -= notification_Activated;
        }

        private void radioLoc_CheckedChanged(object sender, EventArgs e){
            if (radioLocTL.Checked)Config.NotificationPosition = TweetNotification.Position.TopLeft;
            else if (radioLocTR.Checked)Config.NotificationPosition = TweetNotification.Position.TopRight;
            else if (radioLocBL.Checked)Config.NotificationPosition = TweetNotification.Position.BottomLeft;
            else if (radioLocBR.Checked)Config.NotificationPosition = TweetNotification.Position.BottomRight;

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = true;
            notification.ShowNotificationForSettings(false);
        }

        private void radioLocCustom_Click(object sender, EventArgs e){
            if (!Config.IsCustomNotificationPositionSet){
                Config.CustomNotificationPosition = notification.Location;
            }

            Config.NotificationPosition = TweetNotification.Position.Custom;

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = false;
            notification.ShowNotificationForSettings(false);

            if (notification.IsFullyOutsideView() && MessageBox.Show("The notification seems to be outside of view, would you like to reset its position?", "Notification is outside view", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                Config.NotificationPosition = TweetNotification.Position.TopRight;
                notification.MoveToVisibleLocation();

                Config.CustomNotificationPosition = notification.Location;

                Config.NotificationPosition = TweetNotification.Position.Custom;
                notification.MoveToVisibleLocation();
            }
        }

        private void trackBarDuration_ValueChanged(object sender, EventArgs e){
            Config.NotificationDurationValue = trackBarDuration.Value;
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";

            notification.ShowNotificationForSettings(true);
        }

        private void btnDurationShort_Click(object sender, EventArgs e){
            trackBarDuration.Value = 15;
        }

        private void btnDurationMedium_Click(object sender, EventArgs e){
            trackBarDuration.Value = 25;
        }

        private void btnDurationLong_Click(object sender, EventArgs e){
            trackBarDuration.Value = 35;
        }

        private void checkColumnName_CheckedChanged(object sender, EventArgs e){
            Config.DisplayNotificationColumn = checkColumnName.Checked;
            notification.ShowNotificationForSettings(false);
        }

        private void checkNotificationTimer_CheckedChanged(object sender, EventArgs e){
            Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void checkTimerCountDown_CheckedChanged(object sender, EventArgs e){
            Config.NotificationTimerCountDown = checkTimerCountDown.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void checkSkipOnLinkClick_CheckedChanged(object sender, EventArgs e){
            Config.NotificationSkipOnLinkClick = checkSkipOnLinkClick.Checked;
        }

        private void checkNonIntrusive_CheckedChanged(object sender, EventArgs e){
            Config.NotificationNonIntrusiveMode = checkNonIntrusive.Checked;
        }

        private void comboBoxIdlePause_SelectedValueChanged(object sender, EventArgs e){
            Config.NotificationIdlePauseSeconds = IdlePauseSeconds[comboBoxIdlePause.SelectedIndex];
        }

        private void trackBarScrollSpeed_ValueChanged(object sender, EventArgs e){
            if (trackBarScrollSpeed.AlignValueToTick()){
                labelScrollSpeedValue.Text = trackBarScrollSpeed.Value.ToString(CultureInfo.InvariantCulture)+"%";
                Config.NotificationScrollSpeed = trackBarScrollSpeed.Value;
            }
        }

        private void comboBoxDisplay_SelectedValueChanged(object sender, EventArgs e){
            Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
            notification.ShowNotificationForSettings(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value.ToString(CultureInfo.InvariantCulture)+" px";
            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowNotificationForSettings(false);
        }
    }
}
