using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Notification{
    partial class FormNotificationBase : Form{
        protected static int FontSizeLevel{
            get{
                switch(TweetDeckBridge.FontSize){
                    case "largest": return 4;
                    case "large": return 3;
                    case "small": return 1;
                    case "smallest": return 0;
                    default: return 2;
                }
            }
        }

        protected virtual Point PrimaryLocation{
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
        protected virtual bool CanDragWindow => true;

        public new Point Location{
            get{
                return base.Location;
            }

            set{
                Visible = (base.Location = value) != ControlExtensions.InvisibleLocation;
                FormBorderStyle = NotificationBorderStyle;
            }
        }

        protected virtual FormBorderStyle NotificationBorderStyle{
            get{
                if (WindowsUtils.ShouldAvoidToolWindow && Visible){ // Visible = workaround for alt+tab
                    return FormBorderStyle.FixedSingle;
                }
                else{
                    return FormBorderStyle.FixedToolWindow;
                }
            }
        }
        
        protected override bool ShowWithoutActivation => true;
        
        protected float DpiScale { get; }
        protected double SizeScale => DpiScale*Program.UserConfig.ZoomLevel/100.0;

        protected readonly FormBrowser owner;
        protected readonly ChromiumWebBrowser browser;
        
        private readonly ResourceHandlerNotification resourceHandler = new ResourceHandlerNotification();

        private TweetNotification currentNotification;
        private int pauseCounter;
        
        public string CurrentTweetUrl => currentNotification?.TweetUrl;
        public string CurrentQuoteUrl => currentNotification?.QuoteUrl;

        public bool CanViewDetail => currentNotification != null && !string.IsNullOrEmpty(currentNotification.ColumnId) && !string.IsNullOrEmpty(currentNotification.ChirpId);
        public bool IsPaused => pauseCounter > 0;

        protected bool IsCursorOverBrowser => browser.Bounds.Contains(PointToClient(Cursor.Position));
        
        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }

        public event EventHandler Initialized;

        protected FormNotificationBase(FormBrowser owner, bool enableContextMenu){
            InitializeComponent();

            this.owner = owner;
            this.owner.FormClosed += owner_FormClosed;

            this.browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this, enableContextMenu),
                JsDialogHandler = new JavaScriptDialogHandler(),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBase()
            };

            this.browser.Dock = DockStyle.None;
            this.browser.ClientSize = ClientSize;
            this.browser.IsBrowserInitializedChanged += browser_IsBrowserInitializedChanged;

            #if DEBUG
            this.browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            DpiScale = this.GetDPIScale();

            browser.SetupResourceHandler(TwitterUtils.TweetDeckURL, this.resourceHandler);
            browser.SetupResourceHandler(TweetNotification.AppLogo);

            Controls.Add(browser);

            Disposed += (sender, args) => {
                this.browser.Dispose();
                this.owner.FormClosed -= owner_FormClosed;
            };

            // ReSharper disable once VirtualMemberCallInContructor
            UpdateTitle();
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanDragWindow){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        public void TriggerAnalyticsEvent(AnalyticsFile.Event e){
            owner.TriggerAnalyticsEvent(e);
        }

        // event handlers

        private void owner_FormClosed(object sender, FormClosedEventArgs e){
            Close();
        }

        private void browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e){
            if (e.IsBrowserInitialized){
                Initialized?.Invoke(this, EventArgs.Empty);
            }
        }

        // notification methods

        public virtual void HideNotification(){
            browser.Load("about:blank");
            DisplayTooltip(null);

            Location = ControlExtensions.InvisibleLocation;
            currentNotification = null;
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
            return tweet.GenerateHtml(IsCursorOverBrowser ? "td-hover" : string.Empty);
        }

        protected virtual void LoadTweet(TweetNotification tweet){
            currentNotification = tweet;
            resourceHandler.SetHTML(GetTweetHTML(tweet));

            browser.Load(TwitterUtils.TweetDeckURL);
            DisplayTooltip(null);
        }

        protected virtual void SetNotificationSize(int width, int height){
            browser.ClientSize = ClientSize = new Size(BrowserUtils.Scale(width, SizeScale), BrowserUtils.Scale(height, SizeScale));
        }

        protected virtual void UpdateTitle(){
            string title = currentNotification?.ColumnTitle;
            Text = string.IsNullOrEmpty(title) || !Program.UserConfig.DisplayNotificationColumn ? Program.BrandName : Program.BrandName+" - "+title;
        }

        public void ShowTweetDetail(){
            owner.ShowTweetDetail(currentNotification.ColumnId, currentNotification.ChirpId, currentNotification.TweetUrl);
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
