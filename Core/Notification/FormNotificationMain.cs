using CefSharp;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Adapters;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;

namespace TweetDuck.Core.Notification{
    abstract partial class FormNotificationMain : FormNotificationBase{
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
                return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Config.DisplayNotificationTimer || prevFontSize != FontSizeLevel;
            }

            set{
                if (value){
                    prevDisplayTimer = null;
                    prevFontSize = null;
                }
                else{
                    prevDisplayTimer = Config.DisplayNotificationTimer;
                    prevFontSize = FontSizeLevel;
                }
            }
        }

        private int BaseClientWidth{
            get => Config.NotificationSize switch{
                DesktopNotification.Size.Custom => Config.CustomNotificationSize.Width,
                _ => BrowserUtils.Scale(284, SizeScale * (1.0 + 0.05 * FontSizeLevel))
            };
        }

        private int BaseClientHeight{
            get => Config.NotificationSize switch{
                DesktopNotification.Size.Custom => Config.CustomNotificationSize.Height,
                _ => BrowserUtils.Scale(122, SizeScale * (1.0 + 0.08 * FontSizeLevel))
            };
        }

        protected virtual string BodyClasses => IsCursorOverBrowser ? "td-notification td-hover" : "td-notification";
        
        public Size BrowserSize => Config.DisplayNotificationTimer ? new Size(ClientSize.Width, ClientSize.Height-timerBarHeight) : ClientSize;

        protected FormNotificationMain(FormBrowser owner, PluginManager pluginManager, bool enableContextMenu) : base(owner, enableContextMenu){
            InitializeComponent();

            this.plugins = pluginManager;
            this.timerBarHeight = BrowserUtils.Scale(4, DpiScale);
            
            browser.KeyboardHandler = new KeyboardHandlerNotification(this);
            browser.RegisterAsyncJsObject("$TD", new TweetDeckBridge.Notification(owner, this));

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            plugins.Register(PluginEnvironment.Notification, new PluginDispatcher(browser));

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

                if (eventType == NativeMethods.WM_MOUSEWHEEL && IsCursorOverBrowser){
                    browser.SendMouseWheelEvent(0, 0, 0, BrowserUtils.Scale(NativeMethods.GetMouseHookData(lParam), Config.NotificationScrollSpeed*0.01), CefEventFlags.None);
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
            IFrame frame = e.Frame;

            if (frame.IsMain && browser.Address != "about:blank"){
                frame.ExecuteJavaScriptAsync(PropertyBridge.GenerateScript(PropertyBridge.Environment.Notification));
                CefScriptExecutor.RunFile(frame, "notification.js");
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
            progressBarTimer.SetValueInstant(Config.NotificationTimerCountDown ? progressBarTimer.Maximum-value : value);

            if (timeLeft <= 0){
                FinishCurrentNotification();
            }
        }

        // notification methods

        public virtual void ShowNotification(DesktopNotification notification){
            LoadTweet(notification);
        }

        public override void HideNotification(){
            base.HideNotification();
            
            progressBarTimer.Value = Config.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;
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

        protected override string GetTweetHTML(DesktopNotification tweet){
            string html = tweet.GenerateHtml(BodyClasses, HeadLayout, Config.CustomNotificationCSS);

            foreach(InjectedHTML injection in plugins.NotificationInjections){
                html = injection.InjectInto(html);
            }

            return html;
        }

        protected override void LoadTweet(DesktopNotification tweet){
            timerProgress.Stop();
            totalTime = timeLeft = tweet.GetDisplayDuration(Config.NotificationDurationValue);
            progressBarTimer.Value = Config.NotificationTimerCountDown ? progressBarTimer.Maximum : progressBarTimer.Minimum;

            base.LoadTweet(tweet);
        }

        protected override void SetNotificationSize(int width, int height){
            if (Config.DisplayNotificationTimer){
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
