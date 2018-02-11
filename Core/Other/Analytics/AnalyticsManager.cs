using System;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;

namespace TweetDuck.Core.Other.Analytics{
    sealed class AnalyticsManager : IDisposable{
        private static readonly TimeSpan CollectionInterval = TimeSpan.FromDays(7);
        private static readonly Uri CollectionUrl = new Uri("https://tweetduck.chylex.com/breadcrumb/report");
        
        public AnalyticsFile File { get; }

        private readonly FormBrowser browser;
        private readonly PluginManager plugins;
        private readonly Timer currentTimer, saveTimer;

        public AnalyticsManager(FormBrowser browser, PluginManager plugins, string file){
            this.browser = browser;
            this.plugins = plugins;

            this.File = AnalyticsFile.Load(file);
            this.File.PropertyChanged += File_PropertyChanged;

            this.currentTimer = new Timer{ SynchronizingObject = browser };
            this.currentTimer.Elapsed += currentTimer_Elapsed;

            this.saveTimer = new Timer{ SynchronizingObject = browser, Interval = 60000 };
            this.saveTimer.Elapsed += saveTimer_Elapsed;

            if (this.File.LastCollectionVersion != Program.VersionTag){
                ScheduleReportIn(TimeSpan.FromHours(12), string.Empty);
            }
            else{
                RestartTimer();
            }
        }

        public void Dispose(){
            File.PropertyChanged -= File_PropertyChanged;

            if (saveTimer.Enabled){
                File.Save();
            }

            currentTimer.Dispose();
            saveTimer.Dispose();
        }

        private void File_PropertyChanged(object sender, EventArgs e){
            saveTimer.Enabled = true;
        }

        private void saveTimer_Elapsed(object sender, ElapsedEventArgs e){
            saveTimer.Stop();
            File.Save();
        }

        private void ScheduleReportIn(TimeSpan delay, string message = null){
            SetLastDataCollectionTime(DateTime.Now.Subtract(CollectionInterval).Add(delay), message);
        }

        private void SetLastDataCollectionTime(DateTime dt, string message = null){
            File.LastDataCollection = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);
            File.LastCollectionVersion = Program.VersionTag;
            File.LastCollectionMessage = message ?? dt.ToString("g", Program.Culture);

            File.Save();
            RestartTimer();
        }

        private void RestartTimer(){
            TimeSpan diff = DateTime.Now.Subtract(File.LastDataCollection);
            int minutesTillNext = (int)(CollectionInterval.TotalMinutes-Math.Floor(diff.TotalMinutes));
            
            currentTimer.Interval = Math.Max(minutesTillNext, 1)*60000;
            currentTimer.Start();
        }

        private void currentTimer_Elapsed(object sender, ElapsedEventArgs e){
            currentTimer.Stop();

            TimeSpan diff = DateTime.Now.Subtract(File.LastDataCollection);
            
            if (Math.Floor(diff.TotalMinutes) >= CollectionInterval.TotalMinutes){
                SendReport();
            }
            else{
                RestartTimer();
            }
        }
        
        private void SendReport(){
            AnalyticsReportGenerator.ExternalInfo info = AnalyticsReportGenerator.ExternalInfo.From(browser);

            Task.Factory.StartNew(() => {
                AnalyticsReport report = AnalyticsReportGenerator.Create(File, info, plugins);

                #if DEBUG
                System.Diagnostics.Debugger.Break();
                #endif

                BrowserUtils.CreateWebClient().UploadValues(CollectionUrl, "POST", report.ToNameValueCollection());
            }).ContinueWith(task => browser.InvokeAsyncSafe(() => {
                if (task.Status == TaskStatus.RanToCompletion){
                    SetLastDataCollectionTime(DateTime.Now);
                }
                else if (task.Exception != null){
                    string message = null;

                    if (task.Exception.InnerException is WebException e){
                        switch(e.Status){
                            case WebExceptionStatus.ConnectFailure:
                                message = "Connection Error";
                                break;

                            case WebExceptionStatus.NameResolutionFailure:
                                message = "DNS Error";
                                break;

                            case WebExceptionStatus.ProtocolError:
                                HttpWebResponse response = e.Response as HttpWebResponse;
                                message = "HTTP Error "+(response != null ? $"{(int)response.StatusCode} ({response.StatusDescription})" : "(unknown code)");
                                break;
                        }
                    }

                    ScheduleReportIn(TimeSpan.FromHours(4), message ?? "Error: "+(task.Exception.InnerException?.Message ?? task.Exception.Message));
                }
            }));
        }
    }
}
