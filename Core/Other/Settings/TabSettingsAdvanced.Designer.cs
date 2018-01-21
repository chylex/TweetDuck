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
            this.numClearCacheThreshold = new TweetDuck.Core.Controls.NumericUpDownEx();
            this.checkClearCacheAuto = new System.Windows.Forms.CheckBox();
            this.labelApp = new System.Windows.Forms.Label();
            this.panelAppButtons = new System.Windows.Forms.Panel();
            this.labelPerformance = new System.Windows.Forms.Label();
            this.panelClearCacheAuto = new System.Windows.Forms.Panel();
            this.labelCache = new System.Windows.Forms.Label();
            this.panelConfiguration = new System.Windows.Forms.Panel();
            this.labelConfiguration = new System.Windows.Forms.Label();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numClearCacheThreshold)).BeginInit();
            this.panelAppButtons.SuspendLayout();
            this.panelClearCacheAuto.SuspendLayout();
            this.panelConfiguration.SuspendLayout();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(5, 172);
            this.btnClearCache.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(144, 23);
            this.btnClearCache.TabIndex = 5;
            this.btnClearCache.Text = "Clear Cache (calculating)";
            this.btnClearCache.UseVisualStyleBackColor = true;
            // 
            // checkHardwareAcceleration
            // 
            this.checkHardwareAcceleration.AutoSize = true;
            this.checkHardwareAcceleration.Location = new System.Drawing.Point(6, 124);
            this.checkHardwareAcceleration.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.checkHardwareAcceleration.Name = "checkHardwareAcceleration";
            this.checkHardwareAcceleration.Size = new System.Drawing.Size(134, 17);
            this.checkHardwareAcceleration.TabIndex = 3;
            this.checkHardwareAcceleration.Text = "Hardware Acceleration";
            this.checkHardwareAcceleration.UseVisualStyleBackColor = true;
            // 
            // btnEditCefArgs
            // 
            this.btnEditCefArgs.Location = new System.Drawing.Point(5, 3);
            this.btnEditCefArgs.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnEditCefArgs.Name = "btnEditCefArgs";
            this.btnEditCefArgs.Size = new System.Drawing.Size(144, 23);
            this.btnEditCefArgs.TabIndex = 0;
            this.btnEditCefArgs.Text = "Edit CEF Arguments";
            this.btnEditCefArgs.UseVisualStyleBackColor = true;
            // 
            // btnEditCSS
            // 
            this.btnEditCSS.Location = new System.Drawing.Point(155, 3);
            this.btnEditCSS.Name = "btnEditCSS";
            this.btnEditCSS.Size = new System.Drawing.Size(144, 23);
            this.btnEditCSS.TabIndex = 1;
            this.btnEditCSS.Text = "Edit CSS";
            this.btnEditCSS.UseVisualStyleBackColor = true;
            // 
            // btnRestartArgs
            // 
            this.btnRestartArgs.Location = new System.Drawing.Point(155, 32);
            this.btnRestartArgs.Name = "btnRestartArgs";
            this.btnRestartArgs.Size = new System.Drawing.Size(144, 23);
            this.btnRestartArgs.TabIndex = 3;
            this.btnRestartArgs.Text = "Restart with Arguments";
            this.btnRestartArgs.UseVisualStyleBackColor = true;
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(155, 3);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(144, 23);
            this.btnRestart.TabIndex = 2;
            this.btnRestart.Text = "Restart the Program";
            this.btnRestart.UseVisualStyleBackColor = true;
            // 
            // btnOpenAppFolder
            // 
            this.btnOpenAppFolder.Location = new System.Drawing.Point(5, 3);
            this.btnOpenAppFolder.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnOpenAppFolder.Name = "btnOpenAppFolder";
            this.btnOpenAppFolder.Size = new System.Drawing.Size(144, 23);
            this.btnOpenAppFolder.TabIndex = 0;
            this.btnOpenAppFolder.Text = "Open Program Folder";
            this.btnOpenAppFolder.UseVisualStyleBackColor = true;
            // 
            // btnOpenDataFolder
            // 
            this.btnOpenDataFolder.Location = new System.Drawing.Point(5, 32);
            this.btnOpenDataFolder.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnOpenDataFolder.Name = "btnOpenDataFolder";
            this.btnOpenDataFolder.Size = new System.Drawing.Size(144, 23);
            this.btnOpenDataFolder.TabIndex = 1;
            this.btnOpenDataFolder.Text = "Open Data Folder";
            this.btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // numClearCacheThreshold
            // 
            this.numClearCacheThreshold.Increment = 50;
            this.numClearCacheThreshold.Location = new System.Drawing.Point(227, 4);
            this.numClearCacheThreshold.Maximum = 1000;
            this.numClearCacheThreshold.Minimum = 100;
            this.numClearCacheThreshold.Name = "numClearCacheThreshold";
            this.numClearCacheThreshold.Size = new System.Drawing.Size(72, 20);
            this.numClearCacheThreshold.TabIndex = 1;
            this.numClearCacheThreshold.TextSuffix = " MB";
            this.numClearCacheThreshold.Value = 250;
            // 
            // checkClearCacheAuto
            // 
            this.checkClearCacheAuto.AutoSize = true;
            this.checkClearCacheAuto.Location = new System.Drawing.Point(6, 6);
            this.checkClearCacheAuto.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.checkClearCacheAuto.Name = "checkClearCacheAuto";
            this.checkClearCacheAuto.Size = new System.Drawing.Size(215, 17);
            this.checkClearCacheAuto.TabIndex = 0;
            this.checkClearCacheAuto.Text = "Clear Cache Automatically When Above";
            this.checkClearCacheAuto.UseVisualStyleBackColor = true;
            // 
            // labelApp
            // 
            this.labelApp.AutoSize = true;
            this.labelApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelApp.Location = new System.Drawing.Point(0, 0);
            this.labelApp.Margin = new System.Windows.Forms.Padding(0);
            this.labelApp.Name = "labelApp";
            this.labelApp.Size = new System.Drawing.Size(38, 20);
            this.labelApp.TabIndex = 0;
            this.labelApp.Text = "App";
            // 
            // panelAppButtons
            // 
            this.panelAppButtons.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelAppButtons.Controls.Add(this.btnOpenDataFolder);
            this.panelAppButtons.Controls.Add(this.btnOpenAppFolder);
            this.panelAppButtons.Controls.Add(this.btnRestart);
            this.panelAppButtons.Controls.Add(this.btnRestartArgs);
            this.panelAppButtons.Location = new System.Drawing.Point(0, 20);
            this.panelAppButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelAppButtons.Name = "panelAppButtons";
            this.panelAppButtons.Size = new System.Drawing.Size(322, 58);
            this.panelAppButtons.TabIndex = 1;
            // 
            // labelPerformance
            // 
            this.labelPerformance.AutoSize = true;
            this.labelPerformance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelPerformance.Location = new System.Drawing.Point(0, 98);
            this.labelPerformance.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.labelPerformance.Name = "labelPerformance";
            this.labelPerformance.Size = new System.Drawing.Size(100, 20);
            this.labelPerformance.TabIndex = 2;
            this.labelPerformance.Text = "Performance";
            // 
            // panelClearCacheAuto
            // 
            this.panelClearCacheAuto.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelClearCacheAuto.Controls.Add(this.checkClearCacheAuto);
            this.panelClearCacheAuto.Controls.Add(this.numClearCacheThreshold);
            this.panelClearCacheAuto.Location = new System.Drawing.Point(0, 198);
            this.panelClearCacheAuto.Margin = new System.Windows.Forms.Padding(0);
            this.panelClearCacheAuto.Name = "panelClearCacheAuto";
            this.panelClearCacheAuto.Size = new System.Drawing.Size(322, 26);
            this.panelClearCacheAuto.TabIndex = 6;
            // 
            // labelCache
            // 
            this.labelCache.AutoSize = true;
            this.labelCache.Location = new System.Drawing.Point(3, 156);
            this.labelCache.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelCache.Name = "labelCache";
            this.labelCache.Size = new System.Drawing.Size(38, 13);
            this.labelCache.TabIndex = 4;
            this.labelCache.Text = "Cache";
            // 
            // panelConfiguration
            // 
            this.panelConfiguration.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelConfiguration.Controls.Add(this.btnEditCSS);
            this.panelConfiguration.Controls.Add(this.btnEditCefArgs);
            this.panelConfiguration.Location = new System.Drawing.Point(0, 264);
            this.panelConfiguration.Margin = new System.Windows.Forms.Padding(0);
            this.panelConfiguration.Name = "panelConfiguration";
            this.panelConfiguration.Size = new System.Drawing.Size(322, 29);
            this.panelConfiguration.TabIndex = 8;
            // 
            // labelConfiguration
            // 
            this.labelConfiguration.AutoSize = true;
            this.labelConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelConfiguration.Location = new System.Drawing.Point(0, 244);
            this.labelConfiguration.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.labelConfiguration.Name = "labelConfiguration";
            this.labelConfiguration.Size = new System.Drawing.Size(104, 20);
            this.labelConfiguration.TabIndex = 7;
            this.labelConfiguration.Text = "Configuration";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.labelApp);
            this.flowPanel.Controls.Add(this.panelAppButtons);
            this.flowPanel.Controls.Add(this.labelPerformance);
            this.flowPanel.Controls.Add(this.checkHardwareAcceleration);
            this.flowPanel.Controls.Add(this.labelCache);
            this.flowPanel.Controls.Add(this.btnClearCache);
            this.flowPanel.Controls.Add(this.panelClearCacheAuto);
            this.flowPanel.Controls.Add(this.labelConfiguration);
            this.flowPanel.Controls.Add(this.panelConfiguration);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 295);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // TabSettingsAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsAdvanced";
            this.Size = new System.Drawing.Size(340, 313);
            ((System.ComponentModel.ISupportInitialize)(this.numClearCacheThreshold)).EndInit();
            this.panelAppButtons.ResumeLayout(false);
            this.panelClearCacheAuto.ResumeLayout(false);
            this.panelClearCacheAuto.PerformLayout();
            this.panelConfiguration.ResumeLayout(false);
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.CheckBox checkHardwareAcceleration;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnEditCefArgs;
        private System.Windows.Forms.Button btnEditCSS;
        private System.Windows.Forms.Button btnRestartArgs;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnOpenAppFolder;
        private System.Windows.Forms.Button btnOpenDataFolder;
        private System.Windows.Forms.Label labelApp;
        private System.Windows.Forms.Panel panelAppButtons;
        private System.Windows.Forms.Label labelPerformance;
        private System.Windows.Forms.Panel panelClearCacheAuto;
        private System.Windows.Forms.Panel panelConfiguration;
        private System.Windows.Forms.Label labelConfiguration;
        private System.Windows.Forms.Label labelCache;
        private Controls.NumericUpDownEx numClearCacheThreshold;
        private System.Windows.Forms.CheckBox checkClearCacheAuto;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
    }
}
