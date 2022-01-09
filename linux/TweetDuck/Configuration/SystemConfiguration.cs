using TweetLib.Core.Application;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	sealed class SystemConfiguration : IConfigObject<SystemConfiguration>, IAppSystemConfiguration {
		public bool UseSystemProxyForAllConnections => true;

		public SystemConfiguration ConstructWithDefaults() {
			return new SystemConfiguration();
		}
	}
}
