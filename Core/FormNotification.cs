using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Resources;
using TweetDck.Core.Utils;
using TweetDck.Core.Utils.Notification;
using TweetDck.Plugins;
using TweetDck.Plugins.Enums;
using TweetDck.Core.Controls;

namespace TweetDck.Core{
    sealed partial class FormNotification : Form{
        private const string NotificationScriptFile = "notification.js";

        private static readonly string NotificationScriptIdentifier = ScriptLoader.GetRootIdentifier(NotificationScriptFile);
        private static readonly string PluginScriptIdentifier = ScriptLoader.GetRootIdentifier(PluginManager.PluginNotificationScriptFile);

        public Func<bool> CanMoveWindow = () => true;

        public bool IsNotificationVisible{
            get{
                return Location != ControlExtensions.InvisibleLocation;
            }
        }

        private readonly Control owner;
        private readonly PluginManager plugins;
        private readonly ChromiumWebBrowser browser;
        private readonly NotificationFlags flags;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private int timeLeft, totalTime;
        
        private readonly NativeMethods.HookProc mouseHookDelegate;
        private IntPtr mouseHook;

        private bool? prevDisplayTimer;
        private int? prevFontSize;

        private bool RequiresResize{
            get{
                return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Program.UserConfig.DisplayNotificationTimer || prevFontSize != TweetNotification.FontSizeLevel;
            }

            set{
                if (value){
                    prevDisplayTimer = null;
                    prevFontSize = null;
                }
                else{
                    prevDisplayTimer = Program.UserConfig.DisplayNotificationTimer;
                    prevFontSize = TweetNotification.FontSizeLevel;
                }
            }
        }

        private readonly string notificationJS;
        private readonly string pluginJS;

        protected override bool ShowWithoutActivation{
            get{
                return true;
            }
        }

        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }
        public string CurrentUrl { get; private set; }
        public string CurrentQuotedTweetUrl { get; set; }

        public EventHandler Initialized;
        private bool isInitialized;

        private static int BaseClientWidth{
            get{
                int level = TweetNotification.FontSizeLevel;
                return level == 0 ? 284 : (int)Math.Round(284.0*(1.0+0.05*level));
            }
        }

        private static int BaseClientHeight{
            get{
                int level = TweetNotification.FontSizeLevel;
                return level == 0 ? 118 : (int)Math.Round(118.0*(1.0+0.075*level));
            }
        }

        public FormNotification(FormBrowser owner, PluginManager pluginManager, NotificationFlags flags){
            InitializeComponent();

            Text = Program.BrandName;

            this.owner = owner;
            this.plugins = pluginManager;
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
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            if (!flags.HasFlag(NotificationFlags.DisableScripts)){
                notificationJS = ScriptLoader.LoadResource(NotificationScriptFile);
                browser.RegisterJsObject("$TD", new TweetDeckBridge(owner, this));

                if (plugins != null){
                    pluginJS = ScriptLoader.LoadResource(PluginManager.PluginNotificationScriptFile);
                    browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);
                }
            }

            panelBrowser.Controls.Add(browser);

            if (flags.HasFlag(NotificationFlags.AutoHide)){
                Program.UserConfig.MuteToggled += Config_MuteToggled;
                Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;
            }

            mouseHookDelegate = MouseHookProc;

            Disposed += FormNotification_Disposed;
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanMoveWindow()){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        // mouse wheel hook

        private void StartMouseHook(){
            if (mouseHook == IntPtr.Zero){
                mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, mouseHookDelegate, IntPtr.Zero, 0);
            }
        }

        private void StopMouseHook(){
            if (mouseHook != IntPtr.Zero){
                NativeMethods.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam){
            if (!ContainsFocus && wParam.ToInt32() == NativeMethods.WH_MOUSEWHEEL && browser.Bounds.Contains(PointToClient(Cursor.Position))){
                // fuck it, Activate() doesn't work with this
                Point prevPos = Cursor.Position;
                Cursor.Position = PointToScreen(new Point(0, -1));
                NativeMethods.SimulateMouseClick(NativeMethods.MouseButton.Left);
                Cursor.Position = prevPos;
            }

            return NativeMethods.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }

        // event handlers

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer || ContextMenuOpen)return;

            timeLeft -= timerProgress.Interval;

            int value = (int)Math.Round(1025.0*(totalTime-timeLeft)/totalTime);
            progressBarTimer.SetValueInstant(Math.Min(1000, Math.Max(0, Program.UserConfig.NotificationTimerCountDown ? 1000-value : value)));

            if (timeLeft <= 0){
                FinishCurrentTweet();
            }
        }

        private void Config_MuteToggled(object sender, EventArgs e){
            if (Program.UserConfig.MuteNotifications){
                HideNotification(true);
            }
            else if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
        }

        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e){
            if (e.IsBrowserInitialized && Initialized != null){
                Initialized(this, new EventArgs());
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (!e.Frame.IsMain)return;

            if (!isInitialized && !Program.UserConfig.NotificationLegacyLoad){
                isInitialized = true;

                if (Initialized != null){
                    Initialized(this, new EventArgs());
                }
            }
            else if (notificationJS != null && browser.Address != "about:blank" && !flags.HasFlag(NotificationFlags.DisableScripts)){
                ScriptLoader.ExecuteScript(e.Frame, notificationJS, NotificationScriptIdentifier);

                if (plugins != null && plugins.HasAnyPlugin(PluginEnvironment.Notification)){
                    ScriptLoader.ExecuteScript(e.Frame, pluginJS, PluginScriptIdentifier);
                    ScriptLoader.ExecuteFile(e.Frame, PluginManager.PluginGlobalScriptFile);
                    plugins.ExecutePlugins(e.Frame, PluginEnvironment.Notification, false);
                }
            }
        }

        private void FormNotification_FormClosing(object sender, FormClosingEventArgs e){
            if (e.CloseReason == CloseReason.UserClosing){
                HideNotification(false);
                tweetQueue.Clear();
                e.Cancel = true;
            }
        }

        private void FormNotification_Disposed(object sender, EventArgs e){
            browser.Dispose();
            StopMouseHook();
        }

        // notification methods

        public void ShowNotification(TweetNotification notification){
            if (Program.UserConfig.MuteNotifications){
                tweetQueue.Enqueue(notification);
            }
            else{
                tweetQueue.Enqueue(notification);
                UpdateTitle();

                if (totalTime == 0){
                    LoadNextNotification();
                }
            }
        }

        public void ShowNotificationForSettings(bool reset){
            if (reset){
                LoadTweet(TweetNotification.ExampleTweet);
            }
            else{
                PrepareAndDisplayWindow();
            }
        }

        public void PrepareNotificationForScreenshot(Action callback){
            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new CallbackBridge(this, callback));

            browser.FrameLoadEnd += (sender, args) => {
                if (args.Frame.IsMain && browser.Address != "about:blank"){
                    ScriptLoader.ExecuteScript(args.Frame, "window.setTimeout(() => $TD_NotificationScreenshot.trigger(), 25)", "gen:screenshot");
                }
            };
        }

        public void LoadNotificationForScreenshot(TweetNotification tweet, int width, int height){
            browser.LoadHtml(tweet.GenerateHtml(enableCustomCSS: false), "http://tweetdeck.twitter.com/?"+DateTime.Now.Ticks);
            
            Location = ControlExtensions.InvisibleLocation;
            ClientSize = new Size(width, height);
            progressBarTimer.Visible = false;
            panelBrowser.Height = height;
        }

        public void TakeScreenshotAndHide(){
            MoveToVisibleLocation();
            Activate();
            SendKeys.SendWait("%{PRTSC}");
            
            Location = ControlExtensions.InvisibleLocation;
            browser.LoadHtml("", "about:blank");
        }

        public void HideNotification(bool loadBlank){
            if (loadBlank || Program.UserConfig.NotificationLegacyLoad){
                browser.LoadHtml("", "about:blank");
            }

            Location = ControlExtensions.InvisibleLocation;
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? 1000 : 0;
            timerProgress.Stop();
            totalTime = 0;

            StopMouseHook();
        }

        public void OnNotificationReady(){
            UpdateTitle();
            PrepareAndDisplayWindow();
            timerProgress.Start();
        }

        public void FinishCurrentTweet(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else if (flags.HasFlag(NotificationFlags.AutoHide)){
                HideNotification(true);
            }
            else{
                timerProgress.Stop();
            }
        }

        private void LoadNextNotification(){
            LoadTweet(tweetQueue.Dequeue());
        }

        private void LoadTweet(TweetNotification tweet){
            CurrentUrl = tweet.Url;
            CurrentQuotedTweetUrl = string.Empty; // load from JS
            
            timerProgress.Stop();
            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDurationValue);
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? 1000 : 0;

            string bodyClasses = browser.Bounds.Contains(PointToClient(Cursor.Position)) ? "td-hover" : string.Empty;

            browser.LoadHtml(tweet.GenerateHtml(bodyClasses), "http://tweetdeck.twitter.com/?"+DateTime.Now.Ticks);

            if (Program.UserConfig.NotificationLegacyLoad){
                OnNotificationReady();
            }
        }

        private void MoveToVisibleLocation(){
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

        private void PrepareAndDisplayWindow(){
            if (RequiresResize){
                RequiresResize = false;

                if (Program.UserConfig.DisplayNotificationTimer){
                    ClientSize = new Size(BaseClientWidth, BaseClientHeight+4);
                    progressBarTimer.Visible = true;
                }
                else{
                    ClientSize = new Size(BaseClientWidth, BaseClientHeight);
                    progressBarTimer.Visible = false;
                }

                panelBrowser.Height = BaseClientHeight;
            }
            
            MoveToVisibleLocation();
            StartMouseHook();
        }

        private void UpdateTitle(){
            Text = tweetQueue.Count > 0 ? Program.BrandName+" ("+tweetQueue.Count+" more left)" : Program.BrandName;
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
