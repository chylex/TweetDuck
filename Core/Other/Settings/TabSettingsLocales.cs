using System;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsLocales : BaseTabSettings{
        public TabSettingsLocales(){
            InitializeComponent();
            
            toolTip.SetToolTip(checkSpellCheck, "Underlines words that are spelled incorrectly.");
            toolTip.SetToolTip(comboBoxAppLocale, "Language used for spell check and context menu items.");
            toolTip.SetToolTip(comboBoxTranslationTarget, "Language tweets are translated into.");
            
            checkSpellCheck.Checked = Config.EnableSpellCheck;

            try{
                foreach(LocaleUtils.Item item in LocaleUtils.ChromiumLocales){
                    comboBoxAppLocale.Items.Add(item);
                }
            }catch{
                comboBoxAppLocale.Items.Add(new LocaleUtils.Item("en-US"));
            }

            comboBoxAppLocale.SelectedItem = new LocaleUtils.Item(Config.AppLocale);

            foreach(LocaleUtils.Item item in LocaleUtils.TweetDeckTranslationLocales){
                comboBoxTranslationTarget.Items.Add(item);
            }

            comboBoxTranslationTarget.SelectedItem = new LocaleUtils.Item(Config.TranslationTarget);
        }

        public override void OnReady(){
            checkSpellCheck.CheckedChanged += checkSpellCheck_CheckedChanged;
            comboBoxAppLocale.SelectedValueChanged += comboBoxAppLocale_SelectedValueChanged;
            comboBoxTranslationTarget.SelectedValueChanged += comboBoxTranslationTarget_SelectedValueChanged;
        }

        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            Config.EnableSpellCheck = checkSpellCheck.Checked;
            BrowserProcessHandler.UpdatePrefs();
        }

        private void comboBoxAppLocale_SelectedValueChanged(object sender, EventArgs e){
            Config.AppLocale = (comboBoxAppLocale.SelectedItem as LocaleUtils.Item)?.Code;
            PromptRestart();
        }

        private void comboBoxTranslationTarget_SelectedValueChanged(object sender, EventArgs e){
            Config.TranslationTarget = (comboBoxTranslationTarget.SelectedItem as LocaleUtils.Item)?.Code;
        }
    }
}
