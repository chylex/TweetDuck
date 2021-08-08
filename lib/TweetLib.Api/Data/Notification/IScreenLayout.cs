using System.Collections.Generic;

namespace TweetLib.Api.Data.Notification {
	public interface IScreenLayout {
		IScreen PrimaryScreen { get; }
		IScreen TweetDuckScreen { get; }
		List<IScreen> AllScreens { get; }
	}
}
