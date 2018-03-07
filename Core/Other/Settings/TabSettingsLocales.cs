using System;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsLocales : BaseTabSettings{
        public TabSettingsLocales(){
            InitializeComponent();
            
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
            checkSpellCheck.CheckedChanged += checkSpellCheck_CheckedChanged;
            comboBoxSpellCheckLanguage.SelectedValueChanged += comboBoxSpellCheckLanguage_SelectedValueChanged;
            comboBoxTranslationTarget.SelectedValueChanged += comboBoxTranslationTarget_SelectedValueChanged;
        }

        private void checkSpellCheck_CheckedChanged(object sender, EventArgs e){
            Config.EnableSpellCheck = checkSpellCheck.Checked;
            BrowserProcessHandler.UpdatePrefs();
        }

        private void comboBoxSpellCheckLanguage_SelectedValueChanged(object sender, EventArgs e){
            Config.SpellCheckLanguage = (comboBoxSpellCheckLanguage.SelectedItem as LocaleUtils.Item)?.Code ?? "en-US";
            PromptRestart();
        }

        private void comboBoxTranslationTarget_SelectedValueChanged(object sender, EventArgs e){
            Config.TranslationTarget = (comboBoxTranslationTarget.SelectedItem as LocaleUtils.Item)?.Code ?? "en";
        }
    }
}
