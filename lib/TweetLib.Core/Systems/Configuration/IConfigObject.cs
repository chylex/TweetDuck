namespace TweetLib.Core.Systems.Configuration {
	public interface IConfigObject {}

	public interface IConfigObject<T> : IConfigObject where T : IConfigObject<T> {
		T ConstructWithDefaults();
	}
}
