using System;
using System.Windows.Forms;
using TweetDick.Configuration;
using TweetDick.Core.Handling;

namespace TweetDick.Core.Other{
    partial class FormSettings : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        private readonly FormNotification notification;

        public FormSettings(FormBrowser browserForm){
            InitializeComponent();

            notification = new FormNotification(browserForm,false);
            notification.Show(this);

            notification.Move += (sender, args) => {
                if (radioLocCustom.Checked){
                    Config.CustomNotificationPosition = notification.Location;
                }
            };

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

            trackBarEdgeDistance.Value = Config.NotificationEdgeDistance;
            notification.HideNotification();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e){
            Config.Save();
        }

        private void radioLoc_CheckedChanged(object sender, EventArgs e){
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

            trackBarEdgeDistance.Enabled = !radioLocCustom.Checked;
            notification.ShowNotificationForSettings(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowNotificationForSettings(false);
        }

        private void radioDur_CheckedChanged(object sender, EventArgs e){
            if (radioDurShort.Checked)Config.NotificationDuration = TweetNotification.Duration.Short;
            else if (radioDurMedium.Checked)Config.NotificationDuration = TweetNotification.Duration.Medium;
            else if (radioDurLong.Checked)Config.NotificationDuration = TweetNotification.Duration.Long;
            else if (radioDurVeryLong.Checked)Config.NotificationDuration = TweetNotification.Duration.VeryLong;

            notification.ShowNotificationForSettings(true);
        }

        private void radioDur_Click(object sender, EventArgs e){
            notification.ShowNotificationForSettings(true);
        }
    }
}
