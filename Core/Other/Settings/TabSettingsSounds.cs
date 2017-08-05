using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Notification;
using TweetLib.Audio;

namespace TweetDuck.Core.Other.Settings{
    partial class TabSettingsSounds : BaseTabSettings{
        private readonly SoundNotification soundNotification;

        public TabSettingsSounds(){
            InitializeComponent();

            soundNotification = new SoundNotification();
            soundNotification.PlaybackError += sound_PlaybackError;

            tbCustomSound.Text = Config.NotificationSoundPath;
            tbCustomSound_TextChanged(tbCustomSound, new EventArgs());

            Disposed += (sender, args) => soundNotification.Dispose();
        }

        public override void OnReady(){
            tbCustomSound.TextChanged += tbCustomSound_TextChanged;
            btnPlaySound.Click += btnPlaySound_Click;
            btnBrowseSound.Click += btnBrowseSound_Click;
            btnResetSound.Click += btnResetSound_Click;
        }

        public override void OnClosing(){
            Config.NotificationSoundPath = tbCustomSound.Text;
        }

        private void tbCustomSound_TextChanged(object sender, EventArgs e){
            bool isEmpty = string.IsNullOrEmpty(tbCustomSound.Text);
            tbCustomSound.ForeColor = isEmpty || File.Exists(tbCustomSound.Text) ? SystemColors.WindowText : Color.Red;
            btnPlaySound.Enabled = !isEmpty;
            btnResetSound.Enabled = !isEmpty;
        }

        private void btnPlaySound_Click(object sender, EventArgs e){
            soundNotification.Play(tbCustomSound.Text);
        }

        private void sound_PlaybackError(object sender, PlaybackErrorEventArgs e){
            FormMessage.Error("Notification Sound Error", "Could not play custom notification sound.\n"+e.Message, FormMessage.OK);
        }

        private void btnBrowseSound_Click(object sender, EventArgs e){
            using(OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Title = "Custom Notification Sound",
                Filter = "Sound file ("+soundNotification.SupportedFormats+")|"+soundNotification.SupportedFormats+"|All files (*.*)|*.*"
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    tbCustomSound.Text = dialog.FileName;
                }
            }
        }

        private void btnResetSound_Click(object sender, EventArgs e){
            tbCustomSound.Text = string.Empty;
        }
    }
}
