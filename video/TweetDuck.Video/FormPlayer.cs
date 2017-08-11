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

            player.Ocx.enableContextMenu = false;
            player.Ocx.uiMode = "none";
            player.Ocx.settings.setMode("loop", true);
            
            player.Ocx.MediaChange += player_MediaChange;
            player.Ocx.MediaError += player_MediaError;
            
            trackBarVolume.Value = volume; // changes player volume too if non-default

            Application.AddMessageFilter(new MessageFilter(this));
        }

        // Events

        private void FormPlayer_Load(object sender, EventArgs e){
            player.Ocx.URL = videoUrl;
        }

        private void player_MediaChange(object item){
            timerSync.Start();
            Cursor.Current = Cursors.Default;
            NativeMethods.SetWindowOwner(Handle, ownerHandle);
            Marshal.ReleaseComObject(item);
        }

        private void player_MediaError(object pMediaObject){
            Marshal.ReleaseComObject(pMediaObject);
            Environment.Exit(Program.CODE_MEDIA_ERROR);
        }

        private void timerSync_Tick(object sender, EventArgs e){
            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect)){
                int width = rect.Right-rect.Left+1;
                int height = rect.Bottom-rect.Top+1;
                IWMPMedia media = player.Ocx.currentMedia;

                ClientSize = new Size(Math.Min(media.imageSourceWidth, width*3/4), Math.Min(media.imageSourceHeight, height*3/4));
                Location = new Point(rect.Left+(width-ClientSize.Width)/2, rect.Top+(height-ClientSize.Height+SystemInformation.CaptionHeight)/2);

                tablePanel.Visible = ClientRectangle.Contains(PointToClient(Cursor.Position)) || tablePanel.ContainsFocus;

                if (tablePanel.Visible){
                    labelTime.Text = $"{player.Ocx.controls.currentPositionString} / {player.Ocx.currentMedia.durationString}";

                    int value = (int)Math.Round(progressSeek.Maximum*player.Ocx.controls.currentPosition/player.Ocx.currentMedia.duration);

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
            player.Ocx.controls.currentPosition = player.Ocx.currentMedia.duration*progressSeek.PointToClient(Cursor.Position).X/progressSeek.Width;
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e){
            player.Ocx.settings.volume = trackBarVolume.Value;

            if (timerSync.Enabled){
                timerData.Stop();
                timerData.Start();
            }
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
