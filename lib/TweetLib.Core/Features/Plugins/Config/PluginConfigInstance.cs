using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetLib.Core.Systems.Configuration;

namespace TweetLib.Core.Features.Plugins.Config {
	sealed class PluginConfigInstance : IConfigInstance {
		private readonly PluginConfig instance;
		private readonly string filename;

		public PluginConfigInstance(string filename, PluginConfig instance) {
			this.instance = instance;
			this.filename = filename;
		}

		public void Load() {
			try {
				using var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
				string? line = reader.ReadLine();

				if (line == "#Disabled") {
					var newDisabled = new HashSet<string>();

					while ((line = reader.ReadLine()) != null) {
						newDisabled.Add(line);
					}

					instance.Reset(newDisabled);
				}
			} catch (FileNotFoundException) {
				// ignore
			} catch (DirectoryNotFoundException) {
				// ignore
			} catch (Exception e) {
				OnException("Could not read the plugin configuration file. If you continue, the list of disabled plugins will be reset to default.", e);
			}
		}

		public void Save() {
			try {
				using var writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8);
				writer.WriteLine("#Disabled");

				foreach (string identifier in instance.DisabledPlugins) {
					writer.WriteLine(identifier);
				}
			} catch (Exception e) {
				OnException("Could not save the plugin configuration file.", e);
			}
		}

		public void Reload() {
			Load();
		}

		public void Reset() {
			try {
				File.Delete(filename);
				instance.ResetToDefault();
			} catch (Exception e) {
				OnException("Could not delete the plugin configuration file.", e);
				return;
			}

			Reload();
		}

		private static void OnException(string message, Exception e) {
			App.ErrorHandler.HandleException("Plugin Configuration Error", message, true, e);
		}
	}
}
