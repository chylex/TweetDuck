using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace TweetDuck.Video{
    partial class FormPlayer : Form{
        private readonly IntPtr ownerHandle;
        private readonly string videoUrl;

        private bool isPaused;

        public FormPlayer(IntPtr handle, string url){
            InitializeComponent();

            this.ownerHandle = handle;
            this.videoUrl = url;

            player.Ocx.enableContextMenu = false;
            player.Ocx.uiMode = "none";
            player.Ocx.settings.setMode("loop", true);
            
            player.Ocx.MediaChange += player_MediaChange;
            player.Ocx.MediaError += player_MediaError;

            trackBarVolume.Value = 25; // changes player volume

            Application.AddMessageFilter(new MessageFilter(this));
        }

        // Events

        private void FormPlayer_Load(object sender, EventArgs e){
            player.Ocx.URL = videoUrl;
        }

        private void timer_Tick(object sender, EventArgs e){
            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect)){
                int width = rect.Right-rect.Left+1;
                int height = rect.Bottom-rect.Top+1;
                IWMPMedia media = player.Ocx.currentMedia;

                ClientSize = new Size(Math.Min(media.imageSourceWidth, width*3/4), Math.Min(media.imageSourceHeight, height*3/4));
                Location = new Point(rect.Left+(width-ClientSize.Width)/2, rect.Top+(height-ClientSize.Height+SystemInformation.CaptionHeight)/2);

                trackBarVolume.Visible = ClientRectangle.Contains(PointToClient(Cursor.Position)) || trackBarVolume.Focused;
            }
            else{
                Environment.Exit(Program.CODE_OWNER_GONE);
            }
        }

        private void player_MediaChange(object item){
            timer.Start();
            Cursor.Current = Cursors.Default;
            NativeMethods.SetWindowOwner(Handle, ownerHandle);
            Marshal.ReleaseComObject(item);
        }

        private void player_MediaError(object pMediaObject){
            Marshal.ReleaseComObject(pMediaObject);
            Environment.Exit(Program.CODE_MEDIA_ERROR);
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e){
            player.Ocx.settings.volume = trackBarVolume.Value;
        }

        private void trackBarVolume_MouseUp(object sender, MouseEventArgs e){
            player.Focus();
        }

        // Controls & messages

        private void TogglePause(){
            if (isPaused){
                player.Ocx.controls.play();
            }
            else{
                player.Ocx.controls.pause();
            }

            isPaused = !isPaused;
        }

        internal class MessageFilter : IMessageFilter{
            private readonly FormPlayer form;

            public MessageFilter(FormPlayer form){
                this.form = form;
            }

            bool IMessageFilter.PreFilterMessage(ref Message m){
                if (m.Msg == 0x0201){ // WM_LBUTTONDOWN
                    Point cursor = form.PointToClient(Cursor.Position);

                    if (!(cursor.X >= form.trackBarVolume.Location.X && cursor.Y >= form.trackBarVolume.Location.Y)){
                        form.TogglePause();
                        return true;
                    }
                }
                else if (m.Msg == 0x0203 || (m.Msg == 0x0100 && m.WParam.ToInt32() == 0x20)){ // WM_LBUTTONDBLCLK, WM_KEYDOWN, VK_SPACE
                    form.TogglePause();
                }

                return false;
            }
        }
    }
}
