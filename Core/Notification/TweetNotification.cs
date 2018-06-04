using System;
using System.Text;
using CefSharp;
using TweetDuck.Core.Bridge;
using TweetDuck.Data;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification{
    sealed class TweetNotification{
        private const string DefaultHeadLayout = @"<html class=""scroll-v os-windows dark txt-size--14"" lang=""en-US"" id=""tduck"" data-td-font=""medium"" data-td-theme=""dark""><head><meta charset=""utf-8""><link href=""https://ton.twimg.com/tweetdeck-web/web/dist/bundle.4b1f87e09d.css"" rel=""stylesheet""><style type='text/css'>body { background: rgb(34, 36, 38) !important }</style>";
        private static readonly string CustomCSS = ScriptLoader.LoadResource("styles/notification.css") ?? string.Empty;
        public static readonly ResourceLink AppLogo = new ResourceLink("https://ton.twimg.com/tduck/avatar", ResourceHandler.FromByteArray(Properties.Resources.avatar, "image/png"));

        public static TweetNotification Example(string html, int characters){
            return new TweetNotification(string.Empty, string.Empty, "Home", html, characters, string.Empty, string.Empty, true);
        }

        public enum Position{
            TopLeft, TopRight, BottomLeft, BottomRight, Custom
        }

        public enum Size{
            Auto, Custom
        }

        public string ColumnId { get; }
        public string ChirpId { get; }

        public string ColumnTitle { get; }
        public string TweetUrl { get; }
        public string QuoteUrl { get; }
        
        private readonly string html;
        private readonly int characters;
        private readonly bool isExample;

        public TweetNotification(string columnId, string chirpId, string title, string html, int characters, string tweetUrl, string quoteUrl) : this(columnId, chirpId, title, html, characters, tweetUrl, quoteUrl, false){}

        private TweetNotification(string columnId, string chirpId, string title, string html, int characters, string tweetUrl, string quoteUrl, bool isExample){
            this.ColumnId = columnId;
            this.ChirpId = chirpId;

            this.ColumnTitle = title;
            this.TweetUrl = tweetUrl;
            this.QuoteUrl = quoteUrl;

            this.html = html;
            this.characters = characters;
            this.isExample = isExample;
        }

        public int GetDisplayDuration(int value){
            return 2000+Math.Max(1000, value*characters);
        }

        public string GenerateHtml(string bodyClasses){
            StringBuilder build = new StringBuilder();
            build.Append("<!DOCTYPE html>");
            build.Append(TweetDeckBridge.NotificationHeadLayout ?? DefaultHeadLayout);
            build.Append("<style type='text/css'>").Append(CustomCSS).Append("</style>");

            if (!string.IsNullOrEmpty(Program.UserConfig.CustomNotificationCSS)){
                build.Append("<style type='text/css'>").Append(Program.UserConfig.CustomNotificationCSS).Append("</style>");
            }
            
            build.Append("</head>");
            build.Append("<body class='scroll-styled-v");

            if (!string.IsNullOrEmpty(bodyClasses)){
                build.Append(' ').Append(bodyClasses);
            }

            build.Append('\'').Append(isExample ? " td-example-notification" : "").Append("><div class='column' style='width:100%!important;min-height:100vh!important;height:auto!important;overflow:initial!important;'>");
            build.Append(html);
            build.Append("</div></body>");
            build.Append("</html>");
            return build.ToString();
        }
    }
}
