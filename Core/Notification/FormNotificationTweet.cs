using System;
using System.Collections.Generic;
using System.Drawing;
using TweetDck.Plugins;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification{
    sealed partial class FormNotificationTweet : FormNotificationMain{
        private const int NonIntrusiveIdleLimit = 30;

        private bool IsCursorOverNotificationArea{
            get{
                return new Rectangle(PrimaryLocation, Size).Contains(Cursor.Position);
            }
        }

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);

        public FormNotificationTweet(FormBrowser owner, PluginManager pluginManager, NotificationFlags flags) : base(owner, pluginManager, flags){
            InitializeComponent();

            Program.UserConfig.MuteToggled += Config_MuteToggled;
            Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;

            if (Program.UserConfig.MuteNotifications){
                PauseNotification();
            }
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

        private void timerCursorCheck_Tick(object sender, EventArgs e){
            if (!IsCursorOverNotificationArea){
                ResumeNotification();
                timerCursorCheck.Stop();
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
            if (Program.UserConfig.NotificationNonIntrusiveMode && !IsNotificationVisible && IsCursorOverNotificationArea && NativeMethods.GetIdleSeconds() < NonIntrusiveIdleLimit){
                if (!timerCursorCheck.Enabled){
                    PauseNotification();
                    timerCursorCheck.Start();
                }
            }
            else{
                LoadTweet(tweetQueue.Dequeue());
            }
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
