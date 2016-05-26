using System;
using System.Linq;
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

namespace TweetDck.Core{
    sealed partial class FormBrowser : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        public string UpdateInstallerPath { get; private set; }

        private readonly ChromiumWebBrowser browser;
        private readonly TweetDeckBridge bridge;
        private readonly FormNotification notification;
        private readonly UpdateHandler updates;

        private FormSettings currentFormSettings;
        private FormAbout currentFormAbout;
        private bool isLoaded;

        private FormWindowState prevState;

        public FormBrowser(){
            InitializeComponent();

            Text = Program.BrandName;

            bridge = new TweetDeckBridge(this);

            browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/"){
                MenuHandler = new ContextMenuBrowser(this),
                DialogHandler = new DialogHandlerBrowser(this)
            };

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.RegisterJsObject("$TD",bridge);

            Controls.Add(browser);

            Disposed += (sender, args) => browser.Dispose();

            trayIcon.ClickRestore += trayIcon_ClickRestore;
            trayIcon.ClickClose += trayIcon_ClickClose;
            Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

            UpdateTrayIcon();

            notification = CreateNotificationForm(true);
            notification.CanMoveWindow = () => false;
            notification.Show();

            updates = new UpdateHandler(browser,this);
            updates.UpdateAccepted += updates_UpdateAccepted;
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
            return new FormNotification(this,bridge,trayIcon,autoHide);
        }

        // window setup

        private void SetupWindow(){
            if (Config.IsCustomWindowLocationSet){
                Location = Config.WindowLocation;
                Size = Config.WindowSize;
                WindowState = Config.IsMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            if (!Config.IsCustomWindowLocationSet || !Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(Bounds))){
                Location = Screen.PrimaryScreen.WorkingArea.Location;
                Size = Screen.PrimaryScreen.WorkingArea.Size;
                WindowState = FormWindowState.Maximized;
            }

            prevState = WindowState;
            isLoaded = true;
        }

        private void UpdateTrayIcon(){
            trayIcon.Visible = Config.TrayBehavior != TrayIcon.Behavior.Disabled;
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
                foreach(string js in ScriptLoader.LoadResources("code.js").Where(js => js != null)){
                    browser.ExecuteScriptAsync(js);
                }
            }
        }

        private void FormBrowser_Resize(object sender, EventArgs e){
            if (!isLoaded)return;

            if (WindowState != prevState){
                prevState = WindowState;

                if (WindowState == FormWindowState.Minimized){
                    if (Config.TrayBehavior == TrayIcon.Behavior.MinimizeToTray){
                        Hide(); // hides taskbar too?! welp that works I guess
                    }
                }
                else{
                    FormBrowser_ResizeEnd(sender,e);
                }
            }
        }

        private void FormBrowser_ResizeEnd(object sender, EventArgs e){ // also triggers when the window moves
            if (!isLoaded)return;

            if (Location.X != -32000){
                Config.IsMaximized = WindowState == FormWindowState.Maximized;

                if (WindowState == FormWindowState.Normal || (WindowState == FormWindowState.Maximized && !Screen.FromControl(this).Equals(Screen.FromPoint(Config.WindowLocation)))){
                    Config.WindowLocation = Location;
                    Config.WindowSize = Size;
                }

                Config.Save();
            }
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e){
            if (!isLoaded)return;

            if (Config.TrayBehavior == TrayIcon.Behavior.CloseToTray && trayIcon.Visible && e.CloseReason == CloseReason.UserClosing){
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
            if (isLoaded && m.Msg == 0x210 && (m.WParam.ToInt32() & 0xFFFF) == 0x020B){ // WM_PARENTNOTIFY, WM_XBUTTONDOWN
                browser.ExecuteScriptAsync("TDGF_onMouseClickExtra",(m.WParam.ToInt32() >> 16) & 0xFFFF);
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

                currentFormSettings = new FormSettings(this,updates);

                currentFormSettings.FormClosed += (sender, args) => {
                    currentFormSettings = null;

                    if (!prevEnableUpdateCheck && Config.EnableUpdateCheck){
                        Config.DismissedUpdate = string.Empty;
                        Config.Save();
                        updates.Check(false);
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

        public void OnTweetPopup(TweetNotification tweet){
            notification.ShowNotification(tweet);
        }

        public void OnTweetSound(){
            
        }

        public void DisplayTooltip(string text, bool showInNotification){
            if (showInNotification){
                notification.DisplayTooltip(text);
                return;
            }

            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20,10);
                toolTip.Show(text,this,position);
            }
        }

        public void OnImagePasted(){
            browser.ExecuteScriptAsync("TDGF_tryPasteImage",new object[0]);
        }

        public void OnImagePastedFinish(){
            browser.ExecuteScriptAsync("TDGF_tryPasteImageFinish",new object[0]);
        }
    }
}