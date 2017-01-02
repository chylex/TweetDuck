using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TweetDck.Core.Utils{
    static class WindowsUtils{
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
    }
}
