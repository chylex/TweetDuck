﻿namespace TweetDuck.Core.Other.Settings.Dialogs {
    partial class DialogSettingsAnalytics {
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
            this.textBoxReport = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxReport
            // 
            this.textBoxReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReport.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxReport.Location = new System.Drawing.Point(12, 41);
            this.textBoxReport.Multiline = true;
            this.textBoxReport.Name = "textBoxReport";
            this.textBoxReport.ReadOnly = true;
            this.textBoxReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxReport.Size = new System.Drawing.Size(460, 480);
            this.textBoxReport.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(416, 527);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnClose.Size = new System.Drawing.Size(56, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(460, 26);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "When enabled, this data will be sent over a secure network roughly once every wee" +
    "k.\r\nSome numbers in the report were made imprecise on purpose.";
            // 
            // DialogSettingsAnalytics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 562);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.textBoxReport);
            this.MinimumSize = new System.Drawing.Size(450, 340);
            this.Name = "DialogSettingsAnalytics";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxReport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelInfo;
    }
}