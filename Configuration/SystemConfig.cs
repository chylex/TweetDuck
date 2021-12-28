using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	sealed class SystemConfig : BaseConfig {
		private bool _hardwareAcceleration = true;

		public bool ClearCacheAutomatically { get; set; } = true;
		public int ClearCacheThreshold      { get; set; } = 250;

		// SPECIAL PROPERTIES

		public bool HardwareAcceleration {
			get => _hardwareAcceleration;
			set => UpdatePropertyWithCallback(ref _hardwareAcceleration, value, Program.Config.TriggerProgramRestartRequested);
		}

		// END OF CONFIG

		protected override BaseConfig ConstructWithDefaults() {
			return new SystemConfig();
		}
	}
}
