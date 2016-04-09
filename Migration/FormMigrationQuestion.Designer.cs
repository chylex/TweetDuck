namespace TweetDick.Migration {
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
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnAskLater = new System.Windows.Forms.Button();
            this.labelQuestion = new TweetDick.Core.RichTextLabel();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnIgnore
            // 
            this.btnIgnore.AutoSize = true;
            this.btnIgnore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnIgnore.Location = new System.Drawing.Point(356, 0);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
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
            this.panelButtons.Controls.Add(this.btnCopy);
            this.panelButtons.Controls.Add(this.btnMigrate);
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelButtons.Location = new System.Drawing.Point(12, 71);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(480, 23);
            this.panelButtons.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.AutoSize = true;
            this.btnCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopy.Location = new System.Drawing.Point(303, 0);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCopy.Size = new System.Drawing.Size(47, 23);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnMigrate
            // 
            this.btnMigrate.AutoSize = true;
            this.btnMigrate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMigrate.Location = new System.Drawing.Point(239, 0);
            this.btnMigrate.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnMigrate.Size = new System.Drawing.Size(58, 23);
            this.btnMigrate.TabIndex = 3;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // btnAskLater
            // 
            this.btnAskLater.AutoSize = true;
            this.btnAskLater.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAskLater.Location = new System.Drawing.Point(412, 0);
            this.btnAskLater.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnAskLater.Name = "btnAskLater";
            this.btnAskLater.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnAskLater.Size = new System.Drawing.Size(68, 23);
            this.btnAskLater.TabIndex = 0;
            this.btnAskLater.Text = "Ask Later";
            this.btnAskLater.UseVisualStyleBackColor = true;
            this.btnAskLater.Click += new System.EventHandler(this.btnAskLater_Click);
            // 
            // labelQuestion
            // 
            this.labelQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQuestion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelQuestion.Location = new System.Drawing.Point(49, 9);
            this.labelQuestion.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.ReadOnly = true;
            this.labelQuestion.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.labelQuestion.Size = new System.Drawing.Size(443, 56);
            this.labelQuestion.TabIndex = 2;
            this.labelQuestion.TabStop = false;
            this.labelQuestion.Text = "";
            // 
            // FormMigration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 106);
            this.Controls.Add(this.labelQuestion);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMigration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TweetDeck Migration";
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnMigrate;
        private Core.RichTextLabel labelQuestion;
        private System.Windows.Forms.Button btnAskLater;
    }
}