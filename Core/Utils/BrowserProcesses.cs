using System.Collections.Generic;
using System.Diagnostics;
using CefSharp;

namespace TweetDuck.Core.Utils{
    static class BrowserProcesses{
        private static readonly Dictionary<int, int> PIDs = new Dictionary<int, int>();

        public static void Link(int identifier, int pid){
            PIDs[identifier] = pid;
        }

        public static void Forget(int identifier){
            PIDs.Remove(identifier);
            Debug.WriteLine("rip "+identifier);
        }
        
        public static Process FindProcess(IBrowser browser){
            if (PIDs.TryGetValue(browser.Identifier, out int pid) && WindowsUtils.IsChildProcess(pid)){ // child process is checked in two places for safety
                return Process.GetProcessById(pid);
            }
            else{
                return null;
            }
        }
    }
}
