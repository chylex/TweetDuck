namespace TweetDuck.Core.Other.Settings.Dialogs {
    partial class DialogSettingsCSS {
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
            this.textBoxBrowserCSS = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.labelBrowser = new System.Windows.Forms.Label();
            this.labelNotification = new System.Windows.Forms.Label();
            this.textBoxNotificationCSS = new System.Windows.Forms.TextBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.btnOpenWiki = new System.Windows.Forms.Button();
            this.timerTestBrowser = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxBrowserCSS
            // 
            this.textBoxBrowserCSS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBrowserCSS.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxBrowserCSS.Location = new System.Drawing.Point(0, 16);
            this.textBoxBrowserCSS.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.textBoxBrowserCSS.Multiline = true;
            this.textBoxBrowserCSS.Name = "textBoxBrowserCSS";
            this.textBoxBrowserCSS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxBrowserCSS.Size = new System.Drawing.Size(378, 251);
            this.textBoxBrowserCSS.TabIndex = 1;
            this.textBoxBrowserCSS.WordWrap = false;
            this.textBoxBrowserCSS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxBrowserCSS_KeyUp);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(657, 285);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnCancel.Size = new System.Drawing.Size(57, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.AutoSize = true;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(720, 285);
            this.btnApply.Name = "btnApply";
            this.btnApply.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnApply.Size = new System.Drawing.Size(52, 25);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.labelBrowser);
            this.splitContainer.Panel1.Controls.Add(this.textBoxBrowserCSS);
            this.splitContainer.Panel1MinSize = 64;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.labelNotification);
            this.splitContainer.Panel2.Controls.Add(this.textBoxNotificationCSS);
            this.splitContainer.Panel2MinSize = 64;
            this.splitContainer.Size = new System.Drawing.Size(760, 267);
            this.splitContainer.SplitterDistance = 378;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 0;
            // 
            // labelBrowser
            // 
            this.labelBrowser.AutoSize = true;
            this.labelBrowser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBrowser.Location = new System.Drawing.Point(-3, 0);
            this.labelBrowser.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelBrowser.Name = "labelBrowser";
            this.labelBrowser.Size = new System.Drawing.Size(49, 15);
            this.labelBrowser.TabIndex = 0;
            this.labelBrowser.Text = "Browser";
            // 
            // labelNotification
            // 
            this.labelNotification.AutoSize = true;
            this.labelNotification.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotification.Location = new System.Drawing.Point(-3, 0);
            this.labelNotification.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelNotification.Name = "labelNotification";
            this.labelNotification.Size = new System.Drawing.Size(70, 15);
            this.labelNotification.TabIndex = 0;
            this.labelNotification.Text = "Notification";
            // 
            // textBoxNotificationCSS
            // 
            this.textBoxNotificationCSS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNotificationCSS.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxNotificationCSS.Location = new System.Drawing.Point(0, 16);
            this.textBoxNotificationCSS.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.textBoxNotificationCSS.Multiline = true;
            this.textBoxNotificationCSS.Name = "textBoxNotificationCSS";
            this.textBoxNotificationCSS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxNotificationCSS.Size = new System.Drawing.Size(372, 251);
            this.textBoxNotificationCSS.TabIndex = 1;
            this.textBoxNotificationCSS.WordWrap = false;
            // 
            // labelWarning
            // 
            this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelWarning.AutoSize = true;
            this.labelWarning.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWarning.Location = new System.Drawing.Point(94, 290);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(373, 15);
            this.labelWarning.TabIndex = 3;
            this.labelWarning.Text = "The code is not validated, please make sure there are no syntax errors.";
            // 
            // btnOpenWiki
            // 
            this.btnOpenWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenWiki.AutoSize = true;
            this.btnOpenWiki.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenWiki.Location = new System.Drawing.Point(12, 285);
            this.btnOpenWiki.Name = "btnOpenWiki";
            this.btnOpenWiki.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnOpenWiki.Size = new System.Drawing.Size(76, 25);
            this.btnOpenWiki.TabIndex = 4;
            this.btnOpenWiki.Text = "Open Wiki";
            this.btnOpenWiki.UseVisualStyleBackColor = true;
            this.btnOpenWiki.Click += new System.EventHandler(this.btnOpenWiki_Click);
            // 
            // timerTestBrowser
            // 
            this.timerTestBrowser.Interval = 500;
            this.timerTestBrowser.Tick += new System.EventHandler(this.timerTestBrowser_Tick);
            // 
            // DialogSettingsCSS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 322);
            this.Controls.Add(this.btnOpenWiki);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new System.Drawing.Size(620, 160);
            this.Name = "DialogSettingsCSS";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBrowserCSS;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxNotificationCSS;
        private System.Windows.Forms.Label labelBrowser;
        private System.Windows.Forms.Label labelNotification;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Button btnOpenWiki;
        private System.Windows.Forms.Timer timerTestBrowser;
    }
}