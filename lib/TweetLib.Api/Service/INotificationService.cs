using TweetLib.Api.Data.Notification;

namespace TweetLib.Api.Service {
	public interface INotificationService : ITweetDuckService {
		void RegisterDesktopNotificationScreenProvider(IDesktopNotificationScreenProvider provider);
	}
}
