using System;
using System.Text;

namespace TweetDck.Core.Handling{
    sealed class TweetNotification{
        private static string FontSizeClass { get; set; }
        private static string HeadTag { get; set; }

        private static string DefaultFontSizeClass{
            get{
                return "medium";
            }
        }

        private static string DefaultHeadTag{
            get{
                return @"<meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='chrome=1'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/font.5ef884f9f9.css'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/app-dark.5631e0dd42.css'>";
            }
        }

        private static string CustomCSS{
            get{
                return @".scroll-styled-v::-webkit-scrollbar{width:8px}.scroll-styled-v::-webkit-scrollbar-thumb{border-radius:0}";
            }
        }

        public static int FontSizeLevel{
            get{
                switch(FontSizeClass){
                    case "largest": return 4;
                    case "large": return 3;
                    case "medium": return 2;
                    case "small": return 1;
                    default: return 0;
                }
            }
        }

        public static TweetNotification ExampleTweet{
            get{
                StringBuilder build = new StringBuilder();
                build.Append(@"<article><div class='js-stream-item-content item-box js-show-detail'><div class='js-tweet tweet'>");
                build.Append(@"<header class='tweet-header'>");
                build.Append(@"<time class='tweet-timestamp js-timestamp pull-right txt-mute'><a target='_blank' rel='url' href='https://twitter.com/chylexmc' class='txt-small'>0s</a></time>");
                build.Append(@"<a target='_blank' rel='user' href='https://twitter.com/chylexmc' class='account-link link-complex block'>");
                build.Append(@"<div class='obj-left item-img tweet-img'><img width='48' height='48' alt='chylexmc's avatar' src='https://pbs.twimg.com/profile_images/645532929930608642/J56NBJVY_normal.png' class='tweet-avatar avatar pull-right'></div>");
                build.Append(@"<div class='nbfc'><span class='account-inline txt-ellipsis'><b class='fullname link-complex-target'>chylex</b> <span class='username txt-mute'>@chylexmc</span></span></div>");
                build.Append(@"</a>");
                build.Append(@"</header>");
                build.Append(@"<div class='tweet-body'><p class='js-tweet-text tweet-text with-linebreaks'>This is an example tweet, which lets you test the location and duration of popup notifications.</p></div>");
                build.Append(@"</div></div></article>");

                return new TweetNotification(build.ToString(),"",95);
            }
        }

        public static void SetFontSizeClass(string newFSClass){
            FontSizeClass = newFSClass;
        }

        public static void SetHeadTag(string headContents){
            HeadTag = headContents;
        }

        public enum Position{
            TopLeft, TopRight, BottomLeft, BottomRight, Custom
        }

        public enum Duration{
            Short, Medium, Long, VeryLong
        }

        public string Url{
            get{
                return url;
            }
        }

        private readonly string html;
        private readonly string url;
        private readonly int characters;

        public TweetNotification(string html, string url, int characters){
            this.html = html;
            this.url = url;
            this.characters = characters;
        }

        public int GetDisplayDuration(int value){
            return 2000+Math.Max(1000,value*characters);
        }

        public string GenerateHtml(){
            StringBuilder build = new StringBuilder();
            build.Append("<!DOCTYPE html>");
            build.Append("<html class='os-windows txt-base-").Append(FontSizeClass ?? DefaultFontSizeClass).Append("'>");
            build.Append("<head>").Append(HeadTag ?? DefaultHeadTag).Append("<style type='text/css'>").Append(CustomCSS).Append("</style>");
            
            if (!string.IsNullOrEmpty(Program.UserConfig.CustomNotificationCSS)){
                build.Append("<style type='text/css'>").Append(Program.UserConfig.CustomNotificationCSS).Append("</style>");
            }

            build.Append("</head>");
            build.Append("<body class='hearty'><div class='app-columns-container'><div class='column scroll-styled-v' style='width:100%;overflow-y:auto'>");
            build.Append(html);
            build.Append("</div></div></body>");
            build.Append("</html>");
            return build.ToString();
        }
    }
}
