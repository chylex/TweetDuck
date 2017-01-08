using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TweetDck.Core.Utils{
    static class WindowsUtils{
        private static readonly Regex RegexStripHtmlStyles = new Regex(@"\s?(?:style|class)="".*?""");
        private static readonly Regex RegexOffsetClipboardHtml = new Regex(@"(?<=EndHTML:|EndFragment:)(\d+)");

        public static bool CheckFolderWritePermission(string path){
            string testFile = Path.Combine(path, ".test");

            try{
                Directory.CreateDirectory(path);

                using(File.Create(testFile)){}
                File.Delete(testFile);
                return true;
            }catch{
                return false;
            }
        }

        public static Process StartProcess(string file, string arguments, bool runElevated){
            ProcessStartInfo processInfo = new ProcessStartInfo{
                FileName = file,
                Arguments = arguments
            };

            if (runElevated){
                processInfo.Verb = "runas";
            }

            return Process.Start(processInfo);
        }

        public static Timer CreateSingleTickTimer(int timeout){
            Timer timer = new Timer{
                Interval = timeout
            };

            timer.Tick += (sender, args) => timer.Stop();
            return timer;
        }

        public static void ClipboardStripHtmlStyles(){
            if (!Clipboard.ContainsText(TextDataFormat.Html)){
                return;
            }

            string original = Clipboard.GetText(TextDataFormat.Html);
            string updated = RegexStripHtmlStyles.Replace(original, string.Empty);

            int removed = original.Length-updated.Length;
            updated = RegexOffsetClipboardHtml.Replace(updated, match => (int.Parse(match.Value)-removed).ToString().PadLeft(match.Value.Length, '0'));

            Clipboard.SetText(updated, TextDataFormat.Html);
        }
    }
}
