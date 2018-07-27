using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsSounds : BaseTabSettings{
        private readonly Action playSoundNotification;

        public TabSettingsSounds(Action playSoundNotification){
            InitializeComponent();

            this.playSoundNotification = playSoundNotification;
            
            // sound notification

            toolTip.SetToolTip(tbCustomSound, "When empty, the default TweetDeck sound notification is used.");
            
            trackBarVolume.SetValueSafe(Config.NotificationSoundVolume);
            labelVolumeValue.Text = trackBarVolume.Value+"%";

            tbCustomSound.Text = Config.NotificationSoundPath;
            tbCustomSound_TextChanged(tbCustomSound, EventArgs.Empty);
        }

        public override void OnReady(){
            tbCustomSound.TextChanged += tbCustomSound_TextChanged;
            btnPlaySound.Click += btnPlaySound_Click;
            btnBrowseSound.Click += btnBrowseSound_Click;
            btnResetSound.Click += btnResetSound_Click;
        }

        public override void OnClosing(){
            Config.NotificationSoundPath = tbCustomSound.Text;
            Config.NotificationSoundVolume = trackBarVolume.Value;
        }

        #region Sound Notification

        private bool RefreshCanPlay(){
            bool isEmpty = string.IsNullOrEmpty(tbCustomSound.Text);
            bool canPlay = isEmpty || File.Exists(tbCustomSound.Text);

            tbCustomSound.ForeColor = canPlay ? SystemColors.WindowText : Color.Red;
            btnPlaySound.Enabled = canPlay;
            btnResetSound.Enabled = !isEmpty;
            return canPlay;
        }

        private void tbCustomSound_TextChanged(object sender, EventArgs e){
            RefreshCanPlay();
        }

        private void btnPlaySound_Click(object sender, EventArgs e){
            if (RefreshCanPlay()){
                Config.NotificationSoundPath = tbCustomSound.Text;
                Config.NotificationSoundVolume = trackBarVolume.Value;
                playSoundNotification();
            }
        }

        private void btnBrowseSound_Click(object sender, EventArgs e){
            using(OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Title = "Custom Notification Sound",
                Filter = $"Sound file ({SoundNotification.SupportedFormats})|{SoundNotification.SupportedFormats}|All files (*.*)|*.*"
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    tbCustomSound.Text = dialog.FileName;
                }
            }
        }

        private void btnResetSound_Click(object sender, EventArgs e){
            tbCustomSound.Text = string.Empty;
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e){
            volumeUpdateTimer.Stop();
            volumeUpdateTimer.Start();
            labelVolumeValue.Text = trackBarVolume.Value+"%";
        }

        private void volumeUpdateTimer_Tick(object sender, EventArgs e){
            Config.NotificationSoundVolume = trackBarVolume.Value;
            volumeUpdateTimer.Stop();
        }

        #endregion
    }
}
