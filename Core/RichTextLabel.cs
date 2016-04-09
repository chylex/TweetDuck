using System;
using System.Windows.Forms;

namespace TweetDick.Core{
    public partial class RichTextLabel : RichTextBox{
        /// <summary>
        /// Wraps the body of a RTF formatted string with default tags and formatting.
        /// </summary>
        public static string Wrap(string str){
            string rtf = @"{\rtf1\ansi\ansicpg1250\deff0\deflang1029{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}";
            rtf += @"{\*\generator Msftedit 4.20.69.1337;}\viewkind4\uc1\pard\sa200\sl276\slmult1\lang1036\f0\fs20 ";
            rtf += str;
            return rtf;
        }

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
