using TweetLib.Core;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	sealed class SystemConfig : BaseConfig<SystemConfig> {
		private bool _hardwareAcceleration = true;

		public bool ClearCacheAutomatically { get; set; } = true;
		public int ClearCacheThreshold      { get; set; } = 250;

		// SPECIAL PROPERTIES

		public bool HardwareAcceleration {
			get => _hardwareAcceleration;
			set => UpdatePropertyWithCallback(ref _hardwareAcceleration, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		// END OF CONFIG

		public override SystemConfig ConstructWithDefaults() {
			return new SystemConfig();
		}
	}
}
