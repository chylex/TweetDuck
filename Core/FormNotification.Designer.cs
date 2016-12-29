using TweetDck.Core.Controls;

namespace TweetDck.Core {
    partial class FormNotification {
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
            this.panelBrowser = new System.Windows.Forms.Panel();
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.progressBarTimer = new TweetDck.Core.Controls.FlatProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // panelBrowser
            // 
            this.panelBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBrowser.BackColor = System.Drawing.Color.White;
            this.panelBrowser.Location = new System.Drawing.Point(0, 0);
            this.panelBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.panelBrowser.Name = "panelBrowser";
            this.panelBrowser.Size = new System.Drawing.Size(284, 118);
            this.panelBrowser.TabIndex = 0;
            // 
            // timerProgress
            // 
            this.timerProgress.Interval = 16;
            this.timerProgress.Tick += new System.EventHandler(this.timerHideProgress_Tick);
            // 
            // progressBarTimer
            // 
            this.progressBarTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTimer.BackColor = System.Drawing.SystemColors.Control;
            this.progressBarTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(99)))), ((int)(((byte)(153)))));
            this.progressBarTimer.Location = new System.Drawing.Point(0, 118);
            this.progressBarTimer.Margin = new System.Windows.Forms.Padding(0);
            this.progressBarTimer.Maximum = 1000;
            this.progressBarTimer.Name = "progressBarTimer";
            this.progressBarTimer.Size = new System.Drawing.Size(284, 4);
            this.progressBarTimer.TabIndex = 1;
            // 
            // FormNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 122);
            this.Controls.Add(this.progressBarTimer);
            this.Controls.Add(this.panelBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = TweetDck.Core.Controls.ControlExtensions.InvisibleLocation;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNotification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotification_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBrowser;
        private Controls.FlatProgressBar progressBarTimer;
        private System.Windows.Forms.Timer timerProgress;
        private System.Windows.Forms.ToolTip toolTip;
    }
}