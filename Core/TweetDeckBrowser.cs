using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;
using TweetDuck.Resources;

namespace TweetDuck.Core{
    sealed class TweetDeckBrowser : IDisposable{
        private static UserConfig Config => Program.Config.User;

        private const string ErrorUrl = "http://td/error";

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
        private readonly ResourceHandlerFactory resourceHandlerFactory = new ResourceHandlerFactory();

        private string prevSoundNotificationPath = null;

        public TweetDeckBrowser(FormBrowser owner, PluginManager plugins, TweetDeckBridge tdBridge, UpdateBridge updateBridge){
            resourceHandlerFactory.RegisterHandler(TweetNotification.AppLogo);
            resourceHandlerFactory.RegisterHandler(TwitterUtils.LoadingSpinner);

            RequestHandlerBrowser requestHandler = new RequestHandlerBrowser();
            
            this.browser = new ChromiumWebBrowser(TwitterUtils.TweetDeckURL){
                DialogHandler = new FileDialogHandler(),
                DragHandler = new DragHandlerBrowser(requestHandler),
                MenuHandler = new ContextMenuBrowser(owner),
                JsDialogHandler = new JavaScriptDialogHandler(),
                KeyboardHandler = new KeyboardHandlerBrowser(owner),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = requestHandler,
                ResourceHandlerFactory = resourceHandlerFactory
            };

            this.browser.LoadingStateChanged += browser_LoadingStateChanged;
            this.browser.FrameLoadStart += browser_FrameLoadStart;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.browser.LoadError += browser_LoadError;

            this.browser.RegisterAsyncJsObject("$TD", tdBridge);
            this.browser.RegisterAsyncJsObject("$TDU", updateBridge);
            
            this.browser.BrowserSettings.BackgroundColor = (uint)TwitterUtils.BackgroundColor.ToArgb();
            this.browser.Dock = DockStyle.None;
            this.browser.Location = ControlExtensions.InvisibleLocation;
            this.browser.SetupZoomEvents();
            
            owner.Controls.Add(browser);
            plugins.Register(browser, PluginEnvironment.Browser, owner, true);
            
            Config.MuteToggled += Config_MuteToggled;
            Config.SoundNotificationChanged += Config_SoundNotificationInfoChanged;
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
            Config.MuteToggled -= Config_MuteToggled;
            Config.SoundNotificationChanged -= Config_SoundNotificationInfoChanged;
            
            browser.Dispose();
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
                if (TwitterUtils.IsTwitterWebsite(frame)){
                    ScriptLoader.ExecuteFile(frame, "twitter.js", browser);
                }
                
                frame.ExecuteJavaScriptAsync(TwitterUtils.BackgroundColorOverride);
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            IFrame frame = e.Frame;

            if (frame.IsMain){
                if (TwitterUtils.IsTweetDeckWebsite(frame)){
                    UpdateProperties();
                    ScriptLoader.ExecuteFile(frame, "code.js", browser);

                    InjectBrowserCSS();
                    ReinjectCustomCSS(Config.CustomBrowserCSS);
                    Config_SoundNotificationInfoChanged(null, EventArgs.Empty);

                    TweetDeckBridge.ResetStaticProperties();

                    if (Arguments.HasFlag(Arguments.ArgIgnoreGDPR)){
                        ScriptLoader.ExecuteScript(frame, "TD.storage.Account.prototype.requiresConsent = function(){ return false; }", "gen:gdpr");
                    }

                    if (Config.FirstRun){
                        ScriptLoader.ExecuteFile(frame, "introduction.js", browser);
                    }
                }

                ScriptLoader.ExecuteFile(frame, "update.js", browser);
            }

            if (frame.Url == ErrorUrl){
                resourceHandlerFactory.UnregisterHandler(ErrorUrl);
            }
        }

        private void browser_LoadError(object sender, LoadErrorEventArgs e){
            if (e.ErrorCode == CefErrorCode.Aborted){
                return;
            }

            if (!e.FailedUrl.StartsWith("http://td/", StringComparison.Ordinal)){
                string errorPage = ScriptLoader.LoadResourceSilent("pages/error.html");

                if (errorPage != null){
                    resourceHandlerFactory.RegisterHandler(ErrorUrl, ResourceHandler.FromString(errorPage.Replace("{err}", BrowserUtils.GetErrorName(e.ErrorCode))));
                    browser.Load(ErrorUrl);
                }
            }
        }

        private void Config_MuteToggled(object sender, EventArgs e){
            UpdateProperties();
        }

        private void Config_SoundNotificationInfoChanged(object sender, EventArgs e){
            const string soundUrl = "https://ton.twimg.com/tduck/updatesnd";

            bool hasCustomSound = Config.IsCustomSoundNotificationSet;
            string newNotificationPath = Config.NotificationSoundPath;
            
            if (prevSoundNotificationPath != newNotificationPath){
                prevSoundNotificationPath = newNotificationPath;

                if (hasCustomSound){
                    resourceHandlerFactory.RegisterHandler(soundUrl, SoundNotification.CreateFileHandler(newNotificationPath));
                }
                else{
                    resourceHandlerFactory.UnregisterHandler(soundUrl);
                }
            }

            browser.ExecuteScriptAsync("TDGF_setSoundNotificationData", hasCustomSound, Config.NotificationSoundVolume);
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
            browser.ExecuteScriptAsync("TDGF_injectBrowserCSS", ScriptLoader.LoadResource("styles/browser.css", browser)?.TrimEnd() ?? string.Empty);
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

        public void AddSearchColumn(string query){
            browser.ExecuteScriptAsync("TDGF_performSearch", query);
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

        public void ShowUpdateNotification(string versionTag, string releaseNotes){
            browser.ExecuteScriptAsync("TDUF_displayNotification", versionTag, Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(releaseNotes)));
        }

        public void OpenDevTools(){
            browser.ShowDevTools();
        }
    }
}
