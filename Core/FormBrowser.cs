using System.Windows.Forms;
using CefSharp.WinForms;
using System;
using System.Linq;
using TweetDick.Configuration;
using CefSharp;
using TweetDick.Core.Handling;
using TweetDick.Core.Other;
using System.Drawing;
using TweetDick.Resources;

namespace TweetDick.Core{
    partial class FormBrowser : Form{
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

        public FormBrowser(){
            InitializeComponent();

            bridge = new TweetDeckBridge(this);

            browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/"){ MenuHandler = new ContextMenuHandler(this) };
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.RegisterJsObject("$TD",bridge);

            Controls.Add(browser);

            notification = new FormNotification(this,bridge,true);
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
            Config.WindowLocation = Location;
            Config.WindowSize = Size;
            Config.Save();
        }

        private void FormBrowser_WindowStateChanged(object sender, EventArgs e){
            Config.IsMaximized = WindowState != FormWindowState.Normal;
            FormBrowser_ResizeEnd(sender,e);
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