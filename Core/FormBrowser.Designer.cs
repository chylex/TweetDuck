namespace TweetDck.Core {
    sealed partial class FormBrowser {
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
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.muteNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.contextMenuTray;
            this.trayIcon.Icon = global::TweetDck.Properties.Resources.icon;
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            // 
            // contextMenuTray
            // 
            this.contextMenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.muteNotificationsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.contextMenuTray.Name = "contextMenuTray";
            this.contextMenuTray.ShowCheckMargin = true;
            this.contextMenuTray.ShowImageMargin = false;
            this.contextMenuTray.ShowItemToolTips = false;
            this.contextMenuTray.Size = new System.Drawing.Size(174, 92);
            this.contextMenuTray.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuTray_Opening);
            this.contextMenuTray.Opened += new System.EventHandler(this.contextMenuTray_Opened);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // muteNotificationsToolStripMenuItem
            // 
            this.muteNotificationsToolStripMenuItem.CheckOnClick = true;
            this.muteNotificationsToolStripMenuItem.Name = "muteNotificationsToolStripMenuItem";
            this.muteNotificationsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.muteNotificationsToolStripMenuItem.Text = "Mute Notifications";
            this.muteNotificationsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.muteNotificationsToolStripMenuItem_CheckedChanged);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Icon = global::TweetDck.Properties.Resources.icon;
            this.Location = new System.Drawing.Point(-32000, -32000);
            this.Name = "FormBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResizeEnd += new System.EventHandler(this.FormBrowser_ResizeEnd);
            this.Resize += new System.EventHandler(this.FormBrowser_Resize);
            this.contextMenuTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuTray;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem muteNotificationsToolStripMenuItem;
    }
}

