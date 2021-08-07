using TweetLib.Api.Data;

namespace TweetLib.Api {
	public abstract class TweetDuckExtension {
		/// <summary>
		/// Unique identifier of the extension.
		/// </summary>
		public abstract Resource Id { get; }

		/// <summary>
		/// Called when the extension is loaded on startup, or enabled at runtime.
		/// </summary>
		public abstract void Enable(ITweetDuckApi api);
	}
}
