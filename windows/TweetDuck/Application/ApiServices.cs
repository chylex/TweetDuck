using System;
using TweetDuck.Application.Service;
using TweetLib.Api;
using TweetLib.Api.Data;
using TweetLib.Api.Service;
using TweetLib.Core;

namespace TweetDuck.Application {
	static class ApiServices {
		public static NotificationService Notifications { get; } = new NotificationService();

		public static void Register() {
			App.Api.RegisterService<INotificationService>(Notifications);
		}

		internal static NamespacedResource Namespace(Resource path) {
			TweetDuckExtension currentExtension = App.Api.CurrentExtension ?? throw new InvalidOperationException("Cannot use API services outside of designated method calls.");
			return new NamespacedResource(currentExtension.Id, path);
		}
	}
}
