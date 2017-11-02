﻿using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Notification.Example;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsNotifications : BaseTabSettings{
        private static readonly int[] IdlePauseSeconds = { 0, 30, 60, 120, 300 };

        private readonly FormNotificationExample notification;

        public TabSettingsNotifications(FormNotificationExample notification){
            InitializeComponent();

            this.notification = notification;
            
            this.notification.Initialized += (sender, args) => {
                this.InvokeAsyncSafe(() => {
                    this.notification.ShowExampleNotification(true);
                    this.notification.Move += notification_Move;
                    this.notification.ResizeEnd += notification_ResizeEnd;
                });
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

            switch(Config.NotificationSize){
                case TweetNotification.Size.Auto: radioSizeAuto.Checked = true; break;
                case TweetNotification.Size.Custom: radioSizeCustom.Checked = true; break;
            }
            
            toolTip.SetToolTip(trackBarDuration, toolTip.GetToolTip(labelDurationValue));
            trackBarDuration.SetValueSafe(Config.NotificationDurationValue);
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";

            comboBoxIdlePause.Items.Add("Disabled");
            comboBoxIdlePause.Items.Add("30 seconds");
            comboBoxIdlePause.Items.Add("1 minute");
            comboBoxIdlePause.Items.Add("2 minutes");
            comboBoxIdlePause.Items.Add("5 minutes");
            comboBoxIdlePause.SelectedIndex = Math.Max(0, Array.FindIndex(IdlePauseSeconds, val => val == Config.NotificationIdlePauseSeconds));

            comboBoxDisplay.Items.Add("(Same as TweetDuck)");

            foreach(Screen screen in Screen.AllScreens){
                comboBoxDisplay.Items.Add(screen.DeviceName.TrimStart('\\', '.')+" ("+screen.Bounds.Width+"x"+screen.Bounds.Height+")");
            }

            comboBoxDisplay.SelectedIndex = Math.Min(comboBoxDisplay.Items.Count-1, Config.NotificationDisplay);
            
            checkColumnName.Checked = Config.DisplayNotificationColumn;
            checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            checkTimerCountDown.Checked = Config.NotificationTimerCountDown;
            checkMediaPreviews.Checked = Config.NotificationMediaPreviews;
            checkSkipOnLinkClick.Checked = Config.NotificationSkipOnLinkClick;
            checkNonIntrusive.Checked = Config.NotificationNonIntrusiveMode;

            trackBarScrollSpeed.SetValueSafe(Config.NotificationScrollSpeed);
            labelScrollSpeedValue.Text = trackBarScrollSpeed.Value+"%";

            trackBarEdgeDistance.SetValueSafe(Config.NotificationEdgeDistance);
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value+" px";
            
            Disposed += (sender, args) => this.notification.Dispose();
        }

        public override void OnReady(){
            radioLocTL.CheckedChanged += radioLoc_CheckedChanged;
            radioLocTR.CheckedChanged += radioLoc_CheckedChanged;
            radioLocBL.CheckedChanged += radioLoc_CheckedChanged;
            radioLocBR.CheckedChanged += radioLoc_CheckedChanged;
            radioLocCustom.Click += radioLocCustom_Click;

            radioSizeAuto.CheckedChanged += radioSize_CheckedChanged;
            radioSizeCustom.Click += radioSizeCustom_Click;

            trackBarDuration.ValueChanged += trackBarDuration_ValueChanged;
            btnDurationShort.Click += btnDurationShort_Click;
            btnDurationMedium.Click += btnDurationMedium_Click;
            btnDurationLong.Click += btnDurationLong_Click;

            checkColumnName.CheckedChanged += checkColumnName_CheckedChanged;
            checkNotificationTimer.CheckedChanged += checkNotificationTimer_CheckedChanged;
            checkTimerCountDown.CheckedChanged += checkTimerCountDown_CheckedChanged;
            checkMediaPreviews.CheckedChanged += checkMediaPreviews_CheckedChanged;
            checkSkipOnLinkClick.CheckedChanged += checkSkipOnLinkClick_CheckedChanged;
            checkNonIntrusive.CheckedChanged += checkNonIntrusive_CheckedChanged;

            comboBoxIdlePause.SelectedValueChanged += comboBoxIdlePause_SelectedValueChanged;
            trackBarScrollSpeed.ValueChanged += trackBarScrollSpeed_ValueChanged;

            comboBoxDisplay.SelectedValueChanged += comboBoxDisplay_SelectedValueChanged;
            trackBarEdgeDistance.ValueChanged += trackBarEdgeDistance_ValueChanged;
        }

        private void TabSettingsNotifications_ParentChanged(object sender, EventArgs e){
            if (Parent == null){
                notification.HideNotification();
            }
            else{
                notification.ShowExampleNotification(true);
            }
        }

        private void notification_Activated(object sender, EventArgs e){
            notification.Hide();
            notification.Activated -= notification_Activated;
        }

        private void notification_Move(object sender, EventArgs e){
            if (radioLocCustom.Checked && notification.Location != ControlExtensions.InvisibleLocation){
                Config.CustomNotificationPosition = notification.Location;
            }
        }

        private void notification_ResizeEnd(object sender, EventArgs e){
            if (radioSizeCustom.Checked){
                Config.CustomNotificationSize = notification.BrowserSize;
                notification.ShowExampleNotification(false);
            }
        }

        private void radioLoc_CheckedChanged(object sender, EventArgs e){
            if (radioLocTL.Checked)Config.NotificationPosition = TweetNotification.Position.TopLeft;
            else if (radioLocTR.Checked)Config.NotificationPosition = TweetNotification.Position.TopRight;
            else if (radioLocBL.Checked)Config.NotificationPosition = TweetNotification.Position.BottomLeft;
            else if (radioLocBR.Checked)Config.NotificationPosition = TweetNotification.Position.BottomRight;

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = true;
            notification.ShowExampleNotification(false);
        }

        private void radioLocCustom_Click(object sender, EventArgs e){
            if (!Config.IsCustomNotificationPositionSet){
                Config.CustomNotificationPosition = notification.Location;
            }

            Config.NotificationPosition = TweetNotification.Position.Custom;

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = false;
            notification.ShowExampleNotification(false);

            if (notification.IsFullyOutsideView() && FormMessage.Question("Notification is outside view", "The notification seems to be outside of view, would you like to reset its position?", FormMessage.Yes, FormMessage.No)){
                Config.NotificationPosition = TweetNotification.Position.TopRight;
                notification.MoveToVisibleLocation();

                Config.CustomNotificationPosition = notification.Location;

                Config.NotificationPosition = TweetNotification.Position.Custom;
                notification.MoveToVisibleLocation();
            }
        }

        private void radioSize_CheckedChanged(object sender, EventArgs e){
            if (radioSizeAuto.Checked){
                Config.NotificationSize = TweetNotification.Size.Auto;
            }
            
            notification.ShowExampleNotification(false);
        }
        
        private void radioSizeCustom_Click(object sender, EventArgs e){
            if (!Config.IsCustomNotificationSizeSet){
                Config.CustomNotificationSize = notification.BrowserSize;
            }

            Config.NotificationSize = TweetNotification.Size.Custom;
            notification.ShowExampleNotification(false);
        }

        private void trackBarDuration_ValueChanged(object sender, EventArgs e){
            durationUpdateTimer.Stop();
            durationUpdateTimer.Start();

            Config.NotificationDurationValue = trackBarDuration.Value;
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";
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
            notification.ShowExampleNotification(false);
        }

        private void checkNotificationTimer_CheckedChanged(object sender, EventArgs e){
            Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            notification.ShowExampleNotification(true);
        }

        private void checkTimerCountDown_CheckedChanged(object sender, EventArgs e){
            Config.NotificationTimerCountDown = checkTimerCountDown.Checked;
            notification.ShowExampleNotification(true);
        }

        private void checkMediaPreviews_CheckedChanged(object sender, EventArgs e){
            Config.NotificationMediaPreviews = checkMediaPreviews.Checked;
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
                labelScrollSpeedValue.Text = trackBarScrollSpeed.Value+"%";
                Config.NotificationScrollSpeed = trackBarScrollSpeed.Value;
            }
        }

        private void comboBoxDisplay_SelectedValueChanged(object sender, EventArgs e){
            Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
            notification.ShowExampleNotification(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value+" px";
            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowExampleNotification(false);
        }

        private void durationUpdateTimer_Tick(object sender, EventArgs e){
            notification.ShowExampleNotification(true);
            durationUpdateTimer.Stop();
        }
    }
}
