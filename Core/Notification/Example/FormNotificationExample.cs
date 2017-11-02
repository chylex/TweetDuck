﻿using System.Windows.Forms;
using TweetDuck.Plugins;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification.Example{
    sealed class FormNotificationExample : FormNotificationMain{
        public override bool RequiresResize => true;
        protected override bool CanDragWindow => Program.UserConfig.NotificationPosition == TweetNotification.Position.Custom;
        
        protected override FormBorderStyle NotificationBorderStyle{
            get{
                if (Program.UserConfig.NotificationSize == TweetNotification.Size.Custom){
                    switch(base.NotificationBorderStyle){
                        case FormBorderStyle.FixedSingle: return FormBorderStyle.Sizable;
                        case FormBorderStyle.FixedToolWindow: return FormBorderStyle.SizableToolWindow;
                    }
                }

                return base.NotificationBorderStyle;
            }
        }

        private readonly TweetNotification exampleNotification;

        public FormNotificationExample(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, false){
            string exampleTweetHTML = ScriptLoader.LoadResource("pages/example.html", true);

            #if DEBUG
            exampleTweetHTML = exampleTweetHTML.Replace("</p>", @"</p><div style='margin-top:256px'>Scrollbar test padding...</div>");
            #endif

            exampleNotification = TweetNotification.Example(exampleTweetHTML, 95);
        }

        public void ShowExampleNotification(bool reset){
            if (reset){
                LoadTweet(exampleNotification);
            }
            else{
                PrepareAndDisplayWindow();
            }

            UpdateTitle();
        }
    }
}
