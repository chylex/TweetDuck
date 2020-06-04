using System;
using CefSharp;
using TweetDuck.Browser.Adapters;
using TweetLib.Core.Browser;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Plugins{
    sealed class PluginDispatcher : IPluginDispatcher{
        public event EventHandler<PluginDispatchEventArgs> Ready;

        private readonly IWebBrowser browser;
        private readonly IScriptExecutor executor;
        private readonly Func<string, bool> executeOnUrl;

        public PluginDispatcher(IWebBrowser browser, Func<string, bool> executeOnUrl){
            this.executeOnUrl = executeOnUrl;
            this.browser = browser;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.executor = new CefScriptExecutor(browser);
        }

        void IPluginDispatcher.AttachBridge(string name, object bridge){
            browser.RegisterAsyncJsObject(name, bridge);
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            IFrame frame = e.Frame;

            if (frame.IsMain && executeOnUrl(frame.Url)){
                Ready?.Invoke(this, new PluginDispatchEventArgs(executor));
            }
        }
    }
}
