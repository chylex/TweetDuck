using System;
using System.ComponentModel;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetLib.Core.Systems.Configuration;
using Res = TweetDuck.Properties.Resources;

namespace TweetDuck.Browser {
	sealed partial class TrayIcon : Component {
		public enum Behavior { // keep order
			Disabled,
			DisplayOnly,
			MinimizeToTray,
			CloseToTray,
			Combined
		}

		private static UserConfig Config => Program.Config.User;

		public event EventHandler? ClickRestore;
		public event EventHandler? ClickClose;

		public bool Visible {
			get {
				return notifyIcon.Visible;
			}

			set {
				notifyIcon.Visible = value;
				hasNotifications = false;
				UpdateIcon();
			}
		}

		public bool HasNotifications {
			get {
				return hasNotifications;
			}

			set {
				if (hasNotifications != value) {
					hasNotifications = value;
					UpdateIcon();
				}
			}
		}

		private readonly ContextMenu contextMenu;
		private readonly ContextMenuStrip fakeContextMenu;
		private bool hasNotifications;

		private TrayIcon() {
			InitializeComponent();

			this.contextMenu = new ContextMenu();
			this.contextMenu.MenuItems.Add("Restore", menuItemRestore_Click);
			this.contextMenu.MenuItems.Add("Mute notifications", menuItemMuteNotifications_Click);
			this.contextMenu.MenuItems.Add("Close", menuItemClose_Click);
			this.contextMenu.Popup += contextMenu_Popup;

			this.fakeContextMenu = new ContextMenuStrip();
			this.fakeContextMenu.Opening += fakeContextMenu_Opening;
			
			this.notifyIcon.ContextMenuStrip = this.fakeContextMenu;
			this.notifyIcon.Text = Program.BrandName;

			Config.MuteToggled += Config_MuteToggled;
			Disposed += (_, _) => Config.MuteToggled -= Config_MuteToggled;
		}

		public TrayIcon(IContainer container) : this() {
			container.Add(this);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();
				contextMenu.Dispose();
				fakeContextMenu.Dispose();
			}

			base.Dispose(disposing);
		}

		private void UpdateIcon() {
			if (Visible) {
				notifyIcon.Icon = HasNotifications ? Res.icon_tray_new : Config.MuteNotifications ? Res.icon_tray_muted : Res.icon_tray;
			}
		}

		// event handlers

		private void Config_MuteToggled(object? sender, EventArgs e) {
			UpdateIcon();
		}

		private void trayIcon_MouseClick(object? sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				menuItemRestore_Click(sender, e);
			}
		}

		private void fakeContextMenu_Opening(object? sender, CancelEventArgs args) {
			args.Cancel = true;
			contextMenu.Show(notifyIcon, Cursor.Position);
		}

		private void contextMenu_Popup(object? sender, EventArgs e) {
			contextMenu.MenuItems[1].Checked = Config.MuteNotifications;
		}

		private void menuItemRestore_Click(object? sender, EventArgs e) {
			ClickRestore?.Invoke(this, e);
		}

		private void menuItemMuteNotifications_Click(object? sender, EventArgs e) {
			Config.MuteNotifications = !contextMenu.MenuItems[1].Checked;
			Config.Save();
		}

		private void menuItemClose_Click(object? sender, EventArgs e) {
			ClickClose?.Invoke(this, e);
		}
	}

	static class BehaviorExtensions {
		public static bool ShouldDisplayIcon(this TrayIcon.Behavior behavior) {
			return behavior != TrayIcon.Behavior.Disabled;
		}

		public static bool ShouldHideOnMinimize(this TrayIcon.Behavior behavior) {
			return behavior is TrayIcon.Behavior.MinimizeToTray or TrayIcon.Behavior.Combined;
		}

		public static bool ShouldHideOnClose(this TrayIcon.Behavior behavior) {
			return behavior is TrayIcon.Behavior.CloseToTray or TrayIcon.Behavior.Combined;
		}
	}
}
