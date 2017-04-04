using System;
using System.Text;
using TweetDck.Resources;

namespace TweetDck.Core.Notification{
    sealed class TweetNotification{
        private static string FontSizeClass { get; set; }
        private static string HeadTag { get; set; }

        private const string DefaultFontSizeClass = "medium";
        private const string DefaultHeadTag = @"<meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='chrome=1'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/font.5ef884f9f9.css'><link rel='stylesheet' href='https://ton.twimg.com/tweetdeck-web/web/css/app-dark.5631e0dd42.css'>";
        private const string CustomCSS = @"body:before{content:none}body{overflow-y:auto}.scroll-styled-v::-webkit-scrollbar{width:7px}.scroll-styled-v::-webkit-scrollbar-thumb{border-radius:0}.scroll-styled-v::-webkit-scrollbar-track{border-left:0}#td-skip{opacity:0;cursor:pointer;transition:opacity 0.15s ease}.td-hover #td-skip{opacity:0.75}#td-skip:hover{opacity:1}";

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

        private static string ExampleTweetHTML;

        public static TweetNotification ExampleTweet{
            get{
                if (ExampleTweetHTML == null){
                    ExampleTweetHTML = ScriptLoader.LoadResource("pages/example.html", true);

                    #if DEBUG
                    ExampleTweetHTML = ExampleTweetHTML.Replace("</p>", @"</p><div style='margin-top:64px'>Scrollbar test padding...</div>");
                    #endif
                }

                return new TweetNotification(ExampleTweetHTML, "", 95, true);
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

        public string Url{
            get{
                return url;
            }
        }

        private readonly string html;
        private readonly string url;
        private readonly int characters;
        private readonly bool isExample;

        public TweetNotification(string html, string url, int characters) : this(html, url, characters, false){}

        private TweetNotification(string html, string url, int characters, bool isExample){
            this.html = html;
            this.url = url;
            this.characters = characters;
            this.isExample = isExample;
        }

        public int GetDisplayDuration(int value){
            return 2000+Math.Max(1000, value*characters);
        }

        public string GenerateHtml(string bodyClasses = null, bool enableCustomCSS = true){
            StringBuilder build = new StringBuilder();
            build.Append("<!DOCTYPE html>");
            build.Append("<html class='os-windows txt-base-").Append(FontSizeClass ?? DefaultFontSizeClass).Append("'>");
            build.Append("<head>").Append(HeadTag ?? DefaultHeadTag);
            
            if (enableCustomCSS){
                build.Append("<style type='text/css'>").Append(CustomCSS).Append("</style>");

                if (!string.IsNullOrEmpty(Program.UserConfig.CustomNotificationCSS)){
                    build.Append("<style type='text/css'>").Append(Program.UserConfig.CustomNotificationCSS).Append("</style>");
                }
            }
            
            build.Append("</head>");
            build.Append("<body class='hearty scroll-styled-v");

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
