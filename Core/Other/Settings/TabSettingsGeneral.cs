using System;
using TweetDuck.Core.Controls;
using TweetDuck.Updates;
using TweetDuck.Updates.Events;

namespace TweetDuck.Core.Other.Settings{
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
            
            toolTip.SetToolTip(trackBarZoom, toolTip.GetToolTip(labelZoomValue));
            trackBarZoom.SetValueSafe(Config.ZoomLevel);
            labelZoomValue.Text = trackBarZoom.Value+"%";

            checkExpandLinks.Checked = Config.ExpandLinksOnHover;
            checkSwitchAccountSelectors.Checked = Config.SwitchAccountSelectors;
            checkSpellCheck.Checked = Config.EnableSpellCheck;
            checkTrayHighlight.Checked = Config.EnableTrayHighlight;

            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;
        }

        public override void OnReady(){
            checkExpandLinks.CheckedChanged += checkExpandLinks_CheckedChanged;
            checkSwitchAccountSelectors.CheckedChanged += checkSwitchAccountSelectors_CheckedChanged;
            checkSpellCheck.CheckedChanged += checkSpellCheck_CheckedChanged;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;

            comboBoxTrayType.SelectedIndexChanged += comboBoxTrayType_SelectedIndexChanged;
            checkTrayHighlight.CheckedChanged += checkTrayHighlight_CheckedChanged;

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

        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            Config.EnableSpellCheck = checkSpellCheck.Checked;
            PromptRestart();
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e){
            if (trackBarZoom.AlignValueToTick()){
                zoomUpdateTimer.Stop();
                zoomUpdateTimer.Start();
                labelZoomValue.Text = trackBarZoom.Value+"%";
            }
        }

        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
        }

        private void checkTrayHighlight_CheckedChanged(object sender, EventArgs e){
            Config.EnableTrayHighlight = checkTrayHighlight.Checked;
        }

        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            updateCheckEventId = updates.Check(true);

            if (updateCheckEventId == -1){
                FormMessage.Warning("Unsupported System", "Sorry, your system is no longer supported.", FormMessage.OK);
            }
            else{
                btnCheckUpdates.Enabled = false;
                updates.DismissUpdate(string.Empty);
            }
        }

        private void updates_CheckFinished(object sender, UpdateCheckEventArgs e){
            this.InvokeAsyncSafe(() => {
                if (e.EventId == updateCheckEventId){
                    btnCheckUpdates.Enabled = true;

                    if (!e.UpdateAvailable){
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
