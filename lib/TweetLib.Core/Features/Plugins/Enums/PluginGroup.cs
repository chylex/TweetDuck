using System;
using System.Collections.Generic;

namespace TweetLib.Core.Features.Plugins.Enums {
	public enum PluginGroup {
		Official,
		Custom
	}

	public static class PluginGroups {
		public static IEnumerable<PluginGroup> All { get; } = new PluginGroup[] {
			PluginGroup.Official,
			PluginGroup.Custom
		};

		public static string GetSubFolder(this PluginGroup group) {
			return group switch {
				PluginGroup.Official => "official",
				PluginGroup.Custom   => "user",
				_                    => throw new InvalidOperationException($"Invalid plugin group: {group}")
			};
		}

		public static string GetIdentifierPrefix(this PluginGroup group) {
			return group switch {
				PluginGroup.Official => "official/",
				PluginGroup.Custom   => "custom/",
				_                    => "unknown/"
			};
		}
	}
}
