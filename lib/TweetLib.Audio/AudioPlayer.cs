using System;
using System.Runtime.InteropServices;
using TweetLib.Audio.Impl;
using TweetLib.Audio.Utils;

namespace TweetLib.Audio{
    public abstract class AudioPlayer : IDisposable{
        private static bool? IsWMPAvailable;

        public static AudioPlayer New(){
            if (IsWMPAvailable.HasValue){
                if (IsWMPAvailable.Value){
                    return new SoundPlayerImplWMP();
                }
                else{
                    return new SoundPlayerImplFallback();
                }
            }

            try{
                SoundPlayerImplWMP implWMP = new SoundPlayerImplWMP();
                IsWMPAvailable = true;
                return implWMP;
            }catch(COMException){
                IsWMPAvailable = false;
                return new SoundPlayerImplFallback();
            }
        }

        public abstract string SupportedFormats { get; }
        public abstract event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        public abstract void Play(string file);
        public abstract void Stop();
        public abstract void Dispose();
    }
}
