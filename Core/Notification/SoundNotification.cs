using System;
using System.Runtime.InteropServices;
using WMPLib;

namespace TweetDck.Core.Notification{
    sealed class SoundNotification : IDisposable{ // TODO test on windows server
        public const string SupportedFormats = "*.wav;*.mp3;*.mp2;*.m4a;*.mid;*.midi;*.rmi;*.wma;*.aif;*.aifc;*.aiff;*.snd;*.au";

        public int Volume{
            get{
                return player.settings.volume;
            }

            set{
                player.settings.volume = value;
            }
        }

        public event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        private readonly WindowsMediaPlayer player;
        private bool wasTryingToPlay;
        private bool ignorePlaybackError;

        public SoundNotification(){
            player = new WindowsMediaPlayer();
            player.settings.autoStart = false;
            player.settings.enableErrorDialogs = false;
            player.settings.invokeURLs = false;
            player.MediaChange += player_MediaChange;
            player.MediaError += player_MediaError;
        }

        public void Play(string file){
            wasTryingToPlay = true;

            if (player.URL != file){
                player.close();
                player.URL = file;
                ignorePlaybackError = false;
            }
            else{
                player.controls.stop();
            }
            
            player.controls.play();
        }

        public void Stop(){
            player.controls.stop();
        }

        private void player_MediaChange(object item){
            IWMPMedia2 media = item as IWMPMedia2;

            if (media == null){
                OnNotificationSoundError("Unknown error.");
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

                if (!ignorePlaybackError && PlaybackError != null){
                    PlaybackErrorEventArgs args = new PlaybackErrorEventArgs(message);
                    PlaybackError(this, args);
                    ignorePlaybackError = args.Ignore;
                }
            }
        }

        public void Dispose(){
            player.close();
            Marshal.ReleaseComObject(player);
        }

        public class PlaybackErrorEventArgs : EventArgs{
            public string Message { get; private set; }
            public bool Ignore { get; set; }

            public PlaybackErrorEventArgs(string message){
                this.Message = message;
                this.Ignore = false;
            }
        }
    }
}
