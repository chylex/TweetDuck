using System;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Controls{
    public partial class RichTextLabel : RichTextBox{
        /// <summary>
        /// Wraps the body of a RTF formatted string with default tags and formatting.
        /// </summary>
        public static string Wrap(string str){
            string rtf = @"{\rtf1\ansi\ansicpg1250\deff0\deflang1029{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}";
            rtf += @"{\*\generator Msftedit 4.20.69.1337;}\viewkind4\uc1\pard\sa200\sl276\slmult1\lang1036\f0\fs16 ";
            rtf += str;
            return rtf;
        }

        /// <summary>
        /// Wraps URL tags around a link.
        /// </summary>
        public static string AddLink(string url){
            return @"{\field{\*\fldinst{HYPERLINK """+url+@"""}}{\fldrslt{\ul\cf1 "+url+@"}}}";
        }

        /// <summary>
        /// Uses v5 of RichTextBox, which fixes URLs and other crap.
        /// </summary>
        protected override CreateParams CreateParams{
            get{
                CreateParams createParams = base.CreateParams;

                if (NativeMethods.LoadLibrary("msftedit.dll") != IntPtr.Zero){
                    createParams.ClassName = "RICHEDIT50W";
                }

                return createParams;
            }
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
