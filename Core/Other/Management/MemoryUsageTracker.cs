using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Timer = System.Timers.Timer;

namespace TweetDuck.Core.Other.Management{
    sealed class MemoryUsageTracker : IDisposable{
        private const int IntervalMemoryCheck = 60000*30; // 30 minutes
        private const int IntervalCleanupAttempt = 60000*5; // 5 minutes

        private readonly string script;
        private readonly Timer timer;
        private Form owner;
        private IBrowser browser;

        private long threshold;
        private bool needsCleanup;

        public MemoryUsageTracker(string cleanupFunctionName){
            this.script = $"window.{cleanupFunctionName} && window.{cleanupFunctionName}()";

            this.timer = new Timer{ Interval = IntervalMemoryCheck };
            this.timer.Elapsed += timer_Elapsed;
        }

        public void Start(ChromiumWebBrowser control, int thresholdMB){
            Stop();

            this.owner = (Form)control.Parent; // TODO ugly
            this.browser = control.GetBrowser();
            this.threshold = thresholdMB*1024L*1024L;
            this.timer.SynchronizingObject = owner;
            this.timer.Start();
        }

        public void Stop(){
            timer.Stop();
            timer.SynchronizingObject = null;
            owner = null;
            browser = null;
            SetNeedsCleanup(false);
        }

        public void Dispose(){
            timer.SynchronizingObject = null;
            timer.Dispose();
            owner = null;
            browser = null;
        }

        private void SetNeedsCleanup(bool value){
            if (needsCleanup != value){
                needsCleanup = value;
                timer.Interval = value ? IntervalCleanupAttempt : IntervalMemoryCheck; // restarts timer
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e){
            if (owner == null || browser == null){
                return;
            }
            
            if (needsCleanup){
                if (!owner.ContainsFocus){
                    using(IFrame frame = browser.MainFrame){
                        frame.EvaluateScriptAsync(script).ContinueWith(task => {
                            JavascriptResponse response = task.Result;

                            if (response.Success && (response.Result as bool? ?? false)){
                                SetNeedsCleanup(false);
                            }
                        });
                    }
                }
            }
            else{
                try{
                    using(Process process = BrowserProcesses.FindProcess(browser)){
                        if (process?.PrivateMemorySize64 > threshold){
                            SetNeedsCleanup(true);
                        }
                    }
                }catch{
                    // ignore I guess?
                }
            }
        }
    }
}
