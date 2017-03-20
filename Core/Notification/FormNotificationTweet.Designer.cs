using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetDck.Core.Notification {
    partial class FormNotificationTweet {
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
            this.timerCursorCheck = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerCursorCheck
            // 
            this.timerCursorCheck.Interval = 200;
            this.timerCursorCheck.Tick += new System.EventHandler(this.timerCursorCheck_Tick);
            // 
            // FormNotificationTweet
            // 
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotificationTweet_FormClosing);
            this.ResumeLayout(true);

        }

        #endregion

        private System.Windows.Forms.Timer timerCursorCheck;
    }
}