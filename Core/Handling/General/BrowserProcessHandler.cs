using System;
using System.Threading.Tasks;
using CefSharp;
using TweetDuck.Configuration;

namespace TweetDuck.Core.Handling.General{
    sealed class BrowserProcessHandler : IBrowserProcessHandler{
        public static Task UpdatePrefs(){
            return Cef.UIThreadTaskFactory.StartNew(UpdatePrefsInternal);
        }

        private static void UpdatePrefsInternal(){
            UserConfig config = Program.Config.User;

            using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                ctx.SetPreference("browser.enable_spellchecking", config.EnableSpellCheck, out string _);
                ctx.SetPreference("spellcheck.dictionary", config.SpellCheckLanguage, out string _);
                ctx.SetPreference("settings.a11y.animation_policy", config.EnableAnimatedImages ? "allowed" : "none", out string _);
            }
        }

        void IBrowserProcessHandler.OnContextInitialized(){
            UpdatePrefsInternal();
        }

        void IBrowserProcessHandler.OnScheduleMessagePumpWork(long delay){}
        void IDisposable.Dispose(){}
    }
}
