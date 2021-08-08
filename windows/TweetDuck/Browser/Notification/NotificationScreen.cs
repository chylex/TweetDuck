using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Application;
using TweetDuck.Application.Service;
using TweetLib.Api.Data;
using TweetLib.Api.Data.Notification;
using TweetLib.Utils.Serialization.Converters;
using TweetLib.Utils.Static;

namespace TweetDuck.Browser.Notification {
	abstract class NotificationScreen {
		public static List<NotificationScreen> All {
			get {
				var list = new List<NotificationScreen> {
					SameAsTweetDuck.Instance
				};

				for (int index = 1; index <= Screen.AllScreens.Length; index++) {
					list.Add(new Static(index));
				}

				foreach (var provider in ApiServices.Notifications.GetDesktopNotificationScreenProviders()) {
					list.Add(new Provided(provider.NamespacedId));
				}

				return list;
			}
		}

		public abstract string DisplayName { get; }

		private NotificationScreen() {}

		public abstract Screen PickScreen(FormBrowser mainWindow);

		protected abstract string Serialize();

		public sealed class SameAsTweetDuck : NotificationScreen {
			public static SameAsTweetDuck Instance { get; } = new SameAsTweetDuck();

			public override string DisplayName => "(Same as TweetDuck)";

			private SameAsTweetDuck() {}

			public override Screen PickScreen(FormBrowser mainWindow) {
				return Screen.FromControl(mainWindow);
			}

			protected override string Serialize() {
				return "0";
			}

			public override bool Equals(object? obj) {
				return obj is SameAsTweetDuck;
			}

			public override int GetHashCode() {
				return 1828695039;
			}
		}

		private sealed class Static : NotificationScreen {
			public override string DisplayName {
				get {
					Screen? screen = Screen;
					if (screen == null) {
						return $"Unknown ({screenIndex})";
					}

					return screen.DeviceName.TrimStart('\\', '.') + $" ({screen.Bounds.Width}x{screen.Bounds.Height})";
				}
			}

			private Screen? Screen => screenIndex >= 1 && screenIndex <= Screen.AllScreens.Length ? Screen.AllScreens[screenIndex - 1] : null;

			private readonly int screenIndex;

			public Static(int screenIndex) {
				this.screenIndex = screenIndex;
			}

			public override Screen PickScreen(FormBrowser mainWindow) {
				return Screen ?? SameAsTweetDuck.Instance.PickScreen(mainWindow);
			}

			protected override string Serialize() {
				return screenIndex.ToString();
			}

			public override bool Equals(object? obj) {
				return obj is Static other && screenIndex == other.screenIndex;
			}

			public override int GetHashCode() {
				return 31 * screenIndex;
			}
		}

		private sealed class Provided : NotificationScreen {
			public override string DisplayName => Provider?.DisplayName ?? $"Unknown ({resource})";

			private readonly NamespacedResource resource;

			private NotificationService.NamespacedProvider? provider;
			private NotificationService.NamespacedProvider? Provider => provider ??= ApiServices.Notifications.GetDesktopNotificationScreenProviders().Find(p => p.NamespacedId == resource);

			public Provided(NamespacedResource resource) {
				this.resource = resource;
			}

			public override Screen PickScreen(FormBrowser mainWindow) {
				IScreen? pick = Provider?.PickScreen(new WindowsFormsScreenLayout(mainWindow));
				return pick is WindowsFormsScreen screen ? screen.Screen : SameAsTweetDuck.Instance.PickScreen(mainWindow); // TODO
			}

			protected override string Serialize() {
				return resource.Namespace + ":" + resource.Path;
			}

			public override bool Equals(object? obj) {
				return obj is Provided other && resource == other.resource;
			}

			public override int GetHashCode() {
				return resource.GetHashCode();
			}

			private sealed class WindowsFormsScreenLayout : IScreenLayout {
				public IScreen PrimaryScreen => new WindowsFormsScreen(Screen.PrimaryScreen);
				public IScreen TweetDuckScreen => new WindowsFormsScreen(Screen.FromControl(mainWindow));
				public List<IScreen> AllScreens => Screen.AllScreens.Select(static screen => new WindowsFormsScreen(screen)).ToList<IScreen>();

				private readonly FormBrowser mainWindow;

				public WindowsFormsScreenLayout(FormBrowser mainWindow) {
					this.mainWindow = mainWindow;
				}
			}

			private sealed class WindowsFormsScreen : IScreen {
				public Screen Screen { get; }
				public ScreenBounds Bounds { get; }
				public string Name => Screen.DeviceName;
				public bool IsPrimary => Screen.Primary;

				public WindowsFormsScreen(Screen screen) {
					this.Screen = screen;
					this.Bounds = new ScreenBounds(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
				}
			}
		}

		public static readonly BasicTypeConverter<NotificationScreen> Converter = new()  {
			ConvertToString = static value => value.Serialize(),
			ConvertToObject = static value => {
				if (value == "0") {
					return SameAsTweetDuck.Instance;
				}
				else if (int.TryParse(value, out int index)) {
					return new Static(index);
				}

				var resource = StringUtils.SplitInTwo(value, ':');
				if (resource != null) {
					return new Provided(new NamespacedResource(new Resource(resource.Value.before), new Resource(resource.Value.after)));
				}

				return SameAsTweetDuck.Instance;
			}
		};
	}
}
