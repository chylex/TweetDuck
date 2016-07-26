namespace TweetDck.Core.Other.Settings {
    partial class TabSettingsUpdates {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.checkUpdateNotifications = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(9, 32);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(171, 23);
            this.btnCheckUpdates.TabIndex = 15;
            this.btnCheckUpdates.Text = "Check Updates Now";
            this.toolTip.SetToolTip(this.btnCheckUpdates, "Forces an update check, even for updates that had been dismissed.");
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            this.btnCheckUpdates.Click += new System.EventHandler(this.btnCheckUpdates_Click);
            // 
            // checkUpdateNotifications
            // 
            this.checkUpdateNotifications.AutoSize = true;
            this.checkUpdateNotifications.Location = new System.Drawing.Point(9, 9);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 14;
            this.checkUpdateNotifications.Text = "Check Updates Automatically";
            this.toolTip.SetToolTip(this.checkUpdateNotifications, "Checks for updates every hour.\r\nIf an update is dismissed, it will not appear aga" +
        "in.");
            this.checkUpdateNotifications.UseVisualStyleBackColor = true;
            this.checkUpdateNotifications.CheckedChanged += new System.EventHandler(this.checkUpdateNotifications_CheckedChanged);
            // 
            // TabSettingsUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCheckUpdates);
            this.Controls.Add(this.checkUpdateNotifications);
            this.Name = "TabSettingsUpdates";
            this.Size = new System.Drawing.Size(478, 282);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
