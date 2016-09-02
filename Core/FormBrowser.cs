using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Core.Other;
using TweetDck.Resources;
using TweetDck.Core.Controls;
using System.Drawing;
using TweetDck.Updates;
using TweetDck.Plugins;
using TweetDck.Plugins.Events;

namespace TweetDck.Core{
    sealed partial class FormBrowser : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        public string UpdateInstallerPath { get; private set; }

        private readonly ChromiumWebBrowser browser;
        private readonly PluginManager plugins;
        private readonly UpdateHandler updates;

        private FormSettings currentFormSettings;
        private FormAbout currentFormAbout;
        private FormPlugins currentFormPlugins;
        private bool isLoaded;

        private FormWindowState prevState;

        public FormBrowser(PluginManager pluginManager){
            InitializeComponent();

            Text = Program.BrandName;

            this.plugins = pluginManager;
            this.plugins.Reloaded += plugins_Reloaded;
            this.plugins.Config.PluginChangedState += plugins_PluginChangedState;

            FormNotification notification = CreateNotificationForm(true);
            notification.CanMoveWindow = () => false;
            notification.Show();

            this.browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/"){
                MenuHandler = new ContextMenuBrowser(this),
                DialogHandler = new DialogHandlerBrowser(this),
                LifeSpanHandler = new LifeSpanHandler()
            };

            this.browser.LoadingStateChanged += Browser_LoadingStateChanged;
            this.browser.FrameLoadEnd += Browser_FrameLoadEnd;
            this.browser.RegisterJsObject("$TD", new TweetDeckBridge(this, notification));
            this.browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);

            Controls.Add(browser);

            Disposed += (sender, args) => browser.Dispose();

            this.trayIcon.ClickRestore += trayIcon_ClickRestore;
            this.trayIcon.ClickClose += trayIcon_ClickClose;
            Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

            UpdateTrayIcon();

            this.updates = new UpdateHandler(browser, this);
            this.updates.UpdateAccepted += updates_UpdateAccepted;
        }

        private void ShowChildForm(Form form){
            form.Show(this);
            form.MoveToCenter(this);
        }

        private void ForceClose(){
            trayIcon.Visible = false; // checked in FormClosing event
            Close();
        }

        public FormNotification CreateNotificationForm(bool autoHide){
            return new FormNotification(this, plugins, autoHide);
        }

        // window setup

        private void SetupWindow(){
            Config.BrowserWindow.Restore(this, true);
            prevState = WindowState;
            isLoaded = true;
        }

        private void UpdateTrayIcon(){
            trayIcon.Visible = Config.TrayBehavior.ShouldDisplayIcon();
        }

        // active event handlers

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading){
                Invoke(new Action(SetupWindow));
                browser.LoadingStateChanged -= Browser_LoadingStateChanged;
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain){
                ScriptLoader.ExecuteFile(e.Frame, "code.js");

                if (plugins.HasAnyPlugin(PluginEnvironment.Browser)){
                    ScriptLoader.ExecuteFile(e.Frame, PluginManager.PluginBrowserScriptFile);
                    plugins.ExecutePlugins(e.Frame, PluginEnvironment.Browser, true);
                }
            }
        }

        private void FormBrowser_Activated(object sender, EventArgs e){
            if (!isLoaded)return;

            trayIcon.HasNotifications = false;
        }

        private void FormBrowser_Resize(object sender, EventArgs e){
            if (!isLoaded)return;

            if (WindowState != prevState){
                prevState = WindowState;

                if (WindowState == FormWindowState.Minimized){
                    if (Config.TrayBehavior.ShouldHideOnMinimize()){
                        Hide(); // hides taskbar too?! welp that works I guess
                    }
                }
                else{
                    FormBrowser_ResizeEnd(sender, e);
                }
            }
        }

        private void FormBrowser_ResizeEnd(object sender, EventArgs e){ // also triggers when the window moves
            if (!isLoaded)return;

            if (Location.X != -32000){
                Config.BrowserWindow.Save(this);
                Config.Save();
            }
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e){
            if (!isLoaded)return;

            if (Config.TrayBehavior.ShouldHideOnClose() && trayIcon.Visible && e.CloseReason == CloseReason.UserClosing){
                Hide(); // hides taskbar too?! welp that works I guess
                e.Cancel = true;
            }
        }

        private void Config_TrayBehaviorChanged(object sender, EventArgs e){
            if (!isLoaded)return;
            
            UpdateTrayIcon();
        }

        private void trayIcon_ClickRestore(object sender, EventArgs e){
            if (!isLoaded)return;

            isLoaded = false;
            Show();
            SetupWindow();
            Activate();
            UpdateTrayIcon();
        }

        private void trayIcon_ClickClose(object sender, EventArgs e){
            if (!isLoaded)return;

            ForceClose();
        }
        
        private void plugins_Reloaded(object sender, PluginLoadEventArgs e){
            ReloadBrowser();
        }

        private void plugins_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            browser.ExecuteScriptAsync("window.TDPF_setPluginState", e.Plugin, e.IsEnabled ? 1 : 0); // ExecuteScriptAsync cannot handle boolean values as of yet
        }

        private void updates_UpdateAccepted(object sender, UpdateAcceptedEventArgs e){
            Hide();

            FormUpdateDownload downloadForm = new FormUpdateDownload(e.UpdateInfo);
            downloadForm.MoveToCenter(this);
            downloadForm.ShowDialog();

            if (downloadForm.UpdateStatus == FormUpdateDownload.Status.Succeeded){
                UpdateInstallerPath = downloadForm.InstallerPath;
                ForceClose();
            }
            else if (downloadForm.UpdateStatus == FormUpdateDownload.Status.Manual){
                ForceClose();
            }
            else{
                Show();
            }
        }

        protected override void WndProc(ref Message m){
            if (isLoaded && m.Msg == Program.WindowRestoreMessage){
                trayIcon_ClickRestore(trayIcon, new EventArgs());
                return;
            }

            if (isLoaded && m.Msg == 0x210 && (m.WParam.ToInt32() & 0xFFFF) == 0x020B){ // WM_PARENTNOTIFY, WM_XBUTTONDOWN
                browser.ExecuteScriptAsync("TDGF_onMouseClickExtra", (m.WParam.ToInt32() >> 16) & 0xFFFF);
                return;
            }

            base.WndProc(ref m);
        }

        // callback handlers

        public void OpenSettings(){
            if (currentFormSettings != null){
                currentFormSettings.BringToFront();
            }
            else{
                bool prevEnableUpdateCheck = Config.EnableUpdateCheck;

                currentFormSettings = new FormSettings(this, updates);

                currentFormSettings.FormClosed += (sender, args) => {
                    currentFormSettings = null;

                    if (!prevEnableUpdateCheck && Config.EnableUpdateCheck){
                        Config.DismissedUpdate = string.Empty;
                        Config.Save();
                        updates.Check(false);
                    }

                    if (!Config.EnableTrayHighlight){
                        trayIcon.HasNotifications = false;
                    }
                };

                ShowChildForm(currentFormSettings);
            }
        }

        public void OpenAbout(){
            if (currentFormAbout != null){
                currentFormAbout.BringToFront();
            }
            else{
                currentFormAbout = new FormAbout();
                currentFormAbout.FormClosed += (sender, args) => currentFormAbout = null;
                ShowChildForm(currentFormAbout);
            }
        }

        public void OpenPlugins(){
            if (currentFormPlugins != null){
                currentFormPlugins.BringToFront();
            }
            else{
                currentFormPlugins = new FormPlugins(plugins);
                currentFormPlugins.FormClosed += (sender, args) => currentFormPlugins = null;
                ShowChildForm(currentFormPlugins);
            }
        }

        public void OnTweetNotification(){
            if (Config.EnableTrayHighlight && !ContainsFocus){
                trayIcon.HasNotifications = true;
            }
        }

        public void DisplayTooltip(string text){
            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20, 10);
                toolTip.Show(text, this, position);
            }
        }

        public void OnImagePasted(){
            browser.ExecuteScriptAsync("TDGF_tryPasteImage", new object[0]);
        }

        public void OnImagePastedFinish(){
            browser.ExecuteScriptAsync("TDGF_tryPasteImageFinish", new object[0]);
        }

        public void ReloadBrowser(){
            browser.ExecuteScriptAsync("window.location.reload()");
        }
    }
}