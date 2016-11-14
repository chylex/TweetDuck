using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TweetDck.Core.Utils{
    static class WindowsUtils{
        public static bool CheckFolderPermission(string path, FileSystemRights right){
            try{
                AuthorizationRuleCollection collection = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(NTAccount));

                foreach(FileSystemAccessRule rule in collection){
                    if ((rule.FileSystemRights & right) == right){
                        return true;
                    }
                }

                return false;
            }
            catch{
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
    }
}
