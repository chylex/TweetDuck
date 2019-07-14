using System;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Adapters;
using TweetDuck.Core.Utils;
using TweetLib.Core.Browser;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetDuck.Plugins{
    sealed class PluginDispatcher : IPluginDispatcher{
        public event EventHandler<PluginDispatchEventArgs> Ready;

        private readonly IWebBrowser browser;
        private readonly IScriptExecutor executor;

        public PluginDispatcher(Control sync, IWebBrowser browser){
            this.browser = browser;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.executor = new CefScriptExecutor(sync, browser);
        }

        void IPluginDispatcher.AttachBridge(string name, object bridge){
            browser.RegisterAsyncJsObject(name, bridge);
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            IFrame frame = e.Frame;

            if (frame.IsMain && TwitterUtils.IsTweetDeckWebsite(frame)){
                Ready?.Invoke(this, new PluginDispatchEventArgs(executor));
            }
        }
    }
}
