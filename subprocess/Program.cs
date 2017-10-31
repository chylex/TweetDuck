using System;
using System.Diagnostics;
using CefSharp;
using CefSharp.BrowserSubprocess;
using System.Runtime.InteropServices;

namespace TweetDuck.Browser{
    static class Program{
        internal const string Version = "1.2.0.0";

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

        private sealed class RendererProcess : SubProcess{
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            public RendererProcess(string[] args) : base(args){}

            public override void OnBrowserCreated(CefBrowserWrapper wrapper){
                base.OnBrowserCreated(wrapper);
                
                using(Process me = Process.GetCurrentProcess()){
                    PostMessage(HWND_BROADCAST, RegisterWindowMessage("TweetDuckSubProcess"), new UIntPtr((uint)me.Id), new IntPtr(wrapper.BrowserId));
                }
            }
        }

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string messageName);
    }
}
