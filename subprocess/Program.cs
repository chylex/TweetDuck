using System;
using CefSharp.BrowserSubprocess;

namespace TweetDuck.Browser{
    static class Program{
        internal const string Version = "1.4.0.0";

        private static int Main(string[] args){
            SubProcess.EnableHighDPISupport();
            
            const string typePrefix = "--type=";
            string type = Array.Find(args, arg => arg.StartsWith(typePrefix, StringComparison.OrdinalIgnoreCase)).Substring(typePrefix.Length);

            if (type == "renderer"){
                using(SubProcess subProcess = new SubProcess(args)){
                    return subProcess.Run();
                }
            }
            else{
                return SubProcess.ExecuteProcess();
            }
        }
    }
}
