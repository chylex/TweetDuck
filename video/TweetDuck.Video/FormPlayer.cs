using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace TweetDuck.Video{
    partial class FormPlayer : Form{
        private readonly IntPtr ownerHandle;
        private readonly string videoUrl;

        public FormPlayer(IntPtr handle, string url){
            InitializeComponent();

            this.ownerHandle = handle;
            this.videoUrl = url;

            player.Ocx.enableContextMenu = false;
            player.Ocx.uiMode = "none";
            player.Ocx.settings.setMode("loop", true);

            player.Ocx.MediaChange += player_MediaChange;
            player.Ocx.MediaError += player_MediaError;
        }

        private void FormPlayer_Load(object sender, EventArgs e){
            player.Ocx.URL = videoUrl;
        }

        private void timer_Tick(object sender, EventArgs e){
            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect) && rect.Top != -32000){
                int width = rect.Right-rect.Left+1;
                int height = rect.Bottom-rect.Top+1;
                IWMPMedia media = player.Ocx.currentMedia;

                ClientSize = new Size(Math.Min(media.imageSourceWidth, width*3/4), Math.Min(media.imageSourceHeight, height*3/4));
                Location = new Point(rect.Left+(width-ClientSize.Width)/2, rect.Top+(height-ClientSize.Height+SystemInformation.CaptionHeight)/2);
            }
            else{
                Environment.Exit(Program.CODE_OWNER_GONE);
            }
        }

        private void player_MediaChange(object item){
            timer.Start();
            Cursor.Current = Cursors.Hand;
            NativeMethods.SetWindowOwner(Handle, ownerHandle);
            Marshal.ReleaseComObject(item);
        }

        private void player_MediaError(object pMediaObject){
            Marshal.ReleaseComObject(pMediaObject);
            Environment.Exit(Program.CODE_MEDIA_ERROR);
        }
    }
}
