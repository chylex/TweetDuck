using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Core.Bridge;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;
using TweetDck.Plugins;
using TweetDck.Plugins.Enums;
using TweetDck.Resources;

namespace TweetDck.Core.Notification{
    partial class FormNotificationMain : FormNotificationBase{
        private const string NotificationScriptFile = "notification.js";

        private static readonly string NotificationScriptIdentifier = ScriptLoader.GetRootIdentifier(NotificationScriptFile);
        private static readonly string PluginScriptIdentifier = ScriptLoader.GetRootIdentifier(PluginManager.PluginNotificationScriptFile);

        private static readonly string NotificationJS, PluginJS;

        static FormNotificationMain(){
            NotificationJS = ScriptLoader.LoadResource(NotificationScriptFile);
            PluginJS = ScriptLoader.LoadResource(PluginManager.PluginNotificationScriptFile);
        }

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
        
        private readonly PluginManager plugins;

        protected int timeLeft, totalTime;
        protected bool pausedDuringNotification;
        
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

        public FormNotificationMain(FormBrowser owner, PluginManager pluginManager, bool enableContextMenu) : base(owner, enableContextMenu){
            InitializeComponent();

            this.plugins = pluginManager;

            browser.RegisterAsyncJsObject("$TD", new TweetDeckBridge(owner, this));
            browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            mouseHookDelegate = MouseHookProc;
            Disposed += (sender, args) => StopMouseHook();
        }

        // mouse wheel hook

        private void StartMouseHook(){
            if (mouseHook == IntPtr.Zero){
                mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WM_MOUSE_LL, mouseHookDelegate, IntPtr.Zero, 0);
            }
        }

        private void StopMouseHook(){
            if (mouseHook != IntPtr.Zero){
                NativeMethods.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam){ System.Diagnostics.Debug.WriteLine(nCode);
            if (nCode == 0 && wParam.ToInt32() == NativeMethods.WM_MOUSEWHEEL && browser.Bounds.Contains(PointToClient(Cursor.Position)) && !ContainsFocus && !owner.ContainsFocus){
                browser.SendMouseWheelEvent(0, 0, 0, NativeMethods.GetHookWheelDelta(lParam), CefEventFlags.None);
                return new IntPtr(-1);
            }

            return NativeMethods.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }

        // event handlers

        private void FormNotification_FormClosing(object sender, FormClosingEventArgs e){
            if (e.CloseReason == CloseReason.UserClosing){
                HideNotification(false);
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
                e.Frame.ExecuteJavaScriptAsync(PropertyBridge.GenerateScript(PropertyBridge.Properties.ExpandLinksOnHover));
                ScriptLoader.ExecuteScript(e.Frame, NotificationJS, NotificationScriptIdentifier);

                if (plugins.HasAnyPlugin(PluginEnvironment.Notification)){
                    ScriptLoader.ExecuteScript(e.Frame, PluginJS, PluginScriptIdentifier);
                    ScriptLoader.ExecuteFile(e.Frame, PluginManager.PluginGlobalScriptFile);
                    plugins.ExecutePlugins(e.Frame, PluginEnvironment.Notification, false);
                }
            }
        }

        private void timerDisplayDelay_Tick(object sender, EventArgs e){
            OnNotificationReady();
            timerDisplayDelay.Stop();
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer || ContextMenuOpen)return;

            timeLeft -= timerProgress.Interval;

            int value = (int)Math.Round(1025.0*(totalTime-timeLeft)/totalTime);
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

            StopMouseHook();
        }

        public override void FinishCurrentNotification(){
            timerProgress.Stop();
        }

        public override void PauseNotification(){
            if (!IsPaused){
                pausedDuringNotification = IsNotificationVisible;
                timerProgress.Stop();
                StopMouseHook();
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
                ClientSize = new Size(width, height+4);
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

        protected override void OnNotificationReady(){
            PrepareAndDisplayWindow();
            timerProgress.Start();
        }
    }
}
