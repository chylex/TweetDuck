using System;
using System.Drawing;
using System.IO;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Utils.Data;
using TweetLib.Utils.Serialization;
using TweetLib.Utils.Serialization.Converters;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Systems.Configuration {
	public abstract class ConfigManager {
		protected static TypeConverterRegistry ConverterRegistry { get; } = new ();

		static ConfigManager() {
			ConverterRegistry.Register(typeof(WindowState), new BasicTypeConverter<WindowState> {
				ConvertToString = static value => $"{(value.IsMaximized ? 'M' : '_')}{value.Bounds.X} {value.Bounds.Y} {value.Bounds.Width} {value.Bounds.Height}",
				ConvertToObject = static value => {
					int[] elements = StringUtils.ParseInts(value.Substring(1), ' ');

					return new WindowState {
						Bounds = new Rectangle(elements[0], elements[1], elements[2], elements[3]),
						IsMaximized = value[0] == 'M'
					};
				}
			});

			ConverterRegistry.Register(typeof(Point), new BasicTypeConverter<Point> {
				ConvertToString = static value => $"{value.X} {value.Y}",
				ConvertToObject = static value => {
					int[] elements = StringUtils.ParseInts(value, ' ');
					return new Point(elements[0], elements[1]);
				}
			});

			ConverterRegistry.Register(typeof(Size), new BasicTypeConverter<Size> {
				ConvertToString = static value => $"{value.Width} {value.Height}",
				ConvertToObject = static value => {
					int[] elements = StringUtils.ParseInts(value, ' ');
					return new Size(elements[0], elements[1]);
				}
			});
		}

		public string UserPath { get; }
		public string SystemPath { get; }
		public string PluginsPath { get; }

		public event EventHandler? ProgramRestartRequested;

		internal IAppUserConfiguration User { get; }
		internal PluginConfig Plugins { get; }

		protected ConfigManager(string storagePath, IAppUserConfiguration user, PluginConfig plugins) {
			UserPath = Path.Combine(storagePath, "TD_UserConfig.cfg");
			SystemPath = Path.Combine(storagePath, "TD_SystemConfig.cfg");
			PluginsPath = Path.Combine(storagePath, "TD_PluginConfig.cfg");

			User = user;
			Plugins = plugins;
		}

		public abstract void LoadAll();
		public abstract void SaveAll();
		public abstract void ReloadAll();

		internal void Save(IConfigObject instance) {
			this.GetInstanceInfo(instance).Save();
		}

		internal void Reset(IConfigObject instance) {
			this.GetInstanceInfo(instance).Reset();
		}

		public void TriggerProgramRestartRequested() {
			ProgramRestartRequested?.Invoke(this, EventArgs.Empty);
		}

		protected abstract IConfigInstance GetInstanceInfo(IConfigObject instance);
	}

	public sealed class ConfigManager<TUser, TSystem> : ConfigManager where TUser : class, IAppUserConfiguration, IConfigObject<TUser> where TSystem : class, IConfigObject<TSystem> {
		private new TUser User { get; }
		private TSystem System { get; }

		private readonly FileConfigInstance<TUser> infoUser;
		private readonly FileConfigInstance<TSystem> infoSystem;
		private readonly PluginConfigInstance infoPlugins;

		public ConfigManager(string storagePath, ConfigObjects<TUser, TSystem> configObjects) : base(storagePath, configObjects.User, configObjects.Plugins) {
			User = configObjects.User;
			System = configObjects.System;

			infoUser = new FileConfigInstance<TUser>(UserPath, User, "program options", ConverterRegistry);
			infoSystem = new FileConfigInstance<TSystem>(SystemPath, System, "system options", ConverterRegistry);
			infoPlugins = new PluginConfigInstance(PluginsPath, Plugins);
		}

		public override void LoadAll() {
			infoUser.Load();
			infoSystem.Load();
			infoPlugins.Load();
		}

		public override void SaveAll() {
			infoUser.Save();
			infoSystem.Save();
			infoPlugins.Save();
		}

		public override void ReloadAll() {
			infoUser.Reload();
			infoSystem.Reload();
			infoPlugins.Reload();
		}

		protected override IConfigInstance GetInstanceInfo(IConfigObject instance) {
			if (instance == User) {
				return infoUser;
			}
			else if (instance == System) {
				return infoSystem;
			}
			else if (instance == Plugins) {
				return infoPlugins;
			}
			else {
				throw new ArgumentException("Invalid configuration instance: " + instance.GetType());
			}
		}
	}

	public static class ConfigManagerExtensions {
		public static void Save(this IConfigObject instance) {
			App.ConfigManager.Save(instance);
		}

		public static void Reset(this IConfigObject instance) {
			App.ConfigManager.Reset(instance);
		}
	}
}
