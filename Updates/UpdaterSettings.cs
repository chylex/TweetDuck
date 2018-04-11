namespace TweetDuck.Updates{
    sealed class UpdaterSettings{
        public string InstallerDownloadFolder { get; }
        
        public string DismissedUpdate { get; set; }

        public UpdaterSettings(string installerDownloadFolder){
            this.InstallerDownloadFolder = installerDownloadFolder;
        }
    }
}
