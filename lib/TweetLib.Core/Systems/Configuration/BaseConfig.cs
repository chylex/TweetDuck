using System;
using System.Collections.Generic;

namespace TweetLib.Core.Systems.Configuration{
    public abstract class BaseConfig{
        private readonly IConfigManager configManager;

        protected BaseConfig(IConfigManager configManager){
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
            return (T)ConstructWithDefaults(configManager);
        }

        protected abstract BaseConfig ConstructWithDefaults(IConfigManager configManager);

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
