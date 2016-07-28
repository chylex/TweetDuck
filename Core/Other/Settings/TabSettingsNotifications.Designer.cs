namespace TweetDck.Core.Other.Settings {
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
            this.btnDurationMedium = new TweetDck.Core.Controls.FlatButton();
            this.btnDurationLong = new TweetDck.Core.Controls.FlatButton();
            this.btnDurationShort = new TweetDck.Core.Controls.FlatButton();
            this.labelDurationValue = new System.Windows.Forms.Label();
            this.trackBarDuration = new System.Windows.Forms.TrackBar();
            this.groupUserInterface = new System.Windows.Forms.GroupBox();
            this.checkTimerCountDown = new System.Windows.Forms.CheckBox();
            this.checkLegacyLoad = new System.Windows.Forms.CheckBox();
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
            this.groupNotificationLocation.Size = new System.Drawing.Size(183, 264);
            this.groupNotificationLocation.TabIndex = 1;
            this.groupNotificationLocation.TabStop = false;
            this.groupNotificationLocation.Text = "Location";
            // 
            // labelEdgeDistanceValue
            // 
            this.labelEdgeDistanceValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEdgeDistanceValue.Location = new System.Drawing.Point(143, 214);
            this.labelEdgeDistanceValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelEdgeDistanceValue.Name = "labelEdgeDistanceValue";
            this.labelEdgeDistanceValue.Size = new System.Drawing.Size(34, 13);
            this.labelEdgeDistanceValue.TabIndex = 11;
            this.labelEdgeDistanceValue.Text = "0 px";
            this.labelEdgeDistanceValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            // radioLocCustom
            // 
            this.radioLocCustom.AutoSize = true;
            this.radioLocCustom.Location = new System.Drawing.Point(7, 116);
            this.radioLocCustom.Name = "radioLocCustom";
            this.radioLocCustom.Size = new System.Drawing.Size(60, 17);
            this.radioLocCustom.TabIndex = 4;
            this.radioLocCustom.TabStop = true;
            this.radioLocCustom.Text = "Custom";
            this.toolTip.SetToolTip(this.radioLocCustom, "Drag the notification window to the desired location.");
            this.radioLocCustom.UseVisualStyleBackColor = true;
            this.radioLocCustom.CheckedChanged += new System.EventHandler(this.radioLoc_CheckedChanged);
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
            this.trackBarEdgeDistance.Size = new System.Drawing.Size(141, 45);
            this.trackBarEdgeDistance.SmallChange = 2;
            this.trackBarEdgeDistance.TabIndex = 5;
            this.trackBarEdgeDistance.TickFrequency = 4;
            this.trackBarEdgeDistance.Value = 8;
            this.trackBarEdgeDistance.ValueChanged += new System.EventHandler(this.trackBarEdgeDistance_ValueChanged);
            // 
            // groupNotificationDuration
            // 
            this.groupNotificationDuration.Controls.Add(this.tableLayoutDurationButtons);
            this.groupNotificationDuration.Controls.Add(this.labelDurationValue);
            this.groupNotificationDuration.Controls.Add(this.trackBarDuration);
            this.groupNotificationDuration.Location = new System.Drawing.Point(9, 106);
            this.groupNotificationDuration.Name = "groupNotificationDuration";
            this.groupNotificationDuration.Size = new System.Drawing.Size(183, 89);
            this.groupNotificationDuration.TabIndex = 9;
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
            this.tableLayoutDurationButtons.TabIndex = 5;
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
            this.btnDurationMedium.TabIndex = 2;
            this.btnDurationMedium.Text = "Medium";
            this.btnDurationMedium.UseVisualStyleBackColor = true;
            this.btnDurationMedium.Click += new System.EventHandler(this.btnDurationMedium_Click);
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
            this.btnDurationLong.TabIndex = 1;
            this.btnDurationLong.Text = "Long";
            this.btnDurationLong.UseVisualStyleBackColor = true;
            this.btnDurationLong.Click += new System.EventHandler(this.btnDurationLong_Click);
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
            this.btnDurationShort.Click += new System.EventHandler(this.btnDurationShort_Click);
            // 
            // labelDurationValue
            // 
            this.labelDurationValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDurationValue.BackColor = System.Drawing.Color.Transparent;
            this.labelDurationValue.Location = new System.Drawing.Point(129, 20);
            this.labelDurationValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelDurationValue.Name = "labelDurationValue";
            this.labelDurationValue.Size = new System.Drawing.Size(48, 13);
            this.labelDurationValue.TabIndex = 13;
            this.labelDurationValue.Text = "0 ms/c";
            this.labelDurationValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip.SetToolTip(this.labelDurationValue, "Milliseconds per character.");
            // 
            // trackBarDuration
            // 
            this.trackBarDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarDuration.Location = new System.Drawing.Point(6, 19);
            this.trackBarDuration.Maximum = 60;
            this.trackBarDuration.Minimum = 10;
            this.trackBarDuration.Name = "trackBarDuration";
            this.trackBarDuration.Size = new System.Drawing.Size(128, 45);
            this.trackBarDuration.TabIndex = 12;
            this.trackBarDuration.TickFrequency = 5;
            this.trackBarDuration.Value = 25;
            this.trackBarDuration.ValueChanged += new System.EventHandler(this.trackBarDuration_ValueChanged);
            // 
            // groupUserInterface
            // 
            this.groupUserInterface.Controls.Add(this.checkTimerCountDown);
            this.groupUserInterface.Controls.Add(this.checkLegacyLoad);
            this.groupUserInterface.Controls.Add(this.checkNotificationTimer);
            this.groupUserInterface.Location = new System.Drawing.Point(9, 9);
            this.groupUserInterface.Name = "groupUserInterface";
            this.groupUserInterface.Size = new System.Drawing.Size(183, 91);
            this.groupUserInterface.TabIndex = 10;
            this.groupUserInterface.TabStop = false;
            this.groupUserInterface.Text = "General";
            // 
            // checkTimerCountDown
            // 
            this.checkTimerCountDown.AutoSize = true;
            this.checkTimerCountDown.Location = new System.Drawing.Point(6, 44);
            this.checkTimerCountDown.Name = "checkTimerCountDown";
            this.checkTimerCountDown.Size = new System.Drawing.Size(119, 17);
            this.checkTimerCountDown.TabIndex = 6;
            this.checkTimerCountDown.Text = "Timer Counts Down";
            this.toolTip.SetToolTip(this.checkTimerCountDown, "The notification timer counts down instead of up.");
            this.checkTimerCountDown.UseVisualStyleBackColor = true;
            this.checkTimerCountDown.CheckedChanged += new System.EventHandler(this.checkTimerCountDown_CheckedChanged);
            // 
            // checkLegacyLoad
            // 
            this.checkLegacyLoad.AutoSize = true;
            this.checkLegacyLoad.Location = new System.Drawing.Point(6, 67);
            this.checkLegacyLoad.Name = "checkLegacyLoad";
            this.checkLegacyLoad.Size = new System.Drawing.Size(139, 17);
            this.checkLegacyLoad.TabIndex = 5;
            this.checkLegacyLoad.Text = "Legacy Loading System";
            this.toolTip.SetToolTip(this.checkLegacyLoad, "Try enabling if notifications do not display.\r\nMight cause delays and visual arti" +
        "facts.");
            this.checkLegacyLoad.UseVisualStyleBackColor = true;
            this.checkLegacyLoad.CheckedChanged += new System.EventHandler(this.checkLegacyLoad_CheckedChanged);
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
            this.toolTip.SetToolTip(this.checkNotificationTimer, "Shows how much time is left before the current notification disappears.");
            this.checkNotificationTimer.UseVisualStyleBackColor = true;
            this.checkNotificationTimer.CheckedChanged += new System.EventHandler(this.checkNotificationTimer_CheckedChanged);
            // 
            // TabSettingsNotifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupUserInterface);
            this.Controls.Add(this.groupNotificationDuration);
            this.Controls.Add(this.groupNotificationLocation);
            this.Name = "TabSettingsNotifications";
            this.Size = new System.Drawing.Size(478, 282);
            this.ParentChanged += new System.EventHandler(this.TabSettingsNotifications_ParentChanged);
            this.groupNotificationLocation.ResumeLayout(false);
            this.groupNotificationLocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).EndInit();
            this.groupNotificationDuration.ResumeLayout(false);
            this.groupNotificationDuration.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkNotificationTimer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelEdgeDistanceValue;
        private System.Windows.Forms.CheckBox checkLegacyLoad;
        private System.Windows.Forms.CheckBox checkTimerCountDown;
        private System.Windows.Forms.Label labelDurationValue;
        private System.Windows.Forms.TrackBar trackBarDuration;
        private System.Windows.Forms.TableLayoutPanel tableLayoutDurationButtons;
        private TweetDck.Core.Controls.FlatButton btnDurationMedium;
        private TweetDck.Core.Controls.FlatButton btnDurationLong;
        private TweetDck.Core.Controls.FlatButton btnDurationShort;
    }
}
