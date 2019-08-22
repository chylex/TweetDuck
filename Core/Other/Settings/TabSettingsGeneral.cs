using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;
using TweetLib.Core.Features.Updates;
using TweetLib.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsGeneral : BaseTabSettings{
        private readonly Action reloadColumns;

        private readonly UpdateHandler updates;
        private int updateCheckEventId = -1;

        private readonly int browserListIndexDefault;
        private readonly int browserListIndexCustom;

        private readonly int searchEngineIndexDefault;
        private readonly int searchEngineIndexCustom;

        public TabSettingsGeneral(Action reloadColumns, UpdateHandler updates){
            InitializeComponent();

            this.reloadColumns = reloadColumns;
            
            this.updates = updates;
            this.updates.CheckFinished += updates_CheckFinished;

            Disposed += (sender, args) => this.updates.CheckFinished -= updates_CheckFinished;
            
            // user interface

            toolTip.SetToolTip(checkExpandLinks, "Expands links inside the tweets. If disabled,\r\nthe full links show up in a tooltip instead.");
            toolTip.SetToolTip(checkFocusDmInput, "Places cursor into Direct Message input\r\nfield when opening a conversation.");
            toolTip.SetToolTip(checkOpenSearchInFirstColumn, "By default, TweetDeck adds Search columns at the end.\r\nThis option makes them appear before the first column instead.");
            toolTip.SetToolTip(checkKeepLikeFollowDialogsOpen, "Allows liking and following from multiple accounts at once,\r\ninstead of automatically closing the dialog after taking an action.");
            toolTip.SetToolTip(checkBestImageQuality, "When right-clicking a tweet image, the context menu options\r\nwill use links to the original image size (:orig in the URL).");
            toolTip.SetToolTip(checkAnimatedAvatars, "Some old Twitter avatars could be uploaded as animated GIFs.");
            toolTip.SetToolTip(labelZoomValue, "Changes the zoom level.\r\nAlso affects notifications and screenshots.");
            toolTip.SetToolTip(trackBarZoom, toolTip.GetToolTip(labelZoomValue));

            checkExpandLinks.Checked = Config.ExpandLinksOnHover;
            checkFocusDmInput.Checked = Config.FocusDmInput;
            checkOpenSearchInFirstColumn.Checked = Config.OpenSearchInFirstColumn;
            checkKeepLikeFollowDialogsOpen.Checked = Config.KeepLikeFollowDialogsOpen;
            checkBestImageQuality.Checked = Config.BestImageQuality;
            checkAnimatedAvatars.Checked = Config.EnableAnimatedImages;
            
            trackBarZoom.SetValueSafe(Config.ZoomLevel);
            labelZoomValue.Text = trackBarZoom.Value + "%";

            // system tray
            
            toolTip.SetToolTip(comboBoxTrayType, "Changes behavior of the Tray icon.\r\nRight-click the icon for an action menu.");
            toolTip.SetToolTip(checkTrayHighlight, "Highlights the tray icon if there are new tweets.\r\nOnly works for columns with popup or audio notifications.\r\nThe icon resets when the main window is restored.");

            comboBoxTrayType.Items.Add("Disabled");
            comboBoxTrayType.Items.Add("Display Icon Only");
            comboBoxTrayType.Items.Add("Minimize to Tray");
            comboBoxTrayType.Items.Add("Close to Tray");
            comboBoxTrayType.Items.Add("Combined");
            comboBoxTrayType.SelectedIndex = Math.Min(Math.Max((int)Config.TrayBehavior, 0), comboBoxTrayType.Items.Count - 1);

            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
            checkTrayHighlight.Checked = Config.EnableTrayHighlight;

            // updates
            
            toolTip.SetToolTip(checkUpdateNotifications, "Checks for updates every hour.\r\nIf an update is dismissed, it will not appear again.");
            toolTip.SetToolTip(btnCheckUpdates, "Forces an update check, even for updates that had been dismissed.");
            
            checkUpdateNotifications.Checked = Config.EnableUpdateCheck;

            // browser settings
            
            toolTip.SetToolTip(checkSmoothScrolling, "Toggles smooth mouse wheel scrolling.");
            toolTip.SetToolTip(checkTouchAdjustment, "Toggles Chromium touch screen adjustment.\r\nDisabled by default, because it is very imprecise with TweetDeck.");
            toolTip.SetToolTip(checkHardwareAcceleration, "Uses graphics card to improve performance.\r\nDisable if you experience visual glitches, or to save a small amount of RAM.");
            toolTip.SetToolTip(comboBoxBrowserPath, "Sets the default browser for opening links.");
            toolTip.SetToolTip(comboBoxSearchEngine, "Sets the default website for opening searches.");
            
            checkSmoothScrolling.Checked = Config.EnableSmoothScrolling;
            checkTouchAdjustment.Checked = Config.EnableTouchAdjustment;
            checkHardwareAcceleration.Checked = SysConfig.HardwareAcceleration;

            foreach(WindowsUtils.Browser browserInfo in WindowsUtils.FindInstalledBrowsers()){
                comboBoxBrowserPath.Items.Add(browserInfo);
            }
            
            browserListIndexDefault = comboBoxBrowserPath.Items.Add("(default browser)");
            browserListIndexCustom = comboBoxBrowserPath.Items.Add("(custom program...)");
            UpdateBrowserPathSelection();

            comboBoxSearchEngine.Items.Add(new SearchEngine("DuckDuckGo", "https://duckduckgo.com/?q="));
            comboBoxSearchEngine.Items.Add(new SearchEngine("Google", "https://www.google.com/search?q="));
            comboBoxSearchEngine.Items.Add(new SearchEngine("Bing", "https://www.bing.com/search?q="));
            comboBoxSearchEngine.Items.Add(new SearchEngine("Yahoo", "https://search.yahoo.com/search?p="));
            searchEngineIndexDefault = comboBoxSearchEngine.Items.Add("(no engine set)");
            searchEngineIndexCustom = comboBoxSearchEngine.Items.Add("(custom url...)");
            UpdateSearchEngineSelection();

            // locales
            
            toolTip.SetToolTip(checkSpellCheck, "Underlines words that are spelled incorrectly.");
            toolTip.SetToolTip(comboBoxSpellCheckLanguage, "Language used for spell check.");
            toolTip.SetToolTip(comboBoxTranslationTarget, "Language tweets are translated into.");
            
            checkSpellCheck.Checked = Config.EnableSpellCheck;

            try{
                foreach(LocaleUtils.Item item in LocaleUtils.SpellCheckLanguages){
                    comboBoxSpellCheckLanguage.Items.Add(item);
                }
            }catch{
                comboBoxSpellCheckLanguage.Items.Add(new LocaleUtils.Item("en-US"));
            }

            comboBoxSpellCheckLanguage.SelectedItem = new LocaleUtils.Item(Config.SpellCheckLanguage);

            foreach(LocaleUtils.Item item in LocaleUtils.TweetDeckTranslationLocales){
                comboBoxTranslationTarget.Items.Add(item);
            }

            comboBoxTranslationTarget.SelectedItem = new LocaleUtils.Item(Config.TranslationTarget);
        }

        public override void OnReady(){
            checkExpandLinks.CheckedChanged += checkExpandLinks_CheckedChanged;
            checkFocusDmInput.CheckedChanged += checkFocusDmInput_CheckedChanged;
            checkOpenSearchInFirstColumn.CheckedChanged += checkOpenSearchInFirstColumn_CheckedChanged;
            checkKeepLikeFollowDialogsOpen.CheckedChanged += checkKeepLikeFollowDialogsOpen_CheckedChanged;
            checkBestImageQuality.CheckedChanged += checkBestImageQuality_CheckedChanged;
            checkAnimatedAvatars.CheckedChanged += checkAnimatedAvatars_CheckedChanged;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;

            comboBoxTrayType.SelectedIndexChanged += comboBoxTrayType_SelectedIndexChanged;
            checkTrayHighlight.CheckedChanged += checkTrayHighlight_CheckedChanged;
            
            checkUpdateNotifications.CheckedChanged += checkUpdateNotifications_CheckedChanged;
            btnCheckUpdates.Click += btnCheckUpdates_Click;

            checkSmoothScrolling.CheckedChanged += checkSmoothScrolling_CheckedChanged;
            checkTouchAdjustment.CheckedChanged += checkTouchAdjustment_CheckedChanged;
            checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;
            comboBoxBrowserPath.SelectedIndexChanged += comboBoxBrowserPath_SelectedIndexChanged;
            comboBoxSearchEngine.SelectedIndexChanged += comboBoxSearchEngine_SelectedIndexChanged;

            checkSpellCheck.CheckedChanged += checkSpellCheck_CheckedChanged;
            comboBoxSpellCheckLanguage.SelectedValueChanged += comboBoxSpellCheckLanguage_SelectedValueChanged;
            comboBoxTranslationTarget.SelectedValueChanged += comboBoxTranslationTarget_SelectedValueChanged;
        }

        public override void OnClosing(){
            Config.ZoomLevel = trackBarZoom.Value;
        }

        #region User Interface

        private void checkExpandLinks_CheckedChanged(object sender, EventArgs e){
            Config.ExpandLinksOnHover = checkExpandLinks.Checked;
        }

        private void checkFocusDmInput_CheckedChanged(object sender, EventArgs e){
            Config.FocusDmInput = checkFocusDmInput.Checked;
        }

        private void checkOpenSearchInFirstColumn_CheckedChanged(object sender, EventArgs e){
            Config.OpenSearchInFirstColumn = checkOpenSearchInFirstColumn.Checked;
        }

        private void checkKeepLikeFollowDialogsOpen_CheckedChanged(object sender, EventArgs e){
            Config.KeepLikeFollowDialogsOpen = checkKeepLikeFollowDialogsOpen.Checked;
        }

        private void checkBestImageQuality_CheckedChanged(object sender, EventArgs e){
            Config.BestImageQuality = checkBestImageQuality.Checked;
        }

        private void checkAnimatedAvatars_CheckedChanged(object sender, EventArgs e){
            Config.EnableAnimatedImages = checkAnimatedAvatars.Checked;
            BrowserProcessHandler.UpdatePrefs().ContinueWith(task => reloadColumns());
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e){
            if (trackBarZoom.AlignValueToTick()){
                zoomUpdateTimer.Stop();
                zoomUpdateTimer.Start();
                labelZoomValue.Text = trackBarZoom.Value + "%";
            }
        }

        private void zoomUpdateTimer_Tick(object sender, EventArgs e){
            Config.ZoomLevel = trackBarZoom.Value;
            zoomUpdateTimer.Stop();
        }

        #endregion
        #region System Tray
        
        private void comboBoxTrayType_SelectedIndexChanged(object sender, EventArgs e){
            Config.TrayBehavior = (TrayIcon.Behavior)comboBoxTrayType.SelectedIndex;
            checkTrayHighlight.Enabled = Config.TrayBehavior.ShouldDisplayIcon();
        }

        private void checkTrayHighlight_CheckedChanged(object sender, EventArgs e){
            Config.EnableTrayHighlight = checkTrayHighlight.Checked;
        }

        #endregion
        #region Updates
        
        private void checkUpdateNotifications_CheckedChanged(object sender, EventArgs e){
            Config.EnableUpdateCheck = checkUpdateNotifications.Checked;
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e){
            Config.DismissedUpdate = null;

            btnCheckUpdates.Enabled = false;
            updateCheckEventId = updates.Check(true);
        }

        private void updates_CheckFinished(object sender, UpdateCheckEventArgs e){
            if (e.EventId == updateCheckEventId){
                btnCheckUpdates.Enabled = true;

                e.Result.Handle(update => {
                    if (update.VersionTag == Program.VersionTag){
                        FormMessage.Information("No Updates Available", "Your version of TweetDuck is up to date.", FormMessage.OK);
                    }
                }, ex => {
                    Program.Reporter.HandleException("Update Check Error", "An error occurred while checking for updates.", true, ex);
                });
            }
        }

        #endregion
        #region Browser Settings
        
        private void checkSmoothScrolling_CheckedChanged(object sender, EventArgs e){
            Config.EnableSmoothScrolling = checkSmoothScrolling.Checked;
        }

        private void checkTouchAdjustment_CheckedChanged(object sender, EventArgs e){
            Config.EnableTouchAdjustment = checkTouchAdjustment.Checked;
        }
        
        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            SysConfig.HardwareAcceleration = checkHardwareAcceleration.Checked;
        }

        private void UpdateBrowserPathSelection(){
            if (string.IsNullOrEmpty(Config.BrowserPath) || !File.Exists(Config.BrowserPath)){
                comboBoxBrowserPath.SelectedIndex = browserListIndexDefault;
            }
            else{
                WindowsUtils.Browser browserInfo = comboBoxBrowserPath.Items.OfType<WindowsUtils.Browser>().FirstOrDefault(browser => browser.Path == Config.BrowserPath);

                if (browserInfo == null){
                    comboBoxBrowserPath.SelectedIndex = browserListIndexCustom;
                }
                else{
                    comboBoxBrowserPath.SelectedItem = browserInfo;
                }
            }
        }

        private void comboBoxBrowserPath_SelectedIndexChanged(object sender, EventArgs e){
            if (comboBoxBrowserPath.SelectedIndex == browserListIndexCustom){
                using(OpenFileDialog dialog = new OpenFileDialog{
                    AutoUpgradeEnabled = true,
                    DereferenceLinks = true,
                    InitialDirectory = Path.GetDirectoryName(Config.BrowserPath), // returns null if argument is null
                    Title = "Open Links With...",
                    Filter = "Executables (*.exe;*.bat;*.cmd)|*.exe;*.bat;*.cmd|All Files (*.*)|*.*"
                }){
                    if (dialog.ShowDialog() == DialogResult.OK){
                        Config.BrowserPath = dialog.FileName;
                    }
                }

                comboBoxBrowserPath.SelectedIndexChanged -= comboBoxBrowserPath_SelectedIndexChanged;
                UpdateBrowserPathSelection();
                comboBoxBrowserPath.SelectedIndexChanged += comboBoxBrowserPath_SelectedIndexChanged;
            }
            else{
                Config.BrowserPath = (comboBoxBrowserPath.SelectedItem as WindowsUtils.Browser)?.Path; // default browser item is a string and casts to null
            }
        }

        private void comboBoxSearchEngine_SelectedIndexChanged(object sender, EventArgs e){
            if (comboBoxSearchEngine.SelectedIndex == searchEngineIndexCustom){
                using(DialogSettingsSearchEngine dialog = new DialogSettingsSearchEngine()){
                    if (dialog.ShowDialog() == DialogResult.OK){
                        Config.SearchEngineUrl = dialog.Url.Trim();
                    }
                }

                comboBoxSearchEngine.SelectedIndexChanged -= comboBoxSearchEngine_SelectedIndexChanged;
                UpdateSearchEngineSelection();
                comboBoxSearchEngine.SelectedIndexChanged += comboBoxSearchEngine_SelectedIndexChanged;
            }
            else{
                Config.SearchEngineUrl = (comboBoxSearchEngine.SelectedItem as SearchEngine)?.Url; // default search engine item is a string and casts to null
            }
        }

        private void UpdateSearchEngineSelection(){
            if (string.IsNullOrEmpty(Config.SearchEngineUrl)){
                comboBoxSearchEngine.SelectedIndex = searchEngineIndexDefault;
            }
            else{
                SearchEngine engineInfo = comboBoxSearchEngine.Items.OfType<SearchEngine>().FirstOrDefault(engine => engine.Url == Config.SearchEngineUrl);
                
                if (engineInfo == null){
                    comboBoxSearchEngine.SelectedIndex = searchEngineIndexCustom;
                }
                else{
                    comboBoxSearchEngine.SelectedItem = engineInfo;
                }
            }
        }

        private sealed class SearchEngine{
            private string Name { get; }
            public string Url { get; }
            
            public SearchEngine(string name, string url){
                Name = name;
                Url = url;
            }
            
            public override int GetHashCode() => Name.GetHashCode();
            public override bool Equals(object obj) => obj is SearchEngine other && Name == other.Name;
            public override string ToString() => Name;
        }

        #endregion
        #region Locales
        
        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            Config.EnableSpellCheck = checkSpellCheck.Checked;
            BrowserProcessHandler.UpdatePrefs();
        }

        private void comboBoxSpellCheckLanguage_SelectedValueChanged(object sender, EventArgs e){
            Config.SpellCheckLanguage = (comboBoxSpellCheckLanguage.SelectedItem as LocaleUtils.Item)?.Code ?? "en-US";
        }

        private void comboBoxTranslationTarget_SelectedValueChanged(object sender, EventArgs e){
            Config.TranslationTarget = (comboBoxTranslationTarget.SelectedItem as LocaleUtils.Item)?.Code ?? "en";
        }

        #endregion
    }
}
