using System;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling.General;
using TweetDuck.Updates;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsGeneral : BaseTabSettings{
        private readonly FormBrowser browser;
        private readonly UpdateHandler updates;
        private int updateCheckEventId = -1;

        public TabSettingsGeneral(FormBrowser browser, UpdateHandler updates){
            InitializeComponent();

            this.browser = browser;

            this.updates = updates;
            this.updates.CheckFinished += updates_CheckFinished;
            Disposed += (sender, args) => this.updates.CheckFinished -= updates_CheckFinished;
            
            toolTip.SetToolTip(checkExpandLinks, "Expands links inside the tweets. If disabled,\r\nthe full links show up in a tooltip instead.");
            toolTip.SetToolTip(checkSwitchAccountSelectors, "When (re)tweeting, click to select a single account or hold Shift to\r\nselect multiple accounts, instead of TweetDeck\'s default behavior.");
            toolTip.SetToolTip(checkOpenSearchInFirstColumn, "By default, TweetDeck adds Search columns at the end.\r\nThis option makes them appear before the first column instead.");
            toolTip.SetToolTip(checkKeepLikeFollowDialogsOpen, "Allows liking and following from multiple accounts at once,\r\ninstead of automatically closing the dialog after taking an action.");
            toolTip.SetToolTip(checkBestImageQuality, "When right-clicking a tweet image, the context menu options\r\nwill use links to the original image size (:orig in the URL).");
            toolTip.SetToolTip(checkAnimatedAvatars, "Some old Twitter avatars could be uploaded as animated GIFs.");

            toolTip.SetToolTip(labelZoomValue, "Changes the zoom level.\r\nAlso affects notifications and screenshots.");
            toolTip.SetToolTip(trackBarZoom, toolTip.GetToolTip(labelZoomValue));

            toolTip.SetToolTip(checkUpdateNotifications, "Checks for updates every hour.\r\nIf an update is dismissed, it will not appear again.");
            toolTip.SetToolTip(btnCheckUpdates, "Forces an update check, even for updates that had been dismissed.");

            trackBarZoom.SetValueSafe(Config.ZoomLevel);
            labelZoomValue.Text = trackBarZoom.Value+"%";

            checkExpandLinks.Checked = Config.ExpandLinksOnHover;
            checkSwitchAccountSelectors.Checked = Config.SwitchAccountSelectors;
            checkOpenSearchInFirstColumn.Checked = Config.OpenSearchInFirstColumn;
            checkKeepLikeFollowDialogsOpen.Checked = Config.KeepLikeFollowDialogsOpen;
            checkBestImageQuality.Checked = Config.BestImageQuality;
            checkAnimatedAvatars.Checked = Config.EnableAnimatedImages;

            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;
        }

        public override void OnReady(){
            checkExpandLinks.CheckedChanged += checkExpandLinks_CheckedChanged;
            checkSwitchAccountSelectors.CheckedChanged += checkSwitchAccountSelectors_CheckedChanged;
            checkOpenSearchInFirstColumn.CheckedChanged += checkOpenSearchInFirstColumn_CheckedChanged;
            checkKeepLikeFollowDialogsOpen.CheckedChanged += checkKeepLikeFollowDialogsOpen_CheckedChanged;
            checkBestImageQuality.CheckedChanged += checkBestImageQuality_CheckedChanged;
            checkAnimatedAvatars.CheckedChanged += checkAnimatedAvatars_CheckedChanged;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;

            checkUpdateNotifications.CheckedChanged += checkUpdateNotifications_CheckedChanged;
            btnCheckUpdates.Click += btnCheckUpdates_Click;
        }

        public override void OnClosing(){
            Config.ZoomLevel = trackBarZoom.Value;
        }

        private void checkExpandLinks_CheckedChanged(object sender, EventArgs e){
            Config.ExpandLinksOnHover = checkExpandLinks.Checked;
        }

        private void checkSwitchAccountSelectors_CheckedChanged(object sender, EventArgs e){
            Config.SwitchAccountSelectors = checkSwitchAccountSelectors.Checked;
        }

        private void checkOpenSearchInFirstColumn_CheckedChanged(object sender, EventArgs e){
            Config.OpenSearchInFirstColumn = checkOpenSearchInFirstColumn.Checked;
        }

        private void checkKeepLikeFollowDialogsOpen_CheckedChanged(object sender, EventArgs e){
            Config.KeepLikeFollowDialogsOpen = checkKeepLikeFollowDialogsOpen.Checked;
        }

        private void checkBestImageQuality_CheckedChanged(object sender, EventArgs e){
            Config.BestImageQuality = checkBestImageQuality.Checked;
        }

        private void checkAnimatedAvatars_CheckedChanged(object sender, EventArgs e){
            Config.EnableAnimatedImages = checkAnimatedAvatars.Checked;
            BrowserProcessHandler.UpdatePrefs().ContinueWith(task => browser.ReloadColumns());
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e){
            if (trackBarZoom.AlignValueToTick()){
                zoomUpdateTimer.Stop();
                zoomUpdateTimer.Start();
                labelZoomValue.Text = trackBarZoom.Value+"%";
            }
        }

        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            Config.DismissedUpdate = null;

            btnCheckUpdates.Enabled = false;
            updateCheckEventId = updates.Check(true);
        }

        private void updates_CheckFinished(object sender, UpdateEventArgs e){
            this.InvokeAsyncSafe(() => {
                if (e.EventId == updateCheckEventId){
                    btnCheckUpdates.Enabled = true;

                    if (!e.IsUpdateAvailable){
                        FormMessage.Information("No Updates Available", "Your version of TweetDuck is up to date.", FormMessage.OK);
                    }
                }
            });
        }

        private void zoomUpdateTimer_Tick(object sender, EventArgs e){
            Config.ZoomLevel = trackBarZoom.Value;
            zoomUpdateTimer.Stop();
        }
    }
}
