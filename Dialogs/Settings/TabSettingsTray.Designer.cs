namespace TweetDuck.Dialogs.Settings {
    partial class TabSettingsTray {
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowPanelLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.labelTray = new System.Windows.Forms.Label();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.checkTrayHighlight = new System.Windows.Forms.CheckBox();
            this.flowPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            this.flowPanelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowPanelLeft
            // 
            this.flowPanelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPanelLeft.Controls.Add(this.labelTray);
            this.flowPanelLeft.Controls.Add(this.comboBoxTrayType);
            this.flowPanelLeft.Controls.Add(this.labelTrayIcon);
            this.flowPanelLeft.Controls.Add(this.checkTrayHighlight);
            this.flowPanelLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelLeft.Location = new System.Drawing.Point(9, 9);
            this.flowPanelLeft.Name = "flowPanelLeft";
            this.flowPanelLeft.Size = new System.Drawing.Size(300, 462);
            this.flowPanelLeft.TabIndex = 0;
            this.flowPanelLeft.WrapContents = false;
            // 
            // labelTray
            // 
            this.labelTray.AutoSize = true;
            this.labelTray.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelTray.Location = new System.Drawing.Point(0, 0);
            this.labelTray.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.labelTray.Name = "labelTray";
            this.labelTray.Size = new System.Drawing.Size(99, 19);
            this.labelTray.TabIndex = 0;
            this.labelTray.Text = "SYSTEM TRAY";
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(5, 24);
            this.comboBoxTrayType.Margin = new System.Windows.Forms.Padding(5, 4, 3, 3);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(144, 23);
            this.comboBoxTrayType.TabIndex = 1;
            // 
            // labelTrayIcon
            // 
            this.labelTrayIcon.AutoSize = true;
            this.labelTrayIcon.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelTrayIcon.Location = new System.Drawing.Point(3, 59);
            this.labelTrayIcon.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelTrayIcon.Name = "labelTrayIcon";
            this.labelTrayIcon.Size = new System.Drawing.Size(56, 15);
            this.labelTrayIcon.TabIndex = 2;
            this.labelTrayIcon.Text = "Tray Icon";
            // 
            // checkTrayHighlight
            // 
            this.checkTrayHighlight.AutoSize = true;
            this.checkTrayHighlight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkTrayHighlight.Location = new System.Drawing.Point(6, 80);
            this.checkTrayHighlight.Margin = new System.Windows.Forms.Padding(6, 6, 3, 2);
            this.checkTrayHighlight.Name = "checkTrayHighlight";
            this.checkTrayHighlight.Size = new System.Drawing.Size(114, 19);
            this.checkTrayHighlight.TabIndex = 3;
            this.checkTrayHighlight.Text = "Enable Highlight";
            this.checkTrayHighlight.UseVisualStyleBackColor = true;
            // 
            // flowPanelRight
            // 
            this.flowPanelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPanelRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelRight.Location = new System.Drawing.Point(322, 9);
            this.flowPanelRight.Name = "flowPanelRight";
            this.flowPanelRight.Size = new System.Drawing.Size(300, 462);
            this.flowPanelRight.TabIndex = 1;
            this.flowPanelRight.WrapContents = false;
            // 
            // TabSettingsTray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanelRight);
            this.Controls.Add(this.flowPanelLeft);
            this.Name = "TabSettingsTray";
            this.Size = new System.Drawing.Size(631, 480);
            this.flowPanelLeft.ResumeLayout(false);
            this.flowPanelLeft.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.FlowLayoutPanel flowPanelLeft;
        private System.Windows.Forms.Label labelTray;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.Label labelTrayIcon;
        private System.Windows.Forms.CheckBox checkTrayHighlight;
        private System.Windows.Forms.FlowLayoutPanel flowPanelRight;
    }
}
