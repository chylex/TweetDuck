using System;
using TweetLib.Audio;

namespace TweetDuck.Core.Notification{
    sealed class SoundNotification : IDisposable{
        public string SupportedFormats => player.SupportedFormats;
        public event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        private readonly AudioPlayer player;

        public SoundNotification(){
            this.player = AudioPlayer.New();
            this.player.PlaybackError += Player_PlaybackError;
        }

        public void Play(string file){
            player.Play(file);
        }

        private void Player_PlaybackError(object sender, PlaybackErrorEventArgs e){
            PlaybackError?.Invoke(this, e);
        }

        public void Dispose(){
            player.Dispose();
        }
    }
}
