using System;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Bridge{
    sealed class CallbackBridge{
        private readonly Control owner;
        private readonly Action safeCallback;

        public CallbackBridge(Control owner, Action safeCallback){
            this.owner = owner;
            this.safeCallback = safeCallback;
        }

        public void Trigger(){
            owner.InvokeSafe(safeCallback);
        }
    }
}
