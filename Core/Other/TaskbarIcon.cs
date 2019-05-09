using System;
using Microsoft.WindowsAPICodePack.Taskbar;
using TweetDuck.Configuration;
using Res = TweetDuck.Properties.Resources;

namespace TweetDuck.Core.Other{
    sealed class TaskbarIcon : IDisposable{
        private static UserConfig Config => Program.Config.User;

        public bool HasNotifications{
            get{
                return hasNotifications;
            }

            set{
                if (hasNotifications != value){
                    hasNotifications = value;
                    UpdateIcon();
                }
            }
        }

        private bool hasNotifications;

        public TaskbarIcon(){
            Config.MuteToggled += Config_MuteToggled;
        }

        public void Dispose(){
            Config.MuteToggled -= Config_MuteToggled;
        }

        private void Config_MuteToggled(object sender, EventArgs e){
            UpdateIcon();
        }

        public void UpdateIcon(){
            if (hasNotifications){
                TaskbarManager.Instance.SetOverlayIcon(Res.overlay_notification, "Unread Notifications");
            }
            else if (Config.MuteNotifications){
                TaskbarManager.Instance.SetOverlayIcon(Res.overlay_muted, "Notifications Muted");
            }
            else{
                TaskbarManager.Instance.SetOverlayIcon(null, string.Empty);
            }
        }
    }
}
