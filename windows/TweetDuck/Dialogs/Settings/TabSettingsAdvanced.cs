using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Core;

namespace TweetDuck.Dialogs.Settings {
	sealed partial class TabSettingsAdvanced : FormSettings.BaseTab {
		private readonly Action<string?> reinjectBrowserCSS;
		private readonly Action openDevTools;

		public TabSettingsAdvanced(Action<string?> reinjectBrowserCSS, Action openDevTools) {
			InitializeComponent();

			this.reinjectBrowserCSS = reinjectBrowserCSS;
			this.openDevTools = openDevTools;

			// application

			toolTip.SetToolTip(btnOpenAppFolder, "Opens the folder where the app is located.");
			toolTip.SetToolTip(btnOpenDataFolder, "Opens the folder where your profile data is located.");
			toolTip.SetToolTip(btnRestart, "Restarts the program using the same command\r\nline arguments that were used at launch.");
			toolTip.SetToolTip(btnRestartArgs, "Restarts the program with customizable\r\ncommand line arguments.");

			// browser settings

			toolTip.SetToolTip(checkTouchAdjustment, "Toggles Chromium touch screen adjustment.\r\nDisabled by default, because it is very imprecise with TweetDeck.");
			toolTip.SetToolTip(checkAutomaticallyDetectColorProfile, "Automatically detects the color profile of your system.\r\nUses the sRGB profile if disabled.");
			toolTip.SetToolTip(checkHardwareAcceleration, "Uses graphics card to improve performance.\r\nDisable if you experience visual glitches, or to save a small amount of RAM.");

			checkTouchAdjustment.Checked = SysConfig.EnableTouchAdjustment;
			checkAutomaticallyDetectColorProfile.Checked = SysConfig.EnableColorProfileDetection;
			checkHardwareAcceleration.Checked = SysConfig.HardwareAcceleration;

			// browser cache

			toolTip.SetToolTip(btnClearCache, "Clearing cache will free up space taken by downloaded images and other resources.");
			toolTip.SetToolTip(checkClearCacheAuto, "Automatically clears cache when its size exceeds the set threshold. Note that cache can only be cleared when closing TweetDuck.");

			checkClearCacheAuto.Checked = SysConfig.ClearCacheAutomatically;
			numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
			numClearCacheThreshold.SetValueSafe(SysConfig.ClearCacheThreshold);

			BrowserCache.GetCacheSize(task => {
				string text = task.Status == TaskStatus.RanToCompletion ? (int) Math.Ceiling(task.Result / (1024.0 * 1024.0)) + " MB" : "unknown";
				this.InvokeSafe(() => btnClearCache.Text = $"Clear Cache ({text})");
			});

			// configuration

			toolTip.SetToolTip(btnEditCefArgs, "Set custom command line arguments for Chromium Embedded Framework.");
			toolTip.SetToolTip(btnEditCSS, "Set custom CSS for browser and notification windows.");

			// proxy

			toolTip.SetToolTip(checkUseSystemProxyForAllConnections, "Sets whether all connections should automatically detect and use the system proxy.\r\nBy default, only the browser component uses the system proxy, while other parts (such as update checks) ignore it.\r\nDisabled by default because Windows' proxy detection can be really slow.");

			checkUseSystemProxyForAllConnections.Checked = SysConfig.UseSystemProxyForAllConnections;

			// development tools

			toolTip.SetToolTip(checkDevToolsInContextMenu, "Sets whether all context menus include an option to open dev tools.");
			toolTip.SetToolTip(checkDevToolsWindowOnTop, "Sets whether dev tool windows appears on top of other windows.");

			checkDevToolsInContextMenu.Checked = Config.DevToolsInContextMenu;
			checkDevToolsWindowOnTop.Checked = Config.DevToolsWindowOnTop;
		}

		public override void OnReady() {
			btnOpenAppFolder.Click += btnOpenAppFolder_Click;
			btnOpenDataFolder.Click += btnOpenDataFolder_Click;
			btnRestart.Click += btnRestart_Click;
			btnRestartArgs.Click += btnRestartArgs_Click;

			checkTouchAdjustment.CheckedChanged += checkTouchAdjustment_CheckedChanged;
			checkAutomaticallyDetectColorProfile.CheckedChanged += checkAutomaticallyDetectColorProfile_CheckedChanged;
			checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;

			btnClearCache.Click += btnClearCache_Click;
			checkClearCacheAuto.CheckedChanged += checkClearCacheAuto_CheckedChanged;

			btnEditCefArgs.Click += btnEditCefArgs_Click;
			btnEditCSS.Click += btnEditCSS_Click;

			checkUseSystemProxyForAllConnections.CheckedChanged += checkUseSystemProxyForAllConnections_CheckedChanged;

			checkDevToolsWindowOnTop.CheckedChanged += checkDevToolsWindowOnTop_CheckedChanged;
			checkDevToolsInContextMenu.CheckedChanged += checkDevToolsInContextMenuOnCheckedChanged;
		}

		public override void OnClosing() {
			SysConfig.ClearCacheAutomatically = checkClearCacheAuto.Checked;
			SysConfig.ClearCacheThreshold = (int) numClearCacheThreshold.Value;
		}

		#region Application

		private void btnOpenAppFolder_Click(object? sender, EventArgs e) {
			App.SystemHandler.OpenFileExplorer(App.ProgramPath);
		}

		private void btnOpenDataFolder_Click(object? sender, EventArgs e) {
			App.SystemHandler.OpenFileExplorer(App.StoragePath);
		}

		private void btnRestart_Click(object? sender, EventArgs e) {
			Program.Restart();
		}

		private void btnRestartArgs_Click(object? sender, EventArgs e) {
			using DialogSettingsRestart dialog = new DialogSettingsRestart(Arguments.GetCurrentClean());

			if (dialog.ShowDialog() == DialogResult.OK) {
				Program.RestartWithArgs(dialog.Args);
			}
		}

		#endregion

		#region Browser Settings

		private void checkTouchAdjustment_CheckedChanged(object? sender, EventArgs e) {
			SysConfig.EnableTouchAdjustment = checkTouchAdjustment.Checked;
		}

		private void checkAutomaticallyDetectColorProfile_CheckedChanged(object? sender, EventArgs e) {
			SysConfig.EnableColorProfileDetection = checkAutomaticallyDetectColorProfile.Checked;
		}

		private void checkHardwareAcceleration_CheckedChanged(object? sender, EventArgs e) {
			SysConfig.HardwareAcceleration = checkHardwareAcceleration.Checked;
		}

		#endregion

		#region Browser Cache

		private void btnClearCache_Click(object? sender, EventArgs e) {
			btnClearCache.Enabled = false;
			BrowserCache.SetClearOnExit();
			FormMessage.Information("Clear Cache", "Cache will be automatically cleared when TweetDuck exits.", FormMessage.OK);
		}

		private void checkClearCacheAuto_CheckedChanged(object? sender, EventArgs e) {
			numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
		}

		#endregion

		#region Configuration

		private void btnEditCefArgs_Click(object? sender, EventArgs e) {
			var parentForm = ParentForm ?? throw new InvalidOperationException("Dialog does not have a parent form!");
			var form = new DialogSettingsCefArgs(Config.CustomCefArgs);

			form.VisibleChanged += (sender2, args2) => {
				form.MoveToCenter(parentForm);
			};

			form.FormClosed += (sender2, args2) => {
				RestoreParentForm();

				if (form.DialogResult == DialogResult.OK) {
					Config.CustomCefArgs = form.CefArgs;
				}

				form.Dispose();
			};

			form.Show(parentForm);
			NativeMethods.SetFormDisabled(parentForm, true);
		}

		private void btnEditCSS_Click(object? sender, EventArgs e) {
			var parentForm = ParentForm ?? throw new InvalidOperationException("Dialog does not have a parent form!");
			var form = new DialogSettingsCSS(Config.CustomBrowserCSS, Config.CustomNotificationCSS, reinjectBrowserCSS, openDevTools);

			form.VisibleChanged += (sender2, args2) => {
				form.MoveToCenter(parentForm);
			};

			form.FormClosed += (sender2, args2) => {
				RestoreParentForm();

				if (form.DialogResult == DialogResult.OK) {
					Config.CustomBrowserCSS = form.BrowserCSS;
					Config.CustomNotificationCSS = form.NotificationCSS;
				}

				reinjectBrowserCSS(Config.CustomBrowserCSS); // reinject on cancel too, because the CSS is updated while typing
				form.Dispose();
			};

			form.Show(parentForm);
			NativeMethods.SetFormDisabled(parentForm, true);
		}

		private void RestoreParentForm() {
			if (ParentForm != null) { // when the parent is closed first, ParentForm is null in FormClosed event
				NativeMethods.SetFormDisabled(ParentForm, false);
			}
		}

		#endregion

		#region Proxy

		private void checkUseSystemProxyForAllConnections_CheckedChanged(object? sender, EventArgs e) {
			SysConfig.UseSystemProxyForAllConnections = checkUseSystemProxyForAllConnections.Checked;
		}

		#endregion

		#region Development Tools

		private void checkDevToolsInContextMenuOnCheckedChanged(object? sender, EventArgs e) {
			Config.DevToolsInContextMenu = checkDevToolsInContextMenu.Checked;
		}

		private void checkDevToolsWindowOnTop_CheckedChanged(object? sender, EventArgs e) {
			Config.DevToolsWindowOnTop = checkDevToolsWindowOnTop.Checked;
		}

		#endregion
	}
}
