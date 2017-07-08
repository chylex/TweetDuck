using System;
using TweetDuck.Data;

namespace TweetDuck.Configuration{
    static class Arguments{
        // public args
        public const string ArgDataFolder = "-datafolder";
        public const string ArgLocale = "-locale";
        public const string ArgLogging = "-log";
        public const string ArgDebugUpdates = "-debugupdates";

        // internal args
        public const string ArgRestart = "-restart";
        public const string ArgImportCookies = "-importcookies";
        public const string ArgUpdated = "-updated";

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
            args.RemoveFlag(ArgUpdated);
            return args;
        }

        public static CommandLineArgs GetCurrentForInstaller(){
            CommandLineArgs args = GetCurrentClean();
            args.AddFlag(ArgUpdated);
            return args;
        }

        public static string GetCurrentForInstallerCmd(){
            return GetCurrentForInstaller().ToString().Replace("\"", "::");
        }
    }
}
