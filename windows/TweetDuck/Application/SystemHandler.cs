using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Browser;
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Application {
	sealed class SystemHandler : IAppSystemHandler {
		public void OpenBrowser(string? url) {
			if (string.IsNullOrWhiteSpace(url)) {
				return;
			}

			FormManager.RunOnUIThreadAsync(() => {
				var config = Program.Config.User;

				switch (TwitterUrls.Check(url)) {
					case TwitterUrls.UrlType.Fine:
						string? browserPath = config.BrowserPath;

						if (browserPath == null || !File.Exists(browserPath)) {
							OpenAssociatedProgram(url);
						}
						else {
							string quotedUrl = '"' + url + '"';
							string browserArgs = config.BrowserPathArgs == null ? quotedUrl : config.BrowserPathArgs + ' ' + quotedUrl;

							try {
								using (Process.Start(browserPath, browserArgs)) {}
							} catch (Exception e) {
								App.ErrorHandler.HandleException("Error Opening Browser", "Could not open the browser.", true, e);
							}
						}

						break;

					case TwitterUrls.UrlType.Tracking:
						if (config.IgnoreTrackingUrlWarning) {
							goto case TwitterUrls.UrlType.Fine;
						}

						using (FormMessage form = new FormMessage("Blocked URL", "TweetDuck has blocked a tracking url due to privacy concerns. Do you want to visit it anyway?\n" + url, MessageBoxIcon.Warning)) {
							form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel | ControlType.Focused);
							form.AddButton(FormMessage.Yes, DialogResult.Yes, ControlType.Accept);
							form.AddButton("Always Visit", DialogResult.Ignore);

							DialogResult result = form.ShowDialog();

							if (result == DialogResult.Ignore) {
								config.IgnoreTrackingUrlWarning = true;
								config.Save();
							}

							if (result == DialogResult.Ignore || result == DialogResult.Yes) {
								goto case TwitterUrls.UrlType.Fine;
							}
						}

						break;

					case TwitterUrls.UrlType.Invalid:
						FormMessage.Warning("Blocked URL", "A potentially malicious or invalid URL was blocked from opening:\n" + url, FormMessage.OK);
						break;
				}
			});
		}

		public void OpenFileExplorer(string path) {
			if (File.Exists(path)) {
				using (Process.Start("explorer.exe", "/select,\"" + path.Replace('/', '\\') + "\"")) {}
			}
			else if (Directory.Exists(path)) {
				using (Process.Start("explorer.exe", '"' + path.Replace('/', '\\') + '"')) {}
			}
		}

		public IAppSystemHandler.OpenAssociatedProgramFunc OpenAssociatedProgram { get; } = path => {
			try {
				using (Process.Start(new ProcessStartInfo {
					FileName = path,
					UseShellExecute = true,
					ErrorDialog = true
				})) {}
			} catch (Exception e) {
				App.ErrorHandler.HandleException("Error Opening Program", "Could not open the associated program for " + path, true, e);
			}
		};


		public IAppSystemHandler.CopyImageFromFileFunc CopyImageFromFile { get; } = path => {
			FormManager.RunOnUIThreadAsync(() => {
				Image image;

				try {
					image = Image.FromFile(path);
				} catch (Exception ex) {
					FormMessage.Error("Copy Image", "An error occurred while copying the image: " + ex.Message, FormMessage.OK);
					return;
				}

				ClipboardManager.SetImage(image);
			});
		};

		public IAppSystemHandler.CopyTextFunc CopyText { get; } = text => {
			FormManager.RunOnUIThreadAsync(() => ClipboardManager.SetText(text, TextDataFormat.UnicodeText));
		};

		public IAppSystemHandler.SearchTextFunc SearchText { get; } = text => {
			if (string.IsNullOrWhiteSpace(text)) {
				return;
			}

			void PerformSearch() {
				var config = Program.Config.User;
				string? searchUrl = config.SearchEngineUrl;

				if (string.IsNullOrEmpty(searchUrl)) {
					if (FormMessage.Question("Search Options", "You have not configured a default search engine yet, would you like to do it now?", FormMessage.Yes, FormMessage.No)) {
						bool wereSettingsOpen = FormManager.TryFind<FormSettings>() != null;

						FormManager.TryFind<FormBrowser>()?.OpenSettings();

						if (wereSettingsOpen) {
							return;
						}

						FormSettings? settings = FormManager.TryFind<FormSettings>();

						if (settings == null) {
							return;
						}

						settings.FormClosed += (sender, args) => {
							if (args.CloseReason == CloseReason.UserClosing && config.SearchEngineUrl != searchUrl) {
								PerformSearch();
							}
						};
					}
				}
				else {
					App.SystemHandler.OpenBrowser(searchUrl + Uri.EscapeDataString(text));
				}
			}

			FormManager.RunOnUIThreadAsync(PerformSearch);
		};
	}
}
