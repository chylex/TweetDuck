using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsCSS : Form{
        public string BrowserCSS => textBoxBrowserCSS.Text;
        public string NotificationCSS => textBoxNotificationCSS.Text;

        private readonly Action<string> reinjectBrowserCSS;
        private readonly Action openDevTools;

        public DialogSettingsCSS(string browserCSS, string notificationCSS, Action<string> reinjectBrowserCSS, Action openDevTools){
            InitializeComponent();
            
            Text = Program.BrandName + " Options - CSS";

            this.reinjectBrowserCSS = reinjectBrowserCSS;
            this.openDevTools = openDevTools;
            
            textBoxBrowserCSS.EnableMultilineShortcuts();
            textBoxBrowserCSS.Text = browserCSS ?? "";

            textBoxNotificationCSS.EnableMultilineShortcuts();
            textBoxNotificationCSS.Text = notificationCSS ?? "";

            if (!BrowserUtils.HasDevTools){
                btnOpenDevTools.Enabled = false;
            }

            ActiveControl = textBoxBrowserCSS;
            textBoxBrowserCSS.Select(textBoxBrowserCSS.TextLength, 0);
        }

        private void tabPanel_SelectedIndexChanged(object sender, EventArgs e){
            TextBox tb = tabPanel.SelectedTab.Controls.OfType<TextBox>().FirstOrDefault();

            if (tb != null){
                tb.Focus();
                tb.Select(tb.TextLength, 0);
            }
        }

        private void textBoxCSS_KeyDown(object sender, KeyEventArgs e){
            TextBox tb = (TextBox)sender;
            string text = tb.Text;

            if (e.KeyCode == Keys.Back && e.Modifiers == Keys.Control){
                e.SuppressKeyPress = true;

                int deleteTo = tb.SelectionStart;

                if (deleteTo > 0){
                    char initialChar = text[--deleteTo];
                    bool shouldDeleteAlphanumeric = char.IsLetterOrDigit(initialChar);
                
                    while(--deleteTo >= 0){
                        if ((shouldDeleteAlphanumeric && !char.IsLetterOrDigit(text[deleteTo])) ||
                            (!shouldDeleteAlphanumeric && text[deleteTo] != initialChar)){
                            break;
                        }
                    }
                    
                    if (!(deleteTo < text.Length - 1 && text[deleteTo] == '\r' && text[deleteTo + 1] == '\n')){
                        ++deleteTo;
                    }

                    tb.Select(deleteTo, tb.SelectionLength + tb.SelectionStart - deleteTo);
                    tb.SelectedText = string.Empty;
                }
            }
            else if (e.KeyCode == Keys.Back && e.Modifiers == Keys.None){
                int deleteTo = tb.SelectionStart;

                if (deleteTo > 1 && text[deleteTo - 1] == ' ' && text[deleteTo - 2] == ' '){
                    e.SuppressKeyPress = true;

                    tb.Select(deleteTo - 2, 2);
                    tb.SelectedText = string.Empty;
                }
            }
            else if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.None && tb.SelectionLength == 0){
                int insertAt = tb.SelectionStart, cursorOffset = 0;
                string insertText;

                if (insertAt == 0){
                    return;
                }
                else if (text[insertAt - 1] == '{'){
                    insertText = Environment.NewLine + "  ";

                    int nextBracket = insertAt < text.Length ? text.IndexOfAny(new char[]{ '{', '}' }, insertAt + 1) : -1;

                    if (nextBracket == -1 || text[nextBracket] == '{'){
                        string insertExtra = Environment.NewLine + "}";
                        insertText += insertExtra;
                        cursorOffset -= insertExtra.Length;
                    }
                }
                else{
                    int lineStart = text.LastIndexOf('\n', tb.SelectionStart - 1);

                    Match match = Regex.Match(text.Substring(lineStart == -1 ? 0 : lineStart + 1), "^([ \t]+)");
                    insertText = match.Success ? Environment.NewLine + match.Groups[1].Value : null;
                }

                if (!string.IsNullOrEmpty(insertText)){
                    e.SuppressKeyPress = true;
                    tb.Text = text.Insert(insertAt, insertText);
                    tb.SelectionStart = insertAt + cursorOffset + insertText.Length;
                }
            }
        }

        private void textBoxBrowserCSS_KeyUp(object sender, KeyEventArgs e){
            timerTestBrowser.Stop();
            timerTestBrowser.Start();
        }

        private void timerTestBrowser_Tick(object sender, EventArgs e){
            reinjectBrowserCSS(textBoxBrowserCSS.Text);
            timerTestBrowser.Stop();
        }

        private void btnOpenDevTools_Click(object sender, EventArgs e){
            openDevTools();
        }

        private void btnApply_Click(object sender, EventArgs e){
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
