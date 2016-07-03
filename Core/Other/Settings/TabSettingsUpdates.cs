using System;
using System.Windows.Forms;
using TweetDck.Updates;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsUpdates : BaseTabSettings{
        private readonly UpdateHandler updates;
        private int updateCheckEventId;

        public TabSettingsUpdates(UpdateHandler updates){
            InitializeComponent();

            this.updates = updates;

            this.updates.CheckFinished += updates_CheckFinished;
            Disposed += (sender, args) => this.updates.CheckFinished -= updates_CheckFinished;

            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;
        }

        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            if (!Ready)return;

            Config.DismissedUpdate = string.Empty;
            Config.Save();

            updateCheckEventId = updates.Check(true);

            btnCheckUpdates.Enabled = false;
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
