using CefSharp;
using System;

namespace TweetDuck.Core.Handling{
    class BrowserProcessHandler : IBrowserProcessHandler{
        void IBrowserProcessHandler.OnContextInitialized(){
            using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                ctx.SetPreference("browser.enable_spellchecking", Program.UserConfig.EnableSpellCheck, out string _);
            }
        }

        void IBrowserProcessHandler.OnScheduleMessagePumpWork(long delay){}
        void IDisposable.Dispose(){}
    }
}
