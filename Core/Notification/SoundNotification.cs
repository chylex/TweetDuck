using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using TweetDck.Core.Other;

namespace TweetDck.Core.Notification{
    class SoundNotification : IDisposable{
        private readonly FormBrowser browserForm;

        private SoundPlayer notificationSound;
        private bool ignoreNotificationSoundError;

        public SoundNotification(FormBrowser browserForm){
            this.browserForm = browserForm;
        }

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
            if (!ignoreNotificationSoundError){
                ignoreNotificationSoundError = true;

                using(FormMessage form = new FormMessage("Notification Sound Error", "Could not play custom notification sound."+Environment.NewLine+message, MessageBoxIcon.Error)){
                    form.AddButton("Ignore");

                    Button btnOpenSettings = form.AddButton("Open Settings");
                    btnOpenSettings.Width += 16;
                    btnOpenSettings.Location = new Point(btnOpenSettings.Location.X-16, btnOpenSettings.Location.Y);

                    if (form.ShowDialog() == DialogResult.OK && form.ClickedButton == btnOpenSettings){
                        browserForm.OpenSettings(FormSettings.TabIndexNotification);
                    }
                }
            }
        }

        public void Dispose(){
            if (notificationSound != null){
                notificationSound.Dispose();
            }
        }
    }
}
