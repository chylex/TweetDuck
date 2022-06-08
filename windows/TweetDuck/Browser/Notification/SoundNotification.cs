using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Dialogs.Settings;
using TweetDuck.Management;
using TweetLib.Browser.CEF.Data;
using TweetLib.Core.Features.TweetDeck;

namespace TweetDuck.Browser.Notification {
	sealed class SoundNotification : ISoundNotificationHandler {
		public const string SupportedFormats = "*.wav;*.ogg;*.mp3;*.flac;*.opus;*.weba;*.webm";

		private readonly ResourceHandlerRegistry<IResourceHandler> registry;

		public SoundNotification(ResourceHandlerRegistry<IResourceHandler> registry) {
			this.registry = registry;
		}

		public void Unregister(string url) {
			registry.Unregister(url);
		}

		public void Register(string url, string path) {
			var fileHandler = CreateFileHandler(path);
			if (fileHandler.HasValue) {
				var (data, mimeType) = fileHandler.Value;
				registry.RegisterStatic(url, data, mimeType);
			}
		}

		private static (byte[] data, string mimeType)? CreateFileHandler(string path) {
			string mimeType = Path.GetExtension(path) switch {
				".weba" => "audio/webm",
				".webm" => "audio/webm",
				".wav"  => "audio/wav",
				".ogg"  => "audio/ogg",
				".mp3"  => "audio/mp3",
				".flac" => "audio/flac",
				".opus" => "audio/ogg; codecs=opus",
				_       => "application/octet-stream"
			};

			try {
				return (File.ReadAllBytes(path), mimeType);
			} catch {
				FormBrowser? browser = FormManager.TryFind<FormBrowser>();

				browser?.InvokeAsyncSafe(() => {
					using FormMessage form = new FormMessage("Sound Notification Error", "Could not find custom notification sound file:\n" + path, MessageBoxIcon.Error);
					form.AddButton(FormMessage.Ignore, ControlType.Cancel | ControlType.Focused);

					Button btnViewOptions = form.AddButton("View Options");
					btnViewOptions.Width += 16;
					btnViewOptions.Location = new Point(btnViewOptions.Location.X - 16, btnViewOptions.Location.Y);

					if (form.ShowDialog() == DialogResult.OK && form.ClickedButton == btnViewOptions) {
						browser.OpenSettings(typeof(TabSettingsSounds));
					}
				});

				return null;
			}
		}
	}
}
