using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Notification;
using TweetDuck.Browser.Notification.Screenshot;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Dialogs.Settings;
using TweetDuck.Management;
using TweetDuck.Updates;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Resources;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Core.Systems.Updates;

namespace TweetDuck.Browser {
	sealed partial class FormBrowser : Form, CustomKeyboardHandler.IBrowserKeyHandler {
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

		private readonly ResourceCache resourceCache;
		private readonly TweetDeckBrowser browser;
		private readonly ITweetDeckInterface tweetDeckInterface;
		private readonly FormNotificationTweet notification;
		private readonly PluginManager plugins;
		private readonly UpdateChecker updates;
		private readonly ContextMenu contextMenu;
		private readonly uint windowRestoreMessage;

		private bool isLoaded;
		private FormWindowState prevState;

		private TweetScreenshotManager notificationScreenshotManager;
		private VideoPlayer videoPlayer;

		public FormBrowser(ResourceCache resourceCache, PluginManager pluginManager, IUpdateCheckClient updateCheckClient, uint windowRestoreMessage) {
			InitializeComponent();

			Text = Program.BrandName;

			this.resourceCache = resourceCache;

			this.plugins = pluginManager;

			this.tweetDeckInterface = new TweetDeckInterfaceImpl(this);

			this.notification = new FormNotificationTweet(this, tweetDeckInterface, plugins);
			this.notification.Show();

			this.updates = new UpdateChecker(updateCheckClient, TaskScheduler.FromCurrentSynchronizationContext());
			this.updates.InteractionManager.UpdateAccepted += updateInteractionManager_UpdateAccepted;
			this.updates.InteractionManager.UpdateDismissed += updateInteractionManager_UpdateDismissed;

			this.browser = new TweetDeckBrowser(this, plugins, tweetDeckInterface, updates);
			this.contextMenu = ContextMenuBrowser.CreateMenu(this);

			this.windowRestoreMessage = windowRestoreMessage;

			Controls.Add(new MenuStrip { Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

			Config.MuteToggled += Config_MuteToggled;
			Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

			Disposed += (sender, args) => {
				Config.MuteToggled -= Config_MuteToggled;
				Config.TrayBehaviorChanged -= Config_TrayBehaviorChanged;
				browser.Dispose();
			};

			this.trayIcon.ClickRestore += trayIcon_ClickRestore;
			this.trayIcon.ClickClose += trayIcon_ClickClose;

			UpdateTray();

			if (Config.MuteNotifications) {
				UpdateFormIcon();
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

			if (!browser.Enabled) {     // when taking a screenshot, the window is unfocused and
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
				updates.InteractionManager.ClearUpdate();
				updates.InteractionManager.Dispose();
			}
		}

		private void Config_MuteToggled(object sender, EventArgs e) {
			UpdateFormIcon();
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

		private void updateInteractionManager_UpdateAccepted(object sender, UpdateInfo update) {
			this.InvokeAsyncSafe(() => {
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
						App.SystemHandler.OpenBrowser(Program.Website);
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
			});
		}

		private void updateInteractionManager_UpdateDismissed(object sender, UpdateInfo update) {
			this.InvokeAsyncSafe(() => {
				Config.DismissedUpdate = update.VersionTag;
				Config.Save();
			});
		}

		protected override void WndProc(ref Message m) {
			if (isLoaded && m.Msg == windowRestoreMessage) {
				using Process me = Process.GetCurrentProcess();

				if (me.Id == m.WParam.ToInt32()) {
					trayIcon_ClickRestore(trayIcon, EventArgs.Empty);
				}

				return;
			}

			if (browser.Ready && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN) {
				if (videoPlayer is { Running: true }) {
					videoPlayer.Close();
				}
				else {
					browser.Functions.OnMouseClickExtra((m.WParam.ToInt32() >> 16) & 0xFFFF);
				}

				return;
			}

			base.WndProc(ref m);
		}

		// bridge methods

		public void PauseNotification(NotificationPauseReason reason) {
			notification.PauseNotification(reason);
		}

		public void ResumeNotification(NotificationPauseReason reason) {
			notification.ResumeNotification(reason);
		}

		public void ReloadToTweetDeck() {
			#if DEBUG
			ResourceHotSwap.Run();
			resourceCache.ClearCache();
			#else
			if (ModifierKeys.HasFlag(Keys.Shift)) {
				resourceCache.ClearCache();
			}
			#endif

			browser.ReloadToTweetDeck();
		}

		public void OpenDevTools() {
			browser.OpenDevTools();
		}

		// callback handlers

		private void OnIntroductionClosed(bool showGuide) {
			if (Config.FirstRun) {
				Config.FirstRun = false;
				Config.Save();
			}

			if (showGuide) {
				FormGuide.Show();
			}
		}

		private void OpenContextMenu() {
			contextMenu.Show(this, PointToClient(Cursor.Position));
		}

		public void OpenSettings() {
			OpenSettings(null);
		}

		public void OpenSettings(Type startTab) {
			if (!FormManager.TryBringToFront<FormSettings>()) {
				bool prevEnableUpdateCheck = Config.EnableUpdateCheck;

				FormSettings form = new FormSettings(this, plugins, updates, browser.Functions, startTab);

				form.FormClosed += (sender, args) => {
					if (!prevEnableUpdateCheck && Config.EnableUpdateCheck) {
						Config.DismissedUpdate = null;
						Config.Save();

						updates.Check(true);
					}

					if (!Config.EnableTrayHighlight) {
						trayIcon.HasNotifications = false;
					}

					BrowserCache.RefreshTimer();

					if (form.ShouldReloadBrowser) {
						FormManager.TryFind<FormPlugins>()?.Close();
						plugins.Reload(); // also reloads the browser
					}
					else {
						Program.Config.User.TriggerOptionsDialogClosed();
					}

					notification.RequiresResize = true;
					form.Dispose();
				};

				ShowChildForm(form);
			}
		}

		public void OpenAbout() {
			if (!FormManager.TryBringToFront<FormAbout>()) {
				ShowChildForm(new FormAbout());
			}
		}

		public void OpenPlugins() {
			if (!FormManager.TryBringToFront<FormPlugins>()) {
				ShowChildForm(new FormPlugins(plugins));
			}
		}

		private void OpenProfileImport() {
			FormManager.TryFind<FormSettings>()?.Close();

			using DialogSettingsManage dialog = new DialogSettingsManage(plugins, true);

			if (!dialog.IsDisposed && dialog.ShowDialog() == DialogResult.OK && !dialog.IsRestarting) { // needs disposal check because the dialog may be closed in constructor
				BrowserProcessHandler.UpdatePrefs();
				FormManager.TryFind<FormPlugins>()?.Close();
				plugins.Reload(); // also reloads the browser
			}
		}

		private void ShowDesktopNotification(DesktopNotification notification) {
			this.notification.ShowNotification(notification);
		}

		private void OnTweetNotification() { // may be called multiple times, once for each type of notification
			if (Config.EnableTrayHighlight && !ContainsFocus) {
				trayIcon.HasNotifications = true;
			}
		}

		public void SaveVideo(string url, string username) {
			browser.SaveVideo(url, username);
		}

		private void PlayVideo(string videoUrl, string tweetUrl, string username, IJavascriptCallback callShowOverlay) {
			if (Arguments.HasFlag(Arguments.ArgHttpVideo)) {
				videoUrl = Regex.Replace(videoUrl, "^https://", "http://");
			}

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
					App.ErrorHandler.HandleException("Error Opening Video Player", "Could not open the video player.", true, e);
				}
			}
		}

		private void StopVideo() {
			videoPlayer?.Close();
		}

		public bool ShowTweetDetail(string columnId, string chirpId, string fallbackUrl) {
			Activate();

			if (!browser.IsTweetDeckWebsite) {
				FormMessage.Error("View Tweet Detail", "TweetDeck is not currently loaded.", FormMessage.OK);
				return false;
			}

			browser.Functions.ShowTweetDetail(columnId, chirpId, fallbackUrl);
			return true;
		}

		private void OnTweetScreenshotReady(string html, int width) {
			notificationScreenshotManager ??= new TweetScreenshotManager(this, plugins);
			notificationScreenshotManager.Trigger(html, width);
		}

		private void DisplayTooltip(string text) {
			if (string.IsNullOrEmpty(text)) {
				toolTip.Hide(this);
			}
			else {
				Point position = PointToClient(Cursor.Position);
				position.Offset(20, 10);
				toolTip.Show(text, this, position);
			}
		}

		public FormNotificationExample CreateExampleNotification() {
			return new FormNotificationExample(this, tweetDeckInterface, plugins);
		}

		bool CustomKeyboardHandler.IBrowserKeyHandler.HandleBrowserKey(Keys key) {
			if (videoPlayer is { Running: true }) {
				videoPlayer.SendKeyEvent(key);
				return true;
			}

			return false;
		}

		private sealed class TweetDeckInterfaceImpl : ITweetDeckInterface {
			private readonly FormBrowser form;

			public TweetDeckInterfaceImpl(FormBrowser form) {
				this.form = form;
			}

			public void Alert(string type, string contents) {
				MessageBoxIcon icon = type switch {
					"error"   => MessageBoxIcon.Error,
					"warning" => MessageBoxIcon.Warning,
					"info"    => MessageBoxIcon.Information,
					_         => MessageBoxIcon.None
				};

				FormMessage.Show("TweetDuck Browser Message", contents, icon, FormMessage.OK);
			}

			public void DisplayTooltip(string text) {
				form.InvokeAsyncSafe(() => form.DisplayTooltip(text));
			}

			public void FixClipboard() {
				form.InvokeAsyncSafe(ClipboardManager.StripHtmlStyles);
			}

			public int GetIdleSeconds() {
				return NativeMethods.GetIdleSeconds();
			}

			public void OnIntroductionClosed(bool showGuide) {
				form.InvokeAsyncSafe(() => form.OnIntroductionClosed(showGuide));
			}

			public void OnSoundNotification() {
				form.InvokeAsyncSafe(form.OnTweetNotification);
			}

			public void OpenContextMenu() {
				form.InvokeAsyncSafe(form.OpenContextMenu);
			}

			public void OpenProfileImport() {
				form.InvokeAsyncSafe(form.OpenProfileImport);
			}

			public void PlayVideo(string videoUrl, string tweetUrl, string username, object callShowOverlay) {
				form.InvokeAsyncSafe(() => form.PlayVideo(videoUrl, tweetUrl, username, (IJavascriptCallback) callShowOverlay));
			}

			public void ScreenshotTweet(string html, int width) {
				form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width));
			}

			public void ShowDesktopNotification(DesktopNotification notification) {
				form.InvokeAsyncSafe(() => {
					form.OnTweetNotification();
					form.ShowDesktopNotification(notification);
				});
			}

			public void StopVideo() {
				form.InvokeAsyncSafe(form.StopVideo);
			}

			public Task ExecuteCallback(object callback, params object[] parameters) {
				return ((IJavascriptCallback) callback).ExecuteAsync(parameters);
			}
		}
	}
}
