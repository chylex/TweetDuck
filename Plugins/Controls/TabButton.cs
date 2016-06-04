using System;
using System.Windows.Forms;

namespace TweetDck.Plugins.Controls{
    sealed partial class TabButton : Button{
        protected override bool ShowFocusCues{
            get{
                return false;
            }
        }

        public TabButton(){
            GotFocus += TabButton_GotFocus;
        }

        private void TabButton_GotFocus(object sender, EventArgs e){ // removes extra border when focused
            NotifyDefault(false);
        }
    }
}
