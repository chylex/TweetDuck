using System;
using TweetLib.Api;
using TweetLib.Api.Data;
using TweetLib.Core;

namespace TweetDuck.Application {
	static class ApiServices {
		public static void Register() {
		}

		internal static NamespacedResource Namespace(Resource path) {
			TweetDuckExtension currentExtension = App.Api.CurrentExtension ?? throw new InvalidOperationException("Cannot use API services outside of designated method calls.");
			return new NamespacedResource(currentExtension.Id, path);
		}
	}
}
