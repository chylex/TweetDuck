namespace TweetLib.Api {
	public interface ITweetDuckApi {
		T? FindService<T>() where T : class, ITweetDuckService;
	}
}
