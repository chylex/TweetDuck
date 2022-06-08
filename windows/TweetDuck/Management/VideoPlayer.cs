using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Browser;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetLib.Communication.Pipe;
using TweetLib.Core;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Management {
	sealed class VideoPlayer : IDisposable {
		private static UserConfig Config => Program.Config.User;

		public bool Running => currentInstance is { Running: true };

		public event EventHandler? ProcessExited;

		private readonly FormBrowser owner;

		private Instance? currentInstance;
		private bool isClosing;

		public VideoPlayer(FormBrowser owner) {
			this.owner = owner;
			this.owner.FormClosing += owner_FormClosing;
		}

		public void Launch(string videoUrl, string tweetUrl, string username) {
			if (Running) {
				Destroy();
				isClosing = false;
			}

			try {
				DuplexPipe.Server pipe = DuplexPipe.CreateServer();
				pipe.DataIn += pipe_DataIn;

				ProcessStartInfo startInfo = new ProcessStartInfo {
					FileName = Path.Combine(App.ProgramPath, "TweetDuck.Video.exe"),
					Arguments = $"{owner.Handle} {(int) Math.Floor(100F * owner.GetDPIScale())} {Config.VideoPlayerVolume} \"{videoUrl}\" \"{pipe.GenerateToken()}\"",
					RedirectStandardOutput = true
				};

				Process? process;
				if ((process = Process.Start(startInfo)) != null) {
					currentInstance = new Instance(process, pipe, videoUrl, tweetUrl, username);

					process.EnableRaisingEvents = true;
					process.Exited += process_Exited;

					process.BeginOutputReadLine();
					process.OutputDataReceived += process_OutputDataReceived;

					pipe.DisposeToken();
				}
				else {
					pipe.DataIn -= pipe_DataIn;
					pipe.Dispose();
				}
			} catch (Exception e) {
				App.ErrorHandler.HandleException("Video Playback Error", "Error launching video player.", true, e);
			}
		}

		public void SendKeyEvent(Keys key) {
			currentInstance?.Pipe.Write("key", ((int) key).ToString());
		}

		private void pipe_DataIn(object? sender, DuplexPipe.PipeReadEventArgs e) {
			owner.InvokeSafe(() => {
				switch (e.Key) {
					case "vol":
						if (int.TryParse(e.Data, out int volume) && volume != Config.VideoPlayerVolume) {
							Config.VideoPlayerVolume = volume;
							Config.Save();
						}

						break;

					case "download":
						if (currentInstance != null) {
							owner.SaveVideo(currentInstance.VideoUrl, currentInstance.Username);
						}

						break;

					case "rip":
						currentInstance?.Dispose();
						currentInstance = null;

						isClosing = false;
						TriggerProcessExitEventUnsafe();
						break;
				}
			});
		}

		public void Close() {
			if (currentInstance != null) {
				if (isClosing) {
					Destroy();
					isClosing = false;
				}
				else {
					isClosing = true;
					currentInstance.Process.Exited -= process_Exited;
					currentInstance.Pipe.Write("die");
				}
			}
		}

		public void Dispose() {
			ProcessExited = null;

			isClosing = true;
			Destroy();
		}

		private void Destroy() {
			if (currentInstance != null) {
				currentInstance.KillAndDispose();
				currentInstance = null;

				TriggerProcessExitEventUnsafe();
			}
		}

		private void owner_FormClosing(object? sender, FormClosingEventArgs e) {
			if (currentInstance != null) {
				currentInstance.Process.Exited -= process_Exited;
			}
		}

		private void process_OutputDataReceived(object? sender, DataReceivedEventArgs e) {
			if (!string.IsNullOrEmpty(e.Data)) {
				App.Logger.Debug("[VideoPlayer] " + e.Data);
			}
		}

		private void process_Exited(object? sender, EventArgs e) {
			if (currentInstance == null) {
				return;
			}

			int exitCode = currentInstance.Process.ExitCode;
			string tweetUrl = currentInstance.TweetUrl;

			currentInstance.Dispose();
			currentInstance = null;

			switch (exitCode) {
				case 3: // CODE_LAUNCH_FAIL
					if (FormMessage.Error("Video Playback Error", "Error launching video player, this may be caused by missing Windows Media Player. Do you want to open the video in your browser?", FormMessage.Yes, FormMessage.No)) {
						App.SystemHandler.OpenBrowser(tweetUrl);
					}

					break;

				case 4: // CODE_MEDIA_ERROR
					if (FormMessage.Error("Video Playback Error", "The video could not be loaded, most likely due to unknown format. Do you want to open the video in your browser?", FormMessage.Yes, FormMessage.No)) {
						App.SystemHandler.OpenBrowser(tweetUrl);
					}

					break;
			}

			owner.InvokeAsyncSafe(TriggerProcessExitEventUnsafe);
		}

		private void TriggerProcessExitEventUnsafe() {
			ProcessExited?.Invoke(this, EventArgs.Empty);
		}

		private sealed class Instance : IDisposable {
			public bool Running {
				get {
					Process.Refresh();
					return !Process.HasExited;
				}
			}

			public Process Process { get; }
			public DuplexPipe.Server Pipe { get; }

			public string VideoUrl { get; }
			public string TweetUrl { get; }
			public string Username { get; }

			public Instance(Process process, DuplexPipe.Server pipe, string videoUrl, string tweetUrl, string username) {
				this.Process = process;
				this.Pipe = pipe;
				this.VideoUrl = videoUrl;
				this.TweetUrl = tweetUrl;
				this.Username = username;
			}

			public void KillAndDispose() {
				try {
					Process.Kill();
				} catch {
					// kill me instead then
				}

				Dispose();
			}

			public void Dispose() {
				Process.Dispose();
				Pipe.Dispose();
			}
		}
	}
}
