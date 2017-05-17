using System.Runtime.InteropServices;
using TweetDuck.Core.Notification.Sound;

namespace TweetDuck.Core.Notification{
    static class SoundNotification{
        private static bool? IsWMPAvailable;

        public static ISoundNotificationPlayer New(){
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
    }
}
