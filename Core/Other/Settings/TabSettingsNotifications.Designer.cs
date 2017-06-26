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
            this.tableLayoutDurationButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnDurationMedium = new TweetDuck.Core.Controls.FlatButton();
            this.btnDurationLong = new TweetDuck.Core.Controls.FlatButton();
            this.btnDurationShort = new TweetDuck.Core.Controls.FlatButton();
            this.labelDurationValue = new System.Windows.Forms.Label();
            this.trackBarDuration = new System.Windows.Forms.TrackBar();
            this.checkSkipOnLinkClick = new System.Windows.Forms.CheckBox();
            this.checkColumnName = new System.Windows.Forms.CheckBox();
            this.labelIdlePause = new System.Windows.Forms.Label();
            this.comboBoxIdlePause = new System.Windows.Forms.ComboBox();
            this.checkNonIntrusive = new System.Windows.Forms.CheckBox();
            this.checkTimerCountDown = new System.Windows.Forms.CheckBox();
            this.checkNotificationTimer = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.labelGeneral = new System.Windows.Forms.Label();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.labelScrollSpeedValue = new System.Windows.Forms.Label();
            this.trackBarScrollSpeed = new System.Windows.Forms.TrackBar();
            this.labelScrollSpeed = new System.Windows.Forms.Label();
            this.labelLocation = new System.Windows.Forms.Label();
            this.panelLocation = new System.Windows.Forms.Panel();
            this.panelTimer = new System.Windows.Forms.Panel();
            this.labelDuration = new System.Windows.Forms.Label();
            this.labelTimer = new System.Windows.Forms.Label();
            this.labelMiscellaneous = new System.Windows.Forms.Label();
            this.panelMiscellaneous = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).BeginInit();
            this.tableLayoutDurationButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).BeginInit();
            this.panelGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScrollSpeed)).BeginInit();
            this.panelLocation.SuspendLayout();
            this.panelTimer.SuspendLayout();
            this.panelMiscellaneous.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelEdgeDistanceValue
            // 
            this.labelEdgeDistanceValue.Location = new System.Drawing.Point(147, 129);
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
            this.labelDisplay.Location = new System.Drawing.Point(3, 60);
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
            this.comboBoxDisplay.Location = new System.Drawing.Point(5, 76);
            this.comboBoxDisplay.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxDisplay.Name = "comboBoxDisplay";
            this.comboBoxDisplay.Size = new System.Drawing.Size(144, 21);
            this.comboBoxDisplay.TabIndex = 6;
            // 
            // labelEdgeDistance
            // 
            this.labelEdgeDistance.AutoSize = true;
            this.labelEdgeDistance.Location = new System.Drawing.Point(3, 112);
            this.labelEdgeDistance.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelEdgeDistance.Name = "labelEdgeDistance";
            this.labelEdgeDistance.Size = new System.Drawing.Size(103, 13);
            this.labelEdgeDistance.TabIndex = 7;
            this.labelEdgeDistance.Text = "Distance From Edge";
            // 
            // radioLocCustom
            // 
            this.radioLocCustom.Location = new System.Drawing.Point(205, 4);
            this.radioLocCustom.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocCustom.Name = "radioLocCustom";
            this.radioLocCustom.Size = new System.Drawing.Size(65, 41);
            this.radioLocCustom.TabIndex = 4;
            this.radioLocCustom.TabStop = true;
            this.radioLocCustom.Text = "Custom";
            this.toolTip.SetToolTip(this.radioLocCustom, "Drag the notification window to the desired location.");
            this.radioLocCustom.UseVisualStyleBackColor = true;
            // 
            // radioLocBR
            // 
            this.radioLocBR.Location = new System.Drawing.Point(105, 28);
            this.radioLocBR.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocBR.Name = "radioLocBR";
            this.radioLocBR.Size = new System.Drawing.Size(92, 17);
            this.radioLocBR.TabIndex = 3;
            this.radioLocBR.TabStop = true;
            this.radioLocBR.Text = "Bottom Right";
            this.radioLocBR.UseVisualStyleBackColor = true;
            // 
            // radioLocBL
            // 
            this.radioLocBL.Location = new System.Drawing.Point(5, 28);
            this.radioLocBL.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocBL.Name = "radioLocBL";
            this.radioLocBL.Size = new System.Drawing.Size(92, 17);
            this.radioLocBL.TabIndex = 2;
            this.radioLocBL.TabStop = true;
            this.radioLocBL.Text = "Bottom Left";
            this.radioLocBL.UseVisualStyleBackColor = true;
            // 
            // radioLocTR
            // 
            this.radioLocTR.Location = new System.Drawing.Point(105, 4);
            this.radioLocTR.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocTR.Name = "radioLocTR";
            this.radioLocTR.Size = new System.Drawing.Size(92, 17);
            this.radioLocTR.TabIndex = 1;
            this.radioLocTR.TabStop = true;
            this.radioLocTR.Text = "Top Right";
            this.radioLocTR.UseVisualStyleBackColor = true;
            // 
            // radioLocTL
            // 
            this.radioLocTL.Location = new System.Drawing.Point(5, 4);
            this.radioLocTL.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.radioLocTL.Name = "radioLocTL";
            this.radioLocTL.Size = new System.Drawing.Size(92, 17);
            this.radioLocTL.TabIndex = 0;
            this.radioLocTL.TabStop = true;
            this.radioLocTL.Text = "Top Left";
            this.radioLocTL.UseVisualStyleBackColor = true;
            // 
            // trackBarEdgeDistance
            // 
            this.trackBarEdgeDistance.AutoSize = false;
            this.trackBarEdgeDistance.LargeChange = 8;
            this.trackBarEdgeDistance.Location = new System.Drawing.Point(5, 128);
            this.trackBarEdgeDistance.Maximum = 40;
            this.trackBarEdgeDistance.Minimum = 8;
            this.trackBarEdgeDistance.Name = "trackBarEdgeDistance";
            this.trackBarEdgeDistance.Size = new System.Drawing.Size(148, 30);
            this.trackBarEdgeDistance.SmallChange = 2;
            this.trackBarEdgeDistance.TabIndex = 8;
            this.trackBarEdgeDistance.TickFrequency = 4;
            this.trackBarEdgeDistance.Value = 8;
            // 
            // tableLayoutDurationButtons
            // 
            this.tableLayoutDurationButtons.ColumnCount = 3;
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.tableLayoutDurationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationMedium, 0, 0);
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationLong, 1, 0);
            this.tableLayoutDurationButtons.Controls.Add(this.btnDurationShort, 0, 0);
            this.tableLayoutDurationButtons.Location = new System.Drawing.Point(3, 113);
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
            this.labelDurationValue.BackColor = System.Drawing.Color.Transparent;
            this.labelDurationValue.Location = new System.Drawing.Point(147, 77);
            this.labelDurationValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelDurationValue.Name = "labelDurationValue";
            this.labelDurationValue.Size = new System.Drawing.Size(48, 13);
            this.labelDurationValue.TabIndex = 4;
            this.labelDurationValue.Text = "0 ms/c";
            this.labelDurationValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip.SetToolTip(this.labelDurationValue, "Milliseconds per character.");
            // 
            // trackBarDuration
            // 
            this.trackBarDuration.AutoSize = false;
            this.trackBarDuration.Location = new System.Drawing.Point(3, 76);
            this.trackBarDuration.Maximum = 60;
            this.trackBarDuration.Minimum = 10;
            this.trackBarDuration.Name = "trackBarDuration";
            this.trackBarDuration.Size = new System.Drawing.Size(148, 30);
            this.trackBarDuration.TabIndex = 3;
            this.trackBarDuration.TickFrequency = 5;
            this.trackBarDuration.Value = 25;
            // 
            // checkSkipOnLinkClick
            // 
            this.checkSkipOnLinkClick.AutoSize = true;
            this.checkSkipOnLinkClick.Location = new System.Drawing.Point(6, 28);
            this.checkSkipOnLinkClick.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkSkipOnLinkClick.Name = "checkSkipOnLinkClick";
            this.checkSkipOnLinkClick.Size = new System.Drawing.Size(113, 17);
            this.checkSkipOnLinkClick.TabIndex = 1;
            this.checkSkipOnLinkClick.Text = "Skip On Link Click";
            this.toolTip.SetToolTip(this.checkSkipOnLinkClick, "Skips current notification when a link\r\ninside the notification is clicked.");
            this.checkSkipOnLinkClick.UseVisualStyleBackColor = true;
            // 
            // checkColumnName
            // 
            this.checkColumnName.AutoSize = true;
            this.checkColumnName.Location = new System.Drawing.Point(6, 5);
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
            this.labelIdlePause.AutoSize = true;
            this.labelIdlePause.Location = new System.Drawing.Point(3, 60);
            this.labelIdlePause.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelIdlePause.Name = "labelIdlePause";
            this.labelIdlePause.Size = new System.Drawing.Size(89, 13);
            this.labelIdlePause.TabIndex = 2;
            this.labelIdlePause.Text = "Pause When Idle";
            // 
            // comboBoxIdlePause
            // 
            this.comboBoxIdlePause.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdlePause.FormattingEnabled = true;
            this.comboBoxIdlePause.Location = new System.Drawing.Point(5, 76);
            this.comboBoxIdlePause.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxIdlePause.Name = "comboBoxIdlePause";
            this.comboBoxIdlePause.Size = new System.Drawing.Size(144, 21);
            this.comboBoxIdlePause.TabIndex = 3;
            this.toolTip.SetToolTip(this.comboBoxIdlePause, "Pauses new notifications after going idle for a set amount of time.");
            // 
            // checkNonIntrusive
            // 
            this.checkNonIntrusive.AutoSize = true;
            this.checkNonIntrusive.Location = new System.Drawing.Point(6, 5);
            this.checkNonIntrusive.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkNonIntrusive.Name = "checkNonIntrusive";
            this.checkNonIntrusive.Size = new System.Drawing.Size(128, 17);
            this.checkNonIntrusive.TabIndex = 0;
            this.checkNonIntrusive.Text = "Non-Intrusive Popups";
            this.toolTip.SetToolTip(this.checkNonIntrusive, "When not idle and the cursor is within the notification window area,\r\nit will be " +
        "delayed until the cursor moves away to prevent accidental clicks.");
            this.checkNonIntrusive.UseVisualStyleBackColor = true;
            // 
            // checkTimerCountDown
            // 
            this.checkTimerCountDown.AutoSize = true;
            this.checkTimerCountDown.Location = new System.Drawing.Point(6, 28);
            this.checkTimerCountDown.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkTimerCountDown.Name = "checkTimerCountDown";
            this.checkTimerCountDown.Size = new System.Drawing.Size(119, 17);
            this.checkTimerCountDown.TabIndex = 1;
            this.checkTimerCountDown.Text = "Timer Counts Down";
            this.toolTip.SetToolTip(this.checkTimerCountDown, "The notification timer counts down instead of up.");
            this.checkTimerCountDown.UseVisualStyleBackColor = true;
            // 
            // checkNotificationTimer
            // 
            this.checkNotificationTimer.AutoSize = true;
            this.checkNotificationTimer.Location = new System.Drawing.Point(6, 5);
            this.checkNotificationTimer.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkNotificationTimer.Name = "checkNotificationTimer";
            this.checkNotificationTimer.Size = new System.Drawing.Size(145, 17);
            this.checkNotificationTimer.TabIndex = 0;
            this.checkNotificationTimer.Text = "Display Notification Timer";
            this.checkNotificationTimer.UseVisualStyleBackColor = true;
            // 
            // labelGeneral
            // 
            this.labelGeneral.AutoSize = true;
            this.labelGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelGeneral.Location = new System.Drawing.Point(6, 8);
            this.labelGeneral.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelGeneral.Name = "labelGeneral";
            this.labelGeneral.Size = new System.Drawing.Size(66, 20);
            this.labelGeneral.TabIndex = 0;
            this.labelGeneral.Text = "General";
            // 
            // panelGeneral
            // 
            this.panelGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGeneral.Controls.Add(this.checkColumnName);
            this.panelGeneral.Controls.Add(this.checkSkipOnLinkClick);
            this.panelGeneral.Controls.Add(this.labelIdlePause);
            this.panelGeneral.Controls.Add(this.comboBoxIdlePause);
            this.panelGeneral.Location = new System.Drawing.Point(9, 31);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(322, 103);
            this.panelGeneral.TabIndex = 1;
            // 
            // labelScrollSpeedValue
            // 
            this.labelScrollSpeedValue.Location = new System.Drawing.Point(147, 54);
            this.labelScrollSpeedValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelScrollSpeedValue.Name = "labelScrollSpeedValue";
            this.labelScrollSpeedValue.Size = new System.Drawing.Size(34, 13);
            this.labelScrollSpeedValue.TabIndex = 3;
            this.labelScrollSpeedValue.Text = "100%";
            this.labelScrollSpeedValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // trackBarScrollSpeed
            // 
            this.trackBarScrollSpeed.AutoSize = false;
            this.trackBarScrollSpeed.LargeChange = 25;
            this.trackBarScrollSpeed.Location = new System.Drawing.Point(5, 53);
            this.trackBarScrollSpeed.Maximum = 200;
            this.trackBarScrollSpeed.Minimum = 25;
            this.trackBarScrollSpeed.Name = "trackBarScrollSpeed";
            this.trackBarScrollSpeed.Size = new System.Drawing.Size(148, 30);
            this.trackBarScrollSpeed.SmallChange = 5;
            this.trackBarScrollSpeed.TabIndex = 2;
            this.trackBarScrollSpeed.TickFrequency = 25;
            this.trackBarScrollSpeed.Value = 100;
            // 
            // labelScrollSpeed
            // 
            this.labelScrollSpeed.AutoSize = true;
            this.labelScrollSpeed.Location = new System.Drawing.Point(3, 37);
            this.labelScrollSpeed.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelScrollSpeed.Name = "labelScrollSpeed";
            this.labelScrollSpeed.Size = new System.Drawing.Size(67, 13);
            this.labelScrollSpeed.TabIndex = 1;
            this.labelScrollSpeed.Text = "Scroll Speed";
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelLocation.Location = new System.Drawing.Point(6, 158);
            this.labelLocation.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(70, 20);
            this.labelLocation.TabIndex = 2;
            this.labelLocation.Text = "Location";
            // 
            // panelLocation
            // 
            this.panelLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLocation.Controls.Add(this.labelEdgeDistanceValue);
            this.panelLocation.Controls.Add(this.radioLocTL);
            this.panelLocation.Controls.Add(this.labelDisplay);
            this.panelLocation.Controls.Add(this.trackBarEdgeDistance);
            this.panelLocation.Controls.Add(this.comboBoxDisplay);
            this.panelLocation.Controls.Add(this.radioLocTR);
            this.panelLocation.Controls.Add(this.labelEdgeDistance);
            this.panelLocation.Controls.Add(this.radioLocBL);
            this.panelLocation.Controls.Add(this.radioLocCustom);
            this.panelLocation.Controls.Add(this.radioLocBR);
            this.panelLocation.Location = new System.Drawing.Point(9, 181);
            this.panelLocation.Name = "panelLocation";
            this.panelLocation.Size = new System.Drawing.Size(322, 165);
            this.panelLocation.TabIndex = 3;
            // 
            // panelTimer
            // 
            this.panelTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTimer.Controls.Add(this.labelDuration);
            this.panelTimer.Controls.Add(this.checkNotificationTimer);
            this.panelTimer.Controls.Add(this.tableLayoutDurationButtons);
            this.panelTimer.Controls.Add(this.checkTimerCountDown);
            this.panelTimer.Controls.Add(this.labelDurationValue);
            this.panelTimer.Controls.Add(this.trackBarDuration);
            this.panelTimer.Location = new System.Drawing.Point(9, 393);
            this.panelTimer.Name = "panelTimer";
            this.panelTimer.Size = new System.Drawing.Size(322, 144);
            this.panelTimer.TabIndex = 5;
            // 
            // labelDuration
            // 
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new System.Drawing.Point(3, 60);
            this.labelDuration.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(47, 13);
            this.labelDuration.TabIndex = 2;
            this.labelDuration.Text = "Duration";
            // 
            // labelTimer
            // 
            this.labelTimer.AutoSize = true;
            this.labelTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTimer.Location = new System.Drawing.Point(6, 370);
            this.labelTimer.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(48, 20);
            this.labelTimer.TabIndex = 4;
            this.labelTimer.Text = "Timer";
            // 
            // labelMiscellaneous
            // 
            this.labelMiscellaneous.AutoSize = true;
            this.labelMiscellaneous.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelMiscellaneous.Location = new System.Drawing.Point(6, 561);
            this.labelMiscellaneous.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelMiscellaneous.Name = "labelMiscellaneous";
            this.labelMiscellaneous.Size = new System.Drawing.Size(109, 20);
            this.labelMiscellaneous.TabIndex = 6;
            this.labelMiscellaneous.Text = "Miscellaneous";
            // 
            // panelMiscellaneous
            // 
            this.panelMiscellaneous.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMiscellaneous.Controls.Add(this.labelScrollSpeedValue);
            this.panelMiscellaneous.Controls.Add(this.trackBarScrollSpeed);
            this.panelMiscellaneous.Controls.Add(this.checkNonIntrusive);
            this.panelMiscellaneous.Controls.Add(this.labelScrollSpeed);
            this.panelMiscellaneous.Location = new System.Drawing.Point(9, 584);
            this.panelMiscellaneous.Name = "panelMiscellaneous";
            this.panelMiscellaneous.Size = new System.Drawing.Size(322, 90);
            this.panelMiscellaneous.TabIndex = 7;
            // 
            // TabSettingsNotifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMiscellaneous);
            this.Controls.Add(this.labelMiscellaneous);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.panelLocation);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.panelGeneral);
            this.Controls.Add(this.labelGeneral);
            this.Controls.Add(this.panelTimer);
            this.Name = "TabSettingsNotifications";
            this.Size = new System.Drawing.Size(340, 684);
            this.ParentChanged += new System.EventHandler(this.TabSettingsNotifications_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEdgeDistance)).EndInit();
            this.tableLayoutDurationButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).EndInit();
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScrollSpeed)).EndInit();
            this.panelLocation.ResumeLayout(false);
            this.panelLocation.PerformLayout();
            this.panelTimer.ResumeLayout(false);
            this.panelTimer.PerformLayout();
            this.panelMiscellaneous.ResumeLayout(false);
            this.panelMiscellaneous.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelDisplay;
        private System.Windows.Forms.ComboBox comboBoxDisplay;
        private System.Windows.Forms.Label labelEdgeDistance;
        private System.Windows.Forms.TrackBar trackBarEdgeDistance;
        private System.Windows.Forms.RadioButton radioLocCustom;
        private System.Windows.Forms.RadioButton radioLocBR;
        private System.Windows.Forms.RadioButton radioLocBL;
        private System.Windows.Forms.RadioButton radioLocTR;
        private System.Windows.Forms.RadioButton radioLocTL;
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
        private System.Windows.Forms.Label labelGeneral;
        private System.Windows.Forms.Panel panelGeneral;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.Panel panelLocation;
        private System.Windows.Forms.Panel panelTimer;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.Label labelScrollSpeedValue;
        private System.Windows.Forms.TrackBar trackBarScrollSpeed;
        private System.Windows.Forms.Label labelScrollSpeed;
        private System.Windows.Forms.Label labelMiscellaneous;
        private System.Windows.Forms.Panel panelMiscellaneous;
        private System.Windows.Forms.Label labelDuration;
    }
}
