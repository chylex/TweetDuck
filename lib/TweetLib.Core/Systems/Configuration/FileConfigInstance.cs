using System;
using System.IO;
using TweetLib.Utils.Serialization;

namespace TweetLib.Core.Systems.Configuration {
	sealed class FileConfigInstance<T> : IConfigInstance where T : IConfigObject<T> {
		private readonly T instance;
		private readonly SimpleObjectSerializer<T> serializer;

		private readonly string filenameMain;
		private readonly string filenameBackup;
		private readonly string identifier;

		public FileConfigInstance(string filename, T instance, string identifier, TypeConverterRegistry converterRegistry) {
			this.instance = instance;
			this.serializer = new SimpleObjectSerializer<T>(converterRegistry);

			this.filenameMain = filename ?? throw new ArgumentNullException(nameof(filename), "Config file name must not be null!");
			this.filenameBackup = filename + ".bak";
			this.identifier = identifier;
		}

		private void LoadInternal(bool backup) {
			serializer.Read(backup ? filenameBackup : filenameMain, instance);
		}

		public void Load() {
			Exception? firstException = null;

			for (int attempt = 0; attempt < 2; attempt++) {
				try {
					LoadInternal(attempt > 0);

					if (firstException != null) { // silently log exception that caused a backup restore
						App.Logger.Error(firstException.ToString());
					}

					return;
				} catch (FileNotFoundException) {
					// ignore
				} catch (DirectoryNotFoundException) {
					break;
				} catch (Exception e) {
					firstException ??= e;
				}
			}

			if (firstException is FormatException) {
				OnException($"The configuration file for {identifier} is outdated or corrupted. If you continue, your {identifier} will be reset.", firstException);
			}
			else if (firstException is SerializationSoftException sse) {
				OnException($"{sse.Errors.Count} error{(sse.Errors.Count == 1 ? " was" : "s were")} encountered while loading the configuration file for {identifier}. If you continue, some of your {identifier} will be reset.", firstException);
			}
			else if (firstException != null) {
				OnException($"Could not open the configuration file for {identifier}. If you continue, your {identifier} will be reset.", firstException);
			}
		}

		public void Save() {
			try {
				if (File.Exists(filenameMain)) {
					File.Delete(filenameBackup);
					File.Move(filenameMain, filenameBackup);
				}

				serializer.Write(filenameMain, instance);
			} catch (SerializationSoftException e) {
				OnException($"{e.Errors.Count} error{(e.Errors.Count == 1 ? " was" : "s were")} encountered while saving the configuration file for {identifier}.", e);
			} catch (Exception e) {
				OnException($"Could not save the configuration file for {identifier}.", e);
			}
		}

		public void Reload() {
			try {
				LoadInternal(false);
			} catch (FileNotFoundException) {
				try {
					serializer.Write(filenameMain, instance.ConstructWithDefaults());
					LoadInternal(false);
				} catch (Exception e) {
					OnException($"Could not regenerate the configuration file for {identifier}.", e);
				}
			} catch (Exception e) {
				OnException($"Could not reload the configuration file for {identifier}.", e);
			}
		}

		public void Reset() {
			try {
				File.Delete(filenameMain);
				File.Delete(filenameBackup);
			} catch (Exception e) {
				OnException($"Could not delete configuration files to reset {identifier}.", e);
				return;
			}

			Reload();
		}

		private static void OnException(string message, Exception e) {
			App.ErrorHandler.HandleException("Configuration Error", message, true, e);
		}
	}
}
