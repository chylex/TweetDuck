using System;
using System.Collections.Generic;
using TweetDck.Plugins;
using System.Windows.Forms;

namespace TweetDck.Core.Notification{
    sealed class FormNotificationTweet : FormNotificationMain{

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);

        public FormNotificationTweet(FormBrowser owner, PluginManager pluginManager, NotificationFlags flags) : base(owner, pluginManager, flags){
            
            Program.UserConfig.MuteToggled += Config_MuteToggled;
            Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;

            if (Program.UserConfig.MuteNotifications){
                PauseNotification();
            }

            FormClosing += FormNotificationTweet_FormClosing;
        }

        private void FormNotificationTweet_FormClosing(object sender, FormClosingEventArgs e){
            if (e.CloseReason == CloseReason.UserClosing){
                tweetQueue.Clear(); // already canceled
            }
        }

        // event handlers

        private void Config_MuteToggled(object sender, EventArgs e){
            if (Program.UserConfig.MuteNotifications){
                PauseNotification();
            }
            else{
                ResumeNotification();
            }
        }

        // notification methods

        public override void ShowNotification(TweetNotification notification){
            if (IsPaused){
                tweetQueue.Enqueue(notification);
            }
            else{
                tweetQueue.Enqueue(notification);
                UpdateTitle();

                if (totalTime == 0){
                    LoadNextNotification();
                }
            }
        }

        public override void FinishCurrentNotification(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else{
                HideNotification(true);
            }
        }

        public override void ResumeNotification(){
            bool wasPaused = IsPaused;
            base.ResumeNotification();

            if (wasPaused && !IsPaused && !pausedDuringNotification && tweetQueue.Count > 0){
                LoadNextNotification();
            }
        }

        private void LoadNextNotification(){
            LoadTweet(tweetQueue.Dequeue());
        }

        protected override void UpdateTitle(){
            Text = tweetQueue.Count > 0 ? Program.BrandName+" ("+tweetQueue.Count+" more left)" : Program.BrandName;
        }

        protected override void OnNotificationReady(){
            UpdateTitle();
            base.OnNotificationReady();
        }
    }
}
