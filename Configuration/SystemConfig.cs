using System.Diagnostics.CodeAnalysis;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	sealed class SystemConfig : BaseConfig<SystemConfig>, IAppSystemConfiguration {
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public int MigrationVersion { get; set; } = 0;

		private bool _hardwareAcceleration = true;
		private bool _enableTouchAdjustment = false;
		private bool _enableColorProfileDetection = false;
		private bool _useSystemProxyForAllConnections = false;

		public bool ClearCacheAutomatically { get; set; } = true;
		public int ClearCacheThreshold      { get; set; } = 250;

		// SPECIAL PROPERTIES

		public bool HardwareAcceleration {
			get => _hardwareAcceleration;
			set => UpdatePropertyWithCallback(ref _hardwareAcceleration, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		public bool EnableTouchAdjustment {
			get => _enableTouchAdjustment;
			set => UpdatePropertyWithCallback(ref _enableTouchAdjustment, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		public bool EnableColorProfileDetection {
			get => _enableColorProfileDetection;
			set => UpdatePropertyWithCallback(ref _enableColorProfileDetection, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		public bool UseSystemProxyForAllConnections {
			get => _useSystemProxyForAllConnections;
			set => UpdatePropertyWithCallback(ref _useSystemProxyForAllConnections, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		// END OF CONFIG

		#pragma warning disable CS0618

		public bool Migrate() {
			bool hasChanged = false;

			if (MigrationVersion < 1) {
				MigrationVersion = 1;
				hasChanged = true;

				var userConfig = Program.Config.User;
				_enableTouchAdjustment = userConfig.EnableTouchAdjustment;
				_enableColorProfileDetection = userConfig.EnableColorProfileDetection;
				_useSystemProxyForAllConnections = userConfig.UseSystemProxyForAllConnections;
			}

			return hasChanged;
		}

		#pragma warning restore CS0618

		public override SystemConfig ConstructWithDefaults() {
			return new SystemConfig {
				MigrationVersion = 1
			};
		}
	}
}
