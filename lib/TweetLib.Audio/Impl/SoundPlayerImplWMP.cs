using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace TweetLib.Audio.Impl{
    sealed class SoundPlayerImplWMP : AudioPlayer{
        public override string SupportedFormats => "*.wav;*.mp3;*.mp2;*.m4a;*.mid;*.midi;*.rmi;*.wma;*.aif;*.aifc;*.aiff;*.snd;*.au";

        public override event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        private readonly Form owner;
        private readonly ControlWMP wmp;
        private bool wasTryingToPlay;
        private bool ignorePlaybackError;

        private WindowsMediaPlayer Player => wmp.Ocx;
        
        public SoundPlayerImplWMP(){
            owner = new Form();
            wmp = new ControlWMP();
            wmp.BeginInit();
            owner.Controls.Add(wmp);
            wmp.EndInit();
            
            Player.uiMode = "none";
            Player.settings.autoStart = false;
            Player.settings.enableErrorDialogs = false;
            Player.settings.invokeURLs = false;
            Player.settings.volume = 0;
            Player.MediaChange += player_MediaChange;
            Player.MediaError += player_MediaError;
        }

        public override void Play(string file){
            wasTryingToPlay = true;

            try{
                if (Player.URL != file){
                    Player.close();
                    Player.URL = file;
                    ignorePlaybackError = false;
                }
                else{
                    Player.controls.stop();
                }
            
                Player.controls.play();
            }catch(Exception e){
                OnNotificationSoundError("An error occurred in Windows Media Player: "+e.Message);
            }
        }

        public override bool SetVolume(int volume){
            Player.settings.volume = volume;
            return true;
        }

        protected override void Dispose(bool disposing){
            wmp.Dispose();
            owner.Dispose();
        }

        private void player_MediaChange(object item){
            IWMPMedia2 media = item as IWMPMedia2;

            if (media == null){
                OnNotificationSoundError("Unknown error.");
                return;
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            else if (media.Error == null && media.duration == 0.0){
                OnNotificationSoundError("File does not contain an audio track.");
            }
            else if (media.Error != null){
                OnNotificationSoundError(media.Error);
            }

            Marshal.ReleaseComObject(media);
        }

        private void player_MediaError(object pMediaObject){
            IWMPMedia2 media = pMediaObject as IWMPMedia2;

            if (media == null){
                OnNotificationSoundError("Unknown error.");
                return;
            }
            else if (media.Error != null){
                OnNotificationSoundError(media.Error);
            }

            Marshal.ReleaseComObject(media);
        }

        private void OnNotificationSoundError(IWMPErrorItem error){
            OnNotificationSoundError(error.errorCode == -1072885353 ? "Invalid media file." : error.errorDescription);
            Marshal.ReleaseComObject(error);
        }

        private void OnNotificationSoundError(string message){
            if (wasTryingToPlay){
                wasTryingToPlay = false;

                if (!ignorePlaybackError){
                    PlaybackErrorEventArgs args = new PlaybackErrorEventArgs(message);
                    PlaybackError?.Invoke(this, args);
                    ignorePlaybackError = args.Ignore;
                }
            }
        }

        [Clsid("{6bf52a52-394a-11d3-b153-00c04f79faa6}")]
        private sealed class ControlWMP : AxHost{
            public WindowsMediaPlayer Ocx { get; private set; }

            public ControlWMP() : base("6bf52a52-394a-11d3-b153-00c04f79faa6"){}

            protected override void AttachInterfaces(){
                Ocx = (WindowsMediaPlayer)GetOcx();
            }
        }
    }
}
