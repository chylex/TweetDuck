using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;

namespace TweetDuck.Core{
    sealed class TweetDeckBrowser : ITweetDeckBrowser, IDisposable{
        public bool Ready { get; private set; }

        public bool Enabled{
            get => browser.Enabled;
            set => browser.Enabled = value;
        }

        public bool IsTweetDeckWebsite{
            get{
                if (!Ready){
                    return false;
                }

                using(IFrame frame = browser.GetBrowser().MainFrame){
                    return TwitterUtils.IsTweetDeckWebsite(frame);
                }
            }
        }
        
        private readonly ChromiumWebBrowser browser;

        private string prevSoundNotificationPath = null;

        public TweetDeckBrowser(FormBrowser owner, TweetDeckBridge bridge){
            this.browser = new ChromiumWebBrowser(TwitterUtils.TweetDeckURL){
                DialogHandler = new FileDialogHandler(),
                DragHandler = new DragHandlerBrowser(),
                MenuHandler = new ContextMenuBrowser(owner),
                JsDialogHandler = new JavaScriptDialogHandler(),
                KeyboardHandler = new KeyboardHandlerBrowser(owner),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBrowser()
            };

            this.browser.LoadingStateChanged += browser_LoadingStateChanged;
            this.browser.FrameLoadStart += browser_FrameLoadStart;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.browser.LoadError += browser_LoadError;

            this.browser.RegisterAsyncJsObject("$TD", bridge);
            
            this.browser.BrowserSettings.BackgroundColor = (uint)TwitterUtils.BackgroundColor.ToArgb();
            this.browser.Dock = DockStyle.None;
            this.browser.Location = ControlExtensions.InvisibleLocation;

            this.browser.SetupResourceHandler(TweetNotification.AppLogo);
            this.browser.SetupResourceHandler(TwitterUtils.LoadingSpinner);

            owner.Controls.Add(browser);
            
            Program.UserConfig.MuteToggled += UserConfig_MuteToggled;
            Program.UserConfig.ZoomLevelChanged += UserConfig_ZoomLevelChanged;
            Program.UserConfig.SoundNotificationChanged += UserConfig_SoundNotificationInfoChanged;
        }

        // setup and management

        private void OnBrowserReady(){
            if (!Ready){
                browser.Location = Point.Empty;
                browser.Dock = DockStyle.Fill;
                Ready = true;
            }
        }

        public void Focus(){
            browser.Focus();
        }

        public void Dispose(){
            Program.UserConfig.MuteToggled -= UserConfig_MuteToggled;
            Program.UserConfig.ZoomLevelChanged -= UserConfig_ZoomLevelChanged;
            Program.UserConfig.SoundNotificationChanged -= UserConfig_SoundNotificationInfoChanged;
            
            browser.Dispose();
        }
        
        void ITweetDeckBrowser.RegisterBridge(string name, object obj){
            browser.RegisterAsyncJsObject(name, obj);
        }

        void ITweetDeckBrowser.OnFrameLoaded(Action<IFrame> callback){
            browser.FrameLoadEnd += (sender, args) => {
                IFrame frame = args.Frame;

                if (frame.IsMain && TwitterUtils.IsTweetDeckWebsite(frame)){
                    callback(frame);
                }
            };
        }

        void ITweetDeckBrowser.ExecuteFunction(string name, params object[] args){
            browser.ExecuteScriptAsync(name, args);
        }

        // event handlers

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading){
                foreach(string word in TwitterUtils.DictionaryWords){
                    browser.AddWordToDictionary(word);
                }

                browser.BeginInvoke(new Action(OnBrowserReady));
                browser.LoadingStateChanged -= browser_LoadingStateChanged;
            }
        }

        private void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e){
            IFrame frame = e.Frame;

            if (frame.IsMain){
                if (Program.UserConfig.ZoomLevel != 100){
                    BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
                }

                if (TwitterUtils.IsTwitterWebsite(frame)){
                    ScriptLoader.ExecuteFile(frame, "twitter.js");
                }
                
                frame.ExecuteJavaScriptAsync(TwitterUtils.BackgroundColorOverride);
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            IFrame frame = e.Frame;

            if (frame.IsMain && TwitterUtils.IsTweetDeckWebsite(frame)){
                UpdateProperties();
                TweetDeckBridge.RestoreSessionData(frame);
                ScriptLoader.ExecuteFile(frame, "code.js");
                InjectBrowserCSS();
                ReinjectCustomCSS(Program.UserConfig.CustomBrowserCSS);
                UserConfig_SoundNotificationInfoChanged(null, EventArgs.Empty);

                TweetDeckBridge.ResetStaticProperties();

                if (Program.UserConfig.FirstRun){
                    ScriptLoader.ExecuteFile(frame, "introduction.js");
                }
            }
        }

        private void browser_LoadError(object sender, LoadErrorEventArgs e){
            if (e.ErrorCode == CefErrorCode.Aborted){
                return;
            }

            if (!e.FailedUrl.StartsWith("http://td/", StringComparison.Ordinal)){
                string errorPage = ScriptLoader.LoadResource("pages/error.html", true);

                if (errorPage != null){
                    browser.LoadHtml(errorPage.Replace("{err}", BrowserUtils.GetErrorName(e.ErrorCode)), "http://td/error");
                }
            }
        }

        private void UserConfig_MuteToggled(object sender, EventArgs e){
            UpdateProperties();
        }

        private void UserConfig_ZoomLevelChanged(object sender, EventArgs e){
            BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
        }

        private void UserConfig_SoundNotificationInfoChanged(object sender, EventArgs e){
            const string soundUrl = "https://ton.twimg.com/tduck/updatesnd";
            bool hasCustomSound = Program.UserConfig.IsCustomSoundNotificationSet;

            if (prevSoundNotificationPath != Program.UserConfig.NotificationSoundPath){
                browser.SetupResourceHandler(soundUrl, hasCustomSound ? SoundNotification.CreateFileHandler(Program.UserConfig.NotificationSoundPath) : null);
                prevSoundNotificationPath = Program.UserConfig.NotificationSoundPath;
            }

            browser.ExecuteScriptAsync("TDGF_setSoundNotificationData", hasCustomSound, Program.UserConfig.NotificationSoundVolume);
        }

        // external handling

        public void HideVideoOverlay(bool focus){
            if (focus){
                browser.GetBrowser().GetHost().SendFocusEvent(true);
            }

            browser.ExecuteScriptAsync("$('#td-video-player-overlay').remove()");
        }

        // javascript calls

        public void ReloadToTweetDeck(){
            browser.ExecuteScriptAsync($"if(window.TDGF_reload)window.TDGF_reload();else window.location.href='{TwitterUtils.TweetDeckURL}'");
        }

        public void UpdateProperties(){
            browser.ExecuteScriptAsync(PropertyBridge.GenerateScript(PropertyBridge.Environment.Browser));
        }

        public void InjectBrowserCSS(){
            browser.ExecuteScriptAsync("TDGF_injectBrowserCSS", ScriptLoader.LoadResource("styles/browser.css").TrimEnd());
        }

        public void ReinjectCustomCSS(string css){
            browser.ExecuteScriptAsync("TDGF_reinjectCustomCSS", css?.Replace(Environment.NewLine, " ") ?? string.Empty);
        }

        public void OnMouseClickExtra(IntPtr param){
            browser.ExecuteScriptAsync("TDGF_onMouseClickExtra", (param.ToInt32() >> 16) & 0xFFFF);
        }

        public void ShowTweetDetail(string columnId, string chirpId, string fallbackUrl){
            browser.ExecuteScriptAsync("TDGF_showTweetDetail", columnId, chirpId, fallbackUrl);
        }

        public void TriggerTweetScreenshot(){
            browser.ExecuteScriptAsync("TDGF_triggerScreenshot()");
        }

        public void ReloadColumns(){
            browser.ExecuteScriptAsync("TDGF_reloadColumns()");
        }

        public void PlaySoundNotification(){
            browser.ExecuteScriptAsync("TDGF_playSoundNotification()");
        }

        public void ApplyROT13(){
            browser.ExecuteScriptAsync("TDGF_applyROT13()");
        }
    }
}
