using TweetLib.Core.Application;
using TweetLib.Core.Features.Plugins.Config;

namespace TweetLib.Core.Systems.Configuration {
	public sealed class ConfigObjects<TUser, TSystem> where TUser : IAppUserConfiguration where TSystem : IConfigObject {
		public TUser User { get; }
		public TSystem System { get; }
		public PluginConfig Plugins { get; }

		public ConfigObjects(TUser user, TSystem system, PluginConfig plugins) {
			User = user;
			System = system;
			Plugins = plugins;
		}
	}
}
