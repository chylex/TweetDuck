using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Notification;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsNotifications : BaseTabSettings{
        private readonly FormNotificationMain notification;
        private readonly Point initCursorPosition;

        public TabSettingsNotifications(FormNotificationMain notification, bool ignoreAutoClick){
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

            this.notification.Activated += notification_Activated;
            this.notification.Show(this);

            initCursorPosition = ignoreAutoClick ? ControlExtensions.InvisibleLocation : Cursor.Position;

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
            checkNonIntrusive.Checked = Config.NotificationNonIntrusiveMode;

            trackBarEdgeDistance.SetValueSafe(Config.NotificationEdgeDistance);
            labelEdgeDistanceValue.Text = trackBarEdgeDistance.Value.ToString(CultureInfo.InvariantCulture)+" px";

            tbCustomSound.Text = Config.NotificationSoundPath;

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

        private void notification_Activated(object sender, EventArgs e){
            if (Cursor.Position == initCursorPosition && initCursorPosition != ControlExtensions.InvisibleLocation){
                Timer delay = WindowsUtils.CreateSingleTickTimer(1);

                delay.Tick += (sender2, args2) => { // here you can see a disgusting hack to force the freshly opened notification window out of focus
                    NativeMethods.SimulateMouseClick(NativeMethods.MouseButton.Left); // because for some reason, the stupid thing keeps stealing it
                    delay.Dispose(); // even after using ShowWithoutActivation, the CreateParams bullshit, and about a million different combinations
                }; // of trying to force the original form back into focus in various events, so you will have to fucking deal with it, alright

                delay.Start();
            }

            notification.Activated -= notification_Activated;
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

        private void checkNonIntrusive_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.NotificationNonIntrusiveMode = checkNonIntrusive.Checked;
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

        private void tbCustomSound_TextChanged(object sender, EventArgs e){
            // also runs when the control is created, i.e. when Ready is false

            bool fileExists = string.IsNullOrEmpty(tbCustomSound.Text) || File.Exists(tbCustomSound.Text);
            tbCustomSound.ForeColor = fileExists ? SystemColors.WindowText : Color.Maroon;
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
