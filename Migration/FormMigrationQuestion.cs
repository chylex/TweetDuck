using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDck.Migration{
    partial class FormMigrationQuestion : Form{
        public MigrationDecision Decision { get; private set; }

        public FormMigrationQuestion(){
            InitializeComponent();

            labelQuestion.Text = "Hey there, I found some TweetDeck data! Do you want to »Migrate« it and delete the old data folder, »Ignore« the request forever, or try "+Program.BrandName+" out first?\r\nYou may also »Migrate && Purge« which uninstalls TweetDeck too!";
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
