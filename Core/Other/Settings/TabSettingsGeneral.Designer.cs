namespace TweetDuck.Core.Other.Settings {
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
            this.checkUpdateNotifications = new System.Windows.Forms.CheckBox();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.labelZoomValue = new System.Windows.Forms.Label();
            this.checkSwitchAccountSelectors = new System.Windows.Forms.CheckBox();
            this.groupTray = new System.Windows.Forms.GroupBox();
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.groupInterface = new System.Windows.Forms.GroupBox();
            this.trackBarZoom = new System.Windows.Forms.TrackBar();
            this.labelZoom = new System.Windows.Forms.Label();
            this.groupUpdates = new System.Windows.Forms.GroupBox();
            this.zoomUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.groupTray.SuspendLayout();
            this.groupInterface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).BeginInit();
            this.groupUpdates.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(9, 21);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 0;
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
            this.comboBoxTrayType.TabIndex = 0;
            this.toolTip.SetToolTip(this.comboBoxTrayType, "Changes behavior of the Tray icon.\r\nRight-click the icon for an action menu.");
            // 
            // checkTrayHighlight
            // 
            this.checkTrayHighlight.AutoSize = true;
            this.checkTrayHighlight.Location = new System.Drawing.Point(9, 70);
            this.checkTrayHighlight.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkTrayHighlight.Name = "checkTrayHighlight";
            this.checkTrayHighlight.Size = new System.Drawing.Size(103, 17);
            this.checkTrayHighlight.TabIndex = 2;
            this.checkTrayHighlight.Text = "Enable Highlight";
            this.toolTip.SetToolTip(this.checkTrayHighlight, "Highlights the tray icon if there are new tweets.\r\nOnly works for columns with po" +
        "pup or audio notifications.\r\nThe icon resets when the main window is restored.");
            this.checkTrayHighlight.UseVisualStyleBackColor = true;
            // 
            // checkSpellCheck
            // 
            this.checkSpellCheck.AutoSize = true;
            this.checkSpellCheck.Location = new System.Drawing.Point(9, 67);
            this.checkSpellCheck.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSpellCheck.Name = "checkSpellCheck";
            this.checkSpellCheck.Size = new System.Drawing.Size(119, 17);
            this.checkSpellCheck.TabIndex = 2;
            this.checkSpellCheck.Text = "Enable Spell Check";
            this.toolTip.SetToolTip(this.checkSpellCheck, "Underlines words that are spelled incorrectly.");
            this.checkSpellCheck.UseVisualStyleBackColor = true;
            // 
            // checkUpdateNotifications
            // 
            this.checkUpdateNotifications.AutoSize = true;
            this.checkUpdateNotifications.Location = new System.Drawing.Point(9, 21);
            this.checkUpdateNotifications.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 0;
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
            this.btnCheckUpdates.TabIndex = 1;
            this.btnCheckUpdates.Text = "Check Updates Now";
            this.toolTip.SetToolTip(this.btnCheckUpdates, "Forces an update check, even for updates that had been dismissed.");
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // labelZoomValue
            // 
            this.labelZoomValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelZoomValue.BackColor = System.Drawing.Color.Transparent;
            this.labelZoomValue.Location = new System.Drawing.Point(139, 116);
            this.labelZoomValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelZoomValue.Name = "labelZoomValue";
            this.labelZoomValue.Size = new System.Drawing.Size(38, 13);
            this.labelZoomValue.TabIndex = 5;
            this.labelZoomValue.Text = "100%";
            this.labelZoomValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip.SetToolTip(this.labelZoomValue, "Changes the zoom level.\r\nAlso affects notifications and screenshots.");
            // 
            // checkSwitchAccountSelectors
            // 
            this.checkSwitchAccountSelectors.AutoSize = true;
            this.checkSwitchAccountSelectors.Location = new System.Drawing.Point(9, 44);
            this.checkSwitchAccountSelectors.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSwitchAccountSelectors.Name = "checkSwitchAccountSelectors";
            this.checkSwitchAccountSelectors.Size = new System.Drawing.Size(172, 17);
            this.checkSwitchAccountSelectors.TabIndex = 1;
            this.checkSwitchAccountSelectors.Text = "Shift Selects Multiple Accounts";
            this.toolTip.SetToolTip(this.checkSwitchAccountSelectors, "When (re)tweeting, click to select a single account or hold Shift to\r\nselect mult" +
        "iple accounts, instead of TweetDeck\'s default behavior.");
            this.checkSwitchAccountSelectors.UseVisualStyleBackColor = true;
            // 
            // groupTray
            // 
            this.groupTray.Controls.Add(this.checkTrayHighlight);
            this.groupTray.Controls.Add(this.labelTrayIcon);
            this.groupTray.Controls.Add(this.comboBoxTrayType);
            this.groupTray.Location = new System.Drawing.Point(9, 168);
            this.groupTray.Name = "groupTray";
            this.groupTray.Size = new System.Drawing.Size(183, 93);
            this.groupTray.TabIndex = 1;
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
            this.labelTrayIcon.TabIndex = 1;
            this.labelTrayIcon.Text = "Tray Icon";
            // 
            // groupInterface
            // 
            this.groupInterface.Controls.Add(this.checkSwitchAccountSelectors);
            this.groupInterface.Controls.Add(this.labelZoomValue);
            this.groupInterface.Controls.Add(this.trackBarZoom);
            this.groupInterface.Controls.Add(this.labelZoom);
            this.groupInterface.Controls.Add(this.checkSpellCheck);
            this.groupInterface.Controls.Add(this.checkExpandLinks);
            this.groupInterface.Location = new System.Drawing.Point(9, 9);
            this.groupInterface.Name = "groupInterface";
            this.groupInterface.Size = new System.Drawing.Size(183, 153);
            this.groupInterface.TabIndex = 0;
            this.groupInterface.TabStop = false;
            this.groupInterface.Text = "User Interface";
            // 
            // trackBarZoom
            // 
            this.trackBarZoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarZoom.AutoSize = false;
            this.trackBarZoom.LargeChange = 25;
            this.trackBarZoom.Location = new System.Drawing.Point(6, 115);
            this.trackBarZoom.Maximum = 200;
            this.trackBarZoom.Minimum = 50;
            this.trackBarZoom.Name = "trackBarZoom";
            this.trackBarZoom.Size = new System.Drawing.Size(141, 30);
            this.trackBarZoom.SmallChange = 5;
            this.trackBarZoom.TabIndex = 4;
            this.trackBarZoom.TickFrequency = 25;
            this.trackBarZoom.Value = 100;
            // 
            // labelZoom
            // 
            this.labelZoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(5, 99);
            this.labelZoom.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 3;
            this.labelZoom.Text = "Zoom";
            // 
            // groupUpdates
            // 
            this.groupUpdates.Controls.Add(this.checkUpdateNotifications);
            this.groupUpdates.Controls.Add(this.btnCheckUpdates);
            this.groupUpdates.Location = new System.Drawing.Point(198, 9);
            this.groupUpdates.Name = "groupUpdates";
            this.groupUpdates.Size = new System.Drawing.Size(183, 75);
            this.groupUpdates.TabIndex = 2;
            this.groupUpdates.TabStop = false;
            this.groupUpdates.Text = "Updates";
            // 
            // zoomUpdateTimer
            // 
            this.zoomUpdateTimer.Interval = 250;
            this.zoomUpdateTimer.Tick += new System.EventHandler(this.zoomUpdateTimer_Tick);
            // 
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupUpdates);
            this.Controls.Add(this.groupInterface);
            this.Controls.Add(this.groupTray);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(478, 300);
            this.groupTray.ResumeLayout(false);
            this.groupTray.PerformLayout();
            this.groupInterface.ResumeLayout(false);
            this.groupInterface.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).EndInit();
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
        private System.Windows.Forms.GroupBox groupUpdates;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.Label labelZoomValue;
        private System.Windows.Forms.TrackBar trackBarZoom;
        private System.Windows.Forms.Timer zoomUpdateTimer;
        private System.Windows.Forms.CheckBox checkSwitchAccountSelectors;
    }
}
