using System.Windows.Forms;
using CefSharp.WinForms;
using System;
using System.Linq;
using TweetDick.Configuration;
using CefSharp;
using System.IO;
using System.Text;
using TweetDick.Core.Handling;

namespace TweetDick.Core{
    public partial class FormBrowser : Form{
        private static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        private readonly ChromiumWebBrowser browser;
        private readonly TweetDeckBridge bridge;

        public FormBrowser(){
            InitializeComponent();

            bridge = new TweetDeckBridge(this);

            browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/");
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.RegisterJsObject("$TD",bridge);

            Controls.Add(browser);
        }

        protected override void WndProc(ref Message m){
            FormWindowState prevState = WindowState;
            base.WndProc(ref m);

            if (prevState != WindowState && m.Msg == 0x0014){ // WM_ERASEBKGND
                FormBrowser_WindowStateChanged(this,new EventArgs());
            }
        }

        // window setup

        private void SetupWindow(){
            if (!Config.WindowSize.IsEmpty){
                Location = Config.WindowLocation;
                Size = Config.WindowSize;
                WindowState = Config.IsMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            if (Config.WindowSize.IsEmpty || !Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(Bounds))){
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

                string js = File.ReadAllText("code.js",Encoding.UTF8);
                browser.ExecuteScriptAsync(js);
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
    }
}
