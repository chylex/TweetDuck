using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using TweetDuck.Configuration;
using System.Linq;
using TweetDuck.Core.Notification;

namespace TweetDuck.Core.Other.Analytics{
    static class AnalyticsReportGenerator{
        public static AnalyticsReport Create(Control triggerer){
            return new AnalyticsReport{
                { "App Version" , Program.VersionTag },
                { "App Type"    , Program.IsPortable ? "portable" : "installed" },
                0,
                { "System Name"        , SystemName },
                { "System Edition"     , SystemEdition },
                { "System Environment" , Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit" },
                { "System Build"       , SystemBuild },
                0,
                { "Screen Count", Exact(Screen.AllScreens.Length) },
                { "Screen DPI" , Exact(GetDPI(triggerer)) },
                0,
                { "Hardware Acceleration"       , Bool(SysConfig.HardwareAcceleration) },
                { "Browser GC Reload"           , Bool(SysConfig.EnableBrowserGCReload) },
                { "Browser GC Reload Threshold" , Exact(SysConfig.BrowserMemoryThreshold) },
                0,
                { "Expand Links"             , Bool(UserConfig.ExpandLinksOnHover) },
                { "Switch Account Selectors" , Bool(UserConfig.SwitchAccountSelectors) },
                { "Search In First Column"   , Bool(UserConfig.OpenSearchInFirstColumn) },
                { "Best Image Quality"       , Bool(UserConfig.BestImageQuality) },
                { "Spell Check"              , Bool(UserConfig.EnableSpellCheck) },
                { "Zoom"                     , Exact(UserConfig.ZoomLevel) },
                0,
                { "Updates"       , Bool(UserConfig.EnableUpdateCheck) },
                { "Has Dismissed" , Bool(!string.IsNullOrEmpty(UserConfig.DismissedUpdate)) },
                0,
                { "Tray"           , Exact((int)UserConfig.TrayBehavior) },
                { "Tray Highlight" , Bool(UserConfig.EnableTrayHighlight) },
                0,
                { "Notification Position"       , NotificationPosition },
                { "Notification Size"           , NotificationSize },
                { "Notification Timer"          , NotificationTimer },
                { "Notification Timer Speed"    , RoundUp(UserConfig.NotificationDurationValue, 5) },
                { "Notification Scroll Speed"   , Exact(UserConfig.NotificationScrollSpeed) },
                { "Notification Column Title"   , Bool(UserConfig.DisplayNotificationColumn) },
                { "Notification Media Previews" , Bool(UserConfig.NotificationMediaPreviews) },
                { "Notification Link Skip"      , Bool(UserConfig.NotificationSkipOnLinkClick) },
                { "Notification Non-Intrusive"  , Bool(UserConfig.NotificationNonIntrusiveMode) },
                { "Notification Idle Pause"     , Exact(UserConfig.NotificationIdlePauseSeconds) },
                { "Custom Sound Notification"   , string.IsNullOrEmpty(UserConfig.NotificationSoundPath) ? "off" : Path.GetExtension(UserConfig.NotificationSoundPath) },
                0,
                { "Program Arguments"       , ProgramArguments },
                { "Custom CEF Arguments"    , RoundUp((UserConfig.CustomCefArgs ?? string.Empty).Length, 10) },
                { "Custom Browser CSS"      , RoundUp((UserConfig.CustomBrowserCSS ?? string.Empty).Length, 50) },
                { "Custom Notification CSS" , RoundUp((UserConfig.CustomNotificationCSS ?? string.Empty).Length, 50) },
            }.FinalizeReport();
        }

        private static UserConfig UserConfig => Program.UserConfig;
        private static SystemConfig SysConfig => Program.SystemConfig;

        private static string Bool(bool value) => value ? "on" : "off";
        private static string Exact(int value) => value.ToString();
        private static string RoundUp(int value, int multiple) => (multiple*(int)Math.Ceiling((double)value/multiple)).ToString();

        private static string SystemName { get; }
        private static string SystemEdition { get; }
        private static string SystemBuild { get; }
        private static string ProgramArguments { get; }

        private static string NotificationPosition{
            get{
                switch(UserConfig.NotificationPosition){
                    case TweetNotification.Position.TopLeft: return "top left";
                    case TweetNotification.Position.TopRight: return "top right";
                    case TweetNotification.Position.BottomLeft: return "bottom left";
                    case TweetNotification.Position.BottomRight: return "bottom right";
                    default: return "custom";
                }
            }
        }

        private static string NotificationSize{
            get{
                switch(UserConfig.NotificationSize){
                    case TweetNotification.Size.Auto: return "auto";
                    default: return RoundUp(UserConfig.CustomNotificationSize.Width, 20)+"x"+RoundUp(UserConfig.CustomNotificationSize.Height, 20);
                }
            }
        }

        private static string NotificationTimer{
            get{
                if (!UserConfig.DisplayNotificationTimer){
                    return "off";
                }
                else{
                    return UserConfig.NotificationTimerCountDown ? "countdown" : "countup";
                }
            }
        }

        static AnalyticsReportGenerator(){
            string osName, osEdition, osBuild;

            try{
                using(RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false)){
                    // ReSharper disable once PossibleNullReferenceException
                    osName = key.GetValue("ProductName") as string;
                    osEdition = key.GetValue("EditionID") as string;
                    osBuild = key.GetValue("CurrentBuild") as string;
                    
                    if (osName != null && osEdition != null){
                        osName = osName.Replace(osEdition, "").TrimEnd();
                    }
                }
            }catch{
                osName = osEdition = osBuild = null;
            }

            SystemName = osName ?? "Windows (unknown)";
            SystemEdition = osEdition ?? "(unknown)";
            SystemBuild = osBuild ?? "(unknown)";

            Dictionary<string, string> args = new Dictionary<string, string>();
            Arguments.GetCurrentClean().ToDictionary(args);
            ProgramArguments = string.Join(", ", args.Keys.Select(key => key.TrimStart('-')));
        }

        private static int GetDPI(Control control){
            using(Graphics graphics = control.CreateGraphics()){
                return (int)graphics.DpiY;
            }
        }
    }
}
