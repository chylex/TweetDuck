using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Lunixo.ChromiumGtk.Core;
using TweetDuck.Application;
using TweetDuck.Configuration;
using TweetDuck.Utils;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.Request;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Utils.Collections;
using Xilium.CefGlue;
using TweetDeckBrowser = TweetDuck.Browser.TweetDeckBrowser;

namespace TweetDuck {
	static class Program {
		private const string CefDataFolder = "TD_Chromium";
		private const string ConsoleLogFile = "TD_Console.txt";

		[STAThread]
		private static void Main(string[] args) {
			var arguments = CommandLineArgs.FromStringArray('-', args);
			if (arguments.HasFlag("-appversion")) {
				var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
				version = version?[..version.LastIndexOf('.')];
				Console.WriteLine(version ?? "unknown");
				return;
			}
			else if (arguments.HasFlag("-pluginversion")) {
				Console.WriteLine(Plugin.LibVersion);
				return;
			}

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			Gtk.Application.Init();

			Lib.AppLauncher launch = Lib.Initialize(new AppBuilder {
				Setup = new Setup(arguments),
				ErrorHandler = new ErrorHandler(),
				SystemHandler = new SystemHandler(),
				MessageDialogs = new MessageDialogs(),
				FileDialogs = new FileDialogs()
			});

			launch();
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Debug.WriteLine(e.ExceptionObject);
		}

		private sealed class Setup : IAppSetup {
			public bool IsPortable { get; }
			public bool IsDebugLogging => true;
			public string? CustomDataFolder { get; }
			public string? ResourceRewriteRules => null;

			public Setup(CommandLineArgs args) {
				CustomDataFolder = args.GetValue("-datafolder");
				IsPortable = CustomDataFolder == null;
			}

			public ConfigManager CreateConfigManager(string storagePath) {
				var objects = new ConfigObjects<UserConfiguration, SystemConfiguration>(
					new UserConfiguration(),
					new SystemConfiguration(),
					new PluginConfig(Array.Empty<string>())
				);

				return new ConfigManager<UserConfiguration, SystemConfiguration>(storagePath, objects);
			}

			public bool TryLockDataFolder(string lockFile) {
				return true;
			}

			public void BeforeLaunch() {}

			public void Launch(ResourceCache resourceCache, PluginManager pluginManager) {
				var programPath = App.ProgramPath;
				var storagePath = App.StoragePath;

				#if DEBUG
				const CefLogSeverity LogSeverity = CefLogSeverity.Debug;
				#else
				const CefLogSeverity LogSeverity = CefLogSeverity.Disable;
				#endif

				var runtime = new Runtime(new CefSettings {
					BackgroundColor = new CefColor(255, 28, 99, 153),
					CachePath = storagePath,
					BrowserSubprocessPath = Path.Combine(programPath, Lib.BrandName + ".Browser"),
					UserDataPath = Path.Combine(storagePath, CefDataFolder),
					LogFile = Path.Combine(storagePath, ConsoleLogFile),
					LogSeverity = LogSeverity,
					MultiThreadedMessageLoop = false,
					ExternalMessagePump = false
				}, Array.Empty<string>());

				runtime.Initialize(new CustomCefApp(resourceCache, pluginManager));

				using var window = WindowManager.CreateMainWindow(Lib.BrandName);
				window.Destroyed += (_, _) => {
					runtime.QuitMessageLoop();
					App.Close();
				};

				using var tweetDeckBrowser = new TweetDeckBrowser(window, pluginManager);
				window.Add(tweetDeckBrowser.View);
				window.ShowAll();
				runtime.RunMessageLoop();
				runtime.Shutdown();
			}
		}

		private sealed class CustomCefApp : CefApp {
			private readonly ResourceCache resourceCache;
			private readonly PluginManager pluginManager;

			public CustomCefApp(ResourceCache resourceCache, PluginManager pluginManager) {
				this.resourceCache = resourceCache;
				this.pluginManager = pluginManager;
			}

			protected override void OnRegisterCustomSchemes(CefSchemeRegistrar registrar) {
				SchemeHandlerFactory.Register(registrar, new TweetDuckSchemeHandler(resourceCache));
				SchemeHandlerFactory.Register(registrar, new PluginSchemeHandler(resourceCache, pluginManager));
			}
		}
	}
}
