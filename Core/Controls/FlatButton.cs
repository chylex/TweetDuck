using System;
using System.Windows.Forms;

namespace TweetDuck.Core.Controls{
    sealed class FlatButton : Button{
        protected override bool ShowFocusCues => false;

        public FlatButton(){
            GotFocus += FlatButton_GotFocus;
        }

        private void FlatButton_GotFocus(object sender, EventArgs e){ // removes extra border when focused
            NotifyDefault(false);
        }
    }
}
