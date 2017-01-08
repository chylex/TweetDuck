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
            this.groupTray.SuspendLayout();
            this.groupInterface.SuspendLayout();
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
            this.checkExpandLinks.CheckedChanged += new System.EventHandler(this.checkExpandLinks_CheckedChanged);
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
            this.comboBoxTrayType.SelectedIndexChanged += new System.EventHandler(this.comboBoxTrayType_SelectedIndexChanged);
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
            this.checkTrayHighlight.CheckedChanged += new System.EventHandler(this.checkTrayHighlight_CheckedChanged);
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
            this.checkSpellCheck.CheckedChanged += new System.EventHandler(this.checkSpellCheck_CheckedChanged);
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
            this.checkScreenshotBorder.CheckedChanged += new System.EventHandler(this.checkScreenshotBorder_CheckedChanged);
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
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupInterface);
            this.Controls.Add(this.groupTray);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(478, 282);
            this.groupTray.ResumeLayout(false);
            this.groupTray.PerformLayout();
            this.groupInterface.ResumeLayout(false);
            this.groupInterface.PerformLayout();
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
    }
}
