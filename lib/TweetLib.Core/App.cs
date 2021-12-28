using System;
using System.IO;
using TweetLib.Core.Application;
using TweetLib.Utils.Static;

namespace TweetLib.Core {
	public static class App {
		public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
		public static readonly bool IsPortable = File.Exists(Path.Combine(ProgramPath, "makeportable"));

		public static readonly string ResourcesPath = Path.Combine(ProgramPath, "resources");
		public static readonly string PluginPath    = Path.Combine(ProgramPath, "plugins");
		public static readonly string GuidePath     = Path.Combine(ProgramPath, "guide");

		public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : Validate(Builder.Startup, nameof(Builder.Startup)).GetDataFolder();

		public static IAppLogger Logger                       { get; } = Validate(Builder.Logger, nameof(Builder.Logger));
		public static IAppErrorHandler ErrorHandler           { get; } = Validate(Builder.ErrorHandler, nameof(Builder.ErrorHandler));
		public static IAppSystemHandler SystemHandler         { get; } = Validate(Builder.SystemHandler, nameof(Builder.SystemHandler));
		public static IAppDialogHandler DialogHandler         { get; } = Validate(Builder.DialogHandler, nameof(Builder.DialogHandler));
		public static IAppUserConfiguration UserConfiguration { get; } = Validate(Builder.UserConfiguration, nameof(Builder.UserConfiguration));

		public static void Launch() {
			if (!FileUtils.CheckFolderWritePermission(StoragePath)) {
				throw new AppException("Permission Error", "TweetDuck does not have write permissions to the storage folder: " + StoragePath);
			}
		}

		// Setup

		private static AppBuilder Builder => AppBuilder.Instance ?? throw new InvalidOperationException("App is initializing too early");

		private static bool isInitialized = false;

		internal static void Initialize() {
			if (isInitialized) {
				throw new InvalidOperationException("App is already initialized");
			}

			isInitialized = true;
		}

		private static T Validate<T>(T? obj, string name) where T : class {
			return obj ?? throw new InvalidOperationException("Missing property " + name + " on the provided App");
		}
	}

	public sealed class AppBuilder {
		public AppStartup? Startup { get; set; }
		public IAppLogger? Logger { get; set; }
		public IAppErrorHandler? ErrorHandler { get; set; }
		public IAppSystemHandler? SystemHandler { get; set; }
		public IAppDialogHandler? DialogHandler { get; set; }
		public IAppUserConfiguration? UserConfiguration { get; set; }

		internal static AppBuilder? Instance { get; private set; }

		internal void Build() {
			Instance = this;
			App.Initialize();
			Instance = null;
		}
	}
}
