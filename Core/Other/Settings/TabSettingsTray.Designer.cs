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
            this.checkTrayHighlight = new System.Windows.Forms.CheckBox();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.labelTrayIcon = new System.Windows.Forms.Label();
            this.labelTray = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkTrayHighlight
            // 
            this.checkTrayHighlight.AutoSize = true;
            this.checkTrayHighlight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkTrayHighlight.Location = new System.Drawing.Point(6, 81);
            this.checkTrayHighlight.Margin = new System.Windows.Forms.Padding(6, 6, 3, 2);
            this.checkTrayHighlight.Name = "checkTrayHighlight";
            this.checkTrayHighlight.Size = new System.Drawing.Size(114, 19);
            this.checkTrayHighlight.TabIndex = 3;
            this.checkTrayHighlight.Text = "Enable Highlight";
            this.checkTrayHighlight.UseVisualStyleBackColor = true;
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(5, 25);
            this.comboBoxTrayType.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(144, 23);
            this.comboBoxTrayType.TabIndex = 1;
            // 
            // labelTrayIcon
            // 
            this.labelTrayIcon.AutoSize = true;
            this.labelTrayIcon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTrayIcon.Location = new System.Drawing.Point(3, 60);
            this.labelTrayIcon.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelTrayIcon.Name = "labelTrayIcon";
            this.labelTrayIcon.Size = new System.Drawing.Size(56, 15);
            this.labelTrayIcon.TabIndex = 2;
            this.labelTrayIcon.Text = "Tray Icon";
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
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.labelTray);
            this.flowPanel.Controls.Add(this.comboBoxTrayType);
            this.flowPanel.Controls.Add(this.labelTrayIcon);
            this.flowPanel.Controls.Add(this.checkTrayHighlight);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 102);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // TabSettingsTray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsTray";
            this.Size = new System.Drawing.Size(340, 120);
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox checkTrayHighlight;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.Label labelTrayIcon;
        private System.Windows.Forms.Label labelTray;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
    }
}
