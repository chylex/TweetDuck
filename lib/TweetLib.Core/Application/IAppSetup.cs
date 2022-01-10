using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Resources;
using TweetLib.Core.Systems.Configuration;

namespace TweetLib.Core.Application {
	public interface IAppSetup {
		bool IsPortable { get; }
		bool IsDebugLogging { get; }
		string? CustomDataFolder { get; }
		string? ResourceRewriteRules { get; }

		ConfigManager CreateConfigManager(string storagePath);

		bool TryLockDataFolder(string lockFile);

		void BeforeLaunch();

		void Launch(ResourceCache resourceCache, PluginManager pluginManager);
	}
}
