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
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.trackBarZoom = new System.Windows.Forms.TrackBar();
            this.labelZoom = new System.Windows.Forms.Label();
            this.zoomUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.labelUI = new System.Windows.Forms.Label();
            this.panelUI = new System.Windows.Forms.Panel();
            this.labelTray = new System.Windows.Forms.Label();
            this.panelUpdates = new System.Windows.Forms.Panel();
            this.panelTray = new System.Windows.Forms.Panel();
            this.labelUpdates = new System.Windows.Forms.Label();
            this.checkBestImageQuality = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).BeginInit();
            this.panelUI.SuspendLayout();
            this.panelUpdates.SuspendLayout();
            this.panelTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(6, 5);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 0;
            this.checkExpandLinks.Text = "Expand Links When Hovered";
            this.toolTip.SetToolTip(this.checkExpandLinks, "Expands links inside the tweets. If disabled,\r\nthe full links show up in a tooltip instead.");
            this.checkExpandLinks.UseVisualStyleBackColor = true;
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(5, 5);
            this.comboBoxTrayType.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(144, 21);
            this.comboBoxTrayType.TabIndex = 0;
            this.toolTip.SetToolTip(this.comboBoxTrayType, "Changes behavior of the Tray icon.\r\nRight-click the icon for an action menu.");
            // 
            // checkTrayHighlight
            // 
            this.checkTrayHighlight.AutoSize = true;
            this.checkTrayHighlight.Location = new System.Drawing.Point(6, 56);
            this.checkTrayHighlight.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkTrayHighlight.Name = "checkTrayHighlight";
            this.checkTrayHighlight.Size = new System.Drawing.Size(103, 17);
            this.checkTrayHighlight.TabIndex = 2;
            this.checkTrayHighlight.Text = "Enable Highlight";
            this.toolTip.SetToolTip(this.checkTrayHighlight, "Highlights the tray icon if there are new tweets.\r\nOnly works for columns with popup or audio notifications.\r\nThe icon resets when the main window is restored.");
            this.checkTrayHighlight.UseVisualStyleBackColor = true;
            // 
            // checkSpellCheck
            // 
            this.checkSpellCheck.AutoSize = true;
            this.checkSpellCheck.Location = new System.Drawing.Point(6, 74);
            this.checkSpellCheck.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSpellCheck.Name = "checkSpellCheck";
            this.checkSpellCheck.Size = new System.Drawing.Size(119, 17);
            this.checkSpellCheck.TabIndex = 3;
            this.checkSpellCheck.Text = "Enable Spell Check";
            this.toolTip.SetToolTip(this.checkSpellCheck, "Underlines words that are spelled incorrectly.");
            this.checkSpellCheck.UseVisualStyleBackColor = true;
            // 
            // checkUpdateNotifications
            // 
            this.checkUpdateNotifications.AutoSize = true;
            this.checkUpdateNotifications.Location = new System.Drawing.Point(6, 5);
            this.checkUpdateNotifications.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 0;
            this.checkUpdateNotifications.Text = "Check Updates Automatically";
            this.toolTip.SetToolTip(this.checkUpdateNotifications, "Checks for updates every hour.\r\nIf an update is dismissed, it will not appear again.");
            this.checkUpdateNotifications.UseVisualStyleBackColor = true;
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(5, 28);
            this.btnCheckUpdates.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(144, 23);
            this.btnCheckUpdates.TabIndex = 1;
            this.btnCheckUpdates.Text = "Check Updates Now";
            this.toolTip.SetToolTip(this.btnCheckUpdates, "Forces an update check, even for updates that had been dismissed.");
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // labelZoomValue
            // 
            this.labelZoomValue.BackColor = System.Drawing.Color.Transparent;
            this.labelZoomValue.Location = new System.Drawing.Point(147, 123);
            this.labelZoomValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelZoomValue.Name = "labelZoomValue";
            this.labelZoomValue.Size = new System.Drawing.Size(38, 13);
            this.labelZoomValue.TabIndex = 6;
            this.labelZoomValue.Text = "100%";
            this.labelZoomValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip.SetToolTip(this.labelZoomValue, "Changes the zoom level.\r\nAlso affects notifications and screenshots.");
            // 
            // checkSwitchAccountSelectors
            // 
            this.checkSwitchAccountSelectors.AutoSize = true;
            this.checkSwitchAccountSelectors.Location = new System.Drawing.Point(6, 28);
            this.checkSwitchAccountSelectors.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSwitchAccountSelectors.Name = "checkSwitchAccountSelectors";
            this.checkSwitchAccountSelectors.Size = new System.Drawing.Size(172, 17);
            this.checkSwitchAccountSelectors.TabIndex = 1;
            this.checkSwitchAccountSelectors.Text = "Shift Selects Multiple Accounts";
            this.toolTip.SetToolTip(this.checkSwitchAccountSelectors, "When (re)tweeting, click to select a single account or hold Shift to\r\nselect multiple accounts, instead of TweetDeck\'s default behavior.");
            this.checkSwitchAccountSelectors.UseVisualStyleBackColor = true;
            // 
            // labelTrayIcon
            // 
            this.labelTrayIcon.AutoSize = true;
            this.labelTrayIcon.Location = new System.Drawing.Point(3, 38);
            this.labelTrayIcon.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelTrayIcon.Name = "labelTrayIcon";
            this.labelTrayIcon.Size = new System.Drawing.Size(52, 13);
            this.labelTrayIcon.TabIndex = 1;
            this.labelTrayIcon.Text = "Tray Icon";
            // 
            // trackBarZoom
            // 
            this.trackBarZoom.AutoSize = false;
            this.trackBarZoom.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarZoom.LargeChange = 25;
            this.trackBarZoom.Location = new System.Drawing.Point(3, 122);
            this.trackBarZoom.Maximum = 200;
            this.trackBarZoom.Minimum = 50;
            this.trackBarZoom.Name = "trackBarZoom";
            this.trackBarZoom.Size = new System.Drawing.Size(148, 30);
            this.trackBarZoom.SmallChange = 5;
            this.trackBarZoom.TabIndex = 5;
            this.trackBarZoom.TickFrequency = 25;
            this.trackBarZoom.Value = 100;
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(3, 106);
            this.labelZoom.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 4;
            this.labelZoom.Text = "Zoom";
            // 
            // zoomUpdateTimer
            // 
            this.zoomUpdateTimer.Interval = 250;
            this.zoomUpdateTimer.Tick += new System.EventHandler(this.zoomUpdateTimer_Tick);
            // 
            // labelUI
            // 
            this.labelUI.AutoSize = true;
            this.labelUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUI.Location = new System.Drawing.Point(6, 8);
            this.labelUI.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelUI.Name = "labelUI";
            this.labelUI.Size = new System.Drawing.Size(111, 20);
            this.labelUI.TabIndex = 0;
            this.labelUI.Text = "User Interface";
            // 
            // panelUI
            // 
            this.panelUI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelUI.Controls.Add(this.checkBestImageQuality);
            this.panelUI.Controls.Add(this.checkExpandLinks);
            this.panelUI.Controls.Add(this.checkSwitchAccountSelectors);
            this.panelUI.Controls.Add(this.checkSpellCheck);
            this.panelUI.Controls.Add(this.labelZoom);
            this.panelUI.Controls.Add(this.trackBarZoom);
            this.panelUI.Controls.Add(this.labelZoomValue);
            this.panelUI.Location = new System.Drawing.Point(9, 31);
            this.panelUI.Name = "panelUI";
            this.panelUI.Size = new System.Drawing.Size(322, 157);
            this.panelUI.TabIndex = 1;
            // 
            // labelTray
            // 
            this.labelTray.AutoSize = true;
            this.labelTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTray.Location = new System.Drawing.Point(6, 212);
            this.labelTray.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelTray.Name = "labelTray";
            this.labelTray.Size = new System.Drawing.Size(96, 20);
            this.labelTray.TabIndex = 2;
            this.labelTray.Text = "System Tray";
            // 
            // panelUpdates
            // 
            this.panelUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelUpdates.Controls.Add(this.checkUpdateNotifications);
            this.panelUpdates.Controls.Add(this.btnCheckUpdates);
            this.panelUpdates.Location = new System.Drawing.Point(9, 358);
            this.panelUpdates.Name = "panelUpdates";
            this.panelUpdates.Size = new System.Drawing.Size(322, 55);
            this.panelUpdates.TabIndex = 5;
            // 
            // panelTray
            // 
            this.panelTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTray.Controls.Add(this.checkTrayHighlight);
            this.panelTray.Controls.Add(this.comboBoxTrayType);
            this.panelTray.Controls.Add(this.labelTrayIcon);
            this.panelTray.Location = new System.Drawing.Point(9, 235);
            this.panelTray.Name = "panelTray";
            this.panelTray.Size = new System.Drawing.Size(322, 76);
            this.panelTray.TabIndex = 3;
            // 
            // labelUpdates
            // 
            this.labelUpdates.AutoSize = true;
            this.labelUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUpdates.Location = new System.Drawing.Point(6, 335);
            this.labelUpdates.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelUpdates.Name = "labelUpdates";
            this.labelUpdates.Size = new System.Drawing.Size(70, 20);
            this.labelUpdates.TabIndex = 4;
            this.labelUpdates.Text = "Updates";
            // 
            // checkBestImageQuality
            // 
            this.checkBestImageQuality.AutoSize = true;
            this.checkBestImageQuality.Location = new System.Drawing.Point(6, 51);
            this.checkBestImageQuality.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkBestImageQuality.Name = "checkBestImageQuality";
            this.checkBestImageQuality.Size = new System.Drawing.Size(114, 17);
            this.checkBestImageQuality.TabIndex = 2;
            this.checkBestImageQuality.Text = "Best Image Quality";
            this.toolTip.SetToolTip(this.checkBestImageQuality, "When right-clicking a tweet image, the context menu options\r\nwill use links to the original image size (:orig in the URL).");
            this.checkBestImageQuality.UseVisualStyleBackColor = true;
            // 
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelUpdates);
            this.Controls.Add(this.panelTray);
            this.Controls.Add(this.panelUpdates);
            this.Controls.Add(this.labelTray);
            this.Controls.Add(this.panelUI);
            this.Controls.Add(this.labelUI);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(340, 422);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).EndInit();
            this.panelUI.ResumeLayout(false);
            this.panelUI.PerformLayout();
            this.panelUpdates.ResumeLayout(false);
            this.panelUpdates.PerformLayout();
            this.panelTray.ResumeLayout(false);
            this.panelTray.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkExpandLinks;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelTrayIcon;
        private System.Windows.Forms.CheckBox checkTrayHighlight;
        private System.Windows.Forms.CheckBox checkSpellCheck;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.Label labelZoomValue;
        private System.Windows.Forms.TrackBar trackBarZoom;
        private System.Windows.Forms.Timer zoomUpdateTimer;
        private System.Windows.Forms.CheckBox checkSwitchAccountSelectors;
        private System.Windows.Forms.Label labelUI;
        private System.Windows.Forms.Panel panelUI;
        private System.Windows.Forms.Label labelTray;
        private System.Windows.Forms.Panel panelUpdates;
        private System.Windows.Forms.Panel panelTray;
        private System.Windows.Forms.Label labelUpdates;
        private System.Windows.Forms.CheckBox checkBestImageQuality;
    }
}
