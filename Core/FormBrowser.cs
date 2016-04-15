using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Core.Other;
using TweetDck.Resources;

namespace TweetDck.Core{
    sealed partial class FormBrowser : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        private readonly ChromiumWebBrowser browser;
        private readonly TweetDeckBridge bridge;
        private readonly FormNotification notification;

        private FormSettings currentFormSettings;
        private FormAbout currentFormAbout;
        private bool isLoaded;

        public FormBrowser(){
            InitializeComponent();

            Text = Program.BrandName;

            bridge = new TweetDeckBridge(this);

            browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/"){ MenuHandler = new ContextMenuHandler(this) };
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.RegisterJsObject("$TD",bridge);

            Controls.Add(browser);

            Disposed += (sender, args) => browser.Dispose();

            notification = new FormNotification(this,bridge,true){ CanMoveWindow = () => false };
            notification.Show();
        }

        protected override void WndProc(ref Message m){
            FormWindowState prevState = WindowState;
            base.WndProc(ref m);

            if (prevState != WindowState && m.Msg == 0x0014){ // WM_ERASEBKGND
                FormBrowser_WindowStateChanged(this,new EventArgs());
            }
        }

        private void ShowChildForm(Form form){
            form.Show(this);
            form.Location = new Point(Location.X+Width/2-form.Width/2,Location.Y+Height/2-form.Height/2);
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

            isLoaded = true;
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
                string js = ScriptLoader.LoadResource("code.js");

                if (js != null){
                    browser.ExecuteScriptAsync(js);
                }
            }
        }

        private void FormBrowser_ResizeEnd(object sender, EventArgs e){ // also triggers when the window moves
            if (!isLoaded)return;

            if (Location.X != -32000){
                Config.WindowLocation = Location;
                Config.WindowSize = Size;
                Config.Save();
            }
        }

        private void FormBrowser_WindowStateChanged(object sender, EventArgs e){
            if (!isLoaded)return;

            if (WindowState == FormWindowState.Minimized){
                // TODO
            }
            else{
                Config.IsMaximized = WindowState == FormWindowState.Maximized;
                FormBrowser_ResizeEnd(sender,e);
            }
        }

        // callback handlers

        public void InvokeSafe(Action func){
            if (InvokeRequired){
                Invoke(func);
            }
            else{
                func();
            }
        }

        public void OpenSettings(){
            if (currentFormSettings != null){
                currentFormSettings.BringToFront();
            }
            else{
                currentFormSettings = new FormSettings(this);
                currentFormSettings.FormClosed += (sender, args) => currentFormSettings = null;
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
    }
}