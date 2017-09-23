﻿using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other{
    sealed partial class FormGuide : Form{
        private const string GuideUrl = "https://tweetduck.chylex.com/guide/v1/index.html";

        private readonly ChromiumWebBrowser browser;

        public FormGuide(){
            InitializeComponent();

            Text = Program.BrandName+" Guide";

            FormBrowser owner = FormManager.TryFind<FormBrowser>();

            if (owner != null){
                Size = new Size(owner.Size.Width*3/4, owner.Size.Height*3/4);
                VisibleChanged += (sender, args) => this.MoveToCenter(owner);
            }
            
            this.browser = new ChromiumWebBrowser(GuideUrl){
                MenuHandler = new ContextMenuGuide(),
                JsDialogHandler = new JavaScriptDialogHandler(),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBrowser()
            };

            browser.LoadingStateChanged += browser_LoadingStateChanged;
            browser.FrameLoadStart += browser_FrameLoadStart;
            
            browser.BrowserSettings.BackgroundColor = (uint)BackColor.ToArgb();
            browser.Dock = DockStyle.None;
            browser.Location = ControlExtensions.InvisibleLocation;
            Controls.Add(browser);

            Disposed += (sender, args) => {
                Program.UserConfig.ZoomLevelChanged -= Config_ZoomLevelChanged;
                browser.Dispose();
            };

            Program.UserConfig.ZoomLevelChanged += Config_ZoomLevelChanged;
        }

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading){
                this.InvokeAsyncSafe(() => {
                    browser.Location = Point.Empty;
                    browser.Dock = DockStyle.Fill;
                });

                browser.LoadingStateChanged -= browser_LoadingStateChanged;
            }
        }

        private void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e){
            BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
        }

        private void Config_ZoomLevelChanged(object sender, EventArgs e){
            BrowserUtils.SetZoomLevel(browser.GetBrowser(), Program.UserConfig.ZoomLevel);
        }
    }
}
