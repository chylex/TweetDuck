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
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = global::TweetDck.Properties.Resources.icon;
            this.trayIcon.Click += new System.EventHandler(this.trayIcon_Click);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
    }
}

