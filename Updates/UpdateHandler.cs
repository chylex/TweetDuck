using System;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Core;
using TweetDck.Core.Controls;
using TweetDck.Resources;

namespace TweetDck.Updates{
    class UpdateHandler{
        private readonly ChromiumWebBrowser browser;
        private readonly FormBrowser form;

        public event EventHandler<UpdateAcceptedEventArgs> UpdateAccepted;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private int lastEventId;

        public UpdateHandler(ChromiumWebBrowser browser, FormBrowser form){
            this.browser = browser;
            this.form = form;
            browser.FrameLoadEnd += browser_FrameLoadEnd;
            browser.RegisterJsObject("$TDU",new Bridge(this));
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain){
                ScriptLoader.ExecuteFile(e.Frame,"update.js");
            }
        }

        public int Check(bool force){
            browser.ExecuteScriptAsync("TDUF_runUpdateCheck",force,++lastEventId);
            return lastEventId;
        }

        private void TriggerUpdateAcceptedEvent(UpdateAcceptedEventArgs args){
            if (UpdateAccepted != null){
                form.InvokeSafe(() => UpdateAccepted(this,args));
            }
        }

        private void TriggerCheckFinishedEvent(UpdateCheckEventArgs args){
            if (CheckFinished != null){
                form.InvokeSafe(() => CheckFinished(this,args));
            }
        }

        public class Bridge{
            public string BrandName{
                get{
                    return Program.BrandName;
                }
            }

            public string VersionTag{
                get{
                    return Program.VersionTag;
                }
            }

            public bool UpdateCheckEnabled{
                get{
                    return Program.UserConfig.EnableUpdateCheck;
                }
            }

            public string DismissedVersionTag{
                get{
                    return Program.UserConfig.DismissedUpdate ?? string.Empty;
                }
            }

            private readonly UpdateHandler owner;

            public Bridge(UpdateHandler owner){
                this.owner = owner;
            }

            public void OnUpdateCheckFinished(int eventId, bool isUpdateAvailable, string latestVersion){
                owner.TriggerCheckFinishedEvent(new UpdateCheckEventArgs(eventId,isUpdateAvailable,latestVersion));
            }

            public void OnUpdateAccepted(string versionTag, string downloadUrl){
                owner.TriggerUpdateAcceptedEvent(new UpdateAcceptedEventArgs(new UpdateInfo(versionTag,downloadUrl)));
            }

            public void OnUpdateDismissed(string versionTag){
                owner.form.InvokeSafe(() => {
                    Program.UserConfig.DismissedUpdate = versionTag;
                    Program.UserConfig.Save();
                });
            }
        }
    }
}
