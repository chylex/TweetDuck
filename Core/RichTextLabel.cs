using System;
using System.Windows.Forms;

namespace TweetDick.Core{
    public partial class RichTextLabel : RichTextBox{
        public RichTextLabel(){
            InitializeComponent();

            SetStyle(ControlStyles.Selectable,false);
            SetStyle(ControlStyles.UserMouse,true);
            SetStyle(ControlStyles.SupportsTransparentBackColor,true);
        }

        private void RichTextLabel_MouseEnter(object sender, EventArgs e){
            Cursor = Cursors.Default;
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x204 || m.Msg == 0x205){ // WM_RBUTTONDOWN, WM_RBUTTONUP
                return;
            }

            base.WndProc(ref m);
        }
    }
}
