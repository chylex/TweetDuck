using System;
using System.Collections.Generic;
using System.Drawing;
using TweetDuck.Plugins;
using System.Windows.Forms;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Notification{
    sealed partial class FormNotificationTweet : FormNotificationMain{
        private const int NonIntrusiveIdleLimit = 30;
        private const int TrimMinimum = 32;

        private bool IsCursorOverNotificationArea => new Rectangle(PrimaryLocation, Size).Contains(Cursor.Position);

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private bool needsTrim;

        public FormNotificationTweet(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, true){
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
                TrimQueue();
            }
        }

        private void TrimQueue(){
            if (needsTrim){
                tweetQueue.TrimExcess();
                needsTrim = false;
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

        private void timerIdlePauseCheck_Tick(object sender, EventArgs e){
            if (NativeMethods.GetIdleSeconds() < Program.UserConfig.NotificationIdlePauseSeconds){
                ResumeNotification();
                timerIdlePauseCheck.Stop();
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

            needsTrim |= tweetQueue.Count >= TrimMinimum;
        }

        public override void FinishCurrentNotification(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else{
                HideNotification(true);
                TrimQueue();
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
            if (!IsNotificationVisible){
                if (Program.UserConfig.NotificationNonIntrusiveMode && IsCursorOverNotificationArea && NativeMethods.GetIdleSeconds() < NonIntrusiveIdleLimit){
                    if (!timerCursorCheck.Enabled){
                        PauseNotification();
                        timerCursorCheck.Start();
                    }

                    return;
                }
                else if (Program.UserConfig.NotificationIdlePauseSeconds > 0 && NativeMethods.GetIdleSeconds() >= Program.UserConfig.NotificationIdlePauseSeconds){
                    if (!timerIdlePauseCheck.Enabled){
                        PauseNotification();
                        timerIdlePauseCheck.Start();
                    }

                    return;
                }
            }
            
            LoadTweet(tweetQueue.Dequeue());
        }

        protected override void UpdateTitle(){
            base.UpdateTitle();

            if (tweetQueue.Count > 0){
                Text = Text+" ("+tweetQueue.Count+" more left)";
            }
        }

        protected override void OnNotificationReady(){
            UpdateTitle();
            base.OnNotificationReady();
        }
    }
}
