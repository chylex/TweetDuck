using System;
using System.Collections.Generic;

namespace TweetLib.Core.Features.Plugins.Enums {
	public enum PluginEnvironment {
		Browser,
		Notification
	}

	internal static class PluginEnvironments {
		public static IEnumerable<PluginEnvironment> All { get; } = new PluginEnvironment[] {
			PluginEnvironment.Browser,
			PluginEnvironment.Notification
		};

		public static string GetPluginScriptNamespace(this PluginEnvironment environment) {
			return environment switch {
				PluginEnvironment.Browser      => "tweetdeck",
				PluginEnvironment.Notification => "notification",
				_                              => throw new InvalidOperationException($"Invalid plugin environment: {environment}")
			};
		}

		public static string GetPluginScriptFile(this PluginEnvironment environment) {
			return environment switch {
				PluginEnvironment.Browser      => "browser.js",
				PluginEnvironment.Notification => "notification.js",
				_                              => throw new InvalidOperationException($"Invalid plugin environment: {environment}")
			};
		}

		public static string GetPluginScriptVariables(this PluginEnvironment environment) {
			return environment switch {
				PluginEnvironment.Browser      => "$,$TD,$TDP,TD",
				PluginEnvironment.Notification => "$TD,$TDP",
				_                              => string.Empty
			};
		}
	}
}
