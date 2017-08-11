using System.Collections.Generic;
using System.Diagnostics;
using CefSharp;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Management{
    static class BrowserProcesses{
        private static readonly Dictionary<int, int> PIDs = new Dictionary<int, int>();

        public static void Link(int identifier, int pid){
            PIDs[identifier] = pid;
        }

        public static void Forget(int identifier){
            PIDs.Remove(identifier);
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
