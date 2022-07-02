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

		private readonly ContextMenuStrip contextMenu;
		private bool hasNotifications;

		private TrayIcon() {
			InitializeComponent();

			this.contextMenu = new ContextMenuStrip();
			this.contextMenu.Items.Add("Restore", null, menuItemRestore_Click);
			this.contextMenu.Items.Add("Mute notifications", null, menuItemMuteNotifications_Click);
			this.contextMenu.Items.Add("Close", null, menuItemClose_Click);
			this.contextMenu.Opening += contextMenu_Popup;

			this.notifyIcon.ContextMenuStrip = contextMenu;
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

		private void contextMenu_Popup(object? sender, EventArgs e) {
			((ToolStripMenuItem) contextMenu.Items[1]).Checked = Config.MuteNotifications;
		}

		private void menuItemRestore_Click(object? sender, EventArgs e) {
			ClickRestore?.Invoke(this, e);
		}

		private void menuItemMuteNotifications_Click(object? sender, EventArgs e) {
			Config.MuteNotifications = !((ToolStripMenuItem) contextMenu.Items[1]).Checked;
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
