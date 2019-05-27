using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Settings;

namespace TweetDuck.Core.Notification{
    static class SoundNotification{
        public const string SupportedFormats = "*.wav;*.ogg;*.mp3;*.flac;*.opus;*.weba;*.webm";
        
        public static IResourceHandler CreateFileHandler(string path){
            string mimeType = Path.GetExtension(path) switch{
                ".weba" => "audio/webm",
                ".webm" => "audio/webm",
                ".wav"  => "audio/wav",
                ".ogg"  => "audio/ogg",
                ".mp3"  => "audio/mp3",
                ".flac" => "audio/flac",
                ".opus" => "audio/ogg; codecs=opus",
                _       => null
            };

            try{
                return ResourceHandler.FromFilePath(path, mimeType);
            }catch{
                FormBrowser browser = FormManager.TryFind<FormBrowser>();

                browser?.InvokeAsyncSafe(() => {
                    using(FormMessage form = new FormMessage("Sound Notification Error", "Could not find custom notification sound file:\n"+path, MessageBoxIcon.Error)){
                        form.AddButton(FormMessage.Ignore, ControlType.Cancel | ControlType.Focused);
                        
                        Button btnViewOptions = form.AddButton("View Options");
                        btnViewOptions.Width += 16;
                        btnViewOptions.Location = new Point(btnViewOptions.Location.X-16, btnViewOptions.Location.Y);

                        if (form.ShowDialog() == DialogResult.OK && form.ClickedButton == btnViewOptions){
                            browser.OpenSettings(typeof(TabSettingsSounds));
                        }
                    }
                });
                
                return null;
            }
        }
    }
}
