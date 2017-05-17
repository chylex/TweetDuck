using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;

namespace TweetDuck.Core.Bridge{
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
