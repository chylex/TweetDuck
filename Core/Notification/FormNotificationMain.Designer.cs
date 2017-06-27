namespace TweetDuck.Core.Notification {
    partial class FormNotificationMain {
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
            this.timerDisplayDelay = new System.Windows.Forms.Timer(this.components);
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.progressBarTimer = new TweetDuck.Core.Controls.FlatProgressBar();
            this.SuspendLayout();
            // 
            // timerDisplayDelay
            // 
            this.timerDisplayDelay.Interval = 17;
            this.timerDisplayDelay.Tick += new System.EventHandler(this.timerDisplayDelay_Tick);
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
            this.progressBarTimer.Size = new System.Drawing.Size(284, TimerBarHeight);
            this.progressBarTimer.TabIndex = 1;
            // 
            // FormNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 122);
            this.Controls.Add(this.progressBarTimer);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotification_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerDisplayDelay;
        protected System.Windows.Forms.Timer timerProgress;
        private Controls.FlatProgressBar progressBarTimer;
    }
}