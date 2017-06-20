namespace TweetDuck.Core.Other {
    sealed partial class FormSettings {
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
            this.btnClose = new System.Windows.Forms.Button();
            this.labelTip = new System.Windows.Forms.Label();
            this.panelContents = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.Location = new System.Drawing.Point(573, 349);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnClose.Size = new System.Drawing.Size(49, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelTip
            // 
            this.labelTip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTip.AutoSize = true;
            this.labelTip.Location = new System.Drawing.Point(12, 351);
            this.labelTip.Name = "labelTip";
            this.labelTip.Size = new System.Drawing.Size(310, 13);
            this.labelTip.TabIndex = 2;
            this.labelTip.Text = "Tip: Move your cursor over an option to see detailed explanation";
            // 
            // panelContents
            // 
            this.panelContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContents.AutoScroll = true;
            this.panelContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContents.Location = new System.Drawing.Point(135, 12);
            this.panelContents.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panelContents.Name = "panelContents";
            this.panelContents.Size = new System.Drawing.Size(487, 331);
            this.panelContents.TabIndex = 1;
            // 
            // panelButtons
            // 
            this.panelButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelButtons.Location = new System.Drawing.Point(12, 12);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(124, 331);
            this.panelButtons.TabIndex = 0;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 384);
            this.Controls.Add(this.panelContents);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.labelTip);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::TweetDuck.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelTip;
        private System.Windows.Forms.Panel panelContents;
        private System.Windows.Forms.Panel panelButtons;
    }
}