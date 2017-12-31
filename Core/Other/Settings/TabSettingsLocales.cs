using System;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsLocales : BaseTabSettings{
        private readonly FormBrowser browser;

        public TabSettingsLocales(FormBrowser browser){
            InitializeComponent();

            this.browser = browser;
            
            toolTip.SetToolTip(checkSpellCheck, "Underlines words that are spelled incorrectly.");
            toolTip.SetToolTip(comboBoxAppLocale, "Language used for spell check and context menu items.");
            
            checkSpellCheck.Checked = Config.EnableSpellCheck;

            try{
                foreach(LocaleUtils.Item item in LocaleUtils.ChromiumLocales){
                    comboBoxAppLocale.Items.Add(item);
                }
            }catch{
                comboBoxAppLocale.Items.Add(new LocaleUtils.Item("en-US"));
            }

            comboBoxAppLocale.SelectedItem = new LocaleUtils.Item(Config.AppLocale);

        }

        public override void OnReady(){
            checkSpellCheck.CheckedChanged += checkSpellCheck_CheckedChanged;
            comboBoxAppLocale.SelectedValueChanged += comboBoxAppLocale_SelectedValueChanged;
        }

        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            Config.EnableSpellCheck = checkSpellCheck.Checked;
            BrowserProcessHandler.UpdatePrefs();
        }

        private void comboBoxAppLocale_SelectedValueChanged(object sender, EventArgs e){
            Config.AppLocale = (comboBoxAppLocale.SelectedItem as LocaleUtils.Item)?.Code;
            PromptRestart();
        }
    }
}
