using CefSharp;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification{
    partial class FormNotificationMain : FormNotificationBase{
        private const string NotificationScriptFile = "notification.js";
        private const int TimerBarHeight = 4;

        private static readonly string NotificationScriptIdentifier = ScriptLoader.GetRootIdentifier(NotificationScriptFile);
        private static readonly string NotificationJS = ScriptLoader.LoadResource(NotificationScriptFile);
        
        private readonly PluginManager plugins;

        protected int timeLeft, totalTime;
        protected bool pausedDuringNotification;
        
        private readonly NativeMethods.HookProc mouseHookDelegate;
        private IntPtr mouseHook;
        private bool blockXButtonUp;

        private bool? prevDisplayTimer;
        private int? prevFontSize;

        public bool RequiresResize{
            get{
                return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Program.UserConfig.DisplayNotificationTimer || prevFontSize != FontSizeLevel || CanResizeWindow;
            }

            set{
                if (value){
                    prevDisplayTimer = null;
                    prevFontSize = null;
                }
                else{
                    prevDisplayTimer = Program.UserConfig.DisplayNotificationTimer;
                    prevFontSize = FontSizeLevel;
                }
            }
        }

        private int BaseClientWidth{
            get{
                switch(Program.UserConfig.NotificationSize){
                    default:
                        return BrowserUtils.Scale(284, SizeScale*(1.0+0.05*FontSizeLevel));

                    case TweetNotification.Size.Custom:
                        return Program.UserConfig.CustomNotificationSize.Width;
                }
            }
        }

        private int BaseClientHeight{
            get{
                switch(Program.UserConfig.NotificationSize){
                    default:
                        return BrowserUtils.Scale(122, SizeScale*(1.0+0.075*FontSizeLevel));

                    case TweetNotification.Size.Custom:
                        return Program.UserConfig.CustomNotificationSize.Height;
                }
            }
        }

        public Size BrowserSize{
            get => Program.UserConfig.DisplayNotificationTimer ? new Size(ClientSize.Width, ClientSize.Height-TimerBarHeight) : ClientSize;
        }

        public FormNotificationMain(FormBrowser owner, PluginManager pluginManager, bool enableContextMenu) : base(owner, enableContextMenu){
            InitializeComponent();

            this.plugins = pluginManager;
            
            browser.KeyboardHandler = new KeyboardHandlerNotification(this);
            
            browser.RegisterAsyncJsObject("$TD", new TweetDeckBridge(owner, this));
            browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            mouseHookDelegate = MouseHookProc;
            Disposed += (sender, args) => StopMouseHook(true);
        }

        // mouse wheel hook

        private void StartMouseHook(){
            if (mouseHook == IntPtr.Zero){
                mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WM_MOUSE_LL, mouseHookDelegate, IntPtr.Zero, 0);
            }
        }

        private void StopMouseHook(bool force){
            if (mouseHook != IntPtr.Zero && (force || !blockXButtonUp)){
                NativeMethods.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;
                blockXButtonUp = false;
            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam){
            if (nCode == 0){
                int eventType = wParam.ToInt32();

                if (eventType == NativeMethods.WM_MOUSEWHEEL && browser.Bounds.Contains(PointToClient(Cursor.Position))){
                    browser.SendMouseWheelEvent(0, 0, 0, BrowserUtils.Scale(NativeMethods.GetMouseHookData(lParam), Program.UserConfig.NotificationScrollSpeed/100.0), CefEventFlags.None);
                    return NativeMethods.HOOK_HANDLED;
                }
                else if (eventType == NativeMethods.WM_XBUTTONDOWN && DesktopBounds.Contains(Cursor.Position)){
                    int extraButton = NativeMethods.GetMouseHookData(lParam);

                    if (extraButton == 2){ // forward button
                        this.InvokeAsyncSafe(FinishCurrentNotification);
                    }
                    else if (extraButton == 1){ // back button
                        this.InvokeAsyncSafe(Close);
                    }
                    
                    blockXButtonUp = true;
                    return NativeMethods.HOOK_HANDLED;
                }
                else if (eventType == NativeMethods.WM_XBUTTONUP && blockXButtonUp){
                    blockXButtonUp = false;

                    if (!Visible){
                        StopMouseHook(false);
                    }

                    return NativeMethods.HOOK_HANDLED;
                }
            }

            return NativeMethods.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }

        // event handlers

        private void FormNotification_FormClosing(object sender, FormClosingEventArgs e){
            if (e.CloseReason == CloseReason.UserClosing){
                HideNotification(true);
                e.Cancel = true;
            }
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading && browser.Address != "about:blank"){
                this.InvokeSafe(() => {
                    Visible = true; // ensures repaint before moving the window to a visible location
                    timerDisplayDelay.Start();
                });
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && NotificationJS != null && browser.Address != "about:blank"){
                e.Frame.ExecuteJavaScriptAsync(PropertyBridge.GenerateScript(PropertyBridge.Environment.Notification));
                ScriptLoader.ExecuteScript(e.Frame, NotificationJS, NotificationScriptIdentifier);
                plugins.ExecutePlugins(e.Frame, PluginEnvironment.Notification);
            }
        }

        private void timerDisplayDelay_Tick(object sender, EventArgs e){
            OnNotificationReady();
            timerDisplayDelay.Stop();
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer || ContextMenuOpen)return;

            timeLeft -= timerProgress.Interval;

            int value = BrowserUtils.Scale(1025, (totalTime-timeLeft)/(double)totalTime);
            progressBarTimer.SetValueInstant(Math.Min(1000, Math.Max(0, Program.UserConfig.NotificationTimerCountDown ? 1000-value : value)));

            if (timeLeft <= 0){
                FinishCurrentNotification();
            }
        }

        // notification methods

        public virtual void ShowNotification(TweetNotification notification){
            LoadTweet(notification);
        }

        public void ShowNotificationForSettings(bool reset){
            if (reset){
                LoadTweet(TweetNotification.ExampleTweet);
            }
            else{
                PrepareAndDisplayWindow();
            }

            UpdateTitle();
        }

        public override void HideNotification(bool loadBlank){
            base.HideNotification(loadBlank);
            
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? 1000 : 0;
            timerProgress.Stop();
            totalTime = 0;

            StopMouseHook(false);
        }

        public override void FinishCurrentNotification(){
            timerProgress.Stop();
        }

        public override void PauseNotification(){
            if (!IsPaused){
                pausedDuringNotification = IsNotificationVisible;
                timerProgress.Stop();
                StopMouseHook(true);
            }

            base.PauseNotification();
        }

        public override void ResumeNotification(){
            bool wasPaused = IsPaused;
            base.ResumeNotification();

            if (wasPaused && !IsPaused && pausedDuringNotification){
                OnNotificationReady();
            }
        }

        protected override string GetTweetHTML(TweetNotification tweet){
            string html = base.GetTweetHTML(tweet);

            foreach(InjectedHTML injection in plugins.Bridge.NotificationInjections){
                html = injection.Inject(html);
            }

            return html;
        }

        protected override void LoadTweet(TweetNotification tweet){
            timerProgress.Stop();
            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDurationValue);
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? 1000 : 0;

            base.LoadTweet(tweet);
        }

        protected override void SetNotificationSize(int width, int height){
            if (Program.UserConfig.DisplayNotificationTimer){
                ClientSize = new Size(width, height+TimerBarHeight);
                progressBarTimer.Visible = true;
            }
            else{
                ClientSize = new Size(width, height);
                progressBarTimer.Visible = false;
            }

            browser.ClientSize = new Size(width, height);
        }
        
        private void PrepareAndDisplayWindow(){
            if (RequiresResize){
                RequiresResize = false;
                SetNotificationSize(BaseClientWidth, BaseClientHeight);
            }
            
            MoveToVisibleLocation();
            StartMouseHook();
        }

        protected virtual void OnNotificationReady(){
            PrepareAndDisplayWindow();
            timerProgress.Start();
        }
    }
}
