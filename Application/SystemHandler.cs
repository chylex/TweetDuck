using System;
using System.Diagnostics;
using System.IO;
using TweetLib.Core.Application;

namespace TweetDuck.Application{
    class SystemHandler : IAppSystemHandler{
        void IAppSystemHandler.OpenAssociatedProgram(string path){
            try{
                using(Process.Start(new ProcessStartInfo{
                    FileName = path,
                    ErrorDialog = true
                })){}
            }catch(Exception e){
                Program.Reporter.HandleException("Error Opening Program", "Could not open the associated program for " + path, true, e);
            }
        }

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
