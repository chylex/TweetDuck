namespace TweetDck.Core.Other {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.groupNotificationLocation = new System.Windows.Forms.GroupBox();
            this.labelDisplay = new System.Windows.Forms.Label();
            this.comboBoxDisplay = new System.Windows.Forms.ComboBox();
            this.labelEdgeDistance = new System.Windows.Forms.Label();
            this.trackBarEdgeDistance = new System.Windows.Forms.TrackBar();
            this.radioLocCustom = new System.Windows.Forms.RadioButton();
            this.radioLocBR = new System.Windows.Forms.RadioButton();
            this.radioLocBL = new System.Windows.Forms.RadioButton();
            this.radioLocTR = new System.Windows.Forms.RadioButton();
            this.radioLocTL = new System.Windows.Forms.RadioButton();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tableColumn2Panel = new System.Windows.Forms.Panel();
            this.groupUserInterface = new System.Windows.Forms.GroupBox();
            this.checkExpandLinks = new System.Windows.Forms.CheckBox();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.labelTrayType = new System.Windows.Forms.Label();
            this.checkUpdateNotifications = new System.Windows.Forms.CheckBox();
            this.checkNotificationTimer = new System.Windows.Forms.CheckBox();
            this.groupNotificationDuration = new System.Windows.Forms.GroupBox();
            this.radioDurVeryLong = new System.Windows.Forms.RadioButton();
            this.radioDurLong = new System.Windows.Forms.RadioButton();
            this.radioDurMedium = new System.Windows.Forms.RadioButton();
            this.radioDurShort = new System.Windows.Forms.RadioButton();
            this.tableColumn3Panel = new System.Windows.Forms.Panel();
            this.groupAdvancedSettings = new System.Windows.Forms.GroupBox();
            this.btnClearCache = new System.Windows.Forms.Button();
            this.labelMiscellaneous = new System.Windows.Forms.Label();
            this.checkHardwareAcceleration = new System.Windows.Forms.CheckBox();
            this.tableColumn1Panel = new System.Windows.Forms.Panel();
            this.labelUpdateNotifications = new System.Windows.Forms.Label();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.groupNotificationLocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).BeginInit();
            this.tableLayout.SuspendLayout();
            this.tableColumn2Panel.SuspendLayout();
            this.groupUserInterface.SuspendLayout();
            this.groupNotificationDuration.SuspendLayout();
            this.tableColumn3Panel.SuspendLayout();
            this.groupAdvancedSettings.SuspendLayout();
            this.tableColumn1Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupNotificationLocation
            // 
            this.groupNotificationLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupNotificationLocation.Controls.Add(this.labelDisplay);
            this.groupNotificationLocation.Controls.Add(this.comboBoxDisplay);
            this.groupNotificationLocation.Controls.Add(this.labelEdgeDistance);
            this.groupNotificationLocation.Controls.Add(this.trackBarEdgeDistance);
            this.groupNotificationLocation.Controls.Add(this.radioLocCustom);
            this.groupNotificationLocation.Controls.Add(this.radioLocBR);
            this.groupNotificationLocation.Controls.Add(this.radioLocBL);
            this.groupNotificationLocation.Controls.Add(this.radioLocTR);
            this.groupNotificationLocation.Controls.Add(this.radioLocTL);
            this.groupNotificationLocation.Location = new System.Drawing.Point(3, 3);
            this.groupNotificationLocation.Name = "groupNotificationLocation";
            this.groupNotificationLocation.Size = new System.Drawing.Size(183, 324);
            this.groupNotificationLocation.TabIndex = 0;
            this.groupNotificationLocation.TabStop = false;
            this.groupNotificationLocation.Text = "Notification Location";
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Location = new System.Drawing.Point(3, 148);
            this.labelDisplay.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(41, 13);
            this.labelDisplay.TabIndex = 8;
            this.labelDisplay.Text = "Display";
            // 
            // comboBoxDisplay
            // 
            this.comboBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplay.FormattingEnabled = true;
            this.comboBoxDisplay.Location = new System.Drawing.Point(6, 164);
            this.comboBoxDisplay.Name = "comboBoxDisplay";
            this.comboBoxDisplay.Size = new System.Drawing.Size(171, 21);
            this.comboBoxDisplay.TabIndex = 7;
            this.comboBoxDisplay.SelectedValueChanged += new System.EventHandler(this.comboBoxDisplay_SelectedValueChanged);
            // 
            // labelEdgeDistance
            // 
            this.labelEdgeDistance.AutoSize = true;
            this.labelEdgeDistance.Location = new System.Drawing.Point(3, 197);
            this.labelEdgeDistance.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelEdgeDistance.Name = "labelEdgeDistance";
            this.labelEdgeDistance.Size = new System.Drawing.Size(103, 13);
            this.labelEdgeDistance.TabIndex = 6;
            this.labelEdgeDistance.Text = "Distance From Edge";
            // 
            // trackBarEdgeDistance
            // 
            this.trackBarEdgeDistance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarEdgeDistance.LargeChange = 8;
            this.trackBarEdgeDistance.Location = new System.Drawing.Point(6, 213);
            this.trackBarEdgeDistance.Maximum = 40;
            this.trackBarEdgeDistance.Minimum = 8;
            this.trackBarEdgeDistance.Name = "trackBarEdgeDistance";
            this.trackBarEdgeDistance.Size = new System.Drawing.Size(171, 45);
            this.trackBarEdgeDistance.SmallChange = 2;
            this.trackBarEdgeDistance.TabIndex = 5;
            this.trackBarEdgeDistance.TickFrequency = 2;
            this.trackBarEdgeDistance.Value = 8;
            this.trackBarEdgeDistance.ValueChanged += new System.EventHandler(this.trackBarEdgeDistance_ValueChanged);
            // 
            // radioLocCustom
            // 
            this.radioLocCustom.AutoSize = true;
            this.radioLocCustom.Location = new System.Drawing.Point(7, 116);
            this.radioLocCustom.Name = "radioLocCustom";
            this.radioLocCustom.Size = new System.Drawing.Size(60, 17);
            this.radioLocCustom.TabIndex = 4;
            this.radioLocCustom.TabStop = true;
            this.radioLocCustom.Text = "Custom";
            this.radioLocCustom.UseVisualStyleBackColor = true;
            this.radioLocCustom.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
            this.radioLocCustom.Click += new System.EventHandler(this.radioLoc_Click);
            // 
            // radioLocBR
            // 
            this.radioLocBR.AutoSize = true;
            this.radioLocBR.Location = new System.Drawing.Point(7, 92);
            this.radioLocBR.Name = "radioLocBR";
            this.radioLocBR.Size = new System.Drawing.Size(86, 17);
            this.radioLocBR.TabIndex = 3;
            this.radioLocBR.TabStop = true;
            this.radioLocBR.Text = "Bottom Right";
            this.radioLocBR.UseVisualStyleBackColor = true;
            this.radioLocBR.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
            this.radioLocBR.Click += new System.EventHandler(this.radioLoc_Click);
            // 
            // radioLocBL
            // 
            this.radioLocBL.AutoSize = true;
            this.radioLocBL.Location = new System.Drawing.Point(7, 68);
            this.radioLocBL.Name = "radioLocBL";
            this.radioLocBL.Size = new System.Drawing.Size(79, 17);
            this.radioLocBL.TabIndex = 2;
            this.radioLocBL.TabStop = true;
            this.radioLocBL.Text = "Bottom Left";
            this.radioLocBL.UseVisualStyleBackColor = true;
            this.radioLocBL.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
            this.radioLocBL.Click += new System.EventHandler(this.radioLoc_Click);
            // 
            // radioLocTR
            // 
            this.radioLocTR.AutoSize = true;
            this.radioLocTR.Location = new System.Drawing.Point(7, 44);
            this.radioLocTR.Name = "radioLocTR";
            this.radioLocTR.Size = new System.Drawing.Size(72, 17);
            this.radioLocTR.TabIndex = 1;
            this.radioLocTR.TabStop = true;
            this.radioLocTR.Text = "Top Right";
            this.radioLocTR.UseVisualStyleBackColor = true;
            this.radioLocTR.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
            this.radioLocTR.Click += new System.EventHandler(this.radioLoc_Click);
            // 
            // radioLocTL
            // 
            this.radioLocTL.AutoSize = true;
            this.radioLocTL.Location = new System.Drawing.Point(7, 20);
            this.radioLocTL.Name = "radioLocTL";
            this.radioLocTL.Size = new System.Drawing.Size(65, 17);
            this.radioLocTL.TabIndex = 0;
            this.radioLocTL.TabStop = true;
            this.radioLocTL.Text = "Top Left";
            this.radioLocTL.UseVisualStyleBackColor = true;
            this.radioLocTL.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
            this.radioLocTL.Click += new System.EventHandler(this.radioLoc_Click);
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 3;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayout.Controls.Add(this.tableColumn1Panel, 0, 0);
            this.tableLayout.Controls.Add(this.tableColumn2Panel, 1, 0);
            this.tableLayout.Controls.Add(this.tableColumn3Panel, 2, 0);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Size = new System.Drawing.Size(576, 336);
            this.tableLayout.TabIndex = 2;
            // 
            // tableColumn2Panel
            // 
            this.tableColumn2Panel.Controls.Add(this.groupNotificationDuration);
            this.tableColumn2Panel.Controls.Add(this.groupUserInterface);
            this.tableColumn2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableColumn2Panel.Location = new System.Drawing.Point(192, 3);
            this.tableColumn2Panel.Margin = new System.Windows.Forms.Padding(0);
            this.tableColumn2Panel.Name = "tableColumn2Panel";
            this.tableColumn2Panel.Size = new System.Drawing.Size(189, 330);
            this.tableColumn2Panel.TabIndex = 3;
            // 
            // groupUserInterface
            // 
            this.groupUserInterface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupUserInterface.Controls.Add(this.btnCheckUpdates);
            this.groupUserInterface.Controls.Add(this.labelUpdateNotifications);
            this.groupUserInterface.Controls.Add(this.checkExpandLinks);
            this.groupUserInterface.Controls.Add(this.comboBoxTrayType);
            this.groupUserInterface.Controls.Add(this.labelTrayType);
            this.groupUserInterface.Controls.Add(this.checkUpdateNotifications);
            this.groupUserInterface.Controls.Add(this.checkNotificationTimer);
            this.groupUserInterface.Location = new System.Drawing.Point(3, 128);
            this.groupUserInterface.Name = "groupUserInterface";
            this.groupUserInterface.Size = new System.Drawing.Size(183, 199);
            this.groupUserInterface.TabIndex = 3;
            this.groupUserInterface.TabStop = false;
            this.groupUserInterface.Text = "User Interface";
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(6, 45);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 11;
            this.checkExpandLinks.Text = "Expand Links When Hovered";
            this.checkExpandLinks.UseVisualStyleBackColor = true;
            this.checkExpandLinks.CheckedChanged += new System.EventHandler(this.checkExpandLinks_CheckedChanged);
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(6, 92);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(171, 21);
            this.comboBoxTrayType.TabIndex = 10;
            this.comboBoxTrayType.SelectedIndexChanged += new System.EventHandler(this.comboBoxTrayType_SelectedIndexChanged);
            // 
            // labelTrayType
            // 
            this.labelTrayType.AutoSize = true;
            this.labelTrayType.Location = new System.Drawing.Point(3, 76);
            this.labelTrayType.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
            this.labelTrayType.Name = "labelTrayType";
            this.labelTrayType.Size = new System.Drawing.Size(52, 13);
            this.labelTrayType.TabIndex = 9;
            this.labelTrayType.Text = "Tray Icon";
            // 
            // checkUpdateNotifications
            // 
            this.checkUpdateNotifications.AutoSize = true;
            this.checkUpdateNotifications.Location = new System.Drawing.Point(6, 145);
            this.checkUpdateNotifications.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 5;
            this.checkUpdateNotifications.Text = "Check Updates Automatically";
            this.checkUpdateNotifications.UseVisualStyleBackColor = true;
            this.checkUpdateNotifications.CheckedChanged += new System.EventHandler(this.checkUpdateNotifications_CheckedChanged);
            // 
            // checkNotificationTimer
            // 
            this.checkNotificationTimer.AutoSize = true;
            this.checkNotificationTimer.Location = new System.Drawing.Point(6, 21);
            this.checkNotificationTimer.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkNotificationTimer.Name = "checkNotificationTimer";
            this.checkNotificationTimer.Size = new System.Drawing.Size(145, 17);
            this.checkNotificationTimer.TabIndex = 4;
            this.checkNotificationTimer.Text = "Display Notification Timer";
            this.checkNotificationTimer.UseVisualStyleBackColor = true;
            this.checkNotificationTimer.CheckedChanged += new System.EventHandler(this.checkNotificationTimer_CheckedChanged);
            // 
            // groupNotificationDuration
            // 
            this.groupNotificationDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupNotificationDuration.Controls.Add(this.radioDurVeryLong);
            this.groupNotificationDuration.Controls.Add(this.radioDurLong);
            this.groupNotificationDuration.Controls.Add(this.radioDurMedium);
            this.groupNotificationDuration.Controls.Add(this.radioDurShort);
            this.groupNotificationDuration.Location = new System.Drawing.Point(3, 3);
            this.groupNotificationDuration.Name = "groupNotificationDuration";
            this.groupNotificationDuration.Size = new System.Drawing.Size(183, 119);
            this.groupNotificationDuration.TabIndex = 2;
            this.groupNotificationDuration.TabStop = false;
            this.groupNotificationDuration.Text = "Notification Duration";
            // 
            // radioDurVeryLong
            // 
            this.radioDurVeryLong.AutoSize = true;
            this.radioDurVeryLong.Location = new System.Drawing.Point(6, 92);
            this.radioDurVeryLong.Name = "radioDurVeryLong";
            this.radioDurVeryLong.Size = new System.Drawing.Size(73, 17);
            this.radioDurVeryLong.TabIndex = 3;
            this.radioDurVeryLong.TabStop = true;
            this.radioDurVeryLong.Text = "Very Long";
            this.radioDurVeryLong.UseVisualStyleBackColor = true;
            this.radioDurVeryLong.CheckedChanged += new System.EventHandler(this.radioDur_CheckedChanged);
            this.radioDurVeryLong.Click += new System.EventHandler(this.radioDur_Click);
            // 
            // radioDurLong
            // 
            this.radioDurLong.AutoSize = true;
            this.radioDurLong.Location = new System.Drawing.Point(6, 68);
            this.radioDurLong.Name = "radioDurLong";
            this.radioDurLong.Size = new System.Drawing.Size(49, 17);
            this.radioDurLong.TabIndex = 2;
            this.radioDurLong.TabStop = true;
            this.radioDurLong.Text = "Long";
            this.radioDurLong.UseVisualStyleBackColor = true;
            this.radioDurLong.CheckedChanged += new System.EventHandler(this.radioDur_CheckedChanged);
            this.radioDurLong.Click += new System.EventHandler(this.radioDur_Click);
            // 
            // radioDurMedium
            // 
            this.radioDurMedium.AutoSize = true;
            this.radioDurMedium.Location = new System.Drawing.Point(6, 44);
            this.radioDurMedium.Name = "radioDurMedium";
            this.radioDurMedium.Size = new System.Drawing.Size(62, 17);
            this.radioDurMedium.TabIndex = 1;
            this.radioDurMedium.TabStop = true;
            this.radioDurMedium.Text = "Medium";
            this.radioDurMedium.UseVisualStyleBackColor = true;
            this.radioDurMedium.CheckedChanged += new System.EventHandler(this.radioDur_CheckedChanged);
            this.radioDurMedium.Click += new System.EventHandler(this.radioDur_Click);
            // 
            // radioDurShort
            // 
            this.radioDurShort.AutoSize = true;
            this.radioDurShort.Location = new System.Drawing.Point(6, 20);
            this.radioDurShort.Name = "radioDurShort";
            this.radioDurShort.Size = new System.Drawing.Size(50, 17);
            this.radioDurShort.TabIndex = 0;
            this.radioDurShort.TabStop = true;
            this.radioDurShort.Text = "Short";
            this.radioDurShort.UseVisualStyleBackColor = true;
            this.radioDurShort.CheckedChanged += new System.EventHandler(this.radioDur_CheckedChanged);
            this.radioDurShort.Click += new System.EventHandler(this.radioDur_Click);
            // 
            // tableColumn3Panel
            // 
            this.tableColumn3Panel.Controls.Add(this.groupAdvancedSettings);
            this.tableColumn3Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableColumn3Panel.Location = new System.Drawing.Point(381, 3);
            this.tableColumn3Panel.Margin = new System.Windows.Forms.Padding(0);
            this.tableColumn3Panel.Name = "tableColumn3Panel";
            this.tableColumn3Panel.Size = new System.Drawing.Size(192, 330);
            this.tableColumn3Panel.TabIndex = 4;
            // 
            // groupAdvancedSettings
            // 
            this.groupAdvancedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAdvancedSettings.Controls.Add(this.btnClearCache);
            this.groupAdvancedSettings.Controls.Add(this.labelMiscellaneous);
            this.groupAdvancedSettings.Controls.Add(this.checkHardwareAcceleration);
            this.groupAdvancedSettings.Location = new System.Drawing.Point(3, 3);
            this.groupAdvancedSettings.Name = "groupAdvancedSettings";
            this.groupAdvancedSettings.Size = new System.Drawing.Size(183, 324);
            this.groupAdvancedSettings.TabIndex = 0;
            this.groupAdvancedSettings.TabStop = false;
            this.groupAdvancedSettings.Text = "Advanced Settings";
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(9, 68);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(171, 23);
            this.btnClearCache.TabIndex = 11;
            this.btnClearCache.Text = "Clear Cache (calculating)";
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // labelMiscellaneous
            // 
            this.labelMiscellaneous.AutoSize = true;
            this.labelMiscellaneous.Location = new System.Drawing.Point(6, 52);
            this.labelMiscellaneous.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
            this.labelMiscellaneous.Name = "labelMiscellaneous";
            this.labelMiscellaneous.Size = new System.Drawing.Size(74, 13);
            this.labelMiscellaneous.TabIndex = 10;
            this.labelMiscellaneous.Text = "Miscellaneous";
            // 
            // checkHardwareAcceleration
            // 
            this.checkHardwareAcceleration.AutoSize = true;
            this.checkHardwareAcceleration.Location = new System.Drawing.Point(6, 21);
            this.checkHardwareAcceleration.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.checkHardwareAcceleration.Name = "checkHardwareAcceleration";
            this.checkHardwareAcceleration.Size = new System.Drawing.Size(134, 17);
            this.checkHardwareAcceleration.TabIndex = 5;
            this.checkHardwareAcceleration.Text = "Hardware Acceleration";
            this.checkHardwareAcceleration.UseVisualStyleBackColor = true;
            this.checkHardwareAcceleration.CheckedChanged += new System.EventHandler(this.checkHardwareAcceleration_CheckedChanged);
            // 
            // tableColumn1Panel
            // 
            this.tableColumn1Panel.Controls.Add(this.groupNotificationLocation);
            this.tableColumn1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableColumn1Panel.Location = new System.Drawing.Point(3, 3);
            this.tableColumn1Panel.Margin = new System.Windows.Forms.Padding(0);
            this.tableColumn1Panel.Name = "tableColumn1Panel";
            this.tableColumn1Panel.Size = new System.Drawing.Size(189, 330);
            this.tableColumn1Panel.TabIndex = 5;
            // 
            // labelUpdateNotifications
            // 
            this.labelUpdateNotifications.AutoSize = true;
            this.labelUpdateNotifications.Location = new System.Drawing.Point(3, 127);
            this.labelUpdateNotifications.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
            this.labelUpdateNotifications.Name = "labelUpdateNotifications";
            this.labelUpdateNotifications.Size = new System.Drawing.Size(103, 13);
            this.labelUpdateNotifications.TabIndex = 12;
            this.labelUpdateNotifications.Text = "Update Notifications";
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(6, 168);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(171, 23);
            this.btnCheckUpdates.TabIndex = 13;
            this.btnCheckUpdates.Text = "Check Updates Now";
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            this.btnCheckUpdates.Click += new System.EventHandler(this.btnCheckUpdates_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 336);
            this.Controls.Add(this.tableLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.groupNotificationLocation.ResumeLayout(false);
            this.groupNotificationLocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).EndInit();
            this.tableLayout.ResumeLayout(false);
            this.tableColumn2Panel.ResumeLayout(false);
            this.groupUserInterface.ResumeLayout(false);
            this.groupUserInterface.PerformLayout();
            this.groupNotificationDuration.ResumeLayout(false);
            this.groupNotificationDuration.PerformLayout();
            this.tableColumn3Panel.ResumeLayout(false);
            this.groupAdvancedSettings.ResumeLayout(false);
            this.groupAdvancedSettings.PerformLayout();
            this.tableColumn1Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupNotificationLocation;
        private System.Windows.Forms.RadioButton radioLocCustom;
        private System.Windows.Forms.RadioButton radioLocBR;
        private System.Windows.Forms.RadioButton radioLocBL;
        private System.Windows.Forms.RadioButton radioLocTR;
        private System.Windows.Forms.RadioButton radioLocTL;
        private System.Windows.Forms.Label labelEdgeDistance;
        private System.Windows.Forms.TrackBar trackBarEdgeDistance;
        private System.Windows.Forms.Label labelDisplay;
        private System.Windows.Forms.ComboBox comboBoxDisplay;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Panel tableColumn2Panel;
        private System.Windows.Forms.GroupBox groupUserInterface;
        private System.Windows.Forms.GroupBox groupNotificationDuration;
        private System.Windows.Forms.RadioButton radioDurVeryLong;
        private System.Windows.Forms.RadioButton radioDurLong;
        private System.Windows.Forms.RadioButton radioDurMedium;
        private System.Windows.Forms.RadioButton radioDurShort;
        private System.Windows.Forms.CheckBox checkNotificationTimer;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.Label labelTrayType;
        private System.Windows.Forms.CheckBox checkExpandLinks;
        private System.Windows.Forms.Panel tableColumn3Panel;
        private System.Windows.Forms.GroupBox groupAdvancedSettings;
        private System.Windows.Forms.CheckBox checkHardwareAcceleration;
        private System.Windows.Forms.Label labelMiscellaneous;
        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.Panel tableColumn1Panel;
        private System.Windows.Forms.Label labelUpdateNotifications;
        private System.Windows.Forms.Button btnCheckUpdates;
    }
}