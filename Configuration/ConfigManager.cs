using System;
using System.Collections.Generic;
using System.Drawing;
using TweetDuck.Configuration.Instance;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class ConfigManager{
        public UserConfig User { get; }
        public SystemConfig System { get; }
        public PluginConfig Plugins { get; }
        
        public event EventHandler ProgramRestartRequested;

        private readonly FileConfigInstance<UserConfig> infoUser;
        private readonly FileConfigInstance<SystemConfig> infoSystem;
        private readonly PluginConfigInstance infoPlugins;

        private readonly IConfigInstance<BaseConfig>[] infoList;

        public ConfigManager(){
            User = new UserConfig(this);
            System = new SystemConfig(this);
            Plugins = new PluginConfig(this);
            
            infoList = new IConfigInstance<BaseConfig>[]{
                infoUser = new FileConfigInstance<UserConfig>(Program.UserConfigFilePath, User, "program options"),
                infoSystem = new FileConfigInstance<SystemConfig>(Program.SystemConfigFilePath, System, "system options"),
                infoPlugins = new PluginConfigInstance(Program.PluginConfigFilePath, Plugins)
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

        private void TriggerProgramRestartRequested(){
            ProgramRestartRequested?.Invoke(this, EventArgs.Empty);
        }

        private IConfigInstance<BaseConfig> GetInstanceInfo(BaseConfig instance){
            Type instanceType = instance.GetType();
            return Array.Find(infoList, info => info.Instance.GetType() == instanceType); // TODO handle null
        }

        public abstract class BaseConfig{
            private readonly ConfigManager configManager;

            protected BaseConfig(ConfigManager configManager){
                this.configManager = configManager;
            }

            // Management

            public void Save(){
                configManager.GetInstanceInfo(this).Save();
            }

            public void Reload(){
                configManager.GetInstanceInfo(this).Reload();
            }

            public void Reset(){
                configManager.GetInstanceInfo(this).Reset();
            }

            // Construction methods

            public T ConstructWithDefaults<T>() where T : BaseConfig{
                return ConstructWithDefaults(configManager) as T;
            }

            protected abstract BaseConfig ConstructWithDefaults(ConfigManager configManager);

            // Utility methods

            protected void UpdatePropertyWithEvent<T>(ref T field, T value, EventHandler eventHandler){
                if (!EqualityComparer<T>.Default.Equals(field, value)){
                    field = value;
                    eventHandler?.Invoke(this, EventArgs.Empty);
                }
            }

            protected void UpdatePropertyWithRestartRequest<T>(ref T field, T value){
                if (!EqualityComparer<T>.Default.Equals(field, value)){
                    field = value;
                    configManager.TriggerProgramRestartRequested();
                }
            }
        }
    }
}
