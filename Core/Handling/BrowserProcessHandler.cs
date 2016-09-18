using CefSharp;
using System;

namespace TweetDck.Core.Handling{
    class BrowserProcessHandler : IBrowserProcessHandler{
        void IBrowserProcessHandler.OnContextInitialized(){
            using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                string err;
                ctx.SetPreference("browser.enable_spellchecking", false, out err);
            }
        }

        void IBrowserProcessHandler.OnScheduleMessagePumpWork(long delay){}
        void IDisposable.Dispose(){}
    }
}
