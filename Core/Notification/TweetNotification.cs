﻿using System;
using System.Text;
using TweetDuck.Core.Bridge;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification{
    sealed class TweetNotification{
        private const string DefaultHeadLayout = @"<html class='os-windows txt-size--14' data-td-font='medium' data-td-theme='dark'><head><meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='chrome=1'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/font.5ef884f9f9.css'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/app-dark.5631e0dd42.css'><style type='text/css'>body{background:#222426}</style>";
        private static readonly string CustomCSS = ScriptLoader.LoadResource("styles/notification.css");

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

        public string GenerateHtml(string bodyClasses = null, bool enableCustomCSS = true){
            StringBuilder build = new StringBuilder();
            build.Append("<!DOCTYPE html>");
            build.Append(TweetDeckBridge.NotificationHeadLayout ?? DefaultHeadLayout);
            
            if (enableCustomCSS){
                build.Append("<style type='text/css'>").Append(CustomCSS).Append("</style>");

                if (!string.IsNullOrEmpty(Program.UserConfig.CustomNotificationCSS)){
                    build.Append("<style type='text/css'>").Append(Program.UserConfig.CustomNotificationCSS).Append("</style>");
                }
            }
            
            build.Append("</head>");
            build.Append("<body class='scroll-styled-v");

            if (!string.IsNullOrEmpty(bodyClasses)){
                build.Append(' ').Append(bodyClasses);
            }

            build.Append('\'').Append(isExample ? " td-example-notification" : "").Append("><div class='column' style='width:100%;height:auto;overflow:initial;'>");
            build.Append(html);
            build.Append("</div></body>");
            build.Append("</html>");
            return build.ToString();
        }
    }
}
