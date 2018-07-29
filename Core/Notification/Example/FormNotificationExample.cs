using System;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Plugins;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification.Example{
    sealed class FormNotificationExample : FormNotificationMain{
        public override bool RequiresResize => true;
        protected override bool CanDragWindow => Config.NotificationPosition == TweetNotification.Position.Custom;

        protected override FormBorderStyle NotificationBorderStyle{
            get{
                if (Config.NotificationSize == TweetNotification.Size.Custom){
                    switch(base.NotificationBorderStyle){
                        case FormBorderStyle.FixedSingle: return FormBorderStyle.Sizable;
                        case FormBorderStyle.FixedToolWindow: return FormBorderStyle.SizableToolWindow;
                    }
                }

                return base.NotificationBorderStyle;
            }
        }

        protected override string BodyClasses => base.BodyClasses+" td-example";

        public event EventHandler Ready;

        private readonly TweetNotification exampleNotification;

        public FormNotificationExample(FormBrowser owner, PluginManager pluginManager) : base(owner, pluginManager, false){
            browser.LoadingStateChanged += browser_LoadingStateChanged;

            string exampleTweetHTML = ScriptLoader.LoadResourceSilent("pages/example.html")?.Replace("{avatar}", TweetNotification.AppLogo.Url) ?? string.Empty;

            #if DEBUG
            exampleTweetHTML = exampleTweetHTML.Replace("</p>", @"</p><div style='margin-top:256px'>Scrollbar test padding...</div>");
            #endif

            exampleNotification = new TweetNotification(string.Empty, string.Empty, "Home", exampleTweetHTML, 176, string.Empty, string.Empty);
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
