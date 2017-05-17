namespace TweetDuck.Core.Other.Settings {
    partial class TabSettingsNotifications {
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
            this.groupNotificationLocation = new System.Windows.Forms.GroupBox();
            this.labelEdgeDistanceValue = new System.Windows.Forms.Label();
            this.labelDisplay = new System.Windows.Forms.Label();
            this.comboBoxDisplay = new System.Windows.Forms.ComboBox();
            this.labelEdgeDistance = new System.Windows.Forms.Label();
            this.radioLocCustom = new System.Windows.Forms.RadioButton();
            this.radioLocBR = new System.Windows.Forms.RadioButton();
            this.radioLocBL = new System.Windows.Forms.RadioButton();
            this.radioLocTR = new System.Windows.Forms.RadioButton();
            this.radioLocTL = new System.Windows.Forms.RadioButton();
            this.trackBarEdgeDistance = new System.Windows.Forms.TrackBar();
            this.groupNotificationDuration = new System.Windows.Forms.GroupBox();
            this.tableLayoutDurationButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnDurationMedium = new TweetDuck.Core.Controls.FlatButton();
            this.btnDurationLong = new TweetDuck.Core.Controls.FlatButton();
            this.btnDurationShort = new TweetDuck.Core.Controls.FlatButton();
            this.labelDurationValue = new System.Windows.Forms.Label();
            this.trackBarDuration = new System.Windows.Forms.TrackBar();
            this.groupUserInterface = new System.Windows.Forms.GroupBox();
            this.checkSkipOnLinkClick = new System.Windows.Forms.CheckBox();
            this.checkColumnName = new System.Windows.Forms.CheckBox();
            this.labelIdlePause = new System.Windows.Forms.Label();
            this.comboBoxIdlePause = new System.Windows.Forms.ComboBox();
            this.checkNonIntrusive = new System.Windows.Forms.CheckBox();
            this.checkTimerCountDown = new System.Windows.Forms.CheckBox();
            this.checkNotificationTimer = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupNotificationLocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).BeginInit();
            this.groupNotificationDuration.SuspendLayout();
            this.tableLayoutDurationButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).BeginInit();
            this.groupUserInterface.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupNotificationLocation
            // 
            this.groupNotificationLocation.Controls.Add(this.labelEdgeDistanceValue);
            this.groupNotificationLocation.Controls.Add(this.labelDisplay);
            this.groupNotificationLocation.Controls.Add(this.comboBoxDisplay);
            this.groupNotificationLocation.Controls.Add(this.labelEdgeDistance);
            this.groupNotificationLocation.Controls.Add(this.radioLocCustom);
            this.groupNotificationLocation.Controls.Add(this.radioLocBR);
            this.groupNotificationLocation.Controls.Add(this.radioLocBL);
            this.groupNotificationLocation.Controls.Add(this.radioLocTR);
            this.groupNotificationLocation.Controls.Add(this.radioLocTL);
            this.groupNotificationLocation.Controls.Add(this.trackBarEdgeDistance);
            this.groupNotificationLocation.Location = new System.Drawing.Point(198, 9);
            this.groupNotificationLocation.Name = "groupNotificationLocation";
            this.groupNotificationLocation.Size = new System.Drawing.Size(183, 282);
            this.groupNotificationLocation.TabIndex = 2;
            this.groupNotificationLocation.TabStop = false;
            this.groupNotificationLocation.Text = "Location";
            // 
            // labelEdgeDistanceValue
            // 
            this.labelEdgeDistanceValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEdgeDistanceValue.Location = new System.Drawing.Point(143, 217);
            this.labelEdgeDistanceValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelEdgeDistanceValue.Name = "labelEdgeDistanceValue";
            this.labelEdgeDistanceValue.Size = new System.Drawing.Size(34, 13);
            this.labelEdgeDistanceValue.TabIndex = 9;
            this.labelEdgeDistanceValue.Text = "0 px";
            this.labelEdgeDistanceValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Location = new System.Drawing.Point(5, 144);
            this.labelDisplay.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(41, 13);
            this.labelDisplay.TabIndex = 5;
            this.labelDisplay.Text = "Display";
            // 
            // comboBoxDisplay
            // 
            this.comboBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplay.FormattingEnabled = true;
            this.comboBoxDisplay.Location = new System.Drawing.Point(6, 160);
            this.comboBoxDisplay.Name = "comboBoxDisplay";
            this.comboBoxDisplay.Size = new System.Drawing.Size(171, 21);
            this.comboBoxDisplay.TabIndex = 6;
            // 
            // labelEdgeDistance
            // 
            this.labelEdgeDistance.AutoSize = true;
            this.labelEdgeDistance.Location = new System.Drawing.Point(5, 196);
            this.labelEdgeDistance.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelEdgeDistance.Name = "labelEdgeDistance";
            this.labelEdgeDistance.Size = new System.Drawing.Size(103, 13);
            this.labelEdgeDistance.TabIndex = 7;
            this.labelEdgeDistance.Text = "Distance From Edge";
            // 
            // radioLocCustom
            // 
            this.radioLocCustom.AutoSize = true;
            this.radioLocCustom.Location = new System.Drawing.Point(8, 112);
            this.radioLocCustom.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.radioLocCustom.Name = "radioLocCustom";
            this.radioLocCustom.Size = new System.Drawing.Size(60, 17);
            this.radioLocCustom.TabIndex = 4;
            this.radioLocCustom.TabStop = true;
            this.radioLocCustom.Text = "Custom";
            this.toolTip.SetToolTip(this.radioLocCustom, "Drag the notification window to the desired location.");
            this.radioLocCustom.UseVisualStyleBackColor = true;
            // 
            // radioLocBR
            // 
            this.radioLocBR.AutoSize = true;
            this.radioLocBR.Location = new System.Drawing.Point(8, 89);
            this.radioLocBR.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.radioLocBR.Name = "radioLocBR";
            this.radioLocBR.Size = new System.Drawing.Size(86, 17);
            this.radioLocBR.TabIndex = 3;
            this.radioLocBR.TabStop = true;
            this.radioLocBR.Text = "Bottom Right";
            this.radioLocBR.UseVisualStyleBackColor = true;
            // 
            // radioLocBL
            // 
            this.radioLocBL.AutoSize = true;
            this.radioLocBL.Location = new System.Drawing.Point(8, 66);
            this.radioLocBL.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.radioLocBL.Name = "radioLocBL";
            this.radioLocBL.Size = new System.Drawing.Size(79, 17);
            this.radioLocBL.TabIndex = 2;
            this.radioLocBL.TabStop = true;
            this.radioLocBL.Text = "Bottom Left";
            this.radioLocBL.UseVisualStyleBackColor = true;
            // 
            // radioLocTR
            // 
            this.radioLocTR.AutoSize = true;
            this.radioLocTR.Location = new System.Drawing.Point(8, 43);
            this.radioLocTR.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.radioLocTR.Name = "radioLocTR";
            this.radioLocTR.Size = new System.Drawing.Size(72, 17);
            this.radioLocTR.TabIndex = 1;
            this.radioLocTR.TabStop = true;
            this.radioLocTR.Text = "Top Right";
            this.radioLocTR.UseVisualStyleBackColor = true;
            // 
            // radioLocTL
            // 
            this.radioLocTL.AutoSize = true;
            this.radioLocTL.Location = new System.Drawing.Point(8, 20);
            this.radioLocTL.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocTL.Name = "radioLocTL";
            this.radioLocTL.Size = new System.Drawing.Size(65, 17);
            this.radioLocTL.TabIndex = 0;
            this.radioLocTL.TabStop = true;
            this.radioLocTL.Text = "Top Left";
            this.radioLocTL.UseVisualStyleBackColor = true;
            // 
            // trackBarEdgeDistance
            // 
            this.trackBarEdgeDistance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarEdgeDistance.AutoSize = false;
            this.trackBarEdgeDistance.LargeChange = 8;
            this.trackBarEdgeDistance.Location = new System.Drawing.Point(8, 212);
            this.trackBarEdgeDistance.Maximum = 40;
            this.trackBarEdgeDistance.Minimum = 8;
            this.trackBarEdgeDistance.Name = "trackBarEdgeDistance";
            this.trackBarEdgeDistance.Size = new System.Drawing.Size(141, 30);
            this.trackBarEdgeDistance.SmallChange = 2;
            this.trackBarEdgeDistance.TabIndex = 8;
            this.trackBarEdgeDistance.TickFrequency = 4;
            this.trackBarEdgeDistance.Value = 8;
            // 
            // groupNotificationDuration
            // 
            this.groupNotificationDuration.Controls.Add(this.tableLayoutDurationButtons);
            this.groupNotificationDuration.Controls.Add(this.labelDurationValue);
            this.groupNotificationDuration.Controls.Add(this.trackBarDuration);
            this.groupNotificationDuration.Location = new System.Drawing.Point(9, 202);
            this.groupNotificationDuration.Name = "groupNotificationDuration";
            this.groupNotificationDuration.Size = new System.Drawing.Size(183, 89);
            this.groupNotificationDuration.TabIndex = 1;
            this.groupNotificationDuration.TabStop = false;
            this.groupNotificationDuration.Text = "Duration";
            // 
            // tableLayoutDurationButtons
            // 
            this.tableLayoutDurationButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutDurationButtons.ColumnCount = 3;
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationMedium, 0, 0);
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationLong, 1, 0);
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationShort, 0, 0);
            this.tableLayoutDurationButtons.Location = new System.Drawing.Point(6, 56);
            this.tableLayoutDurationButtons.Name = "tableLayoutDurationButtons";
            this.tableLayoutDurationButtons.RowCount = 1;
            this.tableLayoutDurationButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutDurationButtons.Size = new System.Drawing.Size(171, 27);
            this.tableLayoutDurationButtons.TabIndex = 2;
            // 
            // btnDurationMedium
            // 
            this.btnDurationMedium.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDurationMedium.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnDurationMedium.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDurationMedium.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnDurationMedium.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDurationMedium.Location = new System.Drawing.Point(55, 1);
            this.btnDurationMedium.Margin = new System.Windows.Forms.Padding(1);
            this.btnDurationMedium.Name = "btnDurationMedium";
            this.btnDurationMedium.Size = new System.Drawing.Size(59, 25);
            this.btnDurationMedium.TabIndex = 1;
            this.btnDurationMedium.Text = "Medium";
            this.btnDurationMedium.UseVisualStyleBackColor = true;
            // 
            // btnDurationLong
            // 
            this.btnDurationLong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDurationLong.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnDurationLong.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDurationLong.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnDurationLong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDurationLong.Location = new System.Drawing.Point(116, 1);
            this.btnDurationLong.Margin = new System.Windows.Forms.Padding(1);
            this.btnDurationLong.Name = "btnDurationLong";
            this.btnDurationLong.Size = new System.Drawing.Size(54, 25);
            this.btnDurationLong.TabIndex = 2;
            this.btnDurationLong.Text = "Long";
            this.btnDurationLong.UseVisualStyleBackColor = true;
            // 
            // btnDurationShort
            // 
            this.btnDurationShort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDurationShort.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnDurationShort.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDurationShort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnDurationShort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDurationShort.Location = new System.Drawing.Point(1, 1);
            this.btnDurationShort.Margin = new System.Windows.Forms.Padding(1);
            this.btnDurationShort.Name = "btnDurationShort";
            this.btnDurationShort.Size = new System.Drawing.Size(52, 25);
            this.btnDurationShort.TabIndex = 0;
            this.btnDurationShort.Text = "Short";
            this.btnDurationShort.UseVisualStyleBackColor = true;
            // 
            // labelDurationValue
            // 
            this.labelDurationValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDurationValue.BackColor = System.Drawing.Color.Transparent;
            this.labelDurationValue.Location = new System.Drawing.Point(129, 20);
            this.labelDurationValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelDurationValue.Name = "labelDurationValue";
            this.labelDurationValue.Size = new System.Drawing.Size(48, 13);
            this.labelDurationValue.TabIndex = 1;
            this.labelDurationValue.Text = "0 ms/c";
            this.labelDurationValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip.SetToolTip(this.labelDurationValue, "Milliseconds per character.");
            // 
            // trackBarDuration
            // 
            this.trackBarDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarDuration.AutoSize = false;
            this.trackBarDuration.Location = new System.Drawing.Point(6, 19);
            this.trackBarDuration.Maximum = 60;
            this.trackBarDuration.Minimum = 10;
            this.trackBarDuration.Name = "trackBarDuration";
            this.trackBarDuration.Size = new System.Drawing.Size(128, 30);
            this.trackBarDuration.TabIndex = 0;
            this.trackBarDuration.TickFrequency = 5;
            this.trackBarDuration.Value = 25;
            // 
            // groupUserInterface
            // 
            this.groupUserInterface.Controls.Add(this.checkSkipOnLinkClick);
            this.groupUserInterface.Controls.Add(this.checkColumnName);
            this.groupUserInterface.Controls.Add(this.labelIdlePause);
            this.groupUserInterface.Controls.Add(this.comboBoxIdlePause);
            this.groupUserInterface.Controls.Add(this.checkNonIntrusive);
            this.groupUserInterface.Controls.Add(this.checkTimerCountDown);
            this.groupUserInterface.Controls.Add(this.checkNotificationTimer);
            this.groupUserInterface.Location = new System.Drawing.Point(9, 9);
            this.groupUserInterface.Name = "groupUserInterface";
            this.groupUserInterface.Size = new System.Drawing.Size(183, 187);
            this.groupUserInterface.TabIndex = 0;
            this.groupUserInterface.TabStop = false;
            this.groupUserInterface.Text = "General";
            // 
            // checkSkipOnLinkClick
            // 
            this.checkSkipOnLinkClick.AutoSize = true;
            this.checkSkipOnLinkClick.Location = new System.Drawing.Point(9, 90);
            this.checkSkipOnLinkClick.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSkipOnLinkClick.Name = "checkSkipOnLinkClick";
            this.checkSkipOnLinkClick.Size = new System.Drawing.Size(113, 17);
            this.checkSkipOnLinkClick.TabIndex = 3;
            this.checkSkipOnLinkClick.Text = "Skip On Link Click";
            this.toolTip.SetToolTip(this.checkSkipOnLinkClick, "Skips current notification when a link\r\ninside the notification is clicked.");
            this.checkSkipOnLinkClick.UseVisualStyleBackColor = true;
            // 
            // checkColumnName
            // 
            this.checkColumnName.AutoSize = true;
            this.checkColumnName.Location = new System.Drawing.Point(9, 21);
            this.checkColumnName.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkColumnName.Name = "checkColumnName";
            this.checkColumnName.Size = new System.Drawing.Size(129, 17);
            this.checkColumnName.TabIndex = 0;
            this.checkColumnName.Text = "Display Column Name";
            this.toolTip.SetToolTip(this.checkColumnName, "Shows column name each notification originated\r\nfrom in the notification window t" +
        "itle.");
            this.checkColumnName.UseVisualStyleBackColor = true;
            // 
            // labelIdlePause
            // 
            this.labelIdlePause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelIdlePause.AutoSize = true;
            this.labelIdlePause.Location = new System.Drawing.Point(5, 141);
            this.labelIdlePause.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelIdlePause.Name = "labelIdlePause";
            this.labelIdlePause.Size = new System.Drawing.Size(89, 13);
            this.labelIdlePause.TabIndex = 3;
            this.labelIdlePause.Text = "Pause When Idle";
            // 
            // comboBoxIdlePause
            // 
            this.comboBoxIdlePause.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIdlePause.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdlePause.FormattingEnabled = true;
            this.comboBoxIdlePause.Location = new System.Drawing.Point(6, 157);
            this.comboBoxIdlePause.Name = "comboBoxIdlePause";
            this.comboBoxIdlePause.Size = new System.Drawing.Size(171, 21);
            this.comboBoxIdlePause.TabIndex = 4;
            this.toolTip.SetToolTip(this.comboBoxIdlePause, "Pauses new notifications after going idle for a set amount of time.");
            // 
            // checkNonIntrusive
            // 
            this.checkNonIntrusive.AutoSize = true;
            this.checkNonIntrusive.Location = new System.Drawing.Point(9, 113);
            this.checkNonIntrusive.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkNonIntrusive.Name = "checkNonIntrusive";
            this.checkNonIntrusive.Size = new System.Drawing.Size(128, 17);
            this.checkNonIntrusive.TabIndex = 4;
            this.checkNonIntrusive.Text = "Non-Intrusive Popups";
            this.toolTip.SetToolTip(this.checkNonIntrusive, "When not idle and the cursor is within the notification window area,\r\nit will be " +
        "delayed until the cursor moves away to prevent accidental clicks.");
            this.checkNonIntrusive.UseVisualStyleBackColor = true;
            // 
            // checkTimerCountDown
            // 
            this.checkTimerCountDown.AutoSize = true;
            this.checkTimerCountDown.Location = new System.Drawing.Point(9, 67);
            this.checkTimerCountDown.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkTimerCountDown.Name = "checkTimerCountDown";
            this.checkTimerCountDown.Size = new System.Drawing.Size(119, 17);
            this.checkTimerCountDown.TabIndex = 2;
            this.checkTimerCountDown.Text = "Timer Counts Down";
            this.toolTip.SetToolTip(this.checkTimerCountDown, "The notification timer counts down instead of up.");
            this.checkTimerCountDown.UseVisualStyleBackColor = true;
            // 
            // checkNotificationTimer
            // 
            this.checkNotificationTimer.AutoSize = true;
            this.checkNotificationTimer.Location = new System.Drawing.Point(9, 44);
            this.checkNotificationTimer.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkNotificationTimer.Name = "checkNotificationTimer";
            this.checkNotificationTimer.Size = new System.Drawing.Size(145, 17);
            this.checkNotificationTimer.TabIndex = 1;
            this.checkNotificationTimer.Text = "Display Notification Timer";
            this.checkNotificationTimer.UseVisualStyleBackColor = true;
            // 
            // TabSettingsNotifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupUserInterface);
            this.Controls.Add(this.groupNotificationDuration);
            this.Controls.Add(this.groupNotificationLocation);
            this.Name = "TabSettingsNotifications";
            this.Size = new System.Drawing.Size(478, 300);
            this.ParentChanged += new System.EventHandler(this.TabSettingsNotifications_ParentChanged);
            this.groupNotificationLocation.ResumeLayout(false);
            this.groupNotificationLocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).EndInit();
            this.groupNotificationDuration.ResumeLayout(false);
            this.tableLayoutDurationButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).EndInit();
            this.groupUserInterface.ResumeLayout(false);
            this.groupUserInterface.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupNotificationLocation;
        private System.Windows.Forms.Label labelDisplay;
        private System.Windows.Forms.ComboBox comboBoxDisplay;
        private System.Windows.Forms.Label labelEdgeDistance;
        private System.Windows.Forms.TrackBar trackBarEdgeDistance;
        private System.Windows.Forms.RadioButton radioLocCustom;
        private System.Windows.Forms.RadioButton radioLocBR;
        private System.Windows.Forms.RadioButton radioLocBL;
        private System.Windows.Forms.RadioButton radioLocTR;
        private System.Windows.Forms.RadioButton radioLocTL;
        private System.Windows.Forms.GroupBox groupNotificationDuration;
        private System.Windows.Forms.GroupBox groupUserInterface;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelEdgeDistanceValue;
        private System.Windows.Forms.Label labelDurationValue;
        private System.Windows.Forms.TrackBar trackBarDuration;
        private System.Windows.Forms.TableLayoutPanel tableLayoutDurationButtons;
        private TweetDuck.Core.Controls.FlatButton btnDurationMedium;
        private TweetDuck.Core.Controls.FlatButton btnDurationLong;
        private TweetDuck.Core.Controls.FlatButton btnDurationShort;
        private System.Windows.Forms.CheckBox checkNonIntrusive;
        private System.Windows.Forms.Label labelIdlePause;
        private System.Windows.Forms.ComboBox comboBoxIdlePause;
        private System.Windows.Forms.CheckBox checkColumnName;
        private System.Windows.Forms.CheckBox checkSkipOnLinkClick;
        private System.Windows.Forms.CheckBox checkTimerCountDown;
        private System.Windows.Forms.CheckBox checkNotificationTimer;
    }
}
