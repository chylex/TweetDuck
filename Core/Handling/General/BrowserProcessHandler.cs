using System;
using CefSharp;

namespace TweetDuck.Core.Handling.General{
    sealed class BrowserProcessHandler : IBrowserProcessHandler{
        public static void UpdatePrefs(){
            Cef.UIThreadTaskFactory.StartNew(UpdatePrefsInternal);
        }

        private static void UpdatePrefsInternal(){
            using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                ctx.SetPreference("browser.enable_spellchecking", Program.UserConfig.EnableSpellCheck, out string _);
            }
        }

        void IBrowserProcessHandler.OnContextInitialized(){
            UpdatePrefsInternal();
        }

        void IBrowserProcessHandler.OnScheduleMessagePumpWork(long delay){}
        void IDisposable.Dispose(){}
    }
}
