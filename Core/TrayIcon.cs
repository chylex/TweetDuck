using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TweetDck.Core{
    partial class TrayIcon : Component{
        public enum Behavior{ // keep order
            Disabled, DisplayOnly, MinimizeToTray, CloseToTray
        }

        public event EventHandler ClickRestore;
        public event EventHandler ClickClose;

        public bool Visible{
            get{
                return notifyIcon.Visible;
            }

            set{
                notifyIcon.Visible = value;
            }
        }

        public TrayIcon(){
            InitializeComponent();
            notifyIcon.Text = Program.BrandName;
        }

        // event handlers

        private void trayIcon_MouseClick(object sender, MouseEventArgs e){
            if (e.Button == MouseButtons.Left){
                restoreToolStripMenuItem_Click(sender,e);
            }
        }

        private void contextMenuTray_Opening(object sender, CancelEventArgs e){
            muteNotificationsToolStripMenuItem.CheckedChanged -= muteNotificationsToolStripMenuItem_CheckedChanged;
            muteNotificationsToolStripMenuItem.Checked = Program.UserConfig.MuteNotifications;
        }

        private void contextMenuTray_Opened(object sender, EventArgs e){
            muteNotificationsToolStripMenuItem.CheckedChanged += muteNotificationsToolStripMenuItem_CheckedChanged;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e){
            if (ClickRestore != null){
                ClickRestore(this,e);
            }
        }

        private void muteNotificationsToolStripMenuItem_CheckedChanged(object sender, EventArgs e){
            Program.UserConfig.MuteNotifications = muteNotificationsToolStripMenuItem.Checked;
            Program.UserConfig.Save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e){
            if (ClickClose != null){
                ClickClose(this,e);
            }
        }
    }
}
