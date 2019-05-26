using System;
using System.Drawing;
using TweetDuck.Data;
using TweetLib.Core.Features.Configuration;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Serialization.Converters;
using TweetLib.Core.Utils;

namespace TweetDuck.Configuration{
    sealed class ConfigManager : IConfigManager{
        public UserConfig User { get; }
        public SystemConfig System { get; }
        public PluginConfig Plugins { get; }
        
        public event EventHandler ProgramRestartRequested;

        private readonly FileConfigInstance<UserConfig> infoUser;
        private readonly FileConfigInstance<SystemConfig> infoSystem;
        private readonly PluginConfigInstance<PluginConfig> infoPlugins;

        private readonly IConfigInstance<BaseConfig>[] infoList;

        public ConfigManager(){
            User = new UserConfig(this);
            System = new SystemConfig(this);
            Plugins = new PluginConfig(this);
            
            infoList = new IConfigInstance<BaseConfig>[]{
                infoUser = new FileConfigInstance<UserConfig>(Program.UserConfigFilePath, User, "program options"),
                infoSystem = new FileConfigInstance<SystemConfig>(Program.SystemConfigFilePath, System, "system options"),
                infoPlugins = new PluginConfigInstance<PluginConfig>(Program.PluginConfigFilePath, Plugins)
            };

            // TODO refactor further

            infoUser.Serializer.RegisterTypeConverter(typeof(WindowState), WindowState.Converter);

            infoUser.Serializer.RegisterTypeConverter(typeof(Point), new SingleTypeConverter<Point>{
                ConvertToString = value => $"{value.X} {value.Y}",
                ConvertToObject = value => {
                    int[] elements = StringUtils.ParseInts(value, ' ');
                    return new Point(elements[0], elements[1]);
                }
            });
            
            infoUser.Serializer.RegisterTypeConverter(typeof(Size), new SingleTypeConverter<Size>{
                ConvertToString = value => $"{value.Width} {value.Height}",
                ConvertToObject = value => {
                    int[] elements = StringUtils.ParseInts(value, ' ');
                    return new Size(elements[0], elements[1]);
                }
            });
        }

        public void LoadAll(){
            infoUser.Load();
            infoSystem.Load();
            infoPlugins.Load();
        }

        public void SaveAll(){
            infoUser.Save();
            infoSystem.Save();
            infoPlugins.Save();
        }

        public void ReloadAll(){
            infoUser.Reload();
            infoSystem.Reload();
            infoPlugins.Reload();
        }

        void IConfigManager.TriggerProgramRestartRequested(){
            ProgramRestartRequested?.Invoke(this, EventArgs.Empty);
        }

        IConfigInstance<BaseConfig> IConfigManager.GetInstanceInfo(BaseConfig instance){
            Type instanceType = instance.GetType();
            return Array.Find(infoList, info => info.Instance.GetType() == instanceType); // TODO handle null
        }
    }
}
