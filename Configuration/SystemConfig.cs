using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	sealed class SystemConfig : BaseConfig {
		// CONFIGURATION DATA

		private bool _hardwareAcceleration = true;

		public bool ClearCacheAutomatically { get; set; } = true;
		public int ClearCacheThreshold      { get; set; } = 250;

		// SPECIAL PROPERTIES

		public bool HardwareAcceleration {
			get => _hardwareAcceleration;
			set => UpdatePropertyWithRestartRequest(ref _hardwareAcceleration, value);
		}

		// END OF CONFIG

		public SystemConfig(IConfigManager configManager) : base(configManager) {}

		protected override BaseConfig ConstructWithDefaults(IConfigManager configManager) {
			return new SystemConfig(configManager);
		}
	}
}
