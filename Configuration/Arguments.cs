using System;
using TweetDck.Core.Utils;

namespace TweetDck.Configuration{
    static class Arguments{
        // public args
        public const string ArgDataFolder = "-datafolder";
        public const string ArgLocale = "-locale";
        public const string ArgLogging = "-log";
        public const string ArgDebugUpdates = "-debugupdates";

        // internal args
        public const string ArgRestart = "-restart";
        public const string ArgImportCookies = "-importcookies";

        // class data and methods
        private static readonly CommandLineArgs Current = CommandLineArgs.FromStringArray('-', Environment.GetCommandLineArgs());

        public static bool HasFlag(string flag){
            return Current.HasFlag(flag);
        }

        public static string GetValue(string key, string defaultValue){
            return Current.GetValue(key, defaultValue);
        }

        public static CommandLineArgs GetCurrentClean(){
            CommandLineArgs args = Current.Clone();
            args.RemoveFlag(ArgRestart);
            args.RemoveFlag(ArgImportCookies);
            return args;
        }
    }
}
