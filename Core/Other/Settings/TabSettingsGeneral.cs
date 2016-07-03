using System;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsGeneral : BaseTabSettings{
        public TabSettingsGeneral(){
            InitializeComponent();
            
            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior,0),comboBoxTrayType.Items.Count-1);

            checkExpandLinks.Checked = Program.UserConfig.ExpandLinksOnHover;
        }

        private void checkExpandLinks_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.ExpandLinksOnHover = checkExpandLinks.Checked;
        }

        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            if (!Ready)return;

            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
        }
    }
}
