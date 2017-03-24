using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TweetDck.Core{
    partial class TrayIcon : Component{
        public enum Behavior{ // keep order
            Disabled, DisplayOnly, MinimizeToTray, CloseToTray, Combined
        }

        public event EventHandler ClickRestore;
        public event EventHandler ClickClose;

        public bool Visible{
            get{
                return notifyIcon.Visible;
            }

            set{
                if (value){
                    notifyIcon.Icon = Properties.Resources.icon_tray;
                }
                
                notifyIcon.Visible = value;
                hasNotifications = false;
            }
        }

        public bool HasNotifications{
            get{
                return hasNotifications;
            }

            set{
                if (hasNotifications != value && Visible){
                    notifyIcon.Icon = value ? Properties.Resources.icon_tray_new : Properties.Resources.icon_tray;
                    hasNotifications = value;
                }
            }
        }

        private readonly ContextMenu contextMenu;
        private bool hasNotifications;

        public TrayIcon(){
            InitializeComponent();

            this.contextMenu = new ContextMenu();
            this.contextMenu.MenuItems.Add("Restore", menuItemRestore_Click);
            this.contextMenu.MenuItems.Add("Mute notifications", menuItemMuteNotifications_Click);
            this.contextMenu.MenuItems.Add("Close", menuItemClose_Click);
            this.contextMenu.Popup += contextMenu_Popup;
                
            this.notifyIcon.ContextMenu = contextMenu;
            this.notifyIcon.Text = Program.BrandName;
        }

        public TrayIcon(IContainer container) : this(){
            container.Add(this);
        }

        // event handlers

        private void trayIcon_MouseClick(object sender, MouseEventArgs e){
            if (e.Button == MouseButtons.Left){
                menuItemRestore_Click(sender, e);
            }
        }

        private void contextMenu_Popup(object sender, EventArgs e){
            contextMenu.MenuItems[1].Checked = Program.UserConfig.MuteNotifications;
        }

        private void menuItemRestore_Click(object sender, EventArgs e){
            if (ClickRestore != null){
                ClickRestore(this, e);
            }
        }

        private void menuItemMuteNotifications_Click(object sender, EventArgs e){
            Program.UserConfig.MuteNotifications = !contextMenu.MenuItems[1].Checked;
            Program.UserConfig.Save();
        }

        private void menuItemClose_Click(object sender, EventArgs e){
            if (ClickClose != null){
                ClickClose(this, e);
            }
        }
    }

    static class BehaviorExtensions{
        public static bool ShouldDisplayIcon(this TrayIcon.Behavior behavior){
            return behavior != TrayIcon.Behavior.Disabled;
        }

        public static bool ShouldHideOnMinimize(this TrayIcon.Behavior behavior){
            return behavior == TrayIcon.Behavior.MinimizeToTray || behavior == TrayIcon.Behavior.Combined;
        }

        public static bool ShouldHideOnClose(this TrayIcon.Behavior behavior){
            return behavior == TrayIcon.Behavior.CloseToTray || behavior == TrayIcon.Behavior.Combined;
        }
    }
}
