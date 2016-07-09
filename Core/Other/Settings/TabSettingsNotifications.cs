using System;
using System.Windows.Forms;
using TweetDck.Core.Handling;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsNotifications : BaseTabSettings{
        private readonly FormNotification notification;

        public TabSettingsNotifications(FormNotification notification){
            InitializeComponent();

            this.notification = notification;
            this.notification.CanMoveWindow = () => radioLocCustom.Checked;

            this.notification.Move += (sender, args) => {
                if (radioLocCustom.Checked){
                    Config.CustomNotificationPosition = this.notification.Location;
                }
            };
            
            this.notification.Initialized += (sender, args) => {
                this.notification.ShowNotificationForSettings(true);
            };

            this.notification.Show(this);

            switch(Config.NotificationPosition){
                case TweetNotification.Position.TopLeft: radioLocTL.Checked = true; break;
                case TweetNotification.Position.TopRight: radioLocTR.Checked = true; break;
                case TweetNotification.Position.BottomLeft: radioLocBL.Checked = true; break;
                case TweetNotification.Position.BottomRight: radioLocBR.Checked = true; break;
                case TweetNotification.Position.Custom: radioLocCustom.Checked = true; break;
            }

            switch(Config.NotificationDuration){
                case TweetNotification.Duration.Short: radioDurShort.Checked = true; break;
                case TweetNotification.Duration.Medium: radioDurMedium.Checked = true; break;
                case TweetNotification.Duration.Long: radioDurLong.Checked = true; break;
                case TweetNotification.Duration.VeryLong: radioDurVeryLong.Checked = true; break;
            }

            comboBoxDisplay.Items.Add("(Same As "+Program.BrandName+")");

            foreach(Screen screen in Screen.AllScreens){
                comboBoxDisplay.Items.Add(screen.DeviceName+" ("+screen.Bounds.Width+"x"+screen.Bounds.Height+")");
            }

            comboBoxDisplay.SelectedIndex = Math.Min(comboBoxDisplay.Items.Count-1,Config.NotificationDisplay);

            checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
            trackBarEdgeDistance.Value = Config.NotificationEdgeDistance;

            Disposed += (sender, args) => this.notification.Dispose();
        }

        private void TabSettingsNotifications_ParentChanged(object sender, EventArgs e){
            if (Parent == null){
                notification.HideNotification(false);
            }
            else{
                notification.ShowNotificationForSettings(false);
            }
        }

        private void radioLoc_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            if (radioLocTL.Checked)Config.NotificationPosition = TweetNotification.Position.TopLeft;
            else if (radioLocTR.Checked)Config.NotificationPosition = TweetNotification.Position.TopRight;
            else if (radioLocBL.Checked)Config.NotificationPosition = TweetNotification.Position.BottomLeft;
            else if (radioLocBR.Checked)Config.NotificationPosition = TweetNotification.Position.BottomRight;
            else if (radioLocCustom.Checked){
                if (!Config.IsCustomNotificationPositionSet){
                    Config.CustomNotificationPosition = notification.Location;
                }

                Config.NotificationPosition = TweetNotification.Position.Custom;
            }

            comboBoxDisplay.Enabled = trackBarEdgeDistance.Enabled = !radioLocCustom.Checked;
            notification.ShowNotificationForSettings(false);
        }

        private void radioDur_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            if (radioDurShort.Checked)Config.NotificationDuration = TweetNotification.Duration.Short;
            else if (radioDurMedium.Checked)Config.NotificationDuration = TweetNotification.Duration.Medium;
            else if (radioDurLong.Checked)Config.NotificationDuration = TweetNotification.Duration.Long;
            else if (radioDurVeryLong.Checked)Config.NotificationDuration = TweetNotification.Duration.VeryLong;

            notification.ShowNotificationForSettings(true);
        }

        private void radioDur_Click(object sender, EventArgs e){
            if (!Ready)return;

            notification.ShowNotificationForSettings(true);
        }

        private void checkNotificationTimer_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void comboBoxDisplay_SelectedValueChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
            notification.ShowNotificationForSettings(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowNotificationForSettings(false);
        }
    }
}
