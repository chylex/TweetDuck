using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp.BrowserSubprocess;

namespace TweetDuck.Browser{
    static class Program{
        private static int Main(string[] args){
            SubProcess.EnableHighDPISupport();
            
            string FindArg(string key){
                return Array.Find(args, arg => arg.StartsWith(key, StringComparison.OrdinalIgnoreCase)).Substring(key.Length);
            }

            const string typePrefix = "--type=";
            const string parentIdPrefix = "--host-process-id=";

            if (!int.TryParse(FindArg(parentIdPrefix), out int parentId)){
                return 0;
            }

            Task.Factory.StartNew(() => KillWhenHung(parentId), TaskCreationOptions.LongRunning);
            
            if (FindArg(typePrefix) == "renderer"){
                using SubProcess subProcess = new SubProcess(null, args);
                return subProcess.Run();
            }
            else{
                return SubProcess.ExecuteProcess(args);
            }
        }

        private static async void KillWhenHung(int parentId){
            try{
                using Process process = Process.GetProcessById(parentId);
                process.WaitForExit();
            }catch{
                // ded
            }

            await Task.Delay(10000);
            Environment.Exit(0);
        }
    }
}
