using System;
using System.Diagnostics;
using System.Windows.Forms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Core.Utils;
using TweetDck.Core.Controls;
using TweetDck.Updates;

namespace TweetDck.Core.Other{
    sealed partial class FormSettings : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        private readonly FormNotification notification;
        private readonly UpdateHandler updates;

        private bool isLoaded;
        private int updateCheckEventId;

        public FormSettings(FormBrowser browserForm, UpdateHandler updates){
            InitializeComponent();
            Shown += (sender, args) => isLoaded = true;

            Text = Program.BrandName+" Settings";

            this.updates = updates;

            updates.CheckFinished += updates_CheckFinished;
            Disposed += (sender, args) => updates.CheckFinished -= updates_CheckFinished;

            notification = browserForm.CreateNotificationForm(false);
            notification.CanMoveWindow = () => radioLocCustom.Checked;

            notification.Move += (sender, args) => {
                if (radioLocCustom.Checked){
                    Config.CustomNotificationPosition = notification.Location;
                }
            };

            notification.Show(this);

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

            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior,0),comboBoxTrayType.Items.Count-1);

            trackBarEdgeDistance.Value = Config.NotificationEdgeDistance;
            checkNotificationTimer.Checked = Config.DisplayNotificationTimer;
            checkExpandLinks.Checked = Config.ExpandLinksOnHover;
            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;
            checkHardwareAcceleration.Checked = HardwareAcceleration.IsEnabled;

            BrowserCache.CalculateCacheSize(bytes => this.InvokeSafe(() => {
                if (bytes == -1L){
                    btnClearCache.Text = "Clear Cache (unknown size)";
                }
                else{
                    btnClearCache.Text = "Clear Cache ("+(int)Math.Ceiling(bytes/(1024.0*1024.0))+" MB)";
                }
            }));
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e){
            Config.Save();
        }

        private void radioLoc_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

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

        private void radioLoc_Click(object sender, EventArgs e){
            if (!isLoaded)return;

            notification.ShowNotificationForSettings(false);
        }

        private void comboBoxDisplay_SelectedValueChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.NotificationDisplay = comboBoxDisplay.SelectedIndex;
            notification.ShowNotificationForSettings(false);
        }

        private void trackBarEdgeDistance_ValueChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.NotificationEdgeDistance = trackBarEdgeDistance.Value;
            notification.ShowNotificationForSettings(false);
        }

        private void radioDur_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            if (radioDurShort.Checked)Config.NotificationDuration = TweetNotification.Duration.Short;
            else if (radioDurMedium.Checked)Config.NotificationDuration = TweetNotification.Duration.Medium;
            else if (radioDurLong.Checked)Config.NotificationDuration = TweetNotification.Duration.Long;
            else if (radioDurVeryLong.Checked)Config.NotificationDuration = TweetNotification.Duration.VeryLong;

            notification.ShowNotificationForSettings(true);
        }

        private void radioDur_Click(object sender, EventArgs e){
            if (!isLoaded)return;

            notification.ShowNotificationForSettings(true);
        }

        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
        }

        private void checkNotificationTimer_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.DisplayNotificationTimer = checkNotificationTimer.Checked;
            notification.ShowNotificationForSettings(true);
        }

        private void checkExpandLinks_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.ExpandLinksOnHover = checkExpandLinks.Checked;
        }

        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            bool succeeded = false;

            if (checkHardwareAcceleration.Checked){
                if (HardwareAcceleration.CanEnable){
                    succeeded = HardwareAcceleration.Enable();
                }
                else{
                    MessageBox.Show("Cannot enable hardware acceleration, the libraries libEGL.dll and libGLESv2.dll could not be restored.",Program.BrandName+" Settings",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else{
                succeeded = HardwareAcceleration.Disable();
            }

            if (succeeded && MessageBox.Show("The application must restart for the setting to take place. Do you want to restart now?",Program.BrandName+" Settings",MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes){ // TODO
                Process.Start(Application.ExecutablePath,"-restart");
                Application.Exit();
            }
            else if (!succeeded){
                checkHardwareAcceleration.CheckedChanged -= checkHardwareAcceleration_CheckedChanged;
                checkHardwareAcceleration.Checked = HardwareAcceleration.IsEnabled;
                checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;
            }
        }

        private void btnClearCache_Click(object sender, EventArgs e){
            if (!isLoaded)return;

            isLoaded = false;
            btnClearCache.Enabled = false;
            isLoaded = true; // OTHERWISE WINFORMS CALLS THE ONCLICK EVENT FOR A RANDOM RADIO BUTTON WHAT THE CUNTFUCK IS THIS

            BrowserCache.SetClearOnExit();

            MessageBox.Show("Cache will be automatically cleared when "+Program.BrandName+" exits.","Clear Cache",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            if (!isLoaded)return;

            Config.DismissedUpdate = string.Empty;
            Config.Save();

            updateCheckEventId = updates.Check(true);

            isLoaded = false;
            btnCheckUpdates.Enabled = false;
            isLoaded = true; // SAME AS ABOVE
        }

        private void updates_CheckFinished(object sender, UpdateCheckEventArgs e){
            if (e.EventId == updateCheckEventId){
                btnCheckUpdates.Enabled = true;

                if (!e.UpdateAvailable){
                    MessageBox.Show("Your version of "+Program.BrandName+" is up to date.","No Updates Available",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }
    }
}
