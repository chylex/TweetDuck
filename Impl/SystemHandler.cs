using System.Diagnostics;
using System.IO;
using TweetLib.Core.Application;

namespace TweetDuck.Impl{
    class SystemHandler : IAppSystemHandler{
        void IAppSystemHandler.OpenFileExplorer(string path){
            if (File.Exists(path)){
                using(Process.Start("explorer.exe", "/select,\"" + path.Replace('/', '\\') + "\"")){}
            }
            else if (Directory.Exists(path)){
                using(Process.Start("explorer.exe", '"' + path.Replace('/', '\\') + '"')){}
            }
        }
    }
}
