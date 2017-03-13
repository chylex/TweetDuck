using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Controls;
using TweetDck.Core.Handling;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification{
    partial class FormNotificationBase : Form{
        public bool IsNotificationVisible{
            get{
                return Location != ControlExtensions.InvisibleLocation;
            }
        }

        public new Point Location{
            get{
                return base.Location;
            }

            set{
                Visible = (base.Location = value) != ControlExtensions.InvisibleLocation;
            }
        }
        
        public Func<bool> CanMoveWindow = () => true;

        private readonly Control owner;
        protected readonly NotificationFlags flags;
        protected readonly ChromiumWebBrowser browser;

        private int pauseCounter;

        public bool IsPaused{
            get{
                return pauseCounter > 0;
            }
        }

        protected override bool ShowWithoutActivation{
            get{
                return true;
            }
        }

        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }
        public string CurrentUrl { get; private set; }
        public string CurrentQuotedTweetUrl { get; set; }

        public event EventHandler Initialized;

        public FormNotificationBase(Form owner, NotificationFlags flags){
            InitializeComponent();

            this.owner = owner;
            this.flags = flags;

            owner.FormClosed += (sender, args) => Close();

            browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this, !flags.HasFlag(NotificationFlags.DisableContextMenu)),
                LifeSpanHandler = new LifeSpanHandler()
            };

            #if DEBUG
            browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

            panelBrowser.Controls.Add(browser);
            Disposed += (sender, args) => browser.Dispose();

            // ReSharper disable once VirtualMemberCallInContructor
            UpdateTitle();
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanMoveWindow()){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        // event handlers

        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e){
            if (e.IsBrowserInitialized && Initialized != null){
                Initialized(this, new EventArgs());
            }
        }

        // notification methods

        public virtual void HideNotification(bool loadBlank){
            if (loadBlank){
                browser.LoadHtml("", "about:blank");
            }

            Location = ControlExtensions.InvisibleLocation;
        }

        public virtual void FinishCurrentNotification(){}

        public virtual void PauseNotification(){
            if (pauseCounter++ == 0 && IsNotificationVisible){
                Location = ControlExtensions.InvisibleLocation;
            }
        }

        public virtual void ResumeNotification(){
            if (pauseCounter > 0){
                --pauseCounter;
            }
        }

        protected virtual void LoadTweet(TweetNotification tweet){
            CurrentUrl = tweet.Url;
            CurrentQuotedTweetUrl = string.Empty; // load from JS

            string bodyClasses = browser.Bounds.Contains(PointToClient(Cursor.Position)) ? "td-hover" : string.Empty;
            browser.LoadHtml(tweet.GenerateHtml(bodyClasses), "http://tweetdeck.twitter.com/?"+DateTime.Now.Ticks);
        }

        protected virtual void SetNotificationSize(int width, int height){
            ClientSize = new Size(width, height);
            panelBrowser.Height = height;
        }

        protected void MoveToVisibleLocation(){
            UserConfig config = Program.UserConfig;

            Screen screen = Screen.FromControl(owner);

            if (config.NotificationDisplay > 0 && config.NotificationDisplay <= Screen.AllScreens.Length){
                screen = Screen.AllScreens[config.NotificationDisplay-1];
            }
            
            bool needsReactivating = Location == ControlExtensions.InvisibleLocation;
            int edgeDist = config.NotificationEdgeDistance;

            switch(config.NotificationPosition){
                case TweetNotification.Position.TopLeft:
                    Location = new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+edgeDist);
                    break;

                case TweetNotification.Position.TopRight:
                    Location = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);
                    break;

                case TweetNotification.Position.BottomLeft:
                    Location = new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);
                    break;

                case TweetNotification.Position.BottomRight:
                    Location = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);
                    break;

                case TweetNotification.Position.Custom:
                    if (!config.IsCustomNotificationPositionSet){
                        config.CustomNotificationPosition = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);
                        config.Save();
                    }

                    Location = config.CustomNotificationPosition;
                    break;
            }

            if (needsReactivating && flags.HasFlag(NotificationFlags.TopMost)){
                NativeMethods.SetFormPos(this, NativeMethods.HWND_TOPMOST, NativeMethods.SWP_NOACTIVATE);
            }
        }

        protected virtual void OnNotificationReady(){
            MoveToVisibleLocation();
        }

        protected virtual void UpdateTitle(){
            Text = Program.BrandName;
        }

        public void DisplayTooltip(string text){
            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20, 5);
                toolTip.Show(text, this, position); // TODO figure out flickering when moving the mouse
            }
        }
    }
}
