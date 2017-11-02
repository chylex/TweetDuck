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
        
        public string LastCollectionMessage => file.LastCollectionMessage;
        
        private readonly FormBrowser browser;
        private readonly PluginManager plugins;
        private readonly AnalyticsFile file;
        private readonly Timer currentTimer;

        public AnalyticsManager(FormBrowser browser, PluginManager plugins, string file){
            this.browser = browser;
            this.plugins = plugins;
            this.file = AnalyticsFile.Load(file);

            this.currentTimer = new Timer{ SynchronizingObject = browser };
            this.currentTimer.Elapsed += currentTimer_Elapsed;

            if (this.file.LastCollectionVersion != Program.VersionTag){
                ScheduleReportIn(TimeSpan.FromHours(12), string.Empty);
            }
            else{
                RestartTimer();
            }
        }

        public void Dispose(){
            currentTimer.Dispose();
        }

        private void ScheduleReportIn(TimeSpan delay, string message = null){
            SetLastDataCollectionTime(DateTime.Now.Subtract(CollectionInterval).Add(delay), message);
        }

        private void SetLastDataCollectionTime(DateTime dt, string message = null){
            file.LastDataCollection = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);
            file.LastCollectionVersion = Program.VersionTag;
            file.LastCollectionMessage = message ?? dt.ToString("g", Program.Culture);

            file.Save();
            RestartTimer();
        }

        private void RestartTimer(){
            TimeSpan diff = DateTime.Now.Subtract(file.LastDataCollection);
            int minutesTillNext = (int)(CollectionInterval.TotalMinutes-Math.Floor(diff.TotalMinutes));
            
            currentTimer.Interval = Math.Max(minutesTillNext, 1)*60000;
            currentTimer.Start();
        }

        private void currentTimer_Elapsed(object sender, ElapsedEventArgs e){
            currentTimer.Stop();

            TimeSpan diff = DateTime.Now.Subtract(file.LastDataCollection);
            
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
                AnalyticsReport report = AnalyticsReportGenerator.Create(info, plugins);
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
