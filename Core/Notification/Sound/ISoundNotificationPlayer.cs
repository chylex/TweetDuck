using System;

namespace TweetDck.Core.Notification.Sound{
    interface ISoundNotificationPlayer : IDisposable{
        string SupportedFormats { get; }

        event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        void Play(string file);
        void Stop();
    }
}
