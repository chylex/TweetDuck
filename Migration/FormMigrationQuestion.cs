using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Migration{
    partial class FormMigrationQuestion : Form{
        public MigrationDecision Decision { get; private set; }

        public FormMigrationQuestion(){
            InitializeComponent();

            labelQuestion.Rtf = RichTextLabel.Wrap(@"Hey there, I found some TweetDeck data! Do you want to \b Migrate\b0  it and delete the old data folder, \b Ignore\b0  the request forever, or just try "+Program.BrandName+@" first?\par You may also \b Migrate & Purge\b0  which uninstalls TweetDeck too!");
        }

        protected override void OnPaint(PaintEventArgs e){
            e.Graphics.DrawIcon(SystemIcons.Question,10,10);
            base.OnPaint(e);
        }

        private void btnMigrateUninstall_Click(object sender, EventArgs e){
            Close(MigrationDecision.MigratePurge);
        }

        private void btnMigrate_Click(object sender, EventArgs e){
            Close(MigrationDecision.Migrate);
        }

        private void btnIgnore_Click(object sender, EventArgs e){
            Close(MigrationDecision.Ignore);
        }

        private void btnAskLater_Click(object sender, EventArgs e){
            Close(MigrationDecision.AskLater);
        }

        private void Close(MigrationDecision decision){
            Decision = decision;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
