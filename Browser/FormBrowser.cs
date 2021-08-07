using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Bridge;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Handling.General;
using TweetDuck.Browser.Notification;
using TweetDuck.Browser.Notification.Screenshot;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Dialogs.Settings;
using TweetDuck.Management;
using TweetDuck.Management.Analytics;
using TweetDuck.Plugins;
using TweetDuck.Updates;
using TweetDuck.Utils;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Systems.Updates;

namespace TweetDuck.Browser {
	sealed partial class FormBrowser : Form, AnalyticsFile.IProvider {
		private static UserConfig Config => Program.Config.User;

		public bool IsWaiting {
			set {
				if (value) {
					browser.Enabled = false;
					Cursor = Cursors.WaitCursor;
				}
				else {
					browser.Enabled = true;
					Cursor = Cursors.Default;

					if (Focused) { // re-focus browser only if the window or a child is activated
						browser.Focus();
					}
				}
			}
		}

		public UpdateInstaller UpdateInstaller { get; private set; }
		private bool ignoreUpdateCheckError;

		public AnalyticsFile AnalyticsFile => analytics?.File ?? AnalyticsFile.Dummy;

		#pragma warning disable IDE0069 // Disposable fields should be disposed
		private readonly TweetDeckBrowser browser;
		private readonly FormNotificationTweet notification;
		#pragma warning restore IDE0069 // Disposable fields should be disposed

		private readonly PluginManager plugins;
		private readonly UpdateHandler updates;
		private readonly ContextMenu contextMenu;
		private readonly UpdateBridge updateBridge;

		private bool isLoaded;
		private FormWindowState prevState;

		private TweetScreenshotManager notificationScreenshotManager;
		private VideoPlayer videoPlayer;
		private AnalyticsManager analytics;

		public FormBrowser(PluginSchemeFactory pluginScheme) {
			InitializeComponent();

			Text = Program.BrandName;

			this.plugins = new PluginManager(Program.Config.Plugins, Program.PluginPath, Program.PluginDataPath);
			this.plugins.Reloaded += plugins_Reloaded;
			this.plugins.Executed += plugins_Executed;
			this.plugins.Reload();
			pluginScheme.Setup(plugins);

			this.notification = new FormNotificationTweet(this, plugins);
			this.notification.Show();

			this.updates = new UpdateHandler(new UpdateCheckClient(Program.InstallerPath), TaskScheduler.FromCurrentSynchronizationContext());
			this.updates.CheckFinished += updates_CheckFinished;

			this.updateBridge = new UpdateBridge(updates, this);
			this.updateBridge.UpdateAccepted += updateBridge_UpdateAccepted;
			this.updateBridge.UpdateDismissed += updateBridge_UpdateDismissed;

			this.browser = new TweetDeckBrowser(this, plugins, new TweetDeckBridge.Browser(this, notification), updateBridge);
			this.contextMenu = ContextMenuBrowser.CreateMenu(this);

			Controls.Add(new MenuStrip { Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

			Disposed += (sender, args) => {
				Config.MuteToggled -= Config_MuteToggled;
				Config.TrayBehaviorChanged -= Config_TrayBehaviorChanged;
				browser.Dispose();
			};

			Config.MuteToggled += Config_MuteToggled;

			this.trayIcon.ClickRestore += trayIcon_ClickRestore;
			this.trayIcon.ClickClose += trayIcon_ClickClose;
			Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

			UpdateTray();

			if (Config.MuteNotifications) {
				UpdateFormIcon();
			}

			if (Config.AllowDataCollection) {
				analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
			}

			RestoreWindow();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components?.Dispose();

				updates.Dispose();
				contextMenu.Dispose();

				notificationScreenshotManager?.Dispose();
				videoPlayer?.Dispose();
				analytics?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void ShowChildForm(Form form) {
			form.VisibleChanged += (sender, args) => form.MoveToCenter(this);
			form.Show(this);
		}

		public void ForceClose() {
			trayIcon.Visible = false; // checked in FormClosing event
			Close();
		}

		// window setup

		private void RestoreWindow() {
			Config.BrowserWindow.Restore(this, true);
			browser.PrepareSize(ClientSize);

			prevState = WindowState;
			isLoaded = true;
		}

		private void UpdateFormIcon() { // TODO fix to show icon in taskbar too
			Icon = Config.MuteNotifications ? Properties.Resources.icon_muted : Properties.Resources.icon;
		}

		private void UpdateTray() {
			trayIcon.Visible = Config.TrayBehavior.ShouldDisplayIcon();
		}

		// event handlers

		private void timerResize_Tick(object sender, EventArgs e) {
			FormBrowser_ResizeEnd(this, e); // also stops timer
		}

		private void FormBrowser_Activated(object sender, EventArgs e) {
			if (!isLoaded) {
				return;
			}

			trayIcon.HasNotifications = false;

			if (!browser.Enabled) {      // when taking a screenshot, the window is unfocused and
				browser.Enabled = true; // the browser is disabled; if the user clicks back into
			}                           // the window, enable the browser again
		}

		private void FormBrowser_LocationChanged(object sender, EventArgs e) {
			if (!isLoaded) {
				return;
			}

			timerResize.Stop();
			timerResize.Start();
		}

		private void FormBrowser_Resize(object sender, EventArgs e) {
			if (!isLoaded) {
				return;
			}

			if (WindowState != prevState) {
				prevState = WindowState;

				if (WindowState == FormWindowState.Minimized) {
					if (Config.TrayBehavior.ShouldHideOnMinimize()) {
						Hide(); // hides taskbar too?! welp that works I guess
					}
				}
				else {
					FormBrowser_ResizeEnd(sender, e);
				}
			}
			else {
				timerResize.Stop();
				timerResize.Start();
			}
		}

		private void FormBrowser_ResizeEnd(object sender, EventArgs e) { // also triggers when the window moves
			if (!isLoaded) {
				return;
			}

			timerResize.Stop();
			browser.PrepareSize(ClientSize); // needed to pre-size browser control when launched in maximized state

			if (Location != ControlExtensions.InvisibleLocation) {
				Config.BrowserWindow.Save(this);
				Config.Save();
			}
		}

		private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e) {
			if (!isLoaded) {
				return;
			}

			if (Config.TrayBehavior.ShouldHideOnClose() && trayIcon.Visible && e.CloseReason == CloseReason.UserClosing) {
				Hide(); // hides taskbar too?! welp that works I guess
				e.Cancel = true;
			}
		}

		private void FormBrowser_FormClosed(object sender, FormClosedEventArgs e) {
			if (isLoaded && UpdateInstaller == null) {
				updateBridge.Cleanup();
			}
		}

		private void Config_MuteToggled(object sender, EventArgs e) {
			UpdateFormIcon();
			AnalyticsFile.NotificationMutes.Trigger();
		}

		private void Config_TrayBehaviorChanged(object sender, EventArgs e) {
			UpdateTray();
		}

		private void trayIcon_ClickRestore(object sender, EventArgs e) {
			Show();
			RestoreWindow();
			Activate();
			UpdateTray();
		}

		private void trayIcon_ClickClose(object sender, EventArgs e) {
			ForceClose();
		}

		private void plugins_Reloaded(object sender, PluginErrorEventArgs e) {
			if (e.HasErrors) {
				FormMessage.Error("Error Loading Plugins", "The following plugins will not be available until the issues are resolved:\n\n" + string.Join("\n\n", e.Errors), FormMessage.OK);
			}

			if (isLoaded) {
				browser.ReloadToTweetDeck();
			}
		}

		private void plugins_Executed(object sender, PluginErrorEventArgs e) {
			if (e.HasErrors) {
				this.InvokeAsyncSafe(() => { FormMessage.Error("Error Executing Plugins", "Failed to execute the following plugins:\n\n" + string.Join("\n\n", e.Errors), FormMessage.OK); });
			}
		}

		private void updates_CheckFinished(object sender, UpdateCheckEventArgs e) {
			e.Result.Handle(update => {
				string tag = update.VersionTag;

				if (tag != Program.VersionTag && tag != Config.DismissedUpdate) {
					update.BeginSilentDownload();
					browser.ShowUpdateNotification(tag, update.ReleaseNotes);
				}
				else {
					updates.StartTimer();
				}
			}, ex => {
				if (!ignoreUpdateCheckError) {
					Program.Reporter.HandleException("Update Check Error", "An error occurred while checking for updates.", true, ex);
					updates.StartTimer();
				}
			});

			ignoreUpdateCheckError = true;
		}

		private void updateBridge_UpdateAccepted(object sender, UpdateInfo update) {
			FormManager.CloseAllDialogs();

			if (!string.IsNullOrEmpty(Config.DismissedUpdate)) {
				Config.DismissedUpdate = null;
				Config.Save();
			}

			void OnFinished() {
				UpdateDownloadStatus status = update.DownloadStatus;

				if (status == UpdateDownloadStatus.Done) {
					UpdateInstaller = new UpdateInstaller(update.InstallerPath);
					ForceClose();
				}
				else if (status != UpdateDownloadStatus.Canceled && FormMessage.Error("Update Has Failed", "Could not automatically download the update: " + (update.DownloadError?.Message ?? "unknown error") + "\n\nWould you like to open the website and try downloading the update manually?", FormMessage.Yes, FormMessage.No)) {
					BrowserUtils.OpenExternalBrowser(Program.Website);
					ForceClose();
				}
				else {
					Show();
				}
			}

			if (update.DownloadStatus.IsFinished(true)) {
				OnFinished();
			}
			else {
				FormUpdateDownload downloadForm = new FormUpdateDownload(update);

				downloadForm.VisibleChanged += (sender2, args2) => {
					downloadForm.MoveToCenter(this);
					Hide();
				};

				downloadForm.FormClosed += (sender2, args2) => {
					if (downloadForm.DialogResult != DialogResult.OK) {
						update.CancelDownload();
					}

					downloadForm.Dispose();
					OnFinished();
				};

				downloadForm.Show();
			}
		}

		private void updateBridge_UpdateDismissed(object sender, UpdateInfo update) {
			Config.DismissedUpdate = update.VersionTag;
			Config.Save();
		}

		protected override void WndProc(ref Message m) {
			if (isLoaded && m.Msg == Program.WindowRestoreMessage) {
				using Process me = Process.GetCurrentProcess();

				if (me.Id == m.WParam.ToInt32()) {
					trayIcon_ClickRestore(trayIcon, EventArgs.Empty);
				}

				return;
			}

			if (browser.Ready && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN) {
				if (videoPlayer != null && videoPlayer.Running) {
					videoPlayer.Close();
				}
				else {
					browser.OnMouseClickExtra(m.WParam);
					AnalyticsFile.BrowserExtraMouseButtons.Trigger();
				}

				return;
			}

			base.WndProc(ref m);
		}

		// bridge methods

		public void PauseNotification() {
			notification.PauseNotification();
		}

		public void ResumeNotification() {
			notification.ResumeNotification();
		}

		public void ReinjectCustomCSS(string css) {
			browser.ReinjectCustomCSS(css);
		}

		public void ReloadToTweetDeck() {
			Program.Resources.OnReloadTriggered();
			ignoreUpdateCheckError = false;
			browser.ReloadToTweetDeck();
			AnalyticsFile.BrowserReloads.Trigger();
		}

		public void AddSearchColumn(string query) {
			browser.AddSearchColumn(query);
		}

		public void TriggerTweetScreenshot() {
			browser.TriggerTweetScreenshot();
		}

		public void ReloadColumns() {
			browser.ReloadColumns();
		}

		public void PlaySoundNotification() {
			browser.PlaySoundNotification();
		}

		public void ApplyROT13() {
			browser.ApplyROT13();
			AnalyticsFile.UsedROT13.Trigger();
		}

		public void OpenDevTools() {
			browser.OpenDevTools();
		}

		// callback handlers

		public void OnIntroductionClosed(bool showGuide, bool allowDataCollection) {
			if (Config.FirstRun) {
				Config.FirstRun = false;
				Config.AllowDataCollection = allowDataCollection;
				Config.Save();

				if (allowDataCollection && analytics == null) {
					analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
				}
			}

			if (showGuide) {
				FormGuide.Show();
			}
		}

		public void OpenContextMenu() {
			contextMenu.Show(this, PointToClient(Cursor.Position));
		}

		public void OpenSettings() {
			OpenSettings(null);
		}

		public void OpenSettings(Type startTab) {
			if (!FormManager.TryBringToFront<FormSettings>()) {
				bool prevEnableUpdateCheck = Config.EnableUpdateCheck;

				FormSettings form = new FormSettings(this, plugins, updates, analytics, startTab);

				form.FormClosed += (sender, args) => {
					if (!prevEnableUpdateCheck && Config.EnableUpdateCheck) {
						Config.DismissedUpdate = null;
						Config.Save();

						updates.Check(true);
					}

					if (!Config.EnableTrayHighlight) {
						trayIcon.HasNotifications = false;
					}

					if (Config.AllowDataCollection) {
						analytics ??= new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
					}
					else if (analytics != null) {
						analytics.Dispose();
						analytics = null;
					}

					BrowserCache.RefreshTimer();

					if (form.ShouldReloadBrowser) {
						FormManager.TryFind<FormPlugins>()?.Close();
						plugins.Reload(); // also reloads the browser
					}
					else {
						browser.UpdateProperties();
					}

					notification.RequiresResize = true;
					form.Dispose();
				};

				AnalyticsFile.OpenOptions.Trigger();
				ShowChildForm(form);
			}
		}

		public void OpenAbout() {
			if (!FormManager.TryBringToFront<FormAbout>()) {
				AnalyticsFile.OpenAbout.Trigger();
				ShowChildForm(new FormAbout());
			}
		}

		public void OpenPlugins() {
			if (!FormManager.TryBringToFront<FormPlugins>()) {
				AnalyticsFile.OpenPlugins.Trigger();
				ShowChildForm(new FormPlugins(plugins));
			}
		}

		public void OpenProfileImport() {
			FormManager.TryFind<FormSettings>()?.Close();

			using DialogSettingsManage dialog = new DialogSettingsManage(plugins, true);

			if (!dialog.IsDisposed && dialog.ShowDialog() == DialogResult.OK && !dialog.IsRestarting) { // needs disposal check because the dialog may be closed in constructor
				BrowserProcessHandler.UpdatePrefs();
				FormManager.TryFind<FormPlugins>()?.Close();
				plugins.Reload(); // also reloads the browser
			}
		}

		public void OnTweetNotification() { // may be called multiple times, once for each type of notification
			if (Config.EnableTrayHighlight && !ContainsFocus) {
				trayIcon.HasNotifications = true;
			}
		}

		public void OnTweetSound() {
			AnalyticsFile.SoundNotifications.Trigger();
		}

		public void PlayVideo(string videoUrl, string tweetUrl, string username, IJavascriptCallback callShowOverlay) {
			string playerPath = Config.VideoPlayerPath;

			if (playerPath == null || !File.Exists(playerPath)) {
				if (videoPlayer == null) {
					videoPlayer = new VideoPlayer(this);
					videoPlayer.ProcessExited += (sender, args) => browser.HideVideoOverlay(true);
				}

				callShowOverlay.ExecuteAsync();
				callShowOverlay.Dispose();

				videoPlayer.Launch(videoUrl, tweetUrl, username);
			}
			else {
				callShowOverlay.Dispose();

				string quotedUrl = '"' + videoUrl + '"';
				string playerArgs = Config.VideoPlayerPathArgs == null ? quotedUrl : Config.VideoPlayerPathArgs + ' ' + quotedUrl;

				try {
					using (Process.Start(playerPath, playerArgs)) {}
				} catch (Exception e) {
					Program.Reporter.HandleException("Error Opening Video Player", "Could not open the video player.", true, e);
				}
			}

			AnalyticsFile.VideoPlays.Trigger();
		}

		public void StopVideo() {
			videoPlayer?.Close();
		}

		public bool ProcessBrowserKey(Keys key) {
			if (videoPlayer != null && videoPlayer.Running) {
				videoPlayer.SendKeyEvent(key);
				return true;
			}

			return false;
		}

		public void ShowTweetDetail(string columnId, string chirpId, string fallbackUrl) {
			Activate();

			if (!browser.IsTweetDeckWebsite) {
				FormMessage.Error("View Tweet Detail", "TweetDeck is not currently loaded.", FormMessage.OK);
				return;
			}

			notification.FinishCurrentNotification();
			browser.ShowTweetDetail(columnId, chirpId, fallbackUrl);
			AnalyticsFile.TweetDetails.Trigger();
		}

		public void OnTweetScreenshotReady(string html, int width) {
			notificationScreenshotManager ??= new TweetScreenshotManager(this, plugins);
			notificationScreenshotManager.Trigger(html, width);
			AnalyticsFile.TweetScreenshots.Trigger();
		}

		public void DisplayTooltip(string text) {
			if (string.IsNullOrEmpty(text)) {
				toolTip.Hide(this);
			}
			else {
				Point position = PointToClient(Cursor.Position);
				position.Offset(20, 10);
				toolTip.Show(text, this, position);
			}
		}
	}
}
