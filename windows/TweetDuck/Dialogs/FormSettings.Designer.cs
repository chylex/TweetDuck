namespace TweetDuck.Dialogs {
    sealed partial class FormSettings {
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
            this.btnClose = new System.Windows.Forms.Button();
            this.panelContents = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnManageOptions = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.Location = new System.Drawing.Point(747, 500);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnClose.Size = new System.Drawing.Size(50, 25);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelContents
            // 
            this.panelContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContents.AutoScroll = true;
            this.panelContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContents.Location = new System.Drawing.Point(135, 12);
            this.panelContents.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panelContents.Name = "panelContents";
            this.panelContents.Size = new System.Drawing.Size(662, 482);
            this.panelContents.TabIndex = 1;
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelButtons.Location = new System.Drawing.Point(12, 12);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(124, 482);
            this.panelButtons.TabIndex = 0;
            // 
            // btnManageOptions
            // 
            this.btnManageOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnManageOptions.AutoSize = true;
            this.btnManageOptions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnManageOptions.Location = new System.Drawing.Point(12, 500);
            this.btnManageOptions.Name = "btnManageOptions";
            this.btnManageOptions.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnManageOptions.Size = new System.Drawing.Size(109, 25);
            this.btnManageOptions.TabIndex = 4;
            this.btnManageOptions.Text = "Manage Options";
            this.btnManageOptions.UseVisualStyleBackColor = true;
            this.btnManageOptions.Click += new System.EventHandler(this.btnManageOptions_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 537);
            this.Controls.Add(this.btnManageOptions);
            this.Controls.Add(this.panelContents);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.btnClose);
            this.Font = TweetDuck.Controls.ControlExtensions.DefaultFont;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::TweetDuck.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelContents;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnManageOptions;
    }
}