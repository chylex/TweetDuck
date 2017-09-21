using System;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsTray : BaseTabSettings{
        public TabSettingsTray(){
            InitializeComponent();
            
            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.Items.Add("Combined");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior, 0), comboBoxTrayType.Items.Count-1);

            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
            checkTrayHighlight.Checked = Config.EnableTrayHighlight;
        }

        public override void OnReady(){
            comboBoxTrayType.SelectedIndexChanged += comboBoxTrayType_SelectedIndexChanged;
            checkTrayHighlight.CheckedChanged += checkTrayHighlight_CheckedChanged;
        }

        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
        }

        private void checkTrayHighlight_CheckedChanged(object sender, EventArgs e){
            Config.EnableTrayHighlight = checkTrayHighlight.Checked;
        }
    }
}
