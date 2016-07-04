using TweetDck.Core.Controls;

namespace TweetDck.Migration {
    partial class FormMigrationQuestion {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnIgnore = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAskLater = new System.Windows.Forms.Button();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnMigrateUninstall = new System.Windows.Forms.Button();
            this.labelQuestion = new System.Windows.Forms.Label();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnIgnore
            // 
            this.btnIgnore.AutoSize = true;
            this.btnIgnore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnIgnore.Location = new System.Drawing.Point(391, 0);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnIgnore.Size = new System.Drawing.Size(53, 23);
            this.btnIgnore.TabIndex = 1;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButtons.Controls.Add(this.btnAskLater);
            this.panelButtons.Controls.Add(this.btnIgnore);
            this.panelButtons.Controls.Add(this.btnMigrate);
            this.panelButtons.Controls.Add(this.btnMigrateUninstall);
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelButtons.Location = new System.Drawing.Point(12, 67);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(518, 23);
            this.panelButtons.TabIndex = 0;
            // 
            // btnAskLater
            // 
            this.btnAskLater.AutoSize = true;
            this.btnAskLater.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAskLater.Location = new System.Drawing.Point(450, 0);
            this.btnAskLater.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnAskLater.Name = "btnAskLater";
            this.btnAskLater.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnAskLater.Size = new System.Drawing.Size(68, 23);
            this.btnAskLater.TabIndex = 0;
            this.btnAskLater.Text = "Ask Later";
            this.btnAskLater.UseVisualStyleBackColor = true;
            this.btnAskLater.Click += new System.EventHandler(this.btnAskLater_Click);
            // 
            // btnMigrate
            // 
            this.btnMigrate.AutoSize = true;
            this.btnMigrate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMigrate.Location = new System.Drawing.Point(327, 0);
            this.btnMigrate.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnMigrate.Size = new System.Drawing.Size(58, 23);
            this.btnMigrate.TabIndex = 3;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // btnMigrateUninstall
            // 
            this.btnMigrateUninstall.AutoSize = true;
            this.btnMigrateUninstall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMigrateUninstall.Location = new System.Drawing.Point(223, 0);
            this.btnMigrateUninstall.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnMigrateUninstall.Name = "btnMigrateUninstall";
            this.btnMigrateUninstall.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnMigrateUninstall.Size = new System.Drawing.Size(98, 23);
            this.btnMigrateUninstall.TabIndex = 4;
            this.btnMigrateUninstall.Text = "Migrate && Purge";
            this.btnMigrateUninstall.UseVisualStyleBackColor = true;
            this.btnMigrateUninstall.Click += new System.EventHandler(this.btnMigrateUninstall_Click);
            // 
            // labelQuestion
            // 
            this.labelQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelQuestion.Location = new System.Drawing.Point(49, 9);
            this.labelQuestion.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.Size = new System.Drawing.Size(481, 52);
            this.labelQuestion.TabIndex = 2;
            // 
            // FormMigrationQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 102);
            this.Controls.Add(this.labelQuestion);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMigrationQuestion";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TweetDeck Migration";
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.Label labelQuestion;
        private System.Windows.Forms.Button btnAskLater;
        private System.Windows.Forms.Button btnMigrateUninstall;
    }
}