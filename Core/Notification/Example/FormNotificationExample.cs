﻿using System;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
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

        public event EventHandler Ready;

        private readonly TweetNotification exampleNotification;

        public FormNotificationExample(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, false){
            browser.LoadingStateChanged += browser_LoadingStateChanged;

            string exampleTweetHTML = ScriptLoader.LoadResource("pages/example.html", true)?.Replace("{avatar}", TweetNotification.AppLogo.Url) ?? string.Empty;

            #if DEBUG
            exampleTweetHTML = exampleTweetHTML.Replace("</p>", @"</p><div style='margin-top:256px'>Scrollbar test padding...</div>");
            #endif

            exampleNotification = TweetNotification.Example(exampleTweetHTML, 176);
        }

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading){
                Ready?.Invoke(this, EventArgs.Empty);
                browser.LoadingStateChanged -= browser_LoadingStateChanged;
            }
        }

        public override void HideNotification(){
            Location = ControlExtensions.InvisibleLocation;
        }

        public override void FinishCurrentNotification(){}

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
