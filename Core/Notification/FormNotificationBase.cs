using CefSharp.WinForms;
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
    abstract partial class FormNotificationBase : Form, AnalyticsFile.IProvider{
        protected static UserConfig Config => Program.Config.User;

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
                Screen screen;

                if (Config.NotificationDisplay > 0 && Config.NotificationDisplay <= Screen.AllScreens.Length){
                    screen = Screen.AllScreens[Config.NotificationDisplay-1];
                }
                else{
                    screen = Screen.FromControl(owner); // TODO may be disposed?
                }
            
                int edgeDist = Config.NotificationEdgeDistance;

                switch(Config.NotificationPosition){
                    case TweetNotification.Position.TopLeft:
                        return new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+edgeDist);

                    case TweetNotification.Position.TopRight:
                        return new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);

                    case TweetNotification.Position.BottomLeft:
                        return new Point(screen.WorkingArea.X+edgeDist, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);

                    case TweetNotification.Position.BottomRight:
                        return new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);

                    case TweetNotification.Position.Custom:
                        if (!Config.IsCustomNotificationPositionSet){
                            Config.CustomNotificationPosition = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width, screen.WorkingArea.Y+edgeDist);
                            Config.Save();
                        }

                        return Config.CustomNotificationPosition;
                }

                return Location;
            }
        }

        protected bool IsNotificationVisible => Location != ControlExtensions.InvisibleLocation;
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

        public AnalyticsFile AnalyticsFile => owner.AnalyticsFile;
        
        protected override bool ShowWithoutActivation => true;
        
        protected float DpiScale { get; }
        protected double SizeScale => DpiScale*Config.ZoomLevel/100.0;

        protected readonly FormBrowser owner;
        protected readonly ChromiumWebBrowser browser;
        
        private readonly ResourceHandlerNotification resourceHandler = new ResourceHandlerNotification();

        private TweetNotification currentNotification;
        private int pauseCounter;
        
        public string CurrentTweetUrl => currentNotification?.TweetUrl;
        public string CurrentQuoteUrl => currentNotification?.QuoteUrl;

        public bool CanViewDetail => currentNotification != null && !string.IsNullOrEmpty(currentNotification.ColumnId) && !string.IsNullOrEmpty(currentNotification.ChirpId);

        protected bool IsPaused => pauseCounter > 0;
        protected bool IsCursorOverBrowser => browser.Bounds.Contains(PointToClient(Cursor.Position));
        
        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }

        protected FormNotificationBase(FormBrowser owner, bool enableContextMenu){
            InitializeComponent();

            this.owner = owner;
            this.owner.FormClosed += owner_FormClosed;

            ResourceHandlerFactory resourceHandlerFactory = new ResourceHandlerFactory();
            resourceHandlerFactory.RegisterHandler(TwitterUtils.TweetDeckURL, this.resourceHandler);
            resourceHandlerFactory.RegisterHandler(TweetNotification.AppLogo);

            this.browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this, enableContextMenu),
                JsDialogHandler = new JavaScriptDialogHandler(),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBase(false),
                ResourceHandlerFactory = resourceHandlerFactory
            };

            this.browser.Dock = DockStyle.None;
            this.browser.ClientSize = ClientSize;
            this.browser.SetupZoomEvents();

            Controls.Add(browser);

            Disposed += (sender, args) => {
                this.browser.Dispose();
                this.owner.FormClosed -= owner_FormClosed;
            };

            DpiScale = this.GetDPIScale();

            // ReSharper disable once VirtualMemberCallInContructor
            UpdateTitle();
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanDragWindow){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        // event handlers

        private void owner_FormClosed(object sender, FormClosedEventArgs e){
            Close();
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

        protected abstract string GetTweetHTML(TweetNotification tweet);

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
            Text = string.IsNullOrEmpty(title) || !Config.DisplayNotificationColumn ? Program.BrandName : $"{Program.BrandName} - {title}";
        }

        public void ShowTweetDetail(){
            if (currentNotification != null){
                owner.ShowTweetDetail(currentNotification.ColumnId, currentNotification.ChirpId, currentNotification.TweetUrl);
            }
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
