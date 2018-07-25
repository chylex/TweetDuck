namespace TweetDuck.Configuration{
    sealed class SystemConfig : ConfigManager.BaseConfig{

        // CONFIGURATION DATA
        
        public bool _hardwareAcceleration = true;
        
        public bool ClearCacheAutomatically { get; set; } = true;
        public int ClearCacheThreshold      { get; set; } = 250;

        // SPECIAL PROPERTIES
        
        public bool HardwareAcceleration{
            get => _hardwareAcceleration;
            set => UpdatePropertyWithRestartRequest(ref _hardwareAcceleration, value);
        }
        
        // END OF CONFIG

        public SystemConfig(ConfigManager configManager) : base(configManager){}

        protected override ConfigManager.BaseConfig ConstructWithDefaults(ConfigManager configManager){
            return new SystemConfig(configManager);
        }
    }
}
