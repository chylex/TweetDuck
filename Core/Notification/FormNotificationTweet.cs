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

        protected override Point PrimaryLocation => hasTemporarilyMoved && IsNotificationVisible ? Location : base.PrimaryLocation;
        private bool IsCursorOverNotificationArea => new Rectangle(PrimaryLocation, Size).Contains(Cursor.Position);

        protected override bool CanDragWindow{
            get{
                if (ModifierKeys.HasFlag(Keys.Alt)){
                    hasTemporarilyMoved = true;
                    return true;
                }
                else{
                    return false;
                }
            }
        }

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private bool needsTrim;
        private bool hasTemporarilyMoved;

        public FormNotificationTweet(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, true){
            InitializeComponent();

            Program.UserConfig.MuteToggled += Config_MuteToggled;
            Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;

            if (Program.UserConfig.MuteNotifications){
                PauseNotification();
            }
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x00A7){ // WM_NCMBUTTONDOWN
                int hitTest = m.WParam.ToInt32();

                if (hitTest == 2 || hitTest == 20){ // HTCAPTION, HTCLOSE
                    hasTemporarilyMoved = false;
                    MoveToVisibleLocation();
                    return;
                }
            }

            base.WndProc(ref m);
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
            tweetQueue.Enqueue(notification);
            
            if (!IsPaused){
                UpdateTitle();

                if (totalTime == 0){
                    LoadNextNotification();
                }
            }

            needsTrim |= tweetQueue.Count >= TrimMinimum;
            AnalyticsFile.CountDesktopNotifications.Trigger();
        }

        public override void HideNotification(){
            base.HideNotification();
            tweetQueue.Clear();

            if (needsTrim){
                tweetQueue.TrimExcess();
                needsTrim = false;
            }

            hasTemporarilyMoved = false;
        }

        public override void FinishCurrentNotification(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else{
                HideNotification();
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
