using System;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsTray : BaseTabSettings{
        public TabSettingsTray(){
            InitializeComponent();

            // system tray
            
            toolTip.SetToolTip(comboBoxTrayType, "Changes behavior of the Tray icon.\r\nRight-click the icon for an action menu.");
            toolTip.SetToolTip(checkTrayHighlight, "Highlights the tray icon if there are new tweets.\r\nOnly works for columns with popup or audio notifications.\r\nThe icon resets when the main window is restored.");

            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.Items.Add("Combined");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior, 0), comboBoxTrayType.Items.Count - 1);

            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
            checkTrayHighlight.Checked = Config.EnableTrayHighlight;
        }

        public override void OnReady(){
            comboBoxTrayType.SelectedIndexChanged += comboBoxTrayType_SelectedIndexChanged;
            checkTrayHighlight.CheckedChanged += checkTrayHighlight_CheckedChanged;
        }

        #region System Tray
        
        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
        }

        private void checkTrayHighlight_CheckedChanged(object sender, EventArgs e){
            Config.EnableTrayHighlight = checkTrayHighlight.Checked;
        }

        #endregion
    }
}
