namespace TweetDuck.Browser {
    sealed partial class FormBrowser {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.trayIcon = new TrayIcon(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerResize = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerResize
            // 
            this.timerResize.Interval = 500;
            this.timerResize.Tick += new System.EventHandler(this.timerResize_Tick);
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = TweetLib.Core.Features.TweetDeck.TweetDeckBrowser.BackgroundColor;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Font = TweetDuck.Controls.ControlExtensions.DefaultFont;
            this.Icon = Properties.Resources.icon;
            this.Location = TweetDuck.Controls.ControlExtensions.InvisibleLocation;
            this.MinimumSize = new System.Drawing.Size(348, 424);
            this.Name = "FormBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Activated += new System.EventHandler(this.FormBrowser_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBrowser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormBrowser_FormClosed);
            this.ResizeEnd += new System.EventHandler(this.FormBrowser_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.FormBrowser_LocationChanged);
            this.Resize += new System.EventHandler(this.FormBrowser_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private TrayIcon trayIcon;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timerResize;
    }
}

