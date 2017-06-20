namespace TweetDuck.Core.Other.Settings {
    partial class TabSettingsAdvanced {
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
            this.btnClearCache = new System.Windows.Forms.Button();
            this.checkHardwareAcceleration = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnEditCefArgs = new System.Windows.Forms.Button();
            this.btnEditCSS = new System.Windows.Forms.Button();
            this.btnRestartArgs = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnOpenAppFolder = new System.Windows.Forms.Button();
            this.btnOpenDataFolder = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.labelApp = new System.Windows.Forms.Label();
            this.panelApp = new System.Windows.Forms.Panel();
            this.labelPerformance = new System.Windows.Forms.Label();
            this.panelPerformance = new System.Windows.Forms.Panel();
            this.panelConfiguration = new System.Windows.Forms.Panel();
            this.labelConfiguration = new System.Windows.Forms.Label();
            this.panelApp.SuspendLayout();
            this.panelPerformance.SuspendLayout();
            this.panelConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(3, 28);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(171, 23);
            this.btnClearCache.TabIndex = 1;
            this.btnClearCache.Text = "Clear Cache (calculating)";
            this.toolTip.SetToolTip(this.btnClearCache, "Clearing cache will free up space taken by downloaded images and other resources." +
        "");
            this.btnClearCache.UseVisualStyleBackColor = true;
            // 
            // checkHardwareAcceleration
            // 
            this.checkHardwareAcceleration.AutoSize = true;
            this.checkHardwareAcceleration.Location = new System.Drawing.Point(6, 5);
            this.checkHardwareAcceleration.Margin = new System.Windows.Forms.Padding(6, 5, 3, 3);
            this.checkHardwareAcceleration.Name = "checkHardwareAcceleration";
            this.checkHardwareAcceleration.Size = new System.Drawing.Size(134, 17);
            this.checkHardwareAcceleration.TabIndex = 0;
            this.checkHardwareAcceleration.Text = "Hardware Acceleration";
            this.toolTip.SetToolTip(this.checkHardwareAcceleration, "Uses your graphics card to improve performance.\r\nDisable if you experience issues" +
        " with rendering.");
            this.checkHardwareAcceleration.UseVisualStyleBackColor = true;
            // 
            // btnEditCefArgs
            // 
            this.btnEditCefArgs.Location = new System.Drawing.Point(3, 3);
            this.btnEditCefArgs.Name = "btnEditCefArgs";
            this.btnEditCefArgs.Size = new System.Drawing.Size(171, 23);
            this.btnEditCefArgs.TabIndex = 0;
            this.btnEditCefArgs.Text = "Edit CEF Arguments";
            this.toolTip.SetToolTip(this.btnEditCefArgs, "Set custom command line arguments for Chromium Embedded Framework.");
            this.btnEditCefArgs.UseVisualStyleBackColor = true;
            // 
            // btnEditCSS
            // 
            this.btnEditCSS.Location = new System.Drawing.Point(3, 32);
            this.btnEditCSS.Name = "btnEditCSS";
            this.btnEditCSS.Size = new System.Drawing.Size(171, 23);
            this.btnEditCSS.TabIndex = 1;
            this.btnEditCSS.Text = "Edit CSS";
            this.toolTip.SetToolTip(this.btnEditCSS, "Set custom CSS for browser and notification windows.");
            this.btnEditCSS.UseVisualStyleBackColor = true;
            // 
            // btnRestartArgs
            // 
            this.btnRestartArgs.Location = new System.Drawing.Point(3, 90);
            this.btnRestartArgs.Name = "btnRestartArgs";
            this.btnRestartArgs.Size = new System.Drawing.Size(171, 23);
            this.btnRestartArgs.TabIndex = 3;
            this.btnRestartArgs.Text = "Restart with Arguments";
            this.toolTip.SetToolTip(this.btnRestartArgs, "Restarts the program with customizable\r\ncommand line arguments.");
            this.btnRestartArgs.UseVisualStyleBackColor = true;
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(3, 61);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(171, 23);
            this.btnRestart.TabIndex = 2;
            this.btnRestart.Text = "Restart the Program";
            this.toolTip.SetToolTip(this.btnRestart, "Restarts the program using the same command\r\nline arguments that were used at lau" +
        "nch.");
            this.btnRestart.UseVisualStyleBackColor = true;
            // 
            // btnOpenAppFolder
            // 
            this.btnOpenAppFolder.Location = new System.Drawing.Point(3, 3);
            this.btnOpenAppFolder.Name = "btnOpenAppFolder";
            this.btnOpenAppFolder.Size = new System.Drawing.Size(171, 23);
            this.btnOpenAppFolder.TabIndex = 0;
            this.btnOpenAppFolder.Text = "Open Program Folder";
            this.toolTip.SetToolTip(this.btnOpenAppFolder, "Opens the folder where the app is located.");
            this.btnOpenAppFolder.UseVisualStyleBackColor = true;
            // 
            // btnOpenDataFolder
            // 
            this.btnOpenDataFolder.Location = new System.Drawing.Point(3, 32);
            this.btnOpenDataFolder.Name = "btnOpenDataFolder";
            this.btnOpenDataFolder.Size = new System.Drawing.Size(171, 23);
            this.btnOpenDataFolder.TabIndex = 1;
            this.btnOpenDataFolder.Text = "Open Data Folder";
            this.toolTip.SetToolTip(this.btnOpenDataFolder, "Opens the folder where your profile data is located.");
            this.btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.AutoSize = true;
            this.btnReset.Location = new System.Drawing.Point(190, 388);
            this.btnReset.Name = "btnReset";
            this.btnReset.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnReset.Size = new System.Drawing.Size(102, 23);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Restore Defaults";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.AutoSize = true;
            this.btnImport.Location = new System.Drawing.Point(100, 388);
            this.btnImport.Name = "btnImport";
            this.btnImport.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnImport.Size = new System.Drawing.Size(84, 23);
            this.btnImport.TabIndex = 7;
            this.btnImport.Text = "Import Profile";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.AutoSize = true;
            this.btnExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExport.Location = new System.Drawing.Point(9, 388);
            this.btnExport.Name = "btnExport";
            this.btnExport.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnExport.Size = new System.Drawing.Size(85, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export Profile";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // labelApp
            // 
            this.labelApp.AutoSize = true;
            this.labelApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelApp.Location = new System.Drawing.Point(6, 8);
            this.labelApp.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelApp.Name = "labelApp";
            this.labelApp.Size = new System.Drawing.Size(38, 20);
            this.labelApp.TabIndex = 0;
            this.labelApp.Text = "App";
            // 
            // panelApp
            // 
            this.panelApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelApp.Controls.Add(this.btnOpenDataFolder);
            this.panelApp.Controls.Add(this.btnOpenAppFolder);
            this.panelApp.Controls.Add(this.btnRestart);
            this.panelApp.Controls.Add(this.btnRestartArgs);
            this.panelApp.Location = new System.Drawing.Point(9, 31);
            this.panelApp.Name = "panelApp";
            this.panelApp.Size = new System.Drawing.Size(322, 117);
            this.panelApp.TabIndex = 1;
            // 
            // labelPerformance
            // 
            this.labelPerformance.AutoSize = true;
            this.labelPerformance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelPerformance.Location = new System.Drawing.Point(6, 172);
            this.labelPerformance.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelPerformance.Name = "labelPerformance";
            this.labelPerformance.Size = new System.Drawing.Size(100, 20);
            this.labelPerformance.TabIndex = 2;
            this.labelPerformance.Text = "Performance";
            // 
            // panelPerformance
            // 
            this.panelPerformance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPerformance.Controls.Add(this.checkHardwareAcceleration);
            this.panelPerformance.Controls.Add(this.btnClearCache);
            this.panelPerformance.Location = new System.Drawing.Point(9, 195);
            this.panelPerformance.Name = "panelPerformance";
            this.panelPerformance.Size = new System.Drawing.Size(322, 54);
            this.panelPerformance.TabIndex = 3;
            // 
            // panelConfiguration
            // 
            this.panelConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelConfiguration.Controls.Add(this.btnEditCSS);
            this.panelConfiguration.Controls.Add(this.btnEditCefArgs);
            this.panelConfiguration.Location = new System.Drawing.Point(9, 296);
            this.panelConfiguration.Name = "panelConfiguration";
            this.panelConfiguration.Size = new System.Drawing.Size(322, 58);
            this.panelConfiguration.TabIndex = 5;
            // 
            // labelConfiguration
            // 
            this.labelConfiguration.AutoSize = true;
            this.labelConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelConfiguration.Location = new System.Drawing.Point(6, 273);
            this.labelConfiguration.Margin = new System.Windows.Forms.Padding(0, 21, 0, 0);
            this.labelConfiguration.Name = "labelConfiguration";
            this.labelConfiguration.Size = new System.Drawing.Size(104, 20);
            this.labelConfiguration.TabIndex = 4;
            this.labelConfiguration.Text = "Configuration";
            // 
            // TabSettingsAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelConfiguration);
            this.Controls.Add(this.panelConfiguration);
            this.Controls.Add(this.panelPerformance);
            this.Controls.Add(this.labelPerformance);
            this.Controls.Add(this.panelApp);
            this.Controls.Add(this.labelApp);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Name = "TabSettingsAdvanced";
            this.Size = new System.Drawing.Size(340, 420);
            this.panelApp.ResumeLayout(false);
            this.panelPerformance.ResumeLayout(false);
            this.panelPerformance.PerformLayout();
            this.panelConfiguration.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.CheckBox checkHardwareAcceleration;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnEditCefArgs;
        private System.Windows.Forms.Button btnEditCSS;
        private System.Windows.Forms.Button btnRestartArgs;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnOpenAppFolder;
        private System.Windows.Forms.Button btnOpenDataFolder;
        private System.Windows.Forms.Label labelApp;
        private System.Windows.Forms.Panel panelApp;
        private System.Windows.Forms.Label labelPerformance;
        private System.Windows.Forms.Panel panelPerformance;
        private System.Windows.Forms.Panel panelConfiguration;
        private System.Windows.Forms.Label labelConfiguration;
    }
}
