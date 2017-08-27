using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other.Management;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Notification{
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

            set{
                Visible = (base.Location = value) != ControlExtensions.InvisibleLocation;
                FormBorderStyle = GetBorderStyle(CanResizeWindow);
            }
        }

        public bool CanResizeWindow{
            get => FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow;
            set => FormBorderStyle = GetBorderStyle(value);
        }
        
        public Func<bool> CanMoveWindow { get; set; } = () => true;
        protected override bool ShowWithoutActivation => true;

        protected double SizeScale => dpiScale*Program.UserConfig.ZoomMultiplier;

        protected readonly FormBrowser owner;
        protected readonly ChromiumWebBrowser browser;
        
        private readonly ResourceHandlerNotification resourceHandler = new ResourceHandlerNotification();
        private readonly float dpiScale;

        private TweetNotification currentNotification;
        private int pauseCounter;
        
        public string CurrentTweetUrl => currentNotification?.TweetUrl;
        public string CurrentQuoteUrl => currentNotification?.QuoteUrl;
        public bool IsPaused => pauseCounter > 0;
        
        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }

        public event EventHandler Initialized;

        public FormNotificationBase(FormBrowser owner, bool enableContextMenu){
            InitializeComponent();

            this.owner = owner;
            this.owner.FormClosed += owner_FormClosed;

            this.browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this, enableContextMenu),
                JsDialogHandler = new JavaScriptDialogHandler(),
                LifeSpanHandler = new LifeSpanHandler()
            };

            this.browser.Dock = DockStyle.None;
            this.browser.ClientSize = ClientSize;
            this.browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

            #if DEBUG
            this.browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            this.dpiScale = this.GetDPIScale();

            DefaultResourceHandlerFactory handlerFactory = (DefaultResourceHandlerFactory)browser.ResourceHandlerFactory;
            handlerFactory.RegisterHandler(TwitterUtils.TweetDeckURL, this.resourceHandler);

            Controls.Add(browser);

            Disposed += (sender, args) => {
                this.browser.Dispose();
                this.owner.FormClosed -= owner_FormClosed;
            };

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

                int identifier = browser.GetBrowser().Identifier;
                Disposed += (sender2, args2) => BrowserProcesses.Forget(identifier);
            }
        }

        // notification methods

        public virtual void HideNotification(bool loadBlank){
            if (loadBlank){
                browser.Load("about:blank");
            }

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
            string bodyClasses = browser.Bounds.Contains(PointToClient(Cursor.Position)) ? "td-hover" : string.Empty;
            return tweet.GenerateHtml(bodyClasses);
        }

        protected virtual void LoadTweet(TweetNotification tweet){
            currentNotification = tweet;
            resourceHandler.SetHTML(GetTweetHTML(tweet));
            browser.Load(TwitterUtils.TweetDeckURL);
        }

        protected virtual void SetNotificationSize(int width, int height){
            browser.ClientSize = ClientSize = new Size(BrowserUtils.Scale(width, SizeScale), BrowserUtils.Scale(height, SizeScale));
        }

        protected virtual void OnNotificationReady(){
            MoveToVisibleLocation();
        }

        protected virtual void UpdateTitle(){
            string title = currentNotification?.ColumnTitle;
            Text = string.IsNullOrEmpty(title) || !Program.UserConfig.DisplayNotificationColumn ? Program.BrandName : Program.BrandName+" - "+title;
        }

        public void ShowTweetDetail(){
            owner.ShowTweetDetail(currentNotification.ColumnKey, currentNotification.ChirpId);
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

        private FormBorderStyle GetBorderStyle(bool sizable){
            if (WindowsUtils.ShouldAvoidToolWindow && Visible){ // Visible = workaround for alt+tab
                return sizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }
            else{
                return sizable ? FormBorderStyle.SizableToolWindow : FormBorderStyle.FixedToolWindow;
            }
        }
    }
}
