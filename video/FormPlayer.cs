using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TweetDuck.Video.Controls;
using TweetLib.Communication;
using WMPLib;

namespace TweetDuck.Video{
    sealed partial class FormPlayer : Form{
        protected override bool ShowWithoutActivation => true;

        private readonly IntPtr ownerHandle;
        private readonly float ownerDpi;
        private readonly string videoUrl;
        private readonly DuplexPipe pipe;
        
        private readonly ControlWMP player;
        private bool wasCursorInside;
        private bool isPaused;
        private bool isDragging;

        private WindowsMediaPlayer Player => player.Ocx;

        public FormPlayer(IntPtr handle, int dpi, int volume, string url, string token){
            InitializeComponent();

            this.ownerHandle = handle;
            this.ownerDpi = dpi / 100F;
            this.videoUrl = url;
            this.pipe = DuplexPipe.CreateClient(token);
            this.pipe.DataIn += pipe_DataIn;

            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect)){
                ClientSize = new Size(0, 0);
                Location = new Point((rect.Left+rect.Right)/2, (rect.Top+rect.Bottom)/2);
                Opacity = 0;
            }
            
            player = new ControlWMP{
                Dock = DockStyle.Fill
            };

            player.BeginInit();
            Controls.Add(player);
            player.EndInit();

            Player.enableContextMenu = false;
            Player.uiMode = "none";
            Player.settings.autoStart = false;
            Player.settings.enableErrorDialogs = false;
            Player.settings.setMode("loop", true);

            Player.PlayStateChange += player_PlayStateChange;
            Player.MediaError += player_MediaError;
            
            trackBarVolume.Value = volume; // changes player volume too if non-default

            labelTooltip.AttachTooltip(progressSeek, true, args => {
                if (args.X < 0 || args.Y < 0 || args.X >= progressSeek.Width || args.Y >= progressSeek.Height){
                    return null;
                }

                IWMPMedia media = Player.currentMedia;
                int progress = (int)(media.duration*progressSeek.GetProgress(args.X));

                Marshal.ReleaseComObject(media);

                return $"{(progress/60).ToString("00")}:{(progress%60).ToString("00")}";
            });
            
            labelTooltip.AttachTooltip(trackBarVolume, false, args => $"Volume : {trackBarVolume.Value}%");

            labelTooltip.AttachTooltip(imageClose, false, "Close");
            labelTooltip.AttachTooltip(imageDownload, false, "Download");
            labelTooltip.AttachTooltip(imageResize, false, "Fullscreen");

            Application.AddMessageFilter(new MessageFilter(this));
        }

        private int DpiScaled(int value){
            return (int)Math.Round(value*ownerDpi);
        }

        // Events

        private void FormPlayer_Load(object sender, EventArgs e){
            Player.URL = videoUrl;
        }

        private void pipe_DataIn(object sender, DuplexPipe.PipeReadEventArgs e){
            Invoke(new Action(() => {
                switch(e.Key){
                    case "key":
                        HandleKey((Keys)int.Parse(e.Data, NumberStyles.Integer));
                        break;

                    case "die":
                        StopVideo();
                        break;
                }
            }));
        }

        private void player_PlayStateChange(int newState){
            WMPPlayState state = (WMPPlayState)newState;

            if (state == WMPPlayState.wmppsReady){
                Player.controls.play();
            }
            else if (state == WMPPlayState.wmppsPlaying){
                Player.PlayStateChange -= player_PlayStateChange;
                
                timerSync.Start();
                NativeMethods.SetWindowOwner(Handle, ownerHandle);
                Cursor.Current = Cursors.Default;

                SuspendLayout();
                timerSync_Tick(timerSync, EventArgs.Empty);
                Opacity = 1;
                ResumeLayout(true);
            }
        }

        private void player_MediaError(object pMediaObject){
            IWMPErrorItem error = ((IWMPMedia2)pMediaObject).Error;
            Console.Out.WriteLine($"Media Error {error.errorCode}: {error.errorDescription}");

            Marshal.ReleaseComObject(error);
            Marshal.ReleaseComObject(pMediaObject);
            Environment.Exit(Program.CODE_MEDIA_ERROR);
        }

        [HandleProcessCorruptedStateExceptions]
        private void timerSync_Tick(object sender, EventArgs e){
            if (NativeMethods.GetWindowRect(ownerHandle, out NativeMethods.RECT rect)){
                IWMPMedia media = Player.currentMedia;
                IWMPControls controls = Player.controls;

                int ownerLeft = rect.Left;
                int ownerTop = rect.Top;
                int ownerWidth = rect.Right-rect.Left+1;
                int ownerHeight = rect.Bottom-rect.Top+1;
                
                // roughly matches MinimumSize for client bounds
                int minWidth = DpiScaled(332);
                int minHeight = DpiScaled(386);

                if (NativeMethods.GetClientRect(ownerHandle, out NativeMethods.RECT clientSize)){
                    minWidth = Math.Min(minWidth, clientSize.Right);
                    minHeight = Math.Min(minHeight, clientSize.Bottom);
                }

                int maxWidth = Math.Min(DpiScaled(media.imageSourceWidth), ownerWidth*3/4);
                int maxHeight = Math.Min(DpiScaled(media.imageSourceHeight), ownerHeight*3/4);
                
                bool isCursorInside = ClientRectangle.Contains(PointToClient(Cursor.Position));
                
                Size newSize = new Size(Math.Max(minWidth+2, maxWidth), Math.Max(minHeight+2, maxHeight));
                Point newLocation = new Point(ownerLeft+(ownerWidth-newSize.Width)/2, ownerTop+(ownerHeight-newSize.Height+SystemInformation.CaptionHeight)/2);
                
                if (ClientSize != newSize || Location != newLocation){
                    ClientSize = newSize;
                    Location = newLocation;
                }

                tablePanel.Visible = isCursorInside || isDragging;

                if (tablePanel.Visible){
                    labelTime.Text = $"{controls.currentPositionString} / {media.durationString}";

                    int value = (int)Math.Round(progressSeek.Maximum*controls.currentPosition/media.duration);

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
                
                if (controls.currentPosition > media.duration){ // pausing near the end of the video causes WMP to play beyond the end of the video wtf
                    try{
                        controls.stop();
                        controls.currentPosition = 0;
                        controls.play();
                    }catch(AccessViolationException){
                        // something is super retarded here because shit gets disposed between the start of this method and
                        // the controls.play() call even though it runs on the UI thread
                    }
                }
                
                if (isCursorInside && !wasCursorInside){
                    wasCursorInside = true;
                }
                else if (!isCursorInside && wasCursorInside){
                    wasCursorInside = false;

                    if (!Player.fullScreen && Handle == NativeMethods.GetForegroundWindow()){
                        NativeMethods.SetForegroundWindow(ownerHandle);
                    }
                }

                Marshal.ReleaseComObject(media);
                Marshal.ReleaseComObject(controls);
            }
            else{
                Environment.Exit(Program.CODE_OWNER_GONE);
            }
        }

        private void timerData_Tick(object sender, EventArgs e){
            timerData.Stop();
            pipe.Write("vol", trackBarVolume.Value.ToString());
        }

        private void progressSeek_MouseDown(object sender, MouseEventArgs e){
            if (e.Button == MouseButtons.Left){
                IWMPMedia media = Player.currentMedia;
                IWMPControls controls = Player.controls;

                controls.currentPosition = media.duration*progressSeek.GetProgress(progressSeek.PointToClient(Cursor.Position).X);

                Marshal.ReleaseComObject(media);
                Marshal.ReleaseComObject(controls);
            }
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e){
            IWMPSettings settings = Player.settings;
            settings.volume = trackBarVolume.Value;
            
            Marshal.ReleaseComObject(settings);

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

        private void imageClose_Click(object sender, EventArgs e){
            StopVideo();
        }

        private void imageDownload_Click(object sender, EventArgs e){
            pipe.Write("download");
        }

        private void imageResize_Click(object sender, EventArgs e){
            Player.fullScreen = true;
        }

        // Controls & messages

        private bool HandleKey(Keys key){
            switch(key){
                case Keys.Space:
                    TogglePause();
                    return true;

                case Keys.Escape:
                    if (Player.fullScreen){
                        Player.fullScreen = false;
                        NativeMethods.SetForegroundWindow(ownerHandle);
                        return true;
                    }
                    else{
                        StopVideo();
                        return true;
                    }
                    
                default:
                    return false;
            }
        }

        private void TogglePause(){
            IWMPControls controls = Player.controls;

            if (isPaused){
                controls.play();
            }
            else{
                controls.pause();
            }

            isPaused = !isPaused;
            Marshal.ReleaseComObject(controls);
        }

        private void StopVideo(){
            timerSync.Stop();
            Visible = false;
            pipe.Write("rip");
            
            Player.close();
            Close();
        }

        internal sealed class MessageFilter : IMessageFilter{
            private readonly FormPlayer form;

            private bool IsCursorOverVideo{
                get{
                    Point cursor = form.PointToClient(Cursor.Position);
                    return cursor.Y < form.tablePanel.Location.Y;
                }
            }

            public MessageFilter(FormPlayer form){
                this.form = form;
            }

            bool IMessageFilter.PreFilterMessage(ref Message m){
                if (m.Msg == 0x0201){ // WM_LBUTTONDOWN
                    if (IsCursorOverVideo){
                        form.TogglePause();
                        return true;
                    }
                }
                else if (m.Msg == 0x0203){ // WM_LBUTTONDBLCLK
                    if (IsCursorOverVideo){
                        form.TogglePause();
                        form.Player.fullScreen = !form.Player.fullScreen;
                        return true;
                    }
                }
                else if (m.Msg == 0x0100){ // WM_KEYDOWN
                    return form.HandleKey((Keys)m.WParam.ToInt32());
                }
                else if (m.Msg == 0x020B && ((m.WParam.ToInt32() >> 16) & 0xFFFF) == 1){ // WM_XBUTTONDOWN
                    NativeMethods.SetForegroundWindow(form.ownerHandle);
                    Environment.Exit(Program.CODE_USER_REQUESTED);
                }

                return false;
            }
        }
    }
}
