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

        private readonly string html;

        public TweetNotification(string html){
            this.html = html;
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
