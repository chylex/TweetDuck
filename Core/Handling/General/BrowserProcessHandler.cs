using System;
using System.Threading.Tasks;
using CefSharp;

namespace TweetDuck.Core.Handling.General{
    sealed class BrowserProcessHandler : IBrowserProcessHandler{
        public static Task UpdatePrefs(){
            return Cef.UIThreadTaskFactory.StartNew(UpdatePrefsInternal);
        }

        private static void UpdatePrefsInternal(){
            using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                ctx.SetPreference("browser.enable_spellchecking", Program.UserConfig.EnableSpellCheck, out string _);
                ctx.SetPreference("settings.a11y.animation_policy", Program.UserConfig.EnableAnimatedImages ? "allowed" : "none", out string _);
            }
        }

        void IBrowserProcessHandler.OnContextInitialized(){
            UpdatePrefsInternal();
        }

        void IBrowserProcessHandler.OnScheduleMessagePumpWork(long delay){}
        void IDisposable.Dispose(){}
    }
}
