namespace TweetDuck.Core.Other.Settings.Dialogs {
    partial class DialogSettingsRestart {
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
            this.btnRestart = new System.Windows.Forms.Button();
            this.cbLogging = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tbDataFolder = new System.Windows.Forms.TextBox();
            this.tbShortcutTarget = new System.Windows.Forms.TextBox();
            this.labelDataFolder = new System.Windows.Forms.Label();
            this.labelShortcutTarget = new System.Windows.Forms.Label();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnCancel.Location = new System.Drawing.Point(215, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnCancel.Size = new System.Drawing.Size(57, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestart.AutoSize = true;
            this.btnRestart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnRestart.Location = new System.Drawing.Point(152, 146);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnRestart.Size = new System.Drawing.Size(57, 25);
            this.btnRestart.TabIndex = 1;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // cbLogging
            // 
            this.cbLogging.AutoSize = true;
            this.cbLogging.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.cbLogging.Location = new System.Drawing.Point(3, 3);
            this.cbLogging.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.cbLogging.Name = "cbLogging";
            this.cbLogging.Size = new System.Drawing.Size(70, 19);
            this.cbLogging.TabIndex = 0;
            this.cbLogging.Text = "Logging";
            this.toolTip.SetToolTip(this.cbLogging, "Logs JavaScript output into TD_Console.txt file in the data folder.");
            this.cbLogging.UseVisualStyleBackColor = true;
            // 
            // tbDataFolder
            // 
            this.tbDataFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.tbDataFolder.Location = new System.Drawing.Point(3, 54);
            this.tbDataFolder.Name = "tbDataFolder";
            this.tbDataFolder.Size = new System.Drawing.Size(260, 23);
            this.tbDataFolder.TabIndex = 2;
            this.toolTip.SetToolTip(this.tbDataFolder, "Path to the data folder. Must be either an absolute path,\r\nor a simple folder nam" +
        "e that will be created in LocalAppData.");
            // 
            // tbShortcutTarget
            // 
            this.tbShortcutTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbShortcutTarget.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbShortcutTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.tbShortcutTarget.Location = new System.Drawing.Point(3, 110);
            this.tbShortcutTarget.Name = "tbShortcutTarget";
            this.tbShortcutTarget.ReadOnly = true;
            this.tbShortcutTarget.Size = new System.Drawing.Size(260, 23);
            this.tbShortcutTarget.TabIndex = 4;
            this.tbShortcutTarget.Click += new System.EventHandler(this.tbShortcutTarget_Click);
            // 
            // labelDataFolder
            // 
            this.labelDataFolder.AutoSize = true;
            this.labelDataFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.labelDataFolder.Location = new System.Drawing.Point(3, 36);
            this.labelDataFolder.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelDataFolder.Name = "labelDataFolder";
            this.labelDataFolder.Size = new System.Drawing.Size(67, 15);
            this.labelDataFolder.TabIndex = 1;
            this.labelDataFolder.Text = "Data Folder";
            // 
            // labelShortcutTarget
            // 
            this.labelShortcutTarget.AutoSize = true;
            this.labelShortcutTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.labelShortcutTarget.Location = new System.Drawing.Point(3, 92);
            this.labelShortcutTarget.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelShortcutTarget.Name = "labelShortcutTarget";
            this.labelShortcutTarget.Size = new System.Drawing.Size(171, 15);
            this.labelShortcutTarget.TabIndex = 3;
            this.labelShortcutTarget.Text = "Shortcut Target (click to select)";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.cbLogging);
            this.flowPanel.Controls.Add(this.labelDataFolder);
            this.flowPanel.Controls.Add(this.tbDataFolder);
            this.flowPanel.Controls.Add(this.labelShortcutTarget);
            this.flowPanel.Controls.Add(this.tbShortcutTarget);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(266, 136);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // DialogSettingsRestart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 183);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogSettingsRestart";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.CheckBox cbLogging;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelDataFolder;
        private System.Windows.Forms.TextBox tbDataFolder;
        private System.Windows.Forms.TextBox tbShortcutTarget;
        private System.Windows.Forms.Label labelShortcutTarget;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
    }
}