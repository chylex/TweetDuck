namespace TweetLib.Core.Features.Configuration{
    public interface IConfigManager{
        void TriggerProgramRestartRequested();
        IConfigInstance<BaseConfig> GetInstanceInfo(BaseConfig instance);
    }
}
