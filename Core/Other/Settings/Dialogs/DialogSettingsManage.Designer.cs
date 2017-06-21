namespace TweetDuck.Core.Other.Settings.Dialogs {
    partial class DialogSettingsManage {
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.cbConfig = new System.Windows.Forms.CheckBox();
            this.cbSession = new System.Windows.Forms.CheckBox();
            this.cbPluginData = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelExport = new System.Windows.Forms.Panel();
            this.panelDecision = new System.Windows.Forms.Panel();
            this.radioReset = new System.Windows.Forms.RadioButton();
            this.radioExport = new System.Windows.Forms.RadioButton();
            this.radioImport = new System.Windows.Forms.RadioButton();
            this.panelExport.SuspendLayout();
            this.panelDecision.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.Location = new System.Drawing.Point(176, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinue.AutoSize = true;
            this.btnContinue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnContinue.Enabled = false;
            this.btnContinue.Location = new System.Drawing.Point(125, 97);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnContinue.Size = new System.Drawing.Size(45, 23);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.Text = "Next";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // cbConfig
            // 
            this.cbConfig.AutoSize = true;
            this.cbConfig.Location = new System.Drawing.Point(0, 3);
            this.cbConfig.Name = "cbConfig";
            this.cbConfig.Size = new System.Drawing.Size(104, 17);
            this.cbConfig.TabIndex = 0;
            this.cbConfig.Text = "Program Options";
            this.toolTip.SetToolTip(this.cbConfig, "Interface, notification, and update options.");
            this.cbConfig.UseVisualStyleBackColor = true;
            this.cbConfig.CheckedChanged += new System.EventHandler(this.cbConfig_CheckedChanged);
            // 
            // cbSession
            // 
            this.cbSession.AutoSize = true;
            this.cbSession.Location = new System.Drawing.Point(0, 27);
            this.cbSession.Name = "cbSession";
            this.cbSession.Size = new System.Drawing.Size(92, 17);
            this.cbSession.TabIndex = 1;
            this.cbSession.Text = "Login Session";
            this.toolTip.SetToolTip(this.cbSession, "A token that allows logging into the\r\ncurrent TweetDeck account.");
            this.cbSession.UseVisualStyleBackColor = true;
            this.cbSession.CheckedChanged += new System.EventHandler(this.cbSession_CheckedChanged);
            // 
            // cbPluginData
            // 
            this.cbPluginData.AutoSize = true;
            this.cbPluginData.Location = new System.Drawing.Point(0, 51);
            this.cbPluginData.Name = "cbPluginData";
            this.cbPluginData.Size = new System.Drawing.Size(81, 17);
            this.cbPluginData.TabIndex = 2;
            this.cbPluginData.Text = "Plugin Data";
            this.toolTip.SetToolTip(this.cbPluginData, "Data files generated by plugins.\r\nDoes not include the plugins themselves.");
            this.cbPluginData.UseVisualStyleBackColor = true;
            this.cbPluginData.CheckedChanged += new System.EventHandler(this.cbPluginData_CheckedChanged);
            // 
            // panelExport
            // 
            this.panelExport.Controls.Add(this.cbConfig);
            this.panelExport.Controls.Add(this.cbPluginData);
            this.panelExport.Controls.Add(this.cbSession);
            this.panelExport.Location = new System.Drawing.Point(12, 12);
            this.panelExport.Name = "panelExport";
            this.panelExport.Size = new System.Drawing.Size(220, 79);
            this.panelExport.TabIndex = 5;
            this.panelExport.Visible = false;
            // 
            // panelDecision
            // 
            this.panelDecision.Controls.Add(this.radioReset);
            this.panelDecision.Controls.Add(this.radioExport);
            this.panelDecision.Controls.Add(this.radioImport);
            this.panelDecision.Location = new System.Drawing.Point(12, 12);
            this.panelDecision.Name = "panelDecision";
            this.panelDecision.Size = new System.Drawing.Size(220, 79);
            this.panelDecision.TabIndex = 6;
            // 
            // radioReset
            // 
            this.radioReset.AutoSize = true;
            this.radioReset.Location = new System.Drawing.Point(3, 49);
            this.radioReset.Name = "radioReset";
            this.radioReset.Size = new System.Drawing.Size(104, 17);
            this.radioReset.TabIndex = 2;
            this.radioReset.TabStop = true;
            this.radioReset.Text = "Restore Defaults";
            this.radioReset.UseVisualStyleBackColor = true;
            this.radioReset.CheckedChanged += new System.EventHandler(this.radioDecision_CheckedChanged);
            // 
            // radioExport
            // 
            this.radioExport.AutoSize = true;
            this.radioExport.Location = new System.Drawing.Point(3, 26);
            this.radioExport.Name = "radioExport";
            this.radioExport.Size = new System.Drawing.Size(87, 17);
            this.radioExport.TabIndex = 1;
            this.radioExport.TabStop = true;
            this.radioExport.Text = "Export Profile";
            this.radioExport.UseVisualStyleBackColor = true;
            this.radioExport.CheckedChanged += new System.EventHandler(this.radioDecision_CheckedChanged);
            // 
            // radioImport
            // 
            this.radioImport.AutoSize = true;
            this.radioImport.Location = new System.Drawing.Point(3, 3);
            this.radioImport.Name = "radioImport";
            this.radioImport.Size = new System.Drawing.Size(86, 17);
            this.radioImport.TabIndex = 0;
            this.radioImport.TabStop = true;
            this.radioImport.Text = "Import Profile";
            this.radioImport.UseVisualStyleBackColor = true;
            this.radioImport.CheckedChanged += new System.EventHandler(this.radioDecision_CheckedChanged);
            // 
            // DialogSettingsManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 132);
            this.Controls.Add(this.panelDecision);
            this.Controls.Add(this.panelExport);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 170);
            this.Name = "DialogSettingsManage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Options";
            this.panelExport.ResumeLayout(false);
            this.panelExport.PerformLayout();
            this.panelDecision.ResumeLayout(false);
            this.panelDecision.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.CheckBox cbConfig;
        private System.Windows.Forms.CheckBox cbSession;
        private System.Windows.Forms.CheckBox cbPluginData;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panelExport;
        private System.Windows.Forms.Panel panelDecision;
        private System.Windows.Forms.RadioButton radioReset;
        private System.Windows.Forms.RadioButton radioExport;
        private System.Windows.Forms.RadioButton radioImport;
    }
}