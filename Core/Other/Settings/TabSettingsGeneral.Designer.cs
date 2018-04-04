﻿namespace TweetDuck.Core.Other.Settings {
    partial class TabSettingsGeneral {
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
            this.checkExpandLinks = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkUpdateNotifications = new System.Windows.Forms.CheckBox();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.labelZoomValue = new System.Windows.Forms.Label();
            this.checkBestImageQuality = new System.Windows.Forms.CheckBox();
            this.checkOpenSearchInFirstColumn = new System.Windows.Forms.CheckBox();
            this.trackBarZoom = new System.Windows.Forms.TrackBar();
            this.labelZoom = new System.Windows.Forms.Label();
            this.zoomUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.labelUI = new System.Windows.Forms.Label();
            this.panelZoom = new System.Windows.Forms.Panel();
            this.checkAnimatedAvatars = new System.Windows.Forms.CheckBox();
            this.labelUpdates = new System.Windows.Forms.Label();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.checkKeepLikeFollowDialogsOpen = new System.Windows.Forms.CheckBox();
            this.labelBrowserSettings = new System.Windows.Forms.Label();
            this.checkSmoothScrolling = new System.Windows.Forms.CheckBox();
            this.labelBrowserPath = new System.Windows.Forms.Label();
            this.comboBoxBrowserPath = new System.Windows.Forms.ComboBox();
            this.labelSearchEngine = new System.Windows.Forms.Label();
            this.comboBoxSearchEngine = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).BeginInit();
            this.panelZoom.SuspendLayout();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(6, 26);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 1;
            this.checkExpandLinks.Text = "Expand Links When Hovered";
            this.checkExpandLinks.UseVisualStyleBackColor = true;
            // 
            // checkUpdateNotifications
            // 
            this.checkUpdateNotifications.AutoSize = true;
            this.checkUpdateNotifications.Location = new System.Drawing.Point(6, 415);
            this.checkUpdateNotifications.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.checkUpdateNotifications.Name = "checkUpdateNotifications";
            this.checkUpdateNotifications.Size = new System.Drawing.Size(165, 17);
            this.checkUpdateNotifications.TabIndex = 15;
            this.checkUpdateNotifications.Text = "Check Updates Automatically";
            this.checkUpdateNotifications.UseVisualStyleBackColor = true;
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(5, 438);
            this.btnCheckUpdates.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(144, 23);
            this.btnCheckUpdates.TabIndex = 16;
            this.btnCheckUpdates.Text = "Check Updates Now";
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // labelZoomValue
            // 
            this.labelZoomValue.BackColor = System.Drawing.Color.Transparent;
            this.labelZoomValue.Location = new System.Drawing.Point(147, 4);
            this.labelZoomValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelZoomValue.Name = "labelZoomValue";
            this.labelZoomValue.Size = new System.Drawing.Size(38, 13);
            this.labelZoomValue.TabIndex = 1;
            this.labelZoomValue.Text = "100%";
            this.labelZoomValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // checkBestImageQuality
            // 
            this.checkBestImageQuality.AutoSize = true;
            this.checkBestImageQuality.Location = new System.Drawing.Point(6, 95);
            this.checkBestImageQuality.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkBestImageQuality.Name = "checkBestImageQuality";
            this.checkBestImageQuality.Size = new System.Drawing.Size(114, 17);
            this.checkBestImageQuality.TabIndex = 4;
            this.checkBestImageQuality.Text = "Best Image Quality";
            this.checkBestImageQuality.UseVisualStyleBackColor = true;
            // 
            // checkOpenSearchInFirstColumn
            // 
            this.checkOpenSearchInFirstColumn.AutoSize = true;
            this.checkOpenSearchInFirstColumn.Location = new System.Drawing.Point(6, 49);
            this.checkOpenSearchInFirstColumn.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkOpenSearchInFirstColumn.Name = "checkOpenSearchInFirstColumn";
            this.checkOpenSearchInFirstColumn.Size = new System.Drawing.Size(219, 17);
            this.checkOpenSearchInFirstColumn.TabIndex = 2;
            this.checkOpenSearchInFirstColumn.Text = "Add Search Columns Before First Column";
            this.checkOpenSearchInFirstColumn.UseVisualStyleBackColor = true;
            // 
            // trackBarZoom
            // 
            this.trackBarZoom.AutoSize = false;
            this.trackBarZoom.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarZoom.LargeChange = 25;
            this.trackBarZoom.Location = new System.Drawing.Point(3, 3);
            this.trackBarZoom.Maximum = 200;
            this.trackBarZoom.Minimum = 50;
            this.trackBarZoom.Name = "trackBarZoom";
            this.trackBarZoom.Size = new System.Drawing.Size(148, 30);
            this.trackBarZoom.SmallChange = 5;
            this.trackBarZoom.TabIndex = 0;
            this.trackBarZoom.TickFrequency = 25;
            this.trackBarZoom.Value = 100;
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(3, 320);
            this.labelZoom.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 12;
            this.labelZoom.Text = "Zoom";
            // 
            // zoomUpdateTimer
            // 
            this.zoomUpdateTimer.Interval = 250;
            this.zoomUpdateTimer.Tick += new System.EventHandler(this.zoomUpdateTimer_Tick);
            // 
            // labelUI
            // 
            this.labelUI.AutoSize = true;
            this.labelUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUI.Location = new System.Drawing.Point(0, 0);
            this.labelUI.Margin = new System.Windows.Forms.Padding(0);
            this.labelUI.Name = "labelUI";
            this.labelUI.Size = new System.Drawing.Size(111, 20);
            this.labelUI.TabIndex = 0;
            this.labelUI.Text = "User Interface";
            // 
            // panelZoom
            // 
            this.panelZoom.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelZoom.Controls.Add(this.trackBarZoom);
            this.panelZoom.Controls.Add(this.labelZoomValue);
            this.panelZoom.Location = new System.Drawing.Point(0, 333);
            this.panelZoom.Margin = new System.Windows.Forms.Padding(0);
            this.panelZoom.Name = "panelZoom";
            this.panelZoom.Size = new System.Drawing.Size(322, 36);
            this.panelZoom.TabIndex = 13;
            // 
            // checkAnimatedAvatars
            // 
            this.checkAnimatedAvatars.AutoSize = true;
            this.checkAnimatedAvatars.Location = new System.Drawing.Point(6, 118);
            this.checkAnimatedAvatars.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkAnimatedAvatars.Name = "checkAnimatedAvatars";
            this.checkAnimatedAvatars.Size = new System.Drawing.Size(145, 17);
            this.checkAnimatedAvatars.TabIndex = 5;
            this.checkAnimatedAvatars.Text = "Enable Animated Avatars";
            this.checkAnimatedAvatars.UseVisualStyleBackColor = true;
            // 
            // labelUpdates
            // 
            this.labelUpdates.AutoSize = true;
            this.labelUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUpdates.Location = new System.Drawing.Point(0, 389);
            this.labelUpdates.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.labelUpdates.Name = "labelUpdates";
            this.labelUpdates.Size = new System.Drawing.Size(70, 20);
            this.labelUpdates.TabIndex = 14;
            this.labelUpdates.Text = "Updates";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.labelUI);
            this.flowPanel.Controls.Add(this.checkExpandLinks);
            this.flowPanel.Controls.Add(this.checkOpenSearchInFirstColumn);
            this.flowPanel.Controls.Add(this.checkKeepLikeFollowDialogsOpen);
            this.flowPanel.Controls.Add(this.checkBestImageQuality);
            this.flowPanel.Controls.Add(this.checkAnimatedAvatars);
            this.flowPanel.Controls.Add(this.labelBrowserSettings);
            this.flowPanel.Controls.Add(this.checkSmoothScrolling);
            this.flowPanel.Controls.Add(this.labelBrowserPath);
            this.flowPanel.Controls.Add(this.comboBoxBrowserPath);
            this.flowPanel.Controls.Add(this.labelSearchEngine);
            this.flowPanel.Controls.Add(this.comboBoxSearchEngine);
            this.flowPanel.Controls.Add(this.labelZoom);
            this.flowPanel.Controls.Add(this.panelZoom);
            this.flowPanel.Controls.Add(this.labelUpdates);
            this.flowPanel.Controls.Add(this.checkUpdateNotifications);
            this.flowPanel.Controls.Add(this.btnCheckUpdates);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 462);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // checkKeepLikeFollowDialogsOpen
            // 
            this.checkKeepLikeFollowDialogsOpen.AutoSize = true;
            this.checkKeepLikeFollowDialogsOpen.Location = new System.Drawing.Point(6, 72);
            this.checkKeepLikeFollowDialogsOpen.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.checkKeepLikeFollowDialogsOpen.Name = "checkKeepLikeFollowDialogsOpen";
            this.checkKeepLikeFollowDialogsOpen.Size = new System.Drawing.Size(176, 17);
            this.checkKeepLikeFollowDialogsOpen.TabIndex = 3;
            this.checkKeepLikeFollowDialogsOpen.Text = "Keep Like/Follow Dialogs Open";
            this.checkKeepLikeFollowDialogsOpen.UseVisualStyleBackColor = true;
            // 
            // labelBrowserSettings
            // 
            this.labelBrowserSettings.AutoSize = true;
            this.labelBrowserSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelBrowserSettings.Location = new System.Drawing.Point(0, 158);
            this.labelBrowserSettings.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.labelBrowserSettings.Name = "labelBrowserSettings";
            this.labelBrowserSettings.Size = new System.Drawing.Size(130, 20);
            this.labelBrowserSettings.TabIndex = 6;
            this.labelBrowserSettings.Text = "Browser Settings";
            // 
            // checkSmoothScrolling
            // 
            this.checkSmoothScrolling.AutoSize = true;
            this.checkSmoothScrolling.Location = new System.Drawing.Point(6, 184);
            this.checkSmoothScrolling.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.checkSmoothScrolling.Name = "checkSmoothScrolling";
            this.checkSmoothScrolling.Size = new System.Drawing.Size(105, 17);
            this.checkSmoothScrolling.TabIndex = 7;
            this.checkSmoothScrolling.Text = "Smooth Scrolling";
            this.checkSmoothScrolling.UseVisualStyleBackColor = true;
            // 
            // labelBrowserPath
            // 
            this.labelBrowserPath.AutoSize = true;
            this.labelBrowserPath.Location = new System.Drawing.Point(3, 216);
            this.labelBrowserPath.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelBrowserPath.Name = "labelBrowserPath";
            this.labelBrowserPath.Size = new System.Drawing.Size(95, 13);
            this.labelBrowserPath.TabIndex = 8;
            this.labelBrowserPath.Text = "Open Links With...";
            // 
            // comboBoxBrowserPath
            // 
            this.comboBoxBrowserPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBrowserPath.FormattingEnabled = true;
            this.comboBoxBrowserPath.Location = new System.Drawing.Point(5, 232);
            this.comboBoxBrowserPath.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxBrowserPath.Name = "comboBoxBrowserPath";
            this.comboBoxBrowserPath.Size = new System.Drawing.Size(173, 21);
            this.comboBoxBrowserPath.TabIndex = 9;
            // 
            // labelSearchEngine
            // 
            this.labelSearchEngine.AutoSize = true;
            this.labelSearchEngine.Location = new System.Drawing.Point(3, 268);
            this.labelSearchEngine.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelSearchEngine.Name = "labelSearchEngine";
            this.labelSearchEngine.Size = new System.Drawing.Size(77, 13);
            this.labelSearchEngine.TabIndex = 10;
            this.labelSearchEngine.Text = "Search Engine";
            // 
            // comboBoxSearchEngine
            // 
            this.comboBoxSearchEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSearchEngine.FormattingEnabled = true;
            this.comboBoxSearchEngine.Location = new System.Drawing.Point(5, 284);
            this.comboBoxSearchEngine.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxSearchEngine.Name = "comboBoxSearchEngine";
            this.comboBoxSearchEngine.Size = new System.Drawing.Size(173, 21);
            this.comboBoxSearchEngine.TabIndex = 11;
            // 
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(340, 480);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).EndInit();
            this.panelZoom.ResumeLayout(false);
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkExpandLinks;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkUpdateNotifications;
        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.Label labelZoomValue;
        private System.Windows.Forms.TrackBar trackBarZoom;
        private System.Windows.Forms.Timer zoomUpdateTimer;
        private System.Windows.Forms.Label labelUI;
        private System.Windows.Forms.Panel panelZoom;
        private System.Windows.Forms.Label labelUpdates;
        private System.Windows.Forms.CheckBox checkBestImageQuality;
        private System.Windows.Forms.CheckBox checkOpenSearchInFirstColumn;
        private System.Windows.Forms.CheckBox checkAnimatedAvatars;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.CheckBox checkKeepLikeFollowDialogsOpen;
        private System.Windows.Forms.Label labelBrowserPath;
        private System.Windows.Forms.ComboBox comboBoxBrowserPath;
        private System.Windows.Forms.Label labelBrowserSettings;
        private System.Windows.Forms.CheckBox checkSmoothScrolling;
        private System.Windows.Forms.Label labelSearchEngine;
        private System.Windows.Forms.ComboBox comboBoxSearchEngine;
    }
}
