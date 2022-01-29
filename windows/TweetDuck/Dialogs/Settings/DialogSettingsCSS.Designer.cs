namespace TweetDuck.Dialogs.Settings {
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
            this.textBoxNotificationCSS = new System.Windows.Forms.TextBox();
            this.btnOpenDevTools = new System.Windows.Forms.Button();
            this.timerTestBrowser = new System.Windows.Forms.Timer(this.components);
            this.tabPanel = new System.Windows.Forms.TabControl();
            this.tabPageBrowser = new System.Windows.Forms.TabPage();
            this.tabPageNotification = new System.Windows.Forms.TabPage();
            this.tabPanel.SuspendLayout();
            this.tabPageBrowser.SuspendLayout();
            this.tabPageNotification.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxBrowserCSS
            // 
            this.textBoxBrowserCSS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBrowserCSS.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular);
            this.textBoxBrowserCSS.Location = new System.Drawing.Point(3, 3);
            this.textBoxBrowserCSS.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.textBoxBrowserCSS.Multiline = true;
            this.textBoxBrowserCSS.Name = "textBoxBrowserCSS";
            this.textBoxBrowserCSS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxBrowserCSS.Size = new System.Drawing.Size(426, 332);
            this.textBoxBrowserCSS.TabIndex = 0;
            this.textBoxBrowserCSS.WordWrap = false;
            this.textBoxBrowserCSS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCSS_KeyDown);
            this.textBoxBrowserCSS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxBrowserCSS_KeyUp);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(337, 384);
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
            this.btnApply.Location = new System.Drawing.Point(400, 384);
            this.btnApply.Name = "btnApply";
            this.btnApply.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnApply.Size = new System.Drawing.Size(52, 25);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // textBoxNotificationCSS
            // 
            this.textBoxNotificationCSS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxNotificationCSS.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular);
            this.textBoxNotificationCSS.Location = new System.Drawing.Point(3, 3);
            this.textBoxNotificationCSS.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.textBoxNotificationCSS.Multiline = true;
            this.textBoxNotificationCSS.Name = "textBoxNotificationCSS";
            this.textBoxNotificationCSS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxNotificationCSS.Size = new System.Drawing.Size(426, 332);
            this.textBoxNotificationCSS.TabIndex = 0;
            this.textBoxNotificationCSS.WordWrap = false;
            this.textBoxNotificationCSS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCSS_KeyDown);
            // 
            // btnOpenDevTools
            // 
            this.btnOpenDevTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenDevTools.AutoSize = true;
            this.btnOpenDevTools.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenDevTools.Location = new System.Drawing.Point(12, 384);
            this.btnOpenDevTools.Name = "btnOpenDevTools";
            this.btnOpenDevTools.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnOpenDevTools.Size = new System.Drawing.Size(104, 25);
            this.btnOpenDevTools.TabIndex = 3;
            this.btnOpenDevTools.Text = "Open Dev Tools";
            this.btnOpenDevTools.UseVisualStyleBackColor = true;
            this.btnOpenDevTools.Click += new System.EventHandler(this.btnOpenDevTools_Click);
            // 
            // timerTestBrowser
            // 
            this.timerTestBrowser.Interval = 400;
            this.timerTestBrowser.Tick += new System.EventHandler(this.timerTestBrowser_Tick);
            // 
            // tabPanel
            // 
            this.tabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPanel.Controls.Add(this.tabPageBrowser);
            this.tabPanel.Controls.Add(this.tabPageNotification);
            this.tabPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.tabPanel.Location = new System.Drawing.Point(12, 12);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(440, 366);
            this.tabPanel.TabIndex = 0;
            this.tabPanel.SelectedIndexChanged += new System.EventHandler(this.tabPanel_SelectedIndexChanged);
            // 
            // tabPageBrowser
            // 
            this.tabPageBrowser.Controls.Add(this.textBoxBrowserCSS);
            this.tabPageBrowser.Location = new System.Drawing.Point(4, 24);
            this.tabPageBrowser.Name = "tabPageBrowser";
            this.tabPageBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBrowser.Size = new System.Drawing.Size(432, 338);
            this.tabPageBrowser.TabIndex = 0;
            this.tabPageBrowser.Text = "Browser";
            this.tabPageBrowser.UseVisualStyleBackColor = true;
            // 
            // tabPageNotification
            // 
            this.tabPageNotification.Controls.Add(this.textBoxNotificationCSS);
            this.tabPageNotification.Location = new System.Drawing.Point(4, 24);
            this.tabPageNotification.Name = "tabPageNotification";
            this.tabPageNotification.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNotification.Size = new System.Drawing.Size(432, 338);
            this.tabPageNotification.TabIndex = 1;
            this.tabPageNotification.Text = "Notification";
            this.tabPageNotification.UseVisualStyleBackColor = true;
            // 
            // DialogSettingsCSS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 421);
            this.Controls.Add(this.tabPanel);
            this.Controls.Add(this.btnOpenDevTools);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new System.Drawing.Size(320, 240);
            this.Name = "DialogSettingsCSS";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.tabPanel.ResumeLayout(false);
            this.tabPageBrowser.ResumeLayout(false);
            this.tabPageBrowser.PerformLayout();
            this.tabPageNotification.ResumeLayout(false);
            this.tabPageNotification.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBrowserCSS;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TextBox textBoxNotificationCSS;
        private System.Windows.Forms.Button btnOpenDevTools;
        private System.Windows.Forms.Timer timerTestBrowser;
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage tabPageBrowser;
        private System.Windows.Forms.TabPage tabPageNotification;
    }
}