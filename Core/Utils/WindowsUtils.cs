using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

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

        public static bool TrySleepUntil(Func<bool> test, int timeoutMillis, int timeStepMillis){
            for(int waited = 0; waited < timeoutMillis; waited += timeStepMillis){
                if (test()){
                    return true;
                }

                Thread.Sleep(timeStepMillis);
            }

            return false;
        }

        public static void ClipboardStripHtmlStyles(){
            if (!Clipboard.ContainsText(TextDataFormat.Html)){
                return;
            }

            string originalText = Clipboard.GetText(TextDataFormat.UnicodeText);
            string originalHtml = Clipboard.GetText(TextDataFormat.Html);

            string updatedHtml = RegexStripHtmlStyles.Replace(originalHtml, string.Empty);

            int removed = originalHtml.Length-updatedHtml.Length;
            updatedHtml = RegexOffsetClipboardHtml.Replace(updatedHtml, match => (int.Parse(match.Value)-removed).ToString().PadLeft(match.Value.Length, '0'));

            DataObject obj = new DataObject();
            obj.SetText(originalText, TextDataFormat.UnicodeText);
            obj.SetText(updatedHtml, TextDataFormat.Html);
            SetClipboardData(obj);
        }

        public static void SetClipboard(string text, TextDataFormat format){
            if (string.IsNullOrEmpty(text)){
                return;
            }

            DataObject obj = new DataObject();
            obj.SetText(text, format);
            SetClipboardData(obj);
        }

        private static void SetClipboardData(DataObject obj){
            try{
                Clipboard.SetDataObject(obj);
            }catch(ExternalException e){
                Program.Reporter.HandleException("Clipboard Error", Program.BrandName+" could not access the clipboard as it is currently used by another process.", true, e);
            }
        }
    }
}
