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
            this.groupGeneral = new System.Windows.Forms.GroupBox();
            this.groupGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(6, 44);
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
            this.checkUpdateNotifications.Location = new System.Drawing.Point(6, 21);
            this.checkUpdateNotifications.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 14;
            this.checkUpdateNotifications.Text = "Check Updates Automatically";
            this.toolTip.SetToolTip(this.checkUpdateNotifications, "Checks for updates every hour.\r\nIf an update is dismissed, it will not appear aga" +
        "in.");
            this.checkUpdateNotifications.UseVisualStyleBackColor = true;
            this.checkUpdateNotifications.CheckedChanged += new System.EventHandler(this.checkUpdateNotifications_CheckedChanged);
            // 
            // groupGeneral
            // 
            this.groupGeneral.Controls.Add(this.checkUpdateNotifications);
            this.groupGeneral.Controls.Add(this.btnCheckUpdates);
            this.groupGeneral.Location = new System.Drawing.Point(9, 9);
            this.groupGeneral.Name = "groupGeneral";
            this.groupGeneral.Size = new System.Drawing.Size(183, 75);
            this.groupGeneral.TabIndex = 16;
            this.groupGeneral.TabStop = false;
            this.groupGeneral.Text = "General";
            // 
            // TabSettingsUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupGeneral);
            this.Name = "TabSettingsUpdates";
            this.Size = new System.Drawing.Size(478, 282);
            this.groupGeneral.ResumeLayout(false);
            this.groupGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupGeneral;
    }
}
