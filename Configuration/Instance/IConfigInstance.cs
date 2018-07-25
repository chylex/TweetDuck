namespace TweetDuck.Configuration.Instance{
    interface IConfigInstance<out T>{
        T Instance { get; }

        void Save();
        void Reload();
        void Reset();
    }
}
