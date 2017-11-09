using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other.Management;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;
using TweetDuck.Plugins.Events;
using TweetDuck.Resources;
using TweetDuck.Updates;

namespace TweetDuck.Core{
    sealed class TweetDeckBrowser : IDisposable{
        public bool Ready { get; private set; }

        public bool Enabled{
            get => browser.Enabled;
            set => browser.Enabled = value;
        }

        public bool IsTweetDeckWebsite{
            get{
                using(IFrame frame = browser.GetBrowser().MainFrame){
                    return TwitterUtils.IsTweetDeckWebsite(frame);
                }
            }
        }

        public event EventHandler PageLoaded;

        private readonly ChromiumWebBrowser browser;
        private readonly PluginManager plugins;
        private readonly MemoryUsageTracker memoryUsageTracker;

        public TweetDeckBrowser(FormBrowser owner, PluginManager plugins, TweetDeckBridge bridge){
            this.browser = new ChromiumWebBrowser(TwitterUtils.TweetDeckURL){
                DialogHandler = new FileDialogHandler(),
                DragHandler = new DragHandlerBrowser(),
                MenuHandler = new ContextMenuBrowser(owner),
                JsDialogHandler = new JavaScriptDialogHandler(),
                KeyboardHandler = new KeyboardHandlerBrowser(owner),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBrowser()
            };

            #if DEBUG
            this.browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            this.browser.LoadingStateChanged += browser_LoadingStateChanged;
            this.browser.FrameLoadStart += browser_FrameLoadStart;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.browser.LoadError += browser_LoadError;

            this.browser.RegisterAsyncJsObject("$TD", bridge);
            this.browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);
            
            this.browser.BrowserSettings.BackgroundColor = (uint)TwitterUtils.BackgroundColor.ToArgb();
            this.browser.Dock = DockStyle.None;
            this.browser.Location = ControlExtensions.InvisibleLocation;

            owner.Controls.Add(browser);

            this.plugins = plugins;
            this.plugins.PluginChangedState += plugins_PluginChangedState;

            this.memoryUsageTracker = new MemoryUsageTracker("TDGF_tryRunCleanup");

            Program.UserConfig.MuteToggled += UserConfig_MuteToggled;
            Program.UserConfig.ZoomLevelChanged += UserConfig_ZoomLevelChanged;
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
            plugins.PluginChangedState -= plugins_PluginChangedState;

            Program.UserConfig.MuteToggled -= UserConfig_MuteToggled;
            Program.UserConfig.ZoomLevelChanged -= UserConfig_ZoomLevelChanged;

            memoryUsageTracker.Dispose();
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
            if (e.Frame.IsMain){
                memoryUsageTracker.Stop();

                if (Program.UserConfig.ZoomLevel != 100){
                    BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
                }

                if (TwitterUtils.IsTwitterWebsite(e.Frame)){
                    ScriptLoader.ExecuteFile(e.Frame, "twitter.js");
                }
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && TwitterUtils.IsTweetDeckWebsite(e.Frame)){
                e.Frame.ExecuteJavaScriptAsync(TwitterUtils.BackgroundColorFix);

                UpdateProperties();
                TweetDeckBridge.RestoreSessionData(e.Frame);
                ScriptLoader.ExecuteFile(e.Frame, "code.js");
                InjectBrowserCSS();
                ReinjectCustomCSS(Program.UserConfig.CustomBrowserCSS);
                plugins.ExecutePlugins(e.Frame, PluginEnvironment.Browser);

                TweetDeckBridge.ResetStaticProperties();

                if (Program.SystemConfig.EnableBrowserGCReload){
                    memoryUsageTracker.Start(browser, Program.SystemConfig.BrowserMemoryThreshold);
                }

                if (Program.UserConfig.FirstRun){
                    ScriptLoader.ExecuteFile(e.Frame, "introduction.js");
                }

                PageLoaded?.Invoke(this, EventArgs.Empty);
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

        private void plugins_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            browser.ExecuteScriptAsync("TDPF_setPluginState", e.Plugin, e.IsEnabled);
        }

        private void UserConfig_MuteToggled(object sender, EventArgs e){
            UpdateProperties();
        }

        private void UserConfig_ZoomLevelChanged(object sender, EventArgs e){
            BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
        }

        // external handling

        public UpdateHandler CreateUpdateHandler(UpdaterSettings settings){
            return new UpdateHandler(browser, settings);
        }

        public void RefreshMemoryTracker(){
            if (Program.SystemConfig.EnableBrowserGCReload){
                memoryUsageTracker.Start(browser, Program.SystemConfig.BrowserMemoryThreshold);
            }
            else{
                memoryUsageTracker.Stop();
            }
        }

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

        public void ApplyROT13(){
            browser.ExecuteScriptAsync("TDGF_applyROT13()");
        }
    }
}
