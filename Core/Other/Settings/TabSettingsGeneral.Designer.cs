namespace TweetDck.Core.Other.Settings {
    partial class TabSettingsGeneral {
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
            this.checkExpandLinks = new System.Windows.Forms.CheckBox();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkTrayHighlight = new System.Windows.Forms.CheckBox();
            this.checkSpellCheck = new System.Windows.Forms.CheckBox();
            this.checkScreenshotBorder = new System.Windows.Forms.CheckBox();
            this.groupTray = new System.Windows.Forms.GroupBox();
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.groupInterface = new System.Windows.Forms.GroupBox();
            this.groupUpdates = new System.Windows.Forms.GroupBox();
            this.checkUpdateNotifications = new System.Windows.Forms.CheckBox();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.groupTray.SuspendLayout();
            this.groupInterface.SuspendLayout();
            this.groupUpdates.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(9, 21);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 14;
            this.checkExpandLinks.Text = "Expand Links When Hovered";
            this.toolTip.SetToolTip(this.checkExpandLinks, "Expands links inside the tweets. If disabled,\r\nthe full links show up in a toolti" +
        "p instead.");
            this.checkExpandLinks.UseVisualStyleBackColor = true;
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(6, 19);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(171, 21);
            this.comboBoxTrayType.TabIndex = 13;
            this.toolTip.SetToolTip(this.comboBoxTrayType, "Changes behavior of the Tray icon.\r\nRight-click the icon for an action menu.");
            // 
            // checkTrayHighlight
            // 
            this.checkTrayHighlight.AutoSize = true;
            this.checkTrayHighlight.Location = new System.Drawing.Point(9, 70);
            this.checkTrayHighlight.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkTrayHighlight.Name = "checkTrayHighlight";
            this.checkTrayHighlight.Size = new System.Drawing.Size(103, 17);
            this.checkTrayHighlight.TabIndex = 15;
            this.checkTrayHighlight.Text = "Enable Highlight";
            this.toolTip.SetToolTip(this.checkTrayHighlight, "Highlights the tray icon if there are new tweets.\r\nOnly works for columns with po" +
        "pup or audio notifications.\r\nThe icon resets when the main window is restored.");
            this.checkTrayHighlight.UseVisualStyleBackColor = true;
            // 
            // checkSpellCheck
            // 
            this.checkSpellCheck.AutoSize = true;
            this.checkSpellCheck.Location = new System.Drawing.Point(9, 44);
            this.checkSpellCheck.Name = "checkSpellCheck";
            this.checkSpellCheck.Size = new System.Drawing.Size(119, 17);
            this.checkSpellCheck.TabIndex = 15;
            this.checkSpellCheck.Text = "Enable Spell Check";
            this.toolTip.SetToolTip(this.checkSpellCheck, "Underlines words that are spelled incorrectly.");
            this.checkSpellCheck.UseVisualStyleBackColor = true;
            // 
            // checkScreenshotBorder
            // 
            this.checkScreenshotBorder.AutoSize = true;
            this.checkScreenshotBorder.Location = new System.Drawing.Point(9, 67);
            this.checkScreenshotBorder.Name = "checkScreenshotBorder";
            this.checkScreenshotBorder.Size = new System.Drawing.Size(169, 17);
            this.checkScreenshotBorder.TabIndex = 16;
            this.checkScreenshotBorder.Text = "Include Border In Screenshots";
            this.toolTip.SetToolTip(this.checkScreenshotBorder, "Shows the window border in tweet screenshots.");
            this.checkScreenshotBorder.UseVisualStyleBackColor = true;
            // 
            // groupTray
            // 
            this.groupTray.Controls.Add(this.checkTrayHighlight);
            this.groupTray.Controls.Add(this.labelTrayIcon);
            this.groupTray.Controls.Add(this.comboBoxTrayType);
            this.groupTray.Location = new System.Drawing.Point(9, 109);
            this.groupTray.Name = "groupTray";
            this.groupTray.Size = new System.Drawing.Size(183, 93);
            this.groupTray.TabIndex = 15;
            this.groupTray.TabStop = false;
            this.groupTray.Text = "System Tray";
            // 
            // labelTrayIcon
            // 
            this.labelTrayIcon.AutoSize = true;
            this.labelTrayIcon.Location = new System.Drawing.Point(6, 52);
            this.labelTrayIcon.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelTrayIcon.Name = "labelTrayIcon";
            this.labelTrayIcon.Size = new System.Drawing.Size(52, 13);
            this.labelTrayIcon.TabIndex = 14;
            this.labelTrayIcon.Text = "Tray Icon";
            // 
            // groupInterface
            // 
            this.groupInterface.Controls.Add(this.checkScreenshotBorder);
            this.groupInterface.Controls.Add(this.checkSpellCheck);
            this.groupInterface.Controls.Add(this.checkExpandLinks);
            this.groupInterface.Location = new System.Drawing.Point(9, 9);
            this.groupInterface.Name = "groupInterface";
            this.groupInterface.Size = new System.Drawing.Size(183, 90);
            this.groupInterface.TabIndex = 16;
            this.groupInterface.TabStop = false;
            this.groupInterface.Text = "User Interface";
            // 
            // groupUpdates
            // 
            this.groupUpdates.Controls.Add(this.checkUpdateNotifications);
            this.groupUpdates.Controls.Add(this.btnCheckUpdates);
            this.groupUpdates.Location = new System.Drawing.Point(198, 9);
            this.groupUpdates.Name = "groupUpdates";
            this.groupUpdates.Size = new System.Drawing.Size(183, 75);
            this.groupUpdates.TabIndex = 17;
            this.groupUpdates.TabStop = false;
            this.groupUpdates.Text = "Updates";
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
            // 
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupUpdates);
            this.Controls.Add(this.groupInterface);
            this.Controls.Add(this.groupTray);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(478, 282);
            this.groupTray.ResumeLayout(false);
            this.groupTray.PerformLayout();
            this.groupInterface.ResumeLayout(false);
            this.groupInterface.PerformLayout();
            this.groupUpdates.ResumeLayout(false);
            this.groupUpdates.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkExpandLinks;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupTray;
        private System.Windows.Forms.GroupBox groupInterface;
        private System.Windows.Forms.Label labelTrayIcon;
        private System.Windows.Forms.CheckBox checkTrayHighlight;
        private System.Windows.Forms.CheckBox checkSpellCheck;
        private System.Windows.Forms.CheckBox checkScreenshotBorder;
        private System.Windows.Forms.GroupBox groupUpdates;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.Button btnCheckUpdates;
    }
}
