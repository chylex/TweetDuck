using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;

namespace TweetDuck.Core.Notification.Screenshot{
    sealed class ScreenshotBridge{
        private readonly Control owner;

        private readonly Action<int> safeSetHeight;
        private readonly Action safeTriggerScreenshot;

        public ScreenshotBridge(Control owner, Action<int> safeSetHeight, Action safeTriggerScreenshot){
            this.owner = owner;
            this.safeSetHeight = safeSetHeight;
            this.safeTriggerScreenshot = safeTriggerScreenshot;
        }

        public void SetHeight(int tweetHeight){
            owner.InvokeSafe(() => safeSetHeight(tweetHeight));
        }

        public void TriggerScreenshot(){
            owner.InvokeSafe(safeTriggerScreenshot);
        }
    }
}
