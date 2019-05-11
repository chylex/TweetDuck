namespace TweetLib.Core.Features.Configuration{
    public interface IConfigInstance<out T>{
        T Instance { get; }

        void Save();
        void Reload();
        void Reset();
    }
}
