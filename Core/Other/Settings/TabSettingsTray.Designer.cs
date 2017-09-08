namespace TweetDuck.Core.Other.Settings {
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
            this.panelTray = new System.Windows.Forms.Panel();
            this.checkTrayHighlight = new System.Windows.Forms.CheckBox();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.labelTray = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTray
            // 
            this.panelTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTray.Controls.Add(this.checkTrayHighlight);
            this.panelTray.Controls.Add(this.comboBoxTrayType);
            this.panelTray.Controls.Add(this.labelTrayIcon);
            this.panelTray.Location = new System.Drawing.Point(9, 31);
            this.panelTray.Name = "panelTray";
            this.panelTray.Size = new System.Drawing.Size(322, 76);
            this.panelTray.TabIndex = 1;
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
            // labelTray
            // 
            this.labelTray.AutoSize = true;
            this.labelTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTray.Location = new System.Drawing.Point(6, 8);
            this.labelTray.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelTray.Name = "labelTray";
            this.labelTray.Size = new System.Drawing.Size(96, 20);
            this.labelTray.TabIndex = 0;
            this.labelTray.Text = "System Tray";
            // 
            // TabSettingsTray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelTray);
            this.Controls.Add(this.labelTray);
            this.Name = "TabSettingsTray";
            this.Size = new System.Drawing.Size(340, 119);
            this.panelTray.ResumeLayout(false);
            this.panelTray.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTray;
        private System.Windows.Forms.CheckBox checkTrayHighlight;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.Label labelTrayIcon;
        private System.Windows.Forms.Label labelTray;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
