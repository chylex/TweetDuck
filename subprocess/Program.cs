using System;
using System.Diagnostics;
using CefSharp;
using CefSharp.BrowserSubprocess;
using TweetLib.Communication;

namespace TweetDuck.Browser{
    static class Program{
        private static int Main(string[] args){
            SubProcess.EnableHighDPISupport();
            
            const string typePrefix = "--type=";
            string type = Array.Find(args, arg => arg.StartsWith(typePrefix, StringComparison.OrdinalIgnoreCase)).Substring(typePrefix.Length);

            if (type == "renderer"){
                using(RendererProcess subProcess = new RendererProcess(args)){
                    return subProcess.Run();
                }
            }
            else return SubProcess.ExecuteProcess();
        }

        private class RendererProcess : SubProcess{
            public RendererProcess(string[] args) : base(args){}

            public override void OnBrowserCreated(CefBrowserWrapper wrapper){
                base.OnBrowserCreated(wrapper);
                
                using(Process me = Process.GetCurrentProcess()){
                    Comms.BroadcastMessage(Comms.RegisterMessage("TweetDuckSubProcess"), (uint)me.Id, wrapper.BrowserId);
                }
            }
        }
    }
}
