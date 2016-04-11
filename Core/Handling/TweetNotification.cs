using System;
using System.Text;

namespace TweetDick.Core.Handling{
    sealed class TweetNotification{
        private static string FontSizeClass { get; set; }
        private static string HeadTag { get; set; }

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

        private readonly string html;
        private readonly int characters;

        public TweetNotification(string html, int characters){
            this.html = html;
            this.characters = characters;
        }

        public int GetDisplayDuration(Duration modifier){
            int multiplier;

            switch(modifier){
                case Duration.Short: multiplier = 40; break;
                case Duration.Long: multiplier = 60; break;
                case Duration.VeryLong: multiplier = 75; break;
                default: multiplier = 50; break;
            }

            return Math.Max(2500,multiplier*characters);
        }

        public string GenerateHtml(){
            StringBuilder build = new StringBuilder();
            build.Append("<!DOCTYPE html>");
            build.Append("<html class='os-windows ").Append(FontSizeClass).Append("'>");
            build.Append("<head>").Append(HeadTag).Append("</head>");
            build.Append("<body class='hearty'><div class='app-columns-container'><div class='column' style='width:100%'>");
            build.Append(html);
            build.Append("</div></div></body>");
            build.Append("</html>");
            return build.ToString();
        }
    }
}
