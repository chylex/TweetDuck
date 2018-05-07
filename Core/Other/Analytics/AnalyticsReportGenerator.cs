using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using TweetDuck.Configuration;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Core.Other.Analytics{
    static class AnalyticsReportGenerator{
        public static AnalyticsReport Create(AnalyticsFile file, ExternalInfo info, PluginManager plugins){
            Dictionary<string, string> editLayoutDesign = EditLayoutDesignPluginData;

            return new AnalyticsReport{
                { "App Version"   , Program.VersionTag },
                { "App Type"      , Program.IsPortable ? "portable" : "installed" },
                { "App Dev Tools" , Bool(ContextMenuBase.HasDevTools) },
                0,
                { "System Name"        , SystemName },
                { "System Edition"     , SystemEdition },
                { "System Environment" , Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit" },
                { "System Build"       , SystemBuild },
                { "System Locale"      , Program.Culture.Name.ToLower() },
                0,
                { "RAM" , Exact(RamSize) },
                { "GPU" , GpuVendor },
                0,
                { "Screen Count"      , Exact(Screen.AllScreens.Length) },
                { "Screen Resolution" , info.Resolution ?? "(unknown)" },
                { "Screen DPI"        , info.DPI != null ? Exact(info.DPI.Value) : "(unknown)" },
                0,
                { "Hardware Acceleration"     , Bool(SysConfig.HardwareAcceleration) },
                { "Clear Cache Automatically" , Bool(SysConfig.ClearCacheAutomatically) },
                { "Clear Cache Threshold"     , Exact(SysConfig.ClearCacheThreshold) },
                0,
                { "Expand Links"                  , Bool(UserConfig.ExpandLinksOnHover) },
                { "Search In First Column"        , Bool(UserConfig.OpenSearchInFirstColumn) },
                { "Keep Like Follow Dialogs Open" , Bool(UserConfig.KeepLikeFollowDialogsOpen) },
                { "Best Image Quality"            , Bool(UserConfig.BestImageQuality) },
                { "Animated Images"               , Bool(UserConfig.EnableAnimatedImages) },
                0,
                { "Smooth Scrolling" , Bool(UserConfig.EnableSmoothScrolling) },
                { "Custom Browser"   , CustomBrowser },
                { "Zoom"             , Exact(UserConfig.ZoomLevel) },
                0,
                { "Spell Check"                 , Bool(UserConfig.EnableSpellCheck) },
                { "Spell Check Language"        , UserConfig.SpellCheckLanguage.ToLower() },
                { "Translation Target Language" , UserConfig.TranslationTarget },
                0,
                { "Updates"          , Bool(UserConfig.EnableUpdateCheck) },
                { "Update Dismissed" , Bool(!string.IsNullOrEmpty(UserConfig.DismissedUpdate)) },
                0,
                { "Tray"           , TrayMode },
                { "Tray Highlight" , Bool(UserConfig.EnableTrayHighlight) },
                0,
                { "Notification Position"            , NotificationPosition },
                { "Notification Size"                , NotificationSize },
                { "Notification Timer"               , NotificationTimer },
                { "Notification Timer Speed"         , RoundUp(UserConfig.NotificationDurationValue, 5) },
                { "Notification Scroll Speed"        , Exact(UserConfig.NotificationScrollSpeed) },
                { "Notification Column Title"        , Bool(UserConfig.DisplayNotificationColumn) },
                { "Notification Media Previews"      , Bool(UserConfig.NotificationMediaPreviews) },
                { "Notification Link Skip"           , Bool(UserConfig.NotificationSkipOnLinkClick) },
                { "Notification Non-Intrusive"       , Bool(UserConfig.NotificationNonIntrusiveMode) },
                { "Notification Idle Pause"          , Exact(UserConfig.NotificationIdlePauseSeconds) },
                { "Custom Sound Notification"        , string.IsNullOrEmpty(UserConfig.NotificationSoundPath) ? "off" : Path.GetExtension(UserConfig.NotificationSoundPath) },
                { "Custom Sound Notification Volume" , RoundUp(UserConfig.NotificationSoundVolume, 5) },
                0,
                { "Program Arguments"       , List(ProgramArguments) },
                { "Custom CEF Arguments"    , RoundUp((UserConfig.CustomCefArgs ?? string.Empty).Length, 10) },
                { "Custom Browser CSS"      , RoundUp((UserConfig.CustomBrowserCSS ?? string.Empty).Length, 50) },
                { "Custom Notification CSS" , RoundUp((UserConfig.CustomNotificationCSS ?? string.Empty).Length, 50) },
                0,
                { "Plugins All"     , List(plugins.Plugins.Select(Plugin)) },
                { "Plugins Enabled" , List(plugins.Plugins.Where(plugin => plugins.Config.IsEnabled(plugin)).Select(Plugin)) },
                0,
                { "Theme"               , Dict(editLayoutDesign, "_theme",                  "light/def") },
                { "Column Width"        , Dict(editLayoutDesign, "columnWidth",             "310px/def") },
                { "Font Size"           , Dict(editLayoutDesign, "fontSize",                "12px/def") },
                { "Large Quote Font"    , Dict(editLayoutDesign, "increaseQuoteTextSize",   "false/def") },
                { "Small Compose Font"  , Dict(editLayoutDesign, "smallComposeTextSize",    "false/def") },
                { "Avatar Radius"       , Dict(editLayoutDesign, "avatarRadius",            "2/def") },
                { "Hide Tweet Actions"  , Dict(editLayoutDesign, "hideTweetActions",        "true/def") },
                { "Move Tweet Actions"  , Dict(editLayoutDesign, "moveTweetActionsToRight", "true/def") },
                { "Theme Color Tweaks"  , Dict(editLayoutDesign, "themeColorTweaks",        "true/def") },
                { "Revert Icons"        , Dict(editLayoutDesign, "revertIcons",             "true/def") },
                { "Optimize Animations" , Dict(editLayoutDesign, "optimizeAnimations",      "true/def") },
                { "Reply Account Mode"  , ReplyAccountConfigFromPlugin },
                { "Template Count"      , Exact(TemplateCountFromPlugin) },
                0,
                { "Opened Options"                   , LogRound(file.OpenOptions, 4) },
                { "Opened Plugins"                   , LogRound(file.OpenPlugins, 4) },
                { "Opened About"                     , LogRound(file.OpenAbout, 4) },
                { "Opened Guide"                     , LogRound(file.OpenGuide, 4) },
                { "Desktop Notifications"            , LogRound(file.DesktopNotifications, 5) },
                { "Sound Notifications"              , LogRound(file.SoundNotifications, 5) },
                { "Notification Mutes"               , LogRound(file.NotificationMutes, 2) },
                { "Browser Context Menus"            , LogRound(file.BrowserContextMenus, 2) },
                { "Browser Extra Mouse Buttons"      , LogRound(file.BrowserExtraMouseButtons, 2) },
                { "Notification Context Menus"       , LogRound(file.NotificationContextMenus, 2) },
                { "Notification Extra Mouse Buttons" , LogRound(file.NotificationExtraMouseButtons, 2) },
                { "Notification Keyboard Shortcuts"  , LogRound(file.NotificationKeyboardShortcuts, 2) },
                { "Browser Reloads"                  , LogRound(file.BrowserReloads, 2) },
                { "Copied Usernames"                 , LogRound(file.CopiedUsernames, 2) },
                { "Viewed Images"                    , LogRound(file.ViewedImages, 2) },
                { "Downloaded Images"                , LogRound(file.DownloadedImages, 2) },
                { "Downloaded Videos"                , LogRound(file.DownloadedVideos, 2) },
                { "Used ROT13"                       , LogRound(file.UsedROT13, 2) },
                { "Tweet Screenshots"                , LogRound(file.TweetScreenshots, 2) },
                { "Tweet Details"                    , LogRound(file.TweetDetails, 2) },
                { "Video Plays"                      , LogRound(file.VideoPlays, 4) }
            }.FinalizeReport();
        }

        private static UserConfig UserConfig => Program.UserConfig;
        private static SystemConfig SysConfig => Program.SystemConfig;

        private static string Bool(bool value) => value ? "on" : "off";
        private static string Exact(int value) => value.ToString();
        private static string RoundUp(int value, int multiple) => (multiple*(int)Math.Ceiling((double)value/multiple)).ToString();
        private static string LogRound(int value, int logBase) => (value <= 0 ? 0 : (int)Math.Pow(logBase, Math.Floor(Math.Log(value, logBase)))).ToString();
        private static string Plugin(Plugin plugin) => plugin.Group.GetIdentifierPrefixShort()+plugin.Identifier.Substring(plugin.Group.GetIdentifierPrefix().Length);
        private static string Dict(Dictionary<string, string> dict, string key, string def = "(unknown)") => dict.TryGetValue(key, out string value) ? value : def;
        private static string List(IEnumerable<string> list) => string.Join("|", list.DefaultIfEmpty("(none)"));

        private static string SystemName { get; }
        private static string SystemEdition { get; }
        private static string SystemBuild { get; }
        private static int RamSize { get; }
        private static string GpuVendor { get; }
        private static string[] ProgramArguments { get; }

        static AnalyticsReportGenerator(){
            string osName, osEdition, osBuild;

            try{
                using(RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false)){
                    // ReSharper disable once PossibleNullReferenceException
                    osName = key.GetValue("ProductName") as string;
                    osBuild = key.GetValue("CurrentBuild") as string;
                    osEdition = null;
                    
                    if (osName != null){
                        Match match = Regex.Match(osName, @"^(.*?\d+(?:\.\d+)?) (.*)$");

                        if (match.Success){
                            osName = match.Groups[1].Value;
                            osEdition = match.Groups[2].Value;
                        }
                    }
                }
            }catch{
                osName = osEdition = osBuild = null;
            }

            SystemName = osName ?? "Windows (unknown)";
            SystemEdition = osEdition ?? "(unknown)";
            SystemBuild = osBuild ?? "(unknown)";

            try{
                using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory")){
                    foreach(ManagementBaseObject obj in searcher.Get()){
                        RamSize += (int)((ulong)obj["Capacity"]/(1024L*1024L));
                    }
                }
            }catch{
                RamSize = 0;
            }

            string gpu = null;

            try{
                using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController")){
                    foreach(ManagementBaseObject obj in searcher.Get()){
                        string vendor = obj["Caption"] as string;

                        if (!string.IsNullOrEmpty(vendor)){
                            gpu = vendor;
                        }
                    }
                }
            }catch{
                // rip
            }

            GpuVendor = gpu ?? "(unknown)";

            Dictionary<string, string> args = new Dictionary<string, string>();
            Arguments.GetCurrentClean().ToDictionary(args);
            ProgramArguments = args.Keys.Select(key => key.TrimStart('-')).ToArray();
        }

        private static string CustomBrowser{
            get{
                return Path.GetFileName(UserConfig.BrowserPath) ?? string.Empty;
            }
        }

        private static string TrayMode{
            get{
                switch(UserConfig.TrayBehavior){
                    case TrayIcon.Behavior.DisplayOnly: return "icon";
                    case TrayIcon.Behavior.MinimizeToTray: return "minimize";
                    case TrayIcon.Behavior.CloseToTray: return "close";
                    case TrayIcon.Behavior.Combined: return "combined";
                    default: return "off";
                }
            }
        }

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
                    return UserConfig.NotificationTimerCountDown ? "count down" : "count up";
                }
            }
        }

        private static Dictionary<string, string> EditLayoutDesignPluginData{
            get{
                Dictionary<string, string> dict = new Dictionary<string, string>();
                
                try{
                    string data = File.ReadAllText(Path.Combine(Program.PluginDataPath, "official", "edit-design", "config.json"));

                    foreach(Match match in Regex.Matches(data, "\"(\\w+?)\":(.*?)[,}]")){
                        dict[match.Groups[1].Value] = match.Groups[2].Value.Trim('"');
                    }
                }catch{
                    // rip
                }

                return dict;
            }
        }

        private static int TemplateCountFromPlugin{
            get{
                try{
                    string data = File.ReadAllText(Path.Combine(Program.PluginDataPath, "official", "templates", "config.json"));
                    return Math.Min(StringUtils.CountOccurrences(data, "{\"name\":"), StringUtils.CountOccurrences(data, ",\"contents\":"));
                }catch{
                    return 0;
                }
            }
        }

        private static string ReplyAccountConfigFromPlugin{
            get{
                try{
                    string data = File.ReadAllText(Path.Combine(Program.PluginDataPath, "official", "reply-account", "configuration.js")).Replace(" ", "");

                    Match matchType = Regex.Match(data, "defaultAccount:\"([#@])(.*?)\"(?:,|$)");
                    Match matchAdvanced = Regex.Match(data, "useAdvancedSelector:(.*?)(?:,|$)", RegexOptions.Multiline);

                    if (!matchType.Success){
                        return data.Contains("defaultAccount:\"\"") ? "(legacy)" : "(unknown)";
                    }

                    string accType = matchType.Groups[1].Value == "#" ? matchType.Groups[2].Value : "account";
                    return matchAdvanced.Success && !matchAdvanced.Value.Contains("false") ? "advanced/"+accType : accType;
                }catch{
                    return "(unknown)";
                }
            }
        }

        public class ExternalInfo{
            public static ExternalInfo From(Form form){
                if (form == null){
                    return new ExternalInfo();
                }
                else{
                    Screen screen = Screen.FromControl(form); // works on multi-monitor setups even in tray
                    int dpi;

                    using(Graphics graphics = form.CreateGraphics()){
                        dpi = (int)graphics.DpiY;
                    }

                    return new ExternalInfo{
                        Resolution = screen.Bounds.Width+"x"+screen.Bounds.Height,
                        DPI = dpi
                    };
                }
            }

            public string Resolution { get; private set; }
            public int? DPI { get; private set; }

            private ExternalInfo(){}
        }
    }
}
