namespace TweetDick.Core.Other {
    partial class FormAbout {
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
            this.labelAbout = new TweetDick.Core.Controls.RichTextLabel();
            this.SuspendLayout();
            // 
            // labelAbout
            // 
            this.labelAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAbout.BackColor = System.Drawing.Color.White;
            this.labelAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelAbout.Location = new System.Drawing.Point(12, 12);
            this.labelAbout.Name = "labelAbout";
            this.labelAbout.ReadOnly = true;
            this.labelAbout.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.labelAbout.Size = new System.Drawing.Size(360, 126);
            this.labelAbout.TabIndex = 0;
            this.labelAbout.TabStop = false;
            this.labelAbout.Text = "";
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 150);
            this.Controls.Add(this.labelAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About TweetDick";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.RichTextLabel labelAbout;
    }
}