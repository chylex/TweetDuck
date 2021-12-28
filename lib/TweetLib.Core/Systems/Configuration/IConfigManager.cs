namespace TweetLib.Core.Systems.Configuration {
	public interface IConfigManager {
		IConfigInstance<BaseConfig> GetInstanceInfo(BaseConfig instance);
	}
}
