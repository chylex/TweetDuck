using System;
using System.Windows.Forms;
using TweetDck.Updates;
using TweetDck.Updates.Events;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsGeneral : BaseTabSettings{
        private readonly UpdateHandler updates;
        private int updateCheckEventId = -1;

        public TabSettingsGeneral(UpdateHandler updates){
            InitializeComponent();

            this.updates = updates;
            this.updates.CheckFinished += updates_CheckFinished;
            Disposed += (sender, args) => this.updates.CheckFinished -= updates_CheckFinished;
            
            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.Items.Add("Combined");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior, 0), comboBoxTrayType.Items.Count-1);

            checkExpandLinks.Checked = Config.ExpandLinksOnHover;
            checkSpellCheck.Checked = Config.EnableSpellCheck;
            checkScreenshotBorder.Checked = Config.ShowScreenshotBorder;
            checkTrayHighlight.Checked = Config.EnableTrayHighlight;

            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;
        }

        private void checkExpandLinks_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.ExpandLinksOnHover = checkExpandLinks.Checked;
        }

        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.EnableSpellCheck = checkSpellCheck.Checked;
            PromptRestart();
        }

        private void checkScreenshotBorder_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.ShowScreenshotBorder = checkScreenshotBorder.Checked;
        }

        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
        }

        private void checkTrayHighlight_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;
            
            Config.EnableTrayHighlight = checkTrayHighlight.Checked;
        }

        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            if (!Ready)return;

            updateCheckEventId = updates.Check(true);

            if (updateCheckEventId == -1){
                MessageBox.Show("Sorry, your system is no longer supported.", "Unsupported System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else{
                btnCheckUpdates.Enabled = false;
                updates.DismissUpdate(string.Empty);
            }
        }

        private void updates_CheckFinished(object sender, UpdateCheckEventArgs e){
            if (e.EventId == updateCheckEventId){
                btnCheckUpdates.Enabled = true;

                if (!e.UpdateAvailable){
                    MessageBox.Show("Your version of "+Program.BrandName+" is up to date.", "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
