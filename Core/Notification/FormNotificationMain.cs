using CefSharp;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Other.Interfaces;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification{
    abstract partial class FormNotificationMain : FormNotificationBase, ITweetDeckBrowser{
        private readonly PluginManager plugins;
        private readonly int timerBarHeight;

        protected int timeLeft, totalTime;
        protected bool pausedDuringNotification;
        
        private readonly NativeMethods.HookProc mouseHookDelegate;
        private IntPtr mouseHook;
        private bool blockXButtonUp;

        private bool? prevDisplayTimer;
        private int? prevFontSize;

        public virtual bool RequiresResize{
            get{
                return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Program.UserConfig.DisplayNotificationTimer || prevFontSize != FontSizeLevel;
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
                        return BrowserUtils.Scale(122, SizeScale*(1.0+0.08*FontSizeLevel));

                    case TweetNotification.Size.Custom:
                        return Program.UserConfig.CustomNotificationSize.Height;
                }
            }
        }
        
        public Size BrowserSize => Program.UserConfig.DisplayNotificationTimer ? new Size(ClientSize.Width, ClientSize.Height-timerBarHeight) : ClientSize;

        protected FormNotificationMain(FormBrowser owner, PluginManager pluginManager, bool enableContextMenu) : base(owner, enableContextMenu){
            InitializeComponent();

            this.plugins = pluginManager;
            this.timerBarHeight = BrowserUtils.Scale(4, DpiScale);
            
            browser.KeyboardHandler = new KeyboardHandlerNotification(this);
            browser.RegisterAsyncJsObject("$TD", new TweetDeckBridge.Notification(owner, this));

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            plugins.Register(this, PluginEnvironment.Notification, this);

            mouseHookDelegate = MouseHookProc;
            Disposed += (sender, args) => StopMouseHook(true);
        }

        // implementation of ITweetDeckBrowser

        bool ITweetDeckBrowser.IsTweetDeckWebsite => IsNotificationVisible;

        void ITweetDeckBrowser.RegisterBridge(string name, object obj){
            browser.RegisterAsyncJsObject(name, obj);
        }

        void ITweetDeckBrowser.OnFrameLoaded(Action<IFrame> callback){
            browser.FrameLoadEnd += (sender, args) => {
                IFrame frame = args.Frame;

                if (frame.IsMain && browser.Address != "about:blank"){
                    callback(frame);
                }
            };
        }

        void ITweetDeckBrowser.ExecuteFunction(string name, params object[] args){
            browser.ExecuteScriptAsync(name, args);
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

                if (eventType == NativeMethods.WM_MOUSEWHEEL && IsCursorOverBrowser){
                    if (Arguments.HasFlag(Arguments.ArgNotificationScrollWA)){
                        int delta = BrowserUtils.Scale(NativeMethods.GetMouseHookData(lParam), Program.UserConfig.NotificationScrollSpeed*0.01);
                        browser.ExecuteScriptAsync("window.scrollBy", 0, -Math.Round(delta/0.72));
                    }
                    else{
                        browser.SendMouseWheelEvent(0, 0, 0, BrowserUtils.Scale(NativeMethods.GetMouseHookData(lParam), Program.UserConfig.NotificationScrollSpeed*0.01), CefEventFlags.None);
                    }

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
                    this.InvokeAsyncSafe(AnalyticsFile.NotificationExtraMouseButtons.Trigger);
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
                HideNotification();
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
            if (e.Frame.IsMain && browser.Address != "about:blank"){
                e.Frame.ExecuteJavaScriptAsync(PropertyBridge.GenerateScript(PropertyBridge.Environment.Notification));
                ScriptLoader.ExecuteScript(e.Frame, ScriptLoader.LoadResource("notification.js", this), "root:notification");
            }
        }

        private void timerDisplayDelay_Tick(object sender, EventArgs e){
            OnNotificationReady();
            timerDisplayDelay.Stop();
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer || ContextMenuOpen){
                return;
            }

            timeLeft -= timerProgress.Interval;

            int value = BrowserUtils.Scale(progressBarTimer.Maximum+25, (totalTime-timeLeft)/(double)totalTime);
            progressBarTimer.SetValueInstant(Program.UserConfig.NotificationTimerCountDown ? progressBarTimer.Maximum-value : value);

            if (timeLeft <= 0){
                FinishCurrentNotification();
            }
        }

        // notification methods

        public virtual void ShowNotification(TweetNotification notification){
            LoadTweet(notification);
        }

        public override void HideNotification(){
            base.HideNotification();
            
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;
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

            foreach(InjectedHTML injection in plugins.NotificationInjections){
                html = injection.InjectInto(html);
            }

            return html;
        }

        protected override void LoadTweet(TweetNotification tweet){
            timerProgress.Stop();
            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDurationValue);
            progressBarTimer.Value = Program.UserConfig.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;

            base.LoadTweet(tweet);
        }

        protected override void SetNotificationSize(int width, int height){
            if (Program.UserConfig.DisplayNotificationTimer){
                ClientSize = new Size(width, height+timerBarHeight);
                progressBarTimer.Visible = true;
            }
            else{
                ClientSize = new Size(width, height);
                progressBarTimer.Visible = false;
            }

            browser.ClientSize = new Size(width, height);
        }
        
        protected void PrepareAndDisplayWindow(){
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
