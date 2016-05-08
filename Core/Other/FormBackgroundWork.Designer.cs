using TweetDck.Core.Controls;

namespace TweetDck.Core.Other {
    partial class FormBackgroundWork {
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
            this.progressBarUseless = new System.Windows.Forms.ProgressBar();
            this.labelDescription = new TweetDck.Core.Controls.RichTextLabel();
            this.SuspendLayout();
            // 
            // progressBarUseless
            // 
            this.progressBarUseless.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarUseless.Location = new System.Drawing.Point(12, 49);
            this.progressBarUseless.MarqueeAnimationSpeed = 10;
            this.progressBarUseless.Name = "progressBarUseless";
            this.progressBarUseless.Size = new System.Drawing.Size(480, 23);
            this.progressBarUseless.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarUseless.TabIndex = 0;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelDescription.Location = new System.Drawing.Point(12, 12);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.ReadOnly = true;
            this.labelDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.labelDescription.Size = new System.Drawing.Size(480, 31);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.TabStop = false;
            this.labelDescription.Text = "";
            // 
            // FormBackgroundWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 84);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.progressBarUseless);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormBackgroundWork";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TweetDeck Migration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarUseless;
        private RichTextLabel labelDescription;
    }
}