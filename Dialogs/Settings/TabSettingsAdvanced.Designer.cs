namespace TweetDuck.Dialogs.Settings {
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnEditCefArgs = new System.Windows.Forms.Button();
            this.btnEditCSS = new System.Windows.Forms.Button();
            this.btnRestartArgs = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnOpenAppFolder = new System.Windows.Forms.Button();
            this.btnOpenDataFolder = new System.Windows.Forms.Button();
            this.numClearCacheThreshold = new TweetDuck.Controls.NumericUpDownEx();
            this.checkClearCacheAuto = new System.Windows.Forms.CheckBox();
            this.labelApp = new System.Windows.Forms.Label();
            this.panelAppButtons = new System.Windows.Forms.Panel();
            this.labelCache = new System.Windows.Forms.Label();
            this.panelClearCacheAuto = new System.Windows.Forms.Panel();
            this.panelConfiguration = new System.Windows.Forms.Panel();
            this.labelConfiguration = new System.Windows.Forms.Label();
            this.flowPanelLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.labelBrowserSettings = new System.Windows.Forms.Label();
            this.checkHardwareAcceleration = new System.Windows.Forms.CheckBox();
            this.checkAutomaticallyDetectColorProfile = new System.Windows.Forms.CheckBox();
            this.checkTouchAdjustment = new System.Windows.Forms.CheckBox();
            this.labelProxy = new System.Windows.Forms.Label();
            this.checkUseSystemProxyForAllConnections = new System.Windows.Forms.CheckBox();
            this.labelDevTools = new System.Windows.Forms.Label();
            this.checkDevToolsInContextMenu = new System.Windows.Forms.CheckBox();
            this.checkDevToolsWindowOnTop = new System.Windows.Forms.CheckBox();
            this.flowPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            this.panelSeparator = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numClearCacheThreshold)).BeginInit();
            this.panelAppButtons.SuspendLayout();
            this.panelClearCacheAuto.SuspendLayout();
            this.panelConfiguration.SuspendLayout();
            this.flowPanelLeft.SuspendLayout();
            this.flowPanelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClearCache
            // 
            this.btnClearCache.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClearCache.Location = new System.Drawing.Point(5, 145);
            this.btnClearCache.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(143, 25);
            this.btnClearCache.TabIndex = 5;
            this.btnClearCache.Text = "Clear Cache (...)";
            this.btnClearCache.UseVisualStyleBackColor = true;
            // 
            // btnEditCefArgs
            // 
            this.btnEditCefArgs.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEditCefArgs.Location = new System.Drawing.Point(5, 3);
            this.btnEditCefArgs.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnEditCefArgs.Name = "btnEditCefArgs";
            this.btnEditCefArgs.Size = new System.Drawing.Size(143, 25);
            this.btnEditCefArgs.TabIndex = 0;
            this.btnEditCefArgs.Text = "Edit CEF Arguments";
            this.btnEditCefArgs.UseVisualStyleBackColor = true;
            // 
            // btnEditCSS
            // 
            this.btnEditCSS.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEditCSS.Location = new System.Drawing.Point(154, 3);
            this.btnEditCSS.Name = "btnEditCSS";
            this.btnEditCSS.Size = new System.Drawing.Size(143, 25);
            this.btnEditCSS.TabIndex = 1;
            this.btnEditCSS.Text = "Edit CSS";
            this.btnEditCSS.UseVisualStyleBackColor = true;
            // 
            // btnRestartArgs
            // 
            this.btnRestartArgs.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRestartArgs.Location = new System.Drawing.Point(154, 32);
            this.btnRestartArgs.Name = "btnRestartArgs";
            this.btnRestartArgs.Size = new System.Drawing.Size(143, 25);
            this.btnRestartArgs.TabIndex = 3;
            this.btnRestartArgs.Text = "Restart with Arguments";
            this.btnRestartArgs.UseVisualStyleBackColor = true;
            // 
            // btnRestart
            // 
            this.btnRestart.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRestart.Location = new System.Drawing.Point(154, 3);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(143, 25);
            this.btnRestart.TabIndex = 2;
            this.btnRestart.Text = "Restart the Program";
            this.btnRestart.UseVisualStyleBackColor = true;
            // 
            // btnOpenAppFolder
            // 
            this.btnOpenAppFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenAppFolder.Location = new System.Drawing.Point(5, 3);
            this.btnOpenAppFolder.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnOpenAppFolder.Name = "btnOpenAppFolder";
            this.btnOpenAppFolder.Size = new System.Drawing.Size(143, 25);
            this.btnOpenAppFolder.TabIndex = 0;
            this.btnOpenAppFolder.Text = "Open Program Folder";
            this.btnOpenAppFolder.UseVisualStyleBackColor = true;
            // 
            // btnOpenDataFolder
            // 
            this.btnOpenDataFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenDataFolder.Location = new System.Drawing.Point(5, 32);
            this.btnOpenDataFolder.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnOpenDataFolder.Name = "btnOpenDataFolder";
            this.btnOpenDataFolder.Size = new System.Drawing.Size(143, 25);
            this.btnOpenDataFolder.TabIndex = 1;
            this.btnOpenDataFolder.Text = "Open Data Folder";
            this.btnOpenDataFolder.UseVisualStyleBackColor = true;
            // 
            // numClearCacheThreshold
            // 
            this.numClearCacheThreshold.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numClearCacheThreshold.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numClearCacheThreshold.Location = new System.Drawing.Point(210, 5);
            this.numClearCacheThreshold.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numClearCacheThreshold.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numClearCacheThreshold.Name = "numClearCacheThreshold";
            this.numClearCacheThreshold.Size = new System.Drawing.Size(80, 23);
            this.numClearCacheThreshold.TabIndex = 1;
            this.numClearCacheThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numClearCacheThreshold.TextSuffix = " MB";
            this.numClearCacheThreshold.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // checkClearCacheAuto
            // 
            this.checkClearCacheAuto.AutoSize = true;
            this.checkClearCacheAuto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkClearCacheAuto.Location = new System.Drawing.Point(6, 6);
            this.checkClearCacheAuto.Margin = new System.Windows.Forms.Padding(6, 6, 0, 2);
            this.checkClearCacheAuto.Name = "checkClearCacheAuto";
            this.checkClearCacheAuto.Size = new System.Drawing.Size(201, 19);
            this.checkClearCacheAuto.TabIndex = 0;
            this.checkClearCacheAuto.Text = "Clear Automatically When Above";
            this.checkClearCacheAuto.UseVisualStyleBackColor = true;
            // 
            // labelApp
            // 
            this.labelApp.AutoSize = true;
            this.labelApp.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelApp.Location = new System.Drawing.Point(0, 0);
            this.labelApp.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.labelApp.Name = "labelApp";
            this.labelApp.Size = new System.Drawing.Size(97, 19);
            this.labelApp.TabIndex = 0;
            this.labelApp.Text = "APPLICATION";
            // 
            // panelAppButtons
            // 
            this.panelAppButtons.Controls.Add(this.btnOpenDataFolder);
            this.panelAppButtons.Controls.Add(this.btnOpenAppFolder);
            this.panelAppButtons.Controls.Add(this.btnRestart);
            this.panelAppButtons.Controls.Add(this.btnRestartArgs);
            this.panelAppButtons.Location = new System.Drawing.Point(0, 20);
            this.panelAppButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelAppButtons.Name = "panelAppButtons";
            this.panelAppButtons.Size = new System.Drawing.Size(300, 62);
            this.panelAppButtons.TabIndex = 1;
            // 
            // labelCache
            // 
            this.labelCache.AutoSize = true;
            this.labelCache.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelCache.Location = new System.Drawing.Point(0, 122);
            this.labelCache.Margin = new System.Windows.Forms.Padding(0, 30, 0, 1);
            this.labelCache.Name = "labelCache";
            this.labelCache.Size = new System.Drawing.Size(123, 19);
            this.labelCache.TabIndex = 4;
            this.labelCache.Text = "BROWSER CACHE";
            // 
            // panelClearCacheAuto
            // 
            this.panelClearCacheAuto.Controls.Add(this.checkClearCacheAuto);
            this.panelClearCacheAuto.Controls.Add(this.numClearCacheThreshold);
            this.panelClearCacheAuto.Location = new System.Drawing.Point(0, 173);
            this.panelClearCacheAuto.Margin = new System.Windows.Forms.Padding(0);
            this.panelClearCacheAuto.Name = "panelClearCacheAuto";
            this.panelClearCacheAuto.Size = new System.Drawing.Size(300, 28);
            this.panelClearCacheAuto.TabIndex = 6;
            // 
            // panelConfiguration
            // 
            this.panelConfiguration.Controls.Add(this.btnEditCSS);
            this.panelConfiguration.Controls.Add(this.btnEditCefArgs);
            this.panelConfiguration.Location = new System.Drawing.Point(0, 132);
            this.panelConfiguration.Margin = new System.Windows.Forms.Padding(0);
            this.panelConfiguration.Name = "panelConfiguration";
            this.panelConfiguration.Size = new System.Drawing.Size(300, 31);
            this.panelConfiguration.TabIndex = 3;
            // 
            // labelConfiguration
            // 
            this.labelConfiguration.AutoSize = true;
            this.labelConfiguration.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelConfiguration.Location = new System.Drawing.Point(0, 112);
            this.labelConfiguration.Margin = new System.Windows.Forms.Padding(0, 30, 0, 1);
            this.labelConfiguration.Name = "labelConfiguration";
            this.labelConfiguration.Size = new System.Drawing.Size(123, 19);
            this.labelConfiguration.TabIndex = 2;
            this.labelConfiguration.Text = "CONFIGURATION";
            // 
            // flowPanelLeft
            // 
            this.flowPanelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPanelLeft.Controls.Add(this.labelBrowserSettings);
            this.flowPanelLeft.Controls.Add(this.checkHardwareAcceleration);
            this.flowPanelLeft.Controls.Add(this.checkAutomaticallyDetectColorProfile);
            this.flowPanelLeft.Controls.Add(this.checkTouchAdjustment);
            this.flowPanelLeft.Controls.Add(this.labelCache);
            this.flowPanelLeft.Controls.Add(this.btnClearCache);
            this.flowPanelLeft.Controls.Add(this.panelClearCacheAuto);
            this.flowPanelLeft.Controls.Add(this.labelProxy);
            this.flowPanelLeft.Controls.Add(this.checkUseSystemProxyForAllConnections);
            this.flowPanelLeft.Controls.Add(this.labelDevTools);
            this.flowPanelLeft.Controls.Add(this.checkDevToolsInContextMenu);
            this.flowPanelLeft.Controls.Add(this.checkDevToolsWindowOnTop);
            this.flowPanelLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelLeft.Location = new System.Drawing.Point(9, 9);
            this.flowPanelLeft.Name = "flowPanelLeft";
            this.flowPanelLeft.Size = new System.Drawing.Size(300, 462);
            this.flowPanelLeft.TabIndex = 0;
            this.flowPanelLeft.WrapContents = false;
            // 
            // labelBrowserSettings
            // 
            this.labelBrowserSettings.AutoSize = true;
            this.labelBrowserSettings.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelBrowserSettings.Location = new System.Drawing.Point(0, 0);
            this.labelBrowserSettings.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.labelBrowserSettings.Name = "labelBrowserSettings";
            this.labelBrowserSettings.Size = new System.Drawing.Size(143, 19);
            this.labelBrowserSettings.TabIndex = 0;
            this.labelBrowserSettings.Text = "BROWSER SETTINGS";
            // 
            // checkHardwareAcceleration
            // 
            this.checkHardwareAcceleration.AutoSize = true;
            this.checkHardwareAcceleration.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkHardwareAcceleration.Location = new System.Drawing.Point(6, 23);
            this.checkHardwareAcceleration.Margin = new System.Windows.Forms.Padding(6, 3, 3, 2);
            this.checkHardwareAcceleration.Name = "checkHardwareAcceleration";
            this.checkHardwareAcceleration.Size = new System.Drawing.Size(146, 19);
            this.checkHardwareAcceleration.TabIndex = 1;
            this.checkHardwareAcceleration.Text = "Hardware Acceleration";
            this.checkHardwareAcceleration.UseVisualStyleBackColor = true;
            // 
            // checkAutomaticallyDetectColorProfile
            // 
            this.checkAutomaticallyDetectColorProfile.AutoSize = true;
            this.checkAutomaticallyDetectColorProfile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkAutomaticallyDetectColorProfile.Location = new System.Drawing.Point(6, 47);
            this.checkAutomaticallyDetectColorProfile.Margin = new System.Windows.Forms.Padding(6, 3, 3, 2);
            this.checkAutomaticallyDetectColorProfile.Name = "checkAutomaticallyDetectColorProfile";
            this.checkAutomaticallyDetectColorProfile.Size = new System.Drawing.Size(206, 19);
            this.checkAutomaticallyDetectColorProfile.TabIndex = 2;
            this.checkAutomaticallyDetectColorProfile.Text = "Automatically Detect Color Profile";
            this.checkAutomaticallyDetectColorProfile.UseVisualStyleBackColor = true;
            // 
            // checkTouchAdjustment
            // 
            this.checkTouchAdjustment.AutoSize = true;
            this.checkTouchAdjustment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkTouchAdjustment.Location = new System.Drawing.Point(6, 71);
            this.checkTouchAdjustment.Margin = new System.Windows.Forms.Padding(6, 3, 3, 2);
            this.checkTouchAdjustment.Name = "checkTouchAdjustment";
            this.checkTouchAdjustment.Size = new System.Drawing.Size(163, 19);
            this.checkTouchAdjustment.TabIndex = 3;
            this.checkTouchAdjustment.Text = "Touch Screen Adjustment";
            this.checkTouchAdjustment.UseVisualStyleBackColor = true;
            // 
            // labelProxy
            // 
            this.labelProxy.AutoSize = true;
            this.labelProxy.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelProxy.Location = new System.Drawing.Point(0, 231);
            this.labelProxy.Margin = new System.Windows.Forms.Padding(0, 30, 0, 1);
            this.labelProxy.Name = "labelProxy";
            this.labelProxy.Size = new System.Drawing.Size(54, 19);
            this.labelProxy.TabIndex = 7;
            this.labelProxy.Text = "PROXY";
            // 
            // checkUseSystemProxyForAllConnections
            // 
            this.checkUseSystemProxyForAllConnections.AutoSize = true;
            this.checkUseSystemProxyForAllConnections.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkUseSystemProxyForAllConnections.Location = new System.Drawing.Point(6, 257);
            this.checkUseSystemProxyForAllConnections.Margin = new System.Windows.Forms.Padding(6, 6, 3, 2);
            this.checkUseSystemProxyForAllConnections.Name = "checkUseSystemProxyForAllConnections";
            this.checkUseSystemProxyForAllConnections.Size = new System.Drawing.Size(223, 19);
            this.checkUseSystemProxyForAllConnections.TabIndex = 8;
            this.checkUseSystemProxyForAllConnections.Text = "Use System Proxy for All Connections";
            this.checkUseSystemProxyForAllConnections.UseVisualStyleBackColor = true;
            // 
            // labelDevTools
            // 
            this.labelDevTools.AutoSize = true;
            this.labelDevTools.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelDevTools.Location = new System.Drawing.Point(0, 308);
            this.labelDevTools.Margin = new System.Windows.Forms.Padding(0, 30, 0, 1);
            this.labelDevTools.Name = "labelDevTools";
            this.labelDevTools.Size = new System.Drawing.Size(156, 19);
            this.labelDevTools.TabIndex = 9;
            this.labelDevTools.Text = "DEVELOPMENT TOOLS";
            // 
            // checkDevToolsInContextMenu
            // 
            this.checkDevToolsInContextMenu.AutoSize = true;
            this.checkDevToolsInContextMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkDevToolsInContextMenu.Location = new System.Drawing.Point(6, 334);
            this.checkDevToolsInContextMenu.Margin = new System.Windows.Forms.Padding(6, 6, 3, 2);
            this.checkDevToolsInContextMenu.Name = "checkDevToolsInContextMenu";
            this.checkDevToolsInContextMenu.Size = new System.Drawing.Size(201, 19);
            this.checkDevToolsInContextMenu.TabIndex = 10;
            this.checkDevToolsInContextMenu.Text = "Show Dev Tools in Context Menu";
            this.checkDevToolsInContextMenu.UseVisualStyleBackColor = true;
            // 
            // checkDevToolsWindowOnTop
            // 
            this.checkDevToolsWindowOnTop.AutoSize = true;
            this.checkDevToolsWindowOnTop.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkDevToolsWindowOnTop.Location = new System.Drawing.Point(6, 358);
            this.checkDevToolsWindowOnTop.Margin = new System.Windows.Forms.Padding(6, 3, 3, 2);
            this.checkDevToolsWindowOnTop.Name = "checkDevToolsWindowOnTop";
            this.checkDevToolsWindowOnTop.Size = new System.Drawing.Size(168, 19);
            this.checkDevToolsWindowOnTop.TabIndex = 11;
            this.checkDevToolsWindowOnTop.Text = "Dev Tools Window On Top";
            this.checkDevToolsWindowOnTop.UseVisualStyleBackColor = true;
            // 
            // flowPanelRight
            // 
            this.flowPanelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPanelRight.Controls.Add(this.labelApp);
            this.flowPanelRight.Controls.Add(this.panelAppButtons);
            this.flowPanelRight.Controls.Add(this.labelConfiguration);
            this.flowPanelRight.Controls.Add(this.panelConfiguration);
            this.flowPanelRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelRight.Location = new System.Drawing.Point(322, 9);
            this.flowPanelRight.Name = "flowPanelRight";
            this.flowPanelRight.Size = new System.Drawing.Size(300, 462);
            this.flowPanelRight.TabIndex = 1;
            this.flowPanelRight.WrapContents = false;
            // 
            // panelSeparator
            // 
            this.panelSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.panelSeparator.Location = new System.Drawing.Point(312, 0);
            this.panelSeparator.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.panelSeparator.Name = "panelSeparator";
            this.panelSeparator.Size = new System.Drawing.Size(1, 480);
            this.panelSeparator.TabIndex = 3;
            // 
            // TabSettingsAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelSeparator);
            this.Controls.Add(this.flowPanelRight);
            this.Controls.Add(this.flowPanelLeft);
            this.Name = "TabSettingsAdvanced";
            this.Size = new System.Drawing.Size(631, 480);
            ((System.ComponentModel.ISupportInitialize)(this.numClearCacheThreshold)).EndInit();
            this.panelAppButtons.ResumeLayout(false);
            this.panelClearCacheAuto.ResumeLayout(false);
            this.panelClearCacheAuto.PerformLayout();
            this.panelConfiguration.ResumeLayout(false);
            this.flowPanelLeft.ResumeLayout(false);
            this.flowPanelLeft.PerformLayout();
            this.flowPanelRight.ResumeLayout(false);
            this.flowPanelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnEditCefArgs;
        private System.Windows.Forms.Button btnEditCSS;
        private System.Windows.Forms.Button btnRestartArgs;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnOpenAppFolder;
        private System.Windows.Forms.Button btnOpenDataFolder;
        private System.Windows.Forms.Label labelApp;
        private System.Windows.Forms.Panel panelAppButtons;
        private System.Windows.Forms.Label labelCache;
        private System.Windows.Forms.Panel panelClearCacheAuto;
        private System.Windows.Forms.Panel panelConfiguration;
        private System.Windows.Forms.Label labelConfiguration;
        private Controls.NumericUpDownEx numClearCacheThreshold;
        private System.Windows.Forms.CheckBox checkClearCacheAuto;
        private System.Windows.Forms.FlowLayoutPanel flowPanelLeft;
        private System.Windows.Forms.Label labelDevTools;
        private System.Windows.Forms.CheckBox checkDevToolsInContextMenu;
        private System.Windows.Forms.CheckBox checkDevToolsWindowOnTop;
        private System.Windows.Forms.Label labelProxy;
        private System.Windows.Forms.CheckBox checkUseSystemProxyForAllConnections;
        private System.Windows.Forms.FlowLayoutPanel flowPanelRight;
        private System.Windows.Forms.Label labelBrowserSettings;
        private System.Windows.Forms.CheckBox checkTouchAdjustment;
        private System.Windows.Forms.CheckBox checkAutomaticallyDetectColorProfile;
        private System.Windows.Forms.CheckBox checkHardwareAcceleration;
        private System.Windows.Forms.Panel panelSeparator;
    }
}
