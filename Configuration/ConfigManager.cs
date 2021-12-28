using System;
using System.Drawing;
using TweetDuck.Dialogs;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Utils.Serialization.Converters;
using TweetLib.Utils.Static;

namespace TweetDuck.Configuration {
	sealed class ConfigManager : IConfigManager {
		internal sealed class Paths {
			public string UserConfig { get; set; }
			public string SystemConfig { get; set; }
			public string PluginConfig { get; set; }
		}

		public Paths FilePaths { get; }

		public UserConfig User { get; }
		public SystemConfig System { get; }
		public PluginConfig Plugins { get; }

		public event EventHandler ProgramRestartRequested;

		private readonly FileConfigInstance<UserConfig> infoUser;
		private readonly FileConfigInstance<SystemConfig> infoSystem;
		private readonly PluginConfigInstance<PluginConfig> infoPlugins;

		private readonly IConfigInstance<BaseConfig>[] infoList;

		public ConfigManager(UserConfig userConfig, Paths paths) {
			FilePaths = paths;

			User = userConfig;
			System = new SystemConfig();
			Plugins = new PluginConfig();

			infoList = new IConfigInstance<BaseConfig>[] {
				infoUser = new FileConfigInstance<UserConfig>(paths.UserConfig, User, "program options"),
				infoSystem = new FileConfigInstance<SystemConfig>(paths.SystemConfig, System, "system options"),
				infoPlugins = new PluginConfigInstance<PluginConfig>(paths.PluginConfig, Plugins)
			};

			// TODO refactor further

			infoUser.Serializer.RegisterTypeConverter(typeof(WindowState), WindowState.Converter);

			infoUser.Serializer.RegisterTypeConverter(typeof(Point), new BasicTypeConverter<Point> {
				ConvertToString = value => $"{value.X} {value.Y}",
				ConvertToObject = value => {
					int[] elements = StringUtils.ParseInts(value, ' ');
					return new Point(elements[0], elements[1]);
				}
			});

			infoUser.Serializer.RegisterTypeConverter(typeof(Size), new BasicTypeConverter<Size> {
				ConvertToString = value => $"{value.Width} {value.Height}",
				ConvertToObject = value => {
					int[] elements = StringUtils.ParseInts(value, ' ');
					return new Size(elements[0], elements[1]);
				}
			});
		}

		public void LoadAll() {
			infoUser.Load();
			infoSystem.Load();
			infoPlugins.Load();
		}

		public void SaveAll() {
			infoUser.Save();
			infoSystem.Save();
			infoPlugins.Save();
		}

		public void ReloadAll() {
			infoUser.Reload();
			infoSystem.Reload();
			infoPlugins.Reload();
		}

		public void Save(BaseConfig instance) {
			((IConfigManager) this).GetInstanceInfo(instance).Save();
		}

		public void Reset(BaseConfig instance) {
			((IConfigManager) this).GetInstanceInfo(instance).Reset();
		}

		public void TriggerProgramRestartRequested() {
			ProgramRestartRequested?.Invoke(this, EventArgs.Empty);
		}

		IConfigInstance<BaseConfig> IConfigManager.GetInstanceInfo(BaseConfig instance) {
			Type instanceType = instance.GetType();
			return Array.Find(infoList, info => info.Instance.GetType() == instanceType); // TODO handle null
		}
	}

	static class ConfigManagerExtensions {
		public static void Save(this BaseConfig instance) {
			Program.Config.Save(instance);
		}

		public static void Reset(this BaseConfig instance) {
			Program.Config.Reset(instance);
		}
	}
}
