using System;
using System.IO;
using System.Media;

namespace TweetDck.Core.Notification{
    sealed class SoundNotification : IDisposable{

        private SoundPlayer notificationSound;
        private bool ignoreNotificationSoundError;

        public event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        public void Play(string file){
            if (notificationSound == null){
                notificationSound = new SoundPlayer{
                    LoadTimeout = 5000
                };
            }

            if (notificationSound.SoundLocation != file){
                notificationSound.SoundLocation = file;
                ignoreNotificationSoundError = false;
            }

            try{
                notificationSound.Play();
            }catch(FileNotFoundException e){
                OnNotificationSoundError("File not found: "+e.FileName);
            }catch(InvalidOperationException){
                OnNotificationSoundError("File is not a valid sound file.");
            }catch(TimeoutException){
                OnNotificationSoundError("File took too long to load.");
            }
        }



        private void OnNotificationSoundError(string message){
            if (!ignoreNotificationSoundError && PlaybackError != null){
                PlaybackErrorEventArgs args = new PlaybackErrorEventArgs(message);
                PlaybackError(this, args);
                ignoreNotificationSoundError = args.Ignore;
            }
        }

        public void Dispose(){
            if (notificationSound != null){
                notificationSound.Dispose();
            }
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
