namespace TweetLib.Core.Systems.Configuration {
	public interface IConfigManager {
		void TriggerProgramRestartRequested();
		IConfigInstance<BaseConfig> GetInstanceInfo(BaseConfig instance);
	}
}
