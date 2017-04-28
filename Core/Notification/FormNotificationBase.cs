using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Configuration;
using TweetDck.Core.Controls;
using TweetDck.Core.Handling;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification{
    partial class FormNotificationBase : Form{
        protected Point PrimaryLocation{
            get{
                UserConfig config = Program.UserConfig;
                Screen screen;

                if (config.NotificationDisplay > 0 && config.NotificationDisplay <= Screen.AllScreens.Length){
                    screen = Screen.AllScreens[config.NotificationDisplay-1];
                }
                else{
                    screen = Screen.FromControl(owner);
                }
            
                int edgeDist = config.NotificationEdgeDistance;

                switch(config.NotificationPosition){
                    case TweetNotification.Position.TopLeft:
                        return new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+edgeDist);

                    case TweetNotification.Position.TopRight:
                        return new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);

                    case TweetNotification.Position.BottomLeft:
                        return new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);

                    case TweetNotification.Position.BottomRight:
                        return new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);

                    case TweetNotification.Position.Custom:
                        if (!config.IsCustomNotificationPositionSet){
                            config.CustomNotificationPosition = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);
                            config.Save();
                        }

                        return config.CustomNotificationPosition;
                }

                return Location;
            }
        }

        public bool IsNotificationVisible => Location != ControlExtensions.InvisibleLocation;

        public new Point Location{
            get => base.Location;
            set => Visible = (base.Location = value) != ControlExtensions.InvisibleLocation;
        }
        
        public Func<bool> CanMoveWindow = () => true;
        protected override bool ShowWithoutActivation => true;

        protected readonly Form owner;
        protected readonly ChromiumWebBrowser browser;

        private readonly ResourceHandlerNotification resourceHandler = new ResourceHandlerNotification();

        private string currentColumn;
        private int pauseCounter;

        public bool IsPaused => pauseCounter > 0;

        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }
        public string CurrentTweetUrl { get; private set; }
        public string CurrentQuoteUrl { get; private set; }

        public event EventHandler Initialized;

        public FormNotificationBase(Form owner, bool enableContextMenu){
            InitializeComponent();

            this.owner = owner;
            this.owner.FormClosed += owner_FormClosed;

            this.browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this, enableContextMenu),
                LifeSpanHandler = new LifeSpanHandler()
            };

            this.browser.Dock = DockStyle.None;
            this.browser.ClientSize = ClientSize;
            this.browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

            #if DEBUG
            this.browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            DefaultResourceHandlerFactory handlerFactory = (DefaultResourceHandlerFactory)browser.ResourceHandlerFactory;
            handlerFactory.RegisterHandler("https://tweetdeck.twitter.com", this.resourceHandler);

            Controls.Add(browser);

            Disposed += (sender, args) => {
                this.browser.Dispose();
                this.owner.FormClosed -= owner_FormClosed;
            };
            
            if (WindowsUtils.ShouldAvoidToolWindow){
                FormBorderStyle = FormBorderStyle.FixedSingle;
            }

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

        private void owner_FormClosed(object sender, FormClosedEventArgs e){
            Close();
        }

        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e){
            if (e.IsBrowserInitialized){
                Initialized?.Invoke(this, new EventArgs());
            }
        }

        // notification methods

        public virtual void HideNotification(bool loadBlank){
            if (loadBlank){
                browser.Load("about:blank");
            }

            Location = ControlExtensions.InvisibleLocation;
            currentColumn = null;
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

        protected virtual string GetTweetHTML(TweetNotification tweet){
            string bodyClasses = browser.Bounds.Contains(PointToClient(Cursor.Position)) ? "td-hover" : string.Empty;
            return tweet.GenerateHtml(bodyClasses);
        }

        protected virtual void LoadTweet(TweetNotification tweet){
            CurrentTweetUrl = tweet.TweetUrl;
            CurrentQuoteUrl = tweet.QuoteUrl;
            currentColumn = tweet.Column;

            resourceHandler.SetHTML(GetTweetHTML(tweet));
            browser.Load("https://tweetdeck.twitter.com");
        }

        protected virtual void SetNotificationSize(int width, int height){
            browser.ClientSize = ClientSize = new Size((int)Math.Round(width*Program.UserConfig.ZoomMultiplier), (int)Math.Round(height*Program.UserConfig.ZoomMultiplier));
        }

        protected virtual void OnNotificationReady(){
            MoveToVisibleLocation();
        }

        protected virtual void UpdateTitle(){
            Text = string.IsNullOrEmpty(currentColumn) || !Program.UserConfig.DisplayNotificationColumn ? Program.BrandName : Program.BrandName+" - "+currentColumn;
        }

        public void MoveToVisibleLocation(){
            bool needsReactivating = Location == ControlExtensions.InvisibleLocation;
            Location = PrimaryLocation;

            if (needsReactivating){
                NativeMethods.SetFormPos(this, NativeMethods.HWND_TOPMOST, NativeMethods.SWP_NOACTIVATE);
            }
        }

        public void DisplayTooltip(string text){
            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20, 5);
                toolTip.Show(text, this, position);
            }
        }
    }
}
