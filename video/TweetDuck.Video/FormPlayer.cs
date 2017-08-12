using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace TweetDuck.Video{
    partial class FormPlayer : Form{
        private readonly IntPtr ownerHandle;
        private readonly string videoUrl;
        
        private readonly ControlWMP player;
        private bool isPaused;
        private bool isDragging;

        private WindowsMediaPlayer Player => player.Ocx;

        public FormPlayer(IntPtr handle, int volume, string url){
            InitializeComponent();

            this.ownerHandle = handle;
            this.videoUrl = url;
            
            player = new ControlWMP{
                Dock = DockStyle.Fill
            };

            player.BeginInit();
            Controls.Add(player);
            player.EndInit();

            Player.enableContextMenu = false;
            Player.uiMode = "none";
            Player.settings.setMode("loop", true);
            
            Player.MediaChange += player_MediaChange;
            Player.MediaError += player_MediaError;
            
            trackBarVolume.Value = volume; // changes player volume too if non-default

            Application.AddMessageFilter(new MessageFilter(this));
        }

        // Events

        private void FormPlayer_Load(object sender, EventArgs e){
            Player.URL = videoUrl;
        }

        private void player_MediaChange(object item){
            timerSync.Start();
            Cursor.Current = Cursors.Default;
            NativeMethods.SetWindowOwner(Handle, ownerHandle);
            Marshal.ReleaseComObject(item);
        }

        private void player_MediaError(object pMediaObject){
            Console.Out.WriteLine(((IWMPMedia2)pMediaObject).Error.errorDescription);

            Marshal.ReleaseComObject(pMediaObject);
            Environment.Exit(Program.CODE_MEDIA_ERROR);
        }

        private void timerSync_Tick(object sender, EventArgs e){
            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect)){
                int width = rect.Right-rect.Left+1;
                int height = rect.Bottom-rect.Top+1;
                IWMPMedia media = Player.currentMedia;

                ClientSize = new Size(Math.Min(media.imageSourceWidth, width*3/4), Math.Min(media.imageSourceHeight, height*3/4));
                Location = new Point(rect.Left+(width-ClientSize.Width)/2, rect.Top+(height-ClientSize.Height+SystemInformation.CaptionHeight)/2);

                tablePanel.Visible = ClientRectangle.Contains(PointToClient(Cursor.Position)) || isDragging;

                if (tablePanel.Visible){
                    labelTime.Text = $"{Player.controls.currentPositionString} / {Player.currentMedia.durationString}";

                    int value = (int)Math.Round(progressSeek.Maximum*Player.controls.currentPosition/Player.currentMedia.duration);

                    if (value >= progressSeek.Maximum){
                        progressSeek.Value = progressSeek.Maximum;
                        progressSeek.Value = progressSeek.Maximum-1;
                        progressSeek.Value = progressSeek.Maximum;
                    }
                    else{
                        progressSeek.Value = value+1;
                        progressSeek.Value = value;
                    }
                }

                if (Player.controls.currentPosition > Player.currentMedia.duration){ // pausing near the end of the video causes WMP to play beyond the end of the video wtf
                    Player.controls.stop();
                    Player.controls.currentPosition = 0;
                    Player.controls.play();
                }
            }
            else{
                Environment.Exit(Program.CODE_OWNER_GONE);
            }
        }

        private void timerData_Tick(object sender, EventArgs e){
            timerData.Stop();
            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, Program.VideoPlayerMessage, new UIntPtr((uint)trackBarVolume.Value), ownerHandle);
        }

        private void progressSeek_Click(object sender, EventArgs e){
            Player.controls.currentPosition = Player.currentMedia.duration*progressSeek.PointToClient(Cursor.Position).X/progressSeek.Width;
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e){
            Player.settings.volume = trackBarVolume.Value;

            if (timerSync.Enabled){
                timerData.Stop();
                timerData.Start();
            }
        }

        private void trackBarVolume_MouseDown(object sender, MouseEventArgs e){
            isDragging = true;
        }

        private void trackBarVolume_MouseUp(object sender, MouseEventArgs e){
            isDragging = false;
        }

        // Controls & messages

        private void TogglePause(){
            if (isPaused){
                Player.controls.play();
            }
            else{
                Player.controls.pause();
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

                    if (cursor.Y < form.tablePanel.Location.Y){
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
