using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetLib.Core.Application;

namespace TweetDuck.Resources {
	class ScriptLoader : IAppResourceHandler {
		private readonly Dictionary<string, string> cache = new Dictionary<string, string>(16);
		private Control sync;

		public void Initialize(Control sync) {
			this.sync = sync;
		}

		protected void ClearCache() {
			cache.Clear();
		}

		public virtual void OnReloadTriggered() {
			if (Control.ModifierKeys.HasFlag(Keys.Shift)) {
				ClearCache();
			}
		}

		public string Load(string path)       => LoadInternal(path, silent: false);
		public string LoadSilent(string path) => LoadInternal(path, silent: true);

		protected virtual string LocateFile(string path) {
			return Path.Combine(Program.ScriptPath, path);
		}

		private string LoadInternal(string path, bool silent) {
			if (sync == null) {
				throw new InvalidOperationException("Cannot use ScriptLoader before initialization.");
			}
			else if (sync.IsDisposed) {
				return null; // better than crashing I guess...?
			}

			if (cache.TryGetValue(path, out string resourceData)) {
				return resourceData;
			}

			string location = LocateFile(path);
			string resource;

			try {
				string contents = File.ReadAllText(location, Encoding.UTF8);
				int separator;

				// first line can be either:
				// #<version>\r\n
				// #<version>\n

				if (contents[0] != '#') {
					ShowLoadError(silent ? null : sync, $"File {path} appears to be corrupted, please try reinstalling the app.");
					separator = 0;
				}
				else {
					separator = contents.IndexOf('\n');
					string fileVersion = contents.Substring(1, separator - 1).TrimEnd();

					if (fileVersion != Program.VersionTag) {
						ShowLoadError(silent ? null : sync, $"File {path} is made for a different version of TweetDuck ({fileVersion}) and may not function correctly in this version, please try reinstalling the app.");
					}
				}

				resource = contents.Substring(separator).TrimStart();
			} catch (Exception ex) {
				ShowLoadError(silent ? null : sync, $"Could not load {path}. The program will continue running with limited functionality.\n\n{ex.Message}");
				resource = null;
			}

			return cache[path] = resource;
		}

		private static void ShowLoadError(Control sync, string message) {
			sync?.InvokeSafe(() => FormMessage.Error("Resource Error", message, FormMessage.OK));
		}
	}
}
