using System;
using System.Globalization;
using System.Windows.Forms;
using TweetDck.Core.Handling;
using TweetDck.Core.Controls;

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
                this.InvokeSafe(() => this.notification.ShowNotificationForSettings(true));
            };

            this.notification.Show(this);

            switch(Config.NotificationPosition){
                case TweetNotification.Position.TopLeft: radioLocTL.Checked = true; break;
                case TweetNotification.Position.TopRight: radioLocTR.Checked = true; break;
                case TweetNotification.Position.BottomLeft: radioLocBL.Checked = true; break;
                case TweetNotification.Position.BottomRight: radioLocBR.Checked = true; break;
                case TweetNotification.Position.Custom: radioLocCustom.Checked = true; break;
            }

            trackBarDuration.SetValueSafe(Config.NotificationDurationValue);
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";

            comboBoxDisplay.Items.Add("(Same As "+Program.BrandName+")");

            foreach(Screen screen in Screen.AllScreens){
                comboBoxDisplay.Items.Add(screen.DeviceName+" ("+screen.Bounds.Width+"x"+screen.Bounds.Height+")");
            }

            comboBoxDisplay.SelectedIndex = Math.Min(comboBoxDisplay.Items.Count-1, Config.NotificationDisplay);

            checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            checkTimerCountDown.Checked = Config.NotificationTimerCountDown;
            checkLegacyLoad.Checked = Config.NotificationLegacyLoad;

            trackBarEdgeDistance.SetValueSafe(Config.NotificationEdgeDistance);
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value.ToString(CultureInfo.InvariantCulture)+" px";

            tbCustomSound.Text = Config.NotificationSoundPath ?? string.Empty;

            Disposed += (sender, args) => this.notification.Dispose();
        }

        public override void OnClosing(){
            Config.NotificationSoundPath = tbCustomSound.Text;
        }

        private void TabSettingsNotifications_ParentChanged(object sender, EventArgs e){
            if (Parent == null){
                notification.HideNotification(false);
            }
            else{
                notification.ShowNotificationForSettings(true);
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

        private void trackBarDuration_ValueChanged(object sender, EventArgs e){
            if (!Ready)return;
            
            Config.NotificationDurationValue = trackBarDuration.Value;
            labelDurationValue.Text = Config.NotificationDurationValue+" ms/c";

            notification.ShowNotificationForSettings(true);
        }

        private void btnDurationShort_Click(object sender, EventArgs e){
            if (!Ready)return;

            trackBarDuration.Value = 15;
        }

        private void btnDurationMedium_Click(object sender, EventArgs e){
            if (!Ready)return;

            trackBarDuration.Value = 25;
        }

        private void btnDurationLong_Click(object sender, EventArgs e){
            if (!Ready)return;

            trackBarDuration.Value = 35;
        }

        private void checkNotificationTimer_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
            checkTimerCountDown.Enabled = checkNotificationTimer.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void checkTimerCountDown_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationTimerCountDown = checkTimerCountDown.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void checkLegacyLoad_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationLegacyLoad = checkLegacyLoad.Checked;
        }

        private void comboBoxDisplay_SelectedValueChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
            notification.ShowNotificationForSettings(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            if (!Ready)return;
            
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value.ToString(CultureInfo.InvariantCulture)+" px";
            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowNotificationForSettings(false);
        }

        private void btnBrowseSound_Click(object sender, EventArgs e){
            using(OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Title = "Custom Notification Sound",
                Filter = "Wave file (*.wav)|*.wav"
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    tbCustomSound.Text = dialog.FileName;
                }
            }
        }

        private void btnResetSound_Click(object sender, EventArgs e){
            tbCustomSound.Text = string.Empty;
        }
    }
}
